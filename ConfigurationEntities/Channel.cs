using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MacroscopCamMapper.ConfigurationEntities;

public class Channel
{
    private readonly JToken _jTokenBody;

#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    public string Id => (string)_jTokenBody["Id"];
    public string Name => (string)_jTokenBody["Name"];
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8603 // Possible null reference return.
    public ConnectionSettings ConnectionSettings { get; set; }
    public MapSettings MapSettings { get; set; }

    public Channel(JToken jToken)
    {
        _jTokenBody = jToken;
        ConnectionSettings = new(Id);

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        MapSettings = new()
        {
            Latitude = ToDouble(_jTokenBody["MapSettings"]["Latitude"]),
            Longitude = ToDouble(_jTokenBody["MapSettings"]["Longitude"]),
            IsOnMap = (bool)_jTokenBody["MapSettings"]["IsOnMap"]
        };
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8604 // Possible null reference argument.
    }

    private static double ToDouble(JToken token)
    {
        try { return (double)token; }
        catch { return 0; }
    }
}