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
using System.Web.Routing;

namespace Source.Controllers
{
    public class DanhSachThietBiController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /DanhSachThietBi/
        public async Task<ActionResult> Index()
        {
            //Nhóm thiết bị
            var dsLOAITB = new List<string>();
            var qLOAITB = (from d in db.LOAI_THIETBI
                           orderby d.TEN_LOAI
                           select d.TEN_LOAI);
            dsLOAITB.AddRange(qLOAITB.Distinct());
            ViewBag.MA_LOAITB = new SelectList(dsLOAITB);

            //Nhóm thiết bị
            var dsNhomTB = new List<string>();
            var qNhomTB = (from d in db.NHOM_THIETBI
                           orderby d.TEN_NHOM
                           select d.TEN_NHOM);
            dsNhomTB.AddRange(qNhomTB.Distinct());
            ViewBag.MA_NHOMTB = new SelectList(dsNhomTB);

            //Đơn vị
            var dsTenDonVi = new List<string>();
            var qTenDonVi = (from d in db.DON_VI
                             orderby d.TEN_DON_VI
                             select d.TEN_DON_VI);
            dsTenDonVi.AddRange(qTenDonVi.Distinct());
            ViewBag.MA_DON_VI = new SelectList(dsTenDonVi);

            //if (!String.IsNullOrEmpty(Session["BAO_CAO"].ToString()))
            //{
            //    var temp = Session["NHOM_ND"].ToString();
            //    ViewBag.Them = db.NHOM_ND_CHUCNANG.Where(a => a.MA_CHUC_NANG == 8 &&
            //                                             a.MA_QUYEN == 4 &&
            //                                             a.MA_NHOM == temp).FirstOrDefault();

            //    ViewBag.Sua = db.NHOM_ND_CHUCNANG.Where(a => a.MA_CHUC_NANG == 3 &&
            //                                            a.MA_QUYEN == 3 &&
            //                                            a.MA_NHOM == temp).FirstOrDefault();
            //}

            var thietbis = db.THIETBIs.Include(t => t.DON_VI).Include(t => t.LOAI_THIETBI).Include(t => t.NHA_CUNG_CAP);
            return View(await thietbis.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string searchString, string MA_LOAITB, string MA_DON_VI, string MA_NHOMTB)
        {
            //Đơn vị
            var dsTenDonVi = new List<string>();
            var qTenDonVi = (from d in db.DON_VI
                             orderby d.TEN_DON_VI
                             select d.TEN_DON_VI);
            dsTenDonVi.AddRange(qTenDonVi.Distinct());
            ViewBag.MA_DON_VI = new SelectList(dsTenDonVi);

            var dsLOAITB = new List<string>();
            var qLOAITB = (from d in db.LOAI_THIETBI
                           orderby d.TEN_LOAI
                           select d.TEN_LOAI);
            

            //Nhóm thiết bị
            var dsNhomTB = new List<string>();
            var qNhomTB = (from d in db.NHOM_THIETBI
                           orderby d.TEN_NHOM
                           select d.TEN_NHOM);
            dsNhomTB.AddRange(qNhomTB.Distinct());
            ViewBag.MA_NHOMTB = new SelectList(dsNhomTB);

            var thietbis = db.THIETBIs.Include(t => t.DON_VI).Include(t => t.LOAI_THIETBI).Include(t => t.NHA_CUNG_CAP);

            //Tìm kiếm loại thiết bị
            if (!String.IsNullOrEmpty(MA_NHOMTB))
            {
                //Nhóm thiết bị
                qLOAITB = (from d in db.LOAI_THIETBI
                           where d.NHOM_THIETBI.TEN_NHOM == MA_NHOMTB
                           orderby d.TEN_LOAI
                           select d.TEN_LOAI);
                if (!String.IsNullOrEmpty(MA_LOAITB))
                {
                    thietbis = thietbis.Where(data => data.LOAI_THIETBI.TEN_LOAI == MA_LOAITB);
                }
                else
                {
                    thietbis = thietbis.Where(data => data.LOAI_THIETBI.NHOM_THIETBI.TEN_NHOM == MA_NHOMTB);
                }
            }
            else if (!String.IsNullOrEmpty(MA_LOAITB))
            {
                thietbis = thietbis.Where(data => data.LOAI_THIETBI.TEN_LOAI == MA_LOAITB);
            }
            //Tìm tên thiết bị
            else if (!String.IsNullOrEmpty(searchString))
            {
                thietbis = thietbis.Where(data => data.TENTB.Contains(searchString));
            }
            //Tìm đơn vị
            else if (!String.IsNullOrEmpty(MA_DON_VI))
            {
                thietbis = thietbis.Where(data => data.DON_VI.TEN_DON_VI == MA_DON_VI);
            }

            dsLOAITB.AddRange(qLOAITB.Distinct());
            ViewBag.MA_LOAITB = new SelectList(dsLOAITB);
            return View(await thietbis.ToListAsync());
        }

        // GET: /DanhSachThietBi/Details/5
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

        // GET: /DanhSachThietBi/Create
        public ActionResult Create()
        {
            ViewBag.MA_DV = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI");
            ViewBag.MA_LOAITB = new SelectList(db.LOAI_THIETBI, "MA_LOAITB", "TEN_LOAI");
            ViewBag.MA_NCC = new SelectList(db.NHA_CUNG_CAP, "MA_NCC", "TEN_NCC");
            return View();
        }

        // POST: /DanhSachThietBi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MATB,TENTB,SO_SERIAL,GIA_TIEN,THOI_HAN_BAO_HANH,TINH_TRANG,MA_LOAITB,MANS_QL,MA_DV,MA_NCC,NGAY_GD")] THIETBI thietbi)
        {
            if (ModelState.IsValid)
            {
                db.THIETBIs.Add(thietbi);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MA_DV = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", thietbi.MA_DV);
            ViewBag.MA_LOAITB = new SelectList(db.LOAI_THIETBI, "MA_LOAITB", "TEN_LOAI", thietbi.MA_LOAITB);
            ViewBag.MA_NCC = new SelectList(db.NHA_CUNG_CAP, "MA_NCC", "TEN_NCC", thietbi.MA_NCC);
            return View(thietbi);
        }

        // GET: /DanhSachThietBi/Edit/5
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
            ViewBag.MA_NCC = new SelectList(db.NHA_CUNG_CAP, "MA_NCC", "TEN_NCC", thietbi.MA_NCC);
            return View(thietbi);
        }

        // POST: /DanhSachThietBi/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MATB,TENTB,SO_SERIAL,GIA_TIEN,THOI_HAN_BAO_HANH,TINH_TRANG,MA_LOAITB,MANS_QL,MA_DV,MA_NCC,NGAY_GD")] THIETBI thietbi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thietbi).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MA_DV = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", thietbi.MA_DV);
            ViewBag.MA_LOAITB = new SelectList(db.LOAI_THIETBI, "MA_LOAITB", "TEN_LOAI", thietbi.MA_LOAITB);
            ViewBag.MA_NCC = new SelectList(db.NHA_CUNG_CAP, "MA_NCC", "TEN_NCC", thietbi.MA_NCC);
            return View(thietbi);
        }

        // GET: /DanhSachThietBi/Delete/5
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

        // POST: /DanhSachThietBi/Delete/5
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
