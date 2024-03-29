﻿using Source.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Source.Controllers
{
    public class NhomNguoiDungController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /NhomNguoiDung/
        public async Task<ActionResult> Index()
        {
            var nhom_nguoi_dung = db.NHOM_NGUOI_DUNG.Include(n => n.NHOM_ND_CHUCNANG);

            if (Session["QL_ND"] != null)
            {                
                var pHAN_QUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == Session["NHOM_ND"].ToString()
                                                             && a.MA_CHUC_NANG == 16);

                ViewBag.Them = pHAN_QUYEN.Where(a => a.MA_QUYEN == 1);
                ViewBag.Sua = pHAN_QUYEN.Where(a => a.MA_QUYEN == 3);
            }
            else
            {
                return HttpNotFound("You have no accesss permissions at this");
            }

            return View(await nhom_nguoi_dung.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index([Bind(Include = "MA_NHOM,TEN_NHOM,GHI_CHU")] NHOM_NGUOI_DUNG cREATE_NHOM_NGUOI_DUNG, string SAVE, string EDIT)
        {
            var pHAN_QUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == Session["NHOM_ND"].ToString()
                                                             && a.MA_CHUC_NANG == 16);

            ViewBag.Them = pHAN_QUYEN.Where(a => a.MA_QUYEN == 1);
            ViewBag.Sua = pHAN_QUYEN.Where(a => a.MA_QUYEN == 3);

            var nHOM_NGUOI_DUNG = db.NHOM_NGUOI_DUNG.Include(n => n.NHOM_ND_CHUCNANG);

            if (!String.IsNullOrEmpty(SAVE))
            {
                if (String.IsNullOrEmpty(cREATE_NHOM_NGUOI_DUNG.MA_NHOM) || String.IsNullOrEmpty(cREATE_NHOM_NGUOI_DUNG.TEN_NHOM))
                {
                    ViewBag.ErrorMessage = "Xin nhập đầy đủ thông tin";
                }
                else if (nHOM_NGUOI_DUNG.FirstOrDefault(a => a.MA_NHOM == cREATE_NHOM_NGUOI_DUNG.MA_NHOM) != null)
                {
                    ViewBag.ErrorMessage = "Trùng mã nhóm người dùng";
                }
                else if (ModelState.IsValid)
                {
                    db.NHOM_NGUOI_DUNG.Add(cREATE_NHOM_NGUOI_DUNG);
                    await db.SaveChangesAsync();

                    ViewBag.ErrorMessage = "Thêm thành công";
                }
            }
            else if (!String.IsNullOrEmpty(EDIT))
            {
                NHOM_NGUOI_DUNG eDIT_NHOM_NGUOI_DUNG = nHOM_NGUOI_DUNG.Where(a => a.MA_NHOM == cREATE_NHOM_NGUOI_DUNG.MA_NHOM).FirstOrDefault();
                eDIT_NHOM_NGUOI_DUNG.TEN_NHOM = cREATE_NHOM_NGUOI_DUNG.TEN_NHOM;
                eDIT_NHOM_NGUOI_DUNG.GHI_CHU = cREATE_NHOM_NGUOI_DUNG.GHI_CHU;

                if (ModelState.IsValid)
                {
                    db.Entry(eDIT_NHOM_NGUOI_DUNG).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    ViewBag.ErrorMessage = "Sửa thành công";
                }

            }
            return View(await nHOM_NGUOI_DUNG.ToListAsync());
        }

        // GET: /NhomNguoiDung/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHOM_NGUOI_DUNG nhom_nguoi_dung = await db.NHOM_NGUOI_DUNG.FindAsync(id);
            if (nhom_nguoi_dung == null)
            {
                return HttpNotFound();
            }
            return View(nhom_nguoi_dung);
        }

        // GET: /NhomNguoiDung/Create
        public ActionResult Create()
        {
            ViewBag.MA_NHOM = new SelectList(db.NHOM_ND_CHUCNANG, "MA_NHOM", "MA_NHOM");
            return View();
        }

        // POST: /NhomNguoiDung/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MA_NHOM,TEN_NHOM,GHI_CHU")] NHOM_NGUOI_DUNG nhom_nguoi_dung)
        {
            //var nhom_nguoi_dung = new NHOM_NGUOI_DUNG();
            //nhom_nguoi_dung.MA_NHOM = form["MA_NHOM"];
            //nhom_nguoi_dung.TEN_NHOM = form["TEN_NHOM"];
            //nhom_nguoi_dung.GHI_CHU = form["GHI_CHU"];
            if (db.NHOM_NGUOI_DUNG.Count(a => a.MA_NHOM == nhom_nguoi_dung.MA_NHOM) > 0)
            {
                return new HttpStatusCodeResult(404, "Trùng mã nhóm người dùng");
            }

            else if (ModelState.IsValid)
            {
                db.NHOM_NGUOI_DUNG.Add(nhom_nguoi_dung);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // GET: /NhomNguoiDung/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHOM_NGUOI_DUNG nhom_nguoi_dung = await db.NHOM_NGUOI_DUNG.FindAsync(id);
            if (nhom_nguoi_dung == null)
            {
                return HttpNotFound();
            }
            ViewBag.MA_NHOM = new SelectList(db.NHOM_ND_CHUCNANG, "MA_NHOM", "MA_NHOM", nhom_nguoi_dung.MA_NHOM);
            return View(nhom_nguoi_dung);
        }

        // POST: /NhomNguoiDung/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MA_NHOM,TEN_NHOM,GHI_CHU")] NHOM_NGUOI_DUNG nhom_nguoi_dung)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nhom_nguoi_dung).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MA_NHOM = new SelectList(db.NHOM_ND_CHUCNANG, "MA_NHOM", "MA_NHOM", nhom_nguoi_dung.MA_NHOM);
            return View(nhom_nguoi_dung);
        }

        // GET: /NhomNguoiDung/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHOM_NGUOI_DUNG nhom_nguoi_dung = await db.NHOM_NGUOI_DUNG.FindAsync(id);
            if (nhom_nguoi_dung == null)
            {
                return HttpNotFound();
            }
            return View(nhom_nguoi_dung);
        }

        // POST: /NhomNguoiDung/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            NHOM_NGUOI_DUNG nhom_nguoi_dung = await db.NHOM_NGUOI_DUNG.FindAsync(id);
            db.NHOM_NGUOI_DUNG.Remove(nhom_nguoi_dung);
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
