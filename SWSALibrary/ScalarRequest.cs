using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using SimpleWSA.Extensions;
using SimpleWSA.Internal;
using SimpleWSA.Services;

namespace SimpleWSA
{
  public sealed class ScalarRequest : Request
  {
    private readonly string serviceAddress;
    private readonly string token;
    private readonly WebProxy webProxy;
    private readonly ICompressionService compressionService;

    public ScalarRequest(string serviceAddress,
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

    public const string postFormat = "{0}executescalarpost?token={1}&compression={2}";

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
                  return System.Text.Encoding.UTF8.GetString(result);
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

    const string getFormat = "{0}executescalar?token={1}&value={2}";
    protected override object Get(string requestString)
    {
      object resultObject = null;
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
              using (var stream = httpWebResponse.GetResponseStream())
              {
                byte[] result = this.compressionService.Decompress(stream, this.command.ReturnCompressionType);
                if (result != null)
                {
                  return System.Text.Encoding.UTF8.GetString(result);
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
      return resultObject;
    }
  }
}
