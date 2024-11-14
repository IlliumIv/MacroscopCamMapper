using System.Globalization;
using System.Text;

namespace MacroscopCamMapper.CommandLineArguments;

public static class Parameters
{
    public static Parameter ShowHelpMessage { get; } =
        new(prefixes: ["--help", "-h", "-?"],
            format: string.Empty,
            descriptionFormatter: () => "Show this message and exit.",
            sortingOrder: 11,
            parser: (args, i) =>
            {
                ArgumentsHandler.ShowHelp();
                Environment.Exit(0);
                return args.RemoveAt(i, 1);
            });

    public static Parameter ShowEncodings { get; } =
        new(prefixes: ["--show-encodings"],
            format: string.Empty,
            descriptionFormatter: () => "Show all possible encodings and exit.",
            sortingOrder: 9,
            parser: (args, i) =>
            {
                Console.WriteLine(string.Join(", ", Encoding
                    .GetEncodings()
                    .Select(e => new[] { e.Name })
                    .SelectMany(e => e)));
                Environment.Exit(0);
                return args.RemoveAt(i, 1);
            });

    public static Parameter ShowCultures { get; } =
        new(prefixes: ["--show-cultures"],
            format: string.Empty,
            descriptionFormatter: () => "Show all possible cultures and exit.",
            sortingOrder: 9,
            parser: (args, i) =>
            {
                Console.WriteLine(string.Join(", ", CultureInfo
                    .GetCultures(CultureTypes.AllCultures)
                    .Select(c => new[] { c.Name })
                    .Skip(1).SelectMany(c => c)));
                Environment.Exit(0);
                return args.RemoveAt(i, 1);
            });

    public static Parameter Export { get; } =
        new(prefixes: ["--export"],
            descriptionFormatter: () => "Export to file and exit. Overwrite file if it exists.",
            format: "path",
            sortingOrder: 10,
            parser: (args, i) =>
            {
                ConfigurationEntities.Configuration.Export(args[i + 1]);
                Environment.Exit(0);
                return args.RemoveAt(i, 2);
            });

    public static Parameter<bool> ShowVerbose { get; } =
        new(prefixes: ["--verbose", "-v"],
            value: false,
            format: string.Empty,
            descriptionFormatter: () => "Show verbose output.",
            sortingOrder: 9,
            parser: (args, i) =>
            {
                if (ShowVerbose is not null)
                    ShowVerbose.Value = true;
                return args.RemoveAt(i, 1);
            });

    public static Parameter<string> Address { get; } =
        new(prefixes: ["--server", "-s"],
            value: "127.0.0.1",
            format: "url",
            descriptionFormatter: () => $"Server address. Current value is \"{Address?.Value}\".",
            parser: (args, i) =>
            {
                if (Address is not null)
                    Address.Value = args[i + 1];
                return args.RemoveAt(i, 2);
            });

    public static Parameter<ushort> Port { get; } =
        new(prefixes: ["--port", "-p"],
            value: 8080,
            format: "number",
            descriptionFormatter: () => $"Server port. Current value is \"{Port?.Value}\".",
            parser: (args, i) =>
            {
                if (Port is not null)
                    Port.Value = ushort.Parse(args[i + 1]);
                return args.RemoveAt(i, 2);
            });

    public static Parameter<bool> UseSSL { get; } =
        new(prefixes: ["--ssl"],
            value: false,
            format: string.Empty,
            descriptionFormatter: () => $"Connect over HTTPS. Current value is \"{UseSSL?.Value}\".",
            parser: (args, i) =>
            {
                if (UseSSL is not null)
                    UseSSL.Value = true;
                return args.RemoveAt(i, 1);
            });

    public static Parameter<bool> IsActiveDirectoryUser { get; } =
        new(prefixes: ["--active-directory", "-ad"],
            value: false,
            format: string.Empty,
            descriptionFormatter: () => $"Specify that is Active Directory user. Current value is \"{IsActiveDirectoryUser?.Value}\".",
            sortingOrder: 2,
            parser: (args, i) =>
            {
                if (IsActiveDirectoryUser is not null)
                    IsActiveDirectoryUser.Value = true;
                return args.RemoveAt(i, 1);
            });

    public static Parameter<string> Login { get; } =
        new(prefixes: ["--login", "-l"],
            value: "root",
            format: "string",
            descriptionFormatter: () => $"Login. Current value is \"{Login?.Value}\". " +
                $"Must specify {string.Join(" or ", IsActiveDirectoryUser.Prefixes)} if using a Active Directory user.",
            parser: (args, i) =>
            {
                if (Login is not null)
                    Login.Value = args[i + 1];
                return args.RemoveAt(i, 2);
            });

