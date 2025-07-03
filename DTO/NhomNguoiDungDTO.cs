// Project/Namespace: DTO

using System;
using System.ComponentModel; // Cần cho DisplayName
// System.Collections.Generic không cần thiết cho DTO đơn giản này

namespace DTO // Namespace của Project DTO
{
    /// <summary>
    /// Data Transfer Object (DTO) cho Nhóm Người Dùng (Vai trò).
    /// Dùng để truyền thông tin nhóm người dùng giữa các tầng.
    /// </summary>
    public class NhomNguoiDungDTO
    {
        // Các thuộc tính ánh xạ từ Entity Nhomnguoidung
        // Sử dụng DisplayName attribute cho việc hiển thị thân thiện trên UI (ví dụ: ComboBox, DataGridView header)

        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("Mã Nhóm")]
        public string? MaNhomNguoiDung { get; set; } // Theo Entity Nhomnguoidung, MaNhomNguoiDung có thể null

        [DisplayName("Tên Nhóm")]
        public string TenNhomNguoiDung { get; set; } = null!; // Theo Entity Nhomnguoidung, TenNhomNguoiDung không null

        // Không bao gồm các Navigation Properties (ICollection<Nguoidung>, ICollection<Chucnang>) trong DTO đơn giản này.
        // Nếu cần hiển thị số lượng người dùng/chức năng, có thể thêm thuộc tính riêng được tính toán ở tầng BUS.
        // [DisplayName("Số Người Dùng")]
        // public int SoLuongNguoiDung { get; set; }
        // [DisplayName("Số Chức Năng")]
        // public int SoLuongChucNang { get; set; }
    }
}