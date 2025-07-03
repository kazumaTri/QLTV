using DAL;
using DTO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class BUSCtPhieunhap : IBUSCtPhieunhap
    {
        private readonly IDALCtPhieunhap _dalCtPhieuNhap;
        private readonly ILogger<BUSCtPhieunhap> _logger;

        public BUSCtPhieunhap(IDALCtPhieunhap dalCtPhieuNhap, ILogger<BUSCtPhieunhap> logger)
        {
            _dalCtPhieuNhap = dalCtPhieuNhap;
            _logger = logger;
        }

        public async Task<List<CtPhieuNhapDTO>> GetCtPhieuNhapDTOBySoPhieuAsync(int soPhieuNhap)
        {
            _logger.LogInformation("Getting CtPhieuNhap DTOs for SoPhieuNhap: {SoPhieuNhap}", soPhieuNhap);
            var ctPhieuNhaps = await _dalCtPhieuNhap.GetBySoPhieuNhapAsync(soPhieuNhap); // DAL đã Include Sách/Tựa Sách

            // Map từ List<CtPhieunhap> sang List<CtPhieuNhapDTO>
            return ctPhieuNhaps.Select(ct => new CtPhieuNhapDTO
            {
                IdSach = ct.IdSach,
                MaSach = ct.IdSachNavigation?.MaSach,
                TenSach = ct.IdSachNavigation?.IdTuaSachNavigation?.TenTuaSach,
                SoLuongNhap = ct.SoLuongNhap,
                DonGia = ct.DonGia
            }).ToList();
        }
    }
}