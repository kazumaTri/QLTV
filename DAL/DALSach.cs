// Project/Namespace: DAL

// --- USING ---
using DAL.Models; // Namespace chứa các Entity models (Sach, Tuasach, Theloai, Tacgia, Cuonsach)
using Microsoft.EntityFrameworkCore; // Cần cho DbContext, các hàm async của EF, EntityState, ExecuteUpdateAsync, ExecuteDeleteAsync
using Microsoft.Extensions.DependencyInjection; // Cần cho IServiceScopeFactory, GetRequiredService
using Microsoft.Extensions.Logging; // Cần cho ILogger, LogInformation, LogWarning, LogError
using System; // Cần cho ArgumentNullException, InvalidOperationException, Exception
using System.Collections.Generic; // Cần cho List<T>, IEnumerable<T>
using System.Linq; // Cần cho các phương thức LINQ (Where, Select, ToList, Any, Count, Except, FirstOrDefault)
using System.Threading.Tasks; // Cần cho async/await Task, Task.Delay (nếu cần mô phỏng trễ)

namespace DAL
{
    /// <summary>
    /// Data Access Layer triển khai IDALSach, tương tác với DB thông qua EF Core.
    /// Sử dụng IServiceScopeFactory để quản lý vòng đời DbContext cho từng thao tác.
    /// </summary>
    public class DALSach : IDALSach // <<< Triển khai interface IDALSach
    {
        // --- DEPENDENCIES ---
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DALSach> _logger; // Thêm Logger

        // --- CONSTRUCTOR (Sử dụng Dependency Injection) ---
        public DALSach(IServiceScopeFactory scopeFactory, ILogger<DALSach> logger) // Inject Logger
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Khởi tạo Logger
            _logger.LogInformation("DALSach initialized."); // Log khi khởi tạo
        }

        // --- METHODS ---

