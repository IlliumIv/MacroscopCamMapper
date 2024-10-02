using System.Globalization;
using System.Reflection;
using System.Text;

namespace MacroscopCamMapper;

public static class Parameters
{
    public static Parameter ShowHelpMessage { get; } =
        new(prefixes: ["--help", "-h", "-?"], format: string.Empty,
            description: "Show this message and exit.");

    public static Parameter ShowEncodings { get; } =
        new(prefixes: ["--show-encodings"], format: string.Empty,
            description: "Show all possible encodings and exit.");

    public static Parameter ShowCultures { get; } =
        new(prefixes: ["--show-cultures"], format: string.Empty,
            description: "Show all possible cultures and exit.");

    public static Parameter Export { get; } =
        new(prefixes: ["--export"], format: "path",
            description: "Export to file and exit.");

    public static Parameter<bool> ShowVerbose { get; } =
        new(prefixes: ["--verbose", "-v"], value: false, format: string.Empty,
            description: "Show verbose output.");

    public static Parameter<string> Address { get; } =
        new(prefixes: ["--server", "-s"], value: "127.0.0.1", format: "url",
            description: "Server address. Default value is 127.0.0.1.");

    public static Parameter<ushort> Port { get; } =
        new(prefixes: ["--port", "-p"], value: 8080, format: "number",
            description: "Server port. Default value is 8080.");

    public static Parameter<bool> UseSSL { get; } =
        new(prefixes: ["--ssl"], value: false, format: string.Empty,
            description: "Connect over HTTPS.");

    public static Parameter<bool> IsActiveDirectoryUser { get; } =
        new(prefixes: ["--active-directory"], value: false, format: string.Empty,
            description: "Specify that is Active Directory user.");

    public static Parameter<string> Login { get; } =
        new(prefixes: ["--login", "-l"], value: "root", format: "string",
            description: $"Login. Default value is \"root\". " +
            $"Must specify {string.Join(" or ", IsActiveDirectoryUser.Prefixes)} if using a Active Directory user.");

    public static Parameter<string> Password { get; } =
        new(prefixes: ["--password"], value: string.Empty, format: "string",
            description: "Password. Default value is empty string.");

    public static Parameter<string> Column_CameraName { get; } =
        new(prefixes: ["--names"], value: "Имя камеры", format: "string",
            description: "Column header contains names of cameras. Default value is \"Имя камеры\".");

    public static Parameter<string> Column_ChannelId { get; } =
        new(prefixes: ["--channel-id"], value: "Channel Id", format: "string",
            description: "Column header contains ids of channels. Default value is \"Channel Id\".");

    public static Parameter<string> Column_Latitude { get; } =
        new(prefixes: ["--latitude"], value: "Широта", format: "string",
            description: "Column header contains latitude. Default value is \"Широта\".");

    public static Parameter<string> Column_Longitude { get; } =
        new(prefixes: ["--longitude"], value: "Долгота", format: "string",
            description: "Column header contains longitude. Default value is \"Долгота\".");

    public static Parameter<string> Column_OnMap { get; } =
        new(prefixes: ["--on-map"], value: "Размещена на карте", format: "string",
            description: "Column header sets IsOnMap flag. Default value is \"Размещена на карте\". " +
            "Valid values: [true, yes, да]; in any letter case. Any other values either lack of value automatically set IsOnMap flag to false.");

    public static Parameter<Encoding> File_Encoding { get; } =
        new(prefixes: ["--encoding"], value: Encoding.UTF8, format: "string",
            description: $"File encoding. Default value is UTF-8. " +
            $"To see all possible encodings specify {string.Join(", ", ShowEncodings.Prefixes)}.");

    public static Parameter<CultureInfo> File_Culture { get; } =
        new(prefixes: ["--culture"], value: CultureInfo.InvariantCulture, format: "string",
            description: $"File culture. Default value is InvariantCulture. " +
            $"To see all possible cultures specify {string.Join(", ", ShowCultures.Prefixes)}.");

    public static Parameter<string> Column_Delimeter { get; } =
        new(prefixes: ["--delimeter"], value: ",", format: "string",
            description: $"Columns delimeter." +
            $" Default value is \"{File_Culture.Value.TextInfo.ListSeparator}\"." +
            $" It depends on culture, current selected culture is {File_Culture.Value.DisplayName}.");

