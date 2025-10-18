using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NgocHiepCeilingFans.Models;

namespace NgocHiepCeilingFans.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DonHangController : Controller
    {
        private readonly ShopQuatTranDbContext _db;

        public DonHangController(ShopQuatTranDbContext context)
        {
            _db = context;
        }

        public async Task<IActionResult> Index(string trangThai)
        {
            ViewBag.TrangThai = trangThai;

            var query = _db.DonHangs.AsQueryable();

            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(dh => dh.TrangThai == trangThai);
            }

            var donHangs = await query.OrderByDescending(dh => dh.NgayDat).ToListAsync();
            return View(donHangs);
        }


        public async Task<IActionResult> ChiTietDonHang(int id)
        {
            var donHang = await _db.DonHangs
                                   .Include(dh => dh.ChiTietDonHang)
                                   .ThenInclude(dhct => dhct.SanPham)
                                   .FirstOrDefaultAsync(dh => dh.Id == id);

            if (donHang == null)
            {
                TempData["Loi"] = "Đơn hàng không tồn tại.";
                return RedirectToAction("Index");
            }

            return View(donHang);
        }

        [HttpGet]
        public async Task<IActionResult> SuaTrangThai(int id)
        {
            var donHang = await _db.DonHangs.FindAsync(id);
            if (donHang == null)
            {
                TempData["Loi"] = "Đơn hàng không tồn tại.";
                return RedirectToAction("Index");
            }
            return View(donHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaTrangThai(int id, string TrangThaiMoi)
        {
            var donHang = await _db.DonHangs
                                   .Include(dh => dh.ChiTietDonHang)
                                   .FirstOrDefaultAsync(dh => dh.Id == id);

            if (donHang == null)
            {
                TempData["Loi"] = "Đơn hàng không tồn tại.";
                return RedirectToAction("Index");
            }

            if (donHang.TrangThai == "Đã Hủy" || donHang.TrangThai == "Đã Hoàn Thành")
            {
                TempData["Loi"] = $"Không thể thay đổi trạng thái của đơn hàng đã '{donHang.TrangThai}'.";
                return RedirectToAction("Index");
            }

            if (TrangThaiMoi == "Đã Hủy" && donHang.TrangThai != "Đã Hủy")
            {
                if (donHang.ChiTietDonHang != null)
                {
                    foreach (var item in donHang.ChiTietDonHang)
                    {
                        var sanPham = await _db.SanPhams.FindAsync(item.SanPhamId);
                        if (sanPham != null)
                        {
                            sanPham.SoLuongTon += item.SoLuong;
                            _db.SanPhams.Update(sanPham);
                        }
                    }
                }
            }

            donHang.TrangThai = TrangThaiMoi;
            _db.Update(donHang);
            await _db.SaveChangesAsync();
            TempData["ThanhCong"] = $"Đã cập nhật trạng thái đơn hàng #{donHang.Id} thành '{TrangThaiMoi}'.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(int id)
        {
            var donHang = await _db.DonHangs.FindAsync(id);
            if (donHang == null)
            {
                TempData["Loi"] = "Đơn hàng không tồn tại.";
                return RedirectToAction("Index");
            }

            if (donHang.TrangThai != "Đã Hủy" && donHang.TrangThai != "Đã Hoàn Thành")
            {
                TempData["Loi"] = "Vui lòng hủy hoặc hoàn thành đơn hàng trước khi xóa. Xóa vĩnh viễn đơn hàng có thể gây mất dữ liệu thống kê.";
                return RedirectToAction("Index");
            }

            _db.DonHangs.Remove(donHang);
            await _db.SaveChangesAsync();
            TempData["ThanhCong"] = "Đã xóa đơn hàng thành công.";
            return RedirectToAction("Index");
        }
    }
}
