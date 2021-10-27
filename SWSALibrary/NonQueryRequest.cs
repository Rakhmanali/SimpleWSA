using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleWSA.Extensions;
using SimpleWSA.Services;

namespace SimpleWSA
{
  public sealed class NonQueryRequest : Request
  {
    private const string getFormat = "{0}{1}executenonquery?token={2}&value={3}";
    private const string postFormat = "{0}{1}executenonquerypost?token={2}&compression={3}";

    public NonQueryRequest(string serviceAddress,
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
