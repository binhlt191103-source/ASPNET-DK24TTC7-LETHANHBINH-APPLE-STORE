using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ProTechTiveGear.Models;

namespace ProTechTiveGear.Controllers
{
    public class ItemsController : Controller
    {
        private ProTechTiveGearEntities db = new ProTechTiveGearEntities();

        // GET: Items
        public ActionResult Index()
        {
            var items = db.Items.Include(i => i.Brand).Include(i => i.ItemType).Where(a => a.Active == true);
            return View(items.ToList());
        }
        public ActionResult Itemunactive()
        {
            var items = db.Items.Include(i => i.Brand).Include(i => i.ItemType).Where(a => a.Active == false);
            return View(items.ToList());
        }

        // GET: Items/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Item item = db.Items.Find(id);
            if (item == null) return HttpNotFound();
            return View(item);
        }

        // POST: Items/Delete/5 -> ĐÂY LÀ CODE XÓA SẢN PHẨM ĐÚNG
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Item item = db.Items.Find(id);
            if (item == null) return HttpNotFound();

            // Theo sơ đồ edmx: Item -> OrderDetails
            bool hasOrder = db.OrderDetails.Any(x => x.Item.ID == id);
            if (hasOrder)
            {
                // Có đơn hàng rồi thì chỉ ẩn, không xóa cứng
                item.Active = false;
                db.SaveChanges();
                TempData["Message"] = "Sản phẩm đã có trong đơn hàng nên đã chuyển sang mục Ẩn!";
                return RedirectToAction("Index");
            }

            db.Items.Remove(item);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // --- Các hàm khác giữ nguyên như cũ ---
        public ActionResult Details(long? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Item item = db.Items.Find(id);
            if (item == null) return HttpNotFound();
            return View(item);
        }
        public ActionResult Create()
        {
            ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name");
            ViewBag.TypeID = new SelectList(db.ItemTypes, "ID", "TypeName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "ID,Name,PurcharsePrice,SellPrice,Quantity,TypeID,BrandID,Picture,ShortTitle,Describe")] Item item)
        {
            if (ModelState.IsValid)
            {
                item.DateImport = DateTime.Now;
                item.Active = true;
                db.Items.Add(item);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name", item.BrandID);
            ViewBag.TypeID = new SelectList(db.ItemTypes, "ID", "TypeName", item.TypeID);
            return View(item);
        }
        public ActionResult Edit(long? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Item item = db.Items.Find(id);
            if (item == null) return HttpNotFound();
            ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name", item.BrandID);
            ViewBag.TypeID = new SelectList(db.ItemTypes, "ID", "TypeName", item.TypeID);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "ID,Name,PurcharsePrice,SellPrice,DateImport,Quantity,TypeID,BrandID,Picture,ShortTitle,Describe")] Item item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                item.Active = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name", item.BrandID);
            ViewBag.TypeID = new SelectList(db.ItemTypes, "ID", "TypeName", item.TypeID);
            return View(item);
        }
        public ActionResult UnactiveProduct(long? id)
        {
            var temp = db.Items.SingleOrDefault(p => p.ID == id);
            temp.Active = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Active(long? id)
        {
            var temp = db.Items.SingleOrDefault(p => p.ID == id);
            temp.Active = true;
            db.SaveChanges();
            return RedirectToAction("Itemunactive");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}