using System.Globalization;
using System.Windows.Media;

namespace RoboHash.Net.Internals
{
    public static class RoboHelper
    {
        public static Color ConvertHexColor(string color)
        {
            var argb = int.Parse(color.Replace("#", ""), NumberStyles.HexNumber);
            return Color.FromArgb((byte)((argb & -16777216) >> 0x18),
                          (byte)((argb & 0xff0000) >> 0x10),
                          (byte)((argb & 0xff00) >> 8),
                          (byte)(argb & 0xff));
        }
    }
}
