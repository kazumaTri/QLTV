// Project/Namespace: DTO
using System; // Cần cho DateTime
using System.Collections.Generic; // Cần cho List<TacGiaDTO>

namespace DTO
{
    /// <summary>
    /// Data Transfer Object cho thực thể Cuonsach (Cuốn Sách Cụ Thể).
    /// Chứa thông tin từ Cuonsach và các Navigation Property liên quan (Sach, TuaSach, TheLoai, TacGia).
    /// </summary>
    public class CuonSachDTO
    {
        public int Id { get; set; } // ID kỹ thuật của Cuonsach (ẩn trên UI)
        public string? MaCuonSach { get; set; } // Mã Cuốn Sách (hiển thị/nhập)

        public int IdSach { get; set; } // ID của Sach (Đầu sách/Tựa sách) mà cuốn này thuộc về (dùng cho ComboBox/Lookup)
        // Thông tin từ Sach (để hiển thị trên lưới/form chi tiết)
        public string? MaSach { get; set; } // Mã Sach (Đầu sách)
        public decimal DonGia { get; set; } // Đơn giá từ Sach
        public int NamXb { get; set; } // Năm xuất bản từ Sach
        public string NhaXb { get; set; } = null!; // Nhà xuất bản từ Sach

        // Thông tin từ TuaSach (qua Sach)
        public int IdTuaSach { get; set; } // ID Tựa sách (từ Sach.IdTuaSach)
        public string? MaTuaSach { get; set; } // Mã Tựa sách (từ Sach.IdTuaSachNavigation.MaTuaSach)
        public string? TenTuaSach { get; set; } // Tên Tựa sách (từ Sach.IdTuaSachNavigation.TenTuaSach)

        // Thông tin từ TheLoai (qua TuaSach)
        public int IdTheLoai { get; set; } // ID Thể loại (từ Sach.IdTuaSachNavigation.IdTheLoai)
        public string? TenTheLoai { get; set; } // Tên Thể loại (từ Sach.IdTuaSachNavigation.IdTheLoaiNavigation.TenTheLoai)

        // Thông tin từ TacGia (qua TuaSach) - xử lý nhiều tác giả
        public List<TacGiaDTO> TacGias { get; set; } = new List<TacGiaDTO>(); // Danh sách Tác giả liên quan đến Tựa sách

        public int TinhTrang { get; set; } // Trạng thái bằng số (e.g., 0=Có sẵn, 1=Đang mượn, 2=Hỏng/Mất...)
        public string TinhTrangText { get; set; } = "Không xác định"; // Biểu diễn trạng thái bằng text (được tính ở BUS)

        public int DaAn { get; set; } // Cờ xóa mềm (0=Không xóa, 1=Đã xóa)

        // Có thể thêm các thuộc tính khác nếu cần
        // Ví dụ: Ngày nhập kho (nếu có trong Entity Sach hoặc CtPhieunhap)
        // public DateTime? NgayNhapKho { get; set; }
        // Ví dụ: Thông tin phiếu mượn hiện tại nếu sách đang mượn (cần Include Phieumuontra và lấy thông tin)
        // public int? SoPhieuMuonHienTai { get; set; }
        // public DateTime? NgayMuonHienTai { get; set; }
        // public string? TenDocGiaDangMuon { get; set; }
    }
}