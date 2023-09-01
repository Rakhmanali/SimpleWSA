using SimpleWSA.WSALibrary;
using System;
using System.Threading.Tasks;

namespace TempConsole471
{
  internal class Program
  {
    static async Task Main(string[] args)
    {
      Console.WriteLine(".NET Framework 4.7.1 console application");
      Session session = new Session("huuskes@buddy.nl",
                                          "Gromit12!",
                                          34,
                                          "244",
                                          "naitongps",
                                          null);

      await session.CreateByConnectionProviderAddressAsync("https://connectionprovider.naiton.com");

      Console.WriteLine($"token: {SessionContext.GetContext().Token}");

      Console.ReadLine();
    }
  }
}
