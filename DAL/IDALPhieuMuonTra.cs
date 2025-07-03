// DAL/IDALPhieuMuonTra.cs
using DAL.Models;
using System; // Cần cho DateTime
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL
{
    /// <summary>
    /// Interface cho lớp truy cập dữ liệu Phiếu Mượn Trả.
    /// Định nghĩa các phương thức tương tác với cơ sở dữ liệu liên quan đến Phiếu Mượn Trả.
    /// Đã sửa kiểu trả về của AddAsync để khớp với cài đặt phổ biến.
    /// Đã thêm phương thức lấy lịch sử phạt.
    /// </summary>
    public interface IDALPhieuMuonTra
    {
        // --- Các phương thức lấy dữ liệu ---
        Task<List<Phieumuontra>> GetAllAsync();
        Task<Phieumuontra?> GetByIdAsync(int id); // Sử dụng id chung, hoặc đổi thành soPhieuMuonTra nếu cần
        Task<List<Phieumuontra>> GetAllInRangeAsync(DateTime startDate, DateTime endDate); // Lấy theo khoảng ngày

        // --- Các phương thức CRUD ---

        /// <summary>
        /// Thêm mới một phiếu mượn trả vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="phieuMuonTra">Entity Phieumuontra cần thêm.</param>
        /// <returns>Entity Phieumuontra đã được thêm (với ID được cập nhật) nếu thành công, null nếu thất bại.</returns>
        Task<Phieumuontra?> AddAsync(Phieumuontra phieuMuonTra); // <<< Sửa kiểu trả về thành Task<Phieumuontra?>

        /// <summary>
        /// Cập nhật thông tin một phiếu mượn trả hiện có.
        /// </summary>
        /// <param name="phieuMuonTra">Entity Phieumuontra chứa thông tin cập nhật.</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại.</returns>
        Task<bool> UpdateAsync(Phieumuontra phieuMuonTra);

        /// <summary>
        /// Xóa vĩnh viễn một phiếu mượn trả (Cân nhắc kỹ!).
        /// </summary>
        /// <param name="id">ID của phiếu mượn trả cần xóa.</param>
        /// <returns>True nếu xóa thành công, False nếu thất bại.</returns>
        Task<bool> HardDeleteAsync(int id); // Giữ lại HardDelete nếu bạn dùng nó, hoặc thay bằng SoftDelete

        // --- Các phương thức đếm (Ví dụ) ---
        Task<int> CountBorrowedLoansAsync();
        Task<int> CountOverdueLoansAsync(DateTime currentDate);

        // --- Các phương thức truy vấn khác (Ví dụ) ---
        Task<List<Phieumuontra>> GetLoansByDocgiaIdAsync(int idDocGia); // Lấy phiếu đang mượn (chưa trả)
        Task<List<Phieumuontra>> GetOverdueLoansAsync(DateTime currentDate); // Lấy phiếu quá hạn (chưa trả)
        Task<List<Phieumuontra>> GetHistoryByDocGiaIdAsync(int idDocGia); // Lấy toàn bộ lịch sử mượn trả

        // <<< THÊM PHƯƠNG THỨC LẤY LỊCH SỬ PHẠT >>>
        /// <summary>
        /// Lấy danh sách các phiếu mượn trả đã phát sinh tiền phạt của một độc giả cụ thể.
        /// </summary>
        /// <param name="idDocGia">ID của độc giả.</param>
        /// <returns>Danh sách các phiếu mượn trả có SoTienPhat > 0.</returns>
        Task<List<Phieumuontra>> GetFineHistoryByDocGiaIdAsync(int idDocGia);

    }
}