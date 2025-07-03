// File: DAL/DALThamso.cs
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics; // Sử dụng Debug.WriteLine cho logging đơn giản như các hàm khác trong file này
using System.Threading.Tasks;

namespace DAL
{
    /// <summary>
    /// Data Access Layer triển khai IDALThamSo, tương tác với bảng THAMSO trong DB.
    /// </summary>
    public class DALThamso : IDALThamSo
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private const int DEFAULT_THAMSO_ID = 2; // ID mặc định của bộ tham số

        public DALThamso(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        // --- Triển khai các phương thức từ IDALThamSo ---

        public async Task<Thamso?> GetThamSoAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Giả sử DbSet trong QLTVContext tên là "Thamso"
                    return await _context.Thamso
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(ts => ts.Id == DEFAULT_THAMSO_ID);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALThamso.GetThamSoAsync: {ex.Message}");
                    // Xem xét ném lại lỗi hoặc log chi tiết hơn thay vì chỉ trả về null
                    // throw new Exception("Lỗi khi truy cập cơ sở dữ liệu để lấy tham số.", ex);
                    return null; // Trả về null nếu có lỗi DB
                }
            }
        }

        public async Task<bool> UpdateAsync(Thamso thamso)
        {
            if (thamso == null) throw new ArgumentNullException(nameof(thamso));
            // Đảm bảo luôn cập nhật bản ghi có ID mặc định
            thamso.Id = DEFAULT_THAMSO_ID;

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Giả sử DbSet trong QLTVContext tên là "Thamso"
                    var existingThamso = await _context.Thamso.FindAsync(thamso.Id);
                    if (existingThamso == null)
                    {
                        Debug.WriteLine($"UpdateAsync failed: Thamso with ID {thamso.Id} not found.");
                        return false; // Không tìm thấy bản ghi để cập nhật
                    }

                    // Cập nhật tất cả các giá trị từ entity đầu vào vào entity đang được theo dõi
                    _context.Entry(existingThamso).CurrentValues.SetValues(thamso);
                    // Đánh dấu entity là Modified để đảm bảo SaveChanges hoạt động ngay cả khi không có thay đổi giá trị thực sự
                    _context.Entry(existingThamso).State = EntityState.Modified;

                    int affectedRows = await _context.SaveChangesAsync();
                    // Trả về true nếu có ít nhất 1 dòng bị ảnh hưởng (thường là 1)
                    return affectedRows > 0;
                }
                catch (DbUpdateConcurrencyException ex) // Bắt lỗi specific hơn nếu cần
                {
                    Debug.WriteLine($"Concurrency error in DALThamso.UpdateAsync (ID: {thamso.Id}): {ex.Message}");
                    throw; // Ném lại để tầng trên xử lý
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in DALThamso.UpdateAsync (ID: {thamso.Id}): {ex.Message}");
                    throw; // Ném lại lỗi để tầng trên xử lý
                }
            }
        }

        // <<< TRIỂN KHAI PHƯƠNG THỨC AddAsync >>>
        /// <summary>
        /// Thêm mới một bộ tham số vào cơ sở dữ liệu.
        /// Triển khai phương thức từ IDALThamSo (nếu đã thêm vào interface).
        /// </summary>
        /// <param name="thamso">Entity Thamso cần thêm.</param>
        /// <returns>Task chứa Entity Thamso đã được thêm (có ID) hoặc null nếu thất bại.</returns>
        /// <exception cref="ArgumentNullException">Ném ra nếu thamso là null.</exception>
        /// <exception cref="DbUpdateException">Ném ra nếu có lỗi DB khi lưu (ví dụ: duplicate key).</exception>
        /// <exception cref="Exception">Ném ra cho các lỗi không xác định khác.</exception>
        public async Task<Thamso?> AddAsync(Thamso thamso)
        {
            if (thamso == null) throw new ArgumentNullException(nameof(thamso));
            // Nếu bạn muốn ép ID luôn là DEFAULT_THAMSO_ID khi thêm (và DB cho phép insert explicit ID)
            // thamso.Id = DEFAULT_THAMSO_ID;
            // Hoặc nếu ID là identity tự tăng, đảm bảo ID = 0 trước khi thêm
            // thamso.Id = 0;

            Debug.WriteLine($"Attempting to add new Thamso with proposed ID {thamso.Id}");

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Giả sử DbSet trong QLTVContext tên là "Thamso"
                    await _context.Thamso.AddAsync(thamso);
                    int affectedRows = await _context.SaveChangesAsync();

                    if (affectedRows > 0)
                    {
                        Debug.WriteLine($"Successfully added Thamso with final ID {thamso.Id}");
                        return thamso; // Trả về entity đã có ID
                    }
                    else
                    {
                        Debug.WriteLine($"AddAsync for Thamso completed but no rows affected.");
                        return null; // Không có dòng nào được thêm
                    }
                }
                catch (DbUpdateException ex)
                {
                    Debug.WriteLine($"DbUpdateException adding Thamso. Details: {ex.InnerException?.Message ?? ex.Message}");
                    // Kiểm tra lỗi duplicate key nếu ID không phải identity và bạn cố thêm ID đã tồn tại
                    if (ex.InnerException?.Message?.Contains("duplicate key") == true || ex.InnerException?.Message?.Contains("PRIMARY KEY constraint") == true)
                    {
                        throw new InvalidOperationException($"Bộ tham số với ID {thamso.Id} đã tồn tại.", ex);
                    }
                    throw new Exception("Lỗi DB khi thêm tham số.", ex); // Ném lỗi chung
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unexpected error adding Thamso: {ex.Message}");
                    throw new Exception("Lỗi không xác định khi thêm tham số.", ex); // Ném lỗi chung
                }
            }
        }
        // <<< KẾT THÚC TRIỂN KHAI AddAsync >>>


        // --- Helper để lấy giá trị hoặc default ---
        // Helper này sẽ trả về T (non-nullable) vì luôn có giá trị mặc định
        private async Task<T> GetThamSoValueOrDefaultAsync<T>(Func<Thamso, T?> valueSelector, T defaultValue) where T : struct
        {
            try
            {
                var thamso = await GetThamSoAsync();
                if (thamso != null)
                {
                    // Lấy giá trị nullable từ selector
                    T? value = valueSelector(thamso);
                    // Nếu giá trị không null thì trả về nó, ngược lại trả về default
                    return value ?? defaultValue;
                }
                // Nếu không tìm thấy tham số, trả về default
                Debug.WriteLine($"Thamso record not found, returning default value: {defaultValue}");
                return defaultValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting ThamSo value (using default {defaultValue}): {ex.Message}");
                return defaultValue; // Trả về default nếu có lỗi DB
            }
        }


        // --- Triển khai các phương thức lấy giá trị cụ thể (ĐÃ SỬA LỖI) ---

        public async Task<int> GetMinimumReaderAgeAsync()
        {
            // Gọi helper, helper sẽ trả về int (không phải int?)
            return await GetThamSoValueOrDefaultAsync(ts => ts.TuoiToiThieu, 18); // Default 18
        }

        public async Task<int> GetMaximumReaderAgeAsync()
        {
            return await GetThamSoValueOrDefaultAsync(ts => ts.TuoiToiDa, 55); // Default 55
        }

        public async Task<int> GetCardValidityDurationAsync()
        {
            return await GetThamSoValueOrDefaultAsync(ts => ts.ThoiHanThe, 1); // Default 1 năm
        }

        public async Task<int> GetMaxBorrowDaysAsync()
        {
            return await GetThamSoValueOrDefaultAsync(ts => ts.SoNgayMuonToiDa, 7); // Default 7 ngày
        }

        public async Task<int> GetMaxBooksPerLoanAsync()
        {
            return await GetThamSoValueOrDefaultAsync(ts => ts.SoSachMuonToiDa, 5); // Default 5 cuốn
        }

        public async Task<int> GetFinePerDayAsync()
        {
            // Giả sử DonGiaPhat là int? trong DB, nếu là int thì không cần nullable valueSelector
            // return await GetThamSoValueOrDefaultAsync(ts => (int?)ts.DonGiaPhat, 1000); // Nếu DonGiaPhat là int
            return await GetThamSoValueOrDefaultAsync(ts => ts.DonGiaPhat, 1000); // Nếu DonGiaPhat là int?
        }

        public async Task<int> GetPublishingYearGapAsync()
        {
            return await GetThamSoValueOrDefaultAsync(ts => ts.KhoangCachXuatBan, 8); // Default 8 năm
        }

    } // Kết thúc class DALThamso
} // Kết thúc namespace DAL