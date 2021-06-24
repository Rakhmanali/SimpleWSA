using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SimpleWSA.Services
{
  public class CompressionService : ICompressionService
  {
    public byte[] Compress(string source, CompressionType compressionType)
    {
      NumberFormatInfo nfi = new CultureInfo(string.Empty, false).NumberFormat;
      nfi.NumberGroupSeparator = " ";

      if (string.IsNullOrEmpty(source) == true)
      {
        return Encoding.UTF8.GetBytes(string.Empty);
      }

      byte[] result = null;

      switch (compressionType)
      {
        case CompressionType.GZip:
          {
            result = GZipCompress(source);
            break;
          }
        case CompressionType.NONE:
        default:
          {
            result = Encoding.UTF8.GetBytes(source);
            break;
          }
      }

      return result;
    }

    public byte[] Decompress(byte[] bytes, CompressionType compressionType)
    {
      NumberFormatInfo nfi = new CultureInfo(String.Empty, false).NumberFormat;
      nfi.NumberGroupSeparator = " ";

      byte[] result = null;

      switch (compressionType)
      {
        case CompressionType.GZip:
          {
            result = GZipDecompress(bytes);
            break;
          }
        case CompressionType.NONE:
        default:
          {
            result = bytes;
            break;
          }
      }

      return result;
    }

    public byte[] Decompress(Stream stream, CompressionType compressionType)
    {
      byte[] result;

      using (MemoryStream memoryStream = new MemoryStream())
      {
        stream.CopyTo(memoryStream);
        if (compressionType != CompressionType.NONE)
        {
          result = this.Decompress(memoryStream.ToArray(), compressionType);
        }
        else
        {
          result = memoryStream.ToArray();
        }
      }

      return result;
    }

    private byte[] GZipCompress(string source)
    {
      if (string.IsNullOrEmpty(source) == true)
      {
        return Encoding.UTF8.GetBytes(string.Empty);
      }

      byte[] result;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
        {
          byte[] bytes = Encoding.UTF8.GetBytes(source);
          gzipStream.Write(bytes, 0, bytes.Length);
        }
        result = memoryStream.ToArray();
      }

      return result;
    }

    private byte[] GZipDecompress(byte[] commpressedBytes)
    {
      using (MemoryStream compressedStream = new MemoryStream(commpressedBytes))
      {
        using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
        {
          using (MemoryStream resultStream = new MemoryStream())
          {
            byte[] buffer = new byte[4096];
            int count = 0;

            while ((count = gzipStream.Read(buffer, 0, buffer.Length)) > 0)
            {
              resultStream.Write(buffer, 0, count);
            }

            return resultStream.ToArray();
          }
        }
      }
    }
  }
}
