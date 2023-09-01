using System.IO;

namespace SimpleWSA.WSALibrary.Services
{
    public interface ICompressionService
    {
        byte[] Compress(string source, CompressionType compressionType);
        byte[] Decompress(byte[] bytes, CompressionType compressionType);
        byte[] Decompress(Stream stream, CompressionType compressionType);
    }
}
