﻿using SimpleWSA.WSALibrary.Internal;
using SimpleWSA.WSALibrary.Services;
using System;
using System.Data;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace SimpleWSA.WSALibrary
{
  public class Request : IRequest
  {
    protected readonly string serviceAddress;
    protected string token;
    protected readonly Command command;
    protected readonly IConvertingService convertingService;
    protected readonly WebProxy webProxy;
    private readonly IHttpService httpService = new HttpService();
    private readonly string format;

    public Request(Command command, IConvertingService convertingService)
    {
      this.command = command;
      this.convertingService = convertingService;
    }

    public Request(string serviceAddress,
                   string token,
                   Command command,
                   IConvertingService convertingService,
                   WebProxy webProxy,
                   string format)
    {
      this.serviceAddress = serviceAddress;
      this.token = token;
      this.command = command;
      this.convertingService = convertingService;
      this.webProxy = webProxy;
      this.format = format;
    }

    public void SetToken(string token)
    {
      this.token = token;
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

      if (command.GetFromCache == FromCache.TRUE) // Default value is FALSE
      {
        xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_FROM_CACHE, ((int)command.GetFromCache).ToString());
      }

      if (command.WriteSchema == WriteSchema.TRUE) // Default value is FALSE
      {
        xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_WRITE_SCHEMA, ((int)command.WriteSchema).ToString());
      }

      if (command.CommandTimeout != 20) // Default value is 20 seconds
      {
        xmlWriter.WriteElementString("_commandTimeout", $"{command.CommandTimeout}");
      }

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

      if (command.GetFromCache == FromCache.TRUE) // Default value is FALSE
      {
        xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_FROM_CACHE, ((int)command.GetFromCache).ToString());
      }

      if (command.WriteSchema == WriteSchema.TRUE) // Default value is FALSE
      {
        xmlWriter.WriteElementString(Constants.WS_XML_REQUEST_NODE_WRITE_SCHEMA, ((int)command.WriteSchema).ToString());
      }

      if (command.CommandTimeout != 20) // Default value is 20 seconds
      {
        xmlWriter.WriteElementString("_commandTimeout", $"{command.CommandTimeout}");
      }

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

    protected virtual object Get(string requestString, int httpTimeout)
    {
      var requestUri = string.Format(this.format, this.serviceAddress, this.token, HttpUtility.UrlEncode(requestString));
      return this.httpService.Get(requestUri, this.webProxy, httpTimeout);
    }

    protected virtual object Post(string requestString, int httpTimeout)
    {
      string requestUri = string.Format(this.format, this.serviceAddress, this.token, (int)this.command.OutgoingCompressionType);
      return this.httpService.Post(requestUri, requestString, this.webProxy, this.command.OutgoingCompressionType, httpTimeout);
    }

    protected virtual async Task<object> GetAsync(string requestString, int httpTimeout, CancellationToken cancellationToken)
    {
      var requestUri = string.Format(this.format, string.Empty, this.token, HttpUtility.UrlEncode(requestString));
      return await this.httpService.GetAsync(this.serviceAddress, requestUri, this.webProxy, httpTimeout, cancellationToken);
    }

    protected virtual async Task<object> PostAsync(string requestString, int httpTimeout, CancellationToken cancellationToken)
    {
      string requestUri = string.Format(this.format, string.Empty, this.token, (int)this.command.OutgoingCompressionType);
      return await this.httpService.PostAsync(this.serviceAddress, requestUri, requestString, this.webProxy, this.command.OutgoingCompressionType, httpTimeout, cancellationToken);
    }

    public virtual object Send(int httpTimeout)
    {
      string xmlRequest = this.CreateXmlRequest();
      if (this.command.HttpMethod == HttpMethod.POST)
      {
        return this.Post(xmlRequest, httpTimeout);
      }
      return this.Get(xmlRequest, httpTimeout);
    }

    public virtual async Task<object> SendAsync(int httpTimeout, CancellationToken cancellationToken)
    {
      string xmlRequest = this.CreateXmlRequest();
      if (this.command.HttpMethod == HttpMethod.POST)
      {
        return await this.PostAsync(xmlRequest, httpTimeout, cancellationToken);
      }
      return await this.GetAsync(xmlRequest, httpTimeout, cancellationToken);
    }
  }
}
