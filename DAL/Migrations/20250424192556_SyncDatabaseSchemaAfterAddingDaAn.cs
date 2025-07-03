using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class SyncDatabaseSchemaAfterAddingDaAn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.CreateTable(  // <<< Đã comment hoặc xóa
            //     name: "BCLUOTMUONTHEOTHELOAI", // <<< Đã comment hoặc xóa
            //     // ... các dòng khác của CreateTable ... // <<< Đã comment hoặc xóa
            //     ); // <<< Đã comment hoặc xóa

            // // ... Comment hoặc xóa các lệnh CreateTable khác ...

            // // Lệnh AddColumn cho DaAn cũng không cần nếu bạn đã thêm thủ công rồi
            // migrationBuilder.AddColumn<bool>(
            //     name: "DaAn",
            //     table: "NGUOIDUNG",
            //     type: "bit",
            //     nullable: false,
            //     defaultValue: false);

            // => Phương thức Up() có thể hoàn toàn trống sau khi sửa
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BCSACHTRATRE");

            migrationBuilder.DropTable(
                name: "CT_BCLUOTMUONTHEOTHELOAI");

            migrationBuilder.DropTable(
                name: "CT_PHIEUNHAP");

            migrationBuilder.DropTable(
                name: "CT_TACGIA");

            migrationBuilder.DropTable(
                name: "PHANQUYEN");

            migrationBuilder.DropTable(
                name: "PHIEUMUONTRA");

            migrationBuilder.DropTable(
                name: "PHIEUTHU");

            migrationBuilder.DropTable(
                name: "THAMSO");

            migrationBuilder.DropTable(
                name: "BCLUOTMUONTHEOTHELOAI");

            migrationBuilder.DropTable(
                name: "PHIEUNHAPSACH");

            migrationBuilder.DropTable(
                name: "TACGIA");

            migrationBuilder.DropTable(
                name: "CHUCNANG");

            migrationBuilder.DropTable(
                name: "CUONSACH");

            migrationBuilder.DropTable(
                name: "DOCGIA");

            migrationBuilder.DropTable(
                name: "SACH");

            migrationBuilder.DropTable(
                name: "LOAIDOCGIA");

            migrationBuilder.DropTable(
                name: "NGUOIDUNG");

            migrationBuilder.DropTable(
                name: "TUASACH");

            migrationBuilder.DropTable(
                name: "NHOMNGUOIDUNG");

            migrationBuilder.DropTable(
                name: "THELOAI");
        }
    }
}
