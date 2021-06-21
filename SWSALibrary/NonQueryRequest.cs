using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using SimpleWSA.Extensions;
using SimpleWSA.Services;

namespace SimpleWSA
{
  public sealed class NonQueryRequest : Request
  {
    private readonly string serviceAddress;
    private readonly string token;
    private readonly WebProxy webProxy;
    private readonly ICompressionService compressionService;

    public NonQueryRequest(string serviceAddress,
                           string token,
                           Command command,
                           Dictionary<string, string> errorCodes,
                           IConvertingService convertingService,
                           ICompressionService compressionService,
                           WebProxy webProxy) : base(command,
                                                     errorCodes,
                                                     convertingService)
    {
      this.serviceAddress = serviceAddress;
      this.token = token;
      this.webProxy = webProxy;
      this.compressionService = compressionService;
    }

    public const string postFormat = "{0}executenonquerypost?token={1}&compression={2}";

    protected override object Post(string requestString)
    {
      string query = string.Format(postFormat, this.serviceAddress, this.token, (int)this.command.OutgoingCompressionType);
      try
      {
        var webRequest = WebRequest.Create(query);
        if (webRequest != null)
        {
          webRequest.Timeout = 1 * 60 * 60 * 1000;

          byte[] postData = this.compressionService.Compress(requestString, this.command.OutgoingCompressionType);

          webRequest.InitializeWebRequest(this.command.OutgoingCompressionType, postData, this.webProxy);
          using (var httpWebResponse = webRequest.GetResponse() as HttpWebResponse)
          {
            if (httpWebResponse?.StatusCode == HttpStatusCode.OK)
            {
              using (var stream = httpWebResponse.GetResponseStream())
              {
                byte[] result = this.compressionService.Decompress(stream, this.command.ReturnCompressionType);
                if (result != null)
                {
                  return Encoding.UTF8.GetString(result);
                }
              }
            }
          }
        }
      }
      catch (WebException ex)
      {
        if (ex.Response is HttpWebResponse)
        {
          this.CreateAndThrowIfRestServiceException((HttpWebResponse)ex.Response);
        }
        throw;
      }

      return null;
    }

    const string getFormat = "{0}executenonquery?token={1}&value={2}";
    protected override object Get(string requestString)
    {
      string query = string.Format(getFormat, this.serviceAddress, this.token, requestString);
      try
      {
        var webRequest = WebRequest.Create(query);
        if (webRequest != null)
        {
          webRequest.Method = HttpMethod.GET.ToString(); ;
          webRequest.ContentLength = 0;
          webRequest.Timeout = 1 * 60 * 60 * 1000;

          using (var httpWebResponse = webRequest.GetResponse() as HttpWebResponse)
          {
            if (httpWebResponse.StatusCode == HttpStatusCode.OK)
            {
              using (Stream stream = httpWebResponse.GetResponseStream())
              {
                byte[] result = this.compressionService.Decompress(stream, this.command.ReturnCompressionType);
                if (result != null)
                {
                  return Encoding.UTF8.GetString(result);
                }
              }
            }
          }
        }
      }
      catch (WebException ex)
      {
        if (ex.Response is HttpWebResponse)
        {
          this.CreateAndThrowIfRestServiceException((HttpWebResponse)ex.Response);
        }
        throw;
      }
      return null;
    }
  }
}
