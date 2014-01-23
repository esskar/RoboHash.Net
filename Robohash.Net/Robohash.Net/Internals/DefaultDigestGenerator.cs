using System.IO;
using System.Security.Cryptography;

namespace Robohash.Net.Internals
{
    internal class DefaultDigestGenerator : RobohashDigestGeneratorBase
    {
        public override byte[] GenerateDigest(Stream stream)
        {
            using (var sha512 = SHA512.Create())
                return sha512.ComputeHash(stream);
        }
    }
}