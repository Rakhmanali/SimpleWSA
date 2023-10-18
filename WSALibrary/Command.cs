using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using SimpleWSA.WSALibrary.Exceptions;
using SimpleWSA.WSALibrary.Internal;
using SimpleWSA.WSALibrary.Services;

namespace SimpleWSA.WSALibrary
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

    public HttpMethod HttpMethod { get; set; } = HttpMethod.GET;
    public ResponseFormat ResponseFormat { get; set; } = ResponseFormat.JSON;
    public CompressionType OutgoingCompressionType { get; set; } = CompressionType.NONE;
    public CompressionType ReturnCompressionType { get; set; } = CompressionType.NONE;

    // specifies whether being encoded every the text-based data during creating XML request,
    // works in the routine level boundary
    public EncodingType OutgoingEncodingType { get; set; } = EncodingType.NONE;

    // specifies whether the server will encode the text-based data during preparing a response
    // works in the routine level boundary
    public EncodingType ReturnEncodingType { get; set; } = EncodingType.NONE;

    public WriteSchema WriteSchema { get; set; } = WriteSchema.FALSE;
    public FromCache GetFromCache { get; set; } = FromCache.FALSE;
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

    public static string Execute(Command command, RoutineType routineType)
    {
      return Execute(command, routineType, command.HttpMethod, command.ResponseFormat, command.OutgoingCompressionType, command.ReturnCompressionType);
    }

    public static string Execute(Command command, RoutineType routineType, HttpMethod httpMethod)
    {
      return Execute(command, routineType, httpMethod, command.ResponseFormat, command.OutgoingCompressionType, command.ReturnCompressionType);
    }

    public static string Execute(Command command, RoutineType routineType, HttpMethod httpMethod, ResponseFormat responseFormat)
    {
      return Execute(command, routineType, httpMethod, responseFormat, command.OutgoingCompressionType, command.ReturnCompressionType);
    }

    public static string Execute(Command command, RoutineType routineType, HttpMethod httpMethod, ResponseFormat responseFormat, CompressionType outgoingCompressionType)
    {
      return Execute(command, routineType, httpMethod, responseFormat, outgoingCompressionType, command.ReturnCompressionType);
    }

    public static string Execute(Command command,
                                 RoutineType routineType,
                                 HttpMethod httpMethod,
                                 ResponseFormat responseFormat,
                                 CompressionType outgoingCompressionType,
                                 CompressionType returnCompressionType)
    {
      command.HttpMethod = httpMethod;
      command.ResponseFormat = responseFormat;
      command.OutgoingCompressionType = outgoingCompressionType;
      command.ReturnCompressionType = returnCompressionType;

      IConvertingService convertingService = new ConvertingService();

      SessionContext sessionContext = SessionContext.GetContext();

      if (routineType == RoutineType.Scalar)
      {
        ScalarRequest scalarRequest = new ScalarRequest(sessionContext.BaseAddress,
                                                        SessionContext.route,
                                                        sessionContext.Token,
                                                        command,
                                                        convertingService,
                                                        sessionContext.WebProxy);
      scalar_request_label:
        try
        {
          object result = scalarRequest.Send();
          return Convert.ToString(result);
        }
        catch (Exception ex)
        {
          if (ex is RestServiceException rex)
          {
            // keep session alive
            if (rex.Code == "MI008")
            {
              SessionContext.Refresh();
              scalarRequest.SetToken(SessionContext.GetContext().Token);
              goto scalar_request_label;
            }
          }
          throw;
        }
      }
      else if (routineType == RoutineType.NonQuery)
      {
        NonQueryRequest nonqueryRequest = new NonQueryRequest(sessionContext.BaseAddress,
                                                              SessionContext.route,
                                                              sessionContext.Token,
                                                              command,
                                                              convertingService,
                                                              sessionContext.WebProxy);
      nonquery_request_label:
        try
        {
          object result = nonqueryRequest.Send();
          return Convert.ToString(result);
        }
        catch (Exception ex)
        {
          if (ex is RestServiceException rex)
          {
            // keep session alive
            if (rex.Code == "MI008")
            {
              SessionContext.Refresh();
              nonqueryRequest.SetToken(SessionContext.GetContext().Token);
              goto nonquery_request_label;
            }
          }
          throw;
        }
      }
      else if (routineType == RoutineType.DataSet)
      {
        DataSetRequest dataSetRequest = new DataSetRequest(sessionContext.BaseAddress,
                                                           SessionContext.route,
                                                           sessionContext.Token,
                                                           command,
                                                           convertingService,
                                                           sessionContext.WebProxy);
      dataset_request_label:
        try
        {
          object result = dataSetRequest.Send();
          return Convert.ToString(result);
        }
        catch (Exception ex)
        {
          if (ex is RestServiceException rex)
          {
            // keep session alive
            if (rex.Code == "MI008")
            {
              SessionContext.Refresh();
              dataSetRequest.SetToken(SessionContext.GetContext().Token);
              goto dataset_request_label;
            }
          }
          throw;
        }
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

      SessionContext sessionContext = SessionContext.GetContext();

      if (routineType == RoutineType.Scalar)
      {
        ScalarRequest scalarRequest = new ScalarRequest(sessionContext.BaseAddress,
                                                        SessionContext.route,
                                                        sessionContext.Token,
                                                        command,
                                                        convertingService,
                                                        sessionContext.WebProxy);
      scalar_request_label:
        try
        {
          object result = await scalarRequest.SendAsync();
          return Convert.ToString(result);
        }
        catch (Exception ex)
        {
          if (ex is RestServiceException rex)
          {
            // keep session alive
            if (rex.Code == "MI008")
            {
              await SessionContext.RefreshAsync();
              scalarRequest.SetToken(sessionContext.Token);
              goto scalar_request_label;
            }
          }
          throw;
        }
      }
      else if (routineType == RoutineType.NonQuery)
      {
        NonQueryRequest nonqueryRequest = new NonQueryRequest(sessionContext.BaseAddress,
                                                              SessionContext.route,
                                                              sessionContext.Token,
                                                              command,
                                                              convertingService,
                                                              sessionContext.WebProxy);
      nonquery_request_label:
        try
        {
          object result = await nonqueryRequest.SendAsync();
          return Convert.ToString(result);
        }
        catch (Exception ex)
        {
          if (ex is RestServiceException rex)
          {
            // keep session alive
            if (rex.Code == "MI008")
            {
              await SessionContext.RefreshAsync();
              nonqueryRequest.SetToken(sessionContext.Token);
              goto nonquery_request_label;
            }
          }
          throw;
        }
      }
      else if (routineType == RoutineType.DataSet)
      {
        DataSetRequest dataSetRequest = new DataSetRequest(sessionContext.BaseAddress,
                                                           SessionContext.route,
                                                           sessionContext.Token,
                                                           command,
                                                           convertingService,
                                                           sessionContext.WebProxy);
      dataset_request_label:
        try
        {
          object result = await dataSetRequest.SendAsync();
          return Convert.ToString(result);
        }
        catch (Exception ex)
        {
          if (ex is RestServiceException rex)
          {
            // keep session alive
            if (rex.Code == "MI008")
            {
              await SessionContext.RefreshAsync();
              dataSetRequest.SetToken(sessionContext.Token);
              goto dataset_request_label;
            }
          }
          throw;
        }
      }

      return null;
    }

    public static string ExecuteAll(List<Command> commands,
                                    RoutineType routineType,
                                    ResponseFormat responseFormat,
                                    CompressionType outgoingCompressionType,
                                    CompressionType returnCompressionType,
                                    ParallelExecution parallelExecution)
    {
      if (commands == null || commands.Count == 0)
      {
        throw new ArgumentException("commands are required...");
      }

      string postFormat = DataSetRequest.PostFormat;
      if (routineType == RoutineType.Scalar)
      {
        postFormat = ScalarRequest.PostFormat;
      }
      else if (routineType == RoutineType.NonQuery)
      {
        postFormat = NonQueryRequest.PostFormat;
      }

      IConvertingService convertingService = new ConvertingService();

      StringBuilder sb = new StringBuilder();
      sb.Append($"<{Constants.WS_XML_REQUEST_NODE_ROUTINES}>");
      foreach (Command command in commands)
      {
        Request request = new Request(command, convertingService);
        sb.Append(request.CreateXmlRoutine());
      }

      CreateRoutinesLevelXmlNodes(sb, returnCompressionType, parallelExecution, responseFormat);

      sb.Append($"</{Constants.WS_XML_REQUEST_NODE_ROUTINES}>");
      string requestString = sb.ToString();

      Console.WriteLine(requestString);

      IHttpService httpService = new HttpService();

      SessionContext sessionContext = SessionContext.GetContext();

    executeall_post_label:
      try
      {
        string requestUri = string.Format(postFormat, sessionContext.BaseAddress, SessionContext.route, sessionContext.Token, (int)outgoingCompressionType);
        return (string)httpService.Post(requestUri,
                                        requestString,
                                        sessionContext.WebProxy,
                                        outgoingCompressionType,
                                        returnCompressionType);
      }
      catch (Exception ex)
      {
        if (ex is RestServiceException rex)
        {
          // keep session alive
          if (rex.Code == "MI008")
          {
            SessionContext.Refresh();
            goto executeall_post_label;
          }
        }
        throw;
      }
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
