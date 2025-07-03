using DAL.Models;
using System.Collections.Generic;

namespace DAL
{
    public interface IDALNhaXuatBan
    {
        List<NhaXuatBan> GetAll();
        NhaXuatBan GetById(int id);
        bool Add(NhaXuatBan nxb);
        bool Update(NhaXuatBan nxb);
        bool Delete(int id);
        List<NhaXuatBan> SearchByName(string name);
        // Thêm các phương thức khác nếu cần
    }
}