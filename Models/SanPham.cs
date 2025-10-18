using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace NgocHiepCeilingFans.Models
{
    public class SanPham
    {
        [Key]
        public int MaSanPham { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        [StringLength(255)]
        [Display(Name = "Tên sản phẩm")]
        public string TenSanPham { get; set; }

        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }

        [Required(ErrorMessage = "Giá không được để trống.")]
        [Column(TypeName = "money")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
        [Display(Name = "Giá")]
        public decimal Gia { get; set; }

        [Display(Name = "Hình ảnh")]
        public string? HinhAnh { get; set; }

        [Display(Name = "Thương hiệu")]
        public string ThuongHieu { get; set; }

        [Display(Name = "Loại quạt")]
        public string LoaiQuat { get; set; }

        [Required(ErrorMessage = "Số cánh không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số cánh phải lớn hơn 0.")]
        [Display(Name = "Số cánh")]
        public int SoCanh { get; set; }

        [Display(Name = "Màu sắc")]
        public string MauSac { get; set; }

        [Display(Name = "Có đèn")]
        public bool CoDen { get; set; }

        [Required(ErrorMessage = "Thời gian bảo hành không được để trống.")]
        [Range(0, int.MaxValue, ErrorMessage = "Thời gian bảo hành không hợp lệ.")]
        [Display(Name = "Bảo hành (tháng)")]
        public int BaoHanh { get; set; }

        [Required(ErrorMessage = "Số lượng tồn không được để trống.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn không hợp lệ.")]
        [Display(Name = "Số lượng tồn")]
        public int SoLuongTon { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime NgayCapNhat { get; set; }
    }
}