    public static Parameter<string> Password { get; } =
        new(prefixes: ["--password"],
            value: string.Empty,
            format: "string",
            descriptionFormatter: () => $"Password. Current value is \"{Password?.Value}\".",
            sortingOrder: 3,
            parser: (args, i) =>
            {
                if (Password is not null)
                    Password.Value = args[i + 1];
                return args.RemoveAt(i, 2);
            });

    public static Parameter<string> Column_CameraName { get; } =
        new(prefixes: ["--names"],
            value: "Имя камеры",
            format: "string",
            descriptionFormatter: () => $"Column header contains names of cameras. Current value is \"{Column_CameraName?.Value}\".",
            sortingOrder: 6,
            parser: (args, i) =>
            {
                if (Column_CameraName is not null)
                    Column_CameraName.Value = args[i + 1];
                return args.RemoveAt(i, 2);
            });

    public static Parameter<string> Column_ChannelId { get; } =
        new(prefixes: ["--channel-id"],
            value: "Channel Id",
            format: "string",
            descriptionFormatter: () => $"Column header contains ids of channels. Current value is \"{Column_ChannelId?.Value}\".",
            sortingOrder: 6,
            parser: (args, i) =>
            {
                if (Column_ChannelId is not null)
                    Column_ChannelId.Value = args[i + 1];
                return args.RemoveAt(i, 2);
            });

    public static Parameter<string> Column_Latitude { get; } =
        new(prefixes: ["--latitude"],
            value: "Широта",
            format: "string",
            descriptionFormatter: () => $"Column header contains latitude. Current value is \"{Column_Latitude?.Value}\".",
            sortingOrder: 6,
            parser: (args, i) =>
            {
                if (Column_Latitude is not null)
                    Column_Latitude.Value = args[i + 1];
                return args.RemoveAt(i, 2);
            });

    public static Parameter<string> Column_Longitude { get; } =
        new(prefixes: ["--longitude"],
            value: "Долгота",
            format: "string",
            descriptionFormatter: () => $"Column header contains longitude. Current value is \"{Column_Longitude?.Value}\".",
            sortingOrder: 6,
            parser: (args, i) =>
            {
                if (Column_Longitude is not null)
                    Column_Longitude.Value = args[i + 1];
                return args.RemoveAt(i, 2);
            });

    public static Parameter<string> Column_OnMap { get; } =
        new(prefixes: ["--on-map"],
            value: "Размещена на карте",
            format: "string",
            descriptionFormatter: () => $"Column header sets IsOnMap flag. Current value is \"{Column_OnMap?.Value}\". " +
                "Valid values: [true, yes, да]; in any letter case. Any other values either lack of value automatically set IsOnMap flag to false.",
            sortingOrder: 6,
            parser: (args, i) =>
            {
                if (Column_OnMap is not null)
                    Column_OnMap.Value = args[i + 1];
                return args.RemoveAt(i, 2);
            });

    public static Parameter<Encoding> File_Encoding { get; } =
        new(prefixes: ["--encoding"],
            value: Encoding.UTF8,
            format: "string",
            descriptionFormatter: () => $"File encoding. Current value is \"{File_Encoding?.Value.HeaderName}\". " +
                $"To see all possible encodings specify {string.Join(", ", ShowEncodings.Prefixes)}.",
            sortingOrder: 7,
            parser: (args, i) =>
            {
                if (File_Encoding is not null)
                    File_Encoding.Value = Encoding.GetEncoding(args[i + 1]);
                return args.RemoveAt(i, 2);
            });

    public static Parameter<CultureInfo> File_Culture { get; } =
        new(prefixes: ["--culture"],
            value: CultureInfo.InvariantCulture,
            format: "string",
            descriptionFormatter: () => $"File culture. Current value is \"{File_Culture?.Value.DisplayName} ({File_Culture?.Value.Name})\". " +
                $"To see all possible cultures specify {string.Join(", ", ShowCultures.Prefixes)}.",
            sortingOrder: 7,
            parser: (args, i) =>
            {
                if (File_Culture is not null)
                    File_Culture.Value = new CultureInfo(args[i + 1]);
                return args.RemoveAt(i, 2);
            });

    public static Parameter<string> Column_Delimeter { get; } =
        new(prefixes: ["--delimeter"],
            value: ",",
            format: "string",
            descriptionFormatter: () => $"Columns delimeter." +
                $" Current value is \"{File_Culture.Value.TextInfo.ListSeparator}\"." +
                $" It depends on culture, current selected culture is \"{File_Culture.Value.DisplayName} ({File_Culture?.Value.Name})\".",
            sortingOrder: 6,
            parser: (args, i) =>
            {
                if (Column_Delimeter is not null)
                    Column_Delimeter.Value = args[i + 1];
                return args.RemoveAt(i, 2);
            });
}
