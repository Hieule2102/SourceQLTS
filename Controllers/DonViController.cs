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
    public class DonViController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /DonVi/
        public async Task<ActionResult> Index()
        {
            return View(await db.DON_VI.Where(a => a.MA_DON_VI != 7).ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(FormCollection form, string SAVE, string EDIT)
        {
            if(!String.IsNullOrEmpty(SAVE))
            {
                if (String.IsNullOrEmpty(form["TEN_DON_VI"]))
                {
                    ViewBag.ErrorMessage = "Xin nhập tên đơn vị";
                }
                else
                {
                    DON_VI don_vi = new DON_VI();
                    don_vi.TEN_DON_VI = form["TEN_DON_VI"];
                    don_vi.DIA_CHI = form["DIA_CHI"];
                    don_vi.DIEN_THOAI = form["DIEN_THOAI"];
                    don_vi.FAX = form["FAX"];
                    if (!String.IsNullOrEmpty(form["DON_VI_CAP_TREN"]))
                    {
                        var temp = form["DON_VI_CAP_TREN"];
                        don_vi.DON_VI_CAP_TREN = (from a in db.DON_VI
                                                  where a.TEN_DON_VI == temp
                                                  select a.MA_DON_VI).FirstOrDefault();
                    }

                    if (ModelState.IsValid)
                    {
                        db.DON_VI.Add(don_vi);
                        await db.SaveChangesAsync();
                        ViewBag.ErrorMessage = "Thêm thành công!!";
                    }
                }
            }
            else if (!String.IsNullOrEmpty(EDIT))
            {
                var temp = Int32.Parse(form["MA_DON_VI"].ToString());
                DON_VI edit_DONVI = db.DON_VI.Where(a => a.MA_DON_VI == temp).FirstOrDefault();
                edit_DONVI.TEN_DON_VI = form["TEN_DON_VI"];
                edit_DONVI.DIA_CHI = form["DIA_CHI"];
                edit_DONVI.DIEN_THOAI = form["DIEN_THOAI"];
                edit_DONVI.FAX = form["FAX"];

                if (!String.IsNullOrEmpty(form["DON_VI_CAP_TREN"]))
                {
                    var temp1 = form["DON_VI_CAP_TREN"];
                    edit_DONVI.DON_VI_CAP_TREN = (from a in db.DON_VI
                                                  where a.TEN_DON_VI == temp1
                                                  select a.MA_DON_VI).FirstOrDefault();
                }
                else if (String.IsNullOrEmpty(form["DON_VI_CAP_TREN"]))
                {
                    edit_DONVI.DON_VI_CAP_TREN = 7;
                }

                if (ModelState.IsValid)
                {
                    db.Entry(edit_DONVI).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    ViewBag.ErrorMessage = "Sửa thành công";
                }
            }

            return View(await db.DON_VI.Where(a => a.MA_DON_VI != 7).ToListAsync());
        }

        // GET: /DonVi/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DON_VI don_vi = await db.DON_VI.FindAsync(id);
            if (don_vi == null)
            {
                return HttpNotFound();
            }
            return View(don_vi);
        }

        // GET: /DonVi/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /DonVi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MA_DON_VI,TEN_DON_VI,DIA_CHI,DIEN_THOAI,FAX")] DON_VI don_vi)
        {
            //var don_vi = new DON_VI();
            //don_vi.TEN_DON_VI = form["TEN_DON_VI"];
            //don_vi.DIA_CHI = form["DIA_CHI"];
            //don_vi.DIEN_THOAI = form["DIEN_THOAI"];
            //don_vi.FAX = form["FAX"];
            if (ModelState.IsValid)
            {
                db.DON_VI.Add(don_vi);
                await db.SaveChangesAsync();
                ViewBag.ErrorMessage = "Thêm thành công!!";
            }
            return RedirectToAction("Index");
        }

        // GET: /DonVi/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DON_VI don_vi = await db.DON_VI.FindAsync(id);
            if (don_vi == null)
            {
                return HttpNotFound();
            }
            return View(don_vi);
        }

        // POST: /DonVi/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MA_DON_VI,TEN_DON_VI,DIA_CHI,DIEN_THOAI,FAX")] DON_VI don_vi)
        {

            if (ModelState.IsValid)
            {
                db.Entry(don_vi).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(don_vi);
        }

        // GET: /DonVi/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DON_VI don_vi = await db.DON_VI.FindAsync(id);
            if (don_vi == null)
            {
                return HttpNotFound();
            }
            return View(don_vi);
        }

        // POST: /DonVi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DON_VI don_vi = await db.DON_VI.FindAsync(id);
            db.DON_VI.Remove(don_vi);
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
