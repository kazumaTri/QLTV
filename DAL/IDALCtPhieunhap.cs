using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IDALCtPhieunhap
    {
        Task<List<CtPhieunhap>> GetBySoPhieuNhapAsync(int soPhieuNhap);
        Task<bool> AddRangeAsync(IEnumerable<CtPhieunhap> ctPhieuNhaps); // Thêm nhiều chi tiết cùng lúc
                                                                         // Task<bool> DeleteBySoPhieuNhapAsync(int soPhieuNhap); // Nếu cần xóa chi tiết khi xóa phiếu nhập
    }
}