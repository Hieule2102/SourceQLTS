using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Source.Models;

namespace Source.Controllers
{
    public class NhatKyThietBiController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: NhatKyThietBi
        public async Task<ActionResult> Index()
        {
            if (Session["BAO_CAO"] != null)
            {
                //var pHAN_QUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == Session["NHOM_ND"].ToString()
                //                                             && a.MA_CHUC_NANG == 11);

                //ViewBag.Them = db.NHOM_ND_CHUCNANG.Where(a => a.MA_QUYEN == 1);
                //ViewBag.Sua = db.NHOM_ND_CHUCNANG.Where(a => a.MA_QUYEN == 3);
            }
            else
            {
                return HttpNotFound("You have no accesss permissions at this");
            }
            var nHAT_KY_THIET_BI = db.NHAT_KY_THIET_BI.OrderBy(a => a.MA_NHAT_KY).Include(n => n.DIEU_CHUYEN_THIET_BI).Include(n => n.NHAP_KHO).Include(n => n.XAC_NHAN_DIEU_CHUYEN).Include(n => n.XUAT_KHO);
            return View(await nHAT_KY_THIET_BI.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string TINH_TRANG, string SEARCH_STRING)
        {
            var nHAT_KY_THIET_BI = db.NHAT_KY_THIET_BI.OrderBy(a => a.MA_NHAT_KY).Include(n => n.DIEU_CHUYEN_THIET_BI).Include(n => n.NHAP_KHO).Include(n => n.XAC_NHAN_DIEU_CHUYEN).Include(n => n.XUAT_KHO);
            //Tìm tên thiết bị
            if (!String.IsNullOrEmpty(SEARCH_STRING))
            {
                if((nHAT_KY_THIET_BI = nHAT_KY_THIET_BI.Where(data => data.NHAP_KHO.THIETBI.TENTB.Contains(SEARCH_STRING))) != null)
                {
                    ViewBag.MATB = nHAT_KY_THIET_BI.Select(a => a.NHAP_KHO.THIETBI.MATB).FirstOrDefault();
                    ViewBag.TENTB = nHAT_KY_THIET_BI.Select(a => a.NHAP_KHO.THIETBI.TENTB).FirstOrDefault();
                }
                else if ((nHAT_KY_THIET_BI = nHAT_KY_THIET_BI.Where(data => data.XUAT_KHO.THIETBI.TENTB.Contains(SEARCH_STRING))) != null)
                {
                    ViewBag.MATB = nHAT_KY_THIET_BI.Select(a => a.XUAT_KHO.THIETBI.MATB).FirstOrDefault();
                    ViewBag.TENTB = nHAT_KY_THIET_BI.Select(a => a.XUAT_KHO.THIETBI.TENTB).FirstOrDefault();
                }
                else if ((nHAT_KY_THIET_BI = nHAT_KY_THIET_BI.Where(data => data.DIEU_CHUYEN_THIET_BI.THIETBI.TENTB.Contains(SEARCH_STRING))) != null)
                {
                    ViewBag.MATB = nHAT_KY_THIET_BI.Select(a => a.DIEU_CHUYEN_THIET_BI.THIETBI.MATB).FirstOrDefault();
                    ViewBag.TENTB = nHAT_KY_THIET_BI.Select(a => a.DIEU_CHUYEN_THIET_BI.THIETBI.TENTB).FirstOrDefault();
                }
                else if ((nHAT_KY_THIET_BI = nHAT_KY_THIET_BI.Where(data => data.XAC_NHAN_DIEU_CHUYEN.XUAT_KHO.THIETBI.TENTB.Contains(SEARCH_STRING))) != null)
                {
                    ViewBag.MATB = nHAT_KY_THIET_BI.Select(a => a.XAC_NHAN_DIEU_CHUYEN.XUAT_KHO.THIETBI.MATB).FirstOrDefault();
                    ViewBag.TENTB = nHAT_KY_THIET_BI.Select(a => a.XAC_NHAN_DIEU_CHUYEN.XUAT_KHO.THIETBI.TENTB).FirstOrDefault();
                }
                else if ((nHAT_KY_THIET_BI = nHAT_KY_THIET_BI.Where(data => data.XAC_NHAN_DIEU_CHUYEN.DIEU_CHUYEN_THIET_BI.THIETBI.TENTB.Contains(SEARCH_STRING))) != null)
                {
                    ViewBag.MATB = nHAT_KY_THIET_BI.Select(a => a.XAC_NHAN_DIEU_CHUYEN.DIEU_CHUYEN_THIET_BI.THIETBI.MATB).FirstOrDefault();
                    ViewBag.TENTB = nHAT_KY_THIET_BI.Select(a => a.XAC_NHAN_DIEU_CHUYEN.DIEU_CHUYEN_THIET_BI.THIETBI.TENTB).FirstOrDefault();
                }
            }
            else
            {
                ViewBag.MATB = null;
                ViewBag.TENTB = null;
            }

            return View(await nHAT_KY_THIET_BI.ToListAsync());
        }

        // GET: NhatKyThietBi/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHAT_KY_THIET_BI nHAT_KY_THIET_BI = await db.NHAT_KY_THIET_BI.FindAsync(id);
            if (nHAT_KY_THIET_BI == null)
            {
                return HttpNotFound();
            }
            return View(nHAT_KY_THIET_BI);
        }

