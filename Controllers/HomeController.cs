using Shopoo.Utils;
using System.Security.Permissions;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace Shopoo.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        //[Authorize(Roles = Role.Admin)]
        public ActionResult Dashboard()
        {
            return View();
        }

        [Authorize(Roles = Role.Client)]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}