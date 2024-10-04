using System.Reflection;

namespace MacroscopCamMapper.CommandLineArguments;

public static class ArgumentsHandler
{
    private static string _usageDescription = string.Empty;
    private static string _footerDescription = string.Empty;

    public static string[] Parse(string[] args, string usageDescription = "", string footerDescription = "")
    {
        if (string.IsNullOrEmpty(_usageDescription) && !string.IsNullOrEmpty(usageDescription))
            _usageDescription = usageDescription;

        if (string.IsNullOrEmpty(_footerDescription) && !string.IsNullOrEmpty(footerDescription))
            _footerDescription = footerDescription;

        if (args.Length == 0)
        {
            ShowHelp();
            Environment.Exit(0);
        }

        var parameters = typeof(Parameters)
             .GetFields(BindingFlags.NonPublic | BindingFlags.Static)
             .Select(f => f.GetValue(null) as Parameter).OrderBy(p => p?.SortingOrder).ToArray();

        try
        {
            foreach (var p in parameters)
            {
                foreach(var a in args)
                {
                    if (p is not null && p.Prefixes.Any(p => p == a))
                    {
                        args = p.ParseArgs(args);
                    }
                }
            }
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
            Environment.Exit(1);
        }
        catch (IndexOutOfRangeException)
        {
            Console.WriteLine($"Invalid value for parameter: {args[0]}.");
            Environment.Exit(1);
        }

        return args;
    }

    public static void ShowHelp()
    {
        var description = _usageDescription;
        var indent = description.Length;

        var parameters = typeof(Parameters)
             .GetFields(BindingFlags.NonPublic | BindingFlags.Static)
             .Select(f => f.GetValue(null) as Parameter).OrderBy(p => p?.SortingOrder).ToArray();

        for (var i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];

            if (p == null)
                continue;

            var paramDesc = $"{(p.IsRequired ? "" : "[")}" +
                $"{string.Join(", ", p.Prefixes)}" +
                $"{(p.Format == string.Empty ? "" : $" <{p.Format}>")}" +
                $"{(p.IsRequired ? "" : "]")}";

            description = $"{description} {paramDesc}";

            if (description.Length > indent + 55)
            {
                Console.WriteLine(description);
                description = $"{"".PadLeft(indent)}";
                continue;
            }

            if (i == parameters.Length - 1)
                Console.WriteLine(description);
        }

        Console.WriteLine();

        foreach (var param in parameters)
        {
            if (param is null)
                continue;

            Console.WriteLine(string.Format(
                $" {{0,{-(parameters.Max(p => p is null ? 0 : (string.Join(", ", p.Prefixes) + p.Format)
                    .Length) + 5)}}}{{1}}", $"{string.Join(", ", param.Prefixes)}" +
                $"{(param.Format == string.Empty ? "" : $" <{param.Format}>")}", param.Description()));
        }

        if (!string.IsNullOrEmpty(_footerDescription))
            Console.WriteLine($"\n{_footerDescription}");
    }
}
