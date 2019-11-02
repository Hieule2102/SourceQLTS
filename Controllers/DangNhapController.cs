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
    public class DangNhapController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /DangNhap/
        public ActionResult Index()
        {
            if (Session["TEN_DANG_NHAP"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection form)
        {
            if (!String.IsNullOrEmpty(form["TEN_DANG_NHAP"].ToString()))
            {
                var tEN_DANG_NHAP = form["TEN_DANG_NHAP"];
                var mAT_KHAU = form["MAT_KHAU"];

                if (db.NGUOI_DUNG.FirstOrDefault(x => x.TEN_DANG_NHAP == tEN_DANG_NHAP) == null || db.NGUOI_DUNG.FirstOrDefault(x => x.MAT_KHAU == mAT_KHAU) == null)
                {
                    ViewBag.ErrorMessage = "Thông tin đăng nhập không hợp lệ";
                }
                else
                {                    
                    Session["TEN_DANG_NHAP"] = tEN_DANG_NHAP;                    
                    
                    Session["NHOM_ND"] = db.NHOM_ND
                                        .FirstOrDefault(b => b.MA_ND == db.NGUOI_DUNG
                                                                        .FirstOrDefault(a => a.TEN_DANG_NHAP == tEN_DANG_NHAP)
                                                                        .MA_ND)
                                        .MA_NHOM;

                    var temp = Session["NHOM_ND"].ToString();
                    var nHOM_ND = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == temp);

                    Session["CHUC_NANG"] = nHOM_ND.FirstOrDefault(a => a.DM_CHUC_NANG.CHUC_NANG.MA_CHUC_NANG == 1);

                    Session["DANH_MUC"] = nHOM_ND.FirstOrDefault(a => a.DM_CHUC_NANG.CHUC_NANG.MA_CHUC_NANG == 2);

                    Session["BAO_CAO"] = nHOM_ND.FirstOrDefault(a => a.DM_CHUC_NANG.CHUC_NANG.MA_CHUC_NANG == 3);

                    Session["QL_ND"] = nHOM_ND.FirstOrDefault(a => a.DM_CHUC_NANG.CHUC_NANG.MA_CHUC_NANG == 4);

                    var xAC_NHAN = db.XAC_NHAN_DIEU_CHUYEN.Where(a => a.XAC_NHAN == false);                    
                    if (tEN_DANG_NHAP != "admin")
                    {
                        xAC_NHAN = xAC_NHAN.Where(a => (a.XUAT_KHO.NGUOI_DUNG1.TEN_DANG_NHAP == tEN_DANG_NHAP
                                                     || a.DIEU_CHUYEN_THIET_BI.NGUOI_DUNG.TEN_DANG_NHAP == tEN_DANG_NHAP));
                    }
                    Session["COUNT"] = xAC_NHAN.Count();

                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.RemoveAll();
            Session["iS_ALREADY"] = null;
            //return View("~/Views/Home/Index.cshtml");
            return RedirectToAction("Index");
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
