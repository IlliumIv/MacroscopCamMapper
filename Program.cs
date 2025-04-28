using CsvHelper;
using CsvHelper.Configuration;
using MacroscopCamMapper.CommandLineArguments;
using MacroscopCamMapper.ConfigurationEntities;
using Newtonsoft.Json;
using System.Text;

namespace MacroscopCamMapper;

public class Program
{
    static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var nonParams = ArgumentsHandler.Parse(args, $"Usage: {nameof(MacroscopCamMapper)} <files>",
            "See https://github.com/IlliumIv/MacroscopCamMapper/ to check new versions, report bugs and ask for help.");

        var pathes = new HashSet<string>();

        foreach (var path in nonParams)
        {
            var attrs = File.GetAttributes(path);

            if (attrs.HasFlag(FileAttributes.Directory))
                pathes = [.. pathes, .. Directory.GetFiles(path)];
            else
                pathes.Add(path);
        }

        var csvConfig = new CsvConfiguration(Parameters.File_Culture.Value)
        {
            Delimiter = Parameters.Column_Delimeter.Value,
            MissingFieldFound = null,
        };

        foreach (var path in pathes)
        {
            using var reader = new StreamReader(path, Parameters.File_Encoding.Value);
            using var csv = new CsvReader(reader, csvConfig);
            csv.Context.RegisterClassMap<CameraMap>();

            try
            {
                var cameras = csv.GetRecords<Camera>();

                if (Configuration.GetConfiguration(out var configuration))
                {
                    var channelsToModify = new HashSet<Channel>();
                    var camerasIgnored = new HashSet<(Camera Camera, string Reason)>();

                    foreach (var camera in cameras)
                    {                        
                        try
                        {
                            var channels = configuration.Channels.Where(c => c.Id == camera.ChannelId);

                            if (!channels.Any())
                            {
                                channels = configuration.Channels.Where(c => c.Name == camera.Name);
                            }

                            if (!channels.Any())
                            {
                                camerasIgnored.Add((camera, "Not Found"));
                                continue;
                            }

                            if (channels.Count() > 1)
                            {
                                camerasIgnored.Add((camera, "Duplicate "));
                                continue;
                            }

                            var channel = channels.First();

                            channel.MapSettings.Latitude = camera.Latitude;
                            channel.MapSettings.Longitude = camera.Longitude;
                            channel.MapSettings.IsOnMap = camera.GetIsOnMapFlag();

                            channelsToModify.Add(channel);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error on processing {camera.Name} {camera.ChannelId}:");
                            Console.WriteLine(e.StackTrace);
                        }
                    }

                    var request = new HttpRequestMessage(HttpMethod.Put, "configure/channels");
                    request.Content = new StringContent(JsonConvert.SerializeObject(channelsToModify), Encoding.UTF8, "application/json");

                    var success = Connection.SendRequest(request, out var response);

                    if (Parameters.ShowVerbose.Value)
                    {
                        if (success)
                        {
                            Console.WriteLine("These channels has been changed ({0}):{2}{1}{2}", channelsToModify.Count,
                                JsonConvert.SerializeObject(channelsToModify, formatting: Formatting.Indented), Environment.NewLine);
                        }
                        else
                        {
                            Console.WriteLine("These channels not been changed ({0}){2}{1}{2}", channelsToModify.Count,
                                JsonConvert.SerializeObject(channelsToModify, formatting: Formatting.Indented), Environment.NewLine);
                        }
                    }

                    if (camerasIgnored.Count > 0)
                    {
                        Console.WriteLine("These cameras has been ignored ({0}):{2}{1}{2}", camerasIgnored.Count,
                                JsonConvert.SerializeObject(camerasIgnored, formatting: Formatting.Indented), Environment.NewLine);
                    }
                }
            }
            catch (FieldValidationException e)
            {
                var error = e.Message.Split(Environment.NewLine)[0];
                var rawRecord = e?.Context?.Reader?.Parser.RawRecord;
                var row = e?.Context?.Reader?.Parser.Row;

                Console.WriteLine($"Error on parse file. {error}{Environment.NewLine}" +
                    $"  at \"{rawRecord?.TrimEnd('\r', '\n')}\"{Environment.NewLine}" +
                    $"  at {path}:line {row}");
            }
            catch (HeaderValidationException e)
            {
                var error = e.Message.Split(Environment.NewLine)[0];
                var rawRecord = e?.Context?.Reader?.Parser.RawRecord;
                var row = e?.Context?.Reader?.Parser.Row;

                Console.WriteLine($"Error on parse file. {error}{Environment.NewLine}" +
                    $"  at \"{rawRecord?.TrimEnd('\r', '\n')}\"{Environment.NewLine}" +
                    $"  at {path}:line {row}");
            }
        }
    }
}
