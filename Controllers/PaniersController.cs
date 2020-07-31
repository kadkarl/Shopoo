using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Shopoo.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using Shopoo.Utils;
using Microsoft.AspNet.Identity;

namespace Shopoo.Controllers
{
    public class PaniersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IList<ProduitVM> ProduitsPanier;
        private IList<ProduitVM> SessionProduitPanier;

        // GET: Paniers
        public async Task<ActionResult> Index()
        {
            return View(await db.Commandes.ToListAsync());
        }

        [AllowAnonymous]
        public ActionResult AjouterAuPanier(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Produit produit = db.Produits.Find(id);

            if (Session["Panier"] == null)
            {
                ProduitsPanier = new List<ProduitVM>();
                Session["Panier"] = ProduitsPanier;
            }

            SessionProduitPanier = (List<ProduitVM>)Session["Panier"];

            Random random = new Random();

            ProduitVM produitVM = new ProduitVM();

            produitVM.Id = produit.Id;
            produitVM.Libelle = produit.Libelle;
            produitVM.Description = produit.Description;
            produitVM.Prix = produit.Prix;
            produitVM.Image = produit.Image;
            produitVM.QuantiteEnStock = produit.QuantiteEnStock;
            produitVM.UniqIdPanier = random.Next();

            SessionProduitPanier.Add(produitVM);
            Session["Panier"] = SessionProduitPanier;

            return Json(new { success = true, msg = "Produit ajouté au panier" }, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public ActionResult Voir()
        {
            SessionProduitPanier = (List<ProduitVM>)Session["Panier"];
            ViewBag.TotalTTC = Calcul.CalculTotalTTC(SessionProduitPanier);
            return View("Voir", SessionProduitPanier);
        }

        [AllowAnonymous]
        public ActionResult SupprimerProduit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SessionProduitPanier = (List<ProduitVM>)Session["Panier"];
            ProduitVM produit = SessionProduitPanier.Where(p => p.UniqIdPanier == id).SingleOrDefault();
            SessionProduitPanier.Remove(produit);
            ViewBag.TotalTTC = Calcul.CalculTotalTTC(SessionProduitPanier);

            return View("Voir", SessionProduitPanier);
        }

        [AllowAnonymous]
        public ActionResult ValiderPanier()
        {
            bool isLoggedIn = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (Utils.UtilisateurUtil.IsLoggeIn())
            {
                string IdIdentityFramework = System.Web.HttpContext.Current.User.Identity.GetUserId();

                Utilisateur utilisateur = db.Utilisateurs.Where(u => u.IdIdentityFramework == IdIdentityFramework).First<Utilisateur>();

                SessionProduitPanier = (List<ProduitVM>)Session["Panier"];
                Panier panier = new Panier();
                panier.Produits = new List<Produit>();

                for (int i = 0; i < SessionProduitPanier.Count(); i++)
                {
                    Produit produit = new Produit();
                    produit.Id = SessionProduitPanier[i].Id;
                    produit.Libelle = SessionProduitPanier[i].Libelle;
                    produit.Prix = SessionProduitPanier[i].Prix;
                    produit.Description = SessionProduitPanier[i].Description;
                    produit.Image = SessionProduitPanier[i].Image;
                    produit.QuantiteEnStock = SessionProduitPanier[i].QuantiteEnStock--;
                    panier.Produits.Add(produit);
                    panier.Utilisateur = utilisateur;
                }

                db.Paniers.Add(panier);
                db.SaveChanges();

                Random random = new Random();

                Commande commande = new Commande();
                commande.Utilisateur = utilisateur;
                commande.Produits = new List<Produit>();
                commande.Produits = panier.Produits;
                commande.DateCommande = new DateTime().ToString();
                commande.NumeroDeCommande = random.Next();

                commande.TotalTTC = Calcul.CalculTotalTTC(SessionProduitPanier);

                db.Commandes.Add(commande);
                db.SaveChanges();

                return RedirectToAction("Details", "Commandes", new { id = commande.Id });
            }

            return RedirectToAction("Login", "Account");

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
