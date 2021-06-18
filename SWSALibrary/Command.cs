using SimpleWSA.Internal;
using SimpleWSA.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace SimpleWSA
{
  public class Command
  {
    public string Name { get; set; }

    private readonly ParameterCollection parameters = new ParameterCollection();
    public ParameterCollection Parameters
    {
      get
      {
        return parameters;
      }
    }

    public HttpMethod HttpMethod { get; private set; } = HttpMethod.GET;
    public ResponseFormat ResponseFormat { get; private set; } = ResponseFormat.JSON;
    public CompressionType OutgoingCompressionType { get; private set; } = CompressionType.NONE;
    public CompressionType ReturnCompressionType { get; private set; } = CompressionType.NONE;

    // specifies whether being encoded every the text-based data during creating XML request,
    // works in the routine level boundary
    public EncodingType OutgoingEncodingType { get; set; } = EncodingType.NONE;

    // specifies whether the server will encode the text-based data during preparing a response
    // works in the routine level boundary
    public EncodingType ReturnEncodingType { get; set; } = EncodingType.NONE;

    public WriteSchema WriteSchema { get; set; } = WriteSchema.FALSE;
    public GetFromCache GetFromCache { get; set; } = GetFromCache.FALSE;
    public ClearPool ClearPool { get; set; } = ClearPool.FALSE;
    public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

    private int timeout;
    [DefaultValue(20)]
    public int CommandTimeout
    {
      get
      {
        return timeout;
      }

      set
      {
        if (value < 0)
        {
          throw new ArgumentOutOfRangeException("value", "CommandTimeout can't be less than zero.");
        }

        timeout = value;
      }
    }

    public Command(string name)
    {
      this.Name = name;
    }

    public static string Execute(Command command,
                                 RoutineType routineType,
                                 HttpMethod httpMethod = HttpMethod.GET,
                                 ResponseFormat responseFormat = ResponseFormat.JSON,
                                 CompressionType outgoingCompressType = CompressionType.NONE,
                                 CompressionType returnCompressionType = CompressionType.NONE)
    {
      command.HttpMethod = httpMethod;
      command.ResponseFormat = responseFormat;
      command.OutgoingCompressionType = outgoingCompressType;
      command.ReturnCompressionType = returnCompressionType;

      ICompressionService compressionService = new CompressionService();
      IConvertingService convertingService = new ConvertingService();

      if (routineType == RoutineType.Scalar)
      {
        ScalarRequest scalarRequest = new ScalarRequest(SessionContext.RestServiceBufferedModeAddress,
                                                        SessionContext.Token,
                                                        command,
                                                        ErrorCodes.Collection,
                                                        convertingService,
                                                        compressionService,
                                                        SessionContext.WebProxy);
        object result = scalarRequest.Send();
        return Convert.ToString(result);
      }
      else if (routineType == RoutineType.NonQuery)
      {
        NonQueryRequest nonqueryRequest = new NonQueryRequest(SessionContext.RestServiceBufferedModeAddress,
                                                              SessionContext.Token,
                                                              command,
                                                              ErrorCodes.Collection,
                                                              convertingService,
                                                              compressionService,
                                                              SessionContext.WebProxy);
        object result = nonqueryRequest.Send();
        return Convert.ToString(result);
      }
      else if (routineType == RoutineType.DataSet)
      {
        DataSetRequest dataSetRequest = new DataSetRequest(SessionContext.RestServiceBufferedModeAddress,
                                                           SessionContext.Token,
                                                           command,
                                                           ErrorCodes.Collection,
                                                           convertingService,
                                                           compressionService,
                                                           SessionContext.WebProxy);
        object result = dataSetRequest.Send();
        return Convert.ToString(result);
      }

      return null;
    }

    public static string ExecuteAll(List<Command> commands,
                                    RoutineType routineType,
                                    ResponseFormat responseFormat = ResponseFormat.JSON,
                                    CompressionType outgoingCompressionType = CompressionType.NONE,
                                    CompressionType returnCompressionType = CompressionType.NONE,
                                    ParallelExecution parallelExecution = ParallelExecution.TRUE)
    {
      if (commands == null || commands.Count == 0)
      {
        throw new ArgumentException("commands are required...");
      }

      ICompressionService compressionService = new CompressionService();
      IConvertingService convertingService = new ConvertingService();

      if (routineType == RoutineType.Scalar)
      {
        StringBuilder sb = new StringBuilder();
        sb.Append($"<{Constants.WS_XML_REQUEST_NODE_ROUTINES}>");
        foreach (Command command in commands)
        {
          ScalarRequest scalarRequest = new ScalarRequest(SessionContext.RestServiceBufferedModeAddress,
                                            SessionContext.Token,
                                            command,
                                            ErrorCodes.Collection,
                                            convertingService,
                                            compressionService,
                                            SessionContext.WebProxy);
          sb.Append(scalarRequest.CreateXmlRoutine());
        }

        if (returnCompressionType != CompressionType.NONE)
        {
          //sb.Append($"<{Constants.WS_XML_REQUEST_NODE_COMPRESSION}>{((int)returnCompressionType)}</{Constants.WS_XML_REQUEST_NODE_COMPRESSION}>");
          sb.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_COMPRESSION, $"{(int)returnCompressionType}"));
        }

        //sb.Append($"<{Constants.WS_XML_REQUEST_NODE_RETURN_TYPE}>{responseFormat}</{Constants.WS_XML_REQUEST_NODE_RETURN_TYPE}>");
        sb.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_RETURN_TYPE,$"{responseFormat}"));

        if (parallelExecution == ParallelExecution.TRUE)
        {
          //sb.Append($"<{Constants.WS_XML_REQUEST_NODE_PARALLEL_EXECUTION}>1</{Constants.WS_XML_REQUEST_NODE_PARALLEL_EXECUTION}>");
          sb.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_PARALLEL_EXECUTION, "1"));
        }
        else
        {
          //sb.Append($"<{Constants.WS_XML_REQUEST_NODE_PARALLEL_EXECUTION}>0</{Constants.WS_XML_REQUEST_NODE_PARALLEL_EXECUTION}>");
          sb.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_PARALLEL_EXECUTION, "0"));
        }

        if (responseFormat == ResponseFormat.JSON)
        {
          //sb.Append($"<{Constants.WS_XML_REQUEST_NODE_JSON_DATE_FORMAT}>2</{Constants.WS_XML_REQUEST_NODE_JSON_DATE_FORMAT}>");
          sb.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_JSON_DATE_FORMAT, "2"));
        }

        sb.Append($"</{Constants.WS_XML_REQUEST_NODE_ROUTINES}>");
        string requestString = sb.ToString();

        return (string)ScalarRequest.Post(SessionContext.RestServiceBufferedModeAddress,
                                          requestString,
                                          SessionContext.Token,
                                          outgoingCompressionType,
                                          returnCompressionType,
                                          compressionService,
                                          ErrorCodes.Collection,
                                          SessionContext.WebProxy,
                                          ScalarRequest.postFormat);
      }
      else if (routineType == RoutineType.NonQuery)
      {
        StringBuilder sb = new StringBuilder();
        sb.Append($"<{Constants.WS_XML_REQUEST_NODE_ROUTINES}>");
        foreach (Command command in commands)
        {
          NonQueryRequest nonQueryRequest = new NonQueryRequest(SessionContext.RestServiceBufferedModeAddress,
                                                                SessionContext.Token,
                                                                command,
                                                                ErrorCodes.Collection,
                                                                convertingService,
                                                                compressionService,
                                                                SessionContext.WebProxy);
          sb.Append(nonQueryRequest.CreateXmlRoutine());
        }

        if (returnCompressionType != CompressionType.NONE)
        {
          //sb.Append($"<{Constants.WS_XML_REQUEST_NODE_COMPRESSION}>{((int)returnCompressionType)}</{Constants.WS_XML_REQUEST_NODE_COMPRESSION}>");
          sb.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_COMPRESSION, $"{(int)returnCompressionType}"));
        }

        //sb.Append($"<{Constants.WS_XML_REQUEST_NODE_RETURN_TYPE}>{responseFormat}</{Constants.WS_XML_REQUEST_NODE_RETURN_TYPE}>");
        sb.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_RETURN_TYPE, $"{responseFormat}"));

        if (parallelExecution == ParallelExecution.TRUE)
        {
          //sb.Append($"<{Constants.WS_XML_REQUEST_NODE_PARALLEL_EXECUTION}>1</{Constants.WS_XML_REQUEST_NODE_PARALLEL_EXECUTION}>");
          sb.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_PARALLEL_EXECUTION, "1"));
        }
        else
        {
          //sb.Append($"<{Constants.WS_XML_REQUEST_NODE_PARALLEL_EXECUTION}>0</{Constants.WS_XML_REQUEST_NODE_PARALLEL_EXECUTION}>");
          sb.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_PARALLEL_EXECUTION, "0"));
        }

        if (responseFormat == ResponseFormat.JSON)
        {
          //sb.Append($"<{Constants.WS_XML_REQUEST_NODE_JSON_DATE_FORMAT}>2</{Constants.WS_XML_REQUEST_NODE_JSON_DATE_FORMAT}>");
          sb.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_JSON_DATE_FORMAT, "2"));
        }

        sb.Append($"</{Constants.WS_XML_REQUEST_NODE_ROUTINES}>");
        string requestString = sb.ToString();

        return (string)NonQueryRequest.Post(SessionContext.RestServiceBufferedModeAddress,
                                            requestString,
                                            SessionContext.Token,
                                            outgoingCompressionType,
                                            returnCompressionType,
                                            compressionService,
                                            ErrorCodes.Collection,
                                            SessionContext.WebProxy,
                                            NonQueryRequest.postFormat);
      }
      return null;
    }

    private static string CreateXmlNode(string nodeName, string value)
    {
      return $"<{nodeName}>{value}</{nodeName}>";
    }
  }
}
