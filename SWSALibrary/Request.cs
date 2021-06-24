using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using SimpleWSA.Extensions;
using SimpleWSA.Internal;
using SimpleWSA.Services;

namespace SimpleWSA
{
  public class Request : IRequest
  {
    protected readonly string serviceAddress;
    protected readonly string route;
    protected readonly string token;
    protected readonly Command command;
    protected Dictionary<string, string> errorCodes;
    protected readonly IConvertingService convertingService;
    protected readonly ICompressionService compressionService;
    protected readonly WebProxy webProxy;

    public Request(Command command,
                   Dictionary<string, string> errorCodes,
                   IConvertingService convertingService)
    {
      this.command = command;
      this.errorCodes = errorCodes;
      this.convertingService = convertingService;
    }

    public Request(string serviceAddress,
                   string route,
                   string token,
                   Command command,
                   Dictionary<string, string> errorCodes,
                   IConvertingService convertingService,
                   ICompressionService compressionService,
                   WebProxy webProxy)
    {
      this.serviceAddress = serviceAddress;
      this.route = route;
      this.token = token;
      this.command = command;
      this.errorCodes = errorCodes;
      this.convertingService = convertingService;
      this.compressionService = compressionService;
      this.webProxy = webProxy;
    }

    private XmlWriterSettings xmlWriterSettings => new XmlWriterSettings()
    {
      CheckCharacters = true,
      ConformanceLevel = ConformanceLevel.Auto,
      Encoding = Encoding.UTF8,
      CloseOutput = true
    };

    private void WriteArguments(XmlWriter xmlWriter,
                                Command command,
                                IConvertingService convertingService)
    {
      if (command == null || command.Parameters.Count == 0)
      {
        return;
      }

      xmlWriter.WriteStartElement(Constants.WS_XML_REQUEST_NODE_ARGUMENTS);
      object[] value;
      string parameterName;
      foreach (Parameter parameter in command.Parameters)
      {
        parameterName = parameter.Name.ToLower();

        value = convertingService.ConvertObjectToDb(parameter.PgsqlDbType,
                                                    parameter.Value, command.OutgoingEncodingType);
        if (value == null || value.Length == 0)
        {
          xmlWriter.WriteStartElement(parameterName);
          xmlWriter.WriteAttributeString("isNull", "true");
          xmlWriter.WriteEndElement();
        }
        else
        {
          if (command.OutgoingEncodingType == EncodingType.NONE)
          {
            /*
               if outgoingEncodingType == EncodingType.NONE and value is string family object then
               the function ConvertObjectToDb() "converts" it to something like

               <![CDATA[ ... ]]>

               but the XmlWriter converts special symbols to the "safe" for XML, for example, it
               converts "<" to "&lt;"

               To prevent it here we need some additional code and using
               WriteRaw method of XmlWriter
            */
            if (this.IsEncodingAllowedType(parameter.PgsqlDbType) == true)
            {
              foreach (object v in value)
              {
                if (v == null)
                {
                  xmlWriter.WriteElementString(parameterName, Constants.STRING_NULL);
                }
                else
                {
                  xmlWriter.WriteStartElement(parameterName);
                  xmlWriter.WriteRaw(Convert.ToString(v, CultureInfo.InvariantCulture));
                  xmlWriter.WriteEndElement();
                }
              }
            }
            else
            {
              foreach (object v in value)
              {
                if (v == null)
                {
                  xmlWriter.WriteElementString(parameterName, Constants.STRING_NULL);
                }
                else
                {
                  xmlWriter.WriteElementString(parameterName, Convert.ToString(v, CultureInfo.InvariantCulture));
                }
              }
            }
          }
          else
          {
            if (this.IsEncodingAllowedType(parameter.PgsqlDbType) == true)
            {
              int encodingType = (int)command.OutgoingEncodingType;
              string encodedValue;
              foreach (object v in value)
              {
                encodedValue = convertingService.EncodeTo(v, command.OutgoingEncodingType);
                xmlWriter.WriteStartElement(parameterName);
                if (encodedValue.Trim().Length > 0)
                {
                  xmlWriter.WriteAttributeString(Constants.WS_XML_REQUEST_ATTRIBUTE_ENCODING, $"{encodingType}");
                }
                xmlWriter.WriteValue(encodedValue);
                xmlWriter.WriteEndElement();
              }
            }
            else
            {
              foreach (object v in value)
              {
                if (v == null)
                {
                  xmlWriter.WriteElementString(parameterName, Constants.STRING_NULL);
                }
                else
                {
                  xmlWriter.WriteElementString(parameterName, Convert.ToString(v, CultureInfo.InvariantCulture));
                }
              }
            }
          }
        }
      }

      xmlWriter.WriteEndElement(); // _arguments
    }

