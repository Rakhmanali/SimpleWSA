using SimpleWSA.WSALibrary.Internal;
using System.Net;
using System.Threading.Tasks;

namespace SimpleWSA.WSALibrary
{
  public class SessionContext
  {
    public const string route = "/BufferedMode/Service/";

    public string BaseAddress { get; }
    public string Login { get; }
    public string Password { get; }
    public int AppId { get; }
    public string AppVersion { get; }
    public string Domain { get; }
    public WebProxy WebProxy { get; }
    public string Token { get; }

    public SessionContext(string baseAddress,
                          string login,
                          string password,
                          int appId,
                          string appVersion,
                          string domain,
                          WebProxy webProxy,
                          string token)
    {
      BaseAddress = baseAddress;
      Login = login;
      Password = password;
      AppId = appId;
      AppVersion = appVersion;
      Domain = domain;
      WebProxy = webProxy;
      Token = token;
    }

    private static SessionContext sessionContext;

    public void Create()
    {
      sessionContext = this;
    }

    public static SessionContext GetContext()
    {
      return sessionContext;
    }

    public static void Refresh()
    {
      string requestUri = $"{SessionContext.route}{Constants.WS_INITIALIZE_SESSION}";
      SessionContext sessionContext = SessionContext.GetContext();
      SessionService sessionService = new SessionService(sessionContext.BaseAddress,
                                                         requestUri,
                                                         sessionContext.Login,
                                                         sessionContext.Password,
                                                         sessionContext.AppId,
                                                         sessionContext.AppVersion,
                                                         sessionContext.Domain,
                                                         ErrorCodes.Collection,
                                                         sessionContext.WebProxy);
      sessionService.Send(HttpMethod.GET);
    }

    public static async Task RefreshAsync()
    {
      string requestUri = $"{SessionContext.route}{Constants.WS_INITIALIZE_SESSION}";
      SessionContext sessionContext = SessionContext.GetContext();
      SessionService sessionService = new SessionService(sessionContext.BaseAddress,
                                                         requestUri,
                                                         sessionContext.Login,
                                                         sessionContext.Password,
                                                         sessionContext.AppId,
                                                         sessionContext.AppVersion,
                                                         sessionContext.Domain,
                                                         ErrorCodes.Collection,
                                                         sessionContext.WebProxy);
      await sessionService.SendAsync(HttpMethod.GET);
    }
  }
}
