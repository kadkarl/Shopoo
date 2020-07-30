using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Shopoo.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using Shopoo.Utils;

namespace Shopoo.Controllers
{
    public class PaniersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IList<ProduitVM> ProduitsPanier;
        private List<ProduitVM> SessionProduitPanier;

        // GET: Paniers
        public async Task<ActionResult> Index()
        {
            return View(await db.Paniers.ToListAsync());
        }

        // GET: Paniers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Panier panier = await db.Paniers.FindAsync(id);
            if (panier == null)
            {
                return HttpNotFound();
            }
            return View(panier);
        }

        [AllowAnonymous]
        public ActionResult AjouterAuPanier(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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

            if (isLoggedIn)
            {
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
                }

                return View("Voir", panier);
            }

            return RedirectToAction("Login", "Account");

        }

        // GET: Paniers/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Paniers/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,IdUtilisateur")] Panier panier)
        {
            if (ModelState.IsValid)
            {
                db.Paniers.Add(panier);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(panier);
        }

        // GET: Paniers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Panier panier = await db.Paniers.FindAsync(id);
            if (panier == null)
            {
                return HttpNotFound();
            }
            return View(panier);
        }

        // POST: Paniers/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,IdUtilisateur")] Panier panier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(panier).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(panier);
        }

        // GET: Paniers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Panier panier = await db.Paniers.FindAsync(id);
            if (panier == null)
            {
                return HttpNotFound();
            }
            return View(panier);
        }

        // POST: Paniers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Panier panier = await db.Paniers.FindAsync(id);
            db.Paniers.Remove(panier);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
