using System.Collections.Generic;

namespace SimpleWSA
{
  internal class ErrorCodes
  {
    internal readonly static Dictionary<string, string> Collection = new Dictionary<string, string>
    {
      ["MI001"] = "Service function name is empty",
      ["MI002"] = "Login or password is empty or wrong",
      ["MI003"] = "Credentials format error. Use one from - token or login/password for specifying credentials",
      ["MI004"] = "Login/password or token is empty or wrong",
      ["MI005"] = "Token is empty or wrong",
      ["MI006"] = "Service function name is wrong",
      ["MI007"] = "Token is empty",
      ["MI008"] = "Session has expired",
      ["MI009"] = "Using token by other device",
      ["MI010"] = "Login or password is empty",
      ["MI011"] = "Unable to login. Please, check the login and the password",
      ["MI012"] = "Error in loading of verification fields",
      ["MI013"] = "Can't get the connection, login or password is wrong",
      ["MI014"] = "Can't login, login or password is wrong",
      ["MI015"] = "System exception",
      ["MI016"] = "Restriction, you can not use the value for parameter in the given request",
      ["MI017"] = "Restricted function, you can not use this finction ",
      ["MI018"] = "Query string is empty or wrong",
      ["MI019"] = "Query string is not well format",
      ["MI020"] = "Parsing Query string as a XML error",
      ["MI021"] = "SESSION is empty for given token",
      ["MI022"] = "SESSION is empty for given login",
      ["MI023"] = "This account has been temporary blocked due to too many tries. The account will be automatically unblocked after few minutes",
      ["MI024"] = "This account has been temporary suspended due to too many attempts of the function calling with wrong parameters or calling function which doesn't exist",
      ["MI025"] = "Cannot serialize the Session object to JSON",
      ["MI026"] = "Unable to login. You cannot login with your current IP address",
      ["MI027"] = "There is no one session",
      ["MI028"] = "Cannot specify the file name",
      ["MI029"] = "The file does not exist",
      ["MI030"] = "Cannot specify the Large Object Id",
      ["MI031"] = "Not valid Large Object Id",
      ["MI032"] = "The local storage has not have the connection string for the given domain",
      ["MI033"] = "Cannot get the domain from the InitializeSession request",
      ["MI034"] = "Unable to start the work. Please, check the company name",

      ["EX001"] = "System exception",
      ["EX002"] = "PostgreSql function is not return set function",
      ["EX003"] = "PostgreSql function is return set function",
      ["EX004"] = "Query string is not well format",
      ["EX005"] = "RoutineType key is not specified or has a wrong value",
      ["EX006"] = "Restriction, you can not use the value for parameter in the given request",
      ["IS001"] = "InitilizeSession POST contains bad XML request"
    };
  }
}
