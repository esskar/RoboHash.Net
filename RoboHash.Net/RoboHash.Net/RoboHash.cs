using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Policy;
using System.Text;
using RoboHash.Net.Interfaces;
using RoboHash.Net.Internals;

namespace RoboHash.Net
{
    public class RoboHash : RoboHashBase<Image>
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
        public static RoboHash Create(string text)
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
        public static RoboHash Create(byte[] bytes, int offset, int length)
        {
            using (var memory = new MemoryStream(bytes, offset, length))
                return Create(memory);
        }

        /// <summary>
        /// Creates a robohash from the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static RoboHash Create(Stream stream)
        {
            var hexDigest = RoboHash.DigestGenerator.GenerateHexDigest(stream);
            return new RoboHash(hexDigest, RoboHash.ImageFileProvider);
        }

        public RoboHash(string hexDigest, IRoboHashImageFileProvider imageFileProvider)
            : base(hexDigest, imageFileProvider) { }


        /// <summary>
        /// Renders the files.
        /// </summary>
        /// <param name="srcFiles">The source files.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="srcWidth">Width of the source.</param>
        /// <param name="srcHeight">Height of the source.</param>
        /// <param name="destWidth">Width of the dest.</param>
        /// <param name="destHeight">Height of the dest.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        protected override Image RenderFiles(IEnumerable<string> srcFiles, string backgroundColor, int srcWidth, int srcHeight, int destWidth, int destHeight, Options options)
        {
            var retval = new Bitmap(srcWidth, srcHeight);
            using (var canvas = Graphics.FromImage(retval))
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                if (!string.IsNullOrWhiteSpace(backgroundColor))
                {
                    var color = RoboHelper.ConvertHexColor(backgroundColor);
                    using (var brush = new SolidBrush(color))
                        canvas.FillRectangle(brush, 0, 0, srcWidth, srcHeight);
                }                

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

            if (options.HasFlag(Options.Grayscale))
                RoboHelper.MakeBlackAndWhite(ref retval);
            if (options.HasFlag(Options.Blur))
                RoboHelper.Blur(ref retval, 5);

            return retval;
        }
    }
}
