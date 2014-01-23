using System.Web.Mvc;
using RoboHash.Net.Web.Models;

namespace RoboHash.Net.Web.Controllers
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