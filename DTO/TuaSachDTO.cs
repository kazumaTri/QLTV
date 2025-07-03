// Project/Namespace: DTO
using System.Collections.Generic; // Cần cho List

namespace DTO
{
    public class TuaSachDTO
    {
        public int Id { get; set; }
        public string? MaTuaSach { get; set; }
        public string TenTuaSach { get; set; } = null!;
        public int IdTheLoai { get; set; }
        public string? TenTheLoai { get; set; }
        public List<TacGiaDTO> TacGias { get; set; } = new List<TacGiaDTO>(); // <<< Thuộc tính TacGias
        public int SoLuongSach { get; set; } // <<< Thuộc tính SoLuongSach
        public int SoLuongSachConLai { get; set; } // <<< Thuộc tính SoLuongSachConLai
        public int? DaAn { get; set; }

        // Thêm các thuộc tính khác từ TuaSach entity nếu cần hiển thị/làm việc ở GUI
    }
}