using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using SimpleWSA.Internal;

namespace SimpleWSA
{
  public class SessionService
  {
    private readonly string baseAddress;
    private readonly string requestUri;
    private readonly string login;
    private readonly string password;
    private readonly bool isEncrypted;
    private readonly int appId;
    private readonly string appVersion;
    private readonly string domain;
    private readonly Dictionary<string, string> errorCodes;
    private readonly WebProxy webProxy;

    private const string MEDIA_TYPE = "text/xml";

    public SessionService(string baseAddress,
                          string requestUri,
                          string login,
                          string password,
                          bool isEncrypted,
                          int appId,
                          string appVersion,
                          string domain,
                          Dictionary<string, string> errorCodes,
                          WebProxy webProxy)
    {
      this.baseAddress = baseAddress;
      this.requestUri = requestUri;
      this.login = login;
      this.password = password;
      this.isEncrypted = isEncrypted;
      this.appId = appId;
      this.appVersion = appVersion;
      this.domain = domain;
      this.errorCodes = errorCodes;
      this.webProxy = webProxy;
    }

    private XmlWriterSettings xmlWriterSettings => new XmlWriterSettings()
    {
      CheckCharacters = true,
      ConformanceLevel = ConformanceLevel.Auto,
      Encoding = Encoding.UTF8,
      CloseOutput = true
    };

    /*
     <_routines>
       <_routine>
         <_name>InitializeSession</_name>
         <_arguments>
           <login isEncoded="1">c2FkbWluQHVwc3RhaXJzLmNvbQ==</login>
           <password isEncoded="1">R3JvbWl0MTI=</password>
           <isEncrypt>0</isEncrypt>
           <timeout>20</timeout>
           <appId>13</appId>
           <domain>upstairstest</domain>
         </_arguments>
       </_routine>
       <_returnType>XML</_returnType>
     </_routines>
   */

    private string CreateXmlRequest()
    {
      StringBuilder sb = new StringBuilder();

      using (XmlWriter writer = XmlWriter.Create(sb, this.xmlWriterSettings))
      {
        writer.WriteStartElement(Constants.WS_XML_REQUEST_NODE_ROUTINES);
        writer.WriteStartElement(Constants.WS_XML_REQUEST_NODE_ROUTINE);
        writer.WriteElementString(Constants.WS_XML_REQUEST_NODE_NAME, Constants.WS_INITIALIZE_SESSION);
        writer.WriteStartElement(Constants.WS_XML_REQUEST_NODE_ARGUMENTS);

        int encodingType = (int)EncodingType.BASE64;
        writer.WriteStartElement(Constants.WS_LOGIN);
        writer.WriteAttributeString(Constants.WS_IS_ENCODED, $"{encodingType}");
        writer.WriteValue(this.ConvertToBase64String(this.login));
        writer.WriteEndElement();

        writer.WriteStartElement(Constants.WS_PASSWORD);
        writer.WriteAttributeString(Constants.WS_IS_ENCODED, $"{encodingType}");
        writer.WriteValue(this.ConvertToBase64String(this.password));
        writer.WriteEndElement();

        writer.WriteElementString(Constants.WS_IS_ENCRYPT, this.isEncrypted == true ? "1" : "0");

        writer.WriteElementString(Constants.WS_TIMEOUT, "20");
        writer.WriteElementString(Constants.WS_APP_ID, ((int)this.appId).ToString());

        if (this.appVersion?.Length > 0)
        {
          writer.WriteElementString(Constants.WS_APP_VERSION, this.appVersion);
        }

        if (this.domain?.Length > 0)
        {
          writer.WriteElementString(Constants.WS_DOMAIN, this.domain);
        }

        writer.WriteEndElement(); // _arguments
        writer.WriteEndElement();  //_routine
        writer.WriteElementString(Constants.WS_XML_REQUEST_NODE_RETURN_TYPE, Constants.WS_RETURN_TYPE_XML);
        writer.WriteEndElement();  //_routines

        writer.Flush();
        writer.Close();
      }

      return sb.ToString();
    }

