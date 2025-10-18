using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NgocHiepCeilingFans.Migrations
{
    /// <inheritdoc />
    public partial class AddDonHangChiTiet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHang_DonHangs_DonHangId",
                table: "ChiTietDonHang");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHang_SanPhams_SanPhamId",
                table: "ChiTietDonHang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChiTietDonHang",
                table: "ChiTietDonHang");

            migrationBuilder.RenameTable(
                name: "ChiTietDonHang",
                newName: "DonHangChiTiets");

            migrationBuilder.RenameIndex(
                name: "IX_ChiTietDonHang_SanPhamId",
                table: "DonHangChiTiets",
                newName: "IX_DonHangChiTiets_SanPhamId");

            migrationBuilder.RenameIndex(
                name: "IX_ChiTietDonHang_DonHangId",
                table: "DonHangChiTiets",
                newName: "IX_DonHangChiTiets_DonHangId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DonHangChiTiets",
                table: "DonHangChiTiets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DonHangChiTiets_DonHangs_DonHangId",
                table: "DonHangChiTiets",
                column: "DonHangId",
                principalTable: "DonHangs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DonHangChiTiets_SanPhams_SanPhamId",
                table: "DonHangChiTiets",
                column: "SanPhamId",
                principalTable: "SanPhams",
                principalColumn: "MaSanPham",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonHangChiTiets_DonHangs_DonHangId",
                table: "DonHangChiTiets");

            migrationBuilder.DropForeignKey(
                name: "FK_DonHangChiTiets_SanPhams_SanPhamId",
                table: "DonHangChiTiets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DonHangChiTiets",
                table: "DonHangChiTiets");

            migrationBuilder.RenameTable(
                name: "DonHangChiTiets",
                newName: "ChiTietDonHang");

            migrationBuilder.RenameIndex(
                name: "IX_DonHangChiTiets_SanPhamId",
                table: "ChiTietDonHang",
                newName: "IX_ChiTietDonHang_SanPhamId");

            migrationBuilder.RenameIndex(
                name: "IX_DonHangChiTiets_DonHangId",
                table: "ChiTietDonHang",
                newName: "IX_ChiTietDonHang_DonHangId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChiTietDonHang",
                table: "ChiTietDonHang",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHang_DonHangs_DonHangId",
                table: "ChiTietDonHang",
                column: "DonHangId",
                principalTable: "DonHangs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHang_SanPhams_SanPhamId",
                table: "ChiTietDonHang",
                column: "SanPhamId",
                principalTable: "SanPhams",
                principalColumn: "MaSanPham",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
