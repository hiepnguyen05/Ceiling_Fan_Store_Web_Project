using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NgocHiepCeilingFans.Migrations
{
    /// <inheritdoc />
    public partial class TaoBangSanPham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SanPhams",
                columns: table => new
                {
                    MaSanPham = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenSanPham = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gia = table.Column<decimal>(type: "money", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThuongHieu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiQuat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoCanh = table.Column<int>(type: "int", nullable: false),
                    MauSac = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoDen = table.Column<bool>(type: "bit", nullable: false),
                    BaoHanh = table.Column<int>(type: "int", nullable: false),
                    SoLuongTon = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPhams", x => x.MaSanPham);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SanPhams");
        }
    }
}
