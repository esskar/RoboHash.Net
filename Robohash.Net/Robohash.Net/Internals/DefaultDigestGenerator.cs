using System.IO;
using System.Security.Cryptography;

namespace RoboHash.Net.Internals
{
    internal class DefaultDigestGenerator : RoboHashDigestGeneratorBase
    {
        public override byte[] GenerateDigest(Stream stream)
        {
            using (var sha512 = SHA512.Create())
                return sha512.ComputeHash(stream);
        }
    }
}