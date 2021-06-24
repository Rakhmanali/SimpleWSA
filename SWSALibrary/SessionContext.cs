using SimpleWSA.Internal;
using System.Net;
using System.Threading.Tasks;

namespace SimpleWSA
{
  public class SessionContext
  {
    public static string RestServiceAddress { get; private set; }
    public const string Route = "/BufferedMode/Service/";
    public static string Login { get; private set; }
    public static string Password { get; private set; }
    public static bool IsEncrypted { get; private set; }
    public static int AppId { get; private set; }
    public static string AppVersion { get; private set; }
    public static string Domain { get; private set; }
    public static WebProxy WebProxy { get; private set; }
    public static string Token { get; private set; }

    public static void Create(string restServiceAddress,
                              string login,
                              string password,
                              bool isEncrypted,
                              int appId,
                              string appVersion,
                              string domain,
                              WebProxy webProxy,
                              string token)
    {
      RestServiceAddress = restServiceAddress;
      Login = login;
      Password = password;
      IsEncrypted = isEncrypted;
      AppId = appId;
      AppVersion = appVersion;
      Domain = domain;
      WebProxy = webProxy;
      Token = token;
    }

    public static async Task Refresh()
    {
      string requestUri = $"{SessionContext.Route}{Constants.WS_INITIALIZE_SESSION}";
      SessionService sessionService = new SessionService(RestServiceAddress,
                                                         requestUri,
                                                         Login,
                                                         Password,
                                                         IsEncrypted,
                                                         AppId,
                                                         AppVersion,
                                                         Domain,
                                                         ErrorCodes.Collection,
                                                         WebProxy);
      Token = await sessionService.SendAsync(HttpMethod.GET);
    }
  }
}
