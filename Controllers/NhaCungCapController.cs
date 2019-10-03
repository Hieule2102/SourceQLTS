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
    public class NhaCungCapController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /NhaCungCap/
        public async Task<ActionResult> Index()
        {
            if (Session["DANH_MUC"] != null)
            {
                var pHAN_QUYEN = Session["NHOM_ND"].ToString();
                ViewBag.Them = db.NHOM_ND_CHUCNANG.Where(a => a.MA_CHUC_NANG == 7 &&
                                                         a.MA_QUYEN == 1 &&
                                                         a.MA_NHOM == pHAN_QUYEN).FirstOrDefault();

                ViewBag.Sua = db.NHOM_ND_CHUCNANG.Where(a => a.MA_CHUC_NANG == 7 &&
                                                        a.MA_QUYEN == 3 &&
                                                        a.MA_NHOM == pHAN_QUYEN).FirstOrDefault();
            }
            else
            {
                return HttpNotFound("You have no accesss permissions at this");
            }

            return View(await db.NHA_CUNG_CAP.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(FormCollection form, string SAVE, string EDIT)
        {
            var pHAN_QUYEN = Session["NHOM_ND"].ToString();
            ViewBag.Them = db.NHOM_ND_CHUCNANG.Where(a => a.MA_CHUC_NANG == 7 &&
                                                     a.MA_QUYEN == 1 &&
                                                     a.MA_NHOM == pHAN_QUYEN).FirstOrDefault();

            ViewBag.Sua = db.NHOM_ND_CHUCNANG.Where(a => a.MA_CHUC_NANG == 7 &&
                                                    a.MA_QUYEN == 3 &&
                                                    a.MA_NHOM == pHAN_QUYEN).FirstOrDefault();

            if (!String.IsNullOrEmpty(SAVE))
            {
                if (String.IsNullOrEmpty(form["TEN_NCC"]))
                {
                    ViewBag.ErrorMessage = "Xin nhập đầy đủ thông tin";
                }
                else
                {
                    var nha_cung_cap = new NHA_CUNG_CAP();
                    nha_cung_cap.TEN_NCC = form["TEN_NCC"];
                    nha_cung_cap.DIA_CHI = form["DIA_CHI"];
                    nha_cung_cap.DIEN_THOAI = form["DIEN_THOAI"];
                    nha_cung_cap.FAX = form["FAX"];
                    nha_cung_cap.GHI_CHU = form["GHI_CHU"];

                    if (ModelState.IsValid)
                    {
                        db.NHA_CUNG_CAP.Add(nha_cung_cap);
                        ViewBag.ErrorMessage = "Thêm thành công!!";
                        await db.SaveChangesAsync();
                    }
                }

            }
            else if (!String.IsNullOrEmpty(EDIT))
            {
                var temp = Int32.Parse(form["MA_NCC"].ToString());
                NHA_CUNG_CAP edit_nCC = db.NHA_CUNG_CAP.Where(a => a.MA_NCC == temp).FirstOrDefault();
                edit_nCC.TEN_NCC = form["TEN_NCC"];
                edit_nCC.DIA_CHI = form["DIA_CHI"];
                edit_nCC.DIEN_THOAI = form["DIEN_THOAI"];
                edit_nCC.FAX = form["FAX"];
                edit_nCC.GHI_CHU = form["GHI_CHU"];

                if (ModelState.IsValid)
                {
                    db.Entry(edit_nCC).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    ViewBag.ErrorMessage = "Sửa thành công";
                }
            }

            return View(await db.NHA_CUNG_CAP.ToListAsync());
        }

        // GET: /NhaCungCap/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHA_CUNG_CAP nha_cung_cap = await db.NHA_CUNG_CAP.FindAsync(id);
            if (nha_cung_cap == null)
            {
                return HttpNotFound();
            }
            return View(nha_cung_cap);
        }

        // GET: /NhaCungCap/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /NhaCungCap/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TEN_NCC,DIA_CHI,DIEN_THOAI,FAX,GHI_CHU")] NHA_CUNG_CAP nha_cung_cap)
        {
            //var nha_cung_cap = new NHA_CUNG_CAP();
            //nha_cung_cap.TEN_NCC = form["TEN_NCC"];
            //nha_cung_cap.DIA_CHI = form["DIA_CHI"];
            //nha_cung_cap.DIEN_THOAI = form["DIEN_THOAI"];
            //nha_cung_cap.FAX = form["FAX"];
            //nha_cung_cap.GHI_CHU = form["GHI_CHU"];
            if (ModelState.IsValid)
            {
                db.NHA_CUNG_CAP.Add(nha_cung_cap);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // GET: /NhaCungCap/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHA_CUNG_CAP nha_cung_cap = await db.NHA_CUNG_CAP.FindAsync(id);
            if (nha_cung_cap == null)
            {
                return HttpNotFound();
            }
            return View(nha_cung_cap);
        }

        // POST: /NhaCungCap/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MA_NCC,TEN_NCC,MST,DIA_CHI,DIEN_THOAI,FAX,GHI_CHU")] NHA_CUNG_CAP nha_cung_cap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nha_cung_cap).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(nha_cung_cap);
        }

        // GET: /NhaCungCap/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHA_CUNG_CAP nha_cung_cap = await db.NHA_CUNG_CAP.FindAsync(id);
            if (nha_cung_cap == null)
            {
                return HttpNotFound();
            }
            return View(nha_cung_cap);
        }

        // POST: /NhaCungCap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            NHA_CUNG_CAP nha_cung_cap = await db.NHA_CUNG_CAP.FindAsync(id);
            db.NHA_CUNG_CAP.Remove(nha_cung_cap);
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
