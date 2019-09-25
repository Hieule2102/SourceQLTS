using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Source.Models;

namespace Source.Controllers
{
    public class DoiMatKhauController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /DoiMatKhau/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string MAT_KHAU_CU, string MAT_KHAU_MOI, string XAC_NHAN_MAT_KHAU)
        {
            if (String.IsNullOrEmpty(MAT_KHAU_CU) || String.IsNullOrEmpty(MAT_KHAU_MOI) || String.IsNullOrEmpty(XAC_NHAN_MAT_KHAU))
            {
                ViewBag.ErrorMessage = "Xin nhập đầy đủ thông tin";
                return View();
            }
            string temp = Session["TEN_DANG_NHAP"].ToString();
            NGUOI_DUNG nguoi_Dung = db.NGUOI_DUNG.FirstOrDefault(x => x.TEN_DANG_NHAP == temp);
            if (nguoi_Dung.MAT_KHAU != MAT_KHAU_CU)
            {
                ViewBag.ErrorMessage = "Mật khẩu cũ không đúng";
            }
            else if (MAT_KHAU_MOI != XAC_NHAN_MAT_KHAU)
            {
                ViewBag.ErrorMessage = "Mật khẩu xác nhận không trùng với mật khẩu mới";
            }
            else
            {
                nguoi_Dung.MAT_KHAU = MAT_KHAU_MOI;

                if (ModelState.IsValid)
                {
                    db.Entry(nguoi_Dung).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.ErrorMessage = "Mật khẩu thay đổi thành công";
                }
            }

            return View();
        }

        // GET: /DoiMatKhau/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NGUOI_DUNG nguoi_dung = db.NGUOI_DUNG.Find(id);
            if (nguoi_dung == null)
            {
                return HttpNotFound();
            }
            return View(nguoi_dung);
        }

        // GET: /DoiMatKhau/Create
        public ActionResult Create()
        {
            ViewBag.MA_DON_VI = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI");
            return View();
        }

        // POST: /DoiMatKhau/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MA_ND,TEN_ND,MA_DON_VI,EMAIL,TEN_DANG_NHAP,MAT_KHAU")] NGUOI_DUNG nguoi_dung)
        {
            if (ModelState.IsValid)
            {
                db.NGUOI_DUNG.Add(nguoi_dung);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MA_DON_VI = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", nguoi_dung.MA_DON_VI);
            return View(nguoi_dung);
        }

        // GET: /DoiMatKhau/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NGUOI_DUNG nguoi_dung = db.NGUOI_DUNG.Find(id);
            if (nguoi_dung == null)
            {
                return HttpNotFound();
            }
            ViewBag.MA_DON_VI = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", nguoi_dung.MA_DON_VI);
            return View(nguoi_dung);
        }

        // POST: /DoiMatKhau/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MA_ND,TEN_ND,MA_DON_VI,EMAIL,TEN_DANG_NHAP,MAT_KHAU")] NGUOI_DUNG nguoi_dung)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nguoi_dung).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MA_DON_VI = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", nguoi_dung.MA_DON_VI);
            return View(nguoi_dung);
        }

        // GET: /DoiMatKhau/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NGUOI_DUNG nguoi_dung = db.NGUOI_DUNG.Find(id);
            if (nguoi_dung == null)
            {
                return HttpNotFound();
            }
            return View(nguoi_dung);
        }

        // POST: /DoiMatKhau/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            NGUOI_DUNG nguoi_dung = db.NGUOI_DUNG.Find(id);
            db.NGUOI_DUNG.Remove(nguoi_dung);
            db.SaveChanges();
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
