using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWSA.Services
{
  public interface IHttpService
  {
    object Get(string requestUri, WebProxy webProxy, CompressionType returnCompressionType);
    object Post(string requestUri, string requestString, WebProxy webProxy, CompressionType outgoingCompressionType, CompressionType returnCompressionType);
    Task<object> GetAsync(string baseAddress, string requestUri, WebProxy webProxy, CompressionType returnCompressionType);
    Task<object> PostAsync(string baseAddress, string requestUri, string requestString, WebProxy webProxy, CompressionType returnCompressionType);
  }
}
