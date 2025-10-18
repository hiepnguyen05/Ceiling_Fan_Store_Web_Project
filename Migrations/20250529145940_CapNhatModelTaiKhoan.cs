using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NgocHiepCeilingFans.Migrations
{
    /// <inheritdoc />
    public partial class CapNhatModelTaiKhoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaiKhoans",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "TaiKhoans",
                columns: new[] { "Id", "Email", "MatKhau", "TenDangNhap", "VaiTro" },
                values: new object[] { 100, "admin@example.com", "admin123", "admin", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaiKhoans",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.InsertData(
                table: "TaiKhoans",
                columns: new[] { "Id", "Email", "MatKhau", "TenDangNhap", "VaiTro" },
                values: new object[] { 1, "admin@example.com", "admin123", "admin", "Admin" });
        }
    }
}
