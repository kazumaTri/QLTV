// Project/Namespace: DTO

using System;
using System.ComponentModel; // Cần cho DisplayName
// System.Collections.Generic không cần thiết cho DTO đơn giản này

namespace DTO // Namespace của Project DTO
{
    /// <summary>
    /// Data Transfer Object cho Tham số hệ thống.
    /// Dùng để truyền thông tin tham số cấu hình giữa các tầng.
    /// </summary>
    public class ThamSoDTO
    {
        // Các thuộc tính ánh xạ từ Entity Thamso
        // Sử dụng DisplayName attribute cho việc hiển thị thân thiện trên UI (ví dụ: PropertyGrid, Form Cài đặt)

        [DisplayName("ID")]
        public int Id { get; set; } // Thường không hiển thị/chỉnh sửa trên UI

        [DisplayName("Tuổi Tối Thiểu")]
        public int TuoiToiThieu { get; set; }

        [DisplayName("Tuổi Tối Đa")]
        public int TuoiToiDa { get; set; }

        [DisplayName("Thời Hạn Thẻ (tháng)")] // Giả định ThoiHanThe tính bằng tháng
        public int ThoiHanThe { get; set; }

        [DisplayName("Khoảng Cách Xuất Bản (năm)")] // Giả định KhoangCachXuatBan tính bằng năm
        public int KhoangCachXuatBan { get; set; }

        [DisplayName("Số Sách Mượn Tối Đa")]
        public int SoSachMuonToiDa { get; set; }

        [DisplayName("Số Ngày Mượn Tối Đa")]
        public int SoNgayMuonToiDa { get; set; }

        [DisplayName("Đơn Giá Phạt (theo ngày)")] // Giả định DonGiaPhat là tiền phạt mỗi ngày trễ
        public int DonGiaPhat { get; set; }

        [DisplayName("Áp Dụng Quy Định Tiền Thu")] // Giả định AdQdkttienThu là một cờ (ví dụ: 0=Không áp dụng, 1=Áp dụng)
        public int AdQdkttienThu { get; set; } // Bạn có thể cân nhắc dùng bool và mapping 0/1 nếu phù hợp với nghiệp vụ
    }
}