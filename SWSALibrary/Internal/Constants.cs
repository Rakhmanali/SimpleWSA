namespace SimpleWSA.Internal
{
    public class Constants
    {
        public const string WS_LOGIN = "login";
        public const string WS_PASSWORD = "password";
        public const string WS_IPADDRESS = "ipaddress";
        public const string WS_TIMEOUT = "timeout";
        public const string WS_IS_ENCRYPT = "isEncrypt";
        public const string WS_IS_TOKEN = "isToken";
        public const string WS_RETURN_TYPE = "returnType";
        public const string WS_CONNECTION_STRING = "connectionString";
        public const string WS_APP_ID = "appId";
        public const string WS_APP_VERSION = "appVersion";
        public const string WS_SERVICE_ID = "serviceId";
        public const string WS_DOMAIN = "domain";

        /*
          Used only in the InitializeSession requests and shows that whether a string is encoded
          as a Base64 string
        */
        public const string WS_IS_ENCODED = "isEncoded";

        public const string WS_TOKEN = "token";

        public const string WS_INITIALIZE_SESSION = "InitializeSession";

        public const string WS_XML_REQUEST_NODE_ROUTINES = "_routines";
        public const string WS_XML_REQUEST_NODE_ROUTINE = "_routine";
        public const string WS_XML_REQUEST_NODE_NAME = "_name";
        public const string WS_XML_REQUEST_NODE_ARGUMENTS = "_arguments";
        public const string WS_XML_REQUEST_NODE_OPTIONS = "_options";
        public const string WS_XML_REQUEST_NODE_ENCODING = "_encoding";
        public const string WS_XML_REQUEST_NODE_FROM_CACHE = "_fromCache";
        public const string WS_XML_REQUEST_NODE_WRITE_SCHEMA = "_writeSchema";
        public const string WS_XML_REQUEST_NODE_PARALLEL_EXECUTION = "_parallelExecution";
        public const string WS_XML_REQUEST_NODE_RETURN_TYPE = "_returnType";
        public const string WS_XML_REQUEST_NODE_COMPRESSION = "_compression";
        public const string WS_XML_REQUEST_NODE_ALIAS = "_alias";
        public const string WS_XML_REQUEST_NODE_COMMAND_TIMEOUT = "_commandTimeout";
        public const string WS_XML_REQUEST_NODE_CLEAR_POOL = "_clearPool";
        public const string WS_XML_REQUEST_NODE_JSON_DATE_FORMAT = "_jsonDateFormat";
        public const string WS_XML_REQUEST_NODE_ISOLATION_LEVEL = "_isolationLevel";
        public const string WS_XML_REQUEST_NODE_ROUTINE_TYPE = "_routineType";

        public const string WS_XML_REQUEST_ATTRIBUTE_ENCODING = "encoding";

        public const string WS_XML_REQUEST_NODE_ENCODING_ATTRIBUTE_IS_ENTRY = "isentry";

        public const string HTTP_METHOD_GET = "GET";
        public const string HTTP_METHOD_POST = "POST";

        public const string WS_RETURN_TYPE_JSON = "json";
        public const string WS_RETURN_TYPE_JSONP = "jsonp";
        public const string WS_RETURN_TYPE_XML = "xml";
        public const string WS_RETURN_TYPE_HTML = "html";

        public const string STRING_NULL = "null";
    }
}
