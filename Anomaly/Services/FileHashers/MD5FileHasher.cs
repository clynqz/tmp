
using System.Security.Cryptography;
using System.Text;

namespace Anomaly.Services.FileHashers
{
    public class MD5FileHasher : IFileHasher
    {
        public string Hash(Stream fileStream)
        {
            using var md5 = MD5.Create();

            var hashBytes = md5.ComputeHash(fileStream);

            var hashString = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLowerInvariant();

            return hashString;
        }
    }
}
