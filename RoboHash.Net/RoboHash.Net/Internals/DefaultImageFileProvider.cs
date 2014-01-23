using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RoboHash.Net.Interfaces;

namespace RoboHash.Net.Internals
{
    internal class DefaultImageFileProvider : IRoboHashImageFileProvider
    {
        private static readonly string[] _sets;
        private static readonly string[] _backgroundSets;
        private static readonly string[] _colors;
        private static readonly string _resourcePath;

        static DefaultImageFileProvider()
        {

#if SILVERLIGHT
            var assembly = Assembly.GetExecutingAssembly();
#else
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
#endif

            _resourcePath = Path.GetDirectoryName(assembly.Location);
            if (_resourcePath == null)
                throw new InvalidOperationException("Failed to retrieve resource path.");

            _sets = GetDirectoryNames(Path.Combine(_resourcePath, RoboConsts.SetsDir)).ToArray();
            _backgroundSets = GetDirectoryNames(Path.Combine(_resourcePath, RoboConsts.BackgroundsDir)).ToArray();
            _colors = GetDirectoryNames(Path.Combine(_resourcePath, RoboConsts.SetsDir, _sets[0])).ToArray();
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
            get { return _resourcePath; }
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
