using System.IO;

namespace RoboHashDotNet.Interfaces
{
    public interface IRoboHashDigestGenerator
    {
        /// <summary>
        /// Generates the digest from the given stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        byte[] GenerateDigest(Stream stream);

        /// <summary>
        /// Generates the digest from the given stream and encodes it to a hexadecimal string.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        string GenerateHexDigest(Stream stream);
    }
}