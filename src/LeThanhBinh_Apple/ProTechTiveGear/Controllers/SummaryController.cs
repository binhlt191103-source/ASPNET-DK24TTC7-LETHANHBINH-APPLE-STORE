using ProTechTiveGear.Models;
using System.Linq;
using System.Web.Mvc;

namespace ProTechTiveGear.Controllers
{
    public class ProductStat
    {
        public string TypeName { get; set; }
        public int SLBan { get; set; }
    }
    public class MonthlyQuantityStat
    {
        public string Thang { get; set; } // ví dụ: 11/2024
        public int SoLuong { get; set; }
        public int ThangNum { get; set; }
        public int Nam { get; set; }
    }
    public class MonthlyRevenueStat
    {
        public string Thang { get; set; }
        public decimal DoanhThu { get; set; }
        public int ThangNum { get; set; }
        public int Nam { get; set; }
    }

    public class SummaryController : Controller
    {
        ProTechTiveGearEntities db = new ProTechTiveGearEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListOrder()
        {
            return View();
        }

        // 1. CŨ CỦA BÌNH - GIỮ NGUYÊN
        public JsonResult GetProductStats()
        {
            string sqlQuery = @"
                SELECT ItemType.TypeName, SUM(ISNULL(OrderDetail.Quantity, 0)) AS SLBan FROM ItemType
                    LEFT JOIN Item ON Item.TypeID = ItemType.ID
                    LEFT JOIN OrderDetail ON OrderDetail.ItemId = Item.ID
                    GROUP BY ItemType.TypeName
                    ORDER BY SUM(ISNULL(OrderDetail.Quantity, 0)) desc
            ";
            var stats = db.Database.SqlQuery<ProductStat>(sqlQuery).ToList();
            return Json(stats, JsonRequestBehavior.AllowGet);
        }

        // 2. MỚI: Số sản phẩm bán được hằng tháng
        public JsonResult GetMonthlyQuantity()
        {
            string sql = @"
                SELECT 
                    CAST(MONTH([Order].OrderDate) AS VARCHAR) + '/' + CAST(YEAR([Order].OrderDate) AS VARCHAR) AS Thang,
                    SUM(ISNULL(OrderDetail.Quantity,0)) AS SoLuong,
                    MONTH([Order].OrderDate) AS ThangNum,
                    YEAR([Order].OrderDate) AS Nam
                FROM [Order]
                JOIN OrderDetail ON OrderDetail.OrderId = [Order].ID
                GROUP BY YEAR([Order].OrderDate), MONTH([Order].OrderDate)
                ORDER BY Nam, ThangNum
            ";
            var stats = db.Database.SqlQuery<MonthlyQuantityStat>(sql).ToList();
            return Json(stats, JsonRequestBehavior.AllowGet);
        }

        // 3. MỚI: Doanh thu hằng tháng
        public JsonResult GetMonthlyRevenue()
        {
            string sql = @"
                SELECT 
                    CAST(MONTH([Order].OrderDate) AS VARCHAR) + '/' + CAST(YEAR([Order].OrderDate) AS VARCHAR) AS Thang,
                    SUM(ISNULL(OrderDetail.Quantity * Item.SellPrice, 0)) AS DoanhThu,
                    MONTH([Order].OrderDate) AS ThangNum,
                    YEAR([Order].OrderDate) AS Nam
                FROM [Order]
                JOIN OrderDetail ON OrderDetail.OrderId = [Order].ID
                JOIN Item ON Item.ID = OrderDetail.ItemId
                GROUP BY YEAR([Order].OrderDate), MONTH([Order].OrderDate)
                ORDER BY Nam, ThangNum
            ";
            var stats = db.Database.SqlQuery<MonthlyRevenueStat>(sql).ToList();
            return Json(stats, JsonRequestBehavior.AllowGet);
        }
    }
}