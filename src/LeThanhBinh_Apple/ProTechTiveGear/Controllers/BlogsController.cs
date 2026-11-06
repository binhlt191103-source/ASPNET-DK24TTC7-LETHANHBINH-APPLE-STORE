using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProTechTiveGear.Models;

namespace ProTechTiveGear.Controllers
{
    public class BlogsController : Controller
    {
        private ProTechTiveGearEntities db = new ProTechTiveGearEntities();

        public ActionResult Index()
        {
            return View(db.Blogs.OrderByDescending(b => b.ID).ToList());
        }

        public ActionResult Details(long? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Blog blog = db.Blogs.Find(id);
            if (blog == null) return HttpNotFound();
            return View(blog);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Picture,Title,DateImport,Describe,ShortTitle")] Blog blog, HttpPostedFileBase PictureUpload)
        {
            if (ModelState.IsValid)
            {
                // Giống Items - Xử lý upload ảnh
                if (PictureUpload != null && PictureUpload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(PictureUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/img/Blog/"), fileName);
                    if (!Directory.Exists(Path.GetDirectoryName(path)))
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                    PictureUpload.SaveAs(path);
                    blog.Picture = fileName;
                }
                blog.DateImport = DateTime.Now;
                db.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        public ActionResult Edit(long? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Blog blog = db.Blogs.Find(id);
            if (blog == null) return HttpNotFound();
            return View(blog);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Picture,Title,DateImport,Describe,ShortTitle")] Blog blog, HttpPostedFileBase PictureUpload)
        {
            if (ModelState.IsValid)
            {
                var old = db.Blogs.Find(blog.ID);
                old.Title = blog.Title;
                old.ShortTitle = blog.ShortTitle;
                old.Describe = blog.Describe;
                old.DateImport = blog.DateImport;

                if (PictureUpload != null && PictureUpload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(PictureUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/img/Blog/"), fileName);
                    PictureUpload.SaveAs(path);
                    old.Picture = fileName;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        public ActionResult Delete(long? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Blog blog = db.Blogs.Find(id);
            if (blog == null) return HttpNotFound();
            return View(blog);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Blog blog = db.Blogs.Find(id);
            db.Blogs.Remove(blog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}