    public static HashSet<string> Parse(string[] args)
    {
        if (args.Length == 0)
        {
            ShowHelp();
            Environment.Exit(0);
        }

        var paths = new HashSet<string>();

        for (var i = 0; i < args.Length; i++)
        {
            if ((args[i].Length == 2 && !args[i].Contains(':')) || args[i].StartsWith("--"))
            {
                var arg = args[i][1..];
                try
                {
                    if (ShowHelpMessage.Prefixes.Any(p => p[1..] == arg))
                    {
                        ShowHelp();
                        Environment.Exit(0);
                    }

                    if (ShowEncodings.Prefixes.Any(p => p[1..] == arg))
                    {
                        Console.WriteLine(string.Join(", ", Encoding
                            .GetEncodings()
                            .Select(e => new[] { e.Name })
                            .SelectMany(e => e)));
                        Environment.Exit(0);
                    }

                    if (ShowCultures.Prefixes.Any(p => p[1..] == arg))
                    {
                        Console.WriteLine(string.Join(", ", CultureInfo
                            .GetCultures(CultureTypes.AllCultures)
                            .Select(c => new[] { c.Name })
                            .Skip(1).SelectMany(c => c)));
                        Environment.Exit(0);
                    }

                    if (Export.Prefixes.Any(p => p[1..] == arg))
                    {
                        ConfigurationEntities.Configuration.Export(args[i + 1]);
                        Environment.Exit(0);
                    }

                    if (ShowVerbose.Prefixes.Any(p => p[1..] == arg))
                    {
                        ShowVerbose.Value = true;
                        continue;
                    }

                    if (Address.Prefixes.Any(p => p[1..] == arg))
                    {
                        Address.Value = args[i + 1];
                        i++;
                        continue;
                    }

                    if (Port.Prefixes.Any(p => p[1..] == arg))
                    {
                        Port.Value = ushort.Parse(args[i + 1]);
                        i++;
                        continue;
                    }

                    if (UseSSL.Prefixes.Any(p => p[1..] == arg))
                    {
                        UseSSL.Value = true;
                        continue;
                    }

                    if (IsActiveDirectoryUser.Prefixes.Any(p => p[1..] == arg))
                    {
                        IsActiveDirectoryUser.Value = true;
                        continue;
                    }

                    if (Login.Prefixes.Any(p => p[1..] == arg))
                    {
                        Login.Value = args[i + 1];
                        i++;
                        continue;
                    }

                    if (Password.Prefixes.Any(p => p[1..] == arg))
                    {
                        Password.Value = args[i + 1];
                        i++;
                        continue;
                    }

                    if (Column_CameraName.Prefixes.Any(p => p[1..] == arg))
                    {
                        Column_CameraName.Value = args[i + 1];
                        i++;
                        continue;
                    }

                    if (Column_ChannelId.Prefixes.Any(p => p[1..] == arg))
                    {
                        Column_ChannelId.Value = args[i + 1];
                        i++;
                        continue;
                    }

                    if (Column_Latitude.Prefixes.Any(p => p[1..] == arg))
                    {
                        Column_Latitude.Value = args[i + 1];
                        i++;
                        continue;
                    }

                    if (Column_Longitude.Prefixes.Any(p => p[1..] == arg))
                    {
                        Column_Longitude.Value = args[i + 1];
                        i++;
                        continue;
                    }

                    if (Column_OnMap.Prefixes.Any(p => p[1..] == arg))
                    {
                        Column_OnMap.Value = args[i + 1];
                        i++;
                        continue;
                    }

                    if (Column_Delimeter.Prefixes.Any(p => p[1..] == arg))
                    {
                        Column_Delimeter.Value = args[i + 1];
                        i++;
                        continue;
                    }

                    if (File_Encoding.Prefixes.Any(p => p[1..] == arg))
                    {
                        File_Encoding.Value = Encoding.GetEncoding(args[i + 1]);
                        i++;
                        continue;
                    }

                    if (File_Culture.Prefixes.Any(p => p[1..] == arg))
                    {
                        File_Culture.Value = new CultureInfo(args[i + 1]);
                        i++;
                        continue;
                    }

                    throw new InvalidOperationException(message: $"Invalid input parameter: \"{args[i]}\"");
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(e.Message);
                    Environment.Exit(1);
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine($"Invalid value for parameter: {args[i]}.");
                    Environment.Exit(1);
                }
            }
            else
            {
                var attrs = File.GetAttributes(args[i]);
                if (attrs.HasFlag(FileAttributes.Directory))
                    paths = [.. paths, .. Directory.GetFiles(args[i])];
                else
                    paths.Add(args[i]);
            }
        }

        return paths;
    }

    public static void ShowHelp()
    {
        var description = $"Usage: {nameof(MacroscopCamMapper)} <files>";
        var indent = description.Length;

        var parameters = typeof(Parameters)
             .GetFields(BindingFlags.NonPublic | BindingFlags.Static)
             .Select(f => (Parameter)f.GetValue(null)).ToArray();

        for (var i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            var paramDesc = $"{(parameters[i].IsRequired ? "" : "[")}" +
                $"{string.Join(", ", parameters[i].Prefixes)}" +
                $"{(parameters[i].Format == string.Empty ? "" : $" <{parameters[i].Format}>")}" +
                $"{(parameters[i].IsRequired ? "" : "]")}";

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
            Console.WriteLine(string.Format(
                $" {{0,{-(parameters.Max(p => (string.Join(", ", p.Prefixes) + p.Format).Length) + 5)}}}{{1}}", $"{string.Join(", ", param.Prefixes)}" +
                $"{(param.Format == string.Empty ? "" : $" <{param.Format}>")}", param.Description));
        }
    }
}

public class Parameter(string[] prefixes, string format,
    string description = "", bool isRequired = false)
{
    public string[] Prefixes = prefixes;
    public string Description = description;
    public string Format = format;
    public bool IsRequired = isRequired;
}

public class Parameter<T>(string[] prefixes, T value, string format,
    string description = "", bool isRequired = false)
    : Parameter(prefixes, format, description, isRequired) where T : notnull
{
    public T Value = value;
}