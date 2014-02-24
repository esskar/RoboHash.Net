using System.Drawing;
using System.Drawing.Imaging;

namespace RoboHash.Net.Internals
{
    public static class ImageHelper
    {
        private static readonly ColorMatrix _grayscaleMatrix = new ColorMatrix(new[]
        {
           new[] {.3f, .3f, .3f, 0f, 0f},
           new[] {.59f, .59f, .59f, 0f, 0f},
           new[] {.11f, .11f, .11f, 0f, 0f},
           new[] {0f, 0f, 0f, 1f, 0f},
           new[] {0f, 0f, 0f, 0f, 1f}
        });

        public static Bitmap Blur(Bitmap image, int blurSize)
        {
            return Blur(image, new Rectangle(0, 0, image.Width, image.Height), blurSize);
        }

        public static Bitmap Blur(Bitmap image, Rectangle rectangle, int blurSize)
        {
            var blurred = new Bitmap(image.Width, image.Height);

            // make an exact copy of the bitmap provided
            using (var graphics = Graphics.FromImage(blurred))
            {
                var rect = new Rectangle(0, 0, image.Width, image.Height);
                graphics.DrawImage(image, rect, rect, GraphicsUnit.Pixel);
            }

            // look at every pixel in the blur rectangle
            for (var xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (var yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    var blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (var x = xx; (x < xx + blurSize && x < image.Width); x++)
                    {
                        for (var y = yy; (y < yy + blurSize && y < image.Height); y++)
                        {
                            var pixel = blurred.GetPixel(x, y);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (var x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                        for (var y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                            blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }

            return blurred;
        }

        public static void Blur(ref Bitmap bitmap, int blurSize)
        {
            var blurred = Blur(bitmap, blurSize);
            bitmap.Dispose();
            bitmap = blurred;
        }

        public static Bitmap MakeBlackAndWhite(Bitmap original)
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

        public static void MakeBlackAndWhite(ref Bitmap bitmap)
        {
            var baw = MakeBlackAndWhite(bitmap);
            bitmap.Dispose();
            bitmap = baw;
        }
    }
}
