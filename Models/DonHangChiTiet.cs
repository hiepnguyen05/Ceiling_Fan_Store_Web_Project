using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NgocHiepCeilingFans.Models
{
    public class DonHangChiTiet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DonHangId { get; set; }

        [Required]
        public int SanPhamId { get; set; }

        [Required]
        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Gia { get; set; }

        public string? TenSanPham { get; set; } 
        public string? HinhAnh { get; set; }

        [ForeignKey("DonHangId")]
        public DonHang? DonHang { get; set; }

        [ForeignKey("SanPhamId")]
        public SanPham? SanPham { get; set; }
    }
}
