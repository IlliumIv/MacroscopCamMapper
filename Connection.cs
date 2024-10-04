using System.Globalization;
using System.Security.Authentication;
using System.Text;
using MacroscopCamMapper.CommandLineArguments;

namespace MacroscopCamMapper;

public class Connection
{
    private static HttpClient? _macroscopClient;

    public static bool SendRequest(HttpRequestMessage message, out HttpResponseMessage response)
    {
        _macroscopClient ??= Parameters.UseSSL.Value switch
        {
            true => new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                SslProtocols = SslProtocols.Tls12
            })
            {
                BaseAddress = new Uri($"https://{Parameters.Address.Value}:{Parameters.Port.Value}")
            },
            _ => new HttpClient()
            {
                BaseAddress = new Uri($"http://{Parameters.Address.Value}:{Parameters.Port.Value}")
            },
        };

        var authString = $"{Parameters.Login.Value}:" +
            $"{(Parameters.IsActiveDirectoryUser.Value ? Parameters.Password.Value : CreateMD5(Parameters.Password.Value))}";
        message.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes(authString))}");
        response = _macroscopClient.Send(message);

        try
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Error: " + response.StatusCode.ToString());
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
        }
        catch (HttpRequestException e) when (e.InnerException is IOException)
        {
            Console.WriteLine($"{e.Message} {e.InnerException.Message}");
        }

        return false;
    }

    private static string CreateMD5(string input)
    {
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var hashBytes = System.Security.Cryptography.MD5.HashData(inputBytes);
        var stringBuilder = new StringBuilder();

        for (var i = 0; i < hashBytes.Length; i++)
            stringBuilder.Append(hashBytes[i].ToString("X2", CultureInfo.CurrentCulture));

        return stringBuilder.ToString();
    }
}
