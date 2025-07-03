// BUS/IBUSNhomNguoiDung.cs
using DTO; // Cần cho NhomNguoiDungDTO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    /// <summary>
    /// Interface định nghĩa các nghiệp vụ liên quan đến Nhóm Người Dùng.
    /// </summary>
    public interface IBUSNhomNguoiDung
    {
        Task<List<NhomNguoiDungDTO>> GetAllNhomNguoiDungAsync();
        Task<NhomNguoiDungDTO?> GetNhomNguoiDungByIdAsync(int id);
        Task<NhomNguoiDungDTO?> GetNhomNguoiDungByMaAsync(string maNhom);
        Task<NhomNguoiDungDTO?> AddNhomNguoiDungAsync(NhomNguoiDungDTO newNhomDTO);
        Task<bool> UpdateNhomNguoiDungAsync(NhomNguoiDungDTO updatedNhomDTO);
        Task<bool> DeleteNhomNguoiDungAsync(int id);

        // --- PHƯƠNG THỨC MỚI ĐỂ QUẢN LÝ PHÂN QUYỀN ---

        /// <summary>
        /// Lấy danh sách ID các Chức năng đã được gán cho một Nhóm người dùng.
        /// </summary>
        /// <param name="nhomId">ID của Nhóm người dùng.</param>
        /// <returns>Danh sách các ID Chức năng.</returns>
        Task<List<int>> GetChucNangIdsByNhomAsync(int nhomId); // Đổi tên từ GetChucNangIdsByNhomIdAsync để nhất quán BUS

        /// <summary>
        /// Cập nhật danh sách Chức năng được gán cho một Nhóm người dùng.
        /// </summary>
        /// <param name="nhomId">ID của Nhóm người dùng.</param>
        /// <param name="chucNangIds">Danh sách ID các Chức năng mới cần gán.</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại.</returns>
        Task<bool> UpdatePhanQuyenAsync(int nhomId, List<int> chucNangIds);
    }
}