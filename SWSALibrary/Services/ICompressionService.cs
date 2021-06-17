using System.IO;

namespace SimpleWSA.Services
{
    public interface ICompressionService
    {
        byte[] Compress(string source, CompressionType compressionType);
        byte[] Decompress(byte[] bytes, CompressionType compressionType);
        byte[] Decompress(Stream stream, CompressionType compressionType);
    }
}
