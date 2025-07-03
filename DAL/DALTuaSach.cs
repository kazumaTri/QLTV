// --- USING ---
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging; // Thêm using này
using System;
using System.Collections.Generic;
using System.Diagnostics; // Thêm using này để dùng Debug.WriteLine nếu cần
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    public class DALTuaSach : IDALTuaSach
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DALTuaSach> _logger; // Thêm ILogger

        public DALTuaSach(IServiceScopeFactory scopeFactory, ILogger<DALTuaSach> logger) // Inject ILogger
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.LogInformation("DALTuaSach initialized.");
        }

        /// <summary>
        /// Lấy tất cả các Tựa sách chưa bị xóa mềm (DaAn == 0).
        /// Đã thêm logging chi tiết hơn để dễ dàng debug.
        /// </summary>
        /// <returns>Danh sách các Tựa sách.</returns>
        public async Task<List<Tuasach>> GetAllAsync()
        {
            _logger.LogInformation("Attempting to retrieve all non-deleted Tuasach entities.");
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    _logger.LogInformation("Building query for non-deleted Tuasach with Includes...");
                    // Truy vấn cơ sở dữ liệu để lấy các Tựa sách chưa bị xóa mềm
                    // và bao gồm các thông tin liên quan cần thiết (Thể loại, Tác giả, Sách)
                    var query = _context.Tuasach
                        .Where(ts => ts.DaAn == 0) // Chỉ lấy các Tựa sách chưa bị xóa (DaAn = 0)
                        .Include(ts => ts.IdTheLoaiNavigation) // Lấy thông tin Thể loại liên quan
                        .Include(ts => ts.IdTacGia) // Lấy thông tin các Tác giả liên quan (quan hệ M-N)
                        .Include(ts => ts.Sach) // Lấy thông tin các Sách (ấn bản) liên quan
                        .AsNoTracking(); // Sử dụng AsNoTracking vì chỉ đọc dữ liệu, tăng hiệu suất

                    _logger.LogInformation("Executing query GetAllAsync...");
                    // Thực thi truy vấn và lấy kết quả dưới dạng danh sách
                    var tuasachs = await query.ToListAsync();

                    // Log số lượng kết quả trả về
                    _logger.LogInformation("Successfully retrieved {Count} non-deleted Tuasach entities.", tuasachs.Count);

                    // Ghi thêm log nếu danh sách rỗng (giúp xác định vấn đề dữ liệu)
                    if (!tuasachs.Any())
                    {
                        _logger.LogWarning("GetAllAsync query executed successfully, but returned an empty list. Check if there are any Tuasach records with DaAn = 0 in the database.");
                    }

                    return tuasachs;
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi chi tiết nếu có vấn đề xảy ra trong quá trình truy vấn
                    _logger.LogError(ex, "Error in DALTuaSach.GetAllAsync while querying the database.");
                    // Ném lại một Exception mới với thông điệp rõ ràng hơn và bao gồm Exception gốc
                    throw new Exception("Lỗi khi lấy danh sách tựa sách từ cơ sở dữ liệu. Xem InnerException để biết chi tiết.", ex);
                }
            }
        }

        // ... Các phương thức GetByIdAsync, GetByMaAsync, GetByTenAsync, IsExists, etc. giữ nguyên ...
        public async Task<Tuasach?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve non-deleted Tuasach with ID: {Id}", id);
            if (id <= 0) { _logger.LogWarning("Invalid ID provided for GetByIdAsync: {Id}", id); return null; }
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Include các navigation property cần thiết
                    var tuasach = await _context.Tuasach
                        .Where(ts => ts.Id == id && ts.DaAn == 0) // Filter first
                        .Include(ts => ts.IdTheLoaiNavigation)
                        .Include(ts => ts.IdTacGia) // Quan trọng để lấy danh sách tác giả hiện tại
                        .Include(ts => ts.Sach) // Nếu cần thông tin sách liên quan
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (tuasach == null) { _logger.LogInformation("Non-deleted Tuasach with ID: {Id} not found.", id); }
                    else { _logger.LogInformation("Successfully retrieved non-deleted Tuasach with ID: {Id}", id); }
                    return tuasach;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALTuaSach.GetByIdAsync (ID: {Id}).", id);
                    throw new Exception($"Lỗi khi lấy tựa sách theo ID: {id}.", ex); // Wrap lỗi
                }
            }
        }
        public async Task<Tuasach?> GetByMaAsync(string maTuaSach)
        {
            _logger.LogInformation("Attempting to retrieve non-deleted Tuasach with MaTuaSach: {MaTuaSach}", maTuaSach);
            if (string.IsNullOrWhiteSpace(maTuaSach)) { _logger.LogWarning("Null or whitespace MaTuaSach provided for GetByMaAsync.", maTuaSach); return null; }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var tuasach = await _context.Tuasach
                        .Where(ts => ts.DaAn == 0 && ts.MaTuaSach != null && ts.MaTuaSach.ToLower() == maTuaSach.Trim().ToLower()) // Filter first
                        .Include(ts => ts.IdTheLoaiNavigation) // Include nếu cần trả về thông tin thể loại
                        .Include(ts => ts.IdTacGia)           // Include nếu cần trả về thông tin tác giả
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                    if (tuasach == null) { _logger.LogInformation("Non-deleted Tuasach with MaTuaSach: {MaTuaSach} not found.", maTuaSach); }
                    else { _logger.LogInformation("Successfully retrieved non-deleted Tuasach with MaTuaSach: {MaTuaSach}.", maTuaSach); }
                    return tuasach;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALTuaSach.GetByMaAsync (Ma: {MaTuaSach}).", maTuaSach);
                    throw new Exception($"Lỗi khi lấy tựa sách theo mã: {maTuaSach}.", ex); // Wrap lỗi
                }
            }
        }

        public async Task<Tuasach?> GetByTenAsync(string tenTuaSach)
        {
            _logger.LogInformation("Attempting to retrieve non-deleted Tuasach with TenTuaSach: {TenTuaSach}", tenTuaSach);
            if (string.IsNullOrWhiteSpace(tenTuaSach)) { _logger.LogWarning("Null or whitespace TenTuaSach provided for GetByTenAsync.", tenTuaSach); return null; }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var tuasach = await _context.Tuasach
                        .Where(ts => ts.DaAn == 0 && ts.TenTuaSach.ToLower() == tenTuaSach.Trim().ToLower()) // Filter first
                        .Include(ts => ts.IdTheLoaiNavigation)
                        .Include(ts => ts.IdTacGia)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (tuasach == null) { _logger.LogInformation("Non-deleted Tuasach with TenTuaSach: {TenTuaSach} not found.", tenTuaSach); }
                    else { _logger.LogInformation("Successfully retrieved non-deleted Tuasach with TenTuaSach: {TenTuaSach}.", tenTuaSach); }

                    return tuasach;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALTuaSach.GetByTenAsync (Ten: {TenTuaSach}).", tenTuaSach);
                    throw new Exception($"Lỗi khi lấy tựa sách theo tên: {tenTuaSach}.", ex);
                }
            }
        }

        public async Task<Tuasach?> GetByMaIncludingDeletedAsync(string maTuaSach)
        {
            _logger.LogInformation("Attempting to retrieve Tuasach (including deleted) with MaTuaSach: {MaTuaSach}", maTuaSach);
            if (string.IsNullOrWhiteSpace(maTuaSach)) { _logger.LogWarning("Null or whitespace MaTuaSach provided for GetByMaIncludingDeletedAsync.", maTuaSach); return null; }
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var tuasach = await _context.Tuasach
                        .Where(ts => ts.MaTuaSach != null && ts.MaTuaSach.ToLower() == maTuaSach.Trim().ToLower()) // Filter first
                         .Include(ts => ts.IdTheLoaiNavigation)
                        .Include(ts => ts.IdTacGia)
                        .AsNoTracking()
                       .FirstOrDefaultAsync();

                    if (tuasach == null) { _logger.LogInformation("Tuasach (including deleted) with MaTuaSach: {MaTuaSach} not found.", maTuaSach); }
                    else { _logger.LogInformation("Successfully retrieved Tuasach (including deleted) with MaTuaSach: {MaTuaSach}.", maTuaSach); }
                    return tuasach;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALTuaSach.GetByMaIncludingDeletedAsync (Ma: {MaTuaSach}).", maTuaSach);
                    throw new Exception($"Lỗi khi lấy tựa sách (bao gồm đã xóa) theo mã: {maTuaSach}.", ex);
                }
            }
        }

        public async Task<Tuasach?> GetByTenIncludingDeletedAsync(string tenTuaSach)
        {
            _logger.LogInformation("Attempting to retrieve Tuasach (including deleted) with TenTuaSach: {TenTuaSach}", tenTuaSach);
            if (string.IsNullOrWhiteSpace(tenTuaSach)) { _logger.LogWarning("Null or whitespace TenTuaSach provided for GetByTenIncludingDeletedAsync.", tenTuaSach); return null; }
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var tuasach = await _context.Tuasach
                        .Where(ts => ts.TenTuaSach.ToLower() == tenTuaSach.Trim().ToLower()) // Filter first
                        .Include(ts => ts.IdTheLoaiNavigation)
                        .Include(ts => ts.IdTacGia)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (tuasach == null) { _logger.LogInformation("Tuasach (including deleted) with TenTuaSach: {TenTuaSach} not found.", tenTuaSach); }
                    else { _logger.LogInformation("Successfully retrieved Tuasach (including deleted) with TenTuaSach: {TenTuaSach}.", tenTuaSach); }
                    return tuasach;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALTuaSach.GetByTenIncludingDeletedAsync (Ten: {TenTuaSach}).", tenTuaSach);
                    throw new Exception($"Lỗi khi lấy tựa sách (bao gồm đã xóa) theo tên: {tenTuaSach}.", ex);
                }
            }
        }

        public async Task<bool> IsTenTuaSachExistsExcludingIdAsync(string tenTuaSach, int excludeId)
        {
            _logger.LogInformation("Checking if TenTuaSach exists (excluding ID {ExcludeId}): {TenTuaSach}", excludeId, tenTuaSach);
            if (string.IsNullOrWhiteSpace(tenTuaSach)) { _logger.LogWarning("Null or whitespace TenTuaSach provided for existence check.", tenTuaSach); return false; }
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Chỉ cần kiểm tra tồn tại, không cần Include
                    bool exists = await _context.Tuasach.AnyAsync(ts => ts.DaAn == 0 && ts.Id != excludeId && ts.TenTuaSach.ToLower() == tenTuaSach.Trim().ToLower());
                    _logger.LogInformation("TenTuaSach '{TenTuaSach}' existence check (excluding ID {ExcludeId}) result: {Exists}", tenTuaSach, excludeId, exists);
                    return exists;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking TenTuaSach existence.", ex);
                    throw new Exception("Lỗi khi kiểm tra sự tồn tại của tên tựa sách.", ex);
                }
            }
        }

        public async Task<bool> IsTenTuaSachExistsAsync(string tenTuaSach)
        {
            _logger.LogInformation("Checking if TenTuaSach exists (non-deleted): {TenTuaSach}", tenTuaSach);
            return await IsTenTuaSachExistsExcludingIdAsync(tenTuaSach, 0); // Check không loại trừ ID nào
        }

        public async Task<bool> IsMaTuaSachExistsExcludingIdAsync(string maTuaSach, int excludeId)
        {
            _logger.LogInformation("Checking if MaTuaSach exists (excluding ID {ExcludeId}): {MaTuaSach}", excludeId, maTuaSach);
            if (string.IsNullOrWhiteSpace(maTuaSach)) { _logger.LogWarning("Null or whitespace MaTuaSach provided for existence check.", maTuaSach); return false; } // Mã rỗng không coi là tồn tại
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Chỉ cần kiểm tra tồn tại, không cần Include
                    bool exists = await _context.Tuasach.AnyAsync(ts => ts.DaAn == 0 && ts.Id != excludeId && ts.MaTuaSach != null && ts.MaTuaSach.ToLower() == maTuaSach.Trim().ToLower());
                    _logger.LogInformation("MaTuaSach '{MaTuaSach}' existence check (excluding ID {ExcludeId}) result: {Exists}", maTuaSach, excludeId, exists);
                    return exists;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking MaTuaSach existence.", ex);
                    throw new Exception("Lỗi khi kiểm tra sự tồn tại của mã tựa sách.", ex);
                }
            }
        }
        public async Task<bool> IsMaTuaSachExistsAsync(string maTuaSach)
        {
            _logger.LogInformation("Checking if MaTuaSach exists (non-deleted): {MaTuaSach}", maTuaSach);
            return await IsMaTuaSachExistsExcludingIdAsync(maTuaSach, 0); // Check không loại trừ ID nào
        }

        public async Task<bool> IsTheLoaiUsedAsync(int theLoaiId)
        {
            _logger.LogInformation("Checking if TheLoai with ID: {TheLoaiId} is used by any non-deleted Tuasach.", theLoaiId);
            if (theLoaiId <= 0) { _logger.LogWarning("Invalid TheLoaiId provided for IsTheLoaiUsedAsync: {TheLoaiId}", theLoaiId); return false; }
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Chỉ cần kiểm tra tồn tại, không cần Include
                    bool isUsed = await _context.Tuasach.AnyAsync(ts => ts.IdTheLoai == theLoaiId && ts.DaAn == 0);
                    _logger.LogInformation("TheLoai ID: {TheLoaiId} used status: {IsUsed}", theLoaiId, isUsed);
                    return isUsed;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking TheLoai usage (ID: {TheLoaiId}).", theLoaiId);
                    throw new Exception($"Lỗi khi kiểm tra thể loại ID {theLoaiId} đang được sử dụng.", ex); // Wrap lỗi
                }
            }
        }

        public async Task<Tuasach?> AddAsync(Tuasach tuaSach)
        {
            _logger.LogInformation("Attempting to add a new Tuasach.");
            if (tuaSach == null) { _logger.LogError("Attempted to add a null Tuasach entity."); throw new ArgumentNullException(nameof(tuaSach)); }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Đảm bảo các navigation property không gây lỗi khi Add
                    tuaSach.IdTheLoaiNavigation = null!; // EF sẽ tự liên kết qua IdTheLoai
                    // Quan trọng: Xử lý Tác giả (M-N) cần logic riêng, thường là ở BUS
                    tuaSach.IdTacGia ??= new List<Tacgia>(); // Khởi tạo rỗng nếu null
                    tuaSach.Sach ??= new List<Sach>();     // Khởi tạo rỗng

                    // Đảm bảo DaAn được set khi thêm mới
                    tuaSach.DaAn = 0; // Mặc định chưa xóa mềm

                    await _context.Tuasach.AddAsync(tuaSach);
                    int affectedRows = await _context.SaveChangesAsync();

                    if (affectedRows > 0)
                    {
                        _logger.LogInformation("Successfully added Tuasach with generated ID: {Id}", tuaSach.Id);
                        return tuaSach; // Trả về entity đã được thêm (có ID)
                    }
                    _logger.LogWarning("AddAsync for Tuasach completed but no rows were affected.");
                    return null; // Thêm thất bại
                }
                catch (DbUpdateException ex) // Bắt lỗi DB, ví dụ UNIQUE constraint
                {
                    _logger.LogError(ex, "DbUpdateException adding Tuasach. Details: {ErrorMessage}", ex.InnerException?.Message ?? ex.Message);
                    if (ex.InnerException?.Message?.Contains("UNIQUE constraint") == true || ex.InnerException?.Message?.Contains("duplicate key") == true)
                    {
                        string conflictField = ex.InnerException?.Message?.Contains("MaTuaSach") == true ? "Mã tựa sách" : (ex.InnerException?.Message?.Contains("TenTuaSach") == true ? "Tên tựa sách" : "Tựa sách");
                        throw new InvalidOperationException($"{conflictField} đã tồn tại. Vui lòng chọn giá trị khác.", ex);
                    }
                    throw new Exception("Lỗi cơ sở dữ liệu khi thêm tựa sách.", ex); // Wrap lỗi DB khác
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error adding Tuasach.", ex);
                    throw new Exception("Lỗi không xác định khi thêm tựa sách.", ex); // Wrap lỗi
                }
            }
        }

        public async Task<bool> UpdateAsync(Tuasach tuaSach) // Chỉ cập nhật scalar properties
        {
            _logger.LogInformation("Attempting to update Tuasach with ID: {Id}", tuaSach?.Id);
            if (tuaSach == null) { _logger.LogError("Attempted to update a null Tuasach entity."); throw new ArgumentNullException(nameof(tuaSach)); }
            if (tuaSach.Id <= 0) { _logger.LogWarning("Invalid ID provided for Tuasach update: {Id}", tuaSach.Id); throw new ArgumentException("ID tựa sách không hợp lệ để cập nhật.", nameof(tuaSach.Id)); }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var existingEntity = await _context.Tuasach.FindAsync(tuaSach.Id);
                    if (existingEntity == null || existingEntity.DaAn != 0)
                    {
                        _logger.LogInformation("Update failed: Tuasach with ID {Id} not found or already deleted.", tuaSach.Id);
                        return false;
                    }

                    // Cập nhật thủ công từng trường:
                    existingEntity.MaTuaSach = tuaSach.MaTuaSach?.Trim();
                    existingEntity.TenTuaSach = tuaSach.TenTuaSach?.Trim();
                    existingEntity.IdTheLoai = tuaSach.IdTheLoai;

                    int affectedRows = await _context.SaveChangesAsync();
                    if (affectedRows > 0) { _logger.LogInformation("Successfully updated Tuasach with ID: {Id}", tuaSach.Id); }
                    else { _logger.LogInformation("UpdateAsync for Tuasach ID: {Id} completed but no rows were affected (potentially no actual changes detected).", tuaSach.Id); }

                    return affectedRows > 0;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error updating Tuasach (ID: {Id}).", tuaSach.Id);
                    throw;
                }
                catch (DbUpdateException ex) // Bắt lỗi DB, ví dụ UNIQUE constraint
                {
                    _logger.LogError(ex, "DbUpdateException updating Tuasach (ID: {Id}). Details: {ErrorMessage}", tuaSach.Id, ex.InnerException?.Message ?? ex.Message);
                    if (ex.InnerException?.Message?.Contains("UNIQUE constraint") == true || ex.InnerException?.Message?.Contains("duplicate key") == true)
                    {
                        string conflictField = ex.InnerException?.Message?.Contains("MaTuaSach") == true ? "Mã tựa sách" : (ex.InnerException?.Message?.Contains("TenTuaSach") == true ? "Tên tựa sách" : "Tựa sách");
                        throw new InvalidOperationException($"{conflictField} đã tồn tại. Vui lòng chọn giá trị khác.", ex);
                    }
                    throw new Exception($"Lỗi cơ sở dữ liệu khi cập nhật tựa sách ID {tuaSach.Id}.", ex); // Wrap lỗi DB khác
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error updating Tuasach (ID: {Id}).", tuaSach.Id);
                    throw new Exception($"Lỗi không xác định khi cập nhật tựa sách ID {tuaSach.Id}.", ex); // Wrap lỗi
                }
            }
        }


        public async Task<bool> SoftDeleteAsync(int id)
        {
            _logger.LogInformation("Attempting to soft delete Tuasach with ID: {Id}", id);
            if (id <= 0) { _logger.LogWarning("Invalid ID provided for SoftDeleteAsync: {Id}", id); throw new ArgumentException("ID tựa sách không hợp lệ để xóa mềm.", nameof(id)); }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var entity = await _context.Tuasach.Include(ts => ts.Sach).FirstOrDefaultAsync(ts => ts.Id == id && ts.DaAn == 0); // Include Sach để kiểm tra
                    if (entity == null) { _logger.LogInformation("Soft delete failed: Tuasach with ID {Id} not found.", id); return false; } // Không tìm thấy hoặc đã xóa

                    // Kiểm tra nghiệp vụ: Còn sách nào thuộc tựa sách này chưa bị xóa mềm không?
                    if (entity.Sach != null && entity.Sach.Any(s => s.DaAn == 0))
                    {
                        _logger.LogWarning("Soft delete failed for Tuasach ID {Id}: Still has active Sach entities.", id);
                        throw new InvalidOperationException($"Không thể xóa mềm tựa sách '{entity.TenTuaSach?.Trim()}' vì vẫn còn sách thuộc tựa sách này chưa bị xóa mềm.");
                    }

                    entity.DaAn = 1; // Đánh dấu đã xóa mềm
                    int affectedRows = await _context.SaveChangesAsync();

                    if (affectedRows > 0) { _logger.LogInformation("Successfully soft deleted Tuasach with ID: {Id}", id); }
                    else { _logger.LogInformation("Soft delete failed: Tuasach with ID {Id} had no changes detected.", id); }
                    return affectedRows > 0;
                }
                catch (InvalidOperationException ex) // Bắt lỗi nghiệp vụ cụ thể
                {
                    throw; // Ném lại để BUS xử lý
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error soft deleting Tuasach (ID: {Id}).", id);
                    throw;
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "DbUpdateException soft deleting Tuasach (ID: {Id}). Details: {ErrorMessage}", id, ex.InnerException?.Message ?? ex.Message);
                    throw new Exception($"Lỗi cơ sở dữ liệu khi xóa mềm tựa sách ID {id}.", ex); // Wrap lỗi
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error soft deleting Tuasach (ID: {Id}).", id);
                    throw new Exception($"Lỗi không xác định khi xóa mềm tựa sách ID {id}.", ex); // Wrap lỗi
                }
            }
        }


        public async Task<bool> RestoreAsync(int id)
        {
            _logger.LogInformation("Attempting to restore Tuasach with ID: {Id}", id);
            if (id <= 0) { _logger.LogWarning("Invalid ID provided for RestoreAsync: {Id}", id); throw new ArgumentException("ID tựa sách không hợp lệ để phục hồi.", nameof(id)); }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var entity = await _context.Tuasach.FirstOrDefaultAsync(ts => ts.Id == id && ts.DaAn != 0); // Tìm entity ĐÃ bị xóa mềm
                    if (entity == null) { _logger.LogInformation("Restore failed: Tuasach with ID {Id} not found or not soft-deleted.", id); return false; } // Không tìm thấy hoặc chưa bị xóa

                    entity.DaAn = 0; // Phục hồi
                    int affectedRows = await _context.SaveChangesAsync();
                    if (affectedRows > 0) { _logger.LogInformation("Successfully restored Tuasach with ID: {Id}", id); }
                    else { _logger.LogInformation("Restore failed: Tuasach with ID {Id} had no changes detected.", id); }

                    return affectedRows > 0;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error restoring Tuasach (ID: {Id}).", id);
                    throw;
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "DbUpdateException restoring Tuasach (ID: {Id}). Details: {ErrorMessage}", id, ex.InnerException?.Message ?? ex.Message);
                    throw new Exception($"Lỗi cơ sở dữ liệu khi phục hồi tựa sách ID {id}.", ex); // Wrap lỗi
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error restoring Tuasach (ID: {Id}).", id);
                    throw new Exception($"Lỗi không xác định khi phục hồi tựa sách ID {id}.", ex); // Wrap lỗi
                }
            }
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            _logger.LogWarning("Attempting to HARD DELETE Tuasach with ID: {Id}. This is irreversible!", id);
            if (id <= 0) { _logger.LogError("Invalid ID provided for HardDeleteAsync: {Id}", id); throw new ArgumentException("ID tựa sách không hợp lệ để xóa vĩnh viễn.", nameof(id)); }


            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var entity = await _context.Tuasach
                         .Include(ts => ts.Sach) // Bao gồm Sach
                         .Include(ts => ts.IdTacGia) // Bao gồm liên kết M-N với Tacgia
                         .FirstOrDefaultAsync(ts => ts.Id == id);

                    if (entity == null)
                    {
                        _logger.LogInformation("Hard delete failed: Tuasach with ID {Id} not found.", id);
                        await transaction.RollbackAsync();
                        return false;
                    }

                    // Kiểm tra ràng buộc trước khi xóa vĩnh viễn
                    if (entity.Sach != null && entity.Sach.Any())
                    {
                        _logger.LogWarning("Hard delete failed for Tuasach ID {Id}: Still has related Sach entities.", id);
                        await transaction.RollbackAsync();
                        throw new InvalidOperationException($"Không thể xóa vĩnh viễn tựa sách '{entity.TenTuaSach?.Trim()}' vì vẫn còn sách thuộc tựa sách này.");
                    }
                    // Kiểm tra các liên kết M-N với Tác giả (IdTacGia)
                    if (entity.IdTacGia != null)
                    {
                        entity.IdTacGia.Clear(); // Xóa TẤT CẢ các tác giả khỏi tựa sách
                        _logger.LogInformation("Removed Tacgia relationships for Tuasach ID: {Id}.", id);
                    }

                    _context.Tuasach.Remove(entity); // Đánh dấu để xóa vĩnh viễn tựa sách
                    int affectedRows = await _context.SaveChangesAsync(); // Lưu các thay đổi
                    await transaction.CommitAsync(); // Hoàn tất transaction

                    if (affectedRows > 0) { _logger.LogWarning("Successfully HARD DELETED Tuasach with ID: {Id}", id); }
                    else { _logger.LogInformation("Hard delete failed: Tuasach with ID {Id} had no changes detected after removing join entities.", id); }

                    return affectedRows > 0; // True nếu có dòng bị ảnh hưởng (tựa sách)
                }
                catch (InvalidOperationException)
                {
                    await transaction.RollbackAsync();
                    throw; // Ném lại lỗi nghiệp vụ đã bắt hoặc lỗi FK từ SaveChanges
                }
                catch (DbUpdateException ex) // Bắt lỗi DB khác
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "DbUpdateException during hard delete of Tuasach (ID: {Id}). Might be due to FK constraints. Details: {ErrorMessage}", id, ex.InnerException?.Message ?? ex.Message);
                    throw new InvalidOperationException($"Không thể xóa vĩnh viễn tựa sách ID {id} do có dữ liệu liên quan.", ex);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Unexpected error hard deleting Tuasach (ID: {Id}). Details: {ErrorMessage}", id, ex.Message);
                    throw new Exception($"Lỗi không xác định khi xóa vĩnh viễn tựa sách ID {id}.", ex); // Wrap lỗi
                }
            }
        }

        public async Task<List<Tacgia>> GetTacGiasByIdsAsync(List<int> tacGiaIds)
        {
            _logger.LogInformation("Attempting to retrieve Tacgia entities by IDs.");
            if (tacGiaIds == null || !tacGiaIds.Any()) { _logger.LogWarning("GetTacGiasByIdsAsync called with null or empty ID list."); return new List<Tacgia>(); }
            _logger.LogInformation("Retrieving Tacgia entities with IDs: {Ids}", string.Join(", ", tacGiaIds));
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var tacGias = await _context.Tacgia
                                .AsNoTracking()
                                .Where(tg => tacGiaIds.Contains(tg.Id))
                                .ToListAsync();
                    _logger.LogInformation("Successfully retrieved {Count} Tacgia entities by IDs.", tacGias.Count);
                    return tacGias;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting TacGias by IDs.", ex);
                    throw new Exception("Lỗi khi lấy danh sách tác giả theo ID.", ex); // Wrap lỗi
                }
            }
        }

        // --- TRIỂN KHAI PHƯƠNG THỨC UPDATE TÁC GIẢ CHO TỰA SÁCH (M-N) ---
        public async Task<bool> UpdateTuaSachTacGiasAsync(int tuaSachId, IEnumerable<int> newTacGiaIds)
        {
            _logger.LogInformation("Attempting to update Tacgia relationship for Tuasach ID: {TuaSachId}", tuaSachId);
            if (tuaSachId <= 0) { _logger.LogWarning("Invalid TuaSachId provided for UpdateTuaSachTacGiasAsync: {TuaSachId}", tuaSachId); return false; }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                using var transaction = await _context.Database.BeginTransactionAsync(); // Dùng transaction cho an toàn

                try
                {
                    // Lấy Tựa sách hiện tại và bao gồm collection Tác giả hiện tại
                    var tuaSach = await _context.Tuasach
                        .Include(ts => ts.IdTacGia) // Quan trọng: Phải Include collection Tác giả hiện tại
                        .FirstOrDefaultAsync(ts => ts.Id == tuaSachId);

                    if (tuaSach == null)
                    {
                        _logger.LogInformation("Update failed for Tuasach ID {TuaSachId}: Tuasach not found.", tuaSachId);
                        await transaction.RollbackAsync();
                        return false; // Tựa sách không tồn tại
                    }

                    // Lấy danh sách các đối tượng Tác giả MỚI từ DB dựa trên newTacGiaIds
                    var tacGiasToAdd = new List<Tacgia>();
                    var newTacGiaIdList = newTacGiaIds?.ToList() ?? new List<int>();

                    if (newTacGiaIdList.Any())
                    {
                        tacGiasToAdd = await _context.Tacgia
                            .Where(tg => newTacGiaIdList.Contains(tg.Id))
                            .ToListAsync();
                        // Kiểm tra xem tất cả các IDs mới có tồn tại trong DB không
                        if (tacGiasToAdd.Count != newTacGiaIdList.Count)
                        {
                            var missingIds = newTacGiaIdList.Except(tacGiasToAdd.Select(tg => tg.Id));
                            _logger.LogWarning("Update failed for Tuasach ID {TuaSachId}: Some new Tacgia IDs not found: {MissingIds}", tuaSachId, string.Join(", ", missingIds));
                            await transaction.RollbackAsync();
                            throw new ArgumentException($"Một hoặc nhiều ID tác giả được cung cấp không tồn tại.", nameof(newTacGiaIds));
                        }
                    }

                    // Cập nhật collection Tác giả của Tựa sách
                    tuaSach.IdTacGia.Clear(); // Xóa các tác giả cũ khỏi liên kết
                    foreach (var tacGia in tacGiasToAdd)
                    {
                        tuaSach.IdTacGia.Add(tacGia); // Thêm các tác giả mới vào liên kết
                    }
                    _logger.LogInformation("Updated Tacgia collection for Tuasach ID {TuaSachId}. Adding {ToAddCount}.", tuaSachId, tacGiasToAdd.Count);

                    // Lưu thay đổi vào DB
                    int affectedRows = await _context.SaveChangesAsync();
                    await transaction.CommitAsync(); // Hoàn tất transaction

                    _logger.LogInformation("Successfully updated Tacgia relationship for Tuasach ID: {TuaSachId}. {AffectedRows} database rows affected.", tuaSachId, affectedRows);
                    return true; // Trả về true bất kể affectedRows > 0 hay không, vì Clear() cũng là thay đổi hợp lệ
                }
                catch (ArgumentException)
                {
                    await transaction.RollbackAsync();
                    throw; // Ném lại lỗi validation đã bắt
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Concurrency error updating TuaSach-TacGia relationship (TuaSachID: {TuaSachId}).", tuaSachId);
                    throw; // Ném lại lỗi tương tranh
                }
                catch (DbUpdateException ex) // Bắt lỗi DB, ví dụ FK constraints
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "DbUpdateException updating TuaSach-TacGia relationship (TuaSachID: {TuaSachId}). Details: {ErrorMessage}", tuaSachId, ex.InnerException?.Message ?? ex.Message);
                    throw new Exception($"Lỗi cơ sở dữ liệu khi cập nhật tác giả cho tựa sách ID {tuaSachId}.", ex); // Wrap lỗi
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Unexpected error updating TuaSach-TacGia relationship (TuaSachID: {TuaSachId}).", tuaSachId);
                    throw new Exception($"Lỗi không xác định khi cập nhật tác giả cho tựa sách ID {tuaSachId}.", ex); // Wrap lỗi
                }
            }
        }
        // ---------------------------------

        // Phương thức đếm tổng số tựa sách
        public async Task<int> GetTotalCountAsync()
        {
            _logger.LogInformation("Attempting to get total count of non-deleted Tuasach entities.");
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    int count = await _context.Tuasach.CountAsync(ts => ts.DaAn == 0);
                    _logger.LogInformation("Total count of non-deleted Tuasach entities: {Count}", count);
                    return count;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALTuaSach.GetTotalCountAsync.");
                    throw new Exception("Lỗi khi đếm tổng số tựa sách.", ex);
                }
            }
        }

        // --- PHƯƠNG THỨC TÌM KIẾM VÀ LỌC (ĐÃ SỬA LỖI TYPE CAST) ---
        public async Task<List<Tuasach>> SearchAndFilterAsync(string? searchText, int theLoaiId, int tacGiaId)
        {
            _logger.LogInformation("DALTuaSach: Searching/Filtering. Search='{SearchText}', TheLoaiId={TheLoaiId}, TacGiaId={TacGiaId}", searchText, theLoaiId, tacGiaId);
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    _logger.LogInformation("Building dynamic query for SearchAndFilterAsync...");
                    // Bắt đầu với IQueryable<Tuasach>
                    IQueryable<Tuasach> query = _context.Tuasach;

                    // Áp dụng các bộ lọc Where trước
                    query = query.Where(ts => ts.DaAn == 0); // Luôn lọc theo DaAn = 0

                    // Apply Search Filter
                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        string searchLower = searchText.Trim().ToLower();
                        query = query.Where(ts =>
                            (ts.MaTuaSach != null && ts.MaTuaSach.ToLower().Contains(searchLower)) ||
                            (ts.TenTuaSach.ToLower().Contains(searchLower))
                        );
                        _logger.LogInformation("Applied search filter: '{SearchText}'", searchText);
                    }

                    // Apply TheLoai Filter (ignore if Id <= 0 which means 'All')
                    if (theLoaiId > 0)
                    {
                        query = query.Where(ts => ts.IdTheLoai == theLoaiId);
                        _logger.LogInformation("Applied TheLoai filter: ID={TheLoaiId}", theLoaiId);
                    }

                    // Apply TacGia Filter (ignore if Id <= 0 which means 'All')
                    if (tacGiaId > 0)
                    {
                        // Lọc các Tựa sách mà collection IdTacGia chứa một Tác giả có Id trùng khớp
                        // Cần Include IdTacGia trước khi dùng Any() trong Where nếu không EF Core < 7 có thể không dịch được
                        // Tuy nhiên, với EF Core 7+, có thể để Include sau. Để an toàn và rõ ràng, ta Include trước khi ToList.
                        // Hoặc cách khác là lọc trên IdTacGia.Id trực tiếp nếu cấu trúc DB cho phép (không khuyến khích nếu M-N)
                        // -> Cách tốt nhất là giữ Include sau cùng trước ToList.
                        query = query.Where(ts => ts.IdTacGia.Any(tg => tg.Id == tacGiaId));
                        _logger.LogInformation("Applied TacGia filter: ID={TacGiaId}", tacGiaId);
                    }

                    // Áp dụng Include và AsNoTracking sau khi đã lọc
                    var finalQuery = query
                        .Include(ts => ts.IdTheLoaiNavigation)
                        .Include(ts => ts.IdTacGia) // Include tác giả để lọc và hiển thị
                        .Include(ts => ts.Sach)     // Include sách để tính số lượng
                        .AsNoTracking();

                    _logger.LogInformation("Executing SearchAndFilterAsync query...");
                    var result = await finalQuery.ToListAsync(); // Thực thi truy vấn cuối cùng
                    _logger.LogInformation("SearchAndFilterAsync retrieved {Count} entities.", result.Count);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALTuaSach.SearchAndFilterAsync.");
                    throw new Exception("Lỗi khi tìm kiếm và lọc tựa sách từ cơ sở dữ liệu.", ex);
                }
            }
        }
        // ---------------------------------

    } // End class DALTuaSach
} // End namespace DAL
