// Project/Namespace: BUS

// Đảm bảo namespace DTO tồn tại và project BUS có tham chiếu đến project DTO
using DTO;
using System; // Cần cho Task, List<>
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BUS
{
    /// <summary>
    /// Interface cho Business Logic Layer Độc Giả.
    /// Định nghĩa các phương thức nghiệp vụ liên quan đến Độc Giả.
    /// Đã cập nhật để hỗ trợ tìm kiếm, xóa mềm, khôi phục và lọc theo loại độc giả.
    /// </summary>
    public interface IBUSDocGia
    {
        // --- Phương thức để lấy dữ liệu (Queries) ---

        /// <summary>
        /// Lấy danh sách tất cả độc giả, có tùy chọn lọc theo loại độc giả.
        /// </summary>
        /// <param name="includeDeleted">True để bao gồm cả độc giả đã bị xóa mềm.</param>
        /// <param name="idLoaiDocGiaFilter">ID của loại độc giả cần lọc (null hoặc 0 để không lọc).</param>
        /// <returns>Danh sách DocgiaDTO.</returns>
        Task<List<DocgiaDTO>> GetAllDocGiaAsync(bool includeDeleted = false, int? idLoaiDocGiaFilter = null); // <<< ĐÃ THÊM idLoaiDocGiaFilter

        /// <summary>
        /// Lấy thông tin độc giả theo ID.
        /// </summary>
        /// <param name="id">ID độc giả cần lấy.</param>
        /// <param name="includeDeleted">True để lấy cả độc giả đã bị xóa mềm nếu khớp ID.</param>
        /// <returns>DocgiaDTO nếu tìm thấy, null nếu không.</returns>
        Task<DocgiaDTO?> GetDocGiaByIdAsync(int id, bool includeDeleted = false);

        /// <summary>
        /// Lấy thông tin độc giả theo ID Người Dùng liên quan.
        /// </summary>
        /// <param name="idNguoiDung">ID người dùng liên quan.</param>
        /// <param name="includeDeleted">True để lấy cả độc giả đã bị xóa mềm nếu khớp ID người dùng.</param>
        /// <returns>DocgiaDTO nếu tìm thấy, null nếu không.</returns>
        Task<DocgiaDTO?> GetDocGiaByIdNguoiDungAsync(int idNguoiDung, bool includeDeleted = false);

        /// <summary>
        /// Đếm số lượng độc giả đang hoạt động (chưa bị xóa mềm và còn hạn thẻ).
        /// </summary>
        Task<int> GetActiveReaderCountAsync();

        /// <summary>
        /// Tìm kiếm độc giả theo từ khóa (tên, mã, email...), có tùy chọn lọc theo loại độc giả.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm.</param>
        /// <param name="includeDeleted">True để bao gồm cả độc giả đã bị xóa mềm trong kết quả tìm kiếm.</param>
        /// <param name="idLoaiDocGiaFilter">ID của loại độc giả cần lọc (null hoặc 0 để không lọc).</param>
        /// <returns>Danh sách DocgiaDTO khớp với từ khóa và bộ lọc.</returns>
        Task<List<DocgiaDTO>> SearchDocGiaAsync(string keyword, bool includeDeleted = false, int? idLoaiDocGiaFilter = null); // <<< ĐÃ THÊM idLoaiDocGiaFilter


        // --- Phương thức thay đổi dữ liệu (Commands) ---

        /// <summary>
        /// Thêm mới một độc giả (và tài khoản người dùng liên quan).
        /// </summary>
        /// <param name="newDocGiaDTO">DTO chứa thông tin độc giả và tài khoản mới.</param>
        /// <returns>DocgiaDTO của độc giả vừa thêm nếu thành công, null nếu thất bại.</returns>
        Task<DocgiaDTO?> AddDocGiaAsync(DocgiaDTO newDocGiaDTO);

        /// <summary>
        /// Cập nhật thông tin của một độc giả hiện có (và tài khoản người dùng liên quan).
        /// </summary>
        /// <param name="updatedDocGiaDTO">DTO chứa thông tin cập nhật.</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại.</returns>
        Task<bool> UpdateDocGiaAsync(DocgiaDTO updatedDocGiaDTO);

        // --- Xóa mềm và Khôi phục ---

        // Task<bool> HardDeleteDocGiaAsync(int id); // <<< Bỏ hoặc comment phương thức xóa cứng

        /// <summary>
        /// Xóa mềm (đánh dấu ẩn) một độc giả và tài khoản người dùng liên quan.
        /// Thực hiện kiểm tra ràng buộc (ví dụ: phiếu mượn đang hoạt động) trước khi xóa.
        /// </summary>
        /// <param name="id">ID của độc giả cần xóa mềm.</param>
        /// <returns>True nếu xóa mềm thành công, False nếu thất bại.</returns>
        /// <exception cref="InvalidOperationException">Ném ra nếu không thể xóa do vi phạm ràng buộc.</exception>
        Task<bool> SoftDeleteDocGiaAsync(int id);

        /// <summary>
        /// Khôi phục một độc giả đã bị xóa mềm (và tài khoản người dùng liên quan).
        /// </summary>
        /// <param name="id">ID của độc giả cần khôi phục.</param>
        /// <returns>True nếu khôi phục thành công, False nếu thất bại.</returns>
        Task<bool> RestoreDocGiaAsync(int id);

        // --- Các phương thức nghiệp vụ bổ sung (Thêm nếu cần) ---

        /// <summary>
        /// Gia hạn thẻ cho độc giả theo quy định.
        /// </summary>
        /// <param name="idDocGia">ID của độc giả cần gia hạn thẻ.</param>
        /// <returns>True nếu gia hạn thành công.</returns>
        Task<bool> GiaHanTheDocGiaAsync(int idDocGia); // <<< Thêm cho chức năng Gia hạn thẻ

        /// <summary>
        /// Lập phiếu thu tiền phạt cho độc giả.
        /// </summary>
        /// <param name="phieuThuPhatDTO">Thông tin phiếu thu phạt.</param>
        /// <returns>True nếu lập phiếu và cập nhật nợ thành công.</returns>
        Task<bool> LapPhieuThuPhatAsync(PhieuThuPhatDTO phieuThuPhatDTO); // <<< Thêm cho chức năng Thu Phạt (Cần tạo PhieuThuPhatDTO)

        /// <summary>
        /// Đặt lại mật khẩu cho tài khoản người dùng của độc giả.
        /// </summary>
        /// <param name="idNguoiDung">ID người dùng của độc giả.</param>
        /// <returns>Mật khẩu mới (plain text) nếu reset thành công, null nếu thất bại.</returns>
        Task<string?> ResetPasswordAsync(int idNguoiDung); // <<< Thêm cho chức năng Reset Mật khẩu
    }
}