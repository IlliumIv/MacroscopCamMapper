using System.Globalization;
using System.Text;

namespace MacroscopCamMapper.CommandLineArguments;

public static class Parameters
{
    public static Parameter ShowHelpMessage { get; } =
        new(prefixes: ["--help", "-h", "-?"],
            format: string.Empty,
            description: "Show this message and exit.",
            sortingOrder: 11,
            parser: (args) =>
            {
                ArgumentsHandler.ShowHelp();
                Environment.Exit(0);
                return args[1..];
            });

    public static Parameter ShowEncodings { get; } =
        new(prefixes: ["--show-encodings"],
            format: string.Empty,
            description: "Show all possible encodings and exit.",
            sortingOrder: 9,
            parser: (args) =>
            {
                Console.WriteLine(string.Join(", ", Encoding
                    .GetEncodings()
                    .Select(e => new[] { e.Name })
                    .SelectMany(e => e)));
                Environment.Exit(0);
                return args[1..];
            });

    public static Parameter ShowCultures { get; } =
        new(prefixes: ["--show-cultures"],
            format: string.Empty,
            description: "Show all possible cultures and exit.",
            sortingOrder: 9,
            parser: (args) =>
            {
                Console.WriteLine(string.Join(", ", CultureInfo
                    .GetCultures(CultureTypes.AllCultures)
                    .Select(c => new[] { c.Name })
                    .Skip(1).SelectMany(c => c)));
                Environment.Exit(0);
                return args[1..];
            });

    public static Parameter Export { get; } =
        new(prefixes: ["--export"],
            description: "Export to file and exit. Overwrite file if it exists.",
            format: "path",
            sortingOrder: 10,
            parser: (args) =>
            {
                ConfigurationEntities.Configuration.Export(args[1]);
                Environment.Exit(0);
                return args[1..];
            });

    public static Parameter<bool> ShowVerbose { get; } =
        new(prefixes: ["--verbose", "-v"],
            value: false,
            format: string.Empty,
            description: "Show verbose output.",
            sortingOrder: 9,
            parser: (args) =>
            {
                if (ShowVerbose is not null)
                    ShowVerbose.Value = true;
                return args[1..];
            });

    public static Parameter<string> Address { get; } =
        new(prefixes: ["--server", "-s"],
            value: "127.0.0.1",
            format: "url",
            description: "Server address. Default value is 127.0.0.1.",
            parser: (args) =>
            {
                if (Address is not null)
                    Address.Value = args[1];
                return args[2..];
            });

    public static Parameter<ushort> Port { get; } =
        new(prefixes: ["--port", "-p"],
            value: 8080,
            format: "number",
            description: "Server port. Default value is 8080.",
            parser: (args) =>
            {
                if (Port is not null)
                    Port.Value = ushort.Parse(args[1]);
                return args[2..];
            });

    public static Parameter<bool> UseSSL { get; } =
        new(prefixes: ["--ssl"],
            value: false,
            format: string.Empty,
            description: "Connect over HTTPS.",
            parser: (args) =>
            {
                if (UseSSL is not null)
                    UseSSL.Value = true;
                return args[1..];
            });

    public static Parameter<bool> IsActiveDirectoryUser { get; } =
        new(prefixes: ["--active-directory", "-ad"],
            value: false,
            format: string.Empty,
            description: "Specify that is Active Directory user.",
            sortingOrder: 2,
            parser: (args) =>
            {
                if (IsActiveDirectoryUser is not null)
                    IsActiveDirectoryUser.Value = true;
                return args[1..];
            });

    public static Parameter<string> Login { get; } =
        new(prefixes: ["--login", "-l"],
            value: "root",
            format: "string",
            description: $"Login. Default value is \"root\". " +
                $"Must specify {string.Join(" or ", IsActiveDirectoryUser.Prefixes)} if using a Active Directory user.",
            parser: (args) =>
            {
                if (Login is not null)
                    Login.Value = args[1];
                return args[2..];
            });

