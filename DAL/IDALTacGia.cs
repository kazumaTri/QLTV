// Project/Namespace: DAL
using DAL.Models; // Cần để định nghĩa các phương thức nhận/trả về Entity Tacgia
using System.Collections.Generic; // Cần cho List
using System.Threading.Tasks; // Cần cho Task

namespace DAL
{
    /// <summary>
    /// Interface định nghĩa các thao tác truy cập dữ liệu cho Tác giả (Tacgia).
    /// Bao gồm các phương thức CRUD, tìm kiếm, kiểm tra sự tồn tại, và các thao tác Soft Delete/Restore.
    /// </summary>
    public interface IDALTacGia
    {
        // --- Các phương thức lấy dữ liệu ---

        /// <summary>
        /// Lấy tất cả tác giả CHƯA bị xóa mềm.
        /// </summary>
        /// <returns>Danh sách các Tacgia chưa bị xóa mềm.</returns>
        Task<List<Tacgia>> GetAllAsync();

        /// <summary>
        /// Lấy TẤT CẢ tác giả, BAO GỒM cả những tác giả đã bị xóa mềm.
        /// </summary>
        /// <returns>Danh sách tất cả các Tacgia.</returns>
        Task<List<Tacgia>> GetAllIncludingDeletedAsync(); // <<< THÊM MỚI

        /// <summary>
        /// Tìm kiếm tác giả theo Mã hoặc Tên (CHƯA bị xóa mềm).
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm.</param>
        /// <returns>Danh sách các Tacgia phù hợp chưa bị xóa mềm.</returns>
        Task<List<Tacgia>> SearchAsync(string searchTerm);

        /// <summary>
        /// Tìm kiếm TẤT CẢ tác giả theo Mã hoặc Tên (BAO GỒM cả đã xóa mềm).
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm.</param>
        /// <returns>Danh sách tất cả các Tacgia phù hợp.</returns>
        Task<List<Tacgia>> SearchIncludingDeletedAsync(string searchTerm); // <<< THÊM MỚI

        /// <summary>
        /// Lấy tác giả theo ID (chỉ trả về nếu CHƯA bị xóa mềm).
        /// </summary>
        /// <param name="id">ID của tác giả.</param>
        /// <returns>Tacgia tìm thấy hoặc null.</returns>
        Task<Tacgia?> GetByIdAsync(int id);

        /// <summary>
        /// Lấy tác giả theo Mã tác giả (Matacgia) (chỉ trả về nếu CHƯA bị xóa mềm).
        /// </summary>
        /// <param name="maTacGia">Mã tác giả cần tìm.</param>
        /// <returns>Tacgia tìm thấy hoặc null.</returns>
        Task<Tacgia?> GetByMaAsync(string maTacGia);

        /// <summary>
        /// Lấy tác giả theo Tên tác giả (TenTacGia) (chỉ trả về nếu CHƯA bị xóa mềm).
        /// </summary>
        /// <param name="tenTacGia">Tên tác giả cần tìm.</param>
        /// <returns>Tacgia tìm thấy hoặc null.</returns>
        Task<Tacgia?> GetByTenAsync(string tenTacGia);


        // --- Các phương thức kiểm tra sự tồn tại (chỉ kiểm tra trong các bản ghi CHƯA bị xóa mềm) ---

        /// <summary>
        /// Kiểm tra xem Mã tác giả đã tồn tại hay chưa (trong các bản ghi chưa bị xóa mềm).
        /// </summary>
        /// <param name="maTacGia">Mã tác giả cần kiểm tra.</param>
        /// <returns>True nếu tồn tại, False nếu không.</returns>
        Task<bool> IsMaTacGiaExistsAsync(string maTacGia);

        /// <summary>
        /// Kiểm tra xem Mã tác giả đã tồn tại hay chưa, loại trừ một ID cụ thể (trong các bản ghi chưa bị xóa mềm).
        /// </summary>
        /// <param name="maTacGia">Mã tác giả cần kiểm tra.</param>
        /// <param name="excludeId">ID của tác giả cần loại trừ.</param>
        /// <returns>True nếu tồn tại (ở bản ghi khác), False nếu không.</returns>
        Task<bool> IsMaTacGiaExistsExcludingIdAsync(string maTacGia, int excludeId);

        /// <summary>
        /// Kiểm tra xem Tên tác giả đã tồn tại hay chưa (trong các bản ghi chưa bị xóa mềm).
        /// </summary>
        /// <param name="tenTacGia">Tên tác giả cần kiểm tra.</param>
        /// <returns>True nếu tồn tại, False nếu không.</returns>
        Task<bool> IsTenTacGiaExistsAsync(string tenTacGia);

        /// <summary>
        /// Kiểm tra xem Tên tác giả đã tồn tại hay chưa, loại trừ một ID cụ thể (trong các bản ghi chưa bị xóa mềm).
        /// </summary>
        /// <param name="tenTacGia">Tên tác giả cần kiểm tra.</param>
        /// <param name="excludeId">ID của tác giả cần loại trừ.</param>
        /// <returns>True nếu tồn tại (ở bản ghi khác), False nếu không.</returns>
        Task<bool> IsTenTacGiaExistsExcludingIdAsync(string tenTacGia, int excludeId);


        // --- Các phương thức thêm/sửa/xóa ---

        /// <summary>
        /// Thêm mới một tác giả vào cơ sở dữ liệu (mặc định DaAn = false).
        /// </summary>
        /// <param name="tacGia">Đối tượng Tacgia chứa thông tin cần thêm.</param>
        /// <returns>Đối tượng Tacgia đã được thêm hoặc null nếu thất bại.</returns>
        Task<Tacgia?> AddAsync(Tacgia tacGia);

        /// <summary>
        /// Cập nhật thông tin của một tác giả hiện có (chỉ cập nhật nếu chưa bị xóa mềm).
        /// </summary>
        /// <param name="tacGia">Đối tượng Tacgia chứa thông tin cập nhật.</param>
        /// <returns>True nếu cập nhật thành công, False nếu không.</returns>
        Task<bool> UpdateAsync(Tacgia tacGia);

        /// <summary>
        /// Thực hiện xóa mềm tác giả (đánh dấu DaAn = true).
        /// </summary>
        /// <param name="id">ID của tác giả cần xóa mềm.</param>
        /// <returns>True nếu thành công, False nếu không.</returns>
        Task<bool> SoftDeleteAsync(int id); // <<< Đã có

        /// <summary>
        /// Khôi phục tác giả đã bị xóa mềm (đánh dấu DaAn = false).
        /// </summary>
        /// <param name="id">ID của tác giả cần khôi phục.</param>
        /// <returns>True nếu thành công, False nếu không.</returns>
        Task<bool> RestoreAsync(int id); // <<< THÊM MỚI

        /// <summary>
        /// Xóa vĩnh viễn một tác giả khỏi cơ sở dữ liệu. Sử dụng cẩn thận!
        /// </summary>
        /// <param name="id">ID của tác giả cần xóa vĩnh viễn.</param>
        /// <returns>True nếu xóa thành công, False nếu không.</returns>
        Task<bool> HardDeleteAsync(int id); // <<< Giữ lại (hoặc xóa nếu không cần)
    }
}
