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
    public class NhomThietBiController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /NhomThietBi/
        public async Task<ActionResult> Index()
        {
            return View(await db.NHOM_THIETBI.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index([Bind(Include = "MA_NHOMTB,TEN_NHOM,GHI_CHU")] NHOM_THIETBI nHOM_THIETBI, string SAVE, string EDIT)
        {
            if (!String.IsNullOrEmpty(SAVE))
            {
                if (db.NHOM_THIETBI.FirstOrDefault(a => a.MA_NHOMTB == nHOM_THIETBI.MA_NHOMTB) != null)
                {
                    ViewBag.ErrorMessage = "Trùng mã nhóm thiết bị";
                }
                else if (ModelState.IsValid)
                {
                    db.NHOM_THIETBI.Add(nHOM_THIETBI);
                    await db.SaveChangesAsync();
                    ViewBag.ErrorMessage = "Thêm thành công";
                }
            }
            else if (!String.IsNullOrEmpty(EDIT))
            {
                NHOM_THIETBI edit_nHOM_THIETBI = db.NHOM_THIETBI.Where(a => a.MA_NHOMTB == nHOM_THIETBI.MA_NHOMTB).FirstOrDefault();
                edit_nHOM_THIETBI.TEN_NHOM = nHOM_THIETBI.TEN_NHOM;
                edit_nHOM_THIETBI.GHI_CHU = nHOM_THIETBI.GHI_CHU;

                if (ModelState.IsValid)
                {
                    db.Entry(edit_nHOM_THIETBI).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    ViewBag.ErrorMessage = "Sửa thành công";
                }
            }
            
            return View(await db.NHOM_THIETBI.ToListAsync());
        }

        // GET: /NhomThietBi/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHOM_THIETBI nhom_thietbi = await db.NHOM_THIETBI.FindAsync(id);
            if (nhom_thietbi == null)
            {
                return HttpNotFound();
            }
            return View(nhom_thietbi);
        }

        // GET: /NhomThietBi/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /NhomThietBi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MA_NHOMTB,TEN_NHOM,GHI_CHU")] NHOM_THIETBI nhom_thietbi)
        {
            if (db.NHOM_THIETBI.FirstOrDefault(a => a.MA_NHOMTB == nhom_thietbi.MA_NHOMTB) != null)
            {
                ViewBag.ErrorMessage = "Tài khoản không tồn tại";
            }
            else if (ModelState.IsValid)
            {
                db.NHOM_THIETBI.Add(nhom_thietbi);
                await db.SaveChangesAsync();
            }
            return View("Index");
        }

        // GET: /NhomThietBi/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHOM_THIETBI nhom_thietbi = await db.NHOM_THIETBI.FindAsync(id);
            if (nhom_thietbi == null)
            {
                return HttpNotFound();
            }
            return View(nhom_thietbi);
        }

        // POST: /NhomThietBi/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MA_NHOMTB,TEN_NHOM,GHI_CHU")] NHOM_THIETBI nhom_thietbi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nhom_thietbi).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(nhom_thietbi);
        }

        // GET: /NhomThietBi/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHOM_THIETBI nhom_thietbi = await db.NHOM_THIETBI.FindAsync(id);
            if (nhom_thietbi == null)
            {
                return HttpNotFound();
            }
            return View(nhom_thietbi);
        }

        // POST: /NhomThietBi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            NHOM_THIETBI nhom_thietbi = await db.NHOM_THIETBI.FindAsync(id);
            db.NHOM_THIETBI.Remove(nhom_thietbi);
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
