// DAL/IDALThongBao.cs
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL
{
    public interface IDALThongBao
    {
        // Phương thức dùng để hiển thị thông báo cho người dùng cuối
        Task<List<ThongBao>> GetAllActiveAsync();

        // --- THÊM CÁC ĐỊNH NGHĨA SAU CHO VIỆC QUẢN LÝ ---
        Task<List<ThongBao>> GetAllAsync();       // Lấy tất cả thông báo (cho màn hình quản lý)
        Task<ThongBao?> GetByIdAsync(int id);     // Lấy thông báo theo ID (để sửa)
        Task<bool> AddAsync(ThongBao thongBao);   // Thêm thông báo mới
        Task<bool> UpdateAsync(ThongBao thongBao); // Cập nhật thông báo
        Task<bool> DeleteAsync(int id);           // Xóa thông báo
        Task<List<ThongBao>> GetRecentAsync(int count);
        // --- KẾT THÚC PHẦN THÊM ---
    }
}