using System.Drawing;
using System.Drawing.Imaging;

namespace RoboHash.Net.Internals
{
    public static class GrayscaleHelper
    {
        private static readonly ColorMatrix _grayscaleMatrix = new ColorMatrix(new[]
        {
           new[] {.3f, .3f, .3f, 0f, 0f},
           new[] {.59f, .59f, .59f, 0f, 0f},
           new[] {.11f, .11f, .11f, 0f, 0f},
           new[] {0f, 0f, 0f, 1f, 0f},
           new[] {0f, 0f, 0f, 0f, 1f}
        });

        public static Bitmap ConvertToGrayscale(Bitmap original)
        {
            var retval = new Bitmap(original.Width, original.Height);

            using (var convas = Graphics.FromImage(retval))
            {
                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(_grayscaleMatrix);

                convas.DrawImage(original, 
                    new Rectangle(0, 0, original.Width, original.Height), 
                    0, 0, original.Width, original.Height, 
                    GraphicsUnit.Pixel, attributes);
            }

            return retval;
        }

        public static void MakeGray(ref Bitmap bitmap)
        {
            var gray = ConvertToGrayscale(bitmap);
            bitmap.Dispose();
            bitmap = gray;
        }
    }
}
