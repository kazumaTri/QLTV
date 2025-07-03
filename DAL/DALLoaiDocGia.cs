// --- USING ---
using DAL.Models; // Cần using Models cho các Entity (Loaidocgia, Docgia)
using Microsoft.EntityFrameworkCore; // Cần cho DbContext, các hàm Async của EF (ToListAsync, FindAsync, AnyAsync, SaveChangesAsync, DbUpdateException, CountAsync)
using System; // Cần cho ArgumentNullException, InvalidOperationException, Console.WriteLine, Exception, Func<>
using System.Collections.Generic; // Cần cho List
using System.Diagnostics;       // Cần cho Debug
using System.Linq; // Cần cho LINQ (Where, ToLower, AnyAsync, OrderBy, OrderByDescending, Select, Skip, Take, StartsWith)
using System.Linq.Expressions; // Cần cho Expression<> trong AnyAsync
using System.Threading.Tasks; // Cần cho Task
using Microsoft.Extensions.DependencyInjection; // <<< Cần cho IServiceScopeFactory

namespace DAL
{
    /// <summary>
    /// Data Access Layer cho Loại Độc Giả.
    /// Xử lý việc truy cập dữ liệu trực tiếp từ cơ sở dữ liệu thông qua Entity Framework Core.
    /// Đã chuyển sang mô hình nhận IServiceScopeFactory và triển khai IDALLoaiDocGia đầy đủ.
    /// </summary>
    public class DALLoaiDocGia : IDALLoaiDocGia // <<< Implement interface IDALLoaiDocGia
    {
        // --- DEPENDENCIES ---
        private readonly IServiceScopeFactory _scopeFactory;

