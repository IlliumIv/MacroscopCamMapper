using CsvHelper;
using CsvHelper.Configuration;
using MacroscopCamMapper.CommandLineArguments;
using Newtonsoft.Json;

namespace MacroscopCamMapper.ConfigurationEntities;

public class Configuration
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private static dynamic JsonBody { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
   
    public HashSet<Channel> Channels { get; } = [];

    public Configuration(string jsonString)
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        JsonBody = JsonConvert.DeserializeObject<dynamic>(jsonString);
#pragma warning restore CS8601 // Possible null reference assignment.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        foreach (var channel in JsonBody) Channels.Add(new Channel(channel));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }

    private Configuration() { }

    public static void Export(string path)
    {
        if (GetConfiguration(out var configuration))
        {
            var csvConfig = new CsvConfiguration(Parameters.File_Culture.Value)
            {
                Delimiter = Parameters.Column_Delimeter.Value,
                HeaderValidated = null,
                MissingFieldFound = null,
            };

            using var writer = new StreamWriter(path, false, Parameters.File_Encoding.Value);
            using var csv = new CsvWriter(writer, csvConfig);
            csv.Context.RegisterClassMap<CameraMap>();

            var channels = configuration.Channels.Select(c => new Camera() {
                Name = c.Name, ChannelId = c.Id, IsOnMap = $"{c.MapSettings.IsOnMap}",
                Latitude = c.MapSettings.Latitude, Longitude = c.MapSettings.Longitude,
            });

            csv.WriteRecords(channels);
        }
    }

    public static bool GetConfiguration(out Configuration configuration)
    {
        configuration = new();

        if (Connection.SendRequest(new HttpRequestMessage(HttpMethod.Get, "configure/channels"), out var response))
        {
            configuration = new Configuration(response.Content.ReadAsStringAsync().Result);
            return true;
        }

        return false;
    }
}
