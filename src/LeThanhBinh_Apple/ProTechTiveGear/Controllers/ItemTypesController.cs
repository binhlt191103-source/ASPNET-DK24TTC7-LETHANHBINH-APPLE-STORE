using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProTechTiveGear.Models;

namespace ProTechTiveGear.Controllers
{
    public class ItemTypesController : Controller
    {
        private ProTechTiveGearEntities db = new ProTechTiveGearEntities();

        // GET: ItemTypes
        public ActionResult Index()
        {
            var itemTypes = db.ItemTypes.Include(i => i.Menu);
            return View(itemTypes.ToList());
        }

        // GET: ItemTypes/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemType itemType = db.ItemTypes.Find(id);
            if (itemType == null)
            {
                return HttpNotFound();
            }
            return View(itemType);
        }

        // GET: ItemTypes/Create
        public ActionResult Create()
        {
            ViewBag.MenuID = new SelectList(db.Menus.Where(x => x.ID != 1), "ID", "Name");
            return View();
        }

        // POST: ItemTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TypeName,MenuID")] ItemType itemType)
        {
            if (ModelState.IsValid)
            {
                db.ItemTypes.Add(itemType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MenuID = new SelectList(db.Menus, "ID", "Name", itemType.MenuID);
            return View(itemType);
        }

        // GET: ItemTypes/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemType itemType = db.ItemTypes.Find(id);
            if (itemType == null)
            {
                return HttpNotFound();
            }
            ViewBag.MenuID = new SelectList(db.Menus.Where(x => x.ID != 1), "ID", "Name", itemType.MenuID);
            return View(itemType);
        }

        // POST: ItemTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TypeName,MenuID")] ItemType itemType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itemType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MenuID = new SelectList(db.Menus, "ID", "Name", itemType.MenuID);
            return View(itemType);
        }

        // GET: ItemTypes/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemType itemType = db.ItemTypes.Find(id);
            if (itemType == null)
            {
                return HttpNotFound();
            }
            return View(itemType);
        }

        // POST: ItemTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ItemType itemType = db.ItemTypes.Include("Items").FirstOrDefault(x => x.ID == id);
            if (itemType == null) return HttpNotFound();

            // 1. Chỉ đếm sản phẩm đang bán (Active=true) thôi, ẩn rồi thì bỏ qua
            int productCount = db.Items.Count(x => x.TypeID == id && x.Active == true);
            if (productCount > 0)
            {
                ModelState.AddModelError("", $"KHÔNG THỂ XÓA! Còn {productCount} sản phẩm đang bán thuộc loại '{itemType.TypeName}'. Vào Items -> Ẩn hết trước.");
                return View(itemType);
            }

            // 2. Kiểm tra sản phẩm ẩn có dính đơn hàng không (dùng x.Item.ID cho giống ItemsController bạn vừa fix)
            var allItemsOfType = db.Items.Where(x => x.TypeID == id).ToList();
            foreach (var it in allItemsOfType)
            {
                if (db.OrderDetails.Any(o => o.Item.ID == it.ID))
                {
                    ModelState.AddModelError("", $"KHÔNG THỂ XÓA! Sản phẩm ẩn '{it.Name}' đã có trong đơn hàng, không thể xóa cứng.");
                    return View(itemType);
                }
            }

            // 3. Không dính đơn hàng -> xóa luôn 2 sản phẩm ẩn + xóa loại
            db.Items.RemoveRange(allItemsOfType);
            db.ItemTypes.Remove(itemType);
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
