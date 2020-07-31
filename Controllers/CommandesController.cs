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

            return View(await db.Commandes.Where(u => u.Utilisateur.Id == utilisateur.Id).ToListAsync());
        }

        // GET: Commandes/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Commande commande = await db.Commandes.FindAsync(id);
            if (commande == null)
            {
                return HttpNotFound();
            }
            IList<ProduitVM> SessionProduitPanier = (List<ProduitVM>)Session["Panier"];
            ViewBag.TotalTTC = Calcul.CalculTotalTTC(SessionProduitPanier);
            return View(commande);
        }

        public ActionResult Payer()
        {
            string IdIdentityFramework = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Utilisateur utilisateur = db.Utilisateurs.Where(u => u.IdIdentityFramework == IdIdentityFramework).FirstOrDefault<Utilisateur>();

            Panier panier = db.Paniers.Where(p => p.Utilisateur.Id == utilisateur.Id).FirstOrDefault();
            db.Paniers.Remove(panier);
            db.SaveChanges();

            return View();
        }

        // GET: Commandes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Commandes/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,DateCommande,EstValide,IdUtilisateur")] Commande commande)
        {
            if (ModelState.IsValid)
            {
                db.Commandes.Add(commande);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(commande);
        }

        // GET: Commandes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Commande commande = await db.Commandes.FindAsync(id);
            if (commande == null)
            {
                return HttpNotFound();
            }
            return View(commande);
        }

        // POST: Commandes/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,DateCommande,EstValide,IdUtilisateur")] Commande commande)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commande).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(commande);
        }

        // GET: Commandes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Commande commande = await db.Commandes.FindAsync(id);
            if (commande == null)
            {
                return HttpNotFound();
            }
            return View(commande);
        }

        // POST: Commandes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Commande commande = await db.Commandes.FindAsync(id);
            db.Commandes.Remove(commande);
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
