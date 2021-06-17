using System;

namespace SimpleWSA
{
  public class RestServiceException : Exception
  {
    public RestServiceException(string message, string code, string originalMessage) : base(message)
    {
      this.Code = code;
      this.OriginalMessage = originalMessage;
    }

    public string OriginalMessage { get; }
    public string Code { get; }

    public static bool IsSessionEmptyOrExpired(Exception ex)
    {
      if (ex is RestServiceException restServiceException)
      {
        return string.Compare(restServiceException.Code, "MI008", true) == 0 ||
               string.Compare(restServiceException.Code, "MI005", true) == 0;
      }

      return false;
    }
  }
}
