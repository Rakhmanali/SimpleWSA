using SimpleWSA.Internal;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleWSA
{
  public class Session
  {
    private readonly string connectionProviderAddress;
    public readonly string login;
    public readonly string password;
    public readonly bool isEncrypted;
    public readonly int appId;
    public readonly string appVersion;
    public readonly string domain;
    public readonly WebProxy webProxy;

    public Session(string connectionProviderAddress,
                   string login,
                   string password,
                   bool isEncrypted,
                   int appId,
                   string appVersion,
                   string domain,
                   WebProxy webProxy)
    {
      if (string.IsNullOrEmpty(connectionProviderAddress))
      {
        throw new ArgumentException("the connection privider address is required...");
      }
      this.connectionProviderAddress = connectionProviderAddress;


      this.login = login;
      this.password = password;
      this.isEncrypted = isEncrypted;
      this.appId = appId;
      this.appVersion = appVersion;
      this.domain = domain;
      this.webProxy = webProxy;
    }

    public async Task<string> CreateAsync()
    {
      string restServiceAddress = await GetRestServiceAddressAsync(this.domain, this.connectionProviderAddress, this.webProxy);
      restServiceAddress = new Uri(restServiceAddress).GetLeftPart(UriPartial.Authority);

      string requestUri = $"BufferedMode/Service/{Constants.WS_INITIALIZE_SESSION}";
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

    public static async Task<string> GetRestServiceAddressAsync(string domain, string url, WebProxy webProxy)
    {
      HttpClientHandler httpClientHandler = new HttpClientHandler
      {
        Proxy = webProxy,
        UseProxy = webProxy != null
      };

      string apiUrl = $"/dataaccess/{domain}/restservice/address";
      using (HttpClient httpClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(url) })
      {
        using (HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(apiUrl))
        {
          if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
          {
            return await httpResponseMessage.Content.ReadAsStringAsync();
          }
        }
      }

      return null;
    }
  }
}
