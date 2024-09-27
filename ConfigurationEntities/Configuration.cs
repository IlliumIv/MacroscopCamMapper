using CsvHelper;
using Newtonsoft.Json;
using System.Globalization;

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
            var numberFormat = Parameters.File_Culture.Value.NumberFormat;

            using var writer = File.AppendText(path);
            using var csv = new CsvWriter(writer, Parameters.File_Culture.Value);

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
