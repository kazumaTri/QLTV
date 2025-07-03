using System;
using System.Collections.Generic; // Cần cho List<string>

namespace DTO
{
    /// <summary>
    /// Data Transfer Object cho thông tin chi tiết của Sách,
    /// bao gồm thông tin từ Tựa Sách, Thể Loại và Tác Giả liên quan.
    /// </summary>
    public class SachDTO
    {
        public int Id { get; set; }
        public string? MaSach { get; set; } // Mã sách (ví dụ: S00001). Có thể null/empty nếu tự sinh ở DB.

        // Thông tin từ TuaSach
        public int IdTuaSach { get; set; }
        public string TenTuaSach { get; set; } = string.Empty;

        // Thông tin từ TheLoai (qua TuaSach)
        public string TenTheLoai { get; set; } = string.Empty;

        // Thông tin từ TacGia (qua TuaSach)
        public List<string> TenTacGia { get; set; } = new List<string>(); // Danh sách tên các tác giả

        // Thông tin riêng của Sach
        public int SoLuong { get; set; } // Tổng số lượng nhập
        public int SoLuongConLai { get; set; } // Số lượng hiện có trong thư viện
        public int DonGia { get; set; }
        public int NamXb { get; set; } // Năm xuất bản (đã sửa tên thuộc tính theo code BUS/DAL)
        public string NhaXb { get; set; } = string.Empty; // Nhà xuất bản (đã sửa tên thuộc tính theo code BUS/DAL)

        /// <summary>
        /// Trạng thái xóa mềm (true: Đã ẩn/xóa mềm, false: Bình thường).
        /// </summary>
        public bool DaAn { get; set; } // <<< ĐÃ SỬA TỪ int SANG bool

        /// <summary>
        /// Thuộc tính chỉ đọc dùng để hiển thị thông tin sách trong UI.
        /// </summary>
        public string DisplaySach => $"{MaSach} - {TenTuaSach} ({NhaXb}, {NamXb})";

        // Có thể thêm các thuộc tính khác nếu cần hiển thị hoặc xử lý
    }
}