using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SimpleWSA.Exceptions;
using SimpleWSA.Internal;

namespace SimpleWSA
{
  public class Session
  {
    private readonly string login;
    private readonly string password;
    private readonly bool isEncrypted;
    private readonly int appId;
    private readonly string appVersion;
    private readonly string domain;
    private readonly WebProxy webProxy;

    public Session(string login,
                   string password,
                   bool isEncrypted,
                   int appId,
                   string appVersion,
                   string domain,
                   WebProxy webProxy)
    {
      this.login = login;
      this.password = password;
      this.isEncrypted = isEncrypted;
      this.appId = appId;
      this.appVersion = appVersion;
      this.domain = domain;
      this.webProxy = webProxy;
    }

    public async Task<string> CreateByRestServiceAddressAsync(string restServiceAddress)
    {
      string requestUri = $"{SessionContext.route}{Constants.WS_INITIALIZE_SESSION}";
      SessionService sessionService = new SessionService(restServiceAddress,
                                                         requestUri,
                                                         this.login,
                                                         this.password,
                                                         this.isEncrypted,
                                                         this.appId,
                                                         this.appVersion,
                                                         this.domain,
                                                         ErrorCodes.Collection,
                                                         this.webProxy);
      string token = await sessionService.SendAsync(HttpMethod.GET);
      SessionContext.Create(restServiceAddress, this.login, this.password, this.isEncrypted, this.appId, this.appVersion, this.domain, this.webProxy, token);
      return token;
    }

    public async Task<string> CreateByConnectionProviderAddressAsync(string connectionProviderAddress)
    {
      if (connectionProviderAddress == null || connectionProviderAddress.Trim().Length == 0)
      {
        throw new ArgumentException($"{nameof(connectionProviderAddress)} is invalid");
      }

      string restServiceAddress = await GetRestServiceAddressAsync(this.domain, connectionProviderAddress, this.webProxy);
      restServiceAddress = new Uri(restServiceAddress).GetLeftPart(UriPartial.Authority);

      string requestUri = $"{SessionContext.route}{Constants.WS_INITIALIZE_SESSION}";
      SessionService sessionService = new SessionService(restServiceAddress,
                                                         requestUri,
                                                         this.login,
                                                         this.password,
                                                         this.isEncrypted,
                                                         this.appId,
                                                         this.appVersion,
                                                         this.domain,
                                                         ErrorCodes.Collection,
                                                         this.webProxy);
      string token = await sessionService.SendAsync(HttpMethod.GET);
      SessionContext.Create(restServiceAddress, this.login, this.password, this.isEncrypted, this.appId, this.appVersion, this.domain, this.webProxy, token);
      return token;
    }

    public static async Task<string> GetRestServiceAddressAsync(string domain, string connectionProviderAddress, WebProxy webProxy)
    {
      if (domain == null || domain.Trim().Length == 0)
      {
        throw new ArgumentException($"{nameof(domain)} is invalid");
      }

      if (connectionProviderAddress == null || connectionProviderAddress.Trim().Length == 0)
      {
        throw new ArgumentException($"{nameof(connectionProviderAddress)} is invalid");
      }

      HttpClientHandler httpClientHandler = new HttpClientHandler
      {
        Proxy = webProxy,
        UseProxy = webProxy != null
      };

      string apiUrl = $"/dataaccess/{domain}/restservice/address";
      using (HttpClient httpClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(connectionProviderAddress) })
      {
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
  }
}
