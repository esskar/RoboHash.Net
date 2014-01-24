using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RoboHashDotNet.Interfaces;

namespace RoboHashDotNet
{
    public abstract class RoboHashBase<TImage>
    {
        private readonly IRoboHashImageFileProvider _imageFileProvider;

        private readonly long[] _indicies;
        private readonly string _hexDigest;
        
        protected RoboHashBase(string hexDigest, IRoboHashImageFileProvider imageFileProvider)
        {
            if (string.IsNullOrEmpty(hexDigest) || (hexDigest.Length % 2) != 0)
                throw new ArgumentException("hexDigest");
            if (imageFileProvider == null)
                throw new ArgumentNullException("imageFileProvider");

            _hexDigest = hexDigest;
            _imageFileProvider = imageFileProvider;
            _indicies = CreateIndices(_hexDigest, RoboConsts.HashCount).ToArray();
        }

        /// <summary>
        /// Gets the hexadecimal digest.
        /// </summary>
        /// <value>
        /// The hexadecimal digest.
        /// </value>
        public string HexDigest
        {
            get { return _hexDigest; }
        }

        /// <summary>
        /// Gets the indices.
        /// </summary>
        /// <value>
        /// The indices.
        /// </value>
        public long[] Indices
        {
            get { return _indicies; }
        }

        /// <summary>
        /// Renders the specified width.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public TImage Render(int width = 400, int height = 400)
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
        public TImage Render(string set, string backgroundSet, string color, int width, int height)
        {
            // Allow users to manually specify a robot 'set' that they like.
            // Ensure that this is one of the allowed choices, or allow all
            // If they don't set one, take the first entry from sets above.
            if (RoboConsts.Any.Equals(set, StringComparison.OrdinalIgnoreCase))
                set = _imageFileProvider.Sets[_indicies[RoboConsts.SetIndex] % _imageFileProvider.Sets.Length];
            else if (!_imageFileProvider.Sets.Contains(set))
                set = _imageFileProvider.Sets[0];

            // Only set1 is setup to be color-seletable. The others don't have enough pieces in various colors.
            // This could/should probably be expanded at some point.. 
            // Right now, this feature is almost never used. ( It was < 44 requests this year, out of 78M reqs )
            if (_imageFileProvider.Sets[0].Equals(set, StringComparison.OrdinalIgnoreCase))
            {
                if (_imageFileProvider.Colors.Contains(color))
                    set = Path.Combine(set, color);
                else
                    set = Path.Combine(set, _imageFileProvider.Colors[_indicies[RoboConsts.ColorIndex] % _imageFileProvider.Colors.Length]);
            }

            // If they specified a background, ensure it's legal, then give it to them.
            if (RoboConsts.Any.Equals(backgroundSet, StringComparison.OrdinalIgnoreCase))
                backgroundSet = _imageFileProvider.BackgroundSets[_indicies[RoboConsts.BackgroundSetIndex] % _imageFileProvider.BackgroundSets.Length];
            else if (!_imageFileProvider.BackgroundSets.Contains(backgroundSet))
                backgroundSet = null;

            // Each directory in our set represents one piece of the Robot, such as the eyes, nose, mouth, etc.

            // Each directory is named with two numbers - The number before the # is the sort order.
            // This ensures that they always go in the same order when choosing pieces, regardless of OS.

            // The second number is the order in which to apply the pieces.
            // For instance, the head has to go down BEFORE the eyes, or the eyes would be hidden.

            var roboImages = new List<string>();
            
            // First, we'll check if we should generate a background
            if (!string.IsNullOrEmpty(backgroundSet))
                roboImages.Add(this.GetBackgroundImageFile(Path.Combine(_imageFileProvider.BasePath, RoboConsts.BackgroundsDir, backgroundSet)));            

            // Then, we'll get a list of parts of our robot.
            roboImages.AddRange(this.GetSetImageFiles(Path.Combine(_imageFileProvider.BasePath, RoboConsts.SetsDir, set)));

            // Then render the files.
            return this.RenderFiles(roboImages, RoboConsts.ImageWidth, RoboConsts.ImageHeight, width, height);
        }

        /// <summary>
        /// Renders the files.
        /// </summary>
        /// <param name="srcFiles">The source files.</param>
        /// <param name="srcWidth">Width of the source.</param>
        /// <param name="srcHeight">Height of the source.</param>
        /// <param name="destWidth">Width of the dest.</param>
        /// <param name="destHeight">Height of the dest.</param>
        /// <returns></returns>
        protected abstract TImage RenderFiles(IEnumerable<string> srcFiles, int srcWidth, int srcHeight, int destWidth, int destHeight);

        private IEnumerable<string> GetSetImageFiles(string path)
        {
            var index = RoboConsts.ImageIndex;

            var retval = _imageFileProvider.GetDirectories(path)
                .Select(_imageFileProvider.GetFiles)
                .Select(files => files[(int)(_indicies[index++] % files.Count)]).ToList();
            retval.Sort(new ImageFileSorter());

            return retval;
        }

        private string GetBackgroundImageFile(string path)
        {
            var files = _imageFileProvider.GetFiles(path);
            return files[(int) (_indicies[RoboConsts.BackgroundIndex]%files.Count)];
        }

        #region Helpers

        private static IEnumerable<long> CreateIndices(string hexDigest, int hashCount)
        {
            var blockSize = hexDigest.Length / hashCount;
            for (var i = 0; i < hashCount; ++i)
            {
                var sub = hexDigest.Substring(i * blockSize, blockSize);
                yield return Convert.ToInt64(sub, 16);
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
                return string.Compare(Lookup(x), Lookup(y), StringComparison.Ordinal);
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

        #endregion
    }
}
