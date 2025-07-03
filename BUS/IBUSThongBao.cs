// BUS/IBUSThongBao.cs
using DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BUS
{
    public interface IBUSThongBao
    {
        // Phương thức dùng để hiển thị thông báo cho người dùng cuối (frmMain)
        Task<List<ThongBaoDTO>> GetActiveNotificationsAsync();

        // --- THÊM CÁC ĐỊNH NGHĨA SAU CHO VIỆC QUẢN LÝ (ucQuanLyThongBao) ---

        // Lấy tất cả thông báo (cho grid quản lý)
        Task<List<ThongBaoDTO>> GetAllNotificationsAsync();

        // Lấy thông báo theo ID (để sửa)
        Task<ThongBaoDTO?> GetNotificationByIdAsync(int id);

        // Thêm thông báo mới (Trả về kết quả và thông báo lỗi nếu có)
        Task<(bool Success, string ErrorMessage)> AddNotificationAsync(ThongBaoDTO thongBaoDto);

        // Cập nhật thông báo (Trả về kết quả và thông báo lỗi nếu có)
        Task<(bool Success, string ErrorMessage)> UpdateNotificationAsync(ThongBaoDTO thongBaoDto);

        // Xóa thông báo (Trả về kết quả và thông báo lỗi nếu có)
        Task<(bool Success, string ErrorMessage)> DeleteNotificationAsync(int id);
        Task<List<ThongBaoDTO>> GetRecentNotificationsAsync(int count);
        Task<bool> CreateActivityLogAsync(string noiDung, string loaiThongBao = "Hoạt động");

        // --- KẾT THÚC PHẦN THÊM ---
    }
}