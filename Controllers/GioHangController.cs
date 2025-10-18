using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NgocHiepCeilingFans.Models;
using System.Security.Claims;

namespace NgocHiepCeilingFans.Controllers
{
    [Authorize]
    public class GioHangController : Controller
    {
        private readonly ShopQuatTranDbContext _db;

        public GioHangController(ShopQuatTranDbContext context)
        {
            _db = context;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var items = await _db.GioHangs
                .Include(g => g.SanPham)
                .Where(g => g.TaiKhoanId == userId)
                .ToListAsync();
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemVaoGio(int MaSanPham, int SoLuong)
        {
            if (SoLuong <= 0)
            {
                TempData["Loi"] = "Số lượng sản phẩm phải lớn hơn 0.";
                return RedirectToAction("Index", "SanPham");
            }

            var userId = GetUserId();
            var sanPham = await _db.SanPhams.FindAsync(MaSanPham);

            if (sanPham == null)
            {
                TempData["Loi"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index", "SanPham");
            }

            if (SoLuong > sanPham.SoLuongTon)
            {
                TempData["Loi"] = $"Sản phẩm '{sanPham.TenSanPham}' chỉ còn {sanPham.SoLuongTon} cái.";
                return RedirectToAction("ChiTiet", "SanPham", new { id = MaSanPham });
            }

            var itemTrongGio = await _db.GioHangs
                .FirstOrDefaultAsync(g => g.TaiKhoanId == userId && g.MaSanPham == MaSanPham);

            if (itemTrongGio != null)
            {
                if (itemTrongGio.SoLuong + SoLuong > sanPham.SoLuongTon)
                {
                    TempData["Loi"] = $"Tổng số lượng trong giỏ và yêu cầu ({itemTrongGio.SoLuong + SoLuong}) vượt quá số lượng tồn kho ({sanPham.SoLuongTon}).";
                    return RedirectToAction("Index", "GioHang");
                }
                itemTrongGio.SoLuong += SoLuong;
                itemTrongGio.NgayThem = DateTime.Now;
                _db.Update(itemTrongGio);
            }
            else
            {
                _db.GioHangs.Add(new GioHang
                {
                    TaiKhoanId = userId,
                    MaSanPham = MaSanPham,
                    TenSanPham = sanPham.TenSanPham,
                    Gia = sanPham.Gia,
                    HinhAnh = sanPham.HinhAnh,
                    SoLuong = SoLuong,
                    NgayThem = DateTime.Now
                });
            }

            await _db.SaveChangesAsync();
            TempData["ThanhCong"] = "Đã thêm sản phẩm vào giỏ hàng thành công!";
            return RedirectToAction("Index", "GioHang");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(int maSanPham)
        {
            var userId = GetUserId();
            var itemCanXoa = await _db.GioHangs
                .FirstOrDefaultAsync(g => g.TaiKhoanId == userId && g.MaSanPham == maSanPham);

            if (itemCanXoa == null)
            {
                TempData["Loi"] = "Sản phẩm không tồn tại trong giỏ hàng của bạn.";
            }
            else
            {
                _db.GioHangs.Remove(itemCanXoa);
                await _db.SaveChangesAsync();
                TempData["ThanhCong"] = "Đã xóa sản phẩm khỏi giỏ hàng thành công.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DatHang(List<int> danhSachSanPham)
        {
            var userId = GetUserId();

            if (danhSachSanPham == null || !danhSachSanPham.Any())
            {
                TempData["Loi"] = "Vui lòng chọn ít nhất một sản phẩm để đặt hàng.";
                return RedirectToAction("Index");
            }

            var selectedItems = await _db.GioHangs
                .Include(g => g.SanPham)
                .Where(g => g.TaiKhoanId == userId && danhSachSanPham.Contains(g.MaSanPham))
                .ToListAsync();

            if (!selectedItems.Any())
            {
                TempData["Loi"] = "Không tìm thấy sản phẩm được chọn trong giỏ hàng của bạn.";
                return RedirectToAction("Index");
            }

            return View(selectedItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanDatHang(string hoTen, string soDienThoai, string diaChi, string phuongThucThanhToan, List<int> maSanPhams)
        {
            var userId = GetUserId();
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(hoTen)) errors.Add("Vui lòng điền Họ Tên.");
            if (string.IsNullOrWhiteSpace(soDienThoai) || !System.Text.RegularExpressions.Regex.IsMatch(soDienThoai, @"^\d{10}$"))
                errors.Add("Vui lòng điền Số Điện Thoại hợp lệ (10 số).");
            if (string.IsNullOrWhiteSpace(diaChi)) errors.Add("Vui lòng điền Địa Chỉ.");
            if (maSanPhams == null || !maSanPhams.Any()) errors.Add("Không có sản phẩm nào được chọn để đặt hàng.");

            if (errors.Any())
            {
                TempData["Loi"] = string.Join(" ", errors);
                return RedirectToAction("DatHang", new { danhSachSanPham = maSanPhams });
            }

            var itemsToOrder = await _db.GioHangs
                .Where(g => g.TaiKhoanId == userId && maSanPhams.Contains(g.MaSanPham))
                .ToListAsync();

            if (!itemsToOrder.Any())
            {
                TempData["Loi"] = "Không tìm thấy sản phẩm được chọn trong giỏ hàng. Vui lòng kiểm tra lại.";
                return RedirectToAction("Index");
            }

            decimal tongTien = 0;
            var donHangChiTiets = new List<DonHangChiTiet>();

            foreach (var itemGioHang in itemsToOrder)
            {
                var sanPhamGoc = await _db.SanPhams.FindAsync(itemGioHang.MaSanPham);

                if (sanPhamGoc == null || sanPhamGoc.SoLuongTon < itemGioHang.SoLuong)
                {
                    string errorMsg = sanPhamGoc == null ? $"Sản phẩm '{itemGioHang.TenSanPham}' không còn tồn tại." :
                                                           $"Sản phẩm '{itemGioHang.TenSanPham}' chỉ còn {sanPhamGoc.SoLuongTon} cái. Vui lòng cập nhật số lượng.";
                    errors.Add(errorMsg);
                }
                else
                {
                    sanPhamGoc.SoLuongTon -= itemGioHang.SoLuong;
                    _db.SanPhams.Update(sanPhamGoc);
                    tongTien += itemGioHang.Gia * itemGioHang.SoLuong;

                    donHangChiTiets.Add(new DonHangChiTiet
                    {
                        SanPhamId = itemGioHang.MaSanPham,
                        TenSanPham = itemGioHang.TenSanPham,
                        Gia = itemGioHang.Gia,
                        HinhAnh = itemGioHang.HinhAnh,
                        SoLuong = itemGioHang.SoLuong
                    });
                }
            }

            if (errors.Any())
            {
                TempData["Loi"] = string.Join(" ", errors);
                return RedirectToAction("DatHang", new { danhSachSanPham = maSanPhams });
            }

            var donHangMoi = new DonHang
            {
                TaiKhoanId = userId,
                HoTen = hoTen,
                SoDienThoai = soDienThoai,
                DiaChi = diaChi,
                PhuongThucThanhToan = phuongThucThanhToan ?? "Thanh toán khi nhận hàng",
                NgayDat = DateTime.Now,
                TrangThai = "Chờ Xử Lý",
                TongTien = tongTien,
                ChiTietDonHang = donHangChiTiets
            };

            _db.DonHangs.Add(donHangMoi);
            _db.GioHangs.RemoveRange(itemsToOrder);
            await _db.SaveChangesAsync();

            TempData["ThanhCong"] = "Đặt hàng thành công! Vui lòng kiểm tra lịch sử đặt hàng.";
            return RedirectToAction("LichSuDatHang", "DonHang");
        }
    }
}