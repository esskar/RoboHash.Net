using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RoboHash.Net.Interfaces;

namespace RoboHash.Net.Internals
{
    public class DefaultImageFileProvider : IRoboHashImageFileProvider
    {
        private readonly string[] _sets;
        private readonly string[] _backgroundSets;
        private readonly string[] _colors;
        private readonly string _basePath;

        public DefaultImageFileProvider()
            : this(Path.GetDirectoryName(typeof(Resources.Importer).Assembly.Location)) { }

        public DefaultImageFileProvider(string basePath)
        {
            if (basePath == null)
                throw new ArgumentException("basePath");
            _basePath = basePath;
            _sets = GetDirectoryNames(Path.Combine(_basePath, RoboConsts.SetsDir)).ToArray();
            _backgroundSets = GetDirectoryNames(Path.Combine(_basePath, RoboConsts.BackgroundsDir)).ToArray();
            _colors = GetDirectoryNames(Path.Combine(_basePath, RoboConsts.SetsDir, _sets[0])).ToArray();
        }

        public string[] Sets
        {
            get { return _sets; }
        }

        public string[] BackgroundSets
        {
            get { return _backgroundSets; }
        }

        public string[] Colors
        {
            get { return _colors; }
        }

        public string BasePath
        {
            get { return _basePath; }
        }

        public IEnumerable<string> GetDirectories(string path)
        {
            return Directory.EnumerateDirectories(path);
        }

        public IList<string> GetFiles(string path)
        {
#if SILVERLIGHT

            return Directory.EnumerateFiles(path).ToArray();

#else

            return Directory.GetFiles(path);
            
#endif
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
    }
}
