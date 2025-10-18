using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NgocHiepCeilingFans.Models;
using System.IO;

namespace NgocHiepCeilingFans.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SanPhamsController : Controller
    {
        private readonly ShopQuatTranDbContext _context;

        public SanPhamsController(ShopQuatTranDbContext context)
        {
            _context = context;
        }

        private async Task<string> LuuHinhAnh(IFormFile imageFile)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "sanpham");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            
            var filePath = Path.Combine(uploadPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            
            return "/images/sanpham/" + fileName;
        }

        private void XoaHinhAnh(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
        }
        private void LoadDropdowns()
        {
            ViewBag.ThuongHieuList = new SelectList(new[] { 
                "Panasonic", "KDK", "Casablanca", "Lucci Air", "Fanimation", 
                "Minka Aire", "Hunter", "LuxuryFan", "ROYAL", "Khác" 
            });
            
            ViewBag.LoaiQuatList = new SelectList(new[] { 
                "Quạt trần cánh gỗ tự nhiên", "Quạt trần có đèn chùm", 
                "Quạt trần ẩn (giấu cánh)", "Quạt trần có đèn", 
                "Quạt trần hiện đại", "Quạt trần công nghệ thông minh", 
                "Quạt trần trục dài", "Khác" 
            });
            
            ViewBag.SoCanhList = new SelectList(Enumerable.Range(2, 9));
            
            ViewBag.MauSacList = new SelectList(new[] { 
                "Trắng", "Đen", "Nâu", "Vân gỗ", "Vàng đồng", "Bạc", 
                "Đồng", "Xám", "Kem", "Đa màu", "Gỗ tự nhiên", 
                "Gỗ óc chó", "Nâu cafe", "Đồng cổ" 
            });
        }

        public async Task<IActionResult> DanhSach(
            string searchString, 
            string[] thuongHieuFilter, 
            string[] loaiQuatFilter, 
            string[] giaFilter, 
            int?[] soCanhFilter, 
            string sortBy = "name", 
            int pageNumber = 1, 
            int pageSize = 12)
        {
            var query = _context.SanPhams.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.TenSanPham.Contains(searchString) || s.MoTa.Contains(searchString));
            }

            if (thuongHieuFilter?.Length > 0)
            {
                query = query.Where(s => thuongHieuFilter.Contains(s.ThuongHieu));
            }

            if (loaiQuatFilter?.Length > 0)
            {
                query = query.Where(s => loaiQuatFilter.Contains(s.LoaiQuat));
            }

            if (giaFilter?.Length > 0)
            {
                query = query.Where(s => giaFilter.Any(g => 
                    (g == "0-1000000" && s.Gia <= 1000000m) ||
                    (g == "1000001-3000000" && s.Gia >= 1000001m && s.Gia <= 3000000m) ||
                    (g == "3000001-5000000" && s.Gia >= 3000001m && s.Gia <= 5000000m) ||
                    (g == "5000001-" && s.Gia >= 5000001m)
                ));
            }

            if (soCanhFilter?.Any(c => c.HasValue) == true)
            {
                var validSoCanh = soCanhFilter.Where(c => c.HasValue).Select(c => c.Value);
                query = query.Where(s => validSoCanh.Contains(s.SoCanh));
            }

            query = sortBy switch
            {
                "name" => query.OrderBy(s => s.TenSanPham),
                "name_desc" => query.OrderByDescending(s => s.TenSanPham),
                "price" => query.OrderBy(s => s.Gia),
                "price_desc" => query.OrderByDescending(s => s.Gia),
                "date" => query.OrderBy(s => s.NgayTao),
                "date_desc" => query.OrderByDescending(s => s.NgayTao),
                _ => query.OrderBy(s => s.TenSanPham)
            };

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var sanPhams = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.ThuongHieuFilter = thuongHieuFilter ?? Array.Empty<string>();
            ViewBag.LoaiQuatFilter = loaiQuatFilter ?? Array.Empty<string>();
            ViewBag.GiaFilter = giaFilter ?? Array.Empty<string>();
            ViewBag.SoCanhFilter = soCanhFilter ?? Array.Empty<int?>();
            ViewBag.SortBy = sortBy;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;

            LoadDropdowns();
            return View(sanPhams);
        }

        public IActionResult ThemMoi()
        {
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ThemMoi(SanPham sanPham, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(sanPham);
            }

            if (imageFile != null)
            {
                sanPham.HinhAnh = await LuuHinhAnh(imageFile);
            }

            sanPham.NgayTao = DateTime.Now;
            sanPham.NgayCapNhat = DateTime.Now;
            
            _context.SanPhams.Add(sanPham);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Thêm sản phẩm thành công!";
            return RedirectToAction("DanhSach");
        }

        public async Task<IActionResult> ChiTiet(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            return View(sanPham);
        }

        public async Task<IActionResult> ChinhSua(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            
            LoadDropdowns();
            return View(sanPham);
        }

        [HttpPost]
        public async Task<IActionResult> ChinhSua(SanPham sanPham, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(sanPham);
            }

            var existingSanPham = await _context.SanPhams.FindAsync(sanPham.MaSanPham);
            if (existingSanPham == null)
            {
                return NotFound();
            }

            if (imageFile != null)
            {

                XoaHinhAnh(existingSanPham.HinhAnh);
                sanPham.HinhAnh = await LuuHinhAnh(imageFile);
            }
            else
            {
                sanPham.HinhAnh = existingSanPham.HinhAnh;
            }

            sanPham.NgayTao = existingSanPham.NgayTao;
            sanPham.NgayCapNhat = DateTime.Now;

            _context.Entry(existingSanPham).CurrentValues.SetValues(sanPham);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction("DanhSach");
        }

        public async Task<IActionResult> Xoa(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                TempData["ThongBao"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("DanhSach");
            }

            XoaHinhAnh(sanPham.HinhAnh);

            _context.SanPhams.Remove(sanPham);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Xóa sản phẩm thành công!";
            return RedirectToAction("DanhSach");
        }
    }
}