    private bool IsEncodingAllowedType(PgsqlDbType pgsqlDbType)
    {
      bool result = false;
      switch (pgsqlDbType)
      {
        case PgsqlDbType.Varchar:
        case PgsqlDbType.Text:
        case PgsqlDbType.Xml:
        case PgsqlDbType.Json:
        case PgsqlDbType.Jsonb:
        case PgsqlDbType.Varchar | PgsqlDbType.Array:
        case PgsqlDbType.Text | PgsqlDbType.Array:
        case PgsqlDbType.Xml | PgsqlDbType.Array:
        case PgsqlDbType.Json | PgsqlDbType.Array:
        case PgsqlDbType.Jsonb | PgsqlDbType.Array:
          result = true;
          break;
      }
      return result;
    }

    private void WriteOptions(XmlWriter xmlWriter, Command command)
    {
      xmlWriter.WriteStartElement(Constants.WS_XML_REQUEST_NODE_OPTIONS);

      if (command.ClearPool == ClearPool.TRUE)
      {
        xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_CLEAR_POOL, ((int)command.ClearPool).ToString());
      }

      if (command.GetFromCache == GetFromCache.TRUE) // Default value is FALSE
      {
        xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_FROM_CACHE, ((int)command.GetFromCache).ToString());
      }

      if (command.WriteSchema == WriteSchema.TRUE) // Default value is FALSE
      {
        xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_WRITE_SCHEMA, ((int)command.WriteSchema).ToString());
      }

#if DEBUG
      xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_COMMAND_TIMEOUT, "300"); //300 timeout seconds for debugging into pg-functions
#else
                    if (command.CommandTimeout != 20) // Default value is 20 seconds
                    {
                        xmlWriter.WriteElementString("_commandTimeout", $"{command.CommandTimeout}");
                    }
#endif
      if (command.ReturnEncodingType != EncodingType.NONE)
      {
        xmlWriter.WriteStartElement(Constants.WS_XML_REQUEST_NODE_ENCODING);
        xmlWriter.WriteAttributeString(Constants.WS_XML_REQUEST_NODE_ENCODING_ATTRIBUTE_IS_ENTRY, "0");
        xmlWriter.WriteValue(((int)command.ReturnEncodingType).ToString());
        xmlWriter.WriteEndElement();
      }

      if (command.IsolationLevel != IsolationLevel.ReadCommitted) //Default value is ReadCommitted
      {
        xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_ISOLATION_LEVEL, ((int)command.IsolationLevel).ToString());
      }

      xmlWriter.WriteEndElement();  //_options
    }

    private void WriteOptions(XmlWriter xmlWriter, Command command, RoutineType routineType)
    {
      xmlWriter.WriteStartElement(Constants.WS_XML_REQUEST_NODE_OPTIONS);

      if (command.ClearPool == ClearPool.TRUE)
      {
        xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_CLEAR_POOL, ((int)command.ClearPool).ToString());
      }

      if (command.GetFromCache == GetFromCache.TRUE) // Default value is FALSE
      {
        xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_FROM_CACHE, ((int)command.GetFromCache).ToString());
      }

      if (command.WriteSchema == WriteSchema.TRUE) // Default value is FALSE
      {
        xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_WRITE_SCHEMA, ((int)command.WriteSchema).ToString());
      }

#if DEBUG
      xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_COMMAND_TIMEOUT, "300"); //300 timeout seconds for debugging into pg-functions
#else
                    if (command.CommandTimeout != 20) // Default value is 20 seconds
                    {
                        xmlWriter.WriteElementString("_commandTimeout", $"{command.CommandTimeout}");
                    }
