// File: DAL/IDALNguoiDung.cs
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL
{
    /// <summary>
    /// Interface cho lớp truy cập dữ liệu Người Dùng.
    /// Định nghĩa các phương thức tương tác với cơ sở dữ liệu liên quan đến Người Dùng.
    /// Đã cập nhật để hỗ trợ xóa mềm/khôi phục.
    /// </summary>
    public interface IDALNguoiDung
    {
        // --- Phương thức lấy dữ liệu (Read Operations) ---

        /// <summary>
        /// Lấy danh sách tất cả người dùng.
        /// </summary>
        /// <param name="includeDeleted">True để bao gồm cả người dùng đã bị xóa mềm.</param>
        /// <returns>Danh sách Entity Nguoidung.</returns>
        Task<List<Nguoidung>> GetAllAsync(bool includeDeleted = false); // <<< ĐÃ THÊM includeDeleted

        /// <summary>
        /// Lấy thông tin người dùng theo ID.
        /// </summary>
        /// <param name="id">ID người dùng cần lấy.</param>
        /// <param name="includeDeleted">True để lấy cả người dùng đã bị xóa mềm nếu khớp ID.</param>
        /// <returns>Entity Nguoidung nếu tìm thấy, null nếu không.</returns>
        Task<Nguoidung?> GetByIdAsync(int id, bool includeDeleted = false); // <<< ĐÃ THÊM includeDeleted

        /// <summary>
        /// Lấy thông tin người dùng theo Tên đăng nhập (chỉ người dùng chưa bị xóa).
        /// </summary>
        /// <param name="username">Tên đăng nhập.</param>
        /// <returns>Entity Nguoidung nếu tìm thấy và chưa bị xóa, null nếu không.</returns>
        Task<Nguoidung?> GetByUsernameAsync(string username);

        /// <summary>
        /// Lấy thông tin người dùng theo Tên đăng nhập (bao gồm cả người dùng đã bị xóa).
        /// Hữu ích để kiểm tra tên đăng nhập đã tồn tại hay chưa khi thêm mới.
        /// </summary>
        /// <param name="username">Tên đăng nhập.</param>
        /// <returns>Entity Nguoidung nếu tìm thấy (bất kể trạng thái xóa), null nếu không.</returns>
        Task<Nguoidung?> GetByUsernameIncludingDeletedAsync(string username);

        /// <summary>
        /// Kiểm tra thông tin đăng nhập (username và mật khẩu đã hash).
        /// Chỉ trả về người dùng nếu khớp và chưa bị xóa mềm.
        /// </summary>
        /// <param name="username">Tên đăng nhập.</param>
        /// <param name="passwordHash">Mật khẩu đã được hash.</param>
        /// <returns>Entity Nguoidung nếu đăng nhập hợp lệ, null nếu không.</returns>
        Task<Nguoidung?> CheckLoginAsync(string username, string passwordHash); // Đổi tên từ password thành passwordHash cho rõ ràng

        // --- Phương thức thay đổi dữ liệu (Create, Update, Delete, Restore) ---

        /// <summary>
        /// Thêm mới một người dùng.
        /// </summary>
        /// <param name="nguoidung">Entity Nguoidung cần thêm.</param>
        /// <returns>Entity Nguoidung đã thêm (với ID) nếu thành công, null nếu thất bại.</returns>
        Task<Nguoidung?> AddAsync(Nguoidung nguoidung);

        /// <summary>
        /// Cập nhật thông tin người dùng.
        /// </summary>
        /// <param name="nguoidung">Entity Nguoidung chứa thông tin cập nhật.</param>
        /// <returns>True nếu cập nhật thành công.</returns>
        Task<bool> UpdateAsync(Nguoidung nguoidung);

        /// <summary>
        /// Cập nhật mật khẩu cho người dùng.
        /// </summary>
        /// <param name="idNguoiDung">ID của người dùng cần cập nhật mật khẩu.</param>
        /// <param name="hashedPassword">Mật khẩu mới đã được hash.</param>
        /// <returns>True nếu cập nhật thành công.</returns>
        Task<bool> UpdatePasswordAsync(int idNguoiDung, string hashedPassword); // <<< THÊM PHƯƠNG THỨC NÀY

        // Task<bool> HardDeleteAsync(int id); // Bỏ hoặc comment

        /// <summary>
        /// Xóa mềm người dùng.
        /// </summary>
        /// <param name="id">ID người dùng cần xóa mềm.</param>
        /// <returns>True nếu xóa mềm thành công.</returns>
        Task<bool> SoftDeleteAsync(int id);

        /// <summary>
        /// Khôi phục người dùng đã bị xóa mềm.
        /// </summary>
        /// <param name="id">ID người dùng cần khôi phục.</param>
        /// <returns>True nếu khôi phục thành công.</returns>
        Task<bool> RestoreAsync(int id);

        /// <summary>
        /// Kiểm tra xem tên đăng nhập đã tồn tại hay chưa (bao gồm cả đã xóa).
        /// </summary>
        /// <param name="username">Tên đăng nhập cần kiểm tra.</param>
        /// <returns>True nếu tên đăng nhập đã tồn tại.</returns>
        Task<bool> IsUsernameExistsAsync(string username); // Thêm phương thức kiểm tra tồn tại (nếu chưa có trong IDALDocGia hoặc dùng chung)
    }
}