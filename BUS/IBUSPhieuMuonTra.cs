// BUS/IBUSPhieuMuonTra.cs
using DTO; // Cần cho PhieuMuonTraDTO
using System.Collections.Generic; // Cần cho List<T>
using System.Threading.Tasks; // Cần cho Task
using System; // Cần cho DateTime

namespace BUS
{
    /// <summary>
    /// Interface cho Business Logic Layer Phiếu Mượn Trả.
    /// Định nghĩa các phương thức nghiệp vụ liên quan đến Phiếu Mượn Trả.
    /// Đã thêm phương thức lấy lịch sử phạt.
    /// </summary>
    public interface IBUSPhieuMuonTra
    {
        /// <summary>
        /// Lấy tất cả các phiếu mượn trả (có thể cần bộ lọc hoặc phân trang trong thực tế).
        /// </summary>
        /// <returns>Danh sách PhieuMuonTraDTO.</returns>
        Task<List<PhieuMuonTraDTO>> GetAllPhieuMuonTraAsync();

        /// <summary>
        /// Lấy thông tin chi tiết một phiếu mượn trả theo ID (Số phiếu).
        /// </summary>
        /// <param name="soPhieuMuonTra">ID của phiếu mượn trả.</param>
        /// <returns>PhieuMuonTraDTO nếu tìm thấy, null nếu không.</returns>
        Task<PhieuMuonTraDTO?> GetPhieuMuonTraByIdAsync(int soPhieuMuonTra);

        /// <summary>
        /// Lấy danh sách các phiếu mượn đang hoạt động (chưa trả) của một độc giả cụ thể.
        /// </summary>
        /// <param name="idDocGia">ID của độc giả.</param>
        /// <returns>Danh sách PhieuMuonTraDTO đang mượn của độc giả.</returns>
        Task<List<PhieuMuonTraDTO>> GetLoansByDocgiaIdAsync(int idDocGia);

        /// <summary>
        /// Lấy danh sách các phiếu mượn đã quá hạn trả.
        /// </summary>
        /// <returns>Danh sách PhieuMuonTraDTO quá hạn.</returns>
        Task<List<PhieuMuonTraDTO>> GetOverdueLoansAsync();

        /// <summary>
        /// Tạo một phiếu mượn mới.
        /// </summary>
        /// <param name="newPhieuMuonTraDto">DTO chứa thông tin phiếu mượn mới.</param>
        /// <returns>PhieuMuonTraDTO của phiếu vừa tạo nếu thành công, null nếu thất bại.</returns>
        /// <exception cref="ArgumentException">Dữ liệu đầu vào không hợp lệ.</exception>
        /// <exception cref="InvalidOperationException">Vi phạm quy tắc nghiệp vụ (vd: độc giả nợ quá hạn, sách không cho mượn...).</exception>
        Task<PhieuMuonTraDTO?> AddPhieuMuonTraAsync(PhieuMuonTraDTO newPhieuMuonTraDto);

        /// <summary>
        /// Xử lý việc trả sách cho một phiếu mượn.
        /// Logic cụ thể (cập nhật trạng thái, tính phạt, cập nhật ngày trả...) cần được cài đặt trong lớp BUS
        /// dựa trên cấu trúc dữ liệu thực tế.
        /// </summary>
        /// <param name="soPhieuMuonTra">ID của phiếu mượn cần xử lý trả.</param>
        /// <returns>True nếu xử lý trả thành công, False nếu thất bại.</returns>
        /// <exception cref="KeyNotFoundException">Không tìm thấy phiếu mượn.</exception>
        /// <exception cref="InvalidOperationException">Phiếu đã được trả hoặc có lỗi nghiệp vụ khác.</exception>
        Task<bool> ProcessReturnAsync(int soPhieuMuonTra);

        /// <summary>
        /// Xóa vĩnh viễn một phiếu mượn trả (Cân nhắc: Thường không nên xóa hẳn mà nên lưu trữ lịch sử).
        /// </summary>
        /// <param name="soPhieuMuonTra">ID của phiếu mượn trả cần xóa.</param>
        /// <returns>True nếu xóa thành công, False nếu thất bại.</returns>
        /// <exception cref="InvalidOperationException">Không thể xóa do ràng buộc nghiệp vụ.</exception>
        Task<bool> HardDeletePhieuMuonTraAsync(int soPhieuMuonTra); // Xem xét việc dùng Soft Delete thay thế

        /// <summary>
        /// Đếm tổng số lượt sách đang được mượn (chưa trả).
        /// </summary>
        /// <returns>Số lượng sách đang mượn.</returns>
        Task<int> GetBorrowedCountAsync();

        /// <summary>
        /// Đếm số lượt sách mượn bị quá hạn.
        /// </summary>
        /// <returns>Số lượng sách mượn quá hạn.</returns>
        Task<int> GetOverdueCountAsync();

        /// <summary>
        /// Lấy toàn bộ lịch sử mượn/trả của một độc giả.
        /// </summary>
        /// <param name="idDocGia">ID của độc giả.</param>
        /// <returns>Danh sách PhieuMuonTraDTO trong lịch sử của độc giả.</returns>
        Task<List<PhieuMuonTraDTO>> GetHistoryByDocGiaIdAsync(int idDocGia);

        // <<< THÊM PHƯƠNG THỨC LẤY LỊCH SỬ PHẠT >>>
        /// <summary>
        /// Lấy lịch sử các phiếu mượn trả đã phát sinh tiền phạt của một độc giả.
        /// </summary>
        /// <param name="idDocGia">ID của độc giả.</param>
        /// <returns>Danh sách PhieuMuonTraDTO có phát sinh phạt.</returns>
        Task<List<PhieuMuonTraDTO>> GetFineHistoryByDocGiaIdAsync(int idDocGia);

    }
}