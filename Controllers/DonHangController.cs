using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NgocHiepCeilingFans.Models;
using System.Security.Claims;

namespace NgocHiepCeilingFans.Controllers
{
    [Authorize]
    public class DonHangController : Controller
    {
        private readonly ShopQuatTranDbContext _db;

        public DonHangController(ShopQuatTranDbContext context)
        {
            _db = context;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<IActionResult> LichSuDatHang()
        {
            var userId = GetUserId();
            var donHangs = await _db.DonHangs
                .Where(dh => dh.TaiKhoanId == userId)
                .OrderByDescending(dh => dh.NgayDat)
                .ToListAsync();
            return View(donHangs);
        }

        public async Task<IActionResult> ChiTietDonHang(int id)
        {
            var userId = GetUserId();
            var donHang = await _db.DonHangs
                .Include(dh => dh.ChiTietDonHang)
                .ThenInclude(dhct => dhct.SanPham)
                .FirstOrDefaultAsync(dh => dh.Id == id && dh.TaiKhoanId == userId);

            if (donHang == null)
            {
                TempData["Loi"] = "Đơn hàng không tồn tại hoặc bạn không có quyền xem.";
                return RedirectToAction(nameof(LichSuDatHang));
            }
            return View(donHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDonHang(int id)
        {
            var userId = GetUserId();
            var donHang = await _db.DonHangs
                .Include(dh => dh.ChiTietDonHang)
                .FirstOrDefaultAsync(dh => dh.Id == id && dh.TaiKhoanId == userId);

            if (donHang == null)
            {
                TempData["Loi"] = "Đơn hàng không tồn tại hoặc bạn không có quyền hủy.";
                return RedirectToAction(nameof(LichSuDatHang));
            }

            if (donHang.TrangThai != "Chờ Xử Lý")
            {
                TempData["Loi"] = "Chỉ có thể hủy đơn hàng ở trạng thái 'Chờ Xử Lý'.";
                return RedirectToAction(nameof(LichSuDatHang));
            }

            donHang.TrangThai = "Đã Hủy";

            foreach (var item in donHang.ChiTietDonHang)
            {
                var sanPham = await _db.SanPhams.FindAsync(item.SanPhamId);
                if (sanPham != null)
                {
                    sanPham.SoLuongTon += item.SoLuong;
                    _db.SanPhams.Update(sanPham);
                }
            }

            await _db.SaveChangesAsync();
            TempData["ThanhCong"] = "Đơn hàng đã được hủy thành công.";

            return RedirectToAction(nameof(LichSuDatHang));
        }
    }
}