using Source.Models;
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
    public class NguoiDungController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /NguoiDung/
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            if (Session["QL_ND"] != null)
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

                var pHAN_QUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == Session["NHOM_ND"].ToString()
                                                             && a.MA_CHUC_NANG == 15);                

                ViewBag.Them = pHAN_QUYEN.Where(a => a.MA_QUYEN == 1);
                ViewBag.Sua = pHAN_QUYEN.Where(a => a.MA_QUYEN == 3);
            }
            else
            {
                return HttpNotFound("You have no accesss permissions at this");
            }

            var nguoi_dung = db.NGUOI_DUNG.Include(n => n.DON_VI);
            return View(await nguoi_dung.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(FormCollection form, string SAVE, string EDIT, string donVi, string tenNhom, string value)
        {
            var pHAN_QUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == Session["NHOM_ND"].ToString()
                                                             && a.MA_CHUC_NANG == 15);

            ViewBag.Them = pHAN_QUYEN.Where(a => a.MA_QUYEN == 1);
            ViewBag.Sua = pHAN_QUYEN.Where(a => a.MA_QUYEN == 3);

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
                    NGUOI_DUNG create_ND = THEM_NGUOI_DUNG(form);

                    //Thêm nhóm người dùng
                    NHOM_ND create_NHOM_ND = THEM_NHOM_NGUOI_DUNG(form);

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
                //Sửa người dùng
                if(SUA_NGUOI_DUNG(form) != null)
                {
                    db.Entry(SUA_NGUOI_DUNG(form)).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

                //Sửa nhóm người dùng
                if (SUA_NHOM_NGUOI_DUNG(form) != null)
                {
                    db.Entry(SUA_NHOM_NGUOI_DUNG(form)).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                ViewBag.ErrorMessage = "Sửa thành công";
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
        #region Thêm người dùng
        public NGUOI_DUNG THEM_NGUOI_DUNG(FormCollection form)
        {
            NGUOI_DUNG nGUOI_DUNG = new NGUOI_DUNG();
            nGUOI_DUNG.MA_ND = form["MA_ND"];
            nGUOI_DUNG.TEN_ND = form["TEN_ND"];
            nGUOI_DUNG.DIEN_THOAI = Int32.Parse(form["DIEN_THOAI"]);
            nGUOI_DUNG.EMAIL = form["EMAIL"];
            nGUOI_DUNG.MA_DON_VI = Int32.Parse(form["MA_DON_VI"]);
            return nGUOI_DUNG;
        }
        #endregion

        #region Thêm nhóm người dùng
        public NHOM_ND THEM_NHOM_NGUOI_DUNG(FormCollection form)
        {
            NHOM_ND nHOM_ND = new NHOM_ND();
            nHOM_ND.MA_ND = form["MA_ND"];            
            nHOM_ND.MA_NHOM = form["MA_NHOM"];
            return nHOM_ND;
        }
        #endregion

        #region Sửa người dùng
        public NGUOI_DUNG SUA_NGUOI_DUNG(FormCollection form)
        {            
            //Sửa người dùng
            NGUOI_DUNG eDIT_NGUOI_DUNG = new NGUOI_DUNG();
            eDIT_NGUOI_DUNG.MA_ND = form["MA_ND"];
            eDIT_NGUOI_DUNG.TEN_ND = form["TEN_ND"];
            eDIT_NGUOI_DUNG.MA_DON_VI = Int32.Parse(form["MA_DON_VI"]);
            eDIT_NGUOI_DUNG.DIEN_THOAI = Int32.Parse(form["DIEN_THOAI"]);
            eDIT_NGUOI_DUNG.EMAIL = form["EMAIL"];

            //NGUOI_DUNG nGUOI_DUNG = db.NGUOI_DUNG.FirstOrDefault(x => x.MA_ND == eDIT_NGUOI_DUNG.MA_ND);
            //if (nGUOI_DUNG.Equals(eDIT_NGUOI_DUNG))
            //{
            //    return null;
            //}
            return eDIT_NGUOI_DUNG;
        }
        #endregion

        #region Sửa nhóm người dùng
        public NHOM_ND SUA_NHOM_NGUOI_DUNG(FormCollection form)
        {
            //Sửa nhóm người dùng
            NHOM_ND eDIT_NHOM_ND = new NHOM_ND();
            eDIT_NHOM_ND.MA_NHOM_ND = Int32.Parse(form["MA_NHOM_ND"]);
            eDIT_NHOM_ND.MA_ND = form["MA_ND"];
            eDIT_NHOM_ND.MA_NHOM = form["MA_NHOM"];

            NHOM_ND nHOM_ND = db.NHOM_ND.FirstOrDefault(x => x.MA_NHOM_ND == eDIT_NHOM_ND.MA_NHOM_ND);
            if (nHOM_ND.MA_NHOM == eDIT_NHOM_ND.MA_NHOM)
            {
                return null;
            }
            return eDIT_NHOM_ND;
        }
        #endregion        

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

        //public bool DeepEquals(NGUOI_DUNG obj, NGUOI_DUNG another)
        //{
        //    //if (ReferenceEquals(obj, another))
        //    //{
        //    //    return true;
        //    //}
        //    if ((obj == null) || (another == null)) return false;
        //    //So sánh class của 2 object, nếu khác nhau thì trả fail
        //    //if (obj.GetType() != another.GetType())
        //    //{
        //    //    return false;
        //    //}

        //    //Lấy toàn bộ các properties của obj
        //    //sau đó so sánh giá trị của từng property
        //    //foreach (var property in obj.GetType().GetProperties())
        //    //{
        //    //    var objValue = property.GetValue(obj);
        //    //    var anotherValue = property.GetValue(another);
        //    //    if (!objValue.Equals(anotherValue))
        //    //    {
        //    //        return false;
        //    //    }
        //    //}
        //    foreach(var item in obj.GetType())
        //    {
        //        if()
        //        {

        //        }
        //    }

        //    return true;
        //}

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
