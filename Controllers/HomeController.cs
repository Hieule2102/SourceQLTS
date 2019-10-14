using Source.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Source.Controllers
{
    public class HomeController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /DanhSachThietBi/
        public async Task<ActionResult> Index()
        {
            if (Session["TEN_DANG_NHAP"] != null)
            {

            }
            else
            {
                return HttpNotFound("You have no accesss permissions at this");
            }
            var thietbis = db.THIETBIs.Include(t => t.DON_VI).Include(t => t.LOAI_THIETBI).Include(t => t.NHA_CUNG_CAP);
            return View(await thietbis.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string SEARCH_STRING)
        {
            var thietbis = db.THIETBIs.Include(t => t.DON_VI).Include(t => t.LOAI_THIETBI).Include(t => t.NHA_CUNG_CAP);
           
            //Tìm tên thiết bị
            if (!String.IsNullOrEmpty(SEARCH_STRING))
            {
                thietbis = thietbis.Where(data => data.TENTB.Contains(SEARCH_STRING));
            }            
            return View(await thietbis.ToListAsync());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}