using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Reviews2.Models;

namespace Reviews2.Controllers
{
    
    public class MediaItemsController : Controller
    {
        private readonly MediaOpinionsDBEntities db = new MediaOpinionsDBEntities();

        // GET: MediaItems
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(db.MediaItems
                        .Include(m => m.Category)
                        .AsNoTracking()
                        .ToList());
        }

        // GET: MediaItems/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Cargar el MediaItem con sus opiniones y los usuarios asociados
            var mediaItem = db.MediaItems
                             .Include(m => m.Category)
                             .Include(m => m.Opinions.Select(o => o.AspNetUser))  // Cargar usuarios de las opiniones
                             .FirstOrDefault(m => m.Id == id);

            if (mediaItem == null)
            {
                return HttpNotFound();
            }

            return View(mediaItem);
        }

        // GET: MediaItems/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.CategoriaId = new SelectList(db.Categories, "Id", "Nombre");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Titulo,Descripcion,ImagenUrl,Plataforma,FechaPublicacion,CategoriaId")] MediaItem mediaItem)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoriaId = new SelectList(db.Categories, "Id", "Nombre", mediaItem.CategoriaId);
                return View(mediaItem);
            }

            db.MediaItems.Add(mediaItem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: MediaItems/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var mediaItem = db.MediaItems.Find(id);
            if (mediaItem == null) return HttpNotFound();

            ViewBag.CategoriaId = new SelectList(db.Categories, "Id", "Nombre", mediaItem.CategoriaId);
            return View(mediaItem);

        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Titulo,Descripcion,ImagenUrl,Plataforma,FechaPublicacion,CategoriaId")] MediaItem mediaItem)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoriaId = new SelectList(db.Categories, "Id", "Nombre", mediaItem.CategoriaId);
                return View(mediaItem);
            }

            db.Entry(mediaItem).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: MediaItems/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var mediaItem = db.MediaItems
                            .Include(m => m.Category)
                            .FirstOrDefault(m => m.Id == id);

            return mediaItem == null ? (ActionResult)HttpNotFound() : View(mediaItem);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var mediaItem = db.MediaItems.Find(id);
            if (mediaItem != null)
            {
                db.MediaItems.Remove(mediaItem);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}