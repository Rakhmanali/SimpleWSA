﻿using SimpleWSA.WSALibrary.Services;
using System.Net;

namespace SimpleWSA.WSALibrary
{
  public sealed class NonQueryRequest : Request
  {
    private const string getFormat = "{0}/data/execute?token={1}&value={2}&routineType=3";
    private const string postFormat = "{0}/data/execute?token={1}&compression={2}&routineType=3";

    public NonQueryRequest(string serviceAddress,
                           string token,
                           Command command,
                           IConvertingService convertingService,
                           WebProxy webProxy) : base(serviceAddress,
                                                     token,
                                                     command,
                                                     convertingService,
                                                     webProxy,
                                                     command.HttpMethod == HttpMethod.GET ? getFormat : postFormat)
    { }

    public static string PostFormat
    {
      get
      {
        return postFormat;
      }
    }
  }
}
