// Project/Namespace: BUS
using DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BUS
{
    public interface IBUSNguoiDung
    {
        Task<NguoiDungDTO?> DangNhapAsync(string username, string password);
        Task<List<NguoiDungDTO>> GetAllNguoiDungAsync();
        // Cần rawPassword khi thêm mới
        Task<NguoiDungDTO?> AddNguoiDungAsync(NguoiDungDTO nguoiDungDto, string rawPassword);
        // Update không bao gồm mật khẩu ở đây
        Task<bool> UpdateNguoiDungAsync(NguoiDungDTO nguoiDungDto);
        Task<bool> UpdatePasswordAsync(int idNguoiDung, string rawPassword);
        Task<bool> HardDeleteNguoiDungAsync(int id); // Chỉ còn xóa cứng
        Task<bool> DatLaiMatKhauAsync(string tenDangNhap, string matKhauMoi);
        // Đã loại bỏ các phương thức liên quan đến SoftDelete/Restore (vì Entity không có DaAn)
        // Task<bool> CanSoftDeleteNguoiDungAsync(int id); // Bị loại bỏ
        // Task<bool> SoftDeleteNguoiDungAsync(int id); // Bị loại bỏ
        // Task<bool> RestoreNguoiDungAsync(int id); // Bị loại bỏ
    }
}