using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RoboHash.Net.Tests
{
    [TestClass]
    public class RoboArmadaHashTests
    {
        private readonly string[] _inputs =
        {
            "test",
            "test1",
            "test2",
            "test3",
            "test4",
            "test5",
            "test6",
            "test7",
        };

        [TestMethod]
        public void RenderTests()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "tests");
            Directory.CreateDirectory(path);
           
            for (var i = 0; i < _inputs.Length; ++i)
            {
                var r = RoboArmadaHash.Create(_inputs[i]);
                using (var image = r.Render())
                {
                    var name = _inputs[i] + ".armada.png";
                    image.Save(Path.Combine(path, name), ImageFormat.Png);
                }
            }
        }

        [TestMethod]
        public void RenderWithBackgroundTests()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "tests");
            Directory.CreateDirectory(path);
            for (var i = 0; i < _inputs.Length; ++i)
            {
                var r = RoboArmadaHash.Create(_inputs[i]);
                using (var image = r.Render(null, RoboConsts.Any, null, 400, 400))
                {
                    var name = _inputs[i] + ".armada.bg.png";
                    image.Save(Path.Combine(path, name), ImageFormat.Png);
                }
            }
        }
    }
}
