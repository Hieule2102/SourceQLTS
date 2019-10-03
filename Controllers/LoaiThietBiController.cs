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
    public class LoaiThietBiController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /LoaiThietBi/
        public async Task<ActionResult> Index()
        {
            if (Session["DANH_MUC"] != null)
            {
                //Nhóm thiết bị
                var dsNhomTB = new List<string>();
                var qNhomTB = (from d in db.NHOM_THIETBI
                               orderby d.TEN_NHOM
                               select d.TEN_NHOM);
                dsNhomTB.AddRange(qNhomTB.Distinct());
                ViewBag.MA_NHOMTB = new SelectList(dsNhomTB);

                var pHAN_QUYEN = Session["NHOM_ND"].ToString();
                ViewBag.Them = db.NHOM_ND_CHUCNANG.Where(a => a.MA_CHUC_NANG == 5 &&
                                                         a.MA_QUYEN == 1 &&
                                                         a.MA_NHOM == pHAN_QUYEN).FirstOrDefault();

                ViewBag.Sua = db.NHOM_ND_CHUCNANG.Where(a => a.MA_CHUC_NANG == 5 &&
                                                        a.MA_QUYEN == 3 &&
                                                        a.MA_NHOM == pHAN_QUYEN).FirstOrDefault();
            }
            else
            {
                return HttpNotFound("You have no accesss permissions at this");
            }

            var loai_thietbi = db.LOAI_THIETBI.Include(l => l.NHOM_THIETBI);
            return View(await loai_thietbi.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index([Bind(Include = "MA_LOAITB,TEN_LOAI,MA_NHOMTB,GHI_CHU")] LOAI_THIETBI loai_thietbi, FormCollection form, string SAVE, string EDIT)
        {
            var pHAN_QUYEN = Session["NHOM_ND"].ToString();
            ViewBag.Them = db.NHOM_ND_CHUCNANG.Where(a => a.MA_CHUC_NANG == 5 &&
                                                     a.MA_QUYEN == 1 &&
                                                     a.MA_NHOM == pHAN_QUYEN).FirstOrDefault();

            ViewBag.Sua = db.NHOM_ND_CHUCNANG.Where(a => a.MA_CHUC_NANG == 5 &&
                                                    a.MA_QUYEN == 3 &&
                                                    a.MA_NHOM == pHAN_QUYEN).FirstOrDefault();

            if (!String.IsNullOrEmpty(SAVE))
            {
                if (db.LOAI_THIETBI.FirstOrDefault(a => a.MA_LOAITB == loai_thietbi.MA_LOAITB) != null)
                {
                    ViewBag.ErrorMessage = "Trùng mã loại thiết bị";
                }
                else if (ModelState.IsValid)
                {
                    loai_thietbi.MA_NHOMTB = (from m in db.NHOM_THIETBI
                                              where m.TEN_NHOM == loai_thietbi.MA_NHOMTB
                                              select m.MA_NHOMTB).First();
                    db.LOAI_THIETBI.Add(loai_thietbi);
                    await db.SaveChangesAsync();
                    ViewBag.ErrorMessage = "Thêm thành công";

                }

            }
            else if (!String.IsNullOrEmpty(EDIT))
            {
                LOAI_THIETBI edit_LOAITB = db.LOAI_THIETBI.Where(a => a.MA_LOAITB == loai_thietbi.MA_LOAITB).FirstOrDefault();
                edit_LOAITB.TEN_LOAI = loai_thietbi.TEN_LOAI;
                edit_LOAITB.MA_NHOMTB = (from m in db.NHOM_THIETBI
                                         where m.TEN_NHOM == loai_thietbi.MA_NHOMTB
                                         select m.MA_NHOMTB).FirstOrDefault();
                edit_LOAITB.GHI_CHU = form["GHI_CHU"];

                if (ModelState.IsValid)
                {
                    db.Entry(edit_LOAITB).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    ViewBag.ErrorMessage = "Sửa thành công";
                }
            }

            return View(await db.LOAI_THIETBI.Include(l => l.NHOM_THIETBI).ToListAsync());
        }

        // GET: /LoaiThietBi/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOAI_THIETBI loai_thietbi = await db.LOAI_THIETBI.FindAsync(id);
            if (loai_thietbi == null)
            {
                return HttpNotFound();
            }
            return View(loai_thietbi);
        }

        // GET: /LoaiThietBi/Create
        public ActionResult Create()
        {
            ViewBag.MA_NHOMTB = new SelectList(db.NHOM_THIETBI, "MA_NHOMTB", "TEN_NHOM");
            return View();
        }

        // POST: /LoaiThietBi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MA_LOAITB,TEN_LOAI,MA_NHOMTB,GHI_CHU")] LOAI_THIETBI loai_thietbi, FormCollection form)
        {
            if (db.LOAI_THIETBI.Count(a => a.MA_LOAITB == loai_thietbi.MA_LOAITB) > 0)
            {
                return new HttpStatusCodeResult(404, "Trùng mã loại thiết bị");
            }
            else if (ModelState.IsValid)
            {
                loai_thietbi.MA_NHOMTB = (from m in db.NHOM_THIETBI
                                          where m.TEN_NHOM == loai_thietbi.MA_NHOMTB
                                          select m.MA_NHOMTB).First();
                db.LOAI_THIETBI.Add(loai_thietbi);
                await db.SaveChangesAsync();

            }
            return RedirectToAction("Index");
            //ViewBag.MA_NHOMTB = new SelectList(db.NHOM_THIETBI, "MA_NHOMTB", "TEN_NHOM", loai_thietbi.MA_NHOMTB);
            //return View(loai_thietbi);
        }

        // GET: /LoaiThietBi/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOAI_THIETBI loai_thietbi = await db.LOAI_THIETBI.FindAsync(id);
            if (loai_thietbi == null)
            {
                return HttpNotFound();
            }
            ViewBag.MA_NHOMTB = new SelectList(db.NHOM_THIETBI, "MA_NHOMTB", "TEN_NHOM", loai_thietbi.MA_NHOMTB);
            return View(loai_thietbi);
        }

        // POST: /LoaiThietBi/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MA_LOAITB,TEN_LOAI,MA_NHOMTB,GHI_CHU")] LOAI_THIETBI loai_thietbi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loai_thietbi).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MA_NHOMTB = new SelectList(db.NHOM_THIETBI, "MA_NHOMTB", "TEN_NHOM", loai_thietbi.MA_NHOMTB);
            return View(loai_thietbi);
        }

        // GET: /LoaiThietBi/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOAI_THIETBI loai_thietbi = await db.LOAI_THIETBI.FindAsync(id);
            if (loai_thietbi == null)
            {
                return HttpNotFound();
            }
            return View(loai_thietbi);
        }

        // POST: /LoaiThietBi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            LOAI_THIETBI loai_thietbi = await db.LOAI_THIETBI.FindAsync(id);
            db.LOAI_THIETBI.Remove(loai_thietbi);
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
