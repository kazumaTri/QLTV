// Project/Namespace: DAL
using DAL.Models; // Namespace chứa các Entity models (Tacgia, Tuasach, QLTVContext)
using Microsoft.EntityFrameworkCore; // Cần cho DbContext, các hàm async của EF (Include, Where, FirstOrDefaultAsync, ToListAsync, AnyAsync)
using Microsoft.Extensions.DependencyInjection; // Cần cho IServiceScopeFactory
using System; // Cần cho Exception, ArgumentNullException, InvalidOperationException
using System.Collections.Generic; // Cần cho List
using System.Linq; // Cần cho LINQ (Where)
using System.Threading.Tasks; // Cần cho async/await Task

namespace DAL
{
    /// <summary>
    /// Data Access Layer triển khai IDALTacGia, tương tác với DB thông qua EF Core.
    /// Refactor để sử dụng IServiceScopeFactory để quản lý vòng đời DbContext cho từng thao tác.
    /// Sử dụng tên thuộc tính Matacgia (chữ 'm' thường) theo Entity model.
    /// **Cập nhật:** Đã triển khai Soft Delete (DaAn), Restore, và các phương thức lấy dữ liệu bao gồm cả đã xóa.
    /// </summary>
    public class DALTacGia : IDALTacGia // <<< Implement interface
    {
        // --- DEPENDENCIES ---
        private readonly IServiceScopeFactory _scopeFactory;

