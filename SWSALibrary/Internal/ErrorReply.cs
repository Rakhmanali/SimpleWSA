using Newtonsoft.Json;

namespace SimpleWSA.Internal
{
    public sealed class ErrorReply
    {
        public Error Error { get; set; }
    }

    public sealed class Error
    {
        [JsonProperty("function")]
        public string FunctionName { get; set; }

        [JsonProperty("code")]
        public string ErrorCode { get; set; }

        public string Message { get; set; }
    }
}
