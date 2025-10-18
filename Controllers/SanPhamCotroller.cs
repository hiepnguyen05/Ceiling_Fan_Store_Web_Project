using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NgocHiepCeilingFans.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NgocHiepCeilingFans.Controllers
{
    public class SanPhamsController : Controller
    {
        private readonly ShopQuatTranDbContext _context;

        public SanPhamsController(ShopQuatTranDbContext context)
        {
            _context = context;
        }

        private void LoadDropdowns()
        {
            ViewBag.ThuongHieuList = new SelectList(new List<string> { "Panasonic", "KDK", "Casablanca", "Lucci Air", "Fanimation", "Minka Aire", "Hunter", "LuxuryFan", "ROYAL", "Khác" });
            ViewBag.LoaiQuatList = new SelectList(new List<string> { "Quạt trần cánh gỗ tự nhiên", "Quạt trần có đèn chùm", "Quạt trần ẩn (giấu cánh)", "Quạt trần có đèn", "Quạt trần hiện đại", "Quạt trần công nghệ thông minh", "Quạt trần trục dài", "Khác" });
            ViewBag.MauSacList = new SelectList(new List<string> { "Trắng", "Đen", "Nâu", "Vân gỗ", "Vàng đồng", "Bạc", "Đồng", "Xám", "Kem", "Đa màu", "Gỗ tự nhiên", "Gỗ óc chó", "Nâu cafe", "Đồng cổ" });
            ViewBag.SoCanhList = new SelectList(Enumerable.Range(2, 9));
        }

        public async Task<IActionResult> DanhSach(
            string searchString,
            string[] thuongHieuFilter,
            string[] loaiQuatFilter,
            string[] giaFilter,
            int?[] soCanhFilter,
            string sortBy,
            int pageNumber = 1,
            int pageSize = 12)
        {
            Debug.WriteLine($"DanhSach - pageNumber: {pageNumber}, pageSize: {pageSize}, searchString: {searchString}, sortBy: {sortBy}");
            Debug.WriteLine($"thuongHieuFilter: {string.Join(", ", thuongHieuFilter ?? Array.Empty<string>())}");
            Debug.WriteLine($"loaiQuatFilter: {string.Join(", ", loaiQuatFilter ?? Array.Empty<string>())}");
            Debug.WriteLine($"giaFilter: {string.Join(", ", giaFilter ?? Array.Empty<string>())}");
            Debug.WriteLine($"soCanhFilter: {string.Join(", ", soCanhFilter?.Select(c => c?.ToString() ?? "null") ?? Array.Empty<string>())}");

            IQueryable<SanPham> sanPhamsQuery = _context.SanPhams;

            if (!string.IsNullOrEmpty(searchString))
            {
                sanPhamsQuery = sanPhamsQuery.Where(s => s.TenSanPham.Contains(searchString)
                                             || s.MoTa.Contains(searchString)
                                             || s.ThuongHieu.Contains(searchString));
            }

            if (thuongHieuFilter != null && thuongHieuFilter.Any())
            {
                sanPhamsQuery = sanPhamsQuery.Where(s => thuongHieuFilter.Contains(s.ThuongHieu));
            }

            if (loaiQuatFilter != null && loaiQuatFilter.Any())
            {
                sanPhamsQuery = sanPhamsQuery.Where(s => loaiQuatFilter.Contains(s.LoaiQuat));
            }

            List<SanPham> sanPhams;
            if (giaFilter != null && giaFilter.Any())
            {
                var validPriceRanges = new Dictionary<string, (decimal minPrice, decimal maxPrice)>
                {
                    { "0-1000000", (0, 1000000) },
                    { "1000001-3000000", (1000001, 3000000) },
                    { "3000001-5000000", (3000001, 5000000) },
                    { "5000001-", (5000001, decimal.MaxValue) }
                };

                sanPhams = await sanPhamsQuery.ToListAsync();
                var filteredByPriceProducts = new List<SanPham>();

                foreach (var priceRange in giaFilter)
                {
                    if (string.IsNullOrWhiteSpace(priceRange) || !validPriceRanges.ContainsKey(priceRange))
                    {
                        Debug.WriteLine($"Bỏ qua giá không hợp lệ: {priceRange}");
                        continue;
                    }

                    var (minPrice, maxPrice) = validPriceRanges[priceRange];
                    var productsInThisRange = sanPhams.Where(s => s.Gia >= minPrice && s.Gia <= maxPrice);
                    filteredByPriceProducts.AddRange(productsInThisRange);
                }

                sanPhams = filteredByPriceProducts.DistinctBy(s => s.MaSanPham).ToList();
            }
            else
            {
                sanPhams = await sanPhamsQuery.ToListAsync();
            }

            if (soCanhFilter != null && soCanhFilter.Any(c => c.HasValue))
            {
                var validSoCanhFilter = soCanhFilter.Where(c => c.HasValue).Select(c => c.Value).ToArray();
                if (validSoCanhFilter.Length > 0)
                {
                    sanPhams = sanPhams.Where(s => validSoCanhFilter.Contains(s.SoCanh)).ToList();
                }
            }

            switch (sortBy)
            {
                case "price-asc":
                    sanPhams = sanPhams.OrderBy(s => s.Gia).ToList();
                    break;
                case "price-desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.Gia).ToList();
                    break;
                case "name-asc":
                    sanPhams = sanPhams.OrderBy(s => s.TenSanPham).ToList();
                    break;
                case "name-desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.TenSanPham).ToList();
                    break;
                case "newest":
                    sanPhams = sanPhams.OrderByDescending(s => s.NgayTao).ToList();
                    break;
                case "oldest":
                    sanPhams = sanPhams.OrderBy(s => s.NgayTao).ToList();
                    break;
                case "featured":
                default:
                    sanPhams = sanPhams.OrderByDescending(s => s.MaSanPham).ToList();
                    break;
            }

            int totalItems = sanPhams.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages)); 
            sanPhams = sanPhams.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            Debug.WriteLine($"TotalItems: {totalItems}, TotalPages: {totalPages}, CurrentPage: {pageNumber}, PageSize: {pageSize}, DisplayedItems: {sanPhams.Count}");

            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;


            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentThuongHieuFilter = thuongHieuFilter;
            ViewBag.CurrentLoaiQuatFilter = loaiQuatFilter;
            ViewBag.CurrentGiaFilter = giaFilter;
            ViewBag.CurrentSoCanhFilter = soCanhFilter;
            ViewBag.CurrentSortBy = sortBy;

            LoadDropdowns();

            return View(sanPhams);
        }

        public async Task<IActionResult> ChiTiet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FirstOrDefaultAsync(m => m.MaSanPham == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            var randomProducts = await _context.SanPhams
                                              .Where(s => s.MaSanPham != id)
                                              .OrderBy(s => Guid.NewGuid())
                                              .Take(8)
                                              .ToListAsync();

            ViewBag.RandomProducts = randomProducts;

            return View(sanPham);
        }

        public async Task<IActionResult> TheoPhongCach(string loaiQuat = null, int pageNumber = 1, int pageSize = 12)
        {
            IQueryable<SanPham> sanPhamsQuery = _context.SanPhams;

            if (!string.IsNullOrEmpty(loaiQuat))
            {
                sanPhamsQuery = sanPhamsQuery.Where(s => s.LoaiQuat == loaiQuat);
            }

            var sanPhams = await sanPhamsQuery.ToListAsync();
            int totalItems = sanPhams.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));
            sanPhams = sanPhams.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.LoaiQuat = loaiQuat;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;

            return View(sanPhams);
        }

        public async Task<IActionResult> TheoThuongHieu(string thuongHieu = null, int pageNumber = 1, int pageSize = 12)
        {
            IQueryable<SanPham> sanPhamsQuery = _context.SanPhams;

            if (!string.IsNullOrEmpty(thuongHieu))
            {
                sanPhamsQuery = sanPhamsQuery.Where(s => s.ThuongHieu == thuongHieu);
            }

            var sanPhams = await sanPhamsQuery.ToListAsync();
            int totalItems = sanPhams.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));
            sanPhams = sanPhams.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.ThuongHieu = thuongHieu;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;

            return View(sanPhams);
        }
    }
}
