using SimpleWSA.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

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

    public EncodingType OutgoingEncodingType { get; set; } = EncodingType.NONE;
    public CompressionType OutgoingCompressionType { get; set; } = CompressionType.NONE;
    public EncodingType ReturnEncodingType { get; set; } = EncodingType.NONE;
    public CompressionType ReturnCompressionType { get; set; } = CompressionType.NONE;

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
                                 HttpMethod httpMethod= HttpMethod.GET,
                                 ResponseFormat responseFormat = ResponseFormat.JSON)
    {
      command.HttpMethod = httpMethod;
      command.ResponseFormat = responseFormat;

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
                                    HttpMethod httpMethod = HttpMethod.GET,
                                    ResponseFormat responseFormat = ResponseFormat.JSON)
    {
      ICompressionService compressionService = new CompressionService();
      IConvertingService convertingService = new ConvertingService();

      if (routineType == RoutineType.Scalar)
      {
        foreach(Command command in commands)
        {
          ScalarRequest scalarRequest = new ScalarRequest(SessionContext.RestServiceBufferedModeAddress,
                                                        SessionContext.Token,
                                                        command,
                                                        ErrorCodes.Collection,
                                                        convertingService,
                                                        compressionService,
                                                        SessionContext.WebProxy);
        }
      }
        return null;
    }
  }
}
