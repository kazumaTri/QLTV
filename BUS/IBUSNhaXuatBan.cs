using DTO;
using System.Collections.Generic;

namespace BUS
{
    public interface IBUSNhaXuatBan
    {
        List<NhaXuatBanDTO> GetAll();
        NhaXuatBanDTO GetById(int id);
        string AddNhaXuatBan(NhaXuatBanDTO nxbDto); // Trả về string thông báo lỗi/thành công
        string UpdateNhaXuatBan(NhaXuatBanDTO nxbDto);
        string DeleteNhaXuatBan(int id);
        List<NhaXuatBanDTO> SearchByName(string name);
        // Thêm các phương thức nghiệp vụ khác nếu cần
    }
}