using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Shopoo.Models;
using System.Collections.Generic;

namespace Shopoo.Controllers
{
    public class PaniersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IList<Produit> ProduitsPanier;

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
                ProduitsPanier = new List<Produit>();
                Session["Panier"] = ProduitsPanier;
            }
            List<Produit> SessionProduitPanier = (List<Produit>)Session["Panier"];
            SessionProduitPanier.Add(produit);
            Session["Panier"] = SessionProduitPanier;

            return RedirectToAction("Index", "Home");
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
