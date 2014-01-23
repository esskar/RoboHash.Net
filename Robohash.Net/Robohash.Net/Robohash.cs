using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using Robohash.Net.Interfaces;
using Robohash.Net.Internals;

namespace Robohash.Net
{
    public class Robohash : RobohashBase<Image>
    {
        private static readonly IRobohashImageFileProvider _imageFileProvider;
        private static readonly IRobohashDigestGenerator _digestGenerator;

        static Robohash()
        {
            _imageFileProvider = new DefaultImageFileProvider();
            _digestGenerator = new DefaultDigestGenerator();
        }

        /// <summary>
        /// Creates a robohash from the given text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static Robohash Create(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Create(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Creates a robohash from the given byte array.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static Robohash Create(byte[] bytes, int offset, int length)
        {
            using (var memory = new MemoryStream(bytes, offset, length))
                return Create(memory);
        }

        /// <summary>
        /// Creates a robohash from the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static Robohash Create(Stream stream)
        {
            var hexDigest = _digestGenerator.GenerateHexDigest(stream);
            return new Robohash(hexDigest, _imageFileProvider);
        }

        public Robohash(string hexDigest, IRobohashImageFileProvider imageFileProvider)
            : base(hexDigest, imageFileProvider) { }


        protected override Image RenderFiles(IEnumerable<string> srcFiles, int srcWidth, int srcHeight, int destWidth, int destHeight)
        {
            var retval = new Bitmap(srcWidth, srcHeight);
            using (var canvas = Graphics.FromImage(retval))
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                foreach (var imageFile in srcFiles)
                {
                    using (var image = Image.FromFile(imageFile))
                        canvas.DrawImage(image, new Rectangle(0, 0, srcWidth, srcHeight),
                            new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                }                
                canvas.Save();
            }

            if (srcWidth != destWidth || srcHeight != destHeight)
            {
                var resizedImage = new Bitmap(retval, destWidth, destHeight);
                retval.Dispose();

                retval = resizedImage;
            }
            
            return retval;
        }
    }
}
