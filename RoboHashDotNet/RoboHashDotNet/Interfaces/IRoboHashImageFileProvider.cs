using System.Collections.Generic;

namespace RoboHashDotNet.Interfaces
{
    public interface IRoboHashImageFileProvider
    {
        /// <summary>
        /// Gets the available robohash sets.
        /// </summary>
        /// <value>
        /// The sets.
        /// </value>
        string[] Sets { get; }

        /// <summary>
        /// Gets the available background sets.
        /// </summary>
        /// <value>
        /// The background sets.
        /// </value>
        string[] BackgroundSets { get; }

        /// <summary>
        /// Gets the available colors.
        /// </summary>
        /// <value>
        /// The colors.
        /// </value>
        string[] Colors { get; }

        /// <summary>
        /// Gets the base path.
        /// </summary>
        /// <value>
        /// The base path.
        /// </value>
        string BasePath { get; }

        /// <summary>
        /// Gets the directories.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        IEnumerable<string> GetDirectories(string path);

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        IList<string> GetFiles(string path);
    }
}
