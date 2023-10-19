using System.Net;
using System.Threading.Tasks;

namespace SimpleWSA.WSALibrary.Services
{
  public interface IHttpService
  {
    object Get(string requestUri, WebProxy webProxy, CompressionType returnCompressionType);
    object Post(string requestUri, string requestString, WebProxy webProxy, CompressionType outgoingCompressionType, CompressionType returnCompressionType);
    Task<object> GetAsync(string baseAddress, string requestUri, WebProxy webProxy);
    Task<object> PostAsync(string baseAddress, string requestUri, string requestString, WebProxy webProxy, CompressionType outgoingCompressionType);
  }
}
