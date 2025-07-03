// File: DTO/PhieuThuPhatDTO.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    /// <summary>
    /// Data Transfer Object cho việc tạo Phiếu Thu Tiền Phạt.
    /// Chứa thông tin cần thiết để BUS/DAL xử lý nghiệp vụ thu phạt.
    /// </summary>
    public class PhieuThuPhatDTO // <<< Sửa thành public class
    {
        // --- Các thuộc tính cần thiết để tạo phiếu thu ---

        /// <summary>
        /// ID của Độc giả nộp tiền phạt.
        /// </summary>
        public int IdDocGia { get; set; }

        /// <summary>
        /// Số tiền thu (phải lớn hơn 0 và không vượt quá nợ hiện tại).
        /// Kiểu int khớp với Entity Phieuthu.SoTienThu
        /// </summary>
        public int SoTienThu { get; set; }

        /// <summary>
        /// ID của Người dùng (nhân viên) lập phiếu thu.
        /// Có thể null nếu hệ thống không yêu cầu hoặc lấy tự động.
        /// </summary>
        public int? IdNguoiLapPhieu { get; set; }

        // --- Các thuộc tính khác có thể thêm nếu cần hiển thị thông tin liên quan ---
        // Ví dụ:
        // public string? TenDocGia { get; set; } // Tên độc giả (để hiển thị)
        // public int? NoHienTai { get; set; } // Nợ hiện tại trước khi thu (để hiển thị)

        // Constructor (tùy chọn)
        public PhieuThuPhatDTO() { }

        public PhieuThuPhatDTO(int idDocGia, int soTienThu, int? idNguoiLap = null)
        {
            IdDocGia = idDocGia;
            SoTienThu = soTienThu;
            IdNguoiLapPhieu = idNguoiLap;
        }
    }
}