using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using MacroscopCamMapper.CommandLineArguments;
using System.Globalization;

namespace MacroscopCamMapper;

public class Camera
{
    public string? Name { get; set; }
    public string? ChannelId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? IsOnMap { get; set; }

    public bool GetIsOnMapFlag() => IsOnMap?.ToLower() switch
    {
        "true" or "yes" or "да" => true,
        _ => false,
    };
}

public class CameraMap : ClassMap<Camera>
{
    public CameraMap()
    {
        Map(m => m.Name).Name(Parameters.Column_CameraName.Value);
        Map(m => m.ChannelId).Name(Parameters.Column_ChannelId.Value);
        Map(m => m.Latitude).Name(Parameters.Column_Latitude.Value).Validate(a => IsValidLatitude(a.Field));
        Map(m => m.Longitude).Name(Parameters.Column_Longitude.Value).Validate(a => IsValidLongitude(a.Field));
        Map(m => m.IsOnMap).Name(Parameters.Column_OnMap.Value);
    }

    private static bool IsValidLatitude(string d) =>
        double.TryParse(d, NumberStyles.AllowDecimalPoint, Parameters.File_Culture.Value, out var l)
            && IsValidCoordinatePoint(l, -90, 90);

    private static bool IsValidLongitude(string d) =>
        double.TryParse(d, NumberStyles.AllowDecimalPoint, Parameters.File_Culture.Value, out var l)
            && IsValidCoordinatePoint(l, -180, 180);

    private static bool IsValidCoordinatePoint(double d, double min, double max) => d >= min && d <= max;
}