    public static Parameter<string> Password { get; } =
        new(prefixes: ["--password"],
            value: string.Empty,
            format: "string",
            description: "Password. Default value is empty string.",
            sortingOrder: 3,
            parser: (args) =>
            {
                if (Password is not null)
                    Password.Value = args[1];
                return args[2..];
            });

    public static Parameter<string> Column_CameraName { get; } =
        new(prefixes: ["--names"],
            value: "Имя камеры",
            format: "string",
            description: "Column header contains names of cameras. Default value is \"Имя камеры\".",
            sortingOrder: 6,
            parser: (args) =>
            {
                if (Column_CameraName is not null)
                    Column_CameraName.Value = args[1];
                return args[2..];
            });

    public static Parameter<string> Column_ChannelId { get; } =
        new(prefixes: ["--channel-id"],
            value: "Channel Id",
            format: "string",
            description: "Column header contains ids of channels. Default value is \"Channel Id\".",
            sortingOrder: 6,
            parser: (args) =>
            {
                if (Column_ChannelId is not null)
                    Column_ChannelId.Value = args[1];
                return args[2..];
            });

    public static Parameter<string> Column_Latitude { get; } =
        new(prefixes: ["--latitude"],
            value: "Широта",
            format: "string",
            description: $"Column header contains latitude. Default value is \"Широта\".",
            sortingOrder: 6,
            parser: (args) =>
            {
                if (Column_Latitude is not null)
                    Column_Latitude.Value = args[1];
                return args[2..];
            });

    public static Parameter<string> Column_Longitude { get; } =
        new(prefixes: ["--longitude"],
            value: "Долгота",
            format: "string",
            description: "Column header contains longitude. Default value is \"Долгота\".",
            sortingOrder: 6,
            parser: (args) =>
            {
                if (Column_Longitude is not null)
                    Column_Longitude.Value = args[1];
                return args[2..];
            });

    public static Parameter<string> Column_OnMap { get; } =
        new(prefixes: ["--on-map"],
            value: "Размещена на карте",
            format: "string",
            description: "Column header sets IsOnMap flag. Default value is \"Размещена на карте\". " +
                "Valid values: [true, yes, да]; in any letter case. Any other values either lack of value automatically set IsOnMap flag to false.",
            sortingOrder: 6,
            parser: (args) =>
            {
                if (Column_OnMap is not null)
                    Column_OnMap.Value = args[1];
                return args[2..];
            });

    public static Parameter<Encoding> File_Encoding { get; } =
        new(prefixes: ["--encoding"],
            value: Encoding.UTF8,
            format: "string",
            description: $"File encoding. Default value is UTF-8. " +
                $"To see all possible encodings specify {string.Join(", ", ShowEncodings.Prefixes)}.",
            sortingOrder: 7,
            parser: (args) =>
            {
                if (File_Encoding is not null)
                    File_Encoding.Value = Encoding.GetEncoding(args[1]);
                return args[2..];
            });

    public static Parameter<CultureInfo> File_Culture { get; } =
        new(prefixes: ["--culture"],
            value: CultureInfo.InvariantCulture,
            format: "string",
            description: $"File culture. Default value is InvariantCulture. " +
                $"To see all possible cultures specify {string.Join(", ", ShowCultures.Prefixes)}.",
            sortingOrder: 7,
            parser: (args) =>
            {
                if (File_Culture is not null)
                    File_Culture.Value = new CultureInfo(args[1]);
                return args[2..];
            });

    public static Parameter<string> Column_Delimeter { get; } =
        new(prefixes: ["--delimeter"],
            value: ",",
            format: "string",
            description: $"Columns delimeter." +
                $" Default value is \"{File_Culture.Value.TextInfo.ListSeparator}\"." +
                $" It depends on culture, current selected culture is {File_Culture.Value.DisplayName}.",
            sortingOrder: 6,
            parser: (args) =>
            {
                if (Column_Delimeter is not null)
                    Column_Delimeter.Value = args[1];
                return args[2..];
            });
}