using System;

namespace DTO
{
    /// <summary>
    /// Data Transfer Object (DTO) cho Phiếu Mượn Trả.
    /// Dùng để truyền dữ liệu giữa tầng BUS và GUI.
    /// Bao gồm thông tin phiếu và thông tin liên quan (độc giả, cuốn sách).
    /// </summary>
    public class PhieuMuonTraDTO
    {
        // Thuộc tính từ Entity Phieumuontra
        public int SoPhieuMuonTra { get; set; } // Số phiếu (khóa chính)

        public int IdDocGia { get; set; } // Khóa ngoại đến Độc giả
        public int IdCuonSach { get; set; } // Khóa ngoại đến Cuốn sách

        public DateTime NgayMuon { get; set; } // Ngày mượn (non-nullable)
        public DateTime? NgayTra { get; set; } // Ngày trả (nullable)
        public DateTime HanTra { get; set; } // Hạn trả (non-nullable)

        // SoTienPhat là int? trong Entity, giữ nguyên kiểu trong DTO để khớp
        public int? SoTienPhat { get; set; } // Số tiền phạt (nullable integer)


        // Thông tin liên quan từ navigation properties (cần include khi lấy Entity)
        // Thông tin Độc giả
        public string? MaDocGia { get; set; } // Từ Docgia.MaDocGia
        public string? TenDocGia { get; set; } // Từ Docgia.TenDocGia

        // Thông tin Cuốn sách (cần truy cập qua Cuonsach -> Sach -> Tuasach)
        public string? MaCuonSach { get; set; } // Từ Cuonsach.MaCuonSach
        public string? TenTuaSach { get; set; } // Từ Tuasach.TenTuaSach (cần include sâu)

        // Thêm các thuộc tính khác nếu cần hiển thị
        // public string? TenTacGia { get; set; } // Tên tác giả (nếu cần)
        // public string? TenTheLoai { get; set; } // Tên thể loại (nếu cần)
    }
}