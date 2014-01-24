using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using RoboHashDotNet.Web.Models;

namespace RoboHashDotNet.Web.Controllers
{
    public class MainController : Controller
    {
        public ActionResult Default(string token)
        {
            return string.IsNullOrEmpty(token) ? this.Index() : this.Render(token);
        }

        private ActionResult Index()
        {
            return this.View("Index", new MainModel
            {
                IpAddress = this.Request.UserHostAddress
            });
        }

        private ActionResult Render(string token)
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