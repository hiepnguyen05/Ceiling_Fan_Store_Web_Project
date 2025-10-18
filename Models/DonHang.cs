using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NgocHiepCeilingFans.Models
{
    public class DonHang
    {
        public int Id { get; set; }
        public int TaiKhoanId { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public DateTime NgayDat { get; set; }
        public string TrangThai { get; set; }
        public decimal TongTien { get; set; }
       
        public TaiKhoan? TaiKhoan { get; set; }
        public List<DonHangChiTiet> ChiTietDonHang { get; set; }
    }
}