// DAL/IDALNhomNguoiDung.cs
using DAL.Models; // Cần để sử dụng lớp Nhomnguoidung
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL
{
    /// <summary>
    /// Interface cho lớp truy cập dữ liệu Nhóm Người Dùng.
    /// Định nghĩa các phương thức tương tác cơ bản với cơ sở dữ liệu.
    /// </summary>
    public interface IDALNhomNguoiDung
    {
        /// <summary>
        /// Lấy tất cả các nhóm người dùng.
        /// </summary>
        /// <returns>Danh sách các Nhomnguoidung.</returns>
        Task<List<Nhomnguoidung>> GetAllAsync();

        /// <summary>
        /// Lấy thông tin nhóm người dùng theo ID.
        /// </summary>
        /// <param name="id">ID của nhóm người dùng.</param>
        /// <returns>Đối tượng Nhomnguoidung nếu tìm thấy, null nếu không.</returns>
        Task<Nhomnguoidung?> GetByIdAsync(int id);

        /// <summary>
        /// Lấy thông tin nhóm người dùng theo Mã Nhóm Người Dùng (nếu cần).
        /// </summary>
        /// <param name="maNhom">Mã nhóm người dùng.</param>
        /// <returns>Đối tượng Nhomnguoidung nếu tìm thấy, null nếu không.</returns>
        Task<Nhomnguoidung?> GetByMaAsync(string maNhom); // Thêm hàm này nếu bạn cần tìm theo mã

        /// <summary>
        /// Thêm một nhóm người dùng mới.
        /// </summary>
        /// <param name="nhomNguoiDung">Đối tượng Nhomnguoidung cần thêm.</param>
        /// <returns>Đối tượng Nhomnguoidung đã được thêm (với ID) nếu thành công, null nếu thất bại.</returns>
        Task<Nhomnguoidung?> AddAsync(Nhomnguoidung nhomNguoiDung);

        /// <summary>
        /// Cập nhật thông tin một nhóm người dùng hiện có.
        /// </summary>
        /// <param name="nhomNguoiDung">Đối tượng Nhomnguoidung chứa thông tin cập nhật.</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại.</returns>
        Task<bool> UpdateAsync(Nhomnguoidung nhomNguoiDung);

        /// <summary>
        /// Xóa một nhóm người dùng (Lưu ý: Cần xem xét logic xóa cứng hay mềm).
        /// </summary>
        /// <param name="id">ID của nhóm người dùng cần xóa.</param>
        /// <returns>True nếu xóa thành công, False nếu thất bại.</returns>
        Task<bool> DeleteAsync(int id); // Cân nhắc đổi tên thành HardDeleteAsync hoặc thêm SoftDeleteAsync nếu cần
        Task<List<int>> GetChucNangIdsByNhomIdAsync(int nhomId);
        Task<bool> UpdatePhanQuyenAsync(int nhomId, List<int> chucNangIds);
        // Có thể thêm các phương thức khác nếu cần, ví dụ:
        // Task<bool> CheckMaExistsAsync(string maNhom);
        // Task<List<Chucnang>> GetChucNangByNhomIdAsync(int nhomId);
    }
}