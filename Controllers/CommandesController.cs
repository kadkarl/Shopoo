using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Shopoo.Models;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System;
using Shopoo.Utils;
using System.Data.Entity.Migrations;

namespace Shopoo.Controllers
{
    public class CommandesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Commandes
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            string IdIdentityFramework = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Utilisateur utilisateur = db.Utilisateurs.Where(u => u.IdIdentityFramework == IdIdentityFramework).FirstOrDefault<Utilisateur>();

            Session["Utilisateur"] = utilisateur;

            Panier panier = db.Paniers.Where(p => p.Utilisateur.Id == utilisateur.Id).FirstOrDefault();

            if (panier != null)
            {
                IList<ProduitVM> ProduitsPanier = new List<ProduitVM>();
                Random random = new Random();

                foreach (var p in panier.Produits)
                {
                    ProduitVM produitVM = new ProduitVM();
                    produitVM.Id = p.Id;
                    produitVM.Libelle = p.Libelle;
                    produitVM.Description = p.Description;
                    produitVM.Image = p.Image;
                    produitVM.Prix = p.Prix;
                    produitVM.QuantiteEnStock = p.QuantiteEnStock;
                    produitVM.UniqIdPanier = random.Next();
                    ProduitsPanier.Add(produitVM);
                }

                Session["Panier"] = ProduitsPanier;
            }

            return View(await db.Commandes.Where(u => u.Utilisateur.Id == utilisateur.Id).ToListAsync());
        }

        // GET: Commandes/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Commande commande = await db.Commandes.FindAsync(id);
            if (commande == null)
            {
                return HttpNotFound();
            }
            IList<ProduitVM> SessionProduitPanier = (List<ProduitVM>)Session["Panier"];
            return View(commande);
        }

        public ActionResult Payer(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            string IdIdentityFramework = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Utilisateur utilisateur = db.Utilisateurs.Where(u => u.IdIdentityFramework == IdIdentityFramework).FirstOrDefault<Utilisateur>();

            Panier panier = db.Paniers.Where(p => p.Utilisateur.Id == utilisateur.Id).FirstOrDefault();

            Commande commande = db.Commandes.Where(c => c.Id == id).SingleOrDefault();

            commande.EstValide = true;

            db.Commandes.AddOrUpdate(commande);
            db.SaveChanges();

            if (panier != null)
            {
                db.Paniers.Remove(panier);
                db.SaveChanges();
            }

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
