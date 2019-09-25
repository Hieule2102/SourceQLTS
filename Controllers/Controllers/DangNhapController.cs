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
    public class DangNhapController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /DangNhap/
        public ActionResult Index(FormCollection form)
        {
            var temp = form["TEN_DANG_NHAP"];

            if (temp != null)
            {
                var temp1 = form["MAT_KHAU"];
                if (db.NGUOI_DUNG.FirstOrDefault(x => x.TEN_DANG_NHAP == temp) == null)
                {
                    ViewBag.ErrorMessage = "Tài khoản không tồn tại";
                }
                else if (db.NGUOI_DUNG.FirstOrDefault(x => x.MAT_KHAU == temp1) == null)
                {
                    ViewBag.ErrorMessage = "Mật khẩu không đúng";
                }
                else
                {
                    Session["TEN_DANG_NHAP"] = (from b in db.NGUOI_DUNG
                                                where b.TEN_DANG_NHAP == temp
                                                select b.TEN_DANG_NHAP).FirstOrDefault();

                    var ma_ND = (from b in db.NGUOI_DUNG
                                 where b.TEN_DANG_NHAP == temp
                                 select b.MA_ND).FirstOrDefault();

                    Session["NHOM_ND"] = (from b in db.NHOM_ND
                                          where b.MA_ND == ma_ND
                                          select b.MA_NHOM).FirstOrDefault();
                    return View("~/Views/Home/Index.cshtml");
                }
            }
            return View();

        }

        public ActionResult Logout()
        {
            Session.RemoveAll();
            //return View("~/Views/Home/Index.cshtml");
            return RedirectToAction("Index", "Home");
        }


        // GET: /DangNhap/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NGUOI_DUNG nguoi_dung = await db.NGUOI_DUNG.FindAsync(id);
            if (nguoi_dung == null)
            {
                return HttpNotFound();
            }
            return View(nguoi_dung);
        }

        // GET: /DangNhap/Create
        public ActionResult Create()
        {
            ViewBag.MA_DON_VI = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI");
            return View();
        }

        // POST: /DangNhap/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MA_ND,TEN_ND,MA_DON_VI,EMAIL,TEN_DANG_NHAP,MAT_KHAU")] NGUOI_DUNG nguoi_dung)
        {
            if (ModelState.IsValid)
            {
                db.NGUOI_DUNG.Add(nguoi_dung);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MA_DON_VI = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", nguoi_dung.MA_DON_VI);
            return View(nguoi_dung);
        }

        // GET: /DangNhap/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NGUOI_DUNG nguoi_dung = await db.NGUOI_DUNG.FindAsync(id);
            if (nguoi_dung == null)
            {
                return HttpNotFound();
            }
            ViewBag.MA_DON_VI = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", nguoi_dung.MA_DON_VI);
            return View(nguoi_dung);
        }

        // POST: /DangNhap/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MA_ND,TEN_ND,MA_DON_VI,EMAIL,TEN_DANG_NHAP,MAT_KHAU")] NGUOI_DUNG nguoi_dung)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nguoi_dung).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MA_DON_VI = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", nguoi_dung.MA_DON_VI);
            return View(nguoi_dung);
        }

        // GET: /DangNhap/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NGUOI_DUNG nguoi_dung = await db.NGUOI_DUNG.FindAsync(id);
            if (nguoi_dung == null)
            {
                return HttpNotFound();
            }
            return View(nguoi_dung);
        }

        // POST: /DangNhap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            NGUOI_DUNG nguoi_dung = await db.NGUOI_DUNG.FindAsync(id);
            db.NGUOI_DUNG.Remove(nguoi_dung);
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
