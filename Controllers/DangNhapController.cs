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
            if (Session["iS_ALREADY"] != null)
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
                    Session["iS_ALREADY"] = 1;
                    Session["TEN_DANG_NHAP"] = (from b in db.NGUOI_DUNG
                                                where b.TEN_DANG_NHAP == tEN_DANG_NHAP
                                                select b.TEN_DANG_NHAP).FirstOrDefault();

                    var ma_ND = (from b in db.NGUOI_DUNG
                                 where b.TEN_DANG_NHAP == tEN_DANG_NHAP
                                 select b.MA_ND).FirstOrDefault();

                    Session["NHOM_ND"] = (from b in db.NHOM_ND
                                          where b.MA_ND == ma_ND
                                          select b.MA_NHOM).FirstOrDefault();
                    var nhom_ND = Session["NHOM_ND"].ToString();

                    Session["CHUC_NANG"] = (from b in db.NHOM_ND_CHUCNANG
                                            where b.MA_NHOM == nhom_ND && b.DM_CHUC_NANG.CHUC_NANG.MA_CHUC_NANG == 1
                                            select b.MA_NHOM).FirstOrDefault();

                    Session["DANH_MUC"] = (from b in db.NHOM_ND_CHUCNANG
                                           where b.MA_NHOM == nhom_ND && b.DM_CHUC_NANG.CHUC_NANG.MA_CHUC_NANG == 2
                                           select b.MA_NHOM).FirstOrDefault();

                    Session["BAO_CAO"] = (from b in db.NHOM_ND_CHUCNANG
                                          where b.MA_NHOM == nhom_ND && b.DM_CHUC_NANG.CHUC_NANG.MA_CHUC_NANG == 3
                                          select b.MA_NHOM).FirstOrDefault();

                    Session["QL_ND"] = (from b in db.NHOM_ND_CHUCNANG
                                        where b.MA_NHOM == nhom_ND && b.DM_CHUC_NANG.CHUC_NANG.MA_CHUC_NANG == 4
                                        select b.MA_NHOM).FirstOrDefault();

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
