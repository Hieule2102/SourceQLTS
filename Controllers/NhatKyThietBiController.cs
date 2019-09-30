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

            }
            else
            {
                return HttpNotFound("You have no accesss permissions at this");
            }
            var nHAT_KY_THIET_BI = db.NHAT_KY_THIET_BI.Include(n => n.THIETBI);
            return View(await nHAT_KY_THIET_BI.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string TINH_TRANG, string SEARCH_STRING)
        {
            var nHAT_KY_THIET_BI = db.NHAT_KY_THIET_BI.Include(n => n.THIETBI);
            //Tìm tên thiết bị
            if (!String.IsNullOrEmpty(SEARCH_STRING))
            {
                nHAT_KY_THIET_BI = nHAT_KY_THIET_BI.Where(data => data.THIETBI.TENTB.Contains(SEARCH_STRING));
                ViewBag.MATB = nHAT_KY_THIET_BI.Select(a => a.MATB).FirstOrDefault();
                ViewBag.TENTB = nHAT_KY_THIET_BI.Select(a => a.THIETBI.TENTB).FirstOrDefault();
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
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB");
            return View();
        }

        // POST: NhatKyThietBi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MA_NHAT_KY,MATB,TINH_TRANG,NGAY_THUC_HIEN")] NHAT_KY_THIET_BI nHAT_KY_THIET_BI)
        {
            if (ModelState.IsValid)
            {
                db.NHAT_KY_THIET_BI.Add(nHAT_KY_THIET_BI);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB", nHAT_KY_THIET_BI.MATB);
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
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB", nHAT_KY_THIET_BI.MATB);
            return View(nHAT_KY_THIET_BI);
        }

        // POST: NhatKyThietBi/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MA_NHAT_KY,MATB,TINH_TRANG,NGAY_THUC_HIEN")] NHAT_KY_THIET_BI nHAT_KY_THIET_BI)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nHAT_KY_THIET_BI).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB", nHAT_KY_THIET_BI.MATB);
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
