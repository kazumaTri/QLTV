// File: DAL/IDALDocGia.cs
// Đảm bảo các namespace này tồn tại trong project của bạn
using DAL.Models; // Namespace chứa các lớp Entity Model (Docgia, Phieumuontra, etc.)
using System; // Cần cho Func trong Expression
using System.Collections.Generic; // Cần cho List<T>
using System.Linq.Expressions; // Cần cho Expression
using System.Threading.Tasks; // Cần cho Task

namespace DAL
{
    /// <summary>
    /// Interface cho lớp truy cập dữ liệu Độc Giả (Data Access Layer).
    /// Định nghĩa các phương thức tương tác với cơ sở dữ liệu liên quan đến Độc Giả.
    /// Đã cập nhật để hỗ trợ tìm kiếm, xóa mềm, khôi phục, kiểm tra ràng buộc và lọc theo loại độc giả.
    /// </summary>
    public interface IDALDocGia
    {
        // --- Phương thức lấy dữ liệu (Read Operations) ---

        /// <summary>
        /// Lấy danh sách tất cả độc giả, có tùy chọn lọc theo loại độc giả.
        /// </summary>
        /// <param name="includeDeleted">True để bao gồm cả độc giả đã bị xóa mềm.</param>
        /// <param name="idLoaiDocGiaFilter">ID của loại độc giả cần lọc (null hoặc 0 để không lọc).</param>
        /// <returns>Danh sách Entity Docgia.</returns>
        Task<List<Docgia>> GetAllAsync(bool includeDeleted = false, int? idLoaiDocGiaFilter = null); // <<< ĐÃ CẬP NHẬT CHỮ KÝ

        /// <summary>
        /// Lấy thông tin độc giả theo ID.
        /// </summary>
        /// <param name="id">ID độc giả cần lấy.</param>
        /// <param name="includeDeleted">True để lấy cả độc giả đã bị xóa mềm nếu khớp ID.</param>
        /// <returns>Entity Docgia nếu tìm thấy, null nếu không.</returns>
        Task<Docgia?> GetByIdAsync(int id, bool includeDeleted = false);

        /// <summary>
        /// Lấy thông tin độc giả theo ID Người Dùng liên quan.
        /// </summary>
        /// <param name="idNguoiDung">ID người dùng liên quan.</param>
        /// <param name="includeDeleted">True để lấy cả độc giả đã bị xóa mềm nếu khớp ID người dùng.</param>
        /// <returns>Entity Docgia nếu tìm thấy, null nếu không.</returns>
        Task<Docgia?> GetByIdNguoiDungAsync(int idNguoiDung, bool includeDeleted = false);

        /// <summary>
        /// Đếm số lượng độc giả đang hoạt động (chưa bị xóa mềm và còn hạn thẻ).
        /// Cần định nghĩa rõ "còn hạn thẻ" trong implementation.
        /// </summary>
        /// <returns>Số lượng độc giả hoạt động.</returns>
        Task<int> GetActiveReaderCountAsync();

        /// <summary>
        /// Tìm kiếm độc giả theo từ khóa, có tùy chọn lọc theo loại độc giả.
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (tên, mã, email...).</param>
        /// <param name="includeDeleted">True để bao gồm cả độc giả đã bị xóa mềm trong kết quả.</param>
        /// <param name="idLoaiDocGiaFilter">ID của loại độc giả cần lọc (null hoặc 0 để không lọc).</param>
        /// <returns>Danh sách Entity Docgia khớp với từ khóa và bộ lọc.</returns>
        Task<List<Docgia>> SearchAsync(string keyword, bool includeDeleted = false, int? idLoaiDocGiaFilter = null); // <<< ĐÃ CẬP NHẬT CHỮ KÝ

        // --- Phương thức kiểm tra ràng buộc ---

        /// <summary>
        /// Kiểm tra xem độc giả có phiếu mượn nào đang hoạt động (chưa trả) hay không.
        /// </summary>
        /// <param name="docGiaId">ID của độc giả cần kiểm tra.</param>
        /// <returns>True nếu có phiếu mượn đang hoạt động, False nếu không.</returns>
        Task<bool> HasActiveLoansAsync(int docGiaId);


        // --- Phương thức thay đổi dữ liệu (Create, Update, Soft Delete, Restore) ---

        /// <summary>
        /// Thêm mới một độc giả vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="docGia">Entity Docgia cần thêm.</param>
        /// <returns>Entity Docgia đã được thêm (với ID được cập nhật) nếu thành công, null nếu thất bại.</returns>
        Task<Docgia?> AddAsync(Docgia docGia);

        /// <summary>
        /// Cập nhật thông tin một độc giả trong cơ sở dữ liệu.
        /// </summary>
        /// <param name="docGia">Entity Docgia chứa thông tin cập nhật (phải có Id > 0).</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại.</returns>
        Task<bool> UpdateAsync(Docgia docGia);

        /// <summary>
        /// Xóa mềm (đánh dấu DaAn = true) một độc giả trong cơ sở dữ liệu.
        /// </summary>
        /// <param name="id">ID của độc giả cần xóa mềm.</param>
        /// <returns>True nếu xóa mềm thành công, False nếu không tìm thấy hoặc đã bị xóa.</returns>
        Task<bool> SoftDeleteAsync(int id);

        /// <summary>
        /// Khôi phục (đánh dấu DaAn = false) một độc giả đã bị xóa mềm.
        /// </summary>
        /// <param name="id">ID của độc giả cần khôi phục.</param>
        /// <returns>True nếu khôi phục thành công, False nếu không tìm thấy hoặc chưa bị xóa.</returns>
        Task<bool> RestoreAsync(int id);

        /// <summary>
        /// Kiểm tra xem tên đăng nhập đã tồn tại hay chưa (cho người dùng).
        /// </summary>
        Task<bool> IsUsernameExistsAsync(string username);

        /// <summary>
        /// Kiểm tra xem email đã tồn tại hay chưa, có thể loại trừ một ID độc giả cụ thể.
        /// </summary>
        Task<bool> IsEmailExistsAsync(string email, int? excludeDocGiaId = null);

        /// <summary>
        /// Cập nhật tổng nợ hiện tại cho độc giả.
        /// </summary>
        Task<bool> UpdateTongNoAsync(int idDocGia, int newTongNo); // Kiểu dữ liệu int khớp với Entity

        /// <summary>
        /// Cập nhật ngày hết hạn thẻ cho độc giả.
        /// </summary>
        Task<bool> UpdateNgayHetHanAsync(int idDocGia, DateTime ngayHetHanMoi);

        // Task<bool> AnyAsync(Expression<Func<Docgia, bool>> predicate); // Cân nhắc thêm nếu dùng chung
    }
}