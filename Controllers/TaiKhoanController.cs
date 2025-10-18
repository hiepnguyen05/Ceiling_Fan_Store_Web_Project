using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NgocHiepCeilingFans.Models;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using BCrypt.Net;

namespace NgocHiepCeilingFans.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ShopQuatTranDbContext _context;
        private readonly ILogger<TaiKhoanController> _logger;

        public TaiKhoanController(ShopQuatTranDbContext context, ILogger<TaiKhoanController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult DangNhap(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(string tenDangNhap, string matKhau, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
            {
                ViewBag.ThongBao = "Vui lòng nhập tên đăng nhập và mật khẩu.";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            try
            {
                var user = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap);

                if (user == null || !BCrypt.Net.BCrypt.Verify(matKhau, user.MatKhau))
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng.";
                    ViewData["ReturnUrl"] = returnUrl;
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.TenDangNhap),
                    new Claim(ClaimTypes.Role, user.VaiTro ?? "User")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    });

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                if (user.VaiTro == "Admin")
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi đăng nhập: TenDangNhap={tenDangNhap}");
                ViewBag.ThongBao = "Lỗi hệ thống. Vui lòng thử lại.";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangKy(string tenDangNhap, string matKhau, string matKhauXacNhan, string email)
        {
            if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau) ||
                string.IsNullOrEmpty(matKhauXacNhan) || string.IsNullOrEmpty(email))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            if (matKhau != matKhauXacNhan)
            {
                ViewBag.ThongBao = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            try
            {
                if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == tenDangNhap))
                {
                    ViewBag.ThongBao = "Tên đăng nhập đã tồn tại.";
                    return View();
                }

                if (await _context.TaiKhoans.AnyAsync(t => t.Email == email))
                {
                    ViewBag.ThongBao = "Email đã được sử dụng.";
                    return View();
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(matKhau);

                var user = new TaiKhoan
                {
                    TenDangNhap = tenDangNhap,
                    MatKhau = hashedPassword,
                    Email = email,
                    VaiTro = "User"
                };

                _context.TaiKhoans.Add(user);
                await _context.SaveChangesAsync();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.TenDangNhap),
                    new Claim(ClaimTypes.Role, user.VaiTro)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    });

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi đăng ký: TenDangNhap={tenDangNhap}, Email={email}");
                ViewBag.ThongBao = "Lỗi hệ thống. Vui lòng thử lại.";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("DangNhap", "TaiKhoan");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}