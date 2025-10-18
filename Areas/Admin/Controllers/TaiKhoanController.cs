using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NgocHiepCeilingFans.Models;
using System.Linq;

namespace NgocHiepCeilingFans.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TaiKhoanController : Controller
    {
        private readonly ShopQuatTranDbContext _context;

        public TaiKhoanController(ShopQuatTranDbContext context)
        {
            _context = context;
        }

        public IActionResult DanhSach()
        {
            var dsTaiKhoan = _context.TaiKhoans.ToList();
            return View(dsTaiKhoan);
        }

        public IActionResult GanQuyen(int id)
        {
            var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.Id == id);
            if (taiKhoan == null)
                return NotFound();

            return View(taiKhoan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GanQuyen(int id, string VaiTro)
        {
            var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.Id == id);
            if (taiKhoan == null)
                return NotFound();

            if (string.IsNullOrEmpty(VaiTro))
            {
                ModelState.AddModelError("VaiTro", "Vui lòng chọn vai trò");
                return View(taiKhoan);
            }

            taiKhoan.VaiTro = VaiTro;
            _context.SaveChanges();

            return RedirectToAction("DanhSach");
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult XoaTaiKhoan(int id)
        {
            var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.Id == id);
            if (taiKhoan == null)
            {
                TempData["ThongBao"] = "Tài khoản không tồn tại.";
                return RedirectToAction("DanhSach");
            }

            _context.TaiKhoans.Remove(taiKhoan);
            _context.SaveChanges();

            TempData["ThongBao"] = "Xóa tài khoản thành công.";
            return RedirectToAction("DanhSach");
        }


    }
}
