using SimpleWSA;
using System;
using System.Threading.Tasks;

namespace ConsoleApp1
{
  class Program
  {
    static async Task Main(string[] args)
    {
      Session session = new Session("sadmin@naitonrunner.nl",
                                    "Limpopo000!",
                                    34,
                                    "244",
                                    "upstairstest",
                                    null);

      await session.CreateByRestServiceAddressAsync("http://webservice.nsa.naiton.com");

      Console.WriteLine($"token: {SessionContext.GetContext().Token}");

      Console.ReadLine();
    }
  }
}
