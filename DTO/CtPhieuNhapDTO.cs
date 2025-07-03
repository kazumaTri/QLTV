namespace DTO
{
    public class CtPhieuNhapDTO
    {
        public int IdSach { get; set; }
        public string? MaSach { get; set; } // Mã để hiển thị hoặc tìm kiếm
        public string? TenSach { get; set; } // Tên sách để hiển thị
        public int SoLuongNhap { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuongNhap * DonGia; // Tự động tính

        // Thêm constructor hoặc các thuộc tính khác nếu cần
    }
}