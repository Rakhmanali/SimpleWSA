using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWSA.WSALibrary.Services
{
  public interface IHttpService
  {
    object Get(string requestUri, WebProxy webProxy, int httpTimeout);
    object Post(string requestUri, string requestString, WebProxy webProxy, CompressionType outgoingCompressionType, int httpTimeout);
    Task<object> GetAsync(string baseAddress, string requestUri, WebProxy webProxy, int httpTimeout, CancellationToken cancellationToken);
    Task<object> PostAsync(string baseAddress, string requestUri, string requestString, WebProxy webProxy, CompressionType outgoingCompressionType, int httpTimeout, CancellationToken cancellationToken);
  }
}
