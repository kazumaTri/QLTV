// Project/Namespace: DAL
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tacgia = DAL.Models.Tacgia;

namespace DAL
{
    public interface IDALTuaSach
    {
        // ... (các phương thức hiện có như GetAllAsync, GetByIdAsync, AddAsync, UpdateAsync, etc.) ...

        Task<List<Tuasach>> GetAllAsync();
        Task<Tuasach?> GetByIdAsync(int id);
        Task<Tuasach?> GetByMaAsync(string maTuaSach);
        Task<Tuasach?> GetByTenAsync(string tenTuaSach);
        Task<Tuasach?> GetByMaIncludingDeletedAsync(string maTuaSach);
        Task<Tuasach?> GetByTenIncludingDeletedAsync(string tenTuaSach);

        Task<bool> IsTenTuaSachExistsExcludingIdAsync(string tenTuaSach, int excludeId);
        Task<bool> IsTenTuaSachExistsAsync(string tenTuaSach);
        Task<bool> IsMaTuaSachExistsExcludingIdAsync(string maTuaSach, int excludeId);
        Task<bool> IsMaTuaSachExistsAsync(string maTuaSach);
        Task<bool> IsTheLoaiUsedAsync(int theLoaiId);

        Task<Tuasach?> AddAsync(Tuasach tuaSach);
        Task<bool> UpdateAsync(Tuasach tuaSach); // Chỉ cập nhật thuộc tính scalar của Tuasach
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<bool> HardDeleteAsync(int id); // Cần cẩn thận

        Task<List<Tacgia>> GetTacGiasByIdsAsync(List<int> tacGiaIds);

        // --- PHƯƠNG THỨC MỚI ĐỂ CẬP NHẬT TÁC GIẢ CHO TỰA SÁCH ---
        /// <summary>
        /// Cập nhật danh sách tác giả liên kết với một tựa sách.
        /// Xóa các liên kết cũ và tạo các liên kết mới dựa trên danh sách ID tác giả được cung cấp.
        /// </summary>
        /// <param name="tuaSachId">ID của tựa sách cần cập nhật tác giả.</param>
        /// <param name="newTacGiaIds">Danh sách ID của các tác giả mới cần liên kết.</param>
        /// <returns>True nếu cập nhật thành công, False nếu tựa sách không tồn tại.</returns>
        Task<bool> UpdateTuaSachTacGiasAsync(int tuaSachId, IEnumerable<int> newTacGiaIds);
        // -----------------------------------------------------------
        Task<List<Tuasach>> SearchAndFilterAsync(string? searchText, int theLoaiId, int tacGiaId);
    }
}