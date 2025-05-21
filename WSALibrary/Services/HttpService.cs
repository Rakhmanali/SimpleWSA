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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWSA.WSALibrary.Services
{
  public class HttpService : IHttpService
  {
    protected readonly ICompressionService compressionService = new CompressionService();

    public virtual object Get(string requestUri, WebProxy webProxy, int httpTimeout)
    {
      try
      {
        var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
        if (httpWebRequest != null)
        {
          httpWebRequest.Method = nameof(HttpMethod.GET);
          httpWebRequest.ContentLength = 0;
          httpWebRequest.Timeout = httpTimeout;

          if (webProxy != null)
          {
            httpWebRequest.Proxy = webProxy;
          }

          // skip the certificate validation ...
          httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

          httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

          using (var httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
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

    public virtual object Post(string requestUri, string requestString, WebProxy webProxy, CompressionType outgoingCompressionType, int httpTimeout)
    {
      try
      {
        var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
        if (httpWebRequest != null)
        {
          httpWebRequest.Timeout = httpTimeout;

          // skip the certificate validation ...
          httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

          byte[] postData;
          if (outgoingCompressionType != CompressionType.NONE)
          {
            postData = this.compressionService.Compress(requestString, outgoingCompressionType);
          }
          else
          {
            postData = Encoding.UTF8.GetBytes(requestString);
          }

          httpWebRequest.InitializeWebRequest(outgoingCompressionType, postData, webProxy);

          using (var httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
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

    public virtual async Task<object> GetAsync(string baseAddress, string requestUri, WebProxy webProxy, int httpTimeout, CancellationToken cancellationToken)
    {
      var httpClientHandler = new HttpClientHandler
      {
        AutomaticDecompression = DecompressionMethods.GZip,

        ClientCertificateOptions = ClientCertificateOption.Manual,
        ServerCertificateCustomValidationCallback = (httpRequestMessage, certificate, chain, sslPolicyErrors) => true,

        Proxy = webProxy,
        UseProxy = webProxy != null
      };

      using (var httpClient = new HttpClient(httpClientHandler))
      {
        httpClient.BaseAddress = new Uri(baseAddress);
        httpClient.Timeout = TimeSpan.FromMilliseconds(httpTimeout);

        using (var httpResponseMessage = await httpClient.GetAsync(requestUri, cancellationToken))
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

    public virtual async Task<object> PostAsync(string baseAddress, string requestUri, string requestString, WebProxy webProxy, CompressionType outgoingCompressionType, int httpTimeout, CancellationToken cancellationToken)
    {
      var httpClientHandler = new HttpClientHandler
      {
        AutomaticDecompression = DecompressionMethods.GZip,

        ClientCertificateOptions = ClientCertificateOption.Manual,
        ServerCertificateCustomValidationCallback = (httpRequestMessage, certificate, chain, sslPolicyErrors) => true,

        Proxy = webProxy,
        UseProxy = webProxy != null
      };

      byte[] postData;
      if (outgoingCompressionType != CompressionType.NONE)
      {
        postData = this.compressionService.Compress(requestString, outgoingCompressionType);
      }
      else
      {
        postData = Encoding.UTF8.GetBytes(requestString);
      }

      using (var content = new ByteArrayContent(postData))
      {
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");
        if (outgoingCompressionType != CompressionType.NONE)
        {
          content.Headers.ContentEncoding.Add("gzip");
        }

        using (var httpClient = new HttpClient(httpClientHandler))
        {
          httpClient.BaseAddress = new Uri(baseAddress);
          httpClient.Timeout = TimeSpan.FromMilliseconds(httpTimeout);
          using (var httpResponseMessage = await httpClient.PostAsync(requestUri, content, cancellationToken))
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
          throw new DawaException(wsaMessage, errorReply.Error.ErrorCode, errorReply.Error.Message);
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
