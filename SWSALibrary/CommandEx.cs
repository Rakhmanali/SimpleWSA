using System;
using System.Collections.Generic;
using System.Text;
using SimpleWSA.Internal;
using SimpleWSA.Services;

namespace SimpleWSA
{
  public class CommandEx : Command
  {
    public CommandEx(string name) : base(name) { }

    public RoutineType RoutineType { get; set; } = RoutineType.DataSet;

    private const string postFormat = "{0}executemixpost?token={1}&compression={2}";

    public static string ExecuteAll(List<CommandEx> commandExs,
                                    ResponseFormat responseFormat = ResponseFormat.JSON,
                                    CompressionType outgoingCompressionType = CompressionType.NONE,
                                    CompressionType returnCompressionType = CompressionType.NONE,
                                    ParallelExecution parallelExecution = ParallelExecution.TRUE)
    {
      if (commandExs == null || commandExs.Count == 0)
      {
        throw new ArgumentException("commands are required...");
      }

      ICompressionService compressionService = new CompressionService();
      IConvertingService convertingService = new ConvertingService();

      StringBuilder sb = new StringBuilder();
      sb.Append($"<{Constants.WS_XML_REQUEST_NODE_ROUTINES}>");
      foreach (CommandEx commandEx in commandExs)
      {
        Request request = new Request(commandEx,
                                      ErrorCodes.Collection,
                                      convertingService);
        sb.Append(request.CreateXmlRoutine(commandEx.RoutineType));
      }

      CreateRoutinesLevelXmlNodes(sb, returnCompressionType, parallelExecution, responseFormat);

      sb.Append($"</{Constants.WS_XML_REQUEST_NODE_ROUTINES}>");
      string requestString = sb.ToString();

      return (string)Request.Post(SessionContext.RestServiceBufferedModeAddress,
                                  requestString,
                                  SessionContext.Token,
                                  outgoingCompressionType,
                                  returnCompressionType,
                                  compressionService,
                                  ErrorCodes.Collection,
                                  SessionContext.WebProxy,
                                  postFormat);
    }
  }
}
