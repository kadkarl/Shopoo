using Shopoo.Models;
using Shopoo.Utils;
using System.Linq;
using System.Web.Mvc;

namespace Shopoo.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(db.Produits.Where(p => p.MisEnVente == true).ToList<Produit>());
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