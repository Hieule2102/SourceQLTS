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
    public class ThongKeThietBiController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /ThongKeThietBi/
        public async Task<ActionResult> Index()
        {
            //Đơn vị
            var dsTenDonVi = new List<string>();
            var qTenDonVi = (from d in db.DON_VI
                             orderby d.TEN_DON_VI
                             select d.TEN_DON_VI);

            dsTenDonVi.AddRange(qTenDonVi.Distinct());
            ViewBag.donVi = new SelectList(dsTenDonVi);

            //Nhóm thiết bị
            var dsNhomTB = new List<string>();
            var qNhomTB = (from d in db.NHOM_THIETBI
                           orderby d.TEN_NHOM
                           select d.TEN_NHOM);

            dsNhomTB.AddRange(qNhomTB.Distinct());
            ViewBag.nhomTB = new SelectList(dsNhomTB);

            //Loai thiết bị
            var dsLoaiTB = new List<string>();
            var qLoaiTB = (from d in db.LOAI_THIETBI
                           orderby d.TEN_LOAI
                           select d.TEN_LOAI);

            dsLoaiTB.AddRange(qLoaiTB.Distinct());
            ViewBag.loaiTB = new SelectList(dsLoaiTB);


            //Trạng thái
            var dsTrangThai = new List<string>();
            var qTrangThai = (from d in db.THIETBIs
                              orderby d.TINH_TRANG
                              select d.TINH_TRANG);

            dsTrangThai.AddRange(qTrangThai.Distinct());
            ViewBag.trangThai = new SelectList(dsTrangThai);

            var thietbis = db.THIETBIs.Include(t => t.DON_VI).Include(t => t.LOAI_THIETBI).Include(t => t.NHA_CUNG_CAP);
            return View(await thietbis.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string donVi, string nhomTB, string trangThai, string loaiTB)
        {
            //Đơn vị
            var dsTenDonVi = new List<string>();
            var qTenDonVi = (from d in db.DON_VI
                             orderby d.TEN_DON_VI
                             select d.TEN_DON_VI);

            dsTenDonVi.AddRange(qTenDonVi.Distinct());
            ViewBag.donVi = new SelectList(dsTenDonVi);

            //Nhóm thiết bị
            var dsNhomTB = new List<string>();
            var qNhomTB = (from d in db.NHOM_THIETBI
                           orderby d.TEN_NHOM
                           select d.TEN_NHOM);

            dsNhomTB.AddRange(qNhomTB.Distinct());
            ViewBag.nhomTB = new SelectList(dsNhomTB);

            //Loai thiết bị
            var dsLoaiTB = new List<string>();
            var qLoaiTB = (from d in db.LOAI_THIETBI
                           orderby d.TEN_LOAI
                           select d.TEN_LOAI);

            dsLoaiTB.AddRange(qLoaiTB.Distinct());
            ViewBag.loaiTB = new SelectList(dsLoaiTB);

            //Trạng thái
            var dsTrangThai = new List<string>();
            var qTrangThai = (from d in db.THIETBIs
                              orderby d.TINH_TRANG
                              select d.TINH_TRANG);

            dsTrangThai.AddRange(qTrangThai.Distinct());
            ViewBag.trangThai = new SelectList(dsTrangThai);

            var thietbis = db.THIETBIs.Include(t => t.DON_VI).Include(t => t.LOAI_THIETBI).Include(t => t.NHA_CUNG_CAP);

            //Tìm đơn vị
            if (!String.IsNullOrEmpty(donVi))
            {
                thietbis = thietbis.Where(data => data.DON_VI.TEN_DON_VI == donVi);
            }
            //Tìm nhóm thiết bị
            if (!String.IsNullOrEmpty(nhomTB))
            {
                thietbis = thietbis.Where(data => data.LOAI_THIETBI.NHOM_THIETBI.TEN_NHOM == nhomTB);
            }
            //Tìm trạng thái
            if (!String.IsNullOrEmpty(trangThai))
            {
                thietbis = thietbis.Where(data => data.TINH_TRANG == trangThai);
            }
            //Tìm loại thiết bị
            if (!String.IsNullOrEmpty(loaiTB))
            {
                thietbis = thietbis.Where(data => data.LOAI_THIETBI.TEN_LOAI == loaiTB);
            }

            return View(await thietbis.ToListAsync());
        }

        [HttpPost]
        public ActionResult get_LOAITB(string nhom_TB)
        {
            var dsLoaiTB = new List<string>();
            //Tìm loại thiết bị
            if (!String.IsNullOrEmpty(nhom_TB))
            {
                var qLoaiTB = (from d in db.LOAI_THIETBI
                               where d.NHOM_THIETBI.TEN_NHOM == nhom_TB
                               orderby d.TEN_LOAI
                               select d.TEN_LOAI);
                dsLoaiTB.AddRange(qLoaiTB.ToList());
            }

            return Json(dsLoaiTB, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult get_NHOMTB(string nhom_TB)
        {
            var dsLoaiTB = new List<string>();
            //Tìm loại thiết bị
            if (!String.IsNullOrEmpty(nhom_TB))
            {
                var qLoaiTB = (from d in db.LOAI_THIETBI
                               where d.NHOM_THIETBI.TEN_NHOM == nhom_TB
                               orderby d.TEN_LOAI
                               select d.TEN_LOAI);
                dsLoaiTB.AddRange(qLoaiTB.ToList());
            }

            return Json(dsLoaiTB, JsonRequestBehavior.AllowGet);
        }

        // GET: /ThongKeThietBi/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            THIETBI thietbi = await db.THIETBIs.FindAsync(id);
            if (thietbi == null)
            {
                return HttpNotFound();
            }
            return View(thietbi);
        }

        // GET: /ThongKeThietBi/Create
        public ActionResult Create()
        {
            ViewBag.MA_DV = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI");
            ViewBag.MA_LOAITB = new SelectList(db.LOAI_THIETBI, "MA_LOAITB", "TEN_LOAI");
            ViewBag.MA_NHA_CUNG_CAP = new SelectList(db.NHA_CUNG_CAP, "MA_NCC", "TEN_NCC");
            return View();
        }

        // POST: /ThongKeThietBi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MATB,TENTB,SO_SERIAL,GIA_TIEN,THOI_HAN_BAO_HANH,TINH_TRANG,MA_LOAITB,MANS_QL,MA_DV,MA_NHA_CUNG_CAP,NGAY_GD")] THIETBI thietbi)
        {
            if (ModelState.IsValid)
            {
                db.THIETBIs.Add(thietbi);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MA_DV = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", thietbi.MA_DV);
            ViewBag.MA_LOAITB = new SelectList(db.LOAI_THIETBI, "MA_LOAITB", "TEN_LOAI", thietbi.MA_LOAITB);
            ViewBag.MA_NHA_CUNG_CAP = new SelectList(db.NHA_CUNG_CAP, "MA_NCC", "TEN_NCC", thietbi.MA_NCC);
            return View(thietbi);
        }

        // GET: /ThongKeThietBi/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            THIETBI thietbi = await db.THIETBIs.FindAsync(id);
            if (thietbi == null)
            {
                return HttpNotFound();
            }
            ViewBag.MA_DV = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", thietbi.MA_DV);
            ViewBag.MA_LOAITB = new SelectList(db.LOAI_THIETBI, "MA_LOAITB", "TEN_LOAI", thietbi.MA_LOAITB);
            ViewBag.MA_NHA_CUNG_CAP = new SelectList(db.NHA_CUNG_CAP, "MA_NCC", "TEN_NCC", thietbi.MA_NCC);
            return View(thietbi);
        }

        // POST: /ThongKeThietBi/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MATB,TENTB,SO_SERIAL,GIA_TIEN,THOI_HAN_BAO_HANH,TINH_TRANG,MA_LOAITB,MANS_QL,MA_DV,MA_NHA_CUNG_CAP,NGAY_GD")] THIETBI thietbi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thietbi).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MA_DV = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", thietbi.MA_DV);
            ViewBag.MA_LOAITB = new SelectList(db.LOAI_THIETBI, "MA_LOAITB", "TEN_LOAI", thietbi.MA_LOAITB);
            ViewBag.MA_NHA_CUNG_CAP = new SelectList(db.NHA_CUNG_CAP, "MA_NCC", "TEN_NCC", thietbi.MA_NCC);
            return View(thietbi);
        }

        // GET: /ThongKeThietBi/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            THIETBI thietbi = await db.THIETBIs.FindAsync(id);
            if (thietbi == null)
            {
                return HttpNotFound();
            }
            return View(thietbi);
        }

        // POST: /ThongKeThietBi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            THIETBI thietbi = await db.THIETBIs.FindAsync(id);
            db.THIETBIs.Remove(thietbi);
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
