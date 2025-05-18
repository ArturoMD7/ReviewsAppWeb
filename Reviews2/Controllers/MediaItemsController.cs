using Reviews2.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Reviews2.Controllers
{
    public class MediaItemsController : Controller
    {
        private readonly MediaOpinionsDBEntities db = new MediaOpinionsDBEntities();

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
            if (!id.HasValue)
                return RedirectToAction("Index", "Home");

            db.Configuration.ProxyCreationEnabled = false;
            db.Configuration.LazyLoadingEnabled = false;

            var mediaItem = db.MediaItems
                .Include(m => m.Category)
                .Include(m => m.Opinions.Select(o => o.AspNetUser))
                .AsNoTracking()
                .FirstOrDefault(m => m.Id == id.Value);

            if (mediaItem == null)
                return HttpNotFound();

            var usuariosInfo = new Dictionary<string, UserInfoViewModel>();

            foreach (var opinion in mediaItem.Opinions)
            {
                if (opinion.AspNetUser != null && !usuariosInfo.ContainsKey(opinion.UsuarioId))
                {
                    usuariosInfo.Add(opinion.UsuarioId, new UserInfoViewModel
                    {
                        UserName = opinion.AspNetUser.UserName,
                        NombreCompleto = !string.IsNullOrWhiteSpace(opinion.AspNetUser.NombreCompleto)
                            ? opinion.AspNetUser.NombreCompleto
                            : opinion.AspNetUser.UserName,
                        AvatarUrl = !string.IsNullOrWhiteSpace(opinion.AspNetUser.AvatarUrl)
                            ? opinion.AspNetUser.AvatarUrl
                            : "/Content/avatars/default.png"
                    });
                }
            }

            var viewModel = new MediaItemDetailsViewModel
            {
                MediaItem = mediaItem,
                UsuariosInfo = usuariosInfo
            };

            return View(viewModel);
        }


        [AllowAnonymous]
        public ActionResult GetUserAvatar(string url)
        {
            if (string.IsNullOrEmpty(url) || !url.StartsWith("http"))
            {
                return File(Server.MapPath("~/Content/avatars/default.png"), "image/png");
            }

            try
            {
                using (var client = new System.Net.WebClient())
                {
                    client.Headers.Add("User-Agent", "Mozilla/5.0");
                    var imageData = client.DownloadData(url);
                    return File(imageData, "image/jpeg");
                }
            }
            catch
            {
                return File(Server.MapPath("~/Content/avatars/default.png"), "image/png");
            }
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
