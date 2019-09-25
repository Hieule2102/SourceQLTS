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
    public class XacNhanController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: XacNhan
        public async Task<ActionResult> Index()
        {
            var xAC_NHAN_DIEU_CHUYEN = db.XAC_NHAN_DIEU_CHUYEN.Select(a => a.MA_DIEU_CHUYEN).ToList();
            var dIEU_CHUYEN_THIET_BI = db.DIEU_CHUYEN_THIET_BI.Where(a => !xAC_NHAN_DIEU_CHUYEN.Contains(a.MA_DIEU_CHUYEN)); ;
            return View(await dIEU_CHUYEN_THIET_BI.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string XAC_NHAN, FormCollection form, string SEARCH_STRING)
        {
            var xAC_NHAN_DIEU_CHUYEN = db.XAC_NHAN_DIEU_CHUYEN.Select(a => a.MA_DIEU_CHUYEN).ToList();
            var dIEU_CHUYEN_THIET_BI = db.DIEU_CHUYEN_THIET_BI.Where(a => !xAC_NHAN_DIEU_CHUYEN.Contains(a.MA_DIEU_CHUYEN));
            if (!String.IsNullOrEmpty(SEARCH_STRING))
            {
                dIEU_CHUYEN_THIET_BI = db.DIEU_CHUYEN_THIET_BI.Where(a => a.THIETBI.TENTB.Contains(SEARCH_STRING));
            }
            else if (!String.IsNullOrEmpty(XAC_NHAN))
            {
                //Thêm vào xác nhận điều chuyển
                XAC_NHAN_DIEU_CHUYEN a = new XAC_NHAN_DIEU_CHUYEN();
                a.MA_DIEU_CHUYEN = Int32.Parse(form["MA_DIEU_CHUYEN"]);
                //a.MAND_XAC_NHAN = Session["TEN_DANG_NHAP"].ToString();
                a.THOI_GIAN_XAC_NHAN = DateTime.Now;

                //Thêm vào nhật ký thiết bị
                NHAT_KY_THIET_BI nHAT_KY_THIET_BI = new NHAT_KY_THIET_BI();
                nHAT_KY_THIET_BI.MATB = Int32.Parse(form["maTB"]);
                nHAT_KY_THIET_BI.TINH_TRANG = "Đã xác nhận";
                nHAT_KY_THIET_BI.NGAY_THUC_HIEN = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Entry(a).State = EntityState.Modified;
                    db.NHAT_KY_THIET_BI.Add(nHAT_KY_THIET_BI);
                    await db.SaveChangesAsync();
                }
            }
            return View(await dIEU_CHUYEN_THIET_BI.ToListAsync());
        }

        // GET: XacNhan/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XAC_NHAN_DIEU_CHUYEN xAC_NHAN_DIEU_CHUYEN = await db.XAC_NHAN_DIEU_CHUYEN.FindAsync(id);
            if (xAC_NHAN_DIEU_CHUYEN == null)
            {
                return HttpNotFound();
            }
            return View(xAC_NHAN_DIEU_CHUYEN);
        }

        // GET: XacNhan/Create
        public ActionResult Create()
        {
            ViewBag.MA_DIEU_CHUYEN = new SelectList(db.DIEU_CHUYEN_THIET_BI, "MA_DIEU_CHUYEN", "MANS_QL");
            return View();
        }

        // POST: XacNhan/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MA_XAC_NHAN,THOI_GIAN_XAC_NHAN,MAND_XAC_NHAN,MA_DIEU_CHUYEN")] XAC_NHAN_DIEU_CHUYEN xAC_NHAN_DIEU_CHUYEN)
        {
            if (ModelState.IsValid)
            {
                db.XAC_NHAN_DIEU_CHUYEN.Add(xAC_NHAN_DIEU_CHUYEN);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MA_DIEU_CHUYEN = new SelectList(db.DIEU_CHUYEN_THIET_BI, "MA_DIEU_CHUYEN", "MANS_QL", xAC_NHAN_DIEU_CHUYEN.MA_DIEU_CHUYEN);
            return View(xAC_NHAN_DIEU_CHUYEN);
        }

        // GET: XacNhan/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XAC_NHAN_DIEU_CHUYEN xAC_NHAN_DIEU_CHUYEN = await db.XAC_NHAN_DIEU_CHUYEN.FindAsync(id);
            if (xAC_NHAN_DIEU_CHUYEN == null)
            {
                return HttpNotFound();
            }
            ViewBag.MA_DIEU_CHUYEN = new SelectList(db.DIEU_CHUYEN_THIET_BI, "MA_DIEU_CHUYEN", "MANS_QL", xAC_NHAN_DIEU_CHUYEN.MA_DIEU_CHUYEN);
            return View(xAC_NHAN_DIEU_CHUYEN);
        }

        // POST: XacNhan/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MA_XAC_NHAN,THOI_GIAN_XAC_NHAN,MAND_XAC_NHAN,MA_DIEU_CHUYEN")] XAC_NHAN_DIEU_CHUYEN xAC_NHAN_DIEU_CHUYEN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(xAC_NHAN_DIEU_CHUYEN).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MA_DIEU_CHUYEN = new SelectList(db.DIEU_CHUYEN_THIET_BI, "MA_DIEU_CHUYEN", "MANS_QL", xAC_NHAN_DIEU_CHUYEN.MA_DIEU_CHUYEN);
            return View(xAC_NHAN_DIEU_CHUYEN);
        }

        // GET: XacNhan/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XAC_NHAN_DIEU_CHUYEN xAC_NHAN_DIEU_CHUYEN = await db.XAC_NHAN_DIEU_CHUYEN.FindAsync(id);
            if (xAC_NHAN_DIEU_CHUYEN == null)
            {
                return HttpNotFound();
            }
            return View(xAC_NHAN_DIEU_CHUYEN);
        }

        // POST: XacNhan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            XAC_NHAN_DIEU_CHUYEN xAC_NHAN_DIEU_CHUYEN = await db.XAC_NHAN_DIEU_CHUYEN.FindAsync(id);
            db.XAC_NHAN_DIEU_CHUYEN.Remove(xAC_NHAN_DIEU_CHUYEN);
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
