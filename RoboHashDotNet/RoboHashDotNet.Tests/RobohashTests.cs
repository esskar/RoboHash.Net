using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RoboHashDotNet.Tests
{
    [TestClass]
    public class RoboHashTests
    {
        private readonly string[] _inputs =
        {
            "test"
        };

        private readonly string[] _hexDigests =
        {
            "ee26b0dd4af7e749aa1a8ee3c10ae9923f618980772e473f8819a5d4940e0db27ac185f8a0e1d5f84f88bc887fd67b143732c304cc5fa9ad8e6f57f50028a8ff"
        };

        private readonly long[][] _indices =
        {
            new []{ 16365621466287L, 8689954724494L, 15651140704547L, 16911593142062L, 4896136993373L, 5033937449594L, 13298821631517L, 6564044389512L, 8784947790707L, 3036622380969L, 11926704062288L}
        };

        [TestMethod]
        public void HexDigestTests()
        {
            for (var i = 0; i < _inputs.Length; ++i)
            {
                var r = RoboHash.Create(_inputs[i]);
                Assert.AreEqual(_hexDigests[i], r.HexDigest, "HexDigest for input #{0} does not match.", i);
            }
        }

        [TestMethod]
        public void IndicesTests()
        {
            for (var i = 0; i < _inputs.Length; ++i)
            {
                var r = RoboHash.Create(_inputs[i]);

                var h = _indices[i];
                Assert.AreEqual(h.Length, r.Indices.Length, "Indices length for input #{0} does not match.", i);
                for (var x = 0; x < h.Length; ++x)
                {
                    Assert.AreEqual(h[i], r.Indices[i], "Index at position {1} of input #{0} does not match.", i, x);
                }
            }
        }

        [TestMethod]
        public void RenderTests()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "tests");
            Directory.CreateDirectory(path);
            for (var i = 0; i < _inputs.Length; ++i)
            {
                var r = RoboHash.Create(_inputs[i]);
                using (var image = r.Render())
                {
                    var name = _inputs[i] + ".png";
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
                var r = RoboHash.Create(_inputs[i]);
                using (var image = r.Render(null, RoboConsts.Any, null, 400, 400))
                {
                    var name = _inputs[i] + ".bg.png";
                    image.Save(Path.Combine(path, name), ImageFormat.Png);
                }
            }
        }
    }
}
