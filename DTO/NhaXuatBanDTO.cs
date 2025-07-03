// QuanLyThuVien/DTO/NhaXuatBanDTO.cs
namespace DTO
{
    /// <summary>
    /// Data Transfer Object for NhaXuatBan (Publisher)
    /// Used to transfer publisher data between layers (e.g., BUS to GUI).
    /// </summary>
    public class NhaXuatBanDTO
    {
        /// <summary>
        /// Publisher ID (Mã Nhà Xuất Bản)
        /// Matches the Id property in the NhaXuatBan entity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Publisher Name (Tên Nhà Xuất Bản)
        /// Matches the TenNXB property in the NhaXuatBan entity.
        /// </summary>
        public string TenNXB { get; set; } = null!; // Assuming Name is required

        /// <summary>
        /// Publisher Address (Địa Chỉ)
        /// Matches the DiaChi property in the NhaXuatBan entity. Nullable.
        /// </summary>
        public string? DiaChi { get; set; }

        /// <summary>
        /// Publisher Phone Number (Số Điện Thoại)
        /// Matches the DienThoai property in the NhaXuatBan entity. Nullable.
        /// </summary>
        public string? DienThoai { get; set; }
        public string? Email { get; set; }

        // Bạn có thể thêm các thuộc tính khác vào đây nếu cần cho giao diện
        // mà không nhất thiết phải có trong Entity (ví dụ: một trạng thái tính toán nào đó)
        // hoặc lược bỏ bớt thuộc tính từ Entity nếu không dùng đến ở GUI.
        // Ví dụ: Nếu có Email trong Entity và muốn dùng ở DTO:
        // public string? Email { get; set; }
    }
}