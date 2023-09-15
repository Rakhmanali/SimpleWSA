using Microsoft.Extensions.Configuration;
using SimpleWSA.WSALibrary;
using System;
using System.Threading.Tasks;

namespace ConsoleAppNet48
{
  internal class Program
  {
    static async Task Main(string[] args)
    {
      var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
      IConfiguration configuration = builder.Build();

      Session session = new Session(configuration["Login"],
                                    configuration["Password"],
                                    34,
                                    "244",
                                    configuration["Domain"],
                                    null);
      await session.CreateByConnectionProviderAddressAsync("https://connectionprovider.naiton.com");

      Console.WriteLine(SessionContext.GetContext().Token);

      Console.WriteLine("press any key ...");
      Console.ReadLine();
    }
  }
}
