using SimpleWSA.WSALibrary.Exceptions;
using SimpleWSA.WSALibrary.Internal;
using SimpleWSA.WSALibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWSA.WSALibrary
{
  public class CommandEx : Command
  {
    public CommandEx(string name) : base(name) { }

    public RoutineType RoutineType { get; set; } = RoutineType.DataSet;

    private const string postFormat = "{0}{1}executemixpost?token={2}&compression={3}";

    public static string ExecuteAll(List<CommandEx> commandExs,
                                    ResponseFormat responseFormat,
                                    CompressionType outgoingCompressionType,
                                    CompressionType returnCompressionType,
                                    ParallelExecution parallelExecution,
                                    int httpTimeout = 100000)
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

      SessionContext sessionContext = SessionContext.GetContext();

    executeall_post_label:
      try
      {
        var requestUri = string.Format(postFormat, sessionContext.BaseAddress, SessionContext.route, sessionContext.Token, (int)outgoingCompressionType);
        return (string)httpService.Post(requestUri, requestString, sessionContext.WebProxy, outgoingCompressionType, httpTimeout);
      }
      catch (Exception ex)
      {
        if (ex is DawaException rex)
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
