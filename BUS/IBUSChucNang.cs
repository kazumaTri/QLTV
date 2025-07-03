// BUS/IBUSChucNang.cs
using DTO; // Cần để sử dụng ChucNangDTO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    /// <summary>
    /// Interface for Business Logic Layer operations related to ChucNang (Function/Permission).
    /// Defines the contract for business logic concerning functions.
    /// </summary>
    public interface IBUSChucNang
    {
        /// <summary>
        /// Retrieves all available functions as DTOs asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of ChucNangDTO objects representing all available functions.</returns>
        Task<List<ChucNangDTO>> GetAllChucNangAsync();

        // --- Có thể thêm các phương thức khác nếu cần trong tương lai ---
        // Ví dụ:
        // Task<ChucNangDTO?> GetChucNangByIdAsync(int id);
        // Task<ChucNangDTO?> GetChucNangByMaAsync(string maChucNang);
        // (Thường ít khi cần thêm/sửa/xóa ChucNang từ logic nghiệp vụ thông thường)
    }
}