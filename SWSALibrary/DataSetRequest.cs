using System.Collections.Generic;
using System.Net;
using SimpleWSA.Services;

namespace SimpleWSA
{
  public sealed class DataSetRequest : Request
  {
    private const string getFormat = "{0}{1}executereturnset?token={2}&value={3}";
    private const string postFormat = "{0}{1}executereturnsetpost?token={2}&compression={3}";

    public DataSetRequest(string serviceAddress,
                          string route,
                          string token,
                          Command command,
                          IConvertingService convertingService,
                          WebProxy webProxy) : base(serviceAddress,
                                                    route,
                                                    token,
                                                    command,
                                                    convertingService,
                                                    webProxy,
                                                    command.HttpMethod == HttpMethod.GET ? getFormat : postFormat)
    { }

    public static string PostFormat
    {
      get
      {
        return postFormat;
      }
    }
  }
}
