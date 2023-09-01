using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace SimpleWSA.WSALibrary
{
  public class WSACoreTests
  {
    private IConfiguration configuration;

    [SetUp]
    public async Task Setup()
    {
      var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
      configuration = builder.Build();

      Session session = new Session(configuration["Login"],
                                    configuration["Password"],
                                    34,
                                    "244",
                                    configuration["Domain"],
                                    null);
      await session.CreateByConnectionProviderAddressAsync("https://connectionprovider.naiton.com");
    }

    #region non query
    [Test]
    public void GetOutBigint()
    {
      WSALibrary.Command command = new WSALibrary.Command("migration.get_out_bigint");
      command.Parameters.Add("p_parameter", PgsqlDbType.Bigint);
      var response = Command.Execute(command, RoutineType.NonQuery);

      // {\"migration.get_out_bigint\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":9223372036854775807}}}
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_bigint"]["arguments"]["p_parameter"];
      long acrtual = Convert.ToInt64(p);
      long expected = 9223372036854775807;
      Assert.That(acrtual, Is.EqualTo(expected));
    }
    #endregion non query

    // https://www.sean-lloyd.com/post/hash-a-string/
    internal static string HashString(string text, string salt = "")
    {
      if (string.IsNullOrEmpty(text))
      {
        return string.Empty;
      }

      // Uses SHA256 to create the hash
      using (var sha = new System.Security.Cryptography.SHA256Managed())
      {
        // Convert the string to a byte array first, to be processed
        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + salt);
        byte[] hashBytes = sha.ComputeHash(textBytes);

        // Convert back to a string, removing the '-' that BitConverter adds
        string hash = BitConverter
            .ToString(hashBytes)
            .Replace("-", string.Empty);

        return hash;
      }
    }
  }
}