        // --- CONSTRUCTOR (Sử dụng Dependency Injection) ---
        public DALLoaiDocGia(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        // --- Phương thức truy cập dữ liệu (Triển khai từ IDALLoaiDocGia) ---

        /// <inheritdoc/>
        public async Task<List<Loaidocgia>> GetAllAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    return await _context.Loaidocgia.AsNoTracking().ToListAsync(); // Sử dụng AsNoTracking() cho các truy vấn chỉ đọc
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALLoaiDocGia.GetAllAsync: {ex.Message}");
                    throw; // Ném lại lỗi để tầng BUS bắt
                }
            }
        }

        /// <inheritdoc/>
        public async Task<Loaidocgia?> GetByIdAsync(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    return await _context.Loaidocgia.AsNoTracking().FirstOrDefaultAsync(ldg => ldg.Id == id);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALLoaiDocGia.GetByIdAsync (ID: {id}): {ex.Message}");
                    throw;
                }
            }
        }

        /// <inheritdoc/>
        public async Task<Loaidocgia?> GetByMaAsync(string maLoaiDocGia)
        {
            if (string.IsNullOrWhiteSpace(maLoaiDocGia)) return null;

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var normalizedMa = maLoaiDocGia.Trim().ToLower();
                    return await _context.Loaidocgia.AsNoTracking()
                                 .FirstOrDefaultAsync(ldg => ldg.MaLoaiDocGia != null && ldg.MaLoaiDocGia.ToLower() == normalizedMa);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALLoaiDocGia.GetByMaAsync (Ma: {maLoaiDocGia}): {ex.Message}");
                    throw;
                }
            }
        }

        /// <inheritdoc/>
        public async Task<bool> AddAsync(Loaidocgia loaiDocGia)
        {
            if (loaiDocGia == null) throw new ArgumentNullException(nameof(loaiDocGia));
            loaiDocGia.Id = 0; // Đảm bảo ID là 0 khi thêm mới

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                await _context.Loaidocgia.AddAsync(loaiDocGia);
                try
                {
                    return await _context.SaveChangesAsync() > 0;
                }
                catch (DbUpdateException ex)
                {
                    Debug.WriteLine($"Error adding Loaidocgia: {ex.InnerException?.Message ?? ex.Message}");
                    throw; // Ném lại lỗi
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unexpected error adding Loaidocgia: {ex.Message}");
                    throw;
                }
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateAsync(Loaidocgia loaiDocGia)
        {
            if (loaiDocGia == null) throw new ArgumentNullException(nameof(loaiDocGia));
            if (loaiDocGia.Id <= 0) throw new ArgumentException("ID không hợp lệ để cập nhật.", nameof(loaiDocGia.Id));

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                _context.Entry(loaiDocGia).State = EntityState.Modified;
                // Đảm bảo không sửa Mã nếu logic yêu cầu
                _context.Entry(loaiDocGia).Property(p => p.MaLoaiDocGia).IsModified = false;

                try
                {
                    return await _context.SaveChangesAsync() > 0;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Debug.WriteLine($"Concurrency error updating Loaidocgia ID {loaiDocGia.Id}: {ex.Message}");
                    throw new InvalidOperationException("Dữ liệu đã bị thay đổi bởi người khác. Vui lòng tải lại.", ex);
                }
                catch (DbUpdateException ex)
                {
                    Debug.WriteLine($"Error updating Loaidocgia ID {loaiDocGia.Id}: {ex.InnerException?.Message ?? ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unexpected error updating Loaidocgia ID {loaiDocGia.Id}: {ex.Message}");
                    throw;
                }
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID không hợp lệ để xóa.", nameof(id));

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                var loaiDocGiaToDelete = await _context.Loaidocgia.FindAsync(id);
                if (loaiDocGiaToDelete == null)
                {
                    Debug.WriteLine($"LoaiDocGia with ID {id} not found for delete.");
                    return false; // Không tìm thấy
                }

                _context.Loaidocgia.Remove(loaiDocGiaToDelete);

                try
                {
                    return await _context.SaveChangesAsync() > 0;
                }
                catch (DbUpdateException ex) // Bắt lỗi ràng buộc khóa ngoại
                {
                    Debug.WriteLine($"Error deleting Loaidocgia ID {id}: {ex.InnerException?.Message ?? ex.Message}");
                    throw new InvalidOperationException($"Không thể xóa loại độc giả này (ID: {id}) vì có dữ liệu liên quan.", ex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unexpected error deleting Loaidocgia ID {id}: {ex.Message}");
                    throw;
                }
            }
        }

        // --- Triển khai các phương thức bổ sung ---

        /// <inheritdoc/>
        public async Task<List<Loaidocgia>> SearchAsync(string searchTerm)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var normalizedSearchTerm = searchTerm.Trim().ToLower();
                    return await _context.Loaidocgia.AsNoTracking()
                        .Where(ldg =>
                            (ldg.MaLoaiDocGia != null && ldg.MaLoaiDocGia.ToLower().Contains(normalizedSearchTerm)) ||
                            (ldg.TenLoaiDocGia != null && ldg.TenLoaiDocGia.ToLower().Contains(normalizedSearchTerm))
                        )
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALLoaiDocGia.SearchAsync (term: {searchTerm}): {ex.Message}");
                    throw;
                }
            }
        }

        /// <inheritdoc/>
        public async Task<List<Loaidocgia>> GetFilteredAndSortedAsync(string? searchTerm, string sortColumn, bool ascending)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    IQueryable<Loaidocgia> query = _context.Loaidocgia.AsNoTracking();

                    // Filtering
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        var normalizedSearchTerm = searchTerm.Trim().ToLower();
                        query = query.Where(ldg =>
                            (ldg.MaLoaiDocGia != null && ldg.MaLoaiDocGia.ToLower().Contains(normalizedSearchTerm)) ||
                            (ldg.TenLoaiDocGia != null && ldg.TenLoaiDocGia.ToLower().Contains(normalizedSearchTerm))
                        );
                    }

                    // Sorting
                    query = sortColumn switch
                    {
                        nameof(Loaidocgia.TenLoaiDocGia) => ascending ? query.OrderBy(ldg => ldg.TenLoaiDocGia) : query.OrderByDescending(ldg => ldg.TenLoaiDocGia),
                        nameof(Loaidocgia.Id) => ascending ? query.OrderBy(ldg => ldg.Id) : query.OrderByDescending(ldg => ldg.Id),
                        nameof(Loaidocgia.MaLoaiDocGia) or _ => ascending ? query.OrderBy(ldg => ldg.MaLoaiDocGia) : query.OrderByDescending(ldg => ldg.MaLoaiDocGia),
                    };

                    return await query.ToListAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALLoaiDocGia.GetFilteredAndSortedAsync: {ex.Message}");
                    throw;
                }
            }
        }

        /// <inheritdoc/>
        public async Task<List<(Loaidocgia Entity, int Count)>> GetFilteredAndSortedWithCountAsync(string? searchTerm, string sortColumn, bool ascending)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    IQueryable<Loaidocgia> query = _context.Loaidocgia;

                    // Filtering
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        var normalizedSearchTerm = searchTerm.Trim().ToLower();
                        query = query.Where(ldg =>
                            (ldg.MaLoaiDocGia != null && ldg.MaLoaiDocGia.ToLower().Contains(normalizedSearchTerm)) ||
                            (ldg.TenLoaiDocGia != null && ldg.TenLoaiDocGia.ToLower().Contains(normalizedSearchTerm))
                        );
                    }

                    // Sorting
                    query = sortColumn switch
                    {
                        nameof(Loaidocgia.TenLoaiDocGia) => ascending ? query.OrderBy(ldg => ldg.TenLoaiDocGia) : query.OrderByDescending(ldg => ldg.TenLoaiDocGia),
                        nameof(Loaidocgia.Id) => ascending ? query.OrderBy(ldg => ldg.Id) : query.OrderByDescending(ldg => ldg.Id),
                        nameof(Loaidocgia.MaLoaiDocGia) or _ => ascending ? query.OrderBy(ldg => ldg.MaLoaiDocGia) : query.OrderByDescending(ldg => ldg.MaLoaiDocGia),
                    };

                    // Select Entity and Count
                    var queryWithCount = query.Select(ldg => new
                    {
                        Entity = ldg,
                        Count = _context.Docgia.Count(dg => dg.IdLoaiDocGia == ldg.Id)
                    }).AsNoTracking();

                    var results = await queryWithCount.ToListAsync();
                    return results.Select(r => (r.Entity, r.Count)).ToList();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALLoaiDocGia.GetFilteredAndSortedWithCountAsync: {ex.Message}");
                    throw;
                }
            }
        }

        /// <inheritdoc/>
        public async Task<bool> AnyAsync(Expression<Func<Loaidocgia, bool>> predicate)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    return await _context.Loaidocgia.AnyAsync(predicate);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALLoaiDocGia.AnyAsync: {ex.Message}");
                    throw;
                }
            }
        }

        /// <inheritdoc/>
        public async Task<int> CountRelatedDocGiaAsync(int loaiDocGiaId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    return await _context.Docgia.CountAsync(dg => dg.IdLoaiDocGia == loaiDocGiaId);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALLoaiDocGia.CountRelatedDocGiaAsync (ID: {loaiDocGiaId}): {ex.Message}");
                    throw;
                }
            }
        }

        // ****** PHƯƠNG THỨC MỚI ĐỂ SINH MÃ TỰ ĐỘNG ******
        /// <inheritdoc/>
        public async Task<string?> GetLastMaLoaiDocGiaAsync(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) return null;

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Lấy tất cả mã bắt đầu bằng prefix, sắp xếp giảm dần theo phần số
                    // và lấy mã đầu tiên (là mã lớn nhất)
                    // Cách này hiệu quả hơn nếu có index trên cột MaLoaiDocGia
                    var lastCode = await _context.Loaidocgia
                        .Where(l => l.MaLoaiDocGia != null && l.MaLoaiDocGia.StartsWith(prefix))
                        .Select(l => l.MaLoaiDocGia) // Chỉ lấy mã
                                                     // Sắp xếp phức tạp cần thực hiện phía client hoặc dùng SQL thô
                                                     // Tạm thời lấy hết về và xử lý
                        .ToListAsync();

                    // Xử lý tìm số lớn nhất phía client (tương tự cách cũ)
                    int maxNumber = 0;
                    foreach (var code in lastCode) // Dùng lastCode thay vì codesWithPrefix
                    {
                        if (code != null && code.Length > prefix.Length)
                        {
                            string numberPart = code.Substring(prefix.Length);
                            if (int.TryParse(numberPart, out int currentNumber))
                            {
                                if (currentNumber > maxNumber)
                                {
                                    maxNumber = currentNumber;
                                }
                            }
                        }
                    }

                    if (maxNumber > 0)
                    {
                        // Trả về mã tương ứng với số lớn nhất, format D3
                        return $"{prefix}{maxNumber:D3}";
                    }

                    return null; // Không tìm thấy mã hợp lệ
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALLoaiDocGia.GetLastMaLoaiDocGiaAsync (prefix: {prefix}): {ex.Message}");
                    throw new Exception($"Lỗi khi truy vấn mã loại độc giả cuối cùng (prefix: {prefix}).", ex);
                }
            }
        }
        // ****** KẾT THÚC PHƯƠNG THỨC MỚI ******

    } // Kết thúc class DALLoaiDocGia
} // Kết thúc namespace DAL