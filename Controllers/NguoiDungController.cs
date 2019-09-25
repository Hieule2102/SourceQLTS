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
    public class NguoiDungController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /NguoiDung/
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            //Đơn vị
            var dsTenDonVi = new List<string>();
            var qTenDonVi = (from d in db.DON_VI
                             orderby d.TEN_DON_VI
                             select d.TEN_DON_VI);

            dsTenDonVi.AddRange(qTenDonVi.Distinct());
            ViewBag.donVi = new SelectList(dsTenDonVi);

            //Nhóm người dùng
            var dsTenNhom = new List<string>();
            var qTenNhom = (from d in db.NHOM_NGUOI_DUNG
                            orderby d.TEN_NHOM
                            select d.TEN_NHOM);

            dsTenNhom.AddRange(qTenNhom.Distinct());
            ViewBag.tenNhom = new SelectList(dsTenNhom);

            var nguoi_dung = db.NGUOI_DUNG.Include(n => n.DON_VI);

            
            return View(await nguoi_dung.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(FormCollection form, string SAVE, string EDIT, string donVi, string tenNhom, string value)
        {
            //Đơn vị
            var dsTenDonVi = new List<string>();
            var qTenDonVi = (from d in db.DON_VI
                             orderby d.TEN_DON_VI
                             select d.TEN_DON_VI);
            dsTenDonVi.AddRange(qTenDonVi.Distinct());
            ViewBag.donVi = new SelectList(dsTenDonVi);

            //Nhóm người dùng
            var dsTenNhom = new List<string>();
            var qTenNhom = (from d in db.NHOM_NGUOI_DUNG
                            orderby d.TEN_NHOM
                            select d.TEN_NHOM);
            dsTenNhom.AddRange(qTenNhom.Distinct());
            ViewBag.tenNhom = new SelectList(dsTenNhom);


            var nguoi_dung = db.NGUOI_DUNG.Include(n => n.DON_VI);

            if (!String.IsNullOrEmpty(SAVE))
            {
                var temp = form["MA_ND"].ToString();
                if (String.IsNullOrEmpty(form["MA_NHOM"]) || String.IsNullOrEmpty(form["MA_DON_VI"]) 
                   || String.IsNullOrEmpty(form["MA_ND"]) || String.IsNullOrEmpty(form["TEN_ND"]))
                {
                    ViewBag.ErrorMessage = "Xin nhập đầy đủ thông tin";
                }
                else if (db.NGUOI_DUNG.FirstOrDefault(a => a.MA_ND == temp) != null)
                {
                    ViewBag.ErrorMessage = "Trùng mã người dùng";
                }
                else
                {
                    //Thêm người dùng
                    NGUOI_DUNG create_ND = new NGUOI_DUNG();
                    create_ND.MA_ND = form["MA_ND"];
                    create_ND.TEN_ND = form["TEN_ND"];
                    create_ND.DIEN_THOAI = Int32.Parse(form["DIEN_THOAI"]);
                    create_ND.EMAIL = form["EMAIL"];

                    temp = form["MA_DON_VI"].ToString();
                    create_ND.MA_DON_VI = (from a in db.DON_VI
                                           where a.TEN_DON_VI == temp
                                           select a.MA_DON_VI).FirstOrDefault();

                    //Thêm nhóm người dùng
                    NHOM_ND create_NHOM_ND = new NHOM_ND();
                    create_NHOM_ND.MA_ND = form["MA_ND"];

                    temp = form["MA_NHOM"].ToString();
                    create_NHOM_ND.MA_NHOM = (from a in db.NHOM_NGUOI_DUNG
                                           where a.TEN_NHOM == temp
                                           select a.MA_NHOM).FirstOrDefault();

                    if (ModelState.IsValid)
                    {
                        db.NGUOI_DUNG.Add(create_ND);
                        db.NHOM_ND.Add(create_NHOM_ND);
                        await db.SaveChangesAsync();

                        ViewBag.ErrorMessage = "Thêm thành công";
                    }

                }
            }
            else if (!String.IsNullOrEmpty(EDIT))
            {
                var temp = form["MA_ND"].ToString();
                NGUOI_DUNG edit_ND = db.NGUOI_DUNG.FirstOrDefault(x => x.MA_ND == temp);
                NHOM_ND edit_NHOM_ND = db.NHOM_ND.FirstOrDefault(x => x.MA_ND == temp);

                //Sửa người dùng
                edit_ND.TEN_ND = form["TEN_ND"];
                edit_ND.DIEN_THOAI = Int32.Parse(form["DIEN_THOAI"]);
                edit_ND.EMAIL = form["EMAIL"];

                temp = form["MA_DON_VI"].ToString();
                edit_ND.MA_DON_VI = (from a in db.DON_VI
                                       where a.TEN_DON_VI == temp
                                       select a.MA_DON_VI).FirstOrDefault();

                //Sửa nhóm người dùng
                temp = form["MA_NHOM"].ToString();
                edit_NHOM_ND.MA_NHOM = (from a in db.NHOM_NGUOI_DUNG
                                          where a.TEN_NHOM == temp
                                          select a.MA_NHOM).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    db.Entry(edit_ND).State = EntityState.Modified;
                    db.Entry(edit_NHOM_ND).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    ViewBag.ErrorMessage = "Sửa thành công";
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(donVi))
                {
                    nguoi_dung = nguoi_dung.Where(data => data.DON_VI.TEN_DON_VI == donVi);
                }
                else if (!String.IsNullOrEmpty(tenNhom))
                {
                    var nhom_ND = new List<NGUOI_DUNG>();
                    foreach (var a in db.NHOM_ND)
                    {
                        if (a.NHOM_NGUOI_DUNG.TEN_NHOM == tenNhom)
                        {
                            nhom_ND.Add(a.NGUOI_DUNG);
                        }
                    }
                    return View(nhom_ND.ToList());
                }
            }

            return View(await nguoi_dung.ToListAsync());
        }

        // GET: /NguoiDung/Details/5
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

        // GET: /NguoiDung/Create
        public ActionResult Create()
        {
            ViewBag.MA_DON_VI = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI");
            return View();
        }

        // POST: /NguoiDung/Create
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

        // GET: /NguoiDung/Edit/5
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

        // POST: /NguoiDung/Edit/5
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

        // GET: /NguoiDung/Delete/5
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

        // POST: /NguoiDung/Delete/5
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
