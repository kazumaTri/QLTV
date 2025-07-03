// Đảm bảo namespace này tồn tại trong project của bạn
using DAL.Models; // Namespace chứa Entity Loaidocgia và Docgia
using System; // Cần cho Func<> trong AnyAsync
using System.Collections.Generic; // Cần cho List<T>
using System.Linq.Expressions; // Cần cho Expression<> trong AnyAsync
using System.Threading.Tasks; // Cần cho Task

namespace DAL
{
    /// <summary>
    /// Interface cho lớp truy cập dữ liệu Loại Độc Giả (Data Access Layer).
    /// Định nghĩa các phương thức tương tác với cơ sở dữ liệu liên quan đến Loại Độc Giả.
    /// </summary>
    public interface IDALLoaiDocGia
    {
        /// <summary>
        /// Lấy danh sách tất cả loại độc giả.
        /// </summary>
        /// <returns>Danh sách Entity Loaidocgia.</returns>
        Task<List<Loaidocgia>> GetAllAsync();

        /// <summary>
        /// Lấy thông tin loại độc giả theo ID.
        /// </summary>
        /// <param name="id">ID loại độc giả cần lấy.</param>
        /// <returns>Entity Loaidocgia nếu tìm thấy, null nếu không.</returns>
        Task<Loaidocgia?> GetByIdAsync(int id);

        /// <summary>
        /// Lấy thông tin loại độc giả theo Mã Loại.
        /// </summary>
        /// <param name="maLoaiDocGia">Mã loại độc giả cần tìm.</param>
        /// <returns>Entity Loaidocgia nếu tìm thấy, null nếu không.</returns>
        Task<Loaidocgia?> GetByMaAsync(string maLoaiDocGia);

        /// <summary>
        /// Thêm mới một loại độc giả vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="loaiDocGia">Entity Loaidocgia cần thêm.</param>
        /// <returns>True nếu thêm thành công, False nếu thất bại.</returns>
        Task<bool> AddAsync(Loaidocgia loaiDocGia);

        /// <summary>
        /// Cập nhật thông tin một loại độc giả trong cơ sở dữ liệu.
        /// </summary>
        /// <param name="loaiDocGia">Entity Loaidocgia chứa thông tin cập nhật (phải có Id > 0).</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại.</returns>
        Task<bool> UpdateAsync(Loaidocgia loaiDocGia);

        /// <summary>
        /// Xóa một loại độc giả khỏi cơ sở dữ liệu theo ID.
        /// </summary>
        /// <param name="id">ID của loại độc giả cần xóa.</param>
        /// <returns>True nếu xóa thành công, False nếu thất bại (ví dụ: không tìm thấy hoặc lỗi ràng buộc).</returns>
        Task<bool> DeleteAsync(int id);

        // --- Các phương thức bổ sung hỗ trợ tìm kiếm, sắp xếp, validation, hiển thị số lượng ---

        /// <summary>
        /// Tìm kiếm loại độc giả theo từ khóa (Mã hoặc Tên).
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm.</param>
        /// <returns>Danh sách Entity Loaidocgia khớp.</returns>
        Task<List<Loaidocgia>> SearchAsync(string searchTerm);

        /// <summary>
        /// Lấy danh sách loại độc giả đã được lọc và sắp xếp.
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm (có thể null hoặc rỗng).</param>
        /// <param name="sortColumn">Tên cột cần sắp xếp (khớp với tên thuộc tính Entity).</param>
        /// <param name="ascending">True để sắp xếp tăng dần, False để giảm dần.</param>
        /// <returns>Danh sách Entity Loaidocgia đã lọc và sắp xếp.</returns>
        Task<List<Loaidocgia>> GetFilteredAndSortedAsync(string? searchTerm, string sortColumn, bool ascending);

        /// <summary>
        /// Lấy danh sách loại độc giả đã lọc, sắp xếp và kèm theo số lượng độc giả tương ứng.
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm (có thể null hoặc rỗng).</param>
        /// <param name="sortColumn">Tên cột cần sắp xếp (khớp với tên thuộc tính Entity).</param>
        /// <param name="ascending">True để sắp xếp tăng dần, False để giảm dần.</param>
        /// <returns>Danh sách các Tuple chứa Entity Loaidocgia và số lượng độc giả liên quan.</returns>
        Task<List<(Loaidocgia Entity, int Count)>> GetFilteredAndSortedWithCountAsync(string? searchTerm, string sortColumn, bool ascending);

        /// <summary>
        /// Kiểm tra xem có bất kỳ loại độc giả nào thỏa mãn điều kiện hay không.
        /// </summary>
        /// <param name="predicate">Biểu thức điều kiện.</param>
        /// <returns>True nếu có ít nhất một loại độc giả thỏa mãn, False nếu không.</returns>
        Task<bool> AnyAsync(Expression<Func<Loaidocgia, bool>> predicate);

        /// <summary>
        /// Đếm số lượng độc giả thuộc một loại độc giả cụ thể.
        /// </summary>
        /// <param name="loaiDocGiaId">ID của loại độc giả.</param>
        /// <returns>Số lượng độc giả.</returns>
        Task<int> CountRelatedDocGiaAsync(int loaiDocGiaId);
        Task<string?> GetLastMaLoaiDocGiaAsync(string prefix);

    }
}