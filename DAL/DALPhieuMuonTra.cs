// DAL/DALPhieuMuonTra.cs
// --- USING ---
using DAL.Models; // Namespace chứa các Entity models (Phieumuontra, Docgia, Cuonsach, Sach, Tuasach, Theloai, Tacgia)
using Microsoft.EntityFrameworkCore; // Cần cho DbContext, async methods, Include, CountAsync, Where, FirstOrDefaultAsync, ToListAsync
using System; // Cần cho Exception, ArgumentNullException, DateTime, InvalidOperationException
using System.Collections.Generic; // Cần cho List
using System.Linq; // Cần cho LINQ (Where, Any)
using System.Threading.Tasks; // Cần cho async/await Task
using Microsoft.Extensions.DependencyInjection; // <<< Cần cho IServiceScopeFactory


namespace DAL
{
    /// <summary>
    /// Lớp DAL triển khai IDALPhieuMuonTra, tương tác với DB thông qua EF Core.
    /// Đã thêm triển khai các phương thức đếm số sách đang mượn và quá hạn.
    /// Đã refactor để sử dụng IServiceScopeFactory để quản lý vòng đời DbContext cho từng thao tác.
    /// Đã thêm phương thức lấy lịch sử phạt.
    /// **Quan trọng:** Mã này yêu cầu Entity 'Phieumuontra' phải có thuộc tính 'NgayTra' (kiểu DateTime?).
    /// </summary>
    public class DALPhieuMuonTra : IDALPhieuMuonTra // <<< Implement interface IDALPhieuMuonTra
    {
        // --- DEPENDENCIES ---
        private readonly IServiceScopeFactory _scopeFactory;

