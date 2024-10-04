using Newtonsoft.Json;

namespace MacroscopCamMapper.ConfigurationEntities;

public class ConnectionSettings(string channelId)
{
    private readonly string _channelId = channelId;
    private string? _modelId;

    public string ModelId
    {
        get
        {
            if (string.IsNullOrEmpty(_modelId))
                _modelId = GetModelId();
            return _modelId;
        }
        set => _modelId = value;
    }

    private string GetModelId()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"configure/channels/{_channelId}");
        if (Connection.SendRequest(request, out var response))
        {
            var channelAsString = response.Content.ReadAsStringAsync().Result;
            var channelAsDynamic = JsonConvert.DeserializeObject<dynamic>(channelAsString);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return (string)channelAsDynamic["ConnectionSettings"]["ModelId"];
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        return string.Empty;
    }
}
