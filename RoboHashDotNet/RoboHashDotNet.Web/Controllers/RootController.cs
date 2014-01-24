using System.Web.Mvc;
using RoboHashDotNet.Web.Models;

namespace RoboHashDotNet.Web.Controllers
{
    public class RootController : Controller
    {
        public ActionResult Index()
        {
            return this.View(new RootModel
            {
                IpAddress = this.Request.UserHostAddress
            });
        }
    }
}