        /// <summary>
        /// Lấy tất cả các bản ghi Sach từ cơ sở dữ liệu, bao gồm các navigation properties cần thiết cho DTO mapping.
        /// Chỉ lấy các sách chưa bị xóa mềm (DaAn = 0).
        /// </summary>
        /// <returns>Task chứa danh sách các Entity Sach.</returns>
        public async Task<List<Sach>> GetAllAsync()
        {
            _logger.LogInformation("Attempting to retrieve all non-deleted Sach entities.");
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var sachs = await _context.Sach
                        .AsNoTracking() // Không theo dõi thay đổi cho các thao tác chỉ đọc
                        .Where(s => s.DaAn == 0) // Lọc sách chưa bị xóa mềm
                                                 // Include navigation properties cần thiết cho DTO
                        .Include(s => s.IdTuaSachNavigation) // Bao gồm thông tin Tựa sách
                            .ThenInclude(ts => ts.IdTheLoaiNavigation) // Và thông tin Thể loại từ Tựa sách
                        .Include(s => s.IdTuaSachNavigation) // Bao gồm lại thông tin Tựa sách (độc lập Include)
                            .ThenInclude(ts => ts.IdTacGia) // Và danh sách Tác giả từ Tựa sách
                                                            // .Include(s => s.IdNhaXuatBanNavigation) // Giả định không còn FK đến NhaXuatBan
                        .ToListAsync();

                    _logger.LogInformation("Successfully retrieved {Count} non-deleted Sach entities.", sachs.Count);
                    return sachs;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALSach.GetAllAsync.");
                    // Wrap exception để cung cấp thông báo tầng DAL
                    throw new Exception("Lỗi khi lấy danh sách sách từ cơ sở dữ liệu.", ex);
                }
            }
        }

        /// <summary>
        /// Lấy một bản ghi Sach theo ID, bao gồm các navigation properties cần thiết.
        /// Chỉ lấy sách chưa bị xóa mềm (DaAn = 0).
        /// </summary>
        /// <param name="id">ID của sách cần lấy.</param>
        /// <returns>Task chứa Entity Sach hoặc null nếu không tìm thấy hoặc ID không hợp lệ.</returns>
        public async Task<Sach?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve non-deleted Sach with ID: {SachId}", id);
            // Kiểm tra ID hợp lệ trước khi truy vấn
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID provided for GetByIdAsync: {SachId}", id);
                return null;
            }
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var sach = await _context.Sach
                        .AsNoTracking() // Không theo dõi thay đổi
                        .Where(s => s.Id == id && s.DaAn == 0) // Lọc theo ID và trạng thái chưa xóa
                                                               // Include navigation properties
                        .Include(s => s.IdTuaSachNavigation)
                            .ThenInclude(ts => ts.IdTheLoaiNavigation)
                        .Include(s => s.IdTuaSachNavigation)
                            .ThenInclude(ts => ts.IdTacGia)
                        .FirstOrDefaultAsync();

                    if (sach == null)
                    {
                        _logger.LogInformation("Non-deleted Sach with ID: {SachId} not found.", id);
                    }
                    else
                    {
                        _logger.LogInformation("Successfully retrieved non-deleted Sach with ID: {SachId}", id);
                    }
                    return sach;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALSach.GetByIdAsync (ID: {SachId}).", id);
                    // Wrap exception
                    throw new Exception($"Lỗi khi lấy sách theo ID: {id}.", ex);
                }
            }
        }

        /// <summary>
        /// Lấy một bản ghi Sach theo ID, bao gồm các navigation properties cần thiết.
        /// Bao gồm cả các sách đã bị xóa mềm (DaAn = 1).
        /// </summary>
        /// <param name="id">ID của sách cần lấy.</param>
        /// <returns>Task chứa Entity Sach hoặc null nếu không tìm thấy hoặc ID không hợp lệ.</returns>
        public async Task<Sach?> GetByIdIncludingDeletedAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve Sach (including deleted) with ID: {SachId}", id);
            // Kiểm tra ID hợp lệ
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID provided for GetByIdIncludingDeletedAsync: {SachId}", id);
                return null;
            }
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var sach = await _context.Sach
                        .AsNoTracking() // Không theo dõi thay đổi
                        .Where(s => s.Id == id) // Chỉ lọc theo ID, không quan tâm DaAn
                                                // Include navigation properties
                         .Include(s => s.IdTuaSachNavigation)
                            .ThenInclude(ts => ts.IdTheLoaiNavigation)
                        .Include(s => s.IdTuaSachNavigation)
                            .ThenInclude(ts => ts.IdTacGia)
                        .FirstOrDefaultAsync();

                    if (sach == null)
                    {
                        _logger.LogInformation("Sach (including deleted) with ID: {SachId} not found.", id);
                    }
                    else
                    {
                        _logger.LogInformation("Successfully retrieved Sach (including deleted) with ID: {SachId}", id);
                    }
                    return sach;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALSach.GetByIdIncludingDeletedAsync (ID: {SachId}).", id);
                    // Wrap exception
                    throw new Exception($"Lỗi khi lấy sách (bao gồm đã xóa) với ID: {id}.", ex);
                }
            }
        }

        /// <summary>
        /// Lấy một bản ghi Sach theo Mã sách (MaSach), bao gồm các navigation properties cần thiết.
        /// Chỉ lấy sách chưa bị xóa mềm (DaAn = 0).
        /// </summary>
        /// <param name="maSach">Mã sách cần lấy.</param>
        /// <returns>Task chứa Entity Sach hoặc null nếu không tìm thấy hoặc Mã sách không hợp lệ/trống.</returns>
        public async Task<Sach?> GetByMaAsync(string maSach)
        {
            _logger.LogInformation("Attempting to retrieve non-deleted Sach with MaSach: {MaSach}", maSach);
            // Kiểm tra Mã sách hợp lệ
            if (string.IsNullOrWhiteSpace(maSach))
            {
                _logger.LogWarning("Null or whitespace MaSach provided for GetByMaAsync.");
                return null;
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var sach = await _context.Sach
                        .AsNoTracking() // Không theo dõi thay đổi
                        .Where(s => s.MaSach == maSach.Trim() && s.DaAn == 0) // Lọc theo Mã và trạng thái chưa xóa, trim Mã
                                                                              // Include navigation properties
                        .Include(s => s.IdTuaSachNavigation)
                            .ThenInclude(ts => ts.IdTheLoaiNavigation)
                        .Include(s => s.IdTuaSachNavigation)
                            .ThenInclude(ts => ts.IdTacGia)
                        .FirstOrDefaultAsync();

                    if (sach == null)
                    {
                        _logger.LogInformation("Non-deleted Sach with MaSach: {MaSach} not found.", maSach.Trim());
                    }
                    else
                    {
                        _logger.LogInformation("Successfully retrieved non-deleted Sach with MaSach: {MaSach}", maSach.Trim());
                    }
                    return sach;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALSach.GetByMaAsync (MaSach: {MaSach}).", maSach.Trim());
                    // Wrap exception
                    throw new Exception($"Lỗi khi lấy sách theo mã: {maSach.Trim()}.", ex);
                }
            }
        }

        /// <summary>
        /// Lấy một bản ghi Sach theo Mã sách (MaSach), bao gồm các navigation properties cần thiết.
        /// Bao gồm cả các sách đã bị xóa mềm (DaAn = 1).
        /// </summary>
        /// <param name="maSach">Mã sách cần lấy.</param>
        /// <returns>Task chứa Entity Sach hoặc null nếu không tìm thấy hoặc Mã sách không hợp lệ/trống.</returns>
        public async Task<Sach?> GetByMaIncludingDeletedAsync(string maSach)
        {
            _logger.LogInformation("Attempting to retrieve Sach (including deleted) with MaSach: {MaSach}", maSach);
            // Kiểm tra Mã sách hợp lệ
            if (string.IsNullOrWhiteSpace(maSach))
            {
                _logger.LogWarning("Null or whitespace MaSach provided for GetByMaIncludingDeletedAsync.");
                return null;
            }
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var sach = await _context.Sach
                        .AsNoTracking() // Không theo dõi thay đổi
                        .Where(s => s.MaSach == maSach.Trim()) // Chỉ lọc theo Mã, không quan tâm DaAn, trim Mã
                                                               // Include navigation properties
                         .Include(s => s.IdTuaSachNavigation)
                            .ThenInclude(ts => ts.IdTheLoaiNavigation)
                        .Include(s => s.IdTuaSachNavigation)
                            .ThenInclude(ts => ts.IdTacGia)
                        .FirstOrDefaultAsync();

                    if (sach == null)
                    {
                        _logger.LogInformation("Sach (including deleted) with MaSach: {MaSach} not found.", maSach.Trim());
                    }
                    else
                    {
                        _logger.LogInformation("Successfully retrieved Sach (including deleted) with MaSach: {MaSach}", maSach.Trim());
                    }
                    return sach;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALSach.GetByMaIncludingDeletedAsync (MaSach: {MaSach}).", maSach.Trim());
                    // Wrap exception
                    throw new Exception($"Lỗi khi lấy sách (bao gồm đã xóa) theo mã: {maSach.Trim()}.", ex);
                }
            }
        }


        /// <summary>
        /// Lấy danh sách các bản ghi Sach thuộc một Tựa sách cụ thể theo ID Tựa sách.
        /// Chỉ lấy các sách chưa bị xóa mềm (DaAn = 0).
        /// </summary>
        /// <param name="tuaSachId">ID của Tựa sách.</param>
        /// <returns>Task chứa danh sách các Entity Sach thuộc Tựa sách đó.</returns>
        public async Task<List<Sach>> GetByTuaSachIdAsync(int tuaSachId)
        {
            _logger.LogInformation("Attempting to retrieve non-deleted Sach entities for TuaSachId: {TuaSachId}", tuaSachId);
            // Kiểm tra ID Tựa sách hợp lệ
            if (tuaSachId <= 0)
            {
                _logger.LogWarning("Invalid TuaSachId provided for GetByTuaSachIdAsync: {TuaSachId}", tuaSachId);
                return new List<Sach>(); // Trả về danh sách rỗng cho ID không hợp lệ
            }
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    var sachs = await _context.Sach
                        .AsNoTracking() // Không theo dõi thay đổi
                        .Where(s => s.IdTuaSach == tuaSachId && s.DaAn == 0) // Lọc theo IdTuaSach và trạng thái chưa xóa
                                                                             // Include navigation properties
                        .Include(s => s.IdTuaSachNavigation)
                            .ThenInclude(ts => ts.IdTheLoaiNavigation)
                        .Include(s => s.IdTuaSachNavigation)
                            .ThenInclude(ts => ts.IdTacGia)
                        .ToListAsync();

                    _logger.LogInformation("Successfully retrieved {Count} non-deleted Sach entities for TuaSachId: {TuaSachId}", sachs.Count, tuaSachId);
                    return sachs;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALSach.GetByTuaSachIdAsync (TuaSachId: {TuaSachId}).", tuaSachId);
                    // Wrap exception
                    throw new Exception($"Lỗi khi lấy sách theo tựa sách ID: {tuaSachId}.", ex);
                }
            }
        }


        /// <summary>
        /// Lấy tổng số lượng bản ghi Sach chưa bị xóa mềm (DaAn = 0).
        /// </summary>
        /// <returns>Task chứa tổng số lượng.</returns>
        public async Task<int> GetTotalCountAsync()
        {
            _logger.LogInformation("Attempting to get total count of non-deleted Sach entities.");
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Đếm số lượng bản ghi thỏa điều kiện
                    int count = await _context.Sach.CountAsync(s => s.DaAn == 0);
                    _logger.LogInformation("Total count of non-deleted Sach entities: {Count}", count);
                    return count;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DALSach.GetTotalCountAsync.");
                    // Wrap exception
                    throw new Exception("Lỗi khi đếm tổng số sách.", ex);
                }
            }
        }

        /// <summary>
        /// Thêm mới một bản ghi Sach vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="sach">Entity Sach cần thêm.</param>
        /// <returns>Task chứa Entity Sach đã được thêm (có ID tự sinh) hoặc null nếu thêm thất bại (trừ trường hợp ném exception).</returns>
        /// <exception cref="ArgumentNullException">Ném ra nếu entity đầu vào là null.</exception>
        /// <exception cref="DbUpdateException">Ném ra nếu có lỗi DB trong quá trình lưu.</exception>
        /// <exception cref="Exception">Ném ra cho các lỗi không xác định khác.</exception>
        public async Task<Sach?> AddAsync(Sach sach)
        {
            _logger.LogInformation("Attempting to add a new Sach.");
            // Kiểm tra entity đầu vào null
            if (sach == null)
            {
                _logger.LogError("Attempted to add a null Sach entity.");
                throw new ArgumentNullException(nameof(sach), "Entity Sach cần thêm không được rỗng.");
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();

                // Đảm bảo không attach các navigation properties khi Add
                // EF Core sẽ tự xử lý FK dựa trên IdTuaSach
                sach.IdTuaSachNavigation = null!; // Sử dụng null-forgiving operator

                // Đảm bảo các collection properties được khởi tạo để tránh lỗi nếu BUS/mapping không set
                // Giả định bạn đã đổi tên thuộc tính trong Sach.cs thành số nhiều: CtPhieunhaps, Cuonsachs
                // Nếu tên trong Sach.cs vẫn là số ít (CtPhieunhap, Cuonsach) thì sửa ở đây thành số ít
                sach.CtPhieunhap ??= new List<CtPhieunhap>(); // Khởi tạo nếu null
                sach.Cuonsach ??= new List<Cuonsach>(); // Khởi tạo nếu null

                // Đảm bảo trạng thái ban đầu khi thêm là chưa xóa (nếu BUS chưa set hoặc để phòng thủ)
                sach.DaAn = 0; // 0 hoặc false tùy kiểu dữ liệu trong DB (tinyint/bit)

                try
                {
                    await _context.Sach.AddAsync(sach); // Thêm entity vào context
                    int affectedRows = await _context.SaveChangesAsync(); // Lưu thay đổi vào DB
                    if (affectedRows > 0)
                    {
                        // Log ID tự sinh sau khi lưu
                        _logger.LogInformation("Successfully added Sach with generated ID: {SachId}", sach.Id);
                        return sach; // Trả về entity đã có ID (được EF populate sau SaveChanges)
                    }
                    else
                    {
                        _logger.LogWarning("AddAsync for Sach completed but no rows were affected.");
                        return null; // Thêm thất bại (không có dòng nào bị ảnh hưởng)
                    }
                }
                catch (DbUpdateException ex) // Bắt lỗi liên quan đến DB (constraint, FK, etc.)
                {
                    _logger.LogError(ex, "DbUpdateException adding Sach. MaSach: {MaSach}, Details: {ErrorMessage}", sach.MaSach, ex.InnerException?.Message ?? ex.Message);
                    // Kiểm tra lỗi UNIQUE constraint để ném exception rõ ràng hơn
                    // Chuỗi lỗi cụ thể có thể khác nhau tùy loại DB (SQL Server, MySQL, PostgreSql...)
                    if (ex.InnerException?.Message?.Contains("UNIQUE constraint") == true || ex.InnerException?.Message?.Contains("duplicate key") == true)
                    {
                        // Ném lỗi nghiệp vụ (đã được wrap ở DAL) cho tầng BUS
                        // Sử dụng InvalidOperationException như đã thống nhất ở tầng BUS
                        throw new InvalidOperationException($"Mã sách '{sach.MaSach?.Trim()}' đã tồn tại. Vui lòng chọn mã khác.", ex);
                    }
                    // Wrap lỗi DB khác
                    throw new Exception("Lỗi cơ sở dữ liệu khi thêm sách.", ex);
                }
                catch (Exception ex) // Bắt các lỗi không xác định khác
                {
                    _logger.LogError(ex, "Unexpected error adding Sach. MaSach: {MaSach}", sach.MaSach);
                    // Wrap lỗi không xác định
                    throw new Exception("Lỗi không xác định khi thêm sách.", ex);
                }
            }
        }

        /// <summary>
        /// Cập nhật thông tin của một bản ghi Sach.
        /// EF sẽ tìm bản ghi gốc theo ID và cập nhật các thuộc tính từ entity đầu vào.
        /// </summary>
        /// <param name="sach">Entity Sach chứa thông tin cập nhật (phải có ID hợp lệ).</param>
        /// <returns>Task chứa bool, true nếu cập nhật thành công (có dòng bị ảnh hưởng), false nếu không tìm thấy sách hoặc không có thay đổi.</returns>
        /// <exception cref="ArgumentNullException">Ném ra nếu entity đầu vào là null.</exception>
        /// <exception cref="ArgumentException">Ném ra nếu ID sách không hợp lệ.</exception>
        /// <exception cref="DbUpdateConcurrencyException">Ném ra nếu có lỗi đồng thời.</exception>
        /// <exception cref="DbUpdateException">Ném ra nếu có lỗi DB khác.</exception>
        /// <exception cref="Exception">Ném ra cho các lỗi không xác định khác.</exception>
        public async Task<bool> UpdateAsync(Sach sach)
        {
            _logger.LogInformation("Attempting to update Sach with ID: {SachId}", sach?.Id);
            // Kiểm tra entity đầu vào null và ID hợp lệ
            if (sach == null)
            {
                _logger.LogError("Attempted to update a null Sach entity.");
                throw new ArgumentNullException(nameof(sach), "Thông tin sách cần cập nhật không được rỗng.");
            }
            if (sach.Id <= 0)
            {
                _logger.LogError("Invalid Sach ID provided for update: {SachId}", sach.Id);
                throw new ArgumentException("ID sách không hợp lệ để cập nhật.", nameof(sach.Id));
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Lấy entity đang được track bởi context (nếu có) hoặc từ DB.
                    // Không dùng AsNoTracking ở đây vì cần entity được track để EF phát hiện thay đổi.
                    // Lấy cả sách đã xóa mềm nếu BUS cho phép cập nhật sách đã xóa mềm.
                    // Giả định UpdateAsync chỉ cho phép cập nhật sách CHƯA xóa mềm:
                    var existingSach = await _context.Sach.FirstOrDefaultAsync(s => s.Id == sach.Id && s.DaAn == 0);

                    if (existingSach == null)
                    {
                        _logger.LogInformation("Update failed: Sach with ID {SachId} not found or already deleted.", sach.Id);
                        return false; // Không tìm thấy sách hoặc sách đã bị xóa mềm
                    }

                    // Cập nhật thủ công các thuộc tính được phép cập nhật từ 'sach' vào 'existingSach'
                    // KHÔNG cập nhật ID
                    // KHÔNG cập nhật DaAn (trừ khi đây là hàm Restore/SoftDelete)
                    // KHÔNG cập nhật các collection (CtPhieunhap, Cuonsach) ở đây
                    // Cẩn thận với Mã sách: nếu Mã sách là computed column hoặc UI không cho sửa, không cập nhật trường này.
                    // Nếu Mã sách có thể sửa và được gửi từ DTO:
                    existingSach.MaSach = sach.MaSach?.Trim();
                    // Nếu Mã sách KHÔNG thể sửa:
                    // existingSach.MaSach = existingSach.MaSach; // Giữ nguyên giá trị cũ

                    existingSach.IdTuaSach = sach.IdTuaSach;
                    // SoLuong và SoLuongConLai: Nếu UI/BUS cho phép sửa, cập nhật. Nếu không, giữ nguyên.
                    // Giả định UI/BUS có thể sửa SoLuong/SoLuongConLai (như trong DTO của BUS):
                    existingSach.SoLuong = sach.SoLuong;
                    existingSach.SoLuongConLai = sach.SoLuongConLai;
                    // Giả định UI/BUS KHÔNG sửa SoLuong/SoLuongConLai ở màn hình này:
                    // existingSach.SoLuong = existingSach.SoLuong; // Giữ nguyên
                    // existingSach.SoLuongConLai = existingSach.SoLuongConLai; // Giữ nguyên

                    existingSach.DonGia = sach.DonGia;
                    existingSach.NamXb = sach.NamXb; // <<< Tên thuộc tính đúng
                    existingSach.NhaXb = sach.NhaXb?.Trim(); // <<< Tên thuộc tính đúng, trim khoảng trắng

                    // EF Core sẽ tự phát hiện các thay đổi trên 'existingSach' khi SaveChanges
                    int affectedRows = await _context.SaveChangesAsync();

                    // affectedRows > 0 nếu có bất kỳ thuộc tính nào thay đổi so với giá trị gốc trong DB
                    if (affectedRows > 0)
                    {
                        _logger.LogInformation("Successfully updated Sach with ID: {SachId}", sach.Id);
                    }
                    else
                    {
                        _logger.LogInformation("UpdateAsync for Sach ID: {SachId} completed but no rows were affected (potentially no actual changes detected).", sach.Id);
                    }
                    return affectedRows > 0; // Trả về true nếu có ít nhất một dòng bị ảnh hưởng
                }
                catch (DbUpdateConcurrencyException ex) // Xử lý lỗi đồng thời
                {
                    _logger.LogError(ex, "Concurrency error updating Sach (ID: {SachId}).", sach.Id);
                    // Ném lại lỗi đồng thời để tầng BUS/GUI xử lý (ví dụ: hiển thị thông báo hoặc reload dữ liệu)
                    throw;
                }
                catch (DbUpdateException ex) // Bắt lỗi liên quan đến DB khác (constraint, FK, etc.)
                {
                    _logger.LogError(ex, "DbUpdateException updating Sach (ID: {SachId}). Details: {ErrorMessage}", sach.Id, ex.InnerException?.Message ?? ex.Message);
                    // Wrap lỗi DB
                    throw new Exception($"Lỗi cơ sở dữ liệu khi cập nhật sách ID: {sach.Id}.", ex);
                }
                catch (Exception ex) // Bắt các lỗi không xác định khác
                {
                    _logger.LogError(ex, "Unexpected error updating Sach (ID: {SachId}).", sach.Id);
                    // Wrap lỗi không xác định
                    throw new Exception($"Lỗi không xác định khi cập nhật sách ID: {sach.Id}.", ex);
                }
            }
        }

        /// <summary>
        /// Cập nhật thông tin cho nhiều đối tượng Sách cùng lúc.
        /// Thường dùng để cập nhật số lượng sau khi nhập sách.
        /// Triển khai phương thức từ IDALSach.
        /// </summary>
        /// <param name="sachs">Danh sách các đối tượng Sách cần cập nhật (phải có ID hợp lệ).</param>
        /// <returns>Task chứa bool, true nếu cập nhật thành công (ít nhất 1 dòng ảnh hưởng), false nếu danh sách rỗng hoặc có lỗi.</returns>
        /// <exception cref="DbUpdateConcurrencyException">Ném ra nếu có lỗi đồng thời.</exception>
        /// <exception cref="DbUpdateException">Ném ra nếu có lỗi DB khác.</exception>
        /// <exception cref="Exception">Ném ra cho các lỗi không xác định khác.</exception>
        public async Task<bool> UpdateRangeAsync(List<Sach> sachs)
        {
            _logger.LogInformation("Attempting to update a range of {Count} Sach entities.", sachs?.Count ?? 0);
            if (sachs == null || !sachs.Any())
            {
                _logger.LogInformation("UpdateRangeAsync called with empty or null list of Sach.");
                return true; // Không có gì để làm, coi như thành công
            }

            // Lấy danh sách ID để log (optional)
            var sachIds = sachs.Select(s => s.Id).ToList();
            _logger.LogInformation("Updating Sach entities with IDs: {SachIds}", string.Join(", ", sachIds));

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();

                try
                {
                    // Cách 1: Đơn giản nhất, EF tự xử lý Attach và Modified state
                    _context.Sach.UpdateRange(sachs); // <<< SỬA TỪ .Sachs THÀNH .Sach

                    int affectedRows = await _context.SaveChangesAsync(); // Lưu thay đổi

                    // Kiểm tra xem có dòng nào bị ảnh hưởng không
                    if (affectedRows > 0)
                    {
                        _logger.LogInformation("Successfully updated {AffectedRows} Sach entities in range.", affectedRows);
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("UpdateRangeAsync for Sach completed but no rows were affected. IDs: {SachIds}", string.Join(", ", sachIds));
                        return false; // Không có dòng nào thực sự được cập nhật
                    }
                }
                catch (DbUpdateConcurrencyException ex) // Xử lý lỗi đồng thời
                {
                    _logger.LogError(ex, "Concurrency error during UpdateRangeAsync for Sach.");
                    throw; // Ném lại lỗi
                }
                catch (DbUpdateException ex) // Bắt lỗi DB khác
                {
                    _logger.LogError(ex, "DbUpdateException during UpdateRangeAsync for Sach. Details: {ErrorMessage}", ex.InnerException?.Message ?? ex.Message);
                    throw new Exception("Lỗi cơ sở dữ liệu khi cập nhật hàng loạt sách.", ex); // Wrap lỗi
                }
                catch (Exception ex) // Bắt các lỗi khác
                {
                    _logger.LogError(ex, "Unexpected error during UpdateRangeAsync for Sach.");
                    throw new Exception("Lỗi không xác định khi cập nhật hàng loạt sách.", ex); // Wrap lỗi
                }
            }
        }


        /// <summary>
        /// Xóa mềm một bản ghi Sach bằng cách đặt cờ DaAn = 1 (hoặc true tùy kiểu dữ liệu).
        /// Sử dụng ExecuteUpdateAsync để hiệu quả.
        /// </summary>
        /// <param name="id">ID của sách cần xóa mềm.</param>
        /// <returns>Task chứa bool, true nếu xóa mềm thành công (có 1 dòng bị ảnh hưởng), false nếu không tìm thấy sách hoặc sách đã bị xóa.</returns>
        /// <exception cref="ArgumentException">Ném ra nếu ID không hợp lệ.</exception>
        /// <exception cref="Exception">Ném ra cho các lỗi hệ thống khác.</exception>
        public async Task<bool> SoftDeleteAsync(int id)
        {
            _logger.LogInformation("Attempting to soft delete Sach with ID: {SachId}", id);
            // Kiểm tra ID hợp lệ
            if (id <= 0)
            {
                _logger.LogError("Invalid Sach ID provided for soft delete: {SachId}", id);
                throw new ArgumentException("ID sách không hợp lệ để xóa mềm.", nameof(id));
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Dùng ExecuteUpdateAsync để xóa mềm hiệu quả trên DB
                    int affectedRows = await _context.Sach
                        .Where(s => s.Id == id && s.DaAn == 0) // Chỉ xóa mềm sách chưa bị xóa
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(s => s.DaAn, 1) // Đặt cờ DaAn = 1
                            .SetProperty(s => s.SoLuongConLai, 0) // Optional: Reset số lượng còn lại khi xóa (hoặc theo nghiệp vụ khác)
                        );

                    if (affectedRows > 0)
                    {
                        _logger.LogInformation("Successfully soft deleted Sach with ID: {SachId}", id);
                    }
                    else
                    {
                        _logger.LogInformation("Soft delete failed: Sach with ID {SachId} not found or already deleted.", id);
                    }
                    return affectedRows > 0; // True nếu có đúng 1 dòng bị ảnh hưởng
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error soft deleting Sach (ID: {SachId}). Details: {ErrorMessage}", id, ex.InnerException?.Message ?? ex.Message);
                    // Wrap lỗi
                    throw new Exception($"Lỗi cơ sở dữ liệu khi xóa mềm sách ID: {id}.", ex);
                }
            }
        }

        /// <summary>
        /// Kiểm tra xem một Mã sách (MaSach) đã tồn tại trong bảng Sach hay chưa, bao gồm cả các bản ghi đã xóa mềm.
        /// Thường dùng để validate tính duy nhất của Mã sách.
        /// </summary>
        /// <param name="maSach">Mã sách cần kiểm tra.</param>
        /// <returns>Task chứa bool, true nếu Mã sách tồn tại, false nếu không hoặc Mã sách rỗng/trống.</returns>
        /// <exception cref="Exception">Ném ra cho các lỗi hệ thống khi kiểm tra.</exception>
        public async Task<bool> IsMaSachExistsAsync(string maSach)
        {
            _logger.LogInformation("Checking if MaSach exists (including deleted): {MaSach}", maSach);
            // Kiểm tra Mã sách rỗng/trống
            if (string.IsNullOrWhiteSpace(maSach))
            {
                _logger.LogWarning("Null or whitespace MaSach provided for existence check.");
                return false; // Mã rỗng/trống không coi là tồn tại
            }
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Sử dụng AnyAsync để kiểm tra sự tồn tại hiệu quả
                    bool exists = await _context.Sach.AnyAsync(s => s.MaSach == maSach.Trim()); // Trim Mã khi kiểm tra
                    _logger.LogInformation("MaSach '{MaSach}' existence check (including deleted) result: {Exists}", maSach.Trim(), exists);
                    return exists;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking MaSach existence (including deleted): {MaSach}", maSach.Trim());
                    // Wrap lỗi
                    throw new Exception("Lỗi khi kiểm tra sự tồn tại của mã sách.", ex);
                }
            }
        }

        /// <summary>
        /// Kiểm tra xem một Mã sách (MaSach) đã tồn tại trong bảng Sach hay chưa, loại trừ bản ghi có ID cụ thể.
        /// Bao gồm cả các bản ghi đã xóa mềm. Thường dùng khi cập nhật để kiểm tra trùng Mã.
        /// </summary>
        /// <param name="maSach">Mã sách cần kiểm tra.</param>
        /// <param name="excludeId">ID của bản ghi cần loại trừ khỏi quá trình kiểm tra.</param>
        /// <returns>Task chứa bool, true nếu Mã sách tồn tại ở bản ghi khác ID đã cho, false nếu không hoặc Mã sách rỗng/trống.</returns>
        /// <exception cref="Exception">Ném ra cho các lỗi hệ thống khi kiểm tra.</exception>
        public async Task<bool> IsMaSachExistsExcludingIdAsync(string maSach, int excludeId)
        {
            _logger.LogInformation("Checking if MaSach exists (excluding ID {ExcludeId}, including deleted): {MaSach}", excludeId, maSach);
            // Kiểm tra Mã sách rỗng/trống
            if (string.IsNullOrWhiteSpace(maSach))
            {
                _logger.LogWarning("Null or whitespace MaSach provided for existence check (excluding ID).");
                return false; // Mã rỗng/trống không coi là tồn tại
            }
            // excludeId <= 0 vẫn hợp lệ trong trường hợp kiểm tra (ví dụ khi thêm mới tạm dùng 0), chỉ cần nó không lọc sai.

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Sử dụng AnyAsync để kiểm tra sự tồn tại hiệu quả
                    bool exists = await _context.Sach.AnyAsync(s => s.MaSach == maSach.Trim() && s.Id != excludeId); // Trim Mã và loại trừ ID
                    _logger.LogInformation("MaSach '{MaSach}' existence check (excluding ID {ExcludeId}, including deleted) result: {Exists}", maSach.Trim(), excludeId, exists);
                    return exists;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking MaSach existence (excluding ID {ExcludeId}): {MaSach}", excludeId, maSach.Trim());
                    // Wrap lỗi
                    throw new Exception("Lỗi khi kiểm tra sự tồn tại của mã sách (loại trừ ID {excludeId}).", ex);
                }
            }
        }

        /// <summary>
        /// Kiểm tra xem một Sách (ấn bản) có tồn tại bất kỳ CuonSach nào chưa bị xóa mềm (DaAn = 0) thuộc về nó hay không.
        /// Dùng để kiểm tra ràng buộc khi xóa mềm/xóa cứng Sach.
        /// </summary>
        /// <param name="id">ID của Sach (ấn bản).</param>
        /// <returns>Task chứa bool, true nếu có CuonSach chưa xóa mềm thuộc về Sach này, false nếu ngược lại.</returns>
        /// <exception cref="Exception">Ném ra cho các lỗi hệ thống khi kiểm tra.</exception>
        // <<< SỬA TÊN PHƯƠNG THỨC ĐỂ KHỚP INTERFACE >>>
        public async Task<bool> HasActiveCopiesAsync(int id) // Đã đổi tên từ HasAnyActiveCuonSachBySachIdAsync
        {
            _logger.LogInformation("Checking for active CuonSach entries for Sach ID: {SachId} (Matching interface HasActiveCopiesAsync)", id);
            // Kiểm tra ID hợp lệ
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID provided for HasActiveCopiesAsync check: {SachId}", id);
                return false;
            }
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Giả định Cuonsach có thuộc tính DaAn (kiểu int hoặc byte)
                    bool hasActive = await _context.Cuonsach.AnyAsync(cs => cs.IdSach == id && cs.DaAn == 0); // Lọc theo IdSach và trạng thái chưa xóa mềm của CuonSach
                    _logger.LogInformation("Active CuonSach check for Sach ID {SachId} result: {HasActive}", id, hasActive);
                    return hasActive;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking for active CuonSach entries for Sach ID: {SachId}", id);
                    // Wrap lỗi
                    throw new Exception($"Lỗi khi kiểm tra các bản sao hoạt động của sách ID: {id}.", ex);
                }
            }
        }

        /// <summary>
        /// Kiểm tra xem một Sách (ấn bản) có tồn tại bất kỳ CuonSach nào thuộc về nó hay không, bao gồm cả các bản ghi đã xóa mềm (DaAn = 1).
        /// Dùng để kiểm tra ràng buộc khi xóa vĩnh viễn Sach.
        /// </summary>
        /// <param name="id">ID của Sach (ấn bản).</param>
        /// <returns>Task chứa bool, true nếu có bất kỳ CuonSach nào thuộc về Sach này (kể cả đã xóa), false nếu ngược lại.</returns>
        /// <exception cref="Exception">Ném ra cho các lỗi hệ thống khi kiểm tra.</exception>
        // <<< SỬA TÊN PHƯƠC THỨC ĐỂ KHỚP INTERFACE >>>
        public async Task<bool> HasAnyCopiesAsync(int id) // Đã đổi tên từ HasAnyCuonSachBySachIdIncludingDeletedAsync
        {
            _logger.LogInformation("Checking for any CuonSach entries (including deleted) for Sach ID: {SachId} (Matching interface HasAnyCopiesAsync)", id);
            // Kiểm tra ID hợp lệ
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID provided for HasAnyCopiesAsync check: {SachId}", id);
                return false;
            }
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Sử dụng AnyAsync để kiểm tra sự tồn tại hiệu quả
                    bool hasAny = await _context.Cuonsach.AnyAsync(cs => cs.IdSach == id); // Chỉ lọc theo IdSach, không quan tâm DaAn
                    _logger.LogInformation("Any CuonSach check (including deleted) for Sach ID {SachId} result: {HasAny}", id, hasAny);
                    return hasAny;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking for any CuonSach entries for Sach ID: {SachId}", id);
                    // Wrap lỗi
                    throw new Exception($"Lỗi khi kiểm tra các bản sao của sách ID: {id}.", ex);
                }
            }
        }

        /// <summary>
        /// Phục hồi một bản ghi Sach đã bị xóa mềm (Set DaAn = 0).
        /// Sử dụng ExecuteUpdateAsync để hiệu quả.
        /// </summary>
        /// <param name="id">ID của sách cần phục hồi.</param>
        /// <returns>Task chứa bool, true nếu phục hồi thành công (có 1 dòng bị ảnh hưởng), false nếu không tìm thấy sách hoặc sách chưa bị xóa mềm.</returns>
        /// <exception cref="ArgumentException">Ném ra nếu ID không hợp lệ.</exception>
        /// <exception cref="Exception">Ném ra cho các lỗi hệ thống khác.</exception>
        public async Task<bool> RestoreAsync(int id)
        {
            _logger.LogInformation("Attempting to restore Sach with ID: {SachId}", id);
            // Kiểm tra ID hợp lệ
            if (id <= 0)
            {
                _logger.LogError("Invalid Sach ID provided for restore: {SachId}", id);
                throw new ArgumentException("ID sách không hợp lệ để phục hồi.", nameof(id));
            }
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Dùng ExecuteUpdateAsync để phục hồi hiệu quả
                    int affectedRows = await _context.Sach
                        .Where(s => s.Id == id && s.DaAn == 1) // Chỉ phục hồi sách đang bị xóa mềm
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(s => s.DaAn, 0) // Đặt DaAn = 0
                        );

                    if (affectedRows > 0)
                    {
                        _logger.LogInformation("Successfully restored Sach with ID: {SachId}", id);
                    }
                    else
                    {
                        _logger.LogInformation("Restore failed: Sach with ID {SachId} not found or not soft-deleted.", id);
                    }
                    return affectedRows > 0; // True nếu có đúng 1 dòng bị ảnh hưởng
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error restoring Sach (ID: {SachId}). Details: {ErrorMessage}", id, ex.InnerException?.Message ?? ex.Message);
                    // Wrap lỗi
                    throw new Exception($"Lỗi cơ sở dữ liệu khi phục hồi sách ID: {id}.", ex);
                }
            }
        }

        /// <summary>
        /// Xóa vĩnh viễn một bản ghi Sach khỏi cơ sở dữ liệu.
        /// Sử dụng ExecuteDeleteAsync để hiệu quả.
        /// **CẢNH BÁO:** Thao tác này không thể hoàn tác. Việc kiểm tra ràng buộc nghiệp vụ (ví dụ: còn cuốn sách vật lý nào không) NÊN được thực hiện ở tầng BUS TRƯỚC KHI gọi hàm này.
        /// </summary>
        /// <param name="id">ID của sách cần xóa vĩnh viễn.</param>
        /// <returns>Task chứa bool, true nếu xóa thành công (có 1 dòng bị ảnh hưởng), false nếu không tìm thấy sách.</returns>
        /// <exception cref="ArgumentException">Ném ra nếu ID không hợp lệ.</exception>
        /// <exception cref="DbUpdateException">Ném ra nếu có lỗi DB (ví dụ: ràng buộc FK cứng) khi xóa.</exception>
        /// <exception cref="Exception">Ném ra cho các lỗi không xác định khác.</exception>
        public async Task<bool> HardDeleteAsync(int id)
        {
            _logger.LogWarning("Attempting to HARD DELETE Sach with ID: {SachId}. This is irreversible!", id);
            // Kiểm tra ID hợp lệ
            if (id <= 0)
            {
                _logger.LogError("Invalid Sach ID provided for hard delete: {SachId}", id);
                throw new ArgumentException("ID sách không hợp lệ để xóa vĩnh viễn.", nameof(id));
            }

            // *** LƯU Ý QUAN TRỌNG: ***
            // Việc kiểm tra ràng buộc (ví dụ: sách có đang được mượn không, còn CuonSach nào không)
            // NÊN được thực hiện ở tầng BUS TRƯỚC KHI gọi hàm này.
            // Hàm này ở tầng DAL chỉ tập trung vào việc thực thi thao tác DB.
            // Tuy nhiên, vẫn cần bắt lỗi DbUpdateException nếu có Foreign Key constraint cứng ở CSDL.

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QLTVContext>();
                try
                {
                    // Dùng ExecuteDeleteAsync để xóa hiệu quả trên DB
                    int affectedRows = await _context.Sach
                        .Where(s => s.Id == id) // Tìm sách theo ID (có thể xóa cả sách đã soft-deleted nếu không có FK)
                        .ExecuteDeleteAsync(); // Thực thi lệnh DELETE trực tiếp trên DB

                    if (affectedRows > 0)
                    {
                        _logger.LogWarning("Successfully HARD DELETED Sach with ID: {SachId}", id);
                    }
                    else
                    {
                        _logger.LogInformation("Hard delete failed: Sach with ID {SachId} not found.", id);
                    }
                    return affectedRows > 0; // True nếu có đúng 1 dòng bị ảnh hưởng
                }
                catch (DbUpdateException ex) // Bắt lỗi liên quan đến DB, thường là FK constraints
                {
                    _logger.LogError(ex, "DbUpdateException during hard delete of Sach (ID: {SachId}). Might be due to FK constraints. Details: {ErrorMessage}", id, ex.InnerException?.Message ?? ex.Message);
                    // Wrap lỗi DB thành InvalidOperationException để tầng BUS/GUI biết đây là lỗi do ràng buộc dữ liệu
                    throw new InvalidOperationException($"Không thể xóa vĩnh viễn sách ID: {id} do còn dữ liệu liên quan (ví dụ: cuốn sách, phiếu nhập). Vui lòng xóa dữ liệu liên quan trước.", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error hard deleting Sach (ID: {SachId}). Details: {ErrorMessage}", id, ex.Message);
                    // Wrap lỗi không xác định
                    throw new Exception($"Lỗi không xác định khi xóa vĩnh viễn sách ID: {id}.", ex);
                }
            }
        }

        // *** PHƯƠNG THỨC MỚI ĐỂ TRIỂN KHAI IDALSach.GetSoLuongCuonSachDangMuonBySachIdAsync ***
        /// <summary>
        /// Lấy tổng số lượng CuonSach thuộc một Sach (ấn bản) đang ở trạng thái 'Đang mượn'.
        /// Phương thức này được thêm vào để triển khai phương thức tương ứng trong IDALSach.
        /// </summary>
        /// <param name="sachId">ID của ấn bản sách (Sach.Id).</param>
        /// <returns>Task chứa số lượng cuốn sách đang mượn thuộc ấn bản đó.</returns>
        public async Task<int> GetSoLuongCuonSachDangMuonBySachIdAsync(int sachId)
        {
            _logger.LogInformation("Attempting to get count of borrowed CuonSach for Sach ID: {SachId} (Implementing IDALSach method)", sachId);

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
                    // Lọc theo trạng thái 'Đang mượn'. Giả định trạng thái 1 là "Đang cho mượn"
                    // Đồng thời kiểm tra CuonSach chưa bị xóa mềm (nếu có cột DaAn trong CuonSach)
                    int count = await _context.Cuonsach
                        .CountAsync(cs => cs.IdSach == sachId
                                      && cs.TinhTrang == 1 // <-- GIẢ ĐỊNH: 1 là trạng thái "Đã cho mượn"
                                                           // && cs.DaAn == 0  // <-- Thêm điều kiện này nếu CuonSach có cờ DaAn
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
        // *** KẾT THÚC PHƯƠNG THỨC MỚI ***


        // TODO: Implement SearchAsync, GetBy... methods if needed based on IDALSach interface
    } // Kết thúc class DALSach
} // Kết thúc namespace DAL