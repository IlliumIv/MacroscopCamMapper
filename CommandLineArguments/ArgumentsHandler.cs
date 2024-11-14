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
                for (var i = 0; i < args.Length; i++)
                {
                    if (p is not null && p.Prefixes.Any(p => p == args[i]))
                    {
                        args = p.ParseArgs(args, i);
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

    // https://stackoverflow.com/a/457501/21231652
    public static T[] RemoveAt<T>(this T[] source, int index, int skip)
    {
        T[] dest = new T[source.Length - skip];
        if (index > 0)
            Array.Copy(source, 0, dest, 0, index);

        if (index < source.Length - skip)
            Array.Copy(source, index + skip, dest, index, source.Length - index - skip);

        return dest;
    }
}
