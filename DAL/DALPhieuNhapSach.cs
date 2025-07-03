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
    public class DALPhieuNhapSach : IDALPhieuNhapSach
    {
        private readonly QLTVContext _context;
        private readonly ILogger<DALPhieuNhapSach> _logger; // Thêm logger

        public DALPhieuNhapSach(QLTVContext context, ILogger<DALPhieuNhapSach> logger) // Inject logger
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Khởi tạo logger
        }

        public async Task<int> AddAsync(Phieunhapsach phieuNhap)
        {
            if (phieuNhap == null)
            {
                _logger.LogError("Attempted to add a null Phieunhapsach entity.");
                throw new ArgumentNullException(nameof(phieuNhap));
            }
            try
            {
                _context.Phieunhapsach.Add(phieuNhap);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully added Phieunhapsach with SoPhieuNhap: {SoPhieuNhap}", phieuNhap.SoPhieuNhap);
                return phieuNhap.SoPhieuNhap; // Trả về ID (SoPhieuNhap) được tạo tự động
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while adding Phieunhapsach. Details: {ErrorMessage}", ex.InnerException?.Message ?? ex.Message);
                throw new Exception("An error occurred while saving the book entry slip. Please check the logs for details.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in AddAsync for Phieunhapsach.");
                throw; // Ném lại lỗi để lớp trên xử lý
            }
        }

        public async Task<List<Phieunhapsach>> GetAllAsync()
        {
            _logger.LogInformation("Attempting to retrieve all Phieunhapsach entities.");
            try
            {
                // Include chi tiết nếu cần hiển thị ngay, nhưng thường chỉ lấy phiếu nhập chính
                var phieuNhaps = await _context.Phieunhapsach
                                             //.Include(p => p.CtPhieunhaps) // Bỏ comment nếu muốn tải cả chi tiết
                                             .AsNoTracking() // Tăng hiệu năng nếu chỉ đọc
                                             .ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} Phieunhapsach entities.", phieuNhaps.Count);
                return phieuNhaps;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all Phieunhapsach entities.");
                throw;
            }
        }

        public async Task<Phieunhapsach?> GetByIdAsync(int soPhieuNhap)
        {
            _logger.LogInformation("Attempting to retrieve Phieunhapsach with SoPhieuNhap: {SoPhieuNhap}", soPhieuNhap);
            if (soPhieuNhap <= 0)
            {
                _logger.LogWarning("Attempted to get Phieunhapsach with invalid SoPhieuNhap: {SoPhieuNhap}", soPhieuNhap);
                return null;
            }
            try
            {
                // Include chi tiết phiếu nhập khi lấy theo ID
                var phieuNhap = await _context.Phieunhapsach
                                             .Include(p => p.CtPhieunhap) // Tải chi tiết liên quan
                                               .ThenInclude(ct => ct.IdSachNavigation) // Tải thông tin Sách từ chi tiết
                                                   .ThenInclude(s => s.IdTuaSachNavigation) // Tải thông tin Tựa Sách từ Sách
                                             .AsNoTracking()
                                             .FirstOrDefaultAsync(p => p.SoPhieuNhap == soPhieuNhap);

                if (phieuNhap == null)
                {
                    _logger.LogWarning("Phieunhapsach with SoPhieuNhap: {SoPhieuNhap} not found.", soPhieuNhap);
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved Phieunhapsach with SoPhieuNhap: {SoPhieuNhap}", soPhieuNhap);
                }
                return phieuNhap;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Phieunhapsach with SoPhieuNhap: {SoPhieuNhap}", soPhieuNhap);
                throw;
            }
        }

        // Implement các phương thức UpdateAsync, DeleteAsync nếu cần thiết cho nghiệp vụ
    }
}