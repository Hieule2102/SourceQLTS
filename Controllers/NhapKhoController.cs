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
    public class NhapKhoController : Controller
    {
        private QuanLyTaiSanCNTTEntities db = new QuanLyTaiSanCNTTEntities();

        // GET: /NhapKho/
        public ActionResult Index()
        {
            //var nhap_kho = db.NHAP_KHO.Include(n => n.DON_VI).Include(n => n.NGUOI_DUNG).Include(n => n.THIETBI);
            //return View(await nhap_kho.ToListAsync());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(FormCollection form, string SAVE, HttpPostedFileBase HINH_ANH)
        {
            var nhap_kho = db.NHAP_KHO.Include(n => n.DON_VI).Include(n => n.NGUOI_DUNG).Include(n => n.THIETBI);
            var temp = form["SO_SERIAL"].ToString();
            if (form["TENTB"].ToString() == "" || form["SO_SERIAL"].ToString() == "" || form["MA_LOAITB"].ToString() == "")
            {
                ViewBag.ErrorMessage = "Xin nhập đầy đủ thông tin";
            }
            else if (db.THIETBIs.FirstOrDefault(x => x.SO_SERIAL == temp) != null)
            {
                ViewBag.ErrorMessage = "Số Serial đã tồn tại";
            }

            //Lưu
            else if (!String.IsNullOrEmpty(SAVE))
            {
                //Tạo thiết bị
                var thiet_Bi = new THIETBI();
                thiet_Bi.TENTB = form["TENTB"];
                thiet_Bi.SO_SERIAL = form["SO_SERIAL"];
                if (!String.IsNullOrEmpty(form["GIA_TIEN"]))
                {
                    thiet_Bi.GIA_TIEN = Decimal.Parse(form["GIA_TIEN"].ToString());
                }
                else
                {
                    thiet_Bi.GIA_TIEN = 0;
                }
                thiet_Bi.THOI_HAN_BAO_HANH = form["THOI_HAN_BAO_HANH"];

                temp = form["MA_LOAITB"].ToString();
                thiet_Bi.MA_LOAITB = (from p in db.LOAI_THIETBI
                                      where p.TEN_LOAI == temp
                                      select p.MA_LOAITB).FirstOrDefault();

                if (!String.IsNullOrEmpty(form["MA_DON_VI"]))
                {
                    temp = form["MA_DON_VI"].ToString();
                    thiet_Bi.MA_DV = (from p in db.DON_VI
                                      where p.TEN_DON_VI == temp
                                      select p.MA_DON_VI).FirstOrDefault();
                }

                if (!String.IsNullOrEmpty(form["MA_NCC"]))
                {
                    temp = form["MA_NCC"].ToString();
                    thiet_Bi.MA_NCC = (from p in db.NHA_CUNG_CAP
                                       where p.TEN_NCC == temp
                                       select p.MA_NCC).FirstOrDefault();
                }

                thiet_Bi.NGAY_MUA = DateTime.Parse(form["NGAY_MUA"]);

                temp = form["MA_NHOMTB"].ToString();
                var nhomTB = (from d in db.NHOM_THIETBI
                              where d.TEN_NHOM == temp
                              select d.MA_NHOMTB).FirstOrDefault();

                //Tạo cấu hình
                var cau_Hinh = new CAU_HINH();
                if(nhomTB == "PC")
                {
                    if (!String.IsNullOrEmpty(form["thong_so1"]))
                    {
                        temp = form["thong_so1"].ToString();
                        cau_Hinh.CPU = (from p in db.DM_CPU
                                        where p.TEN_CPU == temp
                                        select p.MA_CPU).First();
                    }

                    if (!String.IsNullOrEmpty(form["thong_so2"]))
                    {
                        temp = form["thong_so2"].ToString();
                        cau_Hinh.RAM = (from p in db.DM_RAM
                                        where p.TEN_RAM == temp
                                        select p.MA_RAM).First();
                    }

                    if (!String.IsNullOrEmpty(form["thong_so3"]))
                    {
                        temp = form["thong_so3"].ToString();
                        cau_Hinh.MAN_HINH = (from p in db.DM_MAN_HINH
                                             where p.TEN_MAN_HINH == temp
                                             select p.MA_MAN_HINH).First();
                    }

                    if (!String.IsNullOrEmpty(form["thong_so4"]))
                    {
                        temp = form["thong_so4"].ToString();
                        cau_Hinh.O_CUNG = (from p in db.DM_O_CUNG
                                           where p.TEN_O_CUNG == temp
                                           select p.MA_O_CUNG).First();
                    }

                    if (!String.IsNullOrEmpty(form["thong_so5"]))
                    {
                        temp = form["thong_so5"].ToString();
                        cau_Hinh.VGA = (from p in db.DM_VGA
                                        where p.TEN_VGA == temp
                                        select p.MA_VGA).First();
                    }

                    if (!String.IsNullOrEmpty(form["thong_so6"]))
                    {
                        temp = form["thong_so6"].ToString();
                        cau_Hinh.HE_DIEU_HANH = (from p in db.DM_HDH
                                                 where p.TEN_HDH == temp
                                                 select p.MA_HDH).First();
                    }
                }
                else if(nhomTB == "PR")
                {
                    if (!String.IsNullOrEmpty(form["thong_so1"]))
                    {
                        //temp = form["CPU"].ToString();
                        //cau_Hinh.CPU = (from p in db.DM_CPU
                        //                where p.TEN_CPU == temp
                        //                select p.MA_CPU).First();

                        cau_Hinh.KICH_THUOC = form["thong_so1"];
                    }

                    if (!String.IsNullOrEmpty(form["thong_so2"]))
                    {
                        //temp = form["CPU"].ToString();
                        //cau_Hinh.CPU = (from p in db.DM_CPU
                        //                where p.TEN_CPU == temp
                        //                select p.MA_CPU).First();

                        cau_Hinh.LOAI_MUC = form["thong_so2"];
                    }

                    if (!String.IsNullOrEmpty(form["input_thong_so3"]))
                    {
                        cau_Hinh.TOC_DO = form["input_thong_so3"] + "ppm";
                    }

                    if (!String.IsNullOrEmpty(form["thong_so4"]))
                    {
                        //temp = form["CPU"].ToString();
                        //cau_Hinh.CPU = (from p in db.DM_CPU
                        //                where p.TEN_CPU == temp
                        //                select p.MA_CPU).First();

                        cau_Hinh.DO_PHAN_GIAI = form["thong_so4"];
                    }
                }

                //Tạo nhập kho
                var nhap_Kho_Create = new NHAP_KHO();

                if (!String.IsNullOrEmpty(form["MA_DON_VI"]))
                {
                    temp = form["MA_DON_VI"].ToString();
                    nhap_Kho_Create.MADV_NHAP = (from p in db.DON_VI
                                                 where p.TEN_DON_VI == temp
                                                 select p.MA_DON_VI).FirstOrDefault();
                }
                nhap_Kho_Create.NGAY_NHAP = DateTime.Now;

                //Thêm vào nhật ký thiết bị
                NHAT_KY_THIET_BI nHAT_KY_THIET_BI = new NHAT_KY_THIET_BI();
                nHAT_KY_THIET_BI.TINH_TRANG = "Mới nhập";
                nHAT_KY_THIET_BI.NGAY_THUC_HIEN = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.THIETBIs.Add(thiet_Bi);
                    await db.SaveChangesAsync();

                    temp = form["SO_SERIAL"].ToString();
                    var maTB = (from p in db.THIETBIs
                                where p.SO_SERIAL == temp
                                select p.MATB).First();

                    cau_Hinh.MATB = maTB;
                    db.CAU_HINH.Add(cau_Hinh);

                    nhap_Kho_Create.MATB = maTB;
                    db.NHAP_KHO.Add(nhap_Kho_Create);

                    nHAT_KY_THIET_BI.MATB = maTB;
                    db.NHAT_KY_THIET_BI.Add(nHAT_KY_THIET_BI);

                    await db.SaveChangesAsync();

                    ViewBag.ErrorMessage = "Thêm thành công";
                }

                //Tạo hình ảnh
                if (HINH_ANH != null)
                {
                    var hinh_Anh = new HINH_ANH();

                    temp = form["SO_SERIAL"].ToString();
                    hinh_Anh.MATB = (from p in db.THIETBIs
                                     where p.SO_SERIAL == temp
                                     select p.MATB).First();

                    string ImageName = System.IO.Path.GetFileName(HINH_ANH.FileName);
                    string physicalPath = Server.MapPath("~/Images/" + ImageName);
                    // save image in folder
                    HINH_ANH.SaveAs(physicalPath);

                    hinh_Anh.HINH1 = ImageName;

                    if (ModelState.IsValid)
                    {
                        db.HINH_ANH.Add(hinh_Anh);
                        await db.SaveChangesAsync();
                    }
                }
            }
            return View();
            //return RedirectToAction("Index");
        }

        // GET: /NhapKho/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHAP_KHO nhap_kho = await db.NHAP_KHO.FindAsync(id);
            if (nhap_kho == null)
            {
                return HttpNotFound();
            }
            return View(nhap_kho);
        }

        // GET: /NhapKho/Create
        public ActionResult Create(string value)
        {
            ViewBag.MADV_NHAP = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI");
            ViewBag.MAND_NHAP = new SelectList(db.NGUOI_DUNG, "MA_ND", "TEN_ND");
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB");
            return View();
        }

        // POST: /NhapKho/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create()
        {
            var nhap_kho = db.NHAP_KHO.Include(n => n.DON_VI).Include(n => n.NGUOI_DUNG).Include(n => n.THIETBI);
            return View(await nhap_kho.ToListAsync());
        }

        // GET: /NhapKho/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHAP_KHO nhap_kho = await db.NHAP_KHO.FindAsync(id);
            if (nhap_kho == null)
            {
                return HttpNotFound();
            }
            ViewBag.MADV_NHAP = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", nhap_kho.MADV_NHAP);
            ViewBag.MAND_NHAP = new SelectList(db.NGUOI_DUNG, "MA_ND", "TEN_ND", nhap_kho.MAND_NHAP);
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB", nhap_kho.MATB);
            return View(nhap_kho);
        }

        // POST: /NhapKho/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MA_NHAPKHO,MATB,MADV_NHAP,MAND_NHAP,NGAY")] NHAP_KHO nhap_kho)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nhap_kho).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MADV_NHAP = new SelectList(db.DON_VI, "MA_DON_VI", "TEN_DON_VI", nhap_kho.MADV_NHAP);
            ViewBag.MAND_NHAP = new SelectList(db.NGUOI_DUNG, "MA_ND", "TEN_ND", nhap_kho.MAND_NHAP);
            ViewBag.MATB = new SelectList(db.THIETBIs, "MATB", "TENTB", nhap_kho.MATB);
            return View(nhap_kho);
        }

        // GET: /NhapKho/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHAP_KHO nhap_kho = await db.NHAP_KHO.FindAsync(id);
            if (nhap_kho == null)
            {
                return HttpNotFound();
            }
            return View(nhap_kho);
        }

        // POST: /NhapKho/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            NHAP_KHO nhap_kho = await db.NHAP_KHO.FindAsync(id);
            db.NHAP_KHO.Remove(nhap_kho);
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
