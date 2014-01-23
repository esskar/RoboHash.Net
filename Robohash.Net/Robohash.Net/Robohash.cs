using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Robohash.Net
{
    public class Robohash
    {
        public const string Any = "any";

        private const int HashCount = 11;
        private const string SetsDir = "sets";
        private const string BackgroundsDir = "backgrounds";
        private const int ColorIndex = 0;
        private const int SetIndex = 1;
        private const int BackgroundSetIndex = 2;
        private const int BackgroundIndex = 3;
        private const int ImageIndex = 4;
        private const int Width = 1024;
        private const int Height = 1024;

        private static readonly string _resourcePath;

        public static readonly string[] Sets;
        public static readonly string[] BackgroundSets;
        public static readonly string[] Colors;

        static Robohash()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            _resourcePath = Path.GetDirectoryName(assembly.Location);
            if (_resourcePath == null)
                throw new InvalidOperationException("Failed to retrieve resource path.");
            Sets = GetDirectoryNames(Path.Combine(_resourcePath, SetsDir)).ToArray();
            BackgroundSets = GetDirectoryNames(Path.Combine(_resourcePath, BackgroundsDir)).ToArray();
            Colors = GetDirectoryNames(Path.Combine(_resourcePath, SetsDir, Sets[0])).ToArray();
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
            var robohash = new Robohash();
            robohash.Initialize(stream);
            return robohash;
        }

        private Robohash() { }

        /// <summary>
        /// Initializes the robohash from the given stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        private void Initialize(Stream stream)
        {
            this.HexDigest = CreateHexDigest(stream);
            this.Indices = CreateIndices(this.HexDigest, HashCount).ToArray();

        }

        public string[] AvailableSets { get; private set; }

        /// <summary>
        /// Gets the hexadecimal digest.
        /// </summary>
        /// <value>
        /// The hexadecimal digest.
        /// </value>
        public string HexDigest { get; private set; }

        /// <summary>
        /// Gets the indices.
        /// </summary>
        /// <value>
        /// The indices.
        /// </value>
        public long[] Indices { get; private set; }

        /// <summary>
        /// Renders the specified width.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public Image Render(int width = 400, int height = 400)
        {
            return this.Render(null, null, null, width, height);
        }

        /// <summary>
        /// Renders the specified set.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="backgroundSet">The background set.</param>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public Image Render(string set, string backgroundSet, string color, int width, int height)
        {
            // Allow users to manually specify a robot 'set' that they like.
            // Ensure that this is one of the allowed choices, or allow all
            // If they don't set one, take the first entry from sets above.
            if (set == Any)
                set = Sets[this.Indices[SetIndex] % Sets.Length];
            else if (!Sets.Contains(set))
                set = Sets[0];

            // Only set1 is setup to be color-seletable. The others don't have enough pieces in various colors.
            // This could/should probably be expanded at some point.. 
            // Right now, this feature is almost never used. ( It was < 44 requests this year, out of 78M reqs )
            if (set == Sets[0])
            {
                if (Colors.Contains(color))
                    set = Path.Combine(set, color);
                else
                    set = Path.Combine(set, Colors[this.Indices[ColorIndex] % Colors.Length]);
            }

            // If they specified a background, ensure it's legal, then give it to them.
            if (backgroundSet == Any)
                backgroundSet = BackgroundSets[this.Indices[BackgroundIndex] % BackgroundSets.Length];
            else if (!BackgroundSets.Contains(backgroundSet))
                backgroundSet = null;

            // Each directory in our set represents one piece of the Robot, such as the eyes, nose, mouth, etc.

            // Each directory is named with two numbers - The number before the # is the sort order.
            // This ensures that they always go in the same order when choosing pieces, regardless of OS.

            // The second number is the order in which to apply the pieces.
            // For instance, the head has to go down BEFORE the eyes, or the eyes would be hidden.

            // First, we'll get a list of parts of our robot.
            var roboImages = this.GetImageFiles(Path.Combine(_resourcePath, SetsDir, set));

            var retval = new Bitmap(Width, Height);
            using (var canvas = Graphics.FromImage(retval))
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                foreach (var imageFile in roboImages)
                {
                    using(var image = Image.FromFile(imageFile))
                        canvas.DrawImage(image, new Rectangle(0, 0, Width, Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                }
                canvas.Save();
            }

            return retval;
        }        

        private List<string> GetImageFiles(string path)
        {
            var index = ImageIndex;

            var retval = Directory.EnumerateDirectories(path)
                .Select(Directory.GetFiles)
                .Select(files => files[this.Indices[index++]%files.Length]).ToList();
            retval.Sort(new ImageFileSorter());

            return retval;
        }

        #region Helpers

        private static string CreateHexDigest(Stream stream)
        {
            using (var sha512 = SHA512.Create())
            {
                var digest = sha512.ComputeHash(stream);
                return ConvertToHex(digest);
            }
        }

        private static IEnumerable<long> CreateIndices(string hexDigest, int hashCount)
        {
            var blockSize = hexDigest.Length / hashCount;
            for (var i = 0; i < hashCount; ++i)
            {
                var sub = hexDigest.Substring(i * blockSize, blockSize);
                yield return Convert.ToInt64(sub, 16);
            }
        }

        private static IEnumerable<string> GetDirectoryNames(string path)
        {
            foreach (var iterDir in Directory.EnumerateDirectories(path))
            {
                var dirPath = iterDir;
                if (dirPath.StartsWith(path))
                {
                    dirPath = dirPath.Substring(path.Length);
                    dirPath = dirPath.TrimStart('\\', '/');
                }
                yield return dirPath;
            }
        }

        private class ImageFileSorter : IComparer<string>
        {
            private readonly Dictionary<string, string> _lookup;

            public ImageFileSorter()
            {
                _lookup = new Dictionary<string, string>();
            }

            public int Compare(string x, string y)
            {
                return string.Compare(Lookup(x), Lookup(y), System.StringComparison.Ordinal);
            }

            private string Lookup(string imageFilePath)
            {
                string value;
                if (_lookup.TryGetValue(imageFilePath, out value)) 
                    return value;

                value = imageFilePath.Split('#')[1];
                _lookup.Add(imageFilePath, value);
                return value;
            }
        }

        private static readonly string[] _base16CharTable =
        {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0a", "0b", "0c", "0d", "0e", "0f",
            "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1a", "1b", "1c", "1d", "1e", "1f",
            "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2a", "2b", "2c", "2d", "2e", "2f",
            "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3a", "3b", "3c", "3d", "3e", "3f",
            "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4a", "4b", "4c", "4d", "4e", "4f",
            "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5a", "5b", "5c", "5d", "5e", "5f",
            "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6a", "6b", "6c", "6d", "6e", "6f",
            "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7a", "7b", "7c", "7d", "7e", "7f",
            "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8a", "8b", "8c", "8d", "8e", "8f",
            "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9a", "9b", "9c", "9d", "9e", "9f",
            "a0", "a1", "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9", "aa", "ab", "ac", "ad", "ae", "af",
            "b0", "b1", "b2", "b3", "b4", "b5", "b6", "b7", "b8", "b9", "ba", "bb", "bc", "bd", "be", "bf",
            "c0", "c1", "c2", "c3", "c4", "c5", "c6", "c7", "c8", "c9", "ca", "cb", "cc", "cd", "ce", "cf",
            "d0", "d1", "d2", "d3", "d4", "d5", "d6", "d7", "d8", "d9", "da", "db", "dc", "dd", "de", "df",
            "e0", "e1", "e2", "e3", "e4", "e5", "e6", "e7", "e8", "e9", "ea", "eb", "ec", "ed", "ee", "ef",
            "f0", "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "fa", "fb", "fc", "fd", "fe", "ff"
        };

        private static string ConvertToHex(IList<byte> input)
        {
            if (input == null)
                return string.Empty;

            var sb = new StringBuilder();

            // ReSharper disable ForCanBeConvertedToForeach
            for (var i = 0; i < input.Count; ++i)
                sb.Append(_base16CharTable[input[i]]);
            // ReSharper restore ForCanBeConvertedToForeach

            return sb.ToString();
        }

        #endregion
    }
}
