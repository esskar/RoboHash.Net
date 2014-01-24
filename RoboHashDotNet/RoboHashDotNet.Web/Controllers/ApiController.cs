using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;

namespace RoboHashDotNet.Web.Controllers
{
    public class ApiController : Controller
    {
        public ActionResult Render(string token)
        {
            var r = RoboHash.Create(token);
            using (var image = r.Render())
            {
                var format = ImageFormat.Png;

                var memory = new MemoryStream();
                image.Save(memory, format);
                memory.Seek(0, SeekOrigin.Begin);

                var formatType = format.ToString().ToLowerInvariant();
                var contentType = string.Format("image/{0}", formatType);
                return new FileStreamResult(memory, contentType)
                {
                    FileDownloadName = string.Format("{0}.{1}", token, formatType),                    
                };                
            }
        }
    }
}
