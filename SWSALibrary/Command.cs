using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using SimpleWSA.Internal;
using SimpleWSA.Services;

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
        ScalarRequest scalarRequest = new ScalarRequest(SessionContext.RestServiceAddress,
                                                        SessionContext.Route, 
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
        NonQueryRequest nonqueryRequest = new NonQueryRequest(SessionContext.RestServiceAddress,
                                                              SessionContext.Route,
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
        DataSetRequest dataSetRequest = new DataSetRequest(SessionContext.RestServiceAddress,
                                                           SessionContext.Route,
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

    public static async Task<string> ExecuteAsync(Command command,
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
        ScalarRequest scalarRequest = new ScalarRequest(SessionContext.RestServiceAddress,
                                                        SessionContext.Route,
                                                        SessionContext.Token,
                                                        command,
                                                        ErrorCodes.Collection,
                                                        convertingService,
                                                        compressionService,
                                                        SessionContext.WebProxy);
        object result = await scalarRequest.SendAsync();
        return Convert.ToString(result);
      }
      else if (routineType == RoutineType.NonQuery)
      {
        NonQueryRequest nonqueryRequest = new NonQueryRequest(SessionContext.RestServiceAddress,
                                                              SessionContext.Route,
                                                              SessionContext.Token,
                                                              command,
                                                              ErrorCodes.Collection,
                                                              convertingService,
                                                              compressionService,
                                                              SessionContext.WebProxy);
        object result = await nonqueryRequest.SendAsync();
        return Convert.ToString(result);
      }
      else if (routineType == RoutineType.DataSet)
      {
        DataSetRequest dataSetRequest = new DataSetRequest(SessionContext.RestServiceAddress,
                                                           SessionContext.Route,
                                                           SessionContext.Token,
                                                           command,
                                                           ErrorCodes.Collection,
                                                           convertingService,
                                                           compressionService,
                                                           SessionContext.WebProxy);
        object result = await dataSetRequest.SendAsync();
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

      string postFormat = DataSetRequest.PostFormat;
      if (routineType == RoutineType.Scalar)
      {
        postFormat = ScalarRequest.PostFormat;
      }
      else if (routineType == RoutineType.NonQuery)
      {
        postFormat = NonQueryRequest.PostFormat;
      }

      StringBuilder sb = new StringBuilder();
      sb.Append($"<{Constants.WS_XML_REQUEST_NODE_ROUTINES}>");
      foreach (Command command in commands)
      {
        Request request = new Request(command,
                                      ErrorCodes.Collection,
                                      convertingService);
        sb.Append(request.CreateXmlRoutine());
      }

      CreateRoutinesLevelXmlNodes(sb, returnCompressionType, parallelExecution, responseFormat);

      sb.Append($"</{Constants.WS_XML_REQUEST_NODE_ROUTINES}>");
      string requestString = sb.ToString();

      return (string)Request.Post(SessionContext.RestServiceAddress,
                                  SessionContext.Route,  
                                  requestString,
                                  SessionContext.Token,
                                  outgoingCompressionType,
                                  returnCompressionType,
                                  compressionService,
                                  ErrorCodes.Collection,
                                  SessionContext.WebProxy,
                                  postFormat);
    }

    protected static void CreateRoutinesLevelXmlNodes(StringBuilder sb,
                                                    CompressionType returnCompressionType,
                                                    ParallelExecution parallelExecution,
                                                    ResponseFormat responseFormat)
    {
      if (returnCompressionType != CompressionType.NONE)
      {
        sb.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_COMPRESSION, $"{(int)returnCompressionType}"));
      }

      sb.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_RETURN_TYPE, $"{responseFormat}"));

      if (parallelExecution == ParallelExecution.TRUE)
      {
        sb.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_PARALLEL_EXECUTION, "1"));
      }
      else
      {
        sb.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_PARALLEL_EXECUTION, "0"));
      }

      if (responseFormat == ResponseFormat.JSON)
      {
        sb.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_JSON_DATE_FORMAT, "2"));
      }
    }

    private static string CreateXmlNode(string nodeName, string value)
    {
      return $"<{nodeName}>{value}</{nodeName}>";
    }
  }
}
