// DAL/IDALChucNang.cs
using DAL.Models; // Cần để sử dụng Entity 'Chucnang'
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    /// <summary>
    /// Interface định nghĩa các phương thức truy cập dữ liệu cho ChucNang (Function/Permission).
    /// Các lớp DAL cụ thể (như DALChucNang) sẽ implement interface này.
    /// </summary>
    public interface IDALChucNang
    {
        /// <summary>
        /// Lấy tất cả các Chucnang từ cơ sở dữ liệu một cách bất đồng bộ.
        /// </summary>
        /// <returns>Một Task chứa danh sách các đối tượng Chucnang.</returns>
        Task<List<Chucnang>> GetAllAsync();

        // --- Khai báo các phương thức khác nếu cần ---
        // Ví dụ:
        // Task<Chucnang?> GetByIdAsync(int id);
        // Task<Chucnang?> AddAsync(Chucnang chucNang);
        // Task<bool> UpdateAsync(Chucnang chucNang);
        // Task<bool> DeleteAsync(int id);
    }
}
