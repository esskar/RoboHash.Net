using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using RoboHash.Net.Interfaces;
using RoboHash.Net.Internals;

namespace RoboHash.Net
{
    public class RoboArmadaHash : IRoboHash<Image>
    {
        private static IRoboHashImageFileProvider _imageFileProvider;
        private static IRoboHashDigestGenerator _digestGenerator;

        public static IRoboHashImageFileProvider ImageFileProvider
        {
            get { return _imageFileProvider ?? (_imageFileProvider = new DefaultImageFileProvider()); }
            set { _imageFileProvider = value; }
        }

        public static IRoboHashDigestGenerator DigestGenerator
        {
            get { return _digestGenerator ?? (_digestGenerator = new DefaultDigestGenerator()); }
            set { _digestGenerator = value; }
        }

        /// <summary>
        /// Creates a robohash from the given text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static RoboArmadaHash Create(string text)
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
        public static RoboArmadaHash Create(byte[] bytes, int offset, int length)
        {
            using (var memory = new MemoryStream(bytes, offset, length))
                return Create(memory);
        }

        /// <summary>
        /// Creates a robohash from the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static RoboArmadaHash Create(Stream stream)
        {
            var hexDigest1 = RoboArmadaHash.DigestGenerator.GenerateHexDigest(stream);
            var hexDigest2 = GenerateSubDigest(hexDigest1);
            var hexDigest3 = GenerateSubDigest(hexDigest2);

            return new RoboArmadaHash(hexDigest1, hexDigest2, hexDigest3);
        }


        private static string GenerateSubDigest(string digest)
        {
            var bytes = Encoding.UTF8.GetBytes(digest);
            using (var memory = new MemoryStream(bytes, 0, bytes.Length))
                return RoboArmadaHash.DigestGenerator.GenerateHexDigest(memory);
        }
        private const double Factor = .90;

        private readonly string _hexDigest1;
        private readonly string _hexDigest2;
        private readonly string _hexDigest3;

        public RoboArmadaHash(string hexDigest1, string hexDigest2, string hexDigest3)
        {
            if (string.IsNullOrEmpty(hexDigest1))
                throw new ArgumentNullException("hexDigest1");
            if (string.IsNullOrEmpty(hexDigest2))
                throw new ArgumentNullException("hexDigest2");
            if (string.IsNullOrEmpty(hexDigest3))
                throw new ArgumentNullException("hexDigest3");

            _hexDigest1 = hexDigest1;
            _hexDigest2 = hexDigest2;
            _hexDigest3 = hexDigest3;
        }

        public Image Render(int width = 400, int height = 400)
        {
            return this.Render(null, null, null, width, height);
        }

        public Image Render(string set, string backgroundSet, string color, int width, int height)
        {
            Image image1 = null, image2 = null, image3 = null;
            try
            {
                var facHeight = (int)(height * Factor);
                var facWidth = (int)(width * Factor);


                image1 = RenderOne(_hexDigest1, width, height);
                image2 = RenderOne(_hexDigest2, facWidth, facHeight);
                image3 = RenderOne(_hexDigest3, facWidth, facHeight);

                var robo = RoboHash.Create(Xor(Xor(_hexDigest1, _hexDigest2), _hexDigest3));
                var backgroundImageName = robo.GetBackgroundImageFileName(backgroundSet);

                var retval = new Bitmap(width, height);
                try
                {
                    using (var canvas = Graphics.FromImage(retval))
                    {
                        canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        if (!string.IsNullOrWhiteSpace(backgroundImageName))
                        {
                            using (var image = Image.FromFile(backgroundImageName))
                            {
                                canvas.DrawImage(image, new Rectangle(0, 0, width, height),
                                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                            }
                        }

                        var x = (width - width) / 2;
                        var y = (height - height) / 2;

                        var left = (int)(width / 4.0 - width * 0.05);
                        var right = (int)(width / 4.0 + width * 0.05);

                        var offHeight = height - facHeight;

                        canvas.DrawImage(image2, new Rectangle(x - left, y + offHeight, image2.Width, image2.Height),
                            new Rectangle(0, 0, image2.Width, image2.Height), GraphicsUnit.Pixel);

                        canvas.DrawImage(image3, new Rectangle(x + right, y + offHeight, image3.Width, image3.Height),
                            new Rectangle(0, 0, image3.Width, image3.Height), GraphicsUnit.Pixel);

                        canvas.DrawImage(image1, new Rectangle(x, y, image1.Width, image1.Height),
                            new Rectangle(0, 0, image1.Width, image1.Height), GraphicsUnit.Pixel);
                    }
                }
                catch
                {
                    retval.Dispose();
                }
                return retval;
            }
            finally
            {
                if (image1 != null)
                    image1.Dispose();
                if (image2 != null)
                    image2.Dispose();
                if (image3 != null)
                    image3.Dispose();
            }
        }

        private static Image RenderOne(string hexDigest, int width, int height)
        {
            var robo = RoboHash.Create(hexDigest);
            return robo.Render(width, height);
        }

        private static string Xor(string a, string b)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < a.Length; i++)
                sb.Append((char)(a[i] ^ b[(i % b.Length)]));
            return sb.ToString();
        }
    }
}
