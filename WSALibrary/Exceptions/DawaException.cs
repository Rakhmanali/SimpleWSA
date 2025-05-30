﻿using System;

namespace SimpleWSA.WSALibrary.Exceptions
{
  public class DawaException : Exception
  {
    public DawaException(string message, string code, string originalMessage) : base(message)
    {
      this.Code = code;
      this.OriginalMessage = originalMessage;
    }

    public string OriginalMessage { get; }
    public string Code { get; }

    public static bool IsSessionEmptyOrExpired(Exception ex)
    {
      if (ex is DawaException restServiceException)
      {
        return string.Compare(restServiceException.Code, "MI008", true) == 0 ||
               string.Compare(restServiceException.Code, "MI005", true) == 0;
      }

      return false;
    }
  }
}
