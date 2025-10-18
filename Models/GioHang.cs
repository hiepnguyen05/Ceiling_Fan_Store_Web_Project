using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NgocHiepCeilingFans.Models
{
    public class GioHang
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TaiKhoanId { get; set; }

        [Required]
        public int MaSanPham { get; set; }

        [Required]
        [StringLength(255)]
        public string TenSanPham { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Gia { get; set; }

        [StringLength(255)]
        public string HinhAnh { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }

        public DateTime NgayThem { get; set; } = DateTime.Now;

        [ForeignKey("TaiKhoanId")]
        public TaiKhoan TaiKhoan { get; set; }

        [ForeignKey("MaSanPham")]
        public SanPham SanPham { get; set; }
    }
}