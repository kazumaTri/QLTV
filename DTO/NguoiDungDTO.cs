// Project/Namespace: DTO
using System;

namespace DTO
{
    public class NguoiDungDTO
    {
        public int Id { get; set; }
        public string? MaNguoiDung { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string TenHienThi { get; set; } = string.Empty; // Map từ Nguoidung.TenNguoiDung
        public int? IdNhomNguoiDung { get; set; } // Entity là int
        public string? TenNhomNguoiDung { get; set; }
        public DateTime? NgaySinh { get; set; } // Entity là DateTime?
        public string? ChucVu { get; set; } // Entity là string?
                                            // Đã loại bỏ DaAn (vì Entity không có)
                                            // Đã loại bỏ DienThoai (vì Entity không có)

        // Thuộc tính chỉ dùng khi thêm/sửa mật khẩu (nếu cần tách biệt)
        // public string? MatKhauMoi { get; set; }
    }
}