using System;
using System.Net;

namespace SimpleWSA.Exceptions
{
  public class HttpExceptionEx : Exception
  {
    private readonly int httpStatusCode;

    public HttpExceptionEx(int httpStatusCode)
    {
      this.httpStatusCode = httpStatusCode;
    }

    public HttpExceptionEx(HttpStatusCode httpStatusCode)
    {
      this.httpStatusCode = (int)httpStatusCode;
    }

    public HttpExceptionEx(int httpStatusCode, string message) : base(message)
    {
      this.httpStatusCode = httpStatusCode;
    }

    public HttpExceptionEx(HttpStatusCode httpStatusCode, string message) : base(message)
    {
      this.httpStatusCode = (int)httpStatusCode;
    }

    public HttpExceptionEx(int httpStatusCode, string message, Exception inner) : base(message, inner)
    {
      this.httpStatusCode = httpStatusCode;
    }

    public HttpExceptionEx(HttpStatusCode httpStatusCode, string message, Exception inner) : base(message, inner)
    {
      this.httpStatusCode = (int)httpStatusCode;
    }

    public int StatusCode { get { return this.httpStatusCode; } }
  }
}
