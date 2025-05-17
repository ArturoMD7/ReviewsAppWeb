using Microsoft.AspNet.Identity;
using Reviews2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Reviews2.Controllers
{
    public class OpinionsController : Controller
    {
        private MediaOpinionsDBEntities db = new MediaOpinionsDBEntities();

        // GET: Opinions
        public ActionResult Index()
        {
            var opinions = db.Opinions.Include(o => o.MediaItem);
            return View(opinions.ToList());
        }

        // GET: Opinions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Opinion opinion = db.Opinions.Find(id);
            if (opinion == null)
            {
                return HttpNotFound();
            }
            return View(opinion);
        }

        // GET: Opinions/Create
        public ActionResult Create()
        {
            ViewBag.MediaItemId = new SelectList(db.MediaItems, "Id", "Titulo");
            return View();
        }

        // POST: Opinions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int mediaItemId, string comentario, int calificacion)
        {
            try
            {
                var opinion = new Opinion
                {
                    MediaItemId = mediaItemId,
                    Comentario = comentario,
                    Calificacion = calificacion,
                    Fecha = DateTime.Now,
                    UsuarioId = User.Identity.GetUserId()
                };

                db.Opinions.Add(opinion);
                db.SaveChanges();

                TempData["SuccessMessage"] = "¡Opinión publicada con éxito!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }

            return RedirectToAction("Details", "MediaItems", new { id = mediaItemId });
        }

        // GET: Opinions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Opinion opinion = db.Opinions.Find(id);
            if (opinion == null)
            {
                return HttpNotFound();
            }
            ViewBag.MediaItemId = new SelectList(db.MediaItems, "Id", "Titulo", opinion.MediaItemId);
            return View(opinion);
        }

        // POST: Opinions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UsuarioId,MediaItemId,Comentario,Calificacion,Fecha")] Opinion opinion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(opinion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MediaItemId = new SelectList(db.MediaItems, "Id", "Titulo", opinion.MediaItemId);
            return View(opinion);
        }

        // GET: Opinions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Opinion opinion = db.Opinions.Find(id);
            if (opinion == null)
            {
                return HttpNotFound();
            }
            return View(opinion);
        }

        // POST: Opinions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Opinion opinion = db.Opinions.Find(id);
            db.Opinions.Remove(opinion);
            db.SaveChanges();
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
