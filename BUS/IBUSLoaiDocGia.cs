// Đảm bảo namespace DTO tồn tại và project BUS có tham chiếu đến project DTO
using DTO; // Namespace chứa LoaiDocGiaDTO
using System; // Cần cho các Exception (ArgumentException, InvalidOperationException) nếu khai báo ném lỗi
using System.Collections.Generic; // Cần cho List<T>
using System.Threading.Tasks; // Cần cho Task

namespace BUS
{
    /// <summary>
    /// Interface cho Business Logic Layer Loại Độc Giả.
    /// Định nghĩa các phương thức nghiệp vụ liên quan đến Loại Độc Giả.
    /// </summary>
    public interface IBUSLoaiDocGia
    {
        // --- Phương thức nghiệp vụ CRUD cơ bản ---

        /// <summary>
        /// Lấy danh sách tất cả Loại độc giả DTO.
        /// </summary>
        /// <returns>Danh sách LoaiDocGiaDTO.</returns>
        Task<List<LoaiDocGiaDTO>> GetAllLoaiDocGiaAsync();

        /// <summary>
        /// Lấy Loại độc giả DTO theo ID.
        /// </summary>
        /// <param name="id">ID loại độc giả cần lấy.</param>
        /// <returns>LoaiDocGiaDTO nếu tìm thấy, null nếu không.</returns>
        Task<LoaiDocGiaDTO?> GetLoaiDocGiaByIdAsync(int id);

        /// <summary>
        /// Lấy Loại độc giả DTO theo Mã Loại.
        /// </summary>
        /// <param name="ma">Mã loại độc giả cần lấy.</param>
        /// <returns>LoaiDocGiaDTO nếu tìm thấy, null nếu không.</returns>
        Task<LoaiDocGiaDTO?> GetLoaiDocGiaByMaAsync(string ma);

        // ****** SỬA KIỂU TRẢ VỀ VÀ MÔ TẢ ******
        /// <summary>
        /// Thêm mới một Loại độc giả. Bao gồm validation nghiệp vụ và tự động sinh mã.
        /// </summary>
        /// <param name="loaiDocGiaDto">DTO chứa thông tin loại độc giả cần thêm (MaLoaiDocGia có thể null).</param>
        /// <returns>DTO của loại độc giả đã được thêm (bao gồm ID và Mã được sinh tự động) nếu thành công, null nếu thất bại.</returns>
        /// <exception cref="ArgumentNullException">Ném khi loaiDocGiaDto là null.</exception>
        /// <exception cref="ArgumentException">Ném khi dữ liệu DTO không hợp lệ (ví dụ: tên rỗng).</exception>
        /// <exception cref="InvalidOperationException">Ném khi vi phạm logic nghiệp vụ (ví dụ: trùng mã, trùng tên).</exception>
        Task<LoaiDocGiaDTO?> AddLoaiDocGiaAsync(LoaiDocGiaDTO loaiDocGiaDto); // <<< ĐÃ SỬA

        // ****** SỬA KIỂU TRẢ VỀ VÀ MÔ TẢ ******
        /// <summary>
        /// Cập nhật thông tin một Loại độc giả. Bao gồm validation nghiệp vụ.
        /// </summary>
        /// <param name="loaiDocGiaDto">DTO chứa thông tin loại độc giả cần cập nhật (phải có Id > 0).</param>
        /// <returns>DTO của loại độc giả đã được cập nhật nếu thành công, null nếu thất bại.</returns>
        /// <exception cref="ArgumentNullException">Ném khi loaiDocGiaDto là null.</exception>
        /// <exception cref="ArgumentException">Ném khi ID hoặc dữ liệu DTO không hợp lệ.</exception>
        /// <exception cref="KeyNotFoundException">Ném khi không tìm thấy loại độc giả với ID cung cấp.</exception>
        /// <exception cref="InvalidOperationException">Ném khi vi phạm logic nghiệp vụ (ví dụ: trùng tên với loại khác).</exception>
        Task<LoaiDocGiaDTO?> UpdateLoaiDocGiaAsync(LoaiDocGiaDTO loaiDocGiaDto); // <<< ĐÃ SỬA

        /// <summary>
        /// Xóa một Loại độc giả theo ID. Bao gồm kiểm tra ràng buộc nghiệp vụ.
        /// </summary>
        /// <param name="id">ID của loại độc giả cần xóa.</param>
        /// <returns>True nếu xóa thành công, False nếu thất bại.</returns>
        /// <exception cref="ArgumentException">Ném khi ID không hợp lệ.</exception>
        /// <exception cref="InvalidOperationException">Ném khi không thể xóa do ràng buộc (ví dụ: còn độc giả thuộc loại này).</exception>
        Task<bool> DeleteLoaiDocGiaAsync(int id); // Giữ nguyên Task<bool> cho Delete


        // --- Phương thức hỗ trợ tìm kiếm, sắp xếp, validation, hiển thị số lượng ---
        // (Giữ nguyên các phương thức này)
        Task<List<LoaiDocGiaDTO>> SearchLoaiDocGiaAsync(string searchTerm);
        Task<List<LoaiDocGiaDTO>> GetLoaiDocGiaFilteredAndSortedAsync(string? searchTerm, string sortColumn, bool ascending);
        Task<bool> CheckMaLoaiExistsAsync(string maLoai, int currentId = 0);
        Task<bool> CheckTenLoaiExistsAsync(string tenLoai, int currentId = 0);
        Task<int> CountDocGiaByLoaiAsync(int loaiDocGiaId);
    }
}