#endif
      if (command.ReturnEncodingType != EncodingType.NONE)
      {
        xmlWriter.WriteStartElement(Constants.WS_XML_REQUEST_NODE_ENCODING);
        xmlWriter.WriteAttributeString(Constants.WS_XML_REQUEST_NODE_ENCODING_ATTRIBUTE_IS_ENTRY, "0");
        xmlWriter.WriteValue(((int)command.ReturnEncodingType).ToString());
        xmlWriter.WriteEndElement();
      }

      if (command.IsolationLevel != IsolationLevel.ReadCommitted) //Default value is ReadCommitted
      {
        xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_ISOLATION_LEVEL, ((int)command.IsolationLevel).ToString());
      }


      xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_ROUTINE_TYPE, $"{(int)routineType}");



      xmlWriter.WriteEndElement();  //_options
    }

    private void WriteRoutine(XmlWriter xmlWriter,
                             Command command,
                             IConvertingService convertingService)
    {
      xmlWriter.WriteStartElement(Constants.WS_XML_REQUEST_NODE_ROUTINE);
      xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_NAME, command.Name.ToLower());
      this.WriteArguments(xmlWriter, command, convertingService);
      this.WriteOptions(xmlWriter, command);
      xmlWriter.WriteEndElement();  //_routine
    }

    private void WriteRoutine(XmlWriter xmlWriter,
                             Command command,
                             IConvertingService convertingService, 
                             RoutineType routineType)
    {
      xmlWriter.WriteStartElement(Constants.WS_XML_REQUEST_NODE_ROUTINE);
      xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_NAME, command.Name.ToLower());
      this.WriteArguments(xmlWriter, command, convertingService);
      this.WriteOptions(xmlWriter, command, routineType);
      xmlWriter.WriteEndElement();  //_routine
    }

    private string CreateXmlRequest()
    {
      string result = string.Empty;
      StringBuilder sb = new StringBuilder();
      using (XmlWriter xmlWriter = XmlWriter.Create(sb, this.xmlWriterSettings))
      {
        xmlWriter.WriteStartElement(Constants.WS_XML_REQUEST_NODE_ROUTINES);

        this.WriteRoutine(xmlWriter, this.command, this.convertingService);

        if (this.command.ReturnCompressionType != CompressionType.NONE)
        {
          xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_COMPRESSION, ((int)this.command.ReturnCompressionType).ToString());
        }

        xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_RETURN_TYPE, this.command.ResponseFormat.ToString());

        if (this.command.ResponseFormat == ResponseFormat.JSON)
        {
          xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_JSON_DATE_FORMAT, "2");
        }

        xmlWriter.WriteEndElement();  //_routines

        xmlWriter.Flush();

        result = sb.ToString();
      }

      return result;
    }

    public string CreateXmlRoutine()
    {
      string result = string.Empty;
      StringBuilder sb = new StringBuilder();
      using (XmlWriter xmlWriter = XmlWriter.Create(sb, this.xmlWriterSettings))
      {
        this.WriteRoutine(xmlWriter, this.command, this.convertingService);
        xmlWriter.Flush();
        result = sb.ToString();
      }

      return result;
    }

    public string CreateXmlRoutine(RoutineType routineType)
    {
      string result = string.Empty;
      StringBuilder sb = new StringBuilder();
      using (XmlWriter xmlWriter = XmlWriter.Create(sb, this.xmlWriterSettings))
      {
        this.WriteRoutine(xmlWriter, this.command, this.convertingService, routineType);
        xmlWriter.Flush();
        result = sb.ToString();
      }

      return result;
    }

    public static string postFormat = null;
    //protected virtual object Post(string xmlRequest)
    //{
    //  return null;
    //}
    protected virtual object Post(string requestString)
    {
      string query = string.Format(postFormat, this.serviceAddress, this.route, this.token, (int)this.command.OutgoingCompressionType);
      try
      {
        WebRequest webRequest = WebRequest.Create(query);
        if (webRequest != null)
        {
          webRequest.Timeout = 1 * 60 * 60 * 1000;
          webRequest.Proxy = this.webProxy;

          byte[] postData = this.compressionService.Compress(requestString, this.command.OutgoingCompressionType);
          webRequest.InitializeWebRequest(this.command.OutgoingCompressionType, postData, this.webProxy);
          using (HttpWebResponse httpWebResponse = webRequest.GetResponse() as HttpWebResponse)
          {
            if (httpWebResponse?.StatusCode == HttpStatusCode.OK)
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


    public static string getFormat = null;
    protected virtual object Get(string requestString)
    {
      string query = string.Format(getFormat, this.serviceAddress, this.route, this.token, requestString);
      try
      {
        var webRequest = WebRequest.Create(query);
        if (webRequest != null)
        {
          webRequest.Method = HttpMethod.GET.ToString(); ;
          webRequest.ContentLength = 0;
          webRequest.Timeout = 1 * 60 * 60 * 1000;
          webRequest.Proxy = this.webProxy;

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

    //protected virtual Task<object> PostAsync(string xmlRequest)
    //{
    //  return null;
    //}
    protected virtual async Task<object> PostAsync(string requestString)
    {
      HttpClientHandler httpClientHandler = new HttpClientHandler
      {
        Proxy = this.webProxy,
        UseProxy = this.webProxy != null
      };

      using (HttpClient httpClient = new HttpClient(httpClientHandler))
      {
        httpClient.BaseAddress = new Uri(this.serviceAddress);
        httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

        using (StringContent stringContent = new StringContent(requestString, Encoding.UTF8, "text/xml"))
        {
          string requestUri = string.Format(postFormat, string.Empty, this.route, this.token, (int)this.command.OutgoingCompressionType);
          using (HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(requestUri, stringContent))
          {
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
              using (Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync())
              {
                byte[] result = this.compressionService.Decompress(stream, this.command.ReturnCompressionType);
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

    //protected virtual Task<object> GetAsync(string xmlRequest)
    //{
    //  return null;
    //}
    protected virtual async Task<object> GetAsync(string requestString)
    {
      HttpClientHandler httpClientHandler = new HttpClientHandler
      {
        Proxy = this.webProxy,
        UseProxy = this.webProxy != null
      };

      using (HttpClient httpClient = new HttpClient(httpClientHandler))
      {
        httpClient.BaseAddress = new Uri(this.serviceAddress);
        httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

        string requestUri = string.Format(getFormat, string.Empty, this.route, this.token, requestString);
        using (HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestUri))
        {
          if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
          {
            using (Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync())
            {
              byte[] result = this.compressionService.Decompress(stream, this.command.ReturnCompressionType);
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

    public virtual object Send()
    {
      string xmlRequest = this.CreateXmlRequest();
      if (this.command.HttpMethod == HttpMethod.POST)
      {
        return this.Post(xmlRequest);
      }
      return this.Get(xmlRequest);
    }

    public async Task<object> SendAsync()
    {
      string xmlRequest = this.CreateXmlRequest();
      if (this.command.HttpMethod == HttpMethod.POST)
      {
        return await this.PostAsync(xmlRequest);
      }
      return await this.GetAsync(xmlRequest);
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
          if (this.errorCodes.TryGetValue(errorReply.Error.ErrorCode, out wsaMessage) == false)
          {
            wsaMessage = errorReply.Error.Message;
          }
          throw new RestServiceException(wsaMessage, errorReply.Error.ErrorCode, errorReply.Error.Message);
        }
      }
    }

    public static object Post(string serviceAddress,
                              string route, 
                              string requestString,
                              string token,
                              CompressionType outgoingCompressionType,
                              CompressionType returnCompressionType,
                              ICompressionService compressionService,
                              Dictionary<string, string> errorCodes,
                              WebProxy webProxy, 
                              string postFormat)
    {
      string query = string.Format(postFormat, serviceAddress, route, token, (int)outgoingCompressionType);

      try
      {
        var webRequest = WebRequest.Create(query);
        if (webRequest != null)
        {
          webRequest.Timeout = 1 * 60 * 60 * 1000;

          byte[] postData = compressionService.Compress(requestString, outgoingCompressionType);
          webRequest.InitializeWebRequest(outgoingCompressionType, postData, webProxy);
          using (var httpWebResponse = webRequest.GetResponse() as HttpWebResponse)
          {
            if (httpWebResponse?.StatusCode == HttpStatusCode.OK)
            {
              using (var stream = httpWebResponse.GetResponseStream())
              {
                byte[] result = compressionService.Decompress(stream, returnCompressionType);
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
          HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
          ErrorReply errorReply = JsonConvert.DeserializeObject<ErrorReply>(httpWebResponse.StatusDescription);
          if (errorReply != null)
          {
            string wsaMessage = null;
            if (errorCodes.TryGetValue(errorReply.Error.ErrorCode, out wsaMessage) == false)
            {
              wsaMessage = errorReply.Error.Message;
            }
            throw new RestServiceException(wsaMessage, errorReply.Error.ErrorCode, errorReply.Error.Message);
          }
        }
        throw;
      }

      return null;
    }
  }
}
