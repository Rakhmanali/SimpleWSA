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
    public NonQueryRequest(string serviceAddress,
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

    static NonQueryRequest()
    {
      getFormat = "{0}{1}executenonquery?token={2}&value={3}";
      postFormat = "{0}{1}executenonquerypost?token={2}&compression={3}";
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
