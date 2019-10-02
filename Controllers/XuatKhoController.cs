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
    public class XuatKhoController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /XuatKho/
        public ActionResult Index()
        {
            //var xuat_kho = db.XUAT_KHO.Include(x => x.DON_VI).Include(x => x.DON_VI1).Include(x => x.NGUOI_DUNG).Include(x => x.NGUOI_DUNG1).Include(x => x.THIETBI);
            //return View(await xuat_kho.ToListAsync());

            if (Session["CHUC_NANG"] != null)
            {
                var pHAN_QUYEN = Session["NHOM_ND"].ToString();
                ViewBag.Them = db.NHOM_ND_CHUCNANG.Where(a => a.MA_CHUC_NANG == 2 &&
                                                         a.MA_QUYEN == 1 &&
                                                         a.MA_NHOM == pHAN_QUYEN).FirstOrDefault();  
            }
            else
            {
                return HttpNotFound("You have no accesss permissions at this");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(FormCollection form, string SAVE, string MATB_XK)
        {
            var pHAN_QUYEN = Session["NHOM_ND"].ToString();
            ViewBag.Them = db.NHOM_ND_CHUCNANG.Where(a => a.MA_CHUC_NANG == 2 &&
                                                     a.MA_QUYEN == 1 &&
                                                     a.MA_NHOM == pHAN_QUYEN).FirstOrDefault();

            
            if (!String.IsNullOrEmpty(SAVE))
            {
                if (String.IsNullOrEmpty(form["MATB"]))
                {
                    ViewBag.ErrorMessage = "Xin chọn thiết bị";
                }
                else if (String.IsNullOrEmpty(form["MADV_NHAN"]))
                {
                    ViewBag.ErrorMessage = "Xin chọn đơn vị tiếp nhận";
                }
                else
                {
                    //Tạo xuất kho
                    var xuat_kho = new XUAT_KHO();
                    xuat_kho.MATB = Int32.Parse(form["maTB"]);

                    var temp = form["MADV_QL"].ToString();
                    xuat_kho.MADV_XUAT = (from p in db.DON_VI
                                          where p.TEN_DON_VI == temp
                                          select p.MA_DON_VI).FirstOrDefault();

                    temp = Session["TEN_DANG_NHAP"].ToString();
                    xuat_kho.MAND_XUAT = (from p in db.NGUOI_DUNG
                                          where p.TEN_DANG_NHAP == temp
                                          select p.MA_ND).FirstOrDefault();

                    temp = form["MADV_NHAN"].ToString();
                    xuat_kho.MADV_NHAN = (from p in db.DON_VI
                                          where p.TEN_DON_VI == temp
                                          select p.MA_DON_VI).FirstOrDefault();

                    temp = form["MAND_NHAN"].ToString();
                    xuat_kho.MAND_NHAN = (from p in db.NGUOI_DUNG
                                          where p.TEN_ND == temp
                                          select p.MA_ND).FirstOrDefault();

                    xuat_kho.GHI_CHU = form["GHI_CHU"];
                    xuat_kho.NGAY_XUAT = DateTime.Now;

                    //Thay đổi trạng thái thiết bị
                    var mATB = Int32.Parse(form["maTB"]);
                    var tHIETBI = (from a in db.THIETBIs
                                   where a.MATB == mATB
                                   select a).FirstOrDefault();
                    tHIETBI.TINH_TRANG = "Đang điều chuyển";

                    //Thêm vào nhật ký thiết bị
                    NHAT_KY_THIET_BI nHAT_KY_THIET_BI = new NHAT_KY_THIET_BI();
                    nHAT_KY_THIET_BI.MATB = Int32.Parse(form["MATB"]);
                    nHAT_KY_THIET_BI.TINH_TRANG = "Đang điều chuyển";
                    nHAT_KY_THIET_BI.NGAY_THUC_HIEN = DateTime.Now;

                    if (ModelState.IsValid)
                    {
                        db.Entry(tHIETBI).State = EntityState.Modified;
                        db.XUAT_KHO.Add(xuat_kho);
                        db.NHAT_KY_THIET_BI.Add(nHAT_KY_THIET_BI);
                        await db.SaveChangesAsync();

                        ViewBag.ErrorMessage = "Thêm thành công";
                    }

                    //Thêm vào xác nhận
                    var xAC_NHAN = new XAC_NHAN_DIEU_CHUYEN();
                    xAC_NHAN.MATB = mATB;
                    xAC_NHAN.XAC_NHAN = false;
                    xAC_NHAN.MA_XUAT_KHO = (from a in db.XUAT_KHO
                                            where a.MATB == mATB
                                            select a.MA_XUAT_KHO).FirstOrDefault();

                    db.XAC_NHAN_DIEU_CHUYEN.Add(xAC_NHAN);
                }
            }
            else if(!String.IsNullOrEmpty(MATB_XK))
            {
                ViewBag.MATB_XK = MATB_XK;
            }
            return View();
        }

        // GET: /XuatKho/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XUAT_KHO xuat_kho = await db.XUAT_KHO.FindAsync(id);
            if (xuat_kho == null)
            {
                return HttpNotFound();
            }
            return View(xuat_kho);
        }

        // GET: /XuatKho/Create
        public ActionResult Create()
        {
            ViewBag.MADV_XUAT = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI");
            ViewBag.MADV_NHAN = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI");
            ViewBag.MAND_XUAT = new SelectList(db.NGUOI_DUNG, "MA_ND", "TEN_ND");
            ViewBag.MAND_NHAN = new SelectList(db.NGUOI_DUNG, "MA_ND", "TEN_ND");
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB");
            return View();
        }

        // POST: /XuatKho/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(XUAT_KHO xuat_kho)
        {

            if (ModelState.IsValid)
            {
                db.XUAT_KHO.Add(xuat_kho);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MADV_XUAT = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", xuat_kho.MADV_XUAT);
            ViewBag.MADV_NHAN = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", xuat_kho.MADV_NHAN);
            ViewBag.MAND_XUAT = new SelectList(db.NGUOI_DUNG, "MA_ND", "TEN_ND", xuat_kho.MAND_XUAT);
            ViewBag.MAND_NHAN = new SelectList(db.NGUOI_DUNG, "MA_ND", "TEN_ND", xuat_kho.MAND_NHAN);
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB", xuat_kho.MATB);
            return View(xuat_kho);
        }

        // GET: /XuatKho/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XUAT_KHO xuat_kho = await db.XUAT_KHO.FindAsync(id);
            if (xuat_kho == null)
            {
                return HttpNotFound();
            }
            ViewBag.MADV_XUAT = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", xuat_kho.MADV_XUAT);
            ViewBag.MADV_NHAN = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", xuat_kho.MADV_NHAN);
            ViewBag.MAND_XUAT = new SelectList(db.NGUOI_DUNG, "MA_ND", "TEN_ND", xuat_kho.MAND_XUAT);
            ViewBag.MAND_NHAN = new SelectList(db.NGUOI_DUNG, "MA_ND", "TEN_ND", xuat_kho.MAND_NHAN);
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB", xuat_kho.MATB);
            return View(xuat_kho);
        }

        // POST: /XuatKho/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MA_XUAT_KHO,MATB,MADV_XUAT,MADV_NHAN,MAND_XUAT,MAND_NHAN,NGAY_THUC_HIEN")] XUAT_KHO xuat_kho)
        {
            if (ModelState.IsValid)
            {
                db.Entry(xuat_kho).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MADV_XUAT = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", xuat_kho.MADV_XUAT);
            ViewBag.MADV_NHAN = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", xuat_kho.MADV_NHAN);
            ViewBag.MAND_XUAT = new SelectList(db.NGUOI_DUNG, "MA_ND", "TEN_ND", xuat_kho.MAND_XUAT);
            ViewBag.MAND_NHAN = new SelectList(db.NGUOI_DUNG, "MA_ND", "TEN_ND", xuat_kho.MAND_NHAN);
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB", xuat_kho.MATB);
            return View(xuat_kho);
        }

        // GET: /XuatKho/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XUAT_KHO xuat_kho = await db.XUAT_KHO.FindAsync(id);
            if (xuat_kho == null)
            {
                return HttpNotFound();
            }
            return View(xuat_kho);
        }

        // POST: /XuatKho/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            XUAT_KHO xuat_kho = await db.XUAT_KHO.FindAsync(id);
            db.XUAT_KHO.Remove(xuat_kho);
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
