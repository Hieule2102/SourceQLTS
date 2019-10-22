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
    public class PhanQuyenController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /PhanQuyen/
        public async Task<ActionResult> Index()
        {
            //return View();

            if (Session["QL_ND"] != null)
            {
                var pHAN_QUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == Session["NHOM_ND"].ToString()
                                                             && a.MA_CHUC_NANG == 17);

                ViewBag.Them = pHAN_QUYEN.Where(a => a.MA_QUYEN == 1);
                //ViewBag.Sua = pHAN_QUYEN.Where(a => a.MA_QUYEN == 3);

            }
            else
            {
                return HttpNotFound("You have no accesss permissions at this");
            }
            return View(await db.PHAN_QUYEN.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string SAVE, FormCollection form)
        {
            var pHAN_QUYEN = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == Session["NHOM_ND"].ToString()
                                                             && a.MA_CHUC_NANG == 17);

            ViewBag.Them = pHAN_QUYEN.Where(a => a.MA_QUYEN == 1);
            //ViewBag.Sua = pHAN_QUYEN.Where(a => a.MA_QUYEN == 3);

            if (!String.IsNullOrEmpty(SAVE))
            {
                if (String.IsNullOrEmpty(form["MA_NHOM"]))
                {
                    ViewBag.ErrorMessage = "Xin nhóm người dùng";
                }
                else
                {

                    var mA_NHOM = form["MA_NHOM"].ToString();
                    List<NHOM_ND_CHUCNANG> cAC_CHUC_NANG_TREN_DB = db.NHOM_ND_CHUCNANG.Where(a => a.MA_NHOM == mA_NHOM).ToList();

                    string[] checkedBox = form.GetValues("check");
                    //Tạo nhóm ND - chức năng
                    //Nếu nhóm ND chưa có chức năng nào
                    if (cAC_CHUC_NANG_TREN_DB.Count < 1)
                    {
                        foreach (var item in checkedBox)
                        {
                            string[] sPLIT = item.Split(new char[] { '.' });
                            NHOM_ND_CHUCNANG create_NHOM_ND_CHUCNANG = THEM_CHUC_NANG(
                                                                        form["MA_NHOM"],
                                                                        Int32.Parse(sPLIT[0]),
                                                                        Int32.Parse(sPLIT[1]));                           

                            db.NHOM_ND_CHUCNANG.Add(create_NHOM_ND_CHUCNANG);
                            await db.SaveChangesAsync();
                        }
                    }
                    //Nếu nhóm ND đã có nhiều chức năng
                    else
                    {
                        //List các chức năng trên giao diện web
                        List<NHOM_ND_CHUCNANG> cAC_CHUC_NANG_TREN_WEB = new List<NHOM_ND_CHUCNANG>();

                        //Thêm chức năng cho nhóm người dùng
                        foreach (var item in checkedBox)
                        {
                            string[] sPLIT = item.Split(new char[] { '.' });
                            NHOM_ND_CHUCNANG nHOM_ND_CHUCNANG = THEM_CHUC_NANG(
                                                                form["MA_NHOM"],
                                                                Int32.Parse(sPLIT[0]),
                                                                Int32.Parse(sPLIT[1]));

                            //Kiểm tra nhóm người dùng đã có chức năng này hay chưa??
                            if (cAC_CHUC_NANG_TREN_DB.FirstOrDefault(a => a.MA_CHUC_NANG == Int32.Parse(sPLIT[0])
                                                                       && a.MA_QUYEN == Int32.Parse(sPLIT[1])) == null)
                            {
                                db.NHOM_ND_CHUCNANG.Add(nHOM_ND_CHUCNANG);
                                await db.SaveChangesAsync();
                            }

                            //Thêm tất cả các chức năng trên giao diện vào list để kiểm tra và chỉnh sửa
                            cAC_CHUC_NANG_TREN_WEB.Add(nHOM_ND_CHUCNANG);
                        }

                        //Chỉnh sửa chức năng cho nhóm người dùng                                             
                        foreach (var itemA in cAC_CHUC_NANG_TREN_DB)
                        {
                            if (cAC_CHUC_NANG_TREN_WEB.FirstOrDefault(a => a.MA_CHUC_NANG == itemA.MA_CHUC_NANG
                                                                        && a.MA_QUYEN == itemA.MA_QUYEN) == null)
                            {
                                //Xóa
                                db.NHOM_ND_CHUCNANG.Remove(itemA);
                                await db.SaveChangesAsync();
                            }                            
                        }
                    }
                    ViewBag.ErrorMessage = "Lưu thành công";
                }

            }
            return View(await db.PHAN_QUYEN.ToListAsync());
        }

        #region Thêm chức năng
        public NHOM_ND_CHUCNANG THEM_CHUC_NANG(string mA_NHOM, int? mA_CHUC_NANG, int? mA_QUYEN)
        {
            NHOM_ND_CHUCNANG nHOM_ND_CHUCNANG = new NHOM_ND_CHUCNANG();
            nHOM_ND_CHUCNANG.MA_NHOM = mA_NHOM;
            nHOM_ND_CHUCNANG.MA_CHUC_NANG = mA_CHUC_NANG;
            nHOM_ND_CHUCNANG.MA_QUYEN = mA_QUYEN;

            return nHOM_ND_CHUCNANG;
        }
        #endregion        

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
        public async Task<ActionResult> Create([Bind(Include = "MA_QUYEN,TEN_QUYEN")] PHAN_QUYEN phan_quyen)
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
        public async Task<ActionResult> Edit([Bind(Include = "MA_QUYEN,TEN_QUYEN")] PHAN_QUYEN phan_quyen)
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
