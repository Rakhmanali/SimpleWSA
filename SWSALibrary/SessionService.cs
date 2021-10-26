using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using SimpleWSA.Internal;
using SimpleWSA.Services;

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

    private readonly IHttpService httpService = new HttpService();

    private string Get(string baseaddress, string requestUri, string queryString, WebProxy webProxy)
    {
      string query = $"{baseaddress}/{requestUri}?{queryString}";
      string result = Convert.ToString(this.httpService.Get(query, webProxy, CompressionType.NONE));
      return this.ExtractToken(result);
    }

    private async Task<string> GetAsync(string baseaddress, string requestUri, string queryString, WebProxy webProxy)
    {
      string apiUrl = $"{requestUri}?{queryString}";
      string result = Convert.ToString(await this.httpService.GetAsync(baseaddress, apiUrl, webProxy, CompressionType.NONE));
      return this.ExtractToken(result);
    }


    private async Task<string> PostAsync(string baseAddress, string requestUri, string postData, WebProxy webProxy)
    {
      string result = Convert.ToString(await this.httpService.PostAsync(baseAddress, requestUri, postData, webProxy, CompressionType.NONE));
      return XElement.Parse(result).Element(Constants.WS_TOKEN).Value;
    }

    private readonly string TOKEN_START = $"<{Constants.WS_TOKEN}>";
    private readonly string TOKEN_END = $"</{Constants.WS_TOKEN}>";

    private string ExtractToken(string source)
    {
      int startIndex = source.IndexOf(TOKEN_START) + TOKEN_START.Length;
      int length = source.IndexOf(TOKEN_END) - startIndex;
      return source.Substring(startIndex, length);
    }

    public string Send(HttpMethod httpMethod)
    {
      string result = null;
      //if (httpMethod == HttpMethod.GET)
      //{
      string queryString = this.CreateQueryString();
      result = this.Get(this.baseAddress, this.requestUri, queryString, this.webProxy);

      //}
      //else
      //{
      //  string xmlRequest = this.CreateXmlRequest();
      //  result = this.Post(this.baseAddress, this.requestUri, xmlRequest, this.webProxy);
      //}

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
