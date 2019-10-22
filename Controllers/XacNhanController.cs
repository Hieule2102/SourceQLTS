using Source.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Source.Controllers
{
    public class XacNhanController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: XacNhan
        public async Task<ActionResult> Index()
        {
            if (Session["CHUC_NANG"] != null)
            {
                var pHAN_QUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == Session["NHOM_ND"].ToString()
                                                             && a.MA_CHUC_NANG == 4);

                ViewBag.XacNhan = pHAN_QUYEN.Where(a => a.MA_QUYEN == 5);                     
            }
            else
            {
                return HttpNotFound("You have no accesss permissions at this");
            }

            var xAC_NHAN = db.XAC_NHAN_DIEU_CHUYEN.Where(a => a.XAC_NHAN == false);
            var tEN_DANG_NHAP = Session["TEN_DANG_NHAP"].ToString();
            if (tEN_DANG_NHAP != "admin")
            {
                xAC_NHAN = xAC_NHAN.Where(a => (a.XUAT_KHO.NGUOI_DUNG1.TEN_DANG_NHAP == tEN_DANG_NHAP
                                             || a.DIEU_CHUYEN_THIET_BI.NGUOI_DUNG.TEN_DANG_NHAP == tEN_DANG_NHAP));
            }
            return View(await xAC_NHAN.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string XAC_NHAN, FormCollection form, string SEARCH_STRING)
        {
            var pHAN_QUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == Session["NHOM_ND"].ToString()
                                                            && a.MA_CHUC_NANG == 4);

            ViewBag.XacNhan = pHAN_QUYEN.Where(a => a.MA_QUYEN == 5);

            var xAC_NHAN_DIEU_CHUYEN = db.XAC_NHAN_DIEU_CHUYEN.Where(a => a.XAC_NHAN == false);

            var tEN_DANG_NHAP = Session["TEN_DANG_NHAP"].ToString();
            if (tEN_DANG_NHAP != "admin")
            {
                xAC_NHAN_DIEU_CHUYEN = xAC_NHAN_DIEU_CHUYEN.Where(a => (a.XUAT_KHO.NGUOI_DUNG1.TEN_DANG_NHAP == tEN_DANG_NHAP
                                                                     || a.DIEU_CHUYEN_THIET_BI.NGUOI_DUNG.TEN_DANG_NHAP == tEN_DANG_NHAP)
                                                                     && a.XAC_NHAN == false);
            }
            if (!String.IsNullOrEmpty(SEARCH_STRING))
            {
                xAC_NHAN_DIEU_CHUYEN = xAC_NHAN_DIEU_CHUYEN.Where(a => a.XUAT_KHO.THIETBI.TENTB.Contains(SEARCH_STRING)
                                                                    || a.DIEU_CHUYEN_THIET_BI.THIETBI.TENTB.Contains(SEARCH_STRING));
            }
            else if (!String.IsNullOrEmpty(XAC_NHAN))
            {
                #region Xác nhận
                var mA_XAC_NHAN = Int32.Parse(form["MA_XAC_NHAN"].ToString());
                var xAC_NHAN = db.XAC_NHAN_DIEU_CHUYEN.Where(a => a.MA_XAC_NHAN == mA_XAC_NHAN).FirstOrDefault();                

                var temp = Session["TEN_DANG_NHAP"].ToString();
                xAC_NHAN.MAND_XAC_NHAN = (from p in db.NGUOI_DUNG
                                          where p.TEN_DANG_NHAP == temp
                                          select p.MA_ND).FirstOrDefault();

                xAC_NHAN.XAC_NHAN = true;
                xAC_NHAN.THOI_GIAN_XAC_NHAN = DateTime.Now;
                #endregion

                #region Thay đổi trạng thái thiết bị
                var mATB = form["MATB_temp"].ToString();
                var tHIETBI = db.THIETBIs.Where(a => a.MATB == mATB).FirstOrDefault();
                tHIETBI.MAND_QL = xAC_NHAN.MAND_XAC_NHAN;
                tHIETBI.MA_DV = db.NGUOI_DUNG.Where(a => a.MA_ND == xAC_NHAN.MAND_XAC_NHAN)
                                             .Select(a => a.MA_DON_VI)
                                             .FirstOrDefault();
                tHIETBI.TINH_TRANG = "Đang sử dụng";
                #endregion

                if (ModelState.IsValid)
                {
                    db.Entry(tHIETBI).State = EntityState.Modified;
                    db.Entry(xAC_NHAN).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    ViewBag.ErrorMessage = "Xác nhận thành công";
                }

                #region Thêm vào nhật ký thiết bị
                NHAT_KY_THIET_BI nHAT_KY_THIET_BI = new NHAT_KY_THIET_BI();
                nHAT_KY_THIET_BI.MA_XAC_NHAN = mA_XAC_NHAN;
                nHAT_KY_THIET_BI.TINH_TRANG = "Đã xác nhận";
                db.NHAT_KY_THIET_BI.Add(nHAT_KY_THIET_BI);
                await db.SaveChangesAsync();
                #endregion

                xAC_NHAN_DIEU_CHUYEN = xAC_NHAN_DIEU_CHUYEN.Where(a => a.XAC_NHAN == false);
            }
            return View(await xAC_NHAN_DIEU_CHUYEN.ToListAsync());
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
