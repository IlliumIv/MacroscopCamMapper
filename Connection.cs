using System.Globalization;
using System.Security.Authentication;
using System.Text;

namespace MacroscopCamMapper;

public class Connection
{
    public static bool SendRequest(HttpRequestMessage message, out HttpResponseMessage response)
    {
        var macroscopClient = Parameters.UseSSL.Value switch
        {
            true => new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                SslProtocols = SslProtocols.Tls12
            }),
            _ => new HttpClient(),
        };

        macroscopClient.BaseAddress = new Uri($"{(Parameters.UseSSL.Value ? "https" : "http")}://{Parameters.Address.Value}:{Parameters.Port.Value}");
        var authString = $"{Parameters.Login.Value}:{CreateMD5(Parameters.Password.Value)}";
        message.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes(authString))}");

        response = macroscopClient.Send(message);

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