        // --- CONSTRUCTOR (Sử dụng Dependency Injection) ---
        public DALPhieuMuonTra(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        // --- METHOD IMPLEMENTATIONS (Từ IDALPhieuMuonTra) ---

        // Lấy toàn bộ lịch sử mượn trả của độc giả
        public async Task<List<Phieumuontra>> GetHistoryByDocGiaIdAsync(int idDocGia)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    return await _context.Phieumuontra
                        .AsNoTracking()
                        .Include(pmt => pmt.IdCuonSachNavigation) // Include để lấy thông tin sách
                            .ThenInclude(cs => cs.IdSachNavigation)
                                .ThenInclude(s => s.IdTuaSachNavigation) // Đến tựa sách để lấy tên
                        .Where(pmt => pmt.IdDocGia == idDocGia) // Lọc theo IdDocGia
                        .OrderByDescending(pmt => pmt.NgayMuon) // Sắp xếp theo ngày mượn giảm dần (mới nhất lên đầu)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error GetHistoryByDocGiaIdAsync (DAL - ID DocGia: {idDocGia}): {ex.Message}");
                    throw; // Ném lại lỗi để tầng BUS xử lý
                }
            }
        }

        // <<< THÊM: Triển khai lấy lịch sử phạt >>>
        public async Task<List<Phieumuontra>> GetFineHistoryByDocGiaIdAsync(int idDocGia)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Chỉ lấy những phiếu có SoTienPhat > 0
                    // Giả định SoTienPhat là kiểu nullable int (int?)
                    return await _context.Phieumuontra
                        .AsNoTracking()
                        .Include(pmt => pmt.IdCuonSachNavigation)
                            .ThenInclude(cs => cs.IdSachNavigation)
                                .ThenInclude(s => s.IdTuaSachNavigation)
                        .Where(pmt => pmt.IdDocGia == idDocGia && pmt.SoTienPhat.HasValue && pmt.SoTienPhat > 0) // Lọc theo IdDocGia và có tiền phạt > 0
                        .OrderByDescending(pmt => pmt.NgayTra ?? pmt.HanTra) // Sắp xếp theo ngày trả (nếu có) hoặc hạn trả
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error GetFineHistoryByDocGiaIdAsync (DAL - ID DocGia: {idDocGia}): {ex.Message}");
                    throw;
                }
            }
        }


        public async Task<List<Phieumuontra>> GetAllAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    return await _context.Phieumuontra
                        .AsNoTracking()
                        .Include(pmt => pmt.IdDocGiaNavigation)
                        .Include(pmt => pmt.IdCuonSachNavigation)
                            .ThenInclude(cs => cs.IdSachNavigation)
                                .ThenInclude(s => s.IdTuaSachNavigation)
                                    .ThenInclude(ts => ts.IdTheLoaiNavigation)
                         .Include(pmt => pmt.IdCuonSachNavigation) // Lặp lại để include nhánh khác
                            .ThenInclude(cs => cs.IdSachNavigation)
                                .ThenInclude(s => s.IdTuaSachNavigation)
                                    .ThenInclude(ts => ts.IdTacGia) // Giả định tên đúng là IdTacGia

                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in DALPhieuMuonTra.GetAllAsync: {ex.Message}");
                    throw;
                }
            }
        }


        public async Task<Phieumuontra?> GetByIdAsync(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    return await _context.Phieumuontra
                       .AsNoTracking()
                       .Include(pmt => pmt.IdDocGiaNavigation)
                       .Include(pmt => pmt.IdCuonSachNavigation)
                           .ThenInclude(cs => cs.IdSachNavigation)
                               .ThenInclude(s => s.IdTuaSachNavigation)
                                   .ThenInclude(ts => ts.IdTheLoaiNavigation)
                        .Include(pmt => pmt.IdCuonSachNavigation)
                           .ThenInclude(cs => cs.IdSachNavigation)
                               .ThenInclude(s => s.IdTuaSachNavigation)
                                   .ThenInclude(ts => ts.IdTacGia)
                       .Where(pmt => pmt.SoPhieuMuonTra == id) // Dùng khóa chính SoPhieuMuonTra
                       .FirstOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in DALPhieuMuonTra.GetByIdAsync (ID: {id}): {ex.Message}");
                    throw;
                }
            }
        }

        // Lấy các phiếu đang mượn (chưa trả) của độc giả
        public async Task<List<Phieumuontra>> GetLoansByDocgiaIdAsync(int idDocGia)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    return await _context.Phieumuontra
                        .AsNoTracking()
                        .Include(pmt => pmt.IdCuonSachNavigation)
                            .ThenInclude(cs => cs.IdSachNavigation)
                                .ThenInclude(s => s.IdTuaSachNavigation)
                        .Where(pmt => pmt.IdDocGia == idDocGia && pmt.NgayTra == null) // Lấy phiếu đang mượn (chưa trả)
                        .ToListAsync();
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Error GetLoansByDocgiaIdAsync: {ex.Message}"); throw; }
            }
        }

        // Lấy các phiếu quá hạn (chưa trả)
        public async Task<List<Phieumuontra>> GetOverdueLoansAsync(DateTime currentDate)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    return await _context.Phieumuontra
                        .AsNoTracking()
                        .Include(pmt => pmt.IdDocGiaNavigation)
                        .Include(pmt => pmt.IdCuonSachNavigation)
                            .ThenInclude(cs => cs.IdSachNavigation)
                                .ThenInclude(s => s.IdTuaSachNavigation)
                        .Where(pmt => pmt.NgayTra == null && pmt.HanTra < currentDate.Date) // Chưa trả và quá hạn
                        .ToListAsync();
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Error GetOverdueLoansAsync: {ex.Message}"); throw; }
            }
        }

        // Thêm phiếu mượn mới
        public async Task<Phieumuontra?> AddAsync(Phieumuontra phieuMuonTra)
        {
            if (phieuMuonTra == null) throw new ArgumentNullException(nameof(phieuMuonTra));

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    if (phieuMuonTra.IdDocGiaNavigation != null) phieuMuonTra.IdDocGiaNavigation = null!;
                    if (phieuMuonTra.IdCuonSachNavigation != null) phieuMuonTra.IdCuonSachNavigation = null!;
                    phieuMuonTra.NgayTra = null; // Đảm bảo ngày trả là null khi mới mượn

                    await _context.Phieumuontra.AddAsync(phieuMuonTra);
                    int affectedRows = await _context.SaveChangesAsync();

                    return (affectedRows > 0) ? phieuMuonTra : null; // Trả về entity hoặc null
                }
                catch (DbUpdateException ex) { System.Diagnostics.Debug.WriteLine($"Error adding PhieuMuonTra: {ex.InnerException?.Message ?? ex.Message}"); throw; }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Unexpected error adding PhieuMuonTra: {ex.Message}"); throw; }
            }
        }

        // Cập nhật phiếu mượn (thường là khi trả sách)
        public async Task<bool> UpdateAsync(Phieumuontra phieuMuonTra)
        {
            if (phieuMuonTra == null) throw new ArgumentNullException(nameof(phieuMuonTra));
            if (phieuMuonTra.SoPhieuMuonTra <= 0) throw new ArgumentException("Invalid SoPhieuMuonTra for update.", nameof(phieuMuonTra.SoPhieuMuonTra));

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var existingPhieuMuonTra = await _context.Phieumuontra.FindAsync(phieuMuonTra.SoPhieuMuonTra);
                    if (existingPhieuMuonTra == null) return false;

                    // Cập nhật các trường cho phép
                    existingPhieuMuonTra.NgayTra = phieuMuonTra.NgayTra;
                    existingPhieuMuonTra.SoTienPhat = phieuMuonTra.SoTienPhat;

                    int affectedRows = await _context.SaveChangesAsync();
                    return affectedRows > 0;
                }
                catch (DbUpdateConcurrencyException ex) { System.Diagnostics.Debug.WriteLine($"Concurrency error updating PhieuMuonTra (SoPhieu: {phieuMuonTra.SoPhieuMuonTra})."); throw; }
                catch (DbUpdateException ex) { System.Diagnostics.Debug.WriteLine($"Error updating PhieuMuonTra (SoPhieu: {phieuMuonTra.SoPhieuMuonTra}): {ex.InnerException?.Message ?? ex.Message}"); throw; }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Unexpected error updating PhieuMuonTra (SoPhieu: {phieuMuonTra.SoPhieuMuonTra}): {ex.Message}"); throw; }
            }
        }

        // Xóa phiếu mượn (cẩn thận!)
        public async Task<bool> HardDeleteAsync(int soPhieuMuonTra)
        {
            if (soPhieuMuonTra <= 0) throw new ArgumentException("ID không hợp lệ.", nameof(soPhieuMuonTra));
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var phieuMuonTraToRemove = await _context.Phieumuontra.FindAsync(soPhieuMuonTra);
                    if (phieuMuonTraToRemove == null) return false;

                    // Cân nhắc kiểm tra ràng buộc ở BUS trước khi gọi hàm này

                    _context.Phieumuontra.Remove(phieuMuonTraToRemove);
                    int affectedRows = await _context.SaveChangesAsync();
                    return affectedRows > 0;
                }
                catch (DbUpdateException ex) { System.Diagnostics.Debug.WriteLine($"Error deleting PhieuMuonTra (SoPhieu: {soPhieuMuonTra}): {ex.InnerException?.Message ?? ex.Message}"); throw new InvalidOperationException($"Không thể xóa phiếu mượn trả (Số phiếu: {soPhieuMuonTra}) do ràng buộc dữ liệu.", ex); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Unexpected error deleting PhieuMuonTra (SoPhieu: {soPhieuMuonTra}): {ex.Message}"); throw; }
            }
        }

        // Đếm số sách đang mượn (chưa trả)
        public async Task<int> CountBorrowedLoansAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    return await _context.Phieumuontra.CountAsync(p => p.NgayTra == null);
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Error in DALPhieuMuonTra.CountBorrowedLoansAsync: {ex.Message}"); throw; }
            }
        }

        // Đếm số sách quá hạn (chưa trả)
        public async Task<int> CountOverdueLoansAsync(DateTime currentDate)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    return await _context.Phieumuontra.CountAsync(p => p.NgayTra == null && p.HanTra < currentDate.Date);
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Error in DALPhieuMuonTra.CountOverdueLoansAsync: {ex.Message}"); throw; }
            }
        }

        // Lấy tất cả phiếu mượn trong khoảng thời gian
        public async Task<List<Phieumuontra>> GetAllInRangeAsync(DateTime startDate, DateTime endDate)
        {
            endDate = endDate.Date.AddDays(1).AddTicks(-1);
            startDate = startDate.Date;

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    return await context.Phieumuontra
                        .Where(p => p.NgayMuon >= startDate && p.NgayMuon <= endDate)
                        .Include(p => p.IdCuonSachNavigation)
                            .ThenInclude(cs => cs.IdSachNavigation)
                                .ThenInclude(s => s.IdTuaSachNavigation)
                                    .ThenInclude(ts => ts.IdTheLoaiNavigation)
                        .Include(p => p.IdCuonSachNavigation)
                            .ThenInclude(cs => cs.IdSachNavigation)
                                .ThenInclude(s => s.IdTuaSachNavigation)
                                    .ThenInclude(ts => ts.IdTacGia)
                         .Include(p => p.IdDocGiaNavigation)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in DALPhieuMuonTra.GetAllInRangeAsync ({startDate} - {endDate}): {ex.Message}");
                    throw;
                }
            }
        }

    } // End class DALPhieuMuonTra
} // End namespace DAL