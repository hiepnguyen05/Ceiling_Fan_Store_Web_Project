using Microsoft.EntityFrameworkCore;

namespace NgocHiepCeilingFans.Models
{
    public class ShopQuatTranDbContext : DbContext
    {
        public ShopQuatTranDbContext(DbContextOptions<ShopQuatTranDbContext> options) : base(options)
        {
        }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<DonHangChiTiet> DonHangChiTiets { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
          
        }
    }
    
    
}
