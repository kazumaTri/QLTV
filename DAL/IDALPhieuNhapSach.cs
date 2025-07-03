using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IDALPhieuNhapSach
    {
        Task<List<Phieunhapsach>> GetAllAsync();
        Task<Phieunhapsach?> GetByIdAsync(int soPhieuNhap);
        Task<int> AddAsync(Phieunhapsach phieuNhap); // Trả về Số phiếu nhập mới được tạo
        // Task<bool> UpdateAsync(Phieunhapsach phieuNhap); // Thường ít khi cập nhật phiếu nhập, tùy nghiệp vụ
        // Task<bool> DeleteAsync(int soPhieuNhap);      // Thường ít khi xóa phiếu nhập, tùy nghiệp vụ
    }
}