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
    public class PhanQuyenController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /PhanQuyen/
        public async Task<ActionResult> Index()
        {
            //return View();
            return View(await db.PHAN_QUYEN.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string SAVE, FormCollection form)
        {
            if(!String.IsNullOrEmpty(SAVE))
            {
                if (String.IsNullOrEmpty(form["MA_NHOM"]))
                {
                    ViewBag.ErrorMessage = "Xin nhóm người dùng";
                }
                else
                {
                    var temp = form["MA_NHOM"].ToString();
                    List<NHOM_ND_CHUCNANG> pHANQUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.NHOM_NGUOI_DUNG.TEN_NHOM == temp).ToList();
                    List<NHOM_ND_CHUCNANG> nHOM_ND_CHUCNANGs = new List<NHOM_ND_CHUCNANG>();

                    string[] checkedBox = form.GetValues("check");
                    //Tạo nhóm ND - chức năng
                    foreach (var i in pHANQUYEN)
                    {
                        bool flag = false;
                        foreach (var item in checkedBox)
                        {
                            string[] sPLIT = item.Split(new char[] { '.' });
                            NHOM_ND_CHUCNANG create_NHOM_ND_CHUCNANG = new NHOM_ND_CHUCNANG();
                            create_NHOM_ND_CHUCNANG.MA_NHOM = i.NHOM_NGUOI_DUNG.TEN_NHOM;
                            create_NHOM_ND_CHUCNANG.MA_CHUC_NANG = Int32.Parse(sPLIT[0]);
                            create_NHOM_ND_CHUCNANG.MA_QUYEN = Int32.Parse(sPLIT[1]);

                            if (pHANQUYEN.FirstOrDefault(a => a.MA_CHUC_NANG == Int32.Parse(sPLIT[0]) && 
                                                              a.MA_QUYEN == Int32.Parse(sPLIT[1])) == null)
                            {
                                db.NHOM_ND_CHUCNANG.Add(create_NHOM_ND_CHUCNANG);
                                await db.SaveChangesAsync();
                            }
                            else if(i.MA_CHUC_NANG == Int32.Parse(sPLIT[0]) && i.MA_QUYEN == Int32.Parse(sPLIT[1]))
                            {
                                flag = true;
                            }
                        }
                        if(flag == false)
                        {
                            db.NHOM_ND_CHUCNANG.Remove(i);
                            await db.SaveChangesAsync();
                        }
                    }
                    ViewBag.ErrorMessage = "Lưu thành công";

                }
            }
            return View(await db.PHAN_QUYEN.ToListAsync());
        }

        // GET: /PhanQuyen/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHAN_QUYEN phan_quyen = await db.PHAN_QUYEN.FindAsync(id);
            if (phan_quyen == null)
            {
                return HttpNotFound();
            }
            return View(phan_quyen);
        }

        // GET: /PhanQuyen/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /PhanQuyen/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="MA_QUYEN,TEN_QUYEN")] PHAN_QUYEN phan_quyen)
        {
            if (ModelState.IsValid)
            {
                db.PHAN_QUYEN.Add(phan_quyen);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(phan_quyen);
        }

        // GET: /PhanQuyen/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHAN_QUYEN phan_quyen = await db.PHAN_QUYEN.FindAsync(id);
            if (phan_quyen == null)
            {
                return HttpNotFound();
            }
            return View(phan_quyen);
        }

        // POST: /PhanQuyen/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="MA_QUYEN,TEN_QUYEN")] PHAN_QUYEN phan_quyen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phan_quyen).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(phan_quyen);
        }

        // GET: /PhanQuyen/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHAN_QUYEN phan_quyen = await db.PHAN_QUYEN.FindAsync(id);
            if (phan_quyen == null)
            {
                return HttpNotFound();
            }
            return View(phan_quyen);
        }

        // POST: /PhanQuyen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PHAN_QUYEN phan_quyen = await db.PHAN_QUYEN.FindAsync(id);
            db.PHAN_QUYEN.Remove(phan_quyen);
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
