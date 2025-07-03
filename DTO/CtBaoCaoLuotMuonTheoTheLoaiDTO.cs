// Project/Namespace: DTO

using System;
using System.ComponentModel; // Cần cho DisplayName
// System.Collections.Generic không cần thiết cho DTO đơn giản này

namespace DTO // Namespace của Project DTO
{
    /// <summary>
    /// Data Transfer Object (DTO) cho Chi tiết Báo cáo lượt mượn theo thể loại.
    /// Dùng để truyền dữ liệu chi tiết báo cáo giữa các tầng.
    /// </summary>
    public class CtBaoCaoLuotMuonTheoTheLoaiDTO
    {
        // Các thuộc tính ánh xạ từ Entity CtBcluotmuontheotheloai
        // Sử dụng DisplayName attribute cho việc hiển thị thân thiện trên UI (ví dụ: DataGridView header)

        // IdBaoCao và IdTheLoai là khóa chính phức hợp, thường không hiển thị trực tiếp trên báo cáo lưới,
        // nhưng có thể cần nếu bạn muốn xem chi tiết báo cáo hoặc lọc theo ID.
        // [DisplayName("ID Báo Cáo")]
        public int IdBaoCao { get; set; } // Giữ lại nếu cần tham chiếu đến báo cáo cha

        // [DisplayName("ID Thể Loại")]
        public int IdTheLoai { get; set; } // Giữ lại nếu cần tham chiếu đến thể loại


        [DisplayName("Số Lượt Mượn")]
        public int SoLuotMuon { get; set; }

        [DisplayName("Tỉ Lệ")]
        public decimal? TiLe { get; set; } // Nullable decimal


        // --- THÊM CÁC THUỘC TÍNH TỪ NAVIGATION PROPERTIES ĐỂ HIỂN THỊ THÂN THIỆN ---
        // Dựa trên các Navigation Property trong Entity CtBcluotmuontheotheloai: IdTheLoaiNavigation (đến Theloai)
        // Chúng ta cần thông tin Tên Thể loại để hiển thị trên báo cáo.

        [DisplayName("Thể Loại")]
        public string? TenTheLoai { get; set; } // Thêm thuộc tính này để hiển thị Tên Thể loại

        // Nếu cần thông tin từ báo cáo cha (Bcluotmuontheotheloai), có thể thêm các thuộc tính khác
        // [DisplayName("Mã Báo Cáo")]
        // public string? MaBaoCao { get; set; } // Từ IdBaoCaoNavigation.MaBaoCao


        // Các thuộc tính khác nếu cần
        public string? MaTheLoai { get; set; }
    }
}