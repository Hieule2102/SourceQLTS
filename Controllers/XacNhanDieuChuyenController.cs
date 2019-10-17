using Source.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Source.Controllers
{
    public class XacNhanDieuChuyenController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /XacNhanDieuChuyen/
        public async Task<ActionResult> Index()
        {
            if (Session["BAO_CAO"] != null)
            {

            }
            else
            {
                return HttpNotFound("You have no accesss permissions at this");
            }

            var xac_nhan_dieu_chuyen = db.XAC_NHAN_DIEU_CHUYEN.Where(a => a.XAC_NHAN == true);
            return View(await xac_nhan_dieu_chuyen.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string SEARCH_STRING)
        {
            var xac_nhan_dieu_chuyen = db.XAC_NHAN_DIEU_CHUYEN.Where(a => a.XAC_NHAN == true);

            if (!String.IsNullOrEmpty(SEARCH_STRING))
            {
                //xac_nhan_dieu_chuyen = xac_nhan_dieu_chuyen.Where(a => a.THIETBI.TENTB.Contains(SEARCH_STRING));
            }
            return View(await xac_nhan_dieu_chuyen.ToListAsync());
        }

        // GET: /XacNhanDieuChuyen/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XAC_NHAN_DIEU_CHUYEN xac_nhan_dieu_chuyen = await db.XAC_NHAN_DIEU_CHUYEN.FindAsync(id);
            if (xac_nhan_dieu_chuyen == null)
            {
                return HttpNotFound();
            }
            return View(xac_nhan_dieu_chuyen);
        }

        // GET: /XacNhanDieuChuyen/Create
        public ActionResult Create()
        {
            ViewBag.MA_DIEU_CHUYEN = new SelectList(db.DIEU_CHUYEN_THIET_BI, "MA_DIEU_CHUYEN", "MANS_THEO_DOI");
            return View();
        }

        // POST: /XacNhanDieuChuyen/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MA_XAC_NHAN,THOI_GIAN_XAC_NHAN,MA_DIEU_CHUYEN,MANS_XAC_NHAN")] XAC_NHAN_DIEU_CHUYEN xac_nhan_dieu_chuyen)
        {
            if (ModelState.IsValid)
            {
                db.XAC_NHAN_DIEU_CHUYEN.Add(xac_nhan_dieu_chuyen);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(xac_nhan_dieu_chuyen);
        }

        // GET: /XacNhanDieuChuyen/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XAC_NHAN_DIEU_CHUYEN xac_nhan_dieu_chuyen = await db.XAC_NHAN_DIEU_CHUYEN.FindAsync(id);
            if (xac_nhan_dieu_chuyen == null)
            {
                return HttpNotFound();
            }
            return View(xac_nhan_dieu_chuyen);
        }

        // POST: /XacNhanDieuChuyen/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MA_XAC_NHAN,THOI_GIAN_XAC_NHAN,MA_DIEU_CHUYEN,MANS_XAC_NHAN")] XAC_NHAN_DIEU_CHUYEN xac_nhan_dieu_chuyen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(xac_nhan_dieu_chuyen).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(xac_nhan_dieu_chuyen);
        }

        // GET: /XacNhanDieuChuyen/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XAC_NHAN_DIEU_CHUYEN xac_nhan_dieu_chuyen = await db.XAC_NHAN_DIEU_CHUYEN.FindAsync(id);
            if (xac_nhan_dieu_chuyen == null)
            {
                return HttpNotFound();
            }
            return View(xac_nhan_dieu_chuyen);
        }

        // POST: /XacNhanDieuChuyen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            XAC_NHAN_DIEU_CHUYEN xac_nhan_dieu_chuyen = await db.XAC_NHAN_DIEU_CHUYEN.FindAsync(id);
            db.XAC_NHAN_DIEU_CHUYEN.Remove(xac_nhan_dieu_chuyen);
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