    private async Task<string> PostAsync(string baseAddress, string requestUri, string postData, WebProxy webProxy)
    {
      HttpClientHandler httpClientHandler = new HttpClientHandler
      {
        Proxy = webProxy,
        UseProxy = webProxy != null
      };

      string result = String.Empty;
      using (HttpClient httpClient = new HttpClient(httpClientHandler))
      {
        httpClient.BaseAddress = new Uri(baseAddress);
        httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

        using (StringContent stringContent = new StringContent(postData, Encoding.UTF8, MEDIA_TYPE))
        {
          using (HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(requestUri, stringContent))
          {
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
              result = await httpResponseMessage.Content.ReadAsStringAsync();
              result = XElement.Parse(result).Element(Constants.WS_TOKEN).Value;
            }
            else
            {
              ErrorReply errorReply = JsonConvert.DeserializeObject<ErrorReply>(httpResponseMessage.ReasonPhrase);
              //this.logger.Error($"Post() - The rest service error, code: {errorReply.Error.ErrorCode}, message: {errorReply.Error.Message}");

              string wsaMessage = null;
              if (this.errorCodes.TryGetValue(errorReply.Error.ErrorCode, out wsaMessage) == false)
              {
                wsaMessage = errorReply.Error.Message;
              }
              throw new RestServiceException(wsaMessage, errorReply.Error.ErrorCode, errorReply.Error.Message);
            }
          }
        }
      }

      return result;
    }

    private readonly string TOKEN_START = $"<{Constants.WS_TOKEN}>";
    private readonly string TOKEN_END = $"</{Constants.WS_TOKEN}>";

    private async Task<string> GetAsync(string baseaddress, string requestUri, string queryString, WebProxy webProxy)
    {
      HttpClientHandler httpClientHandler = new HttpClientHandler
      {
        Proxy = webProxy,
        UseProxy = webProxy != null
      };

      string result = String.Empty;
      using (HttpClient httpClient = new HttpClient(httpClientHandler))
      {
        httpClient.BaseAddress = new Uri(baseAddress);
        httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

        string apiUrl = $"{requestUri}?{queryString}";
        using (HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(apiUrl))
        {
          if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
          {
            result = await httpResponseMessage.Content.ReadAsStringAsync();
            int startIndex = result.IndexOf(TOKEN_START) + TOKEN_START.Length;
            int length = result.IndexOf(TOKEN_END) - startIndex;
            result = result.Substring(startIndex, length);
          }
          else
          {
            ErrorReply errorReply = JsonConvert.DeserializeObject<ErrorReply>(httpResponseMessage.ReasonPhrase);
            //this.logger.Error($"Post() - The rest service error, code: {errorReply.Error.ErrorCode}, message: {errorReply.Error.Message}");

            string wsaMessage = null;
            if (this.errorCodes.TryGetValue(errorReply.Error.ErrorCode, out wsaMessage) == false)
            {
              wsaMessage = errorReply.Error.Message;
            }
            throw new RestServiceException(wsaMessage, errorReply.Error.ErrorCode, errorReply.Error.Message);
          }
        }
      }

      return result;
    }

    public async Task<string> SendAsync(HttpMethod httpMethod)
    {
      string result = null;
      if (httpMethod == HttpMethod.GET)
      {
        string queryString = this.CreateQueryString();
        result = await this.GetAsync(this.baseAddress, this.requestUri, queryString, this.webProxy);
      }
      else
      {
        string xmlRequest = this.CreateXmlRequest();
        result = await this.PostAsync(this.baseAddress, this.requestUri, xmlRequest, this.webProxy);
      }

      return result;
    }

    private string ConvertToBase64String(object value)
    {
      string result = String.Empty;

      if (value == null)
      {
        value = Constants.STRING_NULL;
      }

      result = Convert.ToString(value, CultureInfo.InvariantCulture);
      return Convert.ToBase64String(Encoding.UTF8.GetBytes(result));
    }

    private string CreateQueryString()
    {
      StringBuilder sb = new StringBuilder();

      sb.Append($"{Constants.WS_LOGIN}={Convert.ToBase64String(Encoding.UTF8.GetBytes(this.login))}");
      sb.Append($"&{Constants.WS_PASSWORD}={Convert.ToBase64String(Encoding.UTF8.GetBytes(this.password))}");
      sb.Append($"&{Constants.WS_IS_ENCODED}=1");

      sb.Append($"&{Constants.WS_IS_TOKEN}=1");

      if (this.isEncrypted == true)
      {
        sb.Append($"&{Constants.WS_IS_ENCRYPT}=1");
      }
      else
      {
        sb.Append($"&{Constants.WS_IS_ENCRYPT}=0");
      }

      sb.Append($"&{Constants.WS_TIMEOUT}=20");
      sb.Append($"&{Constants.WS_APP_ID}={(int)this.appId}");
      if (this.appVersion?.Length > 0)
      {
        sb.Append($"&{Constants.WS_APP_VERSION}={this.appVersion}");
      }

      if (this.domain?.Length > 0)
      {
        sb.Append($"&{Constants.WS_DOMAIN}={this.domain}");
      }

      sb.Append($"&{Constants.WS_RETURN_TYPE}={Constants.WS_RETURN_TYPE_XML}");

      return sb.ToString();
    }
  }
}
