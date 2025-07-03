using DTO; // Cần using DTO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public interface IBUSCtPhieunhap
    {
        /// <summary>
        /// Lấy danh sách chi tiết của một phiếu nhập dưới dạng DTO.
        /// </summary>
        /// <param name="soPhieuNhap">Số phiếu nhập.</param>
        /// <returns>Danh sách CtPhieuNhapDTO.</returns>
        Task<List<CtPhieuNhapDTO>> GetCtPhieuNhapDTOBySoPhieuAsync(int soPhieuNhap);
    }
}