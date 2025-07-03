// Project/Namespace: DAL

// --- USING ---
using DAL.Models; // Namespace chứa các Entity models (Cuonsach, Sach, Tuasach, Theloai, Tacgia, Bcsachtratre, Phieumuontra, QLTVContext)
using Microsoft.EntityFrameworkCore; // Cần cho DbContext, các hàm async của EF (Include, ThenInclude, Where, FirstOrDefaultAsync, ToListAsync, AnyAsync, CountAsync, ExecuteUpdateAsync, ExecuteDeleteAsync)
using Microsoft.Extensions.DependencyInjection; // Cần cho IServiceScopeFactory, GetRequiredService
using Microsoft.Extensions.Logging; // Cần cho ILogger, LogInformation, LogWarning, LogError
using System; // Cần cho ArgumentNullException, InvalidOperationException, Exception
using System.Collections.Generic; // Cần cho List
using System.Linq; // Cần cho LINQ (Where, Any)
using System.Threading.Tasks; // Cần cho async/await Task

namespace DAL
{
    /// <summary>
    /// Data Access Layer triển khai IDALCuonSach, tương tác với DB thông qua EF Core.
    /// Refactor để sử dụng IServiceScopeFactory để quản lý vòng đời DbContext cho từng thao tác.
    /// Đã thêm triển khai phương thức GetBySachIdAsync và các phương thức từ interface.
    /// </summary>
    public class DALCuonSach : IDALCuonSach // <<< Implement interface IDALCuonSach
    {
        // --- DEPENDENCIES ---
        // Nhận IServiceScopeFactory thay vì QLTVContext
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DALCuonSach> _logger; // Thêm Logger

        // --- CONSTRUCTOR (Sử dụng Dependency Injection) ---
        // Nhận IServiceScopeFactory và ILogger
        public DALCuonSach(IServiceScopeFactory scopeFactory, ILogger<DALCuonSach> logger)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Khởi tạo Logger
            _logger.LogInformation("DALCuonSach initialized.");
        }

        // --- METHOD IMPLEMENTATIONS (Từ IDALCuonSach) ---

        /// <summary>
        /// Lấy tất cả các bản ghi Cuonsach từ cơ sở dữ liệu, bao gồm các navigation properties cần thiết.
        /// Chỉ lấy các cuốn sách chưa bị xóa mềm (DaAn = 0).
        /// </summary>
        /// <returns>Task chứa danh sách các Entity Cuonsach.</returns>
        public async Task<List<Cuonsach>> GetAllAsync()
        {
            _logger.LogInformation("Attempting to retrieve all non-deleted Cuonsach entities.");
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();

                try
                {
                    return await _context.Cuonsach
                        .AsNoTracking() // Nên dùng AsNoTracking cho các thao tác đọc
                        .Where(cs => cs.DaAn == 0) // Chỉ lấy cuốn sách chưa bị xóa mềm
                        .Include(cs => cs.IdSachNavigation) // Nối đến Sach
                            .ThenInclude(s => s.IdTuaSachNavigation) // Từ Sach nối đến Tuasach
                                .ThenInclude(ts => ts.IdTheLoaiNavigation) // Từ Tuasach nối đến Theloai
                                                                           // Nối từ Tuasach đến collection Tacgia (dựa trên cấu trúc Tuasach.cs bạn cung cấp: ICollection<Tacgia> IdTacGia)
                                                                           // Cần lặp lại Include chain đến IdSachNavigation -> IdTuaSachNavigation để "rẽ nhánh" Include Tacgia
                         .Include(cs => cs.IdSachNavigation)
                            .ThenInclude(s => s.IdTuaSachNavigation)
                                .ThenInclude(ts => ts.IdTacGia) // Sử dụng tên navigation property đúng 'IdTacGia'

                        // Có thể include Phieumuontra nếu cần thông tin mượn trả cuối cùng (ví dụ: ai đang mượn?)
                        // .Include(cs => cs.Phieumuontra.OrderByDescending(pmt => pmt.NgayMuon).Take(1)) // Lấy phiếu mượn mới nhất

                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALCuonSach.GetAllAsync.");
                    throw new Exception("Lỗi khi lấy danh sách cuốn sách.", ex); // Wrap lỗi
                }
            }
        }

        /// <summary>
        /// Lấy một bản ghi Cuonsach theo ID, bao gồm các navigation properties cần thiết.
        /// Chỉ lấy cuốn sách chưa bị xóa mềm (DaAn = 0).
        /// </summary>
        /// <param name="id">ID của cuốn sách cần lấy.</param>
        /// <returns>Task chứa Entity Cuonsach hoặc null nếu không tìm thấy hoặc ID không hợp lệ.</returns>
        public async Task<Cuonsach?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve non-deleted Cuonsach with ID: {Id}", id);
            if (id <= 0) return null; // ID không hợp lệ

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();

                try
                {
                    return await _context.Cuonsach
                        .AsNoTracking()
                        .Where(cs => cs.Id == id && cs.DaAn == 0) // Kiểm tra cả ID và DaAn
                        .Include(cs => cs.IdSachNavigation) // Nối đến Sach
                            .ThenInclude(s => s.IdTuaSachNavigation) // Từ Sach nối đến Tuasach
                                .ThenInclude(ts => ts.IdTheLoaiNavigation) // Từ Tuasach nối đến Theloai
                                                                           // Nối từ Tuasach đến collection Tacgia
                         .Include(cs => cs.IdSachNavigation)
                            .ThenInclude(s => s.IdTuaSachNavigation)
                                .ThenInclude(ts => ts.IdTacGia) // Sử dụng tên navigation property đúng 'IdTacGia'

                        // Có thể include Phieumuontra nếu cần thông tin mượn trả cuối cùng
                        // .Include(cs => cs.Phieumuontra.Where(pmt => pmt.NgayTra == null).OrderByDescending(pmt => pmt.NgayMuon).Take(1))

                        .FirstOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALCuonSach.GetByIdAsync (ID: {Id}).", id);
                    throw new Exception($"Lỗi khi lấy cuốn sách theo ID: {id}.", ex); // Wrap lỗi
                }
            }
        }

        /// <summary>
        /// Lấy một bản ghi Cuonsach theo ID, bao gồm các navigation properties cần thiết.
        /// Bao gồm cả các sách đã bị xóa mềm (DaAn = 1).
        /// </summary>
        /// <param name="id">ID của cuốn sách cần lấy.</param>
        /// <returns>Task chứa Entity Cuonsach hoặc null nếu không tìm thấy hoặc ID không hợp lệ.</returns>
        public async Task<Cuonsach?> GetByIdIncludingDeletedAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve Cuonsach (including deleted) with ID: {Id}", id);
            if (id <= 0) return null; // ID không hợp lệ

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();

                try
                {
                    return await _context.Cuonsach
                        .AsNoTracking()
                        .Where(cs => cs.Id == id) // Không kiểm tra DaAn
                        .Include(cs => cs.IdSachNavigation)
                            .ThenInclude(s => s.IdTuaSachNavigation)
                                .ThenInclude(ts => ts.IdTheLoaiNavigation)
                         .Include(cs => cs.IdSachNavigation)
                            .ThenInclude(s => s.IdTuaSachNavigation)
                                .ThenInclude(ts => ts.IdTacGia)
                        // .Include(cs => cs.Phieumuontra) // Include collection nếu cần
                        .FirstOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALCuonSach.GetByIdIncludingDeletedAsync (ID: {Id}).", id);
                    throw new Exception($"Lỗi khi lấy cuốn sách (bao gồm đã xóa) với ID: {id}.", ex); // Wrap lỗi
                }
            }
        }


        // Lấy cuốn sách theo Mã (chưa xóa mềm)
        public async Task<Cuonsach?> GetByMaAsync(string maCuonSach)
        {
            _logger.LogInformation("Attempting to retrieve non-deleted Cuonsach with MaCuonSach: {MaCuonSach}", maCuonSach);
            if (string.IsNullOrWhiteSpace(maCuonSach)) return null;

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    return await _context.Cuonsach
                        .AsNoTracking()
                        .Where(cs => cs.DaAn == 0 && cs.MaCuonSach != null && cs.MaCuonSach.ToLower() == maCuonSach.Trim().ToLower())
                        .Include(cs => cs.IdSachNavigation)
                           .ThenInclude(s => s.IdTuaSachNavigation)
                               .ThenInclude(ts => ts.IdTheLoaiNavigation)
                        .Include(cs => cs.IdSachNavigation)
                           .ThenInclude(s => s.IdTuaSachNavigation)
                               .ThenInclude(ts => ts.IdTacGia)
                        // .Include(cs => cs.Phieumuontra.Where(pmt => pmt.NgayTra == null).OrderByDescending(pmt => pmt.NgayMuon).Take(1))

                        .FirstOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALCuonSach.GetByMaAsync (Ma: {MaCuonSach}).", maCuonSach);
                    throw new Exception($"Lỗi khi lấy cuốn sách theo mã: {maCuonSach}.", ex); // Wrap lỗi
                }
            }
        }

        // Lấy cuốn sách theo Mã (bao gồm cả đã xóa)
        public async Task<Cuonsach?> GetByMaIncludingDeletedAsync(string maCuonSach)
        {
            _logger.LogInformation("Attempting to retrieve Cuonsach (including deleted) with MaCuonSach: {MaCuonSach}", maCuonSach);
            if (string.IsNullOrWhiteSpace(maCuonSach)) return null;

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    return await _context.Cuonsach
                        .AsNoTracking()
                        .Where(cs => cs.MaCuonSach != null && cs.MaCuonSach.ToLower() == maCuonSach.Trim().ToLower()) // Không kiểm tra DaAn
                        .Include(cs => cs.IdSachNavigation)
                           .ThenInclude(s => s.IdTuaSachNavigation)
                               .ThenInclude(ts => ts.IdTheLoaiNavigation)
                        .Include(cs => cs.IdSachNavigation)
                           .ThenInclude(s => s.IdTuaSachNavigation)
                               .ThenInclude(ts => ts.IdTacGia)
                        .FirstOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALCuonSach.GetByMaIncludingDeletedAsync (Ma: {MaCuonSach}).", maCuonSach);
                    throw new Exception($"Lỗi khi lấy cuốn sách (bao gồm đã xóa) theo mã: {maCuonSach}.", ex); // Wrap lỗi
                }
            }
        }

        // Lấy tất cả cuốn sách thuộc một Sach ID (chưa xóa mềm)
        public async Task<List<Cuonsach>> GetBySachIdAsync(int sachId)
        {
            _logger.LogInformation("Attempting to retrieve non-deleted Cuonsach entities for Sach ID: {SachId}", sachId);
            if (sachId <= 0) return new List<Cuonsach>(); // ID không hợp lệ

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    return await _context.Cuonsach
                        .AsNoTracking()
                        .Where(cs => cs.IdSach == sachId && cs.DaAn == 0) // Lọc theo IdSach và DaAn=0
                        .Include(cs => cs.IdSachNavigation) // Nối đến Sach
                            .ThenInclude(s => s.IdTuaSachNavigation) // Từ Sach nối đến Tuasach
                                .ThenInclude(ts => ts.IdTheLoaiNavigation) // Từ Tuasach nối đến Theloai
                                                                           // Nối từ Tuasach đến collection Tacgia
                         .Include(cs => cs.IdSachNavigation)
                            .ThenInclude(s => s.IdTuaSachNavigation)
                                .ThenInclude(ts => ts.IdTacGia) // Sử dụng tên navigation property đúng 'IdTacGia'
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALCuonSach.GetBySachIdAsync (SachId: {SachId}).", sachId);
                    throw new Exception($"Lỗi khi lấy cuốn sách theo Sach ID: {sachId}.", ex); // Wrap lỗi
                }
            }
        }


        // Kiểm tra sự tồn tại theo Mã Cuốn sách (bao gồm tất cả)
        public async Task<bool> IsMaCuonSachExistsAsync(string maCuonSach)
        {
            _logger.LogInformation("Checking if MaCuonSach exists (including deleted): {MaCuonSach}", maCuonSach);
            if (string.IsNullOrWhiteSpace(maCuonSach)) return false;
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Sử dụng AnyAsync để kiểm tra sự tồn tại hiệu quả hơn (không cần load toàn bộ entity)
                    return await _context.Cuonsach.AnyAsync(cs => cs.MaCuonSach != null && cs.MaCuonSach.ToLower() == maCuonSach.Trim().ToLower()); // Không kiểm tra DaAn
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALCuonSach.IsMaCuonSachExistsAsync (Ma: {MaCuonSach}).", maCuonSach);
                    throw new Exception("Lỗi khi kiểm tra sự tồn tại của mã cuốn sách.", ex); // Wrap lỗi
                }
            }
        }

        // Kiểm tra sự tồn tại theo Mã Cuốn sách (loại trừ một ID)
        public async Task<bool> IsMaCuonSachExistsExcludingIdAsync(string maCuonSach, int excludeId)
        {
            _logger.LogInformation("Checking if MaCuonSach exists (excluding ID {ExcludeId}, including deleted): {MaCuonSach}", excludeId, maCuonSach);
            if (string.IsNullOrWhiteSpace(maCuonSach)) return false;
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Sử dụng AnyAsync để kiểm tra sự tồn tại
                    return await _context.Cuonsach.AnyAsync(cs => cs.Id != excludeId && cs.MaCuonSach != null && cs.MaCuonSach.ToLower() == maCuonSach.Trim().ToLower()); // Không kiểm tra DaAn
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALCuonSach.IsMaCuonSachExistsExcludingIdAsync (Ma: {MaCuonSach}, ExcludeId: {ExcludeId}).", maCuonSach, excludeId);
                    throw new Exception($"Lỗi khi kiểm tra sự tồn tại của mã cuốn sách (loại trừ ID {excludeId}).", ex); // Wrap lỗi
                }
            }
        }

        // Kiểm tra xem cuốn sách có đang được mượn không
        public async Task<bool> IsBorrowedAsync(int id)
        {
            _logger.LogInformation("Checking if Cuonsach with ID: {Id} is currently borrowed.", id);
            if (id <= 0) return false; // ID không hợp lệ

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Kiểm tra trong các phiếu mượn trả, có phiếu nào liên quan đến cuốn sách này VÀ chưa được trả (NgayTra == null) không
                    // Hoặc kiểm tra trạng thái 'Đang mượn' được lưu trực tiếp trên entity Cuonsach (ví dụ TinhTrang == 1)
                    // >>> CHỌN MỘT TRONG CÁC CÁCH KIỂM TRA PHÙ HỢP VỚI THIẾT KẾ CỦA BẠN <<<

                    // Cách 1: Kiểm tra qua Phieumuontra
                    // bool isBorrowed = await _context.Phieumuontra.AnyAsync(pmt => pmt.IdCuonSach == id && pmt.NgayTra == null);

                    // Cách 2: Kiểm tra qua TinhTrang của Cuonsach (NẾU CÓ)
                    // Vui lòng thay thế "TinhTrang" và "1" bằng tên thuộc tính và giá trị thực tế
                    bool isBorrowed = await _context.Cuonsach.AnyAsync(cs => cs.Id == id && cs.TinhTrang == 1); // Giả định TinhTrang và 1 là "Đang mượn"


                    _logger.LogInformation("Cuonsach ID: {Id} is borrowed status: {IsBorrowed}", id, isBorrowed);
                    return isBorrowed;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking borrowed status for Cuonsach ID: {Id}.", id);
                    throw new Exception($"Lỗi khi kiểm tra trạng thái mượn của cuốn sách ID {id}.", ex); // Wrap lỗi
                }
            }
        }


        // Thêm mới cuốn sách
        public async Task<Cuonsach?> AddAsync(Cuonsach cuonSach) // Trả về Cuonsach đã thêm để có ID
        {
            _logger.LogInformation("Attempting to add a new Cuonsach.");
            if (cuonSach == null)
            {
                _logger.LogError("Attempted to add a null Cuonsach entity.");
                throw new ArgumentNullException(nameof(cuonSach), "Entity Cuonsach cần thêm không được rỗng.");
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();

                try
                {
                    // Đảm bảo các navigation property không được attach nếu không theo dõi
                    if (cuonSach.IdSachNavigation != null) cuonSach.IdSachNavigation = null!;
                    cuonSach.Bcsachtratre ??= new List<Bcsachtratre>(); // Khởi tạo nếu null
                    cuonSach.Phieumuontra ??= new List<Phieumuontra>(); // Khởi tạo nếu null

                    // Thiết lập giá trị mặc định nếu cần
                    if (cuonSach.DaAn != 0 && cuonSach.DaAn != 1) cuonSach.DaAn = 0; // Mặc định chưa xóa mềm
                    // VUI LÒNG THAY THẾ "TinhTrang" VÀ "0" BẰNG TÊN THUỘC TÍNH VÀ GIÁ TRỊ CHO TRẠNG THÁI MẶC ĐỊNH KHI THÊM MỚI (VÍ DỤ: "CÓ SẴN")
                    if (cuonSach.TinhTrang != 0) cuonSach.TinhTrang = 0; // Giả định 0 là trạng thái "Có sẵn"

                    await _context.Cuonsach.AddAsync(cuonSach);
                    int affectedRows = await _context.SaveChangesAsync();

                    if (affectedRows > 0)
                    {
                        _logger.LogInformation("Successfully added Cuonsach with generated ID: {Id}", cuonSach.Id);
                        return cuonSach; // Trả về entity (giờ có ID nếu là Identity)
                    }
                    _logger.LogWarning("AddAsync for Cuonsach completed but no rows were affected.");
                    return null; // Thêm thất bại
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error adding Cuonsach. MaCuonSach: {MaCuonSach}, Details: {ErrorMessage}", cuonSach.MaCuonSach, ex.InnerException?.Message ?? ex.Message);
                    if (ex.InnerException?.Message?.Contains("UNIQUE constraint") == true || ex.InnerException?.Message?.Contains("duplicate key") == true)
                    {
                        throw new InvalidOperationException($"Mã cuốn sách '{cuonSach.MaCuonSach?.Trim()}' đã tồn tại.", ex);
                    }
                    throw new Exception("Lỗi cơ sở dữ liệu khi thêm cuốn sách.", ex); // Wrap lỗi DB khác
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error adding Cuonsach. MaCuonSach: {MaCuonSach}", cuonSach.MaCuonSach);
                    throw new Exception("Lỗi không xác định khi thêm cuốn sách.", ex); // Wrap lỗi
                }
            }
        }

        // <<< TRIỂN KHAI PHƯƠNG THỨC AddRangeAsync >>>
        /// <summary>
        /// Thêm nhiều đối tượng CuonSach mới vào cơ sở dữ liệu trong một lượt.
        /// Triển khai phương thức từ IDALCuonSach.
        /// </summary>
        /// <param name="cuonSachs">Danh sách các đối tượng CuonSach cần thêm.</param>
        /// <returns>Task chứa bool, true nếu tất cả thêm thành công (số dòng ảnh hưởng khớp số lượng), false nếu danh sách rỗng hoặc có lỗi.</returns>
        /// <exception cref="DbUpdateException">Ném ra nếu có lỗi DB khi lưu.</exception>
        /// <exception cref="Exception">Ném ra cho các lỗi không xác định khác.</exception>
        public async Task<bool> AddRangeAsync(List<Cuonsach> cuonSachs)
        {
            _logger.LogInformation("Attempting to add a range of {Count} CuonSach entities.", cuonSachs?.Count ?? 0);
            if (cuonSachs == null || !cuonSachs.Any())
            {
                _logger.LogInformation("AddRangeAsync called with empty or null list of CuonSach.");
                return true; // Không có gì để thêm, coi như thành công
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                // Nếu logic phức tạp hơn cần transaction, có thể thêm ở đây:
                // using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Đảm bảo các cuốn sách mới có trạng thái hợp lệ trước khi thêm
                    foreach (var cs in cuonSachs)
                    {
                        if (cs.DaAn != 0 && cs.DaAn != 1) cs.DaAn = 0; // Mặc định chưa xóa mềm
                                                                       // VUI LÒNG THAY THẾ "TinhTrang" VÀ "0" BẰNG TÊN THUỘC TÍNH VÀ GIÁ TRỊ CHO TRẠNG THÁI MẶC ĐỊNH KHI THÊM MỚI (VÍ DỤ: "CÓ SẴN")
                        if (cs.TinhTrang != 0) cs.TinhTrang = 0; // Giả định 0 là trạng thái "Có sẵn"
                                                                 // Đảm bảo không attach nav props
                        cs.IdSachNavigation = null!;
                        cs.Bcsachtratre ??= new List<Bcsachtratre>();
                        cs.Phieumuontra ??= new List<Phieumuontra>();
                    }

                    // Sử dụng AddRangeAsync của DbContext để thêm hiệu quả
                    await _context.Cuonsach.AddRangeAsync(cuonSachs);
                    int affectedRows = await _context.SaveChangesAsync(); // Lưu thay đổi

                    // Kiểm tra số dòng bị ảnh hưởng có khớp với số lượng cần thêm không
                    if (affectedRows == cuonSachs.Count)
                    {
                        _logger.LogInformation("Successfully added {Count} CuonSach entities.", affectedRows);
                        // if (transaction != null) await transaction.CommitAsync();
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("SaveChangesAsync added {AffectedRows} rows, but expected to add {ExpectedCount} CuonSach entities.", affectedRows, cuonSachs.Count);
                        // Rollback nếu dùng transaction và coi đây là lỗi
                        // if (transaction != null) await transaction.RollbackAsync(); 
                        return false; // Coi là thất bại nếu số lượng không khớp
                    }
                }
                catch (DbUpdateException ex) // Bắt lỗi DB
                {
                    // if (transaction != null) await transaction.RollbackAsync();
                    _logger.LogError(ex, "DbUpdateException occurred while adding range of CuonSach entities. Details: {ErrorMessage}", ex.InnerException?.Message ?? ex.Message);
                    throw new Exception("Lỗi cơ sở dữ liệu khi thêm hàng loạt cuốn sách.", ex); // Wrap lỗi
                }
                catch (Exception ex) // Bắt các lỗi khác
                {
                    // if (transaction != null) await transaction.RollbackAsync();
                    _logger.LogError(ex, "Unexpected error occurred during AddRangeAsync for CuonSach entities.");
                    throw new Exception("Lỗi không xác định khi thêm hàng loạt cuốn sách.", ex); // Wrap lỗi
                }
            }
        }
        // <<< KẾT THÚC TRIỂN KHAI AddRangeAsync >>>


        // Cập nhật cuốn sách
        public async Task<bool> UpdateAsync(Cuonsach cuonSach)
        {
            _logger.LogInformation("Attempting to update Cuonsach with ID: {Id}", cuonSach?.Id);
            if (cuonSach == null)
            {
                _logger.LogError("Attempted to update a null Cuonsach entity.");
                throw new ArgumentNullException(nameof(cuonSach), "Thông tin cuốn sách cần cập nhật không được rỗng.");
            }
            if (cuonSach.Id <= 0)
            {
                _logger.LogError("Invalid Cuonsach ID for update: {Id}", cuonSach.Id);
                throw new ArgumentException("ID cuốn sách không hợp lệ để cập nhật.", nameof(cuonSach.Id));
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();

                try
                {
                    var existingCuonSach = await _context.Cuonsach.FirstOrDefaultAsync(cs => cs.Id == cuonSach.Id && cs.DaAn == 0);

                    if (existingCuonSach == null)
                    {
                        _logger.LogInformation("Update failed: Cuonsach with ID {Id} not found or already deleted.", cuonSach.Id);
                        return false; // Không tìm thấy hoặc đã bị xóa mềm
                    }

                    // Cập nhật các thuộc tính được phép thay đổi
                    // VUI LÒNG THAY THẾ "TinhTrang" BẰNG TÊN THUỘC TÍNH THỰC TẾ
                    existingCuonSach.TinhTrang = cuonSach.TinhTrang; // Tình trạng có thể thay đổi

                    int affectedRows = await _context.SaveChangesAsync();

                    if (affectedRows > 0)
                    {
                        _logger.LogInformation("Successfully updated Cuonsach with ID: {Id}", cuonSach.Id);
                    }
                    else
                    {
                        _logger.LogInformation("UpdateAsync for Cuonsach ID: {Id} completed but no rows were affected (potentially no actual changes detected).", cuonSach.Id);
                    }
                    return affectedRows > 0;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error updating Cuonsach (ID: {Id}).", cuonSach.Id);
                    throw;
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "DbUpdateException updating Cuonsach (ID: {Id}). Details: {ErrorMessage}", cuonSach.Id, ex.InnerException?.Message ?? ex.Message);
                    throw new Exception($"Lỗi cơ sở dữ liệu khi cập nhật cuốn sách ID: {cuonSach.Id}.", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error updating Cuonsach (ID: {Id}).", cuonSach.Id);
                    throw new Exception($"Lỗi không xác định khi cập nhật cuốn sách ID: {cuonSach.Id}.", ex);
                }
            }
        }

        // Xóa mềm cuốn sách (Set DaAn = 1)
        public async Task<bool> SoftDeleteAsync(int id)
        {
            _logger.LogInformation("Attempting to soft delete Cuonsach with ID: {Id}", id);
            if (id <= 0)
            {
                _logger.LogError("Invalid Cuonsach ID provided for soft delete: {Id}", id);
                throw new ArgumentException("ID cuốn sách không hợp lệ để xóa mềm.", nameof(id));
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Dùng ExecuteUpdate để xóa mềm hiệu quả
                    int affectedRows = await _context.Cuonsach
                        .Where(cs => cs.Id == id && cs.DaAn == 0) // Chỉ xóa mềm cuốn sách chưa bị xóa
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(cs => cs.DaAn, 1) // Đặt DaAn = 1
                                                           // VUI LÒNG THAY THẾ "TinhTrang" VÀ "3" BẰNG TÊN THUỘC TÍNH VÀ GIÁ TRỊ CHO TRẠNG THÁI "ĐÃ XÓA MỀM" (ví dụ: 3 hoặc giá trị khác)
                            .SetProperty(cs => cs.TinhTrang, 3) // Giả định 3 là trạng thái "Đã thanh lý/xóa"
                        );

                    if (affectedRows > 0)
                    {
                        _logger.LogInformation("Successfully soft deleted Cuonsach with ID: {Id}", id);
                    }
                    else
                    {
                        _logger.LogInformation("Soft delete failed: Cuonsach with ID {Id} not found or already deleted.", id);
                    }
                    return affectedRows > 0; // True nếu có đúng 1 dòng bị ảnh hưởng
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "DbUpdateException soft deleting Cuonsach (ID: {Id}). Details: {ErrorMessage}", id, ex.InnerException?.Message ?? ex.Message);
                    throw new Exception($"Lỗi cơ sở dữ liệu khi xóa mềm cuốn sách ID: {id}.", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error soft deleting Cuonsach (ID: {Id}). Details: {ErrorMessage}", id, ex.Message);
                    throw new Exception($"Lỗi không xác định khi xóa mềm cuốn sách ID {id}.", ex); // Wrap lỗi
                }
            }
        }

        // Phục hồi cuốn sách đã xóa mềm (Set DaAn = 0)
        public async Task<bool> RestoreAsync(int id)
        {
            _logger.LogInformation("Attempting to restore Cuonsach with ID: {Id}", id);
            if (id <= 0)
            {
                _logger.LogError("Invalid Cuonsach ID provided for restore: {Id}", id);
                throw new ArgumentException("ID cuốn sách không hợp lệ để phục hồi.", nameof(id));
            }
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Dùng ExecuteUpdate để phục hồi hiệu quả
                    int affectedRows = await _context.Cuonsach
                        .Where(cs => cs.Id == id && cs.DaAn != 0) // Chỉ phục hồi cuốn sách đang bị xóa (DaAn = 1)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(cs => cs.DaAn, 0) // Đặt DaAn = 0
                                                           // VUI LÒNG THAY THẾ "TinhTrang" VÀ "0" BẰNG TÊN THUỘC TÍNH VÀ GIÁ TRỊ CHO TRẠNG THÁI "CÓ SẴN" KHI PHỤC HỒI
                            .SetProperty(cs => cs.TinhTrang, 0) // Giả định 0 là trạng thái "Có sẵn"
                        );

                    if (affectedRows > 0)
                    {
                        _logger.LogInformation("Successfully restored Cuonsach with ID: {Id}", id);
                    }
                    else
                    {
                        _logger.LogInformation("Restore failed: Cuonsach with ID {Id} not found or not soft-deleted.", id);
                    }
                    return affectedRows > 0;
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "DbUpdateException restoring Cuonsach (ID: {Id}). Details: {ErrorMessage}", id, ex.InnerException?.Message ?? ex.Message);
                    throw new Exception($"Lỗi cơ sở dữ liệu khi phục hồi cuốn sách ID: {id}.", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error restoring Cuonsach (ID: {Id}). Details: {ErrorMessage}", id, ex.Message);
                    throw new Exception($"Lỗi không xác định khi phục hồi cuốn sách ID {id}.", ex); // Wrap lỗi
                }
            }
        }


        // Xóa vĩnh viễn cuốn sách (Cẩn thận với ràng buộc FK)
        public async Task<bool> HardDeleteAsync(int id)
        {
            _logger.LogWarning("Attempting to HARD DELETE Cuonsach with ID: {Id}. This is irreversible!", id);
            if (id <= 0)
            {
                _logger.LogError("Invalid Cuonsach ID provided for hard delete: {Id}", id);
                throw new ArgumentException("ID cuốn sách không hợp lệ để xóa vĩnh viễn.", nameof(id));
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                // Nên dùng transaction nếu bạn muốn thực hiện kiểm tra ràng buộc trước khi xóa
                // using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Kiểm tra ràng buộc ở BUS hoặc ở đây (nhưng nên ở BUS)
                    // if (await HasAnyRelatedReferencesAsync(id)) // Gọi hàm kiểm tra (phải được implement)
                    // {
                    //     throw new InvalidOperationException($"Không thể xóa vĩnh viễn cuốn sách ID: {id} do còn dữ liệu liên quan.");
                    // }

                    // Dùng ExecuteDeleteAsync để xóa hiệu quả
                    int affectedRows = await _context.Cuonsach
                        .Where(cs => cs.Id == id) // Tìm theo ID
                        .ExecuteDeleteAsync();

                    if (affectedRows > 0)
                    {
                        _logger.LogWarning("Successfully HARD DELETED Cuonsach with ID: {Id}", id);
                        // await transaction.CommitAsync();
                        return true;
                    }
                    else
                    {
                        _logger.LogInformation("Hard delete failed: Cuonsach with ID {Id} not found.", id);
                        // await transaction.RollbackAsync();
                        return false;
                    }
                }
                catch (InvalidOperationException ioEx) // Bắt lỗi từ check ràng buộc (nếu có)
                {
                    // await transaction.RollbackAsync();
                    _logger.LogError(ioEx, "Business rule violation during hard delete of Cuonsach (ID: {Id}).", id);
                    throw; // Ném lại lỗi nghiệp vụ
                }
                catch (DbUpdateException ex) // Bắt lỗi DB, thường là FK constraint
                {
                    // await transaction.RollbackAsync();
                    _logger.LogError(ex, "DbUpdateException hard deleting Cuonsach (ID: {Id}). Might be due to FK constraints. Details: {ErrorMessage}", id, ex.InnerException?.Message ?? ex.Message);
                    throw new InvalidOperationException($"Không thể xóa vĩnh viễn cuốn sách ID: {id} do có ràng buộc với dữ liệu khác.", ex); // Thông báo chung cho lỗi FK
                }
                catch (Exception ex)
                {
                    // await transaction.RollbackAsync();
                    _logger.LogError(ex, "Unexpected error hard deleting Cuonsach (ID: {Id}). Details: {ErrorMessage}", id, ex.Message);
                    throw new Exception($"Lỗi không xác định khi xóa vĩnh viễn cuốn sách ID {id}.", ex); // Wrap lỗi
                }
            }
        }

        // <<< TRIỂN KHAI PHƯƠNG THỨC HasAnyRelatedReferencesAsync >>>
        // Cần có trong interface IDALCuonSach nếu BUS gọi
        public async Task<bool> HasAnyRelatedReferencesAsync(int cuonSachId)
        {
            _logger.LogInformation("Checking for related references for Cuonsach ID: {Id}", cuonSachId);
            if (cuonSachId <= 0) return false;

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Kiểm tra trong bảng Phieumuontra
                    // >>> VUI LÒNG SỬA TÊN DbSet NẾU KHÔNG PHẢI 'Phieumuontra' <<<
                    bool hasBorrowRecords = await _context.Phieumuontra.AnyAsync(pmt => pmt.IdCuonSach == cuonSachId);
                    if (hasBorrowRecords)
                    {
                        _logger.LogInformation("Found related records in Phieumuontra for Cuonsach ID: {Id}", cuonSachId);
                        return true;
                    }

                    // Kiểm tra trong bảng Bcsachtratre
                    // >>> VUI LÒNG SỬA TÊN DbSet NẾU KHÔNG PHẢI 'Bcsachtratre' <<<
                    bool hasOverdueReports = await _context.Bcsachtratre.AnyAsync(bcs => bcs.IdCuonSach == cuonSachId);
                    if (hasOverdueReports)
                    {
                        _logger.LogInformation("Found related records in Bcsachtratre for Cuonsach ID: {Id}", cuonSachId);
                        return true;
                    }

                    // Thêm kiểm tra các bảng khác nếu cần

                    _logger.LogInformation("No related references found for Cuonsach ID: {Id}", cuonSachId);
                    return false; // Không tìm thấy liên quan
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking related references for Cuonsach ID: {Id}", cuonSachId);
                    throw new Exception($"Lỗi DAL khi kiểm tra ràng buộc cho cuốn sách ID {cuonSachId}.", ex);
                }
            }
        }

        // <<< TRIỂN KHAI PHƯƠNG THỨC HasActiveCopiesAsync >>>
        // Cần có trong interface IDALCuonSach nếu BUS gọi
        public async Task<bool> HasActiveCopiesAsync(int sachId) // <-- Tên này khớp với IDALCuonSach
        {
            _logger.LogInformation("Checking for active CuonSach entries for Sach ID: {SachId} (Implementing IDALCuonSach.HasActiveCopiesAsync)", sachId);
            if (sachId <= 0) return false;

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Kiểm tra CuonSach có IdSach = sachId VÀ chưa bị xóa mềm (DaAn == 0)
                    // >>> VUI LÒNG THAY THẾ "TinhTrang" NẾU TÊN THUỘC TÍNH KHÁC <<<
                    return await _context.Cuonsach.AnyAsync(cs => cs.IdSach == sachId && cs.DaAn == 0); // Chỉ cần kiểm tra DaAn == 0 là đủ cho "active" theo nghĩa tồn tại
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALCuonSach.HasActiveCopiesAsync (SachId: {SachId}). Details: {ErrorMessage}", sachId, ex.InnerException?.Message ?? ex.Message);
                    throw new Exception($"Lỗi DAL khi kiểm tra tồn tại cuốn sách vật lý đang hoạt động theo ấn bản ID {sachId}: {ex.Message}", ex); // Wrap lỗi DAL
                }
            }
        }

        // *** TRIỂN KHAI PHƯƠNG THỨC GetSoLuongCuonSachDangMuonBySachIdAsync ***
        public async Task<int> GetSoLuongCuonSachDangMuonBySachIdAsync(int sachId)
        {
            _logger.LogInformation("Attempting to get count of borrowed CuonSach for Sach ID: {SachId} (Implementing IDALCuonSach method)", sachId);

            if (sachId <= 0)
            {
                _logger.LogWarning("Invalid Sach ID provided for GetSoLuongCuonSachDangMuonBySachIdAsync: {SachId}", sachId);
                return 0; // Trả về 0 cho ID không hợp lệ
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Truy vấn bảng Cuonsach
                    // Lọc theo IdSach
                    // Lọc theo trạng thái 'Đang mượn'.
                    // VUI LÒNG THAY THẾ "TinhTrang" VÀ "1" BẰNG TÊN THUỘC TÍNH VÀ GIÁ TRỊ THỰC TẾ CHO TRẠNG THÁI "ĐANG MƯỢN"
                    int count = await _context.Cuonsach
                        .CountAsync(cs => cs.IdSach == sachId
                                      && cs.TinhTrang == 1 // <<< Giả định 1 là "Đang mượn"
                                                           // && cs.DaAn == 0 // <<< Bỏ comment nếu cần lọc CuonSach chưa xóa mềm
                                       );

                    _logger.LogInformation("Found {Count} borrowed CuonSach for Sach ID: {SachId}", count, sachId);

                    return count;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting count of borrowed CuonSach for Sach ID: {SachId}. Details: {ErrorMessage}", sachId, ex.InnerException?.Message ?? ex.Message);
                    // Ném lại lỗi để tầng BUS xử lý
                    throw new Exception($"Lỗi cơ sở dữ liệu khi đếm số lượng cuốn sách đang mượn của sách ID {sachId}.", ex);
                }
            }
        }
        // *** KẾT THÚC PHƯƠNG THỨC ***

        // Phương thức này có thể không cần thiết nếu đã có HasActiveCopiesAsync và HasAnyCopiesAsync trong interface
        public async Task<bool> HasAnyCuonSachBySachIdIncludingDeletedAsync(int sachId)
        {
            _logger.LogInformation("Checking for any CuonSach entries (including deleted) for Sach ID: {SachId} (Helper method)", sachId);
            if (sachId <= 0) return false;

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    return await _context.Cuonsach.AnyAsync(cs => cs.IdSach == sachId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking for any CuonSach entries (including deleted) for Sach ID: {SachId}", sachId);
                    throw new Exception($"Lỗi DAL khi kiểm tra tồn tại cuốn sách vật lý theo ấn bản ID {sachId}: {ex.Message}", ex);
                }
            }
        }


        // TODO: Implement other methods from IDALCuonSach if any are missing or needed
    } // Kết thúc class DALCuonSach
} // Kết thúc namespace DAL