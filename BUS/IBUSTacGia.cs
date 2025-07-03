// Project/Namespace: BUS
using DTO; // Cần để định nghĩa các phương thức nhận/trả về TacGiaDTO
using System.Collections.Generic; // Cần cho List
using System.Threading.Tasks; // Cần cho Task

namespace BUS
{
    /// <summary>
    /// Interface cho Business Logic Layer của Tác giả.
    /// Đã cập nhật để hỗ trợ Soft Delete, Restore và tùy chọn bao gồm dữ liệu đã xóa.
    /// </summary>
    public interface IBUSTacGia
    {
        /// <summary>
        /// Lấy danh sách tác giả dưới dạng DTO.
        /// </summary>
        /// <param name="includeDeleted">True để bao gồm cả tác giả đã bị xóa mềm, False để chỉ lấy tác giả chưa bị xóa.</param>
        /// <returns>Danh sách TacGiaDTO.</returns>
        Task<List<TacGiaDTO>> GetAllTacGiaAsync(bool includeDeleted = false); // <<< SỬA ĐỔI

        /// <summary>
        /// Tìm kiếm tác giả theo Mã hoặc Tên dưới dạng DTO.
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm.</param>
        /// <param name="includeDeleted">True để tìm kiếm trong cả tác giả đã bị xóa mềm, False để chỉ tìm trong tác giả chưa bị xóa.</param>
        /// <returns>Danh sách TacGiaDTO phù hợp.</returns>
        Task<List<TacGiaDTO>> SearchTacGiaAsync(string searchTerm, bool includeDeleted = false); // <<< SỬA ĐỔI

        /// <summary>
        /// Lấy tác giả theo ID dưới dạng DTO (chỉ trả về nếu chưa bị xóa mềm).
        /// </summary>
        /// <param name="id">ID của tác giả.</param>
        /// <returns>TacGiaDTO tìm thấy hoặc null.</returns>
        Task<TacGiaDTO?> GetTacGiaByIdAsync(int id);

        /// <summary>
        /// Lấy tác giả theo Mã dưới dạng DTO (chỉ trả về nếu chưa bị xóa mềm).
        /// </summary>
        /// <param name="maTacGia">Mã tác giả.</param>
        /// <returns>TacGiaDTO tìm thấy hoặc null.</returns>
        Task<TacGiaDTO?> GetTacGiaByMaAsync(string maTacGia);

        /// <summary>
        /// Lấy tác giả theo Tên dưới dạng DTO (chỉ trả về nếu chưa bị xóa mềm).
        /// </summary>
        /// <param name="tenTacGia">Tên tác giả.</param>
        /// <returns>TacGiaDTO tìm thấy hoặc null.</returns>
        Task<TacGiaDTO?> GetTacGiaByTenAsync(string tenTacGia);

        /// <summary>
        /// Thêm mới tác giả từ DTO.
        /// </summary>
        /// <param name="tacGiaDto">DTO chứa thông tin tác giả mới.</param>
        /// <returns>DTO của tác giả đã được thêm (có ID) hoặc null nếu thất bại.</returns>
        Task<TacGiaDTO?> AddTacGiaAsync(TacGiaDTO tacGiaDto);

        /// <summary>
        /// Cập nhật thông tin tác giả từ DTO (chỉ cập nhật nếu chưa bị xóa mềm).
        /// </summary>
        /// <param name="tacGiaDto">DTO chứa thông tin cập nhật.</param>
        /// <returns>True nếu cập nhật thành công, False nếu không.</returns>
        Task<bool> UpdateTacGiaAsync(TacGiaDTO tacGiaDto);

        /// <summary>
        /// Thực hiện xóa mềm tác giả theo ID (đánh dấu DaAn = true).
        /// </summary>
        /// <param name="id">ID của tác giả cần xóa mềm.</param>
        /// <returns>True nếu thành công, False nếu không.</returns>
        Task<bool> SoftDeleteTacGiaAsync(int id); // <<< THÊM MỚI/XÁC NHẬN

        /// <summary>
        /// Khôi phục tác giả đã bị xóa mềm theo ID (đánh dấu DaAn = false).
        /// </summary>
        /// <param name="id">ID của tác giả cần khôi phục.</param>
        /// <returns>True nếu thành công, False nếu không.</returns>
        Task<bool> RestoreTacGiaAsync(int id); // <<< THÊM MỚI

        /// <summary>
        /// Xóa vĩnh viễn tác giả theo ID (Hard Delete). Sử dụng cẩn thận!
        /// </summary>
        /// <param name="id">ID của tác giả cần xóa vĩnh viễn.</param>
        /// <returns>True nếu xóa thành công, False nếu không.</returns>
        Task<bool> HardDeleteTacGiaAsync(int id); // <<< ĐỔI TÊN từ DeleteTacGiaAsync
    }
}
