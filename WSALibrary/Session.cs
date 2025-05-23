using SimpleWSA.WSALibrary.Exceptions;
using SimpleWSA.WSALibrary.Internal;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWSA.WSALibrary
{
  public class Session
  {
    private readonly string login;
    private readonly string password;
    private readonly int appId;
    private readonly string appVersion;
    private readonly string domain;
    private readonly WebProxy webProxy;

    public Session(string login,
                   string password,
                   int appId,
                   string appVersion,
                   string domain,
                   WebProxy webProxy)
    {
      this.login = login;
      this.password = password;
      this.appId = appId;
      this.appVersion = appVersion;
      this.domain = domain;
      this.webProxy = webProxy;
    }

    public async Task<string> CreateByRestServiceAddressAsync(string baseAddress, int httpTimeout, CancellationToken cancellationToken)
    {
      var requestUri = $"/service/{Constants.WS_INITIALIZE_SESSION}";

      SessionService sessionService = new SessionService(baseAddress,
                                                         requestUri,
                                                         this.login,
                                                         this.password,
                                                         this.appId,
                                                         this.appVersion,
                                                         this.domain,
                                                         ErrorCodes.Collection,
                                                         this.webProxy);
      return await sessionService.SendAsync(HttpMethod.GET, httpTimeout, cancellationToken);
    }

    private async Task<string> GetRestServiceAddressAsync(string domain, string connectionProviderAddress, WebProxy webProxy)
    {
      if (domain == null || domain.Trim().Length == 0)
      {
        throw new ArgumentException($"{nameof(domain)} is invalid");
      }

      if (connectionProviderAddress == null || connectionProviderAddress.Trim().Length == 0)
      {
        throw new ArgumentException($"{nameof(connectionProviderAddress)} is invalid");
      }

      HttpClientHandler httpClientHandler = null;
      if (webProxy != null)
      {
        httpClientHandler = new HttpClientHandler
        {
          Proxy = webProxy,
          UseProxy = webProxy != null
        };
      }

      string apiUrl = $"/dataaccess/{domain}/restservice/address";
      using (HttpClient httpClient = httpClientHandler == null ? new HttpClient() : new HttpClient(httpClientHandler))
      {
        httpClient.BaseAddress = new Uri(connectionProviderAddress);

        using (HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(apiUrl))
        {
          if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
          {
            return await httpResponseMessage.Content.ReadAsStringAsync();
          }
          throw new HttpExceptionEx((int)HttpStatusCode.NotFound, $"BaseAddress: {connectionProviderAddress}, apiUrl: {apiUrl}");
        }
      }
    }

    public async Task<string> CreateByConnectionProviderAddressAsync(string connectionProviderAddress, int httpTimeout, CancellationToken cancellationToken)
    {
      if (connectionProviderAddress == null || connectionProviderAddress.Trim().Length == 0)
      {
        throw new ArgumentException($"{nameof(connectionProviderAddress)} is invalid");
      }

      string restServiceAddress = await this.GetRestServiceAddressAsync(this.domain, connectionProviderAddress, this.webProxy);
      restServiceAddress = new Uri(restServiceAddress).GetLeftPart(UriPartial.Authority);

      return await this.CreateByRestServiceAddressAsync(restServiceAddress, httpTimeout, cancellationToken);
    }
  }
}
