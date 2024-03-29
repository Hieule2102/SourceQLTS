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
    public class XuatKhoController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /XuatKho/
        public ActionResult Index()
        {
            if (Session["CHUC_NANG"] != null)
            {
                var pHAN_QUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == Session["NHOM_ND"].ToString()
                                                             && a.MA_CHUC_NANG == 2);

                ViewBag.Them = pHAN_QUYEN.Where(a => a.MA_QUYEN == 1);
                //ViewBag.Sua = pHAN_QUYEN.Where(a => a.MA_QUYEN == 3);
            }
            else
            {
                return HttpNotFound("You have no accesss permissions at this");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(FormCollection form, string SAVE, string MATB_XK, string MATB_XK_ct)
        {
            var pHAN_QUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == Session["NHOM_ND"].ToString()
                                                             && a.MA_CHUC_NANG == 2);

            ViewBag.Them = pHAN_QUYEN.Where(a => a.MA_QUYEN == 1);
            //ViewBag.Sua = pHAN_QUYEN.Where(a => a.MA_QUYEN == 3);

            if (!String.IsNullOrEmpty(SAVE))
            {
                if (String.IsNullOrEmpty(form["MATB"]))
                {
                    ViewBag.ErrorMessage = "Xin chọn thiết bị";
                }
                else if (String.IsNullOrEmpty(form["MADV_NHAN"]) || String.IsNullOrEmpty(form["MAND_NHAN"]))
                {
                    ViewBag.ErrorMessage = "Xin chọn đơn vị tiếp nhận";
                }
                else
                {
                    var mATB = form["MATB"].Split(new char[] { ',' });
                    foreach (var i in mATB)
                    {
                        //Thay đổi trạng thái thiết bị
                        var tHIETBI = THAY_DOI_TRANG_THAI_TB(i);

                        //Thêm xuất kho
                        var xUAT_KHO = THEM_XUAT_KHO(form, i);

                        if (ModelState.IsValid)
                        {
                            db.Entry(tHIETBI).State = EntityState.Modified;
                            db.XUAT_KHO.Add(xUAT_KHO);
                            await db.SaveChangesAsync();
                        }

                        //Thêm vào xác nhận
                        var xAC_NHAN = XAC_NHAN_XUAT_KHO_TB(i);
                        db.XAC_NHAN_DIEU_CHUYEN.Add(xAC_NHAN);

                        //Thêm vào nhật ký thiết bị
                        var nHAT_KY_THIET_BI = THEM_NHAT_KY_THIET_BI(xAC_NHAN.MA_XUAT_KHO);
                        db.NHAT_KY_THIET_BI.Add(nHAT_KY_THIET_BI);

                        await db.SaveChangesAsync();

                        ViewBag.ErrorMessage = "Thêm thành công";
                    }

                    //Email.EmailUsername = "huytnh@kienlongbank.com";
                    //Email.EmailPassword = "Klb1234567";
                    //Email email = new Email();
                    //email.ToEmail = "hieulmi@kienlongbank.com";
                    //email.Subject = "Thiết bị xuất kho";
                    //email.Body = "Thanks for Registering your account.<br>"
                    //             + "Thiết bị " + form["MATB"] + ".<br>"
                    //             + "Đơn vị quản lý: " + form["MADV_QL"] + ".<br>"
                    //             + "Người xuất: " + Session["TEN_DANG_NHAP"] + ".<br>"
                    //             + "Số lượng: " + Int32.Parse(form["SO_LUONG"].ToString()) + ".<br>"
                    //             + "Ghi chú: " + form["GHI_CHU"].ToString() + ".<br>"
                    //             + "Đơn vị nhận: " + form["MADV_NHAN"] + ".<br>"
                    //             + "Người nhận: " + form["MAND_NHAN"] + ".<br>"
                    //             + "Phương thức vận chuyển: " + form["VAN_CHUYEN"] + ".<br>";
                    //email.IsHtml = true;
                    //email.Send();
                }                
            }
            else if (!String.IsNullOrEmpty(MATB_XK))
            {
                ViewBag.MATB_XK = MATB_XK;
            }
            else if (!String.IsNullOrEmpty(MATB_XK_ct))
            {
                ViewBag.MATB_XK = MATB_XK_ct;
            }
            return View();
        }

        #region Thay đổi trạng thái thiết bị
        public THIETBI THAY_DOI_TRANG_THAI_TB(string mATB)
        {
            var tHIETBI = db.THIETBIs.Where(a => a.MATB == mATB).FirstOrDefault();
            tHIETBI.TINH_TRANG = "Đang xuất kho";

            return tHIETBI;
        }
        #endregion

        #region Thêm xuất kho
        public XUAT_KHO THEM_XUAT_KHO(FormCollection form, string mATB)
        {
            var xUAT_KHO = new XUAT_KHO();
            xUAT_KHO.MATB = mATB;

            var tEN_DANG_NHAP = Session["TEN_DANG_NHAP"].ToString();
            var nGUOI_DUNG = db.NGUOI_DUNG.FirstOrDefault(a => a.TEN_DANG_NHAP == tEN_DANG_NHAP);
            xUAT_KHO.MADV_XUAT = nGUOI_DUNG.MA_DON_VI;
            xUAT_KHO.MAND_XUAT = nGUOI_DUNG.MA_ND;

            xUAT_KHO.MADV_NHAN = Int32.Parse(form["MADV_NHAN"]);
            xUAT_KHO.MAND_NHAN = form["MAND_NHAN"];
            xUAT_KHO.NGAY_XUAT = DateTime.Now;
            xUAT_KHO.SO_LUONG = Int32.Parse(form["SO_LUONG"]);
            xUAT_KHO.GHI_CHU = form["GHI_CHU"];
            xUAT_KHO.VAN_CHUYEN = form["VAN_CHUYEN"];

            return xUAT_KHO;
        }
        #endregion

        #region Thêm vào xác nhận xuất kho thiết bị
        public XAC_NHAN_DIEU_CHUYEN XAC_NHAN_XUAT_KHO_TB(string mATB)
        {
            var xAC_NHAN = new XAC_NHAN_DIEU_CHUYEN();
            xAC_NHAN.XAC_NHAN = false;
            xAC_NHAN.MA_XUAT_KHO = db.XUAT_KHO.FirstOrDefault(a => a.MATB == mATB).MA_XUAT_KHO;
            return xAC_NHAN;
        }
        #endregion

        #region Thêm vào nhật ký thiết bị
        public NHAT_KY_THIET_BI THEM_NHAT_KY_THIET_BI(int? mA_XUAT_KHO)
        {
            NHAT_KY_THIET_BI nHAT_KY_THIET_BI = new NHAT_KY_THIET_BI();
            nHAT_KY_THIET_BI.MA_XUAT_KHO = mA_XUAT_KHO;
            nHAT_KY_THIET_BI.TINH_TRANG = "Đang xuất kho";

            return nHAT_KY_THIET_BI;
        }
        #endregion

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