        // GET: NhatKyThietBi/Create
        public ActionResult Create()
        {
            ViewBag.MA_DIEU_CHUYEN = new SelectList(db.DIEU_CHUYEN_THIET_BI, "MA_DIEU_CHUYEN", "MATB");
            ViewBag.MA_NHAP_KHO = new SelectList(db.NHAP_KHO, "MA_NHAP_KHO", "MATB");
            ViewBag.MA_XAC_NHAN = new SelectList(db.XAC_NHAN_DIEU_CHUYEN, "MA_XAC_NHAN", "MAND_XAC_NHAN");
            ViewBag.MA_XUAT_KHO = new SelectList(db.XUAT_KHO, "MA_XUAT_KHO", "MATB");
            return View();
        }

        // POST: NhatKyThietBi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MA_NHAT_KY,MA_XUAT_KHO,MA_DIEU_CHUYEN,MA_XAC_NHAN,MA_NHAP_KHO,TINH_TRANG")] NHAT_KY_THIET_BI nHAT_KY_THIET_BI)
        {
            if (ModelState.IsValid)
            {
                db.NHAT_KY_THIET_BI.Add(nHAT_KY_THIET_BI);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MA_DIEU_CHUYEN = new SelectList(db.DIEU_CHUYEN_THIET_BI, "MA_DIEU_CHUYEN", "MATB", nHAT_KY_THIET_BI.MA_DIEU_CHUYEN);
            ViewBag.MA_NHAP_KHO = new SelectList(db.NHAP_KHO, "MA_NHAP_KHO", "MATB", nHAT_KY_THIET_BI.MA_NHAP_KHO);
            ViewBag.MA_XAC_NHAN = new SelectList(db.XAC_NHAN_DIEU_CHUYEN, "MA_XAC_NHAN", "MAND_XAC_NHAN", nHAT_KY_THIET_BI.MA_XAC_NHAN);
            ViewBag.MA_XUAT_KHO = new SelectList(db.XUAT_KHO, "MA_XUAT_KHO", "MATB", nHAT_KY_THIET_BI.MA_XUAT_KHO);
            return View(nHAT_KY_THIET_BI);
        }

        // GET: NhatKyThietBi/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHAT_KY_THIET_BI nHAT_KY_THIET_BI = await db.NHAT_KY_THIET_BI.FindAsync(id);
            if (nHAT_KY_THIET_BI == null)
            {
                return HttpNotFound();
            }
            ViewBag.MA_DIEU_CHUYEN = new SelectList(db.DIEU_CHUYEN_THIET_BI, "MA_DIEU_CHUYEN", "MATB", nHAT_KY_THIET_BI.MA_DIEU_CHUYEN);
            ViewBag.MA_NHAP_KHO = new SelectList(db.NHAP_KHO, "MA_NHAP_KHO", "MATB", nHAT_KY_THIET_BI.MA_NHAP_KHO);
            ViewBag.MA_XAC_NHAN = new SelectList(db.XAC_NHAN_DIEU_CHUYEN, "MA_XAC_NHAN", "MAND_XAC_NHAN", nHAT_KY_THIET_BI.MA_XAC_NHAN);
            ViewBag.MA_XUAT_KHO = new SelectList(db.XUAT_KHO, "MA_XUAT_KHO", "MATB", nHAT_KY_THIET_BI.MA_XUAT_KHO);
            return View(nHAT_KY_THIET_BI);
        }

        // POST: NhatKyThietBi/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MA_NHAT_KY,MA_XUAT_KHO,MA_DIEU_CHUYEN,MA_XAC_NHAN,MA_NHAP_KHO,TINH_TRANG")] NHAT_KY_THIET_BI nHAT_KY_THIET_BI)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nHAT_KY_THIET_BI).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MA_DIEU_CHUYEN = new SelectList(db.DIEU_CHUYEN_THIET_BI, "MA_DIEU_CHUYEN", "MATB", nHAT_KY_THIET_BI.MA_DIEU_CHUYEN);
            ViewBag.MA_NHAP_KHO = new SelectList(db.NHAP_KHO, "MA_NHAP_KHO", "MATB", nHAT_KY_THIET_BI.MA_NHAP_KHO);
            ViewBag.MA_XAC_NHAN = new SelectList(db.XAC_NHAN_DIEU_CHUYEN, "MA_XAC_NHAN", "MAND_XAC_NHAN", nHAT_KY_THIET_BI.MA_XAC_NHAN);
            ViewBag.MA_XUAT_KHO = new SelectList(db.XUAT_KHO, "MA_XUAT_KHO", "MATB", nHAT_KY_THIET_BI.MA_XUAT_KHO);
            return View(nHAT_KY_THIET_BI);
        }

        // GET: NhatKyThietBi/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHAT_KY_THIET_BI nHAT_KY_THIET_BI = await db.NHAT_KY_THIET_BI.FindAsync(id);
            if (nHAT_KY_THIET_BI == null)
            {
                return HttpNotFound();
            }
            return View(nHAT_KY_THIET_BI);
        }

        // POST: NhatKyThietBi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            NHAT_KY_THIET_BI nHAT_KY_THIET_BI = await db.NHAT_KY_THIET_BI.FindAsync(id);
            db.NHAT_KY_THIET_BI.Remove(nHAT_KY_THIET_BI);
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
