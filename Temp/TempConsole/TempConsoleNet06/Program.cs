using SimpleWSA.WSALibrary;

Console.WriteLine(".Net Core 6.0 console application");

Session session = new Session("huuskes@buddy.nl",
                                    "Gromit12!",
                                    34,
                                    "244",
                                    "naitongps",
                                    null);

await session.CreateByConnectionProviderAddressAsync("https://connectionprovider.naiton.com");

Console.WriteLine($"token: {SessionContext.GetContext().Token}");

Console.ReadLine();