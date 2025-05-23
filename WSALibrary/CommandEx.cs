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

    private const string postFormat = "{0}/data/execute/all?token={1}&compression={2}";

    // TO DO: create async kind of this function
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

      var stringBuilder = new StringBuilder();
      stringBuilder.Append($"<{Constants.WS_XML_REQUEST_NODE_ROUTINES}>");
      foreach (CommandEx commandEx in commandExs)
      {
        var request = new Request(commandEx, convertingService);
        stringBuilder.Append(request.CreateXmlRoutine(commandEx.RoutineType));
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
        var requestUri = string.Format(postFormat, sessionContext.BaseAddress, sessionContext.Token, (int)outgoingCompressionType);
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

  }
}
