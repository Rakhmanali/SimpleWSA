using SimpleWSA.WSALibrary.Exceptions;
using SimpleWSA.WSALibrary.Internal;
using SimpleWSA.WSALibrary.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

    public static string Execute(Command command, RoutineType routineType, int httpTimeout = 100000)
    {
      return Execute(command, routineType, command.HttpMethod, command.ResponseFormat, command.OutgoingCompressionType, command.ReturnCompressionType, httpTimeout);
    }

    public static string Execute(Command command, RoutineType routineType, HttpMethod httpMethod, int httpTimeout = 100000)
    {
      return Execute(command, routineType, httpMethod, command.ResponseFormat, command.OutgoingCompressionType, command.ReturnCompressionType, httpTimeout);
    }

    public static string Execute(Command command, RoutineType routineType, HttpMethod httpMethod, ResponseFormat responseFormat, int httpTimeout = 100000)
    {
      return Execute(command, routineType, httpMethod, responseFormat, command.OutgoingCompressionType, command.ReturnCompressionType, httpTimeout);
    }

    public static string Execute(Command command, RoutineType routineType, HttpMethod httpMethod, ResponseFormat responseFormat, CompressionType outgoingCompressionType, int httpTimeout = 100000)
    {
      return Execute(command, routineType, httpMethod, responseFormat, outgoingCompressionType, command.ReturnCompressionType, httpTimeout);
    }

    public static string Execute(Command command,
                                 RoutineType routineType,
                                 HttpMethod httpMethod,
                                 ResponseFormat responseFormat,
                                 CompressionType outgoingCompressionType,
                                 CompressionType returnCompressionType,
                                 int httpTimeout = 100000)
    {
      command.HttpMethod = httpMethod;
      command.ResponseFormat = responseFormat;
      command.OutgoingCompressionType = outgoingCompressionType;
      command.ReturnCompressionType = returnCompressionType;

      IConvertingService convertingService = new ConvertingService();

      var attempt = 0;
      var sessionContext = SessionContext.GetContext();

      if (routineType == RoutineType.Scalar)
      {
        var scalarRequest = new ScalarRequest(sessionContext.BaseAddress,
                                              SessionContext.route,
                                              sessionContext.Token,
                                              command,
                                              convertingService,
                                              sessionContext.WebProxy);
      scalar_request_label:
        try
        {
          var result = scalarRequest.Send(httpTimeout);
          return Convert.ToString(result);
        }
        catch (Exception ex)
        {
          if (attempt == 0)
          {
            if (DawaException.IsSessionEmptyOrExpired(ex) == true)
            {
              attempt++;
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
        var nonqueryRequest = new NonQueryRequest(sessionContext.BaseAddress,
                                                  SessionContext.route,
                                                  sessionContext.Token,
                                                  command,
                                                  convertingService,
                                                  sessionContext.WebProxy);
      nonquery_request_label:
        try
        {
          var result = nonqueryRequest.Send(httpTimeout);
          return Convert.ToString(result);
        }
        catch (Exception ex)
        {
          if (attempt == 0)
          {
            if (DawaException.IsSessionEmptyOrExpired(ex) == true)
            {
              attempt++;
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
        var dataSetRequest = new DataSetRequest(sessionContext.BaseAddress,
                                                SessionContext.route,
                                                sessionContext.Token,
                                                command,
                                                convertingService,
                                                sessionContext.WebProxy);
      dataset_request_label:
        try
        {
          var result = dataSetRequest.Send(httpTimeout);
          return Convert.ToString(result);
        }
        catch (Exception ex)
        {
          if (attempt == 0)
          {
            if (DawaException.IsSessionEmptyOrExpired(ex) == true)
            {
              attempt++;
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

    public static async Task<string> ExecuteAsync(Command command, RoutineType routineType, int httpTimeout = 100000, CancellationToken cancellationToken = default)
    {
      return await ExecuteAsync(command, routineType, command.HttpMethod, command.ResponseFormat, command.OutgoingCompressionType, command.ReturnCompressionType, httpTimeout, cancellationToken);
    }

    public static async Task<string> ExecuteAsync(Command command, RoutineType routineType, HttpMethod httpMethod, int httpTimeout = 100000, CancellationToken cancellationToken = default)
    {
      return await ExecuteAsync(command, routineType, httpMethod, command.ResponseFormat, command.OutgoingCompressionType, command.ReturnCompressionType, httpTimeout, cancellationToken);
    }

    public static async Task<string> ExecuteAsync(Command command, RoutineType routineType, HttpMethod httpMethod, ResponseFormat responseFormat, int httpTimeout = 100000, CancellationToken cancellationToken = default)
    {
      return await ExecuteAsync(command, routineType, httpMethod, responseFormat, command.OutgoingCompressionType, command.ReturnCompressionType, httpTimeout, cancellationToken);
    }

    public static async Task<string> ExecuteAsync(Command command, RoutineType routineType, HttpMethod httpMethod, ResponseFormat responseFormat, CompressionType outgoingCompressionType, int httpTimeout = 100000, CancellationToken cancellationToken = default)
    {
      return await ExecuteAsync(command, routineType, httpMethod, responseFormat, outgoingCompressionType, command.ReturnCompressionType, httpTimeout, cancellationToken);
    }

    public static async Task<string> ExecuteAsync(Command command,
                                                  RoutineType routineType,
                                                  HttpMethod httpMethod,
                                                  ResponseFormat responseFormat,
                                                  CompressionType outgoingCompressType,
                                                  CompressionType returnCompressionType,
                                                  int httpTimeout = 100000,
                                                  CancellationToken cancellationToken = default)
    {
      command.HttpMethod = httpMethod;
      command.ResponseFormat = responseFormat;
      command.OutgoingCompressionType = outgoingCompressType;
      command.ReturnCompressionType = returnCompressionType;

      ICompressionService compressionService = new CompressionService();
      IConvertingService convertingService = new ConvertingService();

      var attempt = 0;
      var sessionContext = SessionContext.GetContext();

      if (routineType == RoutineType.Scalar)
      {
        var scalarRequest = new ScalarRequest(sessionContext.BaseAddress,
                                              SessionContext.route,
                                              sessionContext.Token,
                                              command,
                                              convertingService,
                                              sessionContext.WebProxy);
      scalar_request_label:
        try
        {
          var result = await scalarRequest.SendAsync(httpTimeout, cancellationToken);
          return Convert.ToString(result);
        }
        catch (Exception ex)
        {
          if (attempt == 0)
          {
            if (DawaException.IsSessionEmptyOrExpired(ex) == true)
            {
              attempt++;
              await SessionContext.RefreshAsync();
              scalarRequest.SetToken(SessionContext.GetContext().Token);
              goto scalar_request_label;
            }
          }
          throw;
        }
      }
      else if (routineType == RoutineType.NonQuery)
      {
        var nonqueryRequest = new NonQueryRequest(sessionContext.BaseAddress,
                                                  SessionContext.route,
                                                  sessionContext.Token,
                                                  command,
                                                  convertingService,
                                                  sessionContext.WebProxy);
      nonquery_request_label:
        try
        {
          var result = await nonqueryRequest.SendAsync(httpTimeout, cancellationToken);
          return Convert.ToString(result);
        }
        catch (Exception ex)
        {
          if (attempt == 0)
          {
            if (DawaException.IsSessionEmptyOrExpired(ex) == true)
            {
              attempt++;
              await SessionContext.RefreshAsync();
              nonqueryRequest.SetToken(SessionContext.GetContext().Token);
              goto nonquery_request_label;
            }
          }
          throw;
        }
      }
      else if (routineType == RoutineType.DataSet)
      {
        var dataSetRequest = new DataSetRequest(sessionContext.BaseAddress,
                                                SessionContext.route,
                                                sessionContext.Token,
                                                command,
                                                convertingService,
                                                sessionContext.WebProxy);
      dataset_request_label:

        try
        {
          var result = await dataSetRequest.SendAsync(httpTimeout, cancellationToken);
          return Convert.ToString(result);
        }
        catch (Exception ex)
        {
          if (attempt == 0)
          {
            if (DawaException.IsSessionEmptyOrExpired(ex) == true)
            {
              attempt++;
              await SessionContext.RefreshAsync();
              dataSetRequest.SetToken(SessionContext.GetContext().Token);
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
                                    ParallelExecution parallelExecution,
                                    int httpTimeout = 100000)
    {
      if (commands == null || commands.Count == 0)
      {
        throw new ArgumentException("commands are required...");
      }

      var postFormat = DataSetRequest.PostFormat;
      if (routineType == RoutineType.Scalar)
      {
        postFormat = ScalarRequest.PostFormat;
      }
      else if (routineType == RoutineType.NonQuery)
      {
        postFormat = NonQueryRequest.PostFormat;
      }

      IConvertingService convertingService = new ConvertingService();

      var stringBuilder = new StringBuilder();
      stringBuilder.Append($"<{Constants.WS_XML_REQUEST_NODE_ROUTINES}>");
      foreach (var command in commands)
      {
        var request = new Request(command, convertingService);
        stringBuilder.Append(request.CreateXmlRoutine());
      }

      CreateRoutinesLevelXmlNodes(stringBuilder, returnCompressionType, parallelExecution, responseFormat);

      stringBuilder.Append($"</{Constants.WS_XML_REQUEST_NODE_ROUTINES}>");
      var requestString = stringBuilder.ToString();

      IHttpService httpService = new HttpService();

      var attempt = 0;
      var sessionContext = SessionContext.GetContext();

    executeall_post_label:
      try
      {
        var requestUri = string.Format(postFormat, sessionContext.BaseAddress, SessionContext.route, sessionContext.Token, (int)outgoingCompressionType);
        return (string)httpService.Post(requestUri, requestString, sessionContext.WebProxy, outgoingCompressionType, httpTimeout);
      }
      catch (Exception ex)
      {
        if (attempt == 0)
        {
          if (DawaException.IsSessionEmptyOrExpired(ex) == true)
          {
            attempt++;
            SessionContext.Refresh();
            goto executeall_post_label;

          }
        }
        throw;
      }
    }

    protected static void CreateRoutinesLevelXmlNodes(StringBuilder stringBuilder,
                                                      CompressionType returnCompressionType,
                                                      ParallelExecution parallelExecution,
                                                      ResponseFormat responseFormat)
    {
      if (returnCompressionType != CompressionType.NONE)
      {
        stringBuilder.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_COMPRESSION, $"{(int)returnCompressionType}"));
      }

      stringBuilder.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_RETURN_TYPE, $"{responseFormat}"));

      if (parallelExecution == ParallelExecution.TRUE)
      {
        stringBuilder.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_PARALLEL_EXECUTION, "1"));
      }
      else
      {
        stringBuilder.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_PARALLEL_EXECUTION, "0"));
      }

      if (responseFormat == ResponseFormat.JSON)
      {
        stringBuilder.Append(CreateXmlNode(Constants.WS_XML_REQUEST_NODE_JSON_DATE_FORMAT, "2"));
      }
    }

    private static string CreateXmlNode(string nodeName, string value)
    {
      return $"<{nodeName}>{value}</{nodeName}>";
    }
  }
}
