using System;
using System.Collections.Generic;
using System.Text;
using SimpleWSA.Exceptions;
using SimpleWSA.Internal;
using SimpleWSA.Services;

namespace SimpleWSA
{
  public class CommandEx : Command
  {
    public CommandEx(string name) : base(name) { }

    public RoutineType RoutineType { get; set; } = RoutineType.DataSet;

    private const string postFormat = "{0}{1}executemixpost?token={2}&compression={3}";

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

      IConvertingService convertingService = new ConvertingService();

      StringBuilder sb = new StringBuilder();
      sb.Append($"<{Constants.WS_XML_REQUEST_NODE_ROUTINES}>");
      foreach (CommandEx commandEx in commandExs)
      {
        Request request = new Request(commandEx, convertingService);
        sb.Append(request.CreateXmlRoutine(commandEx.RoutineType));
      }

      CreateRoutinesLevelXmlNodes(sb, returnCompressionType, parallelExecution, responseFormat);

      sb.Append($"</{Constants.WS_XML_REQUEST_NODE_ROUTINES}>");
      string requestString = sb.ToString();

      IHttpService httpService = new HttpService();

      executeall_post_label:
      try
      {
        string requestUri = string.Format(postFormat, SessionContext.RestServiceAddress, SessionContext.route, SessionContext.Token, (int)outgoingCompressionType);
        return (string)httpService.Post(requestUri,
                                        requestString,
                                        SessionContext.WebProxy,
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
  }
}
