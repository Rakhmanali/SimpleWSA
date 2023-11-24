using Newtonsoft.Json;
using SimpleWSA.WSALibrary.Exceptions;
using SimpleWSA.WSALibrary.Extensions;
using SimpleWSA.WSALibrary.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWSA.WSALibrary.Services
{
  public class HttpService : IHttpService
  {
    protected readonly ICompressionService compressionService = new CompressionService();

    public virtual object Get(string requestUri, WebProxy webProxy, CompressionType returnCompressionType)
    {
      try
      {
        var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
        if (httpWebRequest != null)
        {
          httpWebRequest.Method = HttpMethod.GET.ToString();
          httpWebRequest.ContentLength = 0;
          httpWebRequest.Timeout = 1 * 60 * 60 * 1000;

          if (webProxy != null)
          {
            httpWebRequest.Proxy = webProxy;
          }

          httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

          using (var httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
          {
            if (httpWebResponse.StatusCode == HttpStatusCode.OK)
            {
              using (var stream = httpWebResponse.GetResponseStream())
              {
                using (var memoryStream = new MemoryStream())
                {
                  stream.CopyTo(memoryStream);
                  return Encoding.UTF8.GetString(memoryStream.ToArray());
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
        var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
        if (httpWebRequest != null)
        {
          httpWebRequest.Timeout = 1 * 60 * 60 * 1000;

          if (webProxy != null)
          {
            httpWebRequest.Proxy = webProxy;
          }

          httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

          byte[] postData = this.compressionService.Compress(requestString, outgoingCompressionType);
          httpWebRequest.InitializeWebRequest(outgoingCompressionType, postData, webProxy);

          using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
          {
            if (httpWebResponse?.StatusCode == HttpStatusCode.OK)
            {
              using (var stream = httpWebResponse.GetResponseStream())
              {
                using (var memoryStream = new MemoryStream())
                {
                  stream.CopyTo(memoryStream);
                  return Encoding.UTF8.GetString(memoryStream.ToArray());
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

    public virtual async Task<object> GetAsync(string baseAddress, string requestUri, WebProxy webProxy)
    {
      var httpClientHandler = new HttpClientHandler();
      httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip;
      if (webProxy != null)
      {
        httpClientHandler.Proxy = webProxy;
        httpClientHandler.UseProxy = true;
      }

      using (var httpClient = new HttpClient(httpClientHandler))
      {
        httpClient.BaseAddress = new Uri(baseAddress);
        httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

        using (var httpResponseMessage = await httpClient.GetAsync(requestUri))
        {
          if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
          {
            return await httpResponseMessage.Content.ReadAsStringAsync();
          }
          else
          {
            CreateAndThrowIfRestServiceException(httpResponseMessage);
            httpResponseMessage.EnsureSuccessStatusCode();
          }
        }
      }
      return null;
    }

    public virtual async Task<object> PostAsync(string baseAddress, string requestUri, string requestString, WebProxy webProxy, CompressionType outgoingCompressionType)
    {
      var httpClientHandler = new HttpClientHandler();
      httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip;
      if (webProxy != null)
      {
        httpClientHandler.Proxy = webProxy;
        httpClientHandler.UseProxy = true;
      }

      using (HttpClient httpClient = new HttpClient(httpClientHandler))
      {
        httpClient.BaseAddress = new Uri(baseAddress);
        httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

        byte[] postData = this.compressionService.Compress(requestString, outgoingCompressionType);
        using (var byteArrayContent = new ByteArrayContent(postData))
        {
          byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(outgoingCompressionType.SetWebRequestContentType());
          using (HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(requestUri, byteArrayContent))
          {
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
              return await httpResponseMessage.Content.ReadAsStringAsync();
            }
            else
            {
              this.CreateAndThrowIfRestServiceException(httpResponseMessage);
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

      var statusDescription = httpWebResponse.Headers.Get(Constants.HTTP_RESPONSE_STATUS_DESCRIPTION_KEY);
      this.CreateAndThrowIfRestServiceException(statusDescription);
    }

    protected void CreateAndThrowIfRestServiceException(string source)
    {
      if (!string.IsNullOrEmpty(source))
      {
        ErrorReply errorReply = null;
        try
        {
          errorReply = JsonConvert.DeserializeObject<ErrorReply>(source);
        }
        catch { }

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

    private void CreateAndThrowIfRestServiceException(HttpResponseMessage httpResponseMessage)
    {
      this.CreateAndThrowIfRestServiceException(httpResponseMessage.ReasonPhrase);

      if (httpResponseMessage.Headers.TryGetValues(Constants.HTTP_RESPONSE_STATUS_DESCRIPTION_KEY, out IEnumerable<string> values))
      {
        this.CreateAndThrowIfRestServiceException(values.First());
      }
    }

  }
}
