using Microsoft.Extensions.Configuration;
using SimpleWSA.WSALibrary;

var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder().AddJsonFile("appsettings.json");
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