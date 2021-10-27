using Newtonsoft.Json;
using SimpleWSA.Exceptions;
using SimpleWSA.Extensions;
using SimpleWSA.Internal;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWSA.Services
{
  public class HttpService : IHttpService
  {
    protected readonly ICompressionService compressionService = new CompressionService();

    public virtual object Get(string requestUri, WebProxy webProxy, CompressionType returnCompressionType)
    {
      try
      {
        var webRequest = WebRequest.Create(requestUri);
        if (webRequest != null)
        {
          webRequest.Method = HttpMethod.GET.ToString(); ;
          webRequest.ContentLength = 0;
          webRequest.Timeout = 1 * 60 * 60 * 1000;
          webRequest.Proxy = webProxy;

          using (var httpWebResponse = webRequest.GetResponse() as HttpWebResponse)
          {
            if (httpWebResponse.StatusCode == HttpStatusCode.OK)
            {
              using (Stream stream = httpWebResponse.GetResponseStream())
              {
                byte[] result = this.compressionService.Decompress(stream, returnCompressionType);
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

    public virtual object Post(string requestUri, string requestString, WebProxy webProxy, CompressionType outgoingCompressionType, CompressionType returnCompressionType)
    {
      try
      {
        WebRequest webRequest = WebRequest.Create(requestUri);
        if (webRequest != null)
        {
          webRequest.Timeout = 1 * 60 * 60 * 1000;
          webRequest.Proxy = webProxy;

          byte[] postData = this.compressionService.Compress(requestString, outgoingCompressionType);
          webRequest.InitializeWebRequest(outgoingCompressionType, postData, webProxy);
          using (HttpWebResponse httpWebResponse = webRequest.GetResponse() as HttpWebResponse)
          {
            if (httpWebResponse?.StatusCode == HttpStatusCode.OK)
            {
              using (Stream stream = httpWebResponse.GetResponseStream())
              {
                byte[] result = this.compressionService.Decompress(stream, returnCompressionType);
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

    public virtual async Task<object> GetAsync(string baseAddress, string requestUri, WebProxy webProxy, CompressionType returnCompressionType)
    {
      HttpClientHandler httpClientHandler = new HttpClientHandler
      {
        Proxy = webProxy,
        UseProxy = webProxy != null
      };

      using (HttpClient httpClient = new HttpClient(httpClientHandler))
      {
        httpClient.BaseAddress = new Uri(baseAddress);
        httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

        using (HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestUri))
        {
          if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
          {
            using (Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync())
            {
              byte[] result = this.compressionService.Decompress(stream, returnCompressionType);
              if (result != null)
              {
                return Encoding.UTF8.GetString(result);
              }
            }
          }
          else
          {
            this.CreateAndThrowIfRestServiceException(httpResponseMessage.ReasonPhrase);
            httpResponseMessage.EnsureSuccessStatusCode();
          }
        }
      }
      return null;
    }

    public virtual async Task<object> PostAsync(string baseAddress, string requestUri, string requestString, WebProxy webProxy, CompressionType returnCompressionType)
    {
      HttpClientHandler httpClientHandler = new HttpClientHandler
      {
        Proxy = webProxy,
        UseProxy = webProxy != null
      };

      using (HttpClient httpClient = new HttpClient(httpClientHandler))
      {
        httpClient.BaseAddress = new Uri(baseAddress);
        httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

        using (StringContent stringContent = new StringContent(requestString, Encoding.UTF8, "text/xml"))
        {
          using (HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(requestUri, stringContent))
          {
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
              using (Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync())
              {
                byte[] result = this.compressionService.Decompress(stream, returnCompressionType);
                if (result != null)
                {
                  return Encoding.UTF8.GetString(result);
                }
              }
            }
            else
            {
              this.CreateAndThrowIfRestServiceException(httpResponseMessage.ReasonPhrase);
              httpResponseMessage.EnsureSuccessStatusCode();
            }
          }
        }
      }
      return null;
    }

    protected void CreateAndThrowIfRestServiceException(HttpWebResponse httpWebResponse)
    {
      this.CreateAndThrowIfRestServiceException(httpWebResponse.StatusDescription);
    }

    protected void CreateAndThrowIfRestServiceException(string source)
    {
      if (!string.IsNullOrEmpty(source))
      {
        ErrorReply errorReply = JsonConvert.DeserializeObject<ErrorReply>(source);
        if (errorReply != null)
        {
          string wsaMessage = null;
          if (ErrorCodes.Collection.TryGetValue(errorReply.Error.ErrorCode, out wsaMessage) == false)
          {
            wsaMessage = errorReply.Error.Message;
          }
          throw new RestServiceException(wsaMessage, errorReply.Error.ErrorCode, errorReply.Error.Message);
        }
      }
    }
  }
}
