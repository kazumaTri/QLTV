// Project/Namespace: DTO
namespace DTO
{
    public class TacGiaDTO
    {
        public int Id { get; set; }
        public string? Matacgia { get; set; } // <<< Thuộc tính Matacgia (chữ 'm' thường)
        public string TenTacGia { get; set; } = null!;
        public bool DaAn { get; set; }
        // Có thể thêm các thuộc tính khác nếu cần
    }
}