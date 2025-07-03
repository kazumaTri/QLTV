using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Thêm using này
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DALCtPhieunhap : IDALCtPhieunhap
    {
        private readonly QLTVContext _context;
        private readonly ILogger<DALCtPhieunhap> _logger; // Thêm logger

        public DALCtPhieunhap(QLTVContext context, ILogger<DALCtPhieunhap> logger) // Inject logger
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Khởi tạo logger
        }

        public async Task<bool> AddRangeAsync(IEnumerable<CtPhieunhap> ctPhieuNhaps)
        {
            if (ctPhieuNhaps == null || !ctPhieuNhaps.Any())
            {
                _logger.LogWarning("Attempted to add an empty or null collection of CtPhieunhap.");
                // throw new ArgumentNullException(nameof(ctPhieuNhaps)); // Hoặc trả về false tùy logic
                return false;
            }

            int soPhieuNhap = ctPhieuNhaps.First().SoPhieuNhap; // Giả sử tất cả chi tiết thuộc cùng 1 phiếu
            _logger.LogInformation("Attempting to add {Count} CtPhieunhap items for SoPhieuNhap: {SoPhieuNhap}", ctPhieuNhaps.Count(), soPhieuNhap);

            try
            {
                await _context.CtPhieunhap.AddRangeAsync(ctPhieuNhaps);
                int affectedRows = await _context.SaveChangesAsync();
                bool success = affectedRows == ctPhieuNhaps.Count(); // Kiểm tra xem tất cả đã được thêm thành công chưa

                if (success)
                {
                    _logger.LogInformation("Successfully added {Count} CtPhieunhap items for SoPhieuNhap: {SoPhieuNhap}", affectedRows, soPhieuNhap);
                }
                else
                {
                    _logger.LogWarning("Potentially incomplete add operation for CtPhieunhap for SoPhieuNhap: {SoPhieuNhap}. Expected {ExpectedCount}, Added {ActualCount}", soPhieuNhap, ctPhieuNhaps.Count(), affectedRows);
                }
                return success;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while adding range of CtPhieunhap for SoPhieuNhap: {SoPhieuNhap}. Details: {ErrorMessage}", soPhieuNhap, ex.InnerException?.Message ?? ex.Message);
                throw new Exception("An error occurred while saving the book entry details. Please check the logs for details.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in AddRangeAsync for CtPhieunhap for SoPhieuNhap: {SoPhieuNhap}", soPhieuNhap);
                throw;
            }
        }

        public async Task<List<CtPhieunhap>> GetBySoPhieuNhapAsync(int soPhieuNhap)
        {
            _logger.LogInformation("Attempting to retrieve CtPhieunhap items for SoPhieuNhap: {SoPhieuNhap}", soPhieuNhap);
            if (soPhieuNhap <= 0)
            {
                _logger.LogWarning("Attempted to get CtPhieunhap with invalid SoPhieuNhap: {SoPhieuNhap}", soPhieuNhap);
                return new List<CtPhieunhap>(); // Trả về danh sách rỗng
            }
            try
            {
                var details = await _context.CtPhieunhap
                                            .Where(ct => ct.SoPhieuNhap == soPhieuNhap)
                                            .Include(ct => ct.IdSachNavigation) // Tải thông tin Sách
                                                .ThenInclude(s => s.IdTuaSachNavigation) // Tải thông tin Tựa Sách từ Sách
                                            .AsNoTracking()
                                            .ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} CtPhieunhap items for SoPhieuNhap: {SoPhieuNhap}", details.Count, soPhieuNhap);
                return details;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving CtPhieunhap for SoPhieuNhap: {SoPhieuNhap}", soPhieuNhap);
                throw;
            }
        }
    }
}