using Microsoft.AspNet.Identity;
using Shopoo.Models;
using System;
using System.Collections.Generic;
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
            if (Utils.UtilisateurUtil.IsLoggeIn())
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
            }

            return View(db.Produits.Where(p => p.MisEnVente == true).ToList<Produit>());
        }

        [Authorize(Roles = "Admin")]
        //[AllowAnonymous]
        public ActionResult Dashboard()
        {
            return View();
        }

        //[Authorize(Roles = "Client")]
        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}