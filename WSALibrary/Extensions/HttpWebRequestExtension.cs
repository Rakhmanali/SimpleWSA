using System.Net;

namespace SimpleWSA.WSALibrary.Extensions
{
  public static class HttpWebRequestExtension
  {
    public static HttpWebRequest InitializeWebRequest(this HttpWebRequest httpWebRequest, CompressionType outgoingCompressionType, byte[] postData, WebProxy webProxy)
    {
      httpWebRequest.ContentType = "application/xml";
      if (outgoingCompressionType == CompressionType.GZip)
      {
        httpWebRequest.Headers.Set("Content-Encoding", "gzip");
      }
      httpWebRequest.Method = nameof(HttpMethod.POST);
      httpWebRequest.ContentLength = postData.Length;
      if (webProxy != null)
      {
        httpWebRequest.Proxy = webProxy;
      }
      httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

      using (var stream = httpWebRequest.GetRequestStream())
      {
        stream.Write(postData, 0, postData.Length);
        stream.Close();
      }

      return httpWebRequest;
    }
  }
}
