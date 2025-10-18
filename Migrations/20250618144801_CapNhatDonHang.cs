using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NgocHiepCeilingFans.Migrations
{
    /// <inheritdoc />
    public partial class CapNhatDonHang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChiTietSanPham",
                table: "DonHangs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChiTietSanPham",
                table: "DonHangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
