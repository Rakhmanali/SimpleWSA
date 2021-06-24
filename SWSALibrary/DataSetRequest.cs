using System.Collections.Generic;
using System.Net;
using SimpleWSA.Services;

namespace SimpleWSA
{
  public sealed class DataSetRequest : Request
  {
    public DataSetRequest(string serviceAddress,
                          string route,
                          string token,
                          Command command,
                          Dictionary<string, string> errorCodes,
                          IConvertingService convertingService,
                          ICompressionService compressionService,
                          WebProxy webProxy) : base(serviceAddress,
                                                   route,
                                                   token,
                                                   command,
                                                   errorCodes,
                                                   convertingService,
                                                   compressionService,
                                                   webProxy)
    { }

    static DataSetRequest()
    {
      getFormat = "{0}{1}executereturnset?token={2}&value={3}";
      postFormat = "{0}{1}executereturnsetpost?token={2}&compression={3}";
    }

    public static string PostFormat
    {
      get
      {
        return postFormat;
      }
    }
  }
}