        // --- CONSTRUCTOR (Sử dụng Dependency Injection) ---
        public DALTacGia(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        // --- METHOD IMPLEMENTATIONS (Từ IDALTacGia) ---

        // Lấy tất cả tác giả (CHƯA bị xóa mềm)
        public async Task<List<Tacgia>> GetAllAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var query = _context.Tacgia.AsNoTracking();

                    // *** LỌC SOFT DELETE ***
                    query = query.Where(tg => !tg.DaAn); // Chỉ lấy những tác giả chưa bị ẩn

                    return await query.OrderBy(tg => tg.TenTacGia).ToListAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in DALTacGia.GetAllAsync: {ex.Message}");
                    throw;
                }
            }
        }

        // Lấy TẤT CẢ tác giả (BAO GỒM cả đã xóa mềm)
        public async Task<List<Tacgia>> GetAllIncludingDeletedAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // KHÔNG lọc DaAn ở đây
                    return await _context.Tacgia.AsNoTracking().OrderBy(tg => tg.TenTacGia).ToListAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in DALTacGia.GetAllIncludingDeletedAsync: {ex.Message}");
                    throw;
                }
            }
        }


        // Tìm kiếm tác giả theo Mã hoặc Tên (CHƯA bị xóa mềm)
        public async Task<List<Tacgia>> SearchAsync(string searchTerm)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var query = _context.Tacgia.AsNoTracking();

                    // *** LỌC SOFT DELETE ***
                    query = query.Where(tg => !tg.DaAn); // Chỉ tìm trong những tác giả chưa bị ẩn

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        string lowerSearchTerm = searchTerm.Trim().ToLower();
                        query = query.Where(tg =>
                            (tg.Matacgia != null && tg.Matacgia.ToLower().Contains(lowerSearchTerm)) ||
                            (tg.TenTacGia != null && tg.TenTacGia.ToLower().Contains(lowerSearchTerm))
                        );
                    }
                    return await query.OrderBy(tg => tg.TenTacGia).ToListAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in DALTacGia.SearchAsync (SearchTerm: '{searchTerm}'): {ex.Message}");
                    throw;
                }
            }
        }

        // Tìm kiếm TẤT CẢ tác giả theo Mã hoặc Tên (BAO GỒM cả đã xóa mềm)
        public async Task<List<Tacgia>> SearchIncludingDeletedAsync(string searchTerm)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var query = _context.Tacgia.AsNoTracking();

                    // KHÔNG lọc DaAn ở đây

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        string lowerSearchTerm = searchTerm.Trim().ToLower();
                        query = query.Where(tg =>
                            (tg.Matacgia != null && tg.Matacgia.ToLower().Contains(lowerSearchTerm)) ||
                            (tg.TenTacGia != null && tg.TenTacGia.ToLower().Contains(lowerSearchTerm))
                        );
                    }
                    return await query.OrderBy(tg => tg.TenTacGia).ToListAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in DALTacGia.SearchIncludingDeletedAsync (SearchTerm: '{searchTerm}'): {ex.Message}");
                    throw;
                }
            }
        }

        // Lấy tác giả theo ID (chỉ trả về nếu CHƯA bị xóa mềm)
        public async Task<Tacgia?> GetByIdAsync(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var query = _context.Tacgia
                        .AsNoTracking()
                        .Where(tg => tg.Id == id);

                    // *** LỌC SOFT DELETE ***
                    query = query.Where(tg => !tg.DaAn); // Chỉ lấy nếu chưa bị ẩn

                    return await query.FirstOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in DALTacGia.GetByIdAsync (ID: {id}): {ex.Message}");
                    throw;
                }
            }
        }

        // Lấy tác giả theo Mã (chỉ trả về nếu CHƯA bị xóa mềm)
        public async Task<Tacgia?> GetByMaAsync(string maTacGia)
        {
            if (string.IsNullOrWhiteSpace(maTacGia)) return null;
            string maTrimmedLower = maTacGia.Trim().ToLower();

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var query = _context.Tacgia
                        .AsNoTracking()
                        .Where(tg => tg.Matacgia != null && tg.Matacgia.ToLower() == maTrimmedLower);

                    // *** LỌC SOFT DELETE ***
                    query = query.Where(tg => !tg.DaAn); // Chỉ lấy nếu chưa bị ẩn

                    return await query.FirstOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in DALTacGia.GetByMaAsync (Ma: {maTacGia}): {ex.Message}");
                    throw;
                }
            }
        }

        // Lấy tác giả theo Tên (chỉ trả về nếu CHƯA bị xóa mềm)
        public async Task<Tacgia?> GetByTenAsync(string tenTacGia)
        {
            if (string.IsNullOrWhiteSpace(tenTacGia)) return null;
            string tenTrimmedLower = tenTacGia.Trim().ToLower();

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var query = _context.Tacgia
                       .AsNoTracking()
                       .Where(tg => tg.TenTacGia != null && tg.TenTacGia.ToLower() == tenTrimmedLower);

                    // *** LỌC SOFT DELETE ***
                    query = query.Where(tg => !tg.DaAn); // Chỉ lấy nếu chưa bị ẩn

                    return await query.FirstOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in DALTacGia.GetByTenAsync (Ten: {tenTacGia}): {ex.Message}");
                    throw;
                }
            }
        }

        // Kiểm tra sự tồn tại theo Mã Tác giả (chỉ kiểm tra các tác giả CHƯA bị xóa mềm)
        public async Task<bool> IsMaTacGiaExistsAsync(string maTacGia)
        {
            if (string.IsNullOrWhiteSpace(maTacGia)) return false;
            string maTrimmedLower = maTacGia.Trim().ToLower();
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var query = _context.Tacgia.Where(tg => tg.Matacgia != null && tg.Matacgia.ToLower() == maTrimmedLower);

                    // *** LỌC SOFT DELETE ***
                    query = query.Where(tg => !tg.DaAn); // Chỉ kiểm tra trong những tác giả chưa bị ẩn

                    return await query.AnyAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in DALTacGia.IsMaTacGiaExistsAsync (Ma: {maTacGia}): {ex.Message}");
                    throw;
                }
            }
        }

        // Kiểm tra sự tồn tại theo Mã Tác giả (loại trừ một ID, chỉ kiểm tra các tác giả CHƯA bị xóa mềm)
        public async Task<bool> IsMaTacGiaExistsExcludingIdAsync(string maTacGia, int excludeId)
        {
            if (string.IsNullOrWhiteSpace(maTacGia)) return false;
            string maTrimmedLower = maTacGia.Trim().ToLower();
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var query = _context.Tacgia.Where(tg => tg.Id != excludeId && tg.Matacgia != null && tg.Matacgia.ToLower() == maTrimmedLower);

                    // *** LỌC SOFT DELETE ***
                    query = query.Where(tg => !tg.DaAn); // Chỉ kiểm tra trong những tác giả chưa bị ẩn

                    return await query.AnyAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in DALTacGia.IsMaTacGiaExistsExcludingIdAsync (Ma: {maTacGia}, ExcludeId: {excludeId}): {ex.Message}");
                    throw;
                }
            }
        }

        // Kiểm tra sự tồn tại theo Tên Tác giả (chỉ kiểm tra các tác giả CHƯA bị xóa mềm)
        public async Task<bool> IsTenTacGiaExistsAsync(string tenTacGia)
        {
            if (string.IsNullOrWhiteSpace(tenTacGia)) return false;
            string tenTrimmedLower = tenTacGia.Trim().ToLower();
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var query = _context.Tacgia.Where(tg => tg.TenTacGia != null && tg.TenTacGia.ToLower() == tenTrimmedLower);

                    // *** LỌC SOFT DELETE ***
                    query = query.Where(tg => !tg.DaAn); // Chỉ kiểm tra trong những tác giả chưa bị ẩn

                    return await query.AnyAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in DALTacGia.IsTenTacGiaExistsAsync (Ten: {tenTacGia}): {ex.Message}");
                    throw;
                }
            }
        }

        // Kiểm tra sự tồn tại theo Tên Tác giả (loại trừ một ID, chỉ kiểm tra các tác giả CHƯA bị xóa mềm)
        public async Task<bool> IsTenTacGiaExistsExcludingIdAsync(string tenTacGia, int excludeId)
        {
            if (string.IsNullOrWhiteSpace(tenTacGia)) return false;
            string tenTrimmedLower = tenTacGia.Trim().ToLower();
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var query = _context.Tacgia.Where(tg => tg.Id != excludeId && tg.TenTacGia != null && tg.TenTacGia.ToLower() == tenTrimmedLower);

                    // *** LỌC SOFT DELETE ***
                    query = query.Where(tg => !tg.DaAn); // Chỉ kiểm tra trong những tác giả chưa bị ẩn

                    return await query.AnyAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in DALTacGia.IsTenTacGiaExistsExcludingIdAsync (Ten: {tenTacGia}, ExcludeId: {excludeId}): {ex.Message}");
                    throw;
                }
            }
        }

        // Thêm mới tác giả
        public async Task<Tacgia?> AddAsync(Tacgia tacGia)
        {
            if (tacGia == null) throw new ArgumentNullException(nameof(tacGia));

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();

                // *** ĐẢM BẢO DaAn LÀ FALSE KHI THÊM MỚI ***
                tacGia.DaAn = false;

                // Xử lý navigation property
                if (tacGia.IdTuaSach != null)
                {
                    tacGia.IdTuaSach = new List<Tuasach>();
                }

                try
                {
                    await _context.Tacgia.AddAsync(tacGia);
                    int affectedRows = await _context.SaveChangesAsync();

                    if (affectedRows > 0)
                    {
                        return tacGia; // Trả về entity (giờ có ID)
                    }
                    return null;
                }
                catch (DbUpdateException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"DbUpdateException adding Tacgia: {ex.InnerException?.Message ?? ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Unexpected error adding Tacgia: {ex.Message}");
                    throw;
                }
            }
        }

        // Cập nhật tác giả
        public async Task<bool> UpdateAsync(Tacgia tacGia)
        {
            if (tacGia == null) throw new ArgumentNullException(nameof(tacGia));
            if (tacGia.Id <= 0) throw new ArgumentException("ID tác giả không hợp lệ để cập nhật.", nameof(tacGia.Id));

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Lấy entity đang tồn tại từ DB để cập nhật
                    var existingTacgia = await _context.Tacgia
                                                 // *** LỌC SOFT DELETE ***
                                                 // Chỉ cho phép sửa nếu chưa bị xóa mềm
                                                 .Where(tg => !tg.DaAn)
                                                 .FirstOrDefaultAsync(tg => tg.Id == tacGia.Id);

                    if (existingTacgia == null)
                    {
                        // Không tìm thấy hoặc đã bị xóa mềm
                        return false;
                    }

                    // Cập nhật các thuộc tính scalar
                    existingTacgia.Matacgia = tacGia.Matacgia;
                    existingTacgia.TenTacGia = tacGia.TenTacGia;
                    // Không cập nhật DaAn ở đây, dùng RestoreAsync để bỏ ẩn

                    _context.Entry(existingTacgia).State = EntityState.Modified;
                    int affectedRows = await _context.SaveChangesAsync();
                    return affectedRows > 0;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"DbUpdateConcurrencyException updating Tacgia (ID: {tacGia.Id}): {ex.Message}");
                    throw;
                }
                catch (DbUpdateException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"DbUpdateException updating Tacgia (ID: {tacGia.Id}): {ex.InnerException?.Message ?? ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Unexpected error updating Tacgia (ID: {tacGia.Id}): {ex.Message}");
                    throw;
                }
            }
        }

        // *** PHƯƠNG THỨC SOFT DELETE ***
        public async Task<bool> SoftDeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID tác giả không hợp lệ để xóa.", nameof(id));

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var tacgiaToSoftDelete = await _context.Tacgia.FirstOrDefaultAsync(tg => tg.Id == id);

                    if (tacgiaToSoftDelete == null)
                    {
                        return false; // Không tìm thấy
                    }

                    if (tacgiaToSoftDelete.DaAn)
                    {
                        return true; // Đã bị ẩn rồi, coi như thành công
                    }

                    tacgiaToSoftDelete.DaAn = true; // Đánh dấu là đã xóa (ẩn)
                    _context.Entry(tacgiaToSoftDelete).State = EntityState.Modified;
                    int affectedRows = await _context.SaveChangesAsync();
                    return affectedRows > 0;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error soft deleting Tacgia (ID: {id}): {ex.Message}");
                    throw;
                }
            }
        }

        // *** PHƯƠNG THỨC KHÔI PHỤC (RESTORE) ***
        public async Task<bool> RestoreAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID tác giả không hợp lệ để khôi phục.", nameof(id));

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var tacgiaToRestore = await _context.Tacgia.FirstOrDefaultAsync(tg => tg.Id == id);

                    if (tacgiaToRestore == null)
                    {
                        return false; // Không tìm thấy
                    }

                    if (!tacgiaToRestore.DaAn)
                    {
                        return true; // Không bị ẩn, coi như thành công
                    }

                    tacgiaToRestore.DaAn = false; // Đánh dấu là không bị ẩn
                    _context.Entry(tacgiaToRestore).State = EntityState.Modified;
                    int affectedRows = await _context.SaveChangesAsync();
                    return affectedRows > 0;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error restoring Tacgia (ID: {id}): {ex.Message}");
                    throw;
                }
            }
        }


        // Xóa vĩnh viễn tác giả (Hard Delete - Sử dụng cẩn thận!)
        public async Task<bool> HardDeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID tác giả không hợp lệ để xóa vĩnh viễn.", nameof(id));

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var tacgiaToRemove = await _context.Tacgia
                         .Include(tg => tg.IdTuaSach) // Include để kiểm tra ràng buộc
                         .FirstOrDefaultAsync(tg => tg.Id == id);

                    if (tacgiaToRemove == null)
                    {
                        return false; // Không tìm thấy
                    }

                    // Kiểm tra ràng buộc trước khi xóa vĩnh viễn
                    if (tacgiaToRemove.IdTuaSach != null && tacgiaToRemove.IdTuaSach.Any())
                    {
                        throw new InvalidOperationException($"Không thể xóa vĩnh viễn tác giả '{tacgiaToRemove.TenTacGia}' (ID: {id}) vì còn liên kết với {tacgiaToRemove.IdTuaSach.Count} tựa sách.");
                    }

                    _context.Tacgia.Remove(tacgiaToRemove);
                    int affectedRows = await _context.SaveChangesAsync();
                    return affectedRows > 0;
                }
                catch (InvalidOperationException ex) // Bắt lỗi nghiệp vụ đã throw
                {
                    System.Diagnostics.Debug.WriteLine($"FK Violation hard deleting Tacgia (ID: {id}): {ex.Message}");
                    throw;
                }
                catch (DbUpdateException ex) // Bắt lỗi DB khác
                {
                    System.Diagnostics.Debug.WriteLine($"DbUpdateException hard deleting Tacgia (ID: {id}): {ex.InnerException?.Message ?? ex.Message}");
                    throw new InvalidOperationException($"Không thể xóa vĩnh viễn tác giả (ID: {id}) do lỗi cơ sở dữ liệu. Chi tiết: {ex.InnerException?.Message ?? ex.Message}", ex);
                }
                catch (Exception ex) // Bắt lỗi không mong muốn khác
                {
                    System.Diagnostics.Debug.WriteLine($"Unexpected error hard deleting Tacgia (ID: {id}): {ex.Message}");
                    throw;
                }
            }
        }

    } // Kết thúc class DALTacGia
} // Kết thúc namespace DAL
