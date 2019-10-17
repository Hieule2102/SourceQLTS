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
        public async Task<ActionResult> Index(FormCollection form, string SAVE, string MATB_XK, string MATB_XK_ct)
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
                    #region Thay đổi trạng thái thiết bị
                    var temp = form["maTB"].ToString();
                    var tHIETBI = (from a in db.THIETBIs
                                   where a.MATB == temp
                                   select a).FirstOrDefault();
                    tHIETBI.TINH_TRANG = "Đang xuất kho";
                    #endregion

                    #region Tạo xuất kho
                    var xuat_kho = new XUAT_KHO();
                    xuat_kho.MATB = tHIETBI.MATB;

                    temp = form["MADV_QL"].ToString();
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

                    xuat_kho.NGAY_XUAT = DateTime.Now;
                    xuat_kho.SO_LUONG = Int32.Parse(form["SO_LUONG"]);
                    xuat_kho.GHI_CHU = form["GHI_CHU"];
                    xuat_kho.VAN_CHUYEN = form["VAN_CHUYEN"];
                    #endregion

                    if (ModelState.IsValid)
                    {
                        db.Entry(tHIETBI).State = EntityState.Modified;
                        db.XUAT_KHO.Add(xuat_kho);
                        await db.SaveChangesAsync();

                        ViewBag.ErrorMessage = "Thêm thành công";
                    }

                    #region Thêm vào xác nhận
                    var xAC_NHAN = new XAC_NHAN_DIEU_CHUYEN();
                    xAC_NHAN.XAC_NHAN = false;
                    xAC_NHAN.MA_XUAT_KHO = (from a in db.XUAT_KHO
                                            where a.MATB == tHIETBI.MATB
                                            select a.MA_XUAT_KHO).FirstOrDefault();

                    db.XAC_NHAN_DIEU_CHUYEN.Add(xAC_NHAN);
                    #endregion

                    #region Thêm vào nhật ký thiết bị
                    NHAT_KY_THIET_BI nHAT_KY_THIET_BI = new NHAT_KY_THIET_BI();
                    nHAT_KY_THIET_BI.MA_XUAT_KHO = xAC_NHAN.MA_XUAT_KHO;
                    nHAT_KY_THIET_BI.TINH_TRANG = "Đang xuất kho";
                    db.NHAT_KY_THIET_BI.Add(nHAT_KY_THIET_BI);
                    #endregion

                    await db.SaveChangesAsync();

                    Email.EmailUsername = "angellove27101997@gmail.com";
                    Email.EmailPassword = "toantran168";
                    Email email = new Email();
                    email.ToEmail = "tnhuy2710@gmail.com";
                    email.Subject = "Thiết bị xuất kho";
                    email.Body = "Thanks for Registering your account.<br>"
                                 + "Thiết bị " + form["MATB"] + ".<br>"
                                 + "Đơn vị quản lý: " + form["MADV_QL"] + ".<br>"
                                 + "Người xuất: " + Session["TEN_DANG_NHAP"] + ".<br>"
                                 + "Số lượng: " + Int32.Parse(form["SO_LUONG"].ToString()) + ".<br>"
                                 + "Ghi chú: " + form["GHI_CHU"].ToString() + ".<br>"
                                 + "Đơn vị nhận: " + form["MADV_NHAN"] + ".<br>"
                                 + "Người nhận: " + form["MAND_NHAN"] + ".<br>"
                                 + "Phương thức vận chuyển: " + form["VAN_CHUYEN"] + ".<br>";
                    email.IsHtml = true;
                    email.Send();
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
