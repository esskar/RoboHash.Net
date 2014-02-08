namespace RoboHash.Net.Interfaces
{
    public interface IRoboHash<out TImage>
    {
        /// <summary>
        /// Renders the specified width.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        TImage Render(int width = 400, int height = 400);

        /// <summary>
        /// Renders the specified set.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <param name="backgroundSet">The background set.</param>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        TImage Render(string set, string backgroundSet, string color, int width, int height);
    }
}