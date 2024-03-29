﻿using Source.Models;
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
    public class DanhSachThietBiController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /DanhSachThietBi/
        public async Task<ActionResult> Index()
        {

            if (Session["BAO_CAO"] != null)
            {
                //var pHAN_QUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == Session["NHOM_ND"].ToString()
                //                                             && a.MA_CHUC_NANG == 9);

                //ViewBag.Them = db.NHOM_ND_CHUCNANG.Where(a => a.MA_QUYEN == 1);
                //ViewBag.Sua = db.NHOM_ND_CHUCNANG.Where(a => a.MA_QUYEN == 3);

                //Nhóm thiết bị
                var dsLOAITB = db.LOAI_THIETBI.Select(a => a.TEN_LOAI)
                                              .ToList()
                                              .Distinct();
                ViewBag.MA_LOAITB = new SelectList(dsLOAITB);

                //Nhóm thiết bị
                var dsNhomTB = db.NHOM_THIETBI.Select(a => a.TEN_NHOM)
                                              .ToList()
                                              .Distinct(); ;
                ViewBag.MA_NHOMTB = new SelectList(dsNhomTB);

                //Đơn vị
                var dsTenDonVi = db.DON_VI.Select(a => a.TEN_DON_VI)
                                           .ToList()
                                           .Distinct();
                ViewBag.MA_DON_VI = new SelectList(dsTenDonVi);
            }
            else
            {
                return HttpNotFound("You have no accesss permissions at this");
            }
            var thietbis = db.THIETBIs.OrderBy(t => t.MATB).Include(t => t.DON_VI).Include(t => t.LOAI_THIETBI).Include(t => t.NHA_CUNG_CAP);
            return View(await thietbis.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string SEARCH_STRING, string MA_LOAITB, string MA_DON_VI, string MA_NHOMTB)
        {
            //Nhóm thiết bị
            var dsLOAITB = db.LOAI_THIETBI.Select(a => a.TEN_LOAI)
                                          .ToList()
                                          .Distinct();
            ViewBag.MA_LOAITB = new SelectList(dsLOAITB);

            //Nhóm thiết bị
            var dsNhomTB = db.NHOM_THIETBI.Select(a => a.TEN_NHOM)
                                          .ToList()
                                          .Distinct(); ;
            ViewBag.MA_NHOMTB = new SelectList(dsNhomTB);

            //Đơn vị
            var dsTenDonVi = db.DON_VI.Select(a => a.TEN_DON_VI)
                                       .ToList()
                                       .Distinct();
            ViewBag.MA_DON_VI = new SelectList(dsTenDonVi);

            var thietbis = db.THIETBIs.Include(t => t.DON_VI).Include(t => t.LOAI_THIETBI).Include(t => t.NHA_CUNG_CAP);

            //Tìm kiếm loại thiết bị
            if (!String.IsNullOrEmpty(MA_NHOMTB))
            {
                //Nhóm thiết bị
                dsLOAITB = db.LOAI_THIETBI.Where(a => a.NHOM_THIETBI.TEN_NHOM.Contains(MA_NHOMTB))
                                          .Select(a => a.TEN_LOAI)
                                          .ToList()
                                          .Distinct();
                ViewBag.MA_LOAITB = new SelectList(dsLOAITB);
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
            else if (!String.IsNullOrEmpty(SEARCH_STRING))
            {
                thietbis = thietbis.Where(data => data.TENTB.Contains(SEARCH_STRING));
            }
            //Tìm đơn vị
            else if (!String.IsNullOrEmpty(MA_DON_VI))
            {
                thietbis = thietbis.Where(data => data.DON_VI.TEN_DON_VI == MA_DON_VI);
            }            

            return View(await thietbis.OrderBy(a => a.MATB).ToListAsync());
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
