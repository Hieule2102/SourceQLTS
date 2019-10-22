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
    public class DieuChuyenThietBiController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /DieuChuyenThietBi/
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["CHUC_NANG"] != null)
            {
                var pHAN_QUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == Session["NHOM_ND"].ToString()
                                                             && a.MA_CHUC_NANG == 3);

                ViewBag.Them = db.NHOM_ND_CHUCNANG.Where(a => a.MA_QUYEN == 1);
                //ViewBag.Sua = db.NHOM_ND_CHUCNANG.Where(a => a.MA_QUYEN == 3);
            }
            else
            {
                return HttpNotFound("You have no accesss permissions at this");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(FormCollection form, string SAVE, string MATB_DC, string MATB_DC_ct)
        {
            var pHAN_QUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == Session["NHOM_ND"].ToString()
                                                             && a.MA_CHUC_NANG == 3);

            ViewBag.Them = db.NHOM_ND_CHUCNANG.Where(a => a.MA_QUYEN == 1);
            //ViewBag.Sua = db.NHOM_ND_CHUCNANG.Where(a => a.MA_QUYEN == 3);

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
                    //Tạo điều chuyển thiết bị
                    var dIEU_CHUYEN_THIET_BI = THEM_DIEU_CHUYEN_THIET_BI(form);

                    //Thay đổi trạng thái thiết bị
                    var tHIETBI = TRANG_THAI_THIETBI(form, dIEU_CHUYEN_THIET_BI.MATB);

                    if (ModelState.IsValid)
                    {
                        db.DIEU_CHUYEN_THIET_BI.Add(dIEU_CHUYEN_THIET_BI);
                        db.Entry(tHIETBI).State = EntityState.Modified;

                        await db.SaveChangesAsync();
                    }

                    //Thêm vào xác nhận điều chuyển thiết bị
                    var xAC_NHAN = XAC_NHAN_DIEU_CHUYEN_TB(tHIETBI.MATB);
                    db.XAC_NHAN_DIEU_CHUYEN.Add(xAC_NHAN);

                    //Thêm vào nhật ký thiết bị
                    NHAT_KY_THIET_BI nHAT_KY_THIET_BI = NHAT_KY_TB(xAC_NHAN.MA_DIEU_CHUYEN);
                    db.NHAT_KY_THIET_BI.Add(nHAT_KY_THIET_BI);

                    await db.SaveChangesAsync();

                    ViewBag.ErrorMessage = "Thêm thành công";
                }
            }
            else if (!String.IsNullOrEmpty(MATB_DC))
            {
                ViewBag.MATB_DC = MATB_DC ;
            }
            else if (!String.IsNullOrEmpty(MATB_DC_ct))
            {
                ViewBag.MATB_DC = MATB_DC_ct;
            }
            return View();
        }

        #region Tạo điều chuyển thiết bị
        public DIEU_CHUYEN_THIET_BI THEM_DIEU_CHUYEN_THIET_BI(FormCollection form)
        {
            DIEU_CHUYEN_THIET_BI dIEU_CHUYEN_THIET_BI = new DIEU_CHUYEN_THIET_BI();
            dIEU_CHUYEN_THIET_BI.MATB = form["MATB"].ToString();

            var temp = Session["TEN_DANG_NHAP"].ToString();
            var nD_DIEU_CHUYEN = db.NGUOI_DUNG.FirstOrDefault(a => a.TEN_DANG_NHAP == temp);
            dIEU_CHUYEN_THIET_BI.MAND_DIEU_CHUYEN = nD_DIEU_CHUYEN.MA_ND;
            dIEU_CHUYEN_THIET_BI.MADV_DIEU_CHUYEN = nD_DIEU_CHUYEN.MA_DON_VI;

            dIEU_CHUYEN_THIET_BI.MADV_NHAN = Int32.Parse(form["MADV_NHAN"]);
            dIEU_CHUYEN_THIET_BI.MAND_NHAN = form["MAND_NHAN"];
            dIEU_CHUYEN_THIET_BI.SO_LUONG = Int32.Parse(form["SO_LUONG"]);
            dIEU_CHUYEN_THIET_BI.NGAY_DIEU_CHUYEN = DateTime.Now;
            dIEU_CHUYEN_THIET_BI.GHI_CHU = form["GHI_CHU"];
            dIEU_CHUYEN_THIET_BI.VAN_CHUYEN = form["VAN_CHUYEN"];
            return dIEU_CHUYEN_THIET_BI;
        }
        #endregion

        #region Thay đổi trạng thái thiết bị
        public THIETBI TRANG_THAI_THIETBI(FormCollection form, string mATB)
        {
            
            var tHIETBI = db.THIETBIs.Where(a => a.MATB == mATB)
                                     .FirstOrDefault();
            tHIETBI.TINH_TRANG = "Đang điều chuyển";
            return tHIETBI;
        }
        #endregion

        #region Thêm vào xác nhận điều chuyển thiết bị
        public XAC_NHAN_DIEU_CHUYEN XAC_NHAN_DIEU_CHUYEN_TB(string mATB)
        {
            var xAC_NHAN = new XAC_NHAN_DIEU_CHUYEN();
            xAC_NHAN.XAC_NHAN = false;
            xAC_NHAN.MA_DIEU_CHUYEN = db.DIEU_CHUYEN_THIET_BI.FirstOrDefault(a => a.MATB == mATB).MA_DIEU_CHUYEN;
            return xAC_NHAN;
        }
        #endregion

        #region Thêm vào nhật ký thiết bị
        public NHAT_KY_THIET_BI NHAT_KY_TB(int? mA_DIEU_CHUYEN)
        {
            NHAT_KY_THIET_BI nHAT_KY_THIET_BI = new NHAT_KY_THIET_BI();
            nHAT_KY_THIET_BI.MA_DIEU_CHUYEN = mA_DIEU_CHUYEN;
            nHAT_KY_THIET_BI.TINH_TRANG = "Đang điều chuyển";
            return nHAT_KY_THIET_BI;
        }
        #endregion

        //GET: /DieuChuyenThietBi/Details/5
        public async Task<ActionResult> Details(string MATB_DC)
        {
            if (!String.IsNullOrEmpty(MATB_DC))
            {
                var temp = Int32.Parse(MATB_DC);
                THIETBI tHIET_BI = await db.THIETBIs.FindAsync(temp);
                return View(tHIET_BI);
            }
            return View();
        }

        // GET: /DieuChuyenThietBi/Create
        public ActionResult Create()
        {
            ViewBag.MADV_QL = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI");
            ViewBag.MADV_NHAN = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI");
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB");
            return View();
        }

        // POST: /DieuChuyenThietBi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DIEU_CHUYEN_THIET_BI dieu_chuyen_thiet_bi)
        {
            if (ModelState.IsValid)
            {
                db.DIEU_CHUYEN_THIET_BI.Add(dieu_chuyen_thiet_bi);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MADV_QL = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", dieu_chuyen_thiet_bi.MADV_DIEU_CHUYEN);
            ViewBag.MADV_NHAN = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", dieu_chuyen_thiet_bi.MADV_NHAN);
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB", dieu_chuyen_thiet_bi.MATB);
            return View(dieu_chuyen_thiet_bi);
        }

        // GET: /DieuChuyenThietBi/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DIEU_CHUYEN_THIET_BI dieu_chuyen_thiet_bi = await db.DIEU_CHUYEN_THIET_BI.FindAsync(id);
            if (dieu_chuyen_thiet_bi == null)
            {
                return HttpNotFound();
            }
            ViewBag.MADV_QL = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", dieu_chuyen_thiet_bi.MADV_DIEU_CHUYEN);
            ViewBag.MADV_NHAN = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", dieu_chuyen_thiet_bi.MADV_NHAN);
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB", dieu_chuyen_thiet_bi.MATB);
            return View(dieu_chuyen_thiet_bi);
        }

        // POST: /DieuChuyenThietBi/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MA_DIEU_CHUYEN,MATB,MANS_THEO_DOI,MADV_QL,MANS_QL,MADV_NHAN,MANS_NHAN,NGAY_CHUYEN,GHI_CHU")] DIEU_CHUYEN_THIET_BI dieu_chuyen_thiet_bi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dieu_chuyen_thiet_bi).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MADV_QL = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", dieu_chuyen_thiet_bi.MADV_DIEU_CHUYEN);
            ViewBag.MADV_NHAN = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", dieu_chuyen_thiet_bi.MADV_NHAN);
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB", dieu_chuyen_thiet_bi.MATB);
            return View(dieu_chuyen_thiet_bi);
        }

        // GET: /DieuChuyenThietBi/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DIEU_CHUYEN_THIET_BI dieu_chuyen_thiet_bi = await db.DIEU_CHUYEN_THIET_BI.FindAsync(id);
            if (dieu_chuyen_thiet_bi == null)
            {
                return HttpNotFound();
            }
            return View(dieu_chuyen_thiet_bi);
        }

        // POST: /DieuChuyenThietBi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DIEU_CHUYEN_THIET_BI dieu_chuyen_thiet_bi = await db.DIEU_CHUYEN_THIET_BI.FindAsync(id);
            db.DIEU_CHUYEN_THIET_BI.Remove(dieu_chuyen_thiet_bi);
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
