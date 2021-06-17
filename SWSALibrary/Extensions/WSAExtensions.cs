using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SimpleWSA.Extensions
{
  public static class WSAExtensions
  {
    public static WebRequest InitializeWebRequest(this WebRequest webRequest, CompressionType outgoingCompressionType, byte[] postData, WebProxy webProxy)
    {
      webRequest.ContentType = outgoingCompressionType.SetWebRequestContentType();
      webRequest.Method = HttpMethod.POST.ToString();
      webRequest.ContentLength = postData.Length;
      webRequest.Proxy = webProxy;

      using (Stream postStream = webRequest.GetRequestStream())
      {
        postStream.Write(postData, 0, postData.Length);
        postStream.Close();
      }

      return webRequest;
    }

    public static PgsqlDbType IsPgsqlArray(this PgsqlDbType pgsqlDbType)
    {
      return (pgsqlDbType & PgsqlDbType.Array) == 0 ? pgsqlDbType : (pgsqlDbType & PgsqlDbType.Array);
    }

    public static byte[] SparseArray(this byte[] inArray)
    {
      return inArray.Where(ch => XmlConvert.IsXmlChar(Convert.ToChar(ch))).ToArray();
    }

    public static string SparseString(this string input)
    {
      char[] validXmlChars = input.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();

      return new string(validXmlChars);
    }

    public static XElement ToXElement(this byte[] byteArray, LoadOptions lo)
    {
      XElement xe = null;

      XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
      {
        CheckCharacters = false,
        IgnoreWhitespace = true
      };

      using (MemoryStream memoryStream = new MemoryStream(byteArray))
      {
        using (XmlReader xmlReader = XmlReader.Create(memoryStream, xmlReaderSettings))
        {
          xe = XElement.Load(xmlReader);
        }
      }

      return xe;
    }

    private static string SanitizeXmlString(string xml)
    {
      if (xml == null)
      {
        throw new ArgumentNullException("xml");
      }

      StringBuilder sb = new StringBuilder(xml.Length);

      foreach (char ch in xml)
      {
        if (XmlConvert.IsXmlChar(ch))
        {
          sb.Append(ch);
        }
      }

      //Regex rgx = new Regex(">(?<prefix>.*)&(?<sufix>.*)<");
      //return rgx.Replace(sb.ToString(), ">${prefix}&amp;${sufix}<");
      return sb.ToString().Replace("&", "&amp;");
    }

    private static string SetWebRequestContentType(this CompressionType compressionType)
    {
      return compressionType == CompressionType.NONE ? "text/xml" : "application/octet-stream";
    }
  }
}
