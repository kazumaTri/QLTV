// File: DAL/IDALPhieuThu.cs
using DAL.Models; // Namespace chứa Entity Phieuthu
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL
{
    /// <summary>
    /// Interface cho lớp truy cập dữ liệu Phiếu Thu Tiền Phạt.
    /// Định nghĩa các phương thức cơ bản để tương tác với bảng PhieuThu trong cơ sở dữ liệu.
    /// </summary>
    public interface IDALPhieuThu // <<< Sửa thành public interface
    {
        /// <summary>
        /// Thêm mới một phiếu thu vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="phieuThu">Entity Phieuthu cần thêm.</param>
        /// <returns>Entity Phieuthu đã được thêm (với ID được cập nhật) nếu thành công, null nếu thất bại.</returns>
        Task<Phieuthu?> AddAsync(Phieuthu phieuThu);

        /// <summary>
        /// Lấy thông tin phiếu thu theo ID.
        /// </summary>
        /// <param name="id">ID phiếu thu cần lấy.</param>
        /// <returns>Entity Phieuthu nếu tìm thấy, null nếu không.</returns>
        Task<Phieuthu?> GetByIdAsync(int id);

        /// <summary>
        /// Lấy danh sách tất cả phiếu thu.
        /// </summary>
        /// <returns>Danh sách Entity Phieuthu.</returns>
        Task<List<Phieuthu>> GetAllAsync();

        /// <summary>
        /// Lấy danh sách phiếu thu của một độc giả cụ thể.
        /// </summary>
        /// <param name="docGiaId">ID của độc giả.</param>
        /// <returns>Danh sách Entity Phieuthu của độc giả đó.</returns>
        Task<List<Phieuthu>> GetByDocGiaIdAsync(int docGiaId);

        // Có thể thêm các phương thức khác nếu cần (Update, Delete, Search...)
        // Ví dụ:
        // Task<bool> UpdateAsync(Phieuthu phieuThu);
        // Task<bool> DeleteAsync(int id);
    }
}