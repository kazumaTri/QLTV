// --- USING ---
using DAL;
using DAL.Models;
using DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging; // Thêm using này nếu chưa có
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BUS
{
    public class BUSTuaSach : IBUSTuaSach
    {
        private readonly IDALTuaSach _dalTuaSach;
        // Inject thêm các BUS khác nếu cần validation chéo (ví dụ: IBUSTheLoai, IBUSTacGia)
        private readonly IBUSTheLoai _busTheLoai;
        private readonly IBUSTacGia _busTacGia;
        private readonly ILogger<BUSTuaSach> _logger; // Thêm ILogger

        // Inject ILogger vào constructor
        public BUSTuaSach(IDALTuaSach dalTuaSach, IBUSTheLoai busTheLoai, IBUSTacGia busTacGia, ILogger<BUSTuaSach> logger)
        {
            _dalTuaSach = dalTuaSach ?? throw new ArgumentNullException(nameof(dalTuaSach));
            _busTheLoai = busTheLoai ?? throw new ArgumentNullException(nameof(busTheLoai));
            _busTacGia = busTacGia ?? throw new ArgumentNullException(nameof(busTacGia));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Gán logger
            _logger.LogInformation("BUSTuaSach initialized.");
        }

        // Hàm mapping từ Entity sang DTO (giữ nguyên logic)
        private TuaSachDTO MapToTuaSachDTO(Tuasach entity)
        {
            if (entity == null)
            {
                _logger.LogWarning("MapToTuaSachDTO received a null entity.");
                return null!; // Trả về null để có thể lọc ra sau này
            }
            try
            {
                var dto = new TuaSachDTO
                {
                    Id = entity.Id,
                    MaTuaSach = entity.MaTuaSach,
                    TenTuaSach = entity.TenTuaSach,
                    IdTheLoai = entity.IdTheLoai,
                    TenTheLoai = entity.IdTheLoaiNavigation?.TenTheLoai, // Null-conditional access
                    DaAn = entity.DaAn,
                    // Tính toán số lượng dựa trên collection Sach (nếu đã được Include từ DAL)
                    SoLuongSach = entity.Sach?.Count(s => s.DaAn == 0) ?? 0, // Chỉ đếm sách chưa xóa mềm
                    SoLuongSachConLai = entity.Sach?
                                        .Where(s => s.DaAn == 0) // Chỉ xét các Sach chưa xóa mềm
                                        .Sum(s => s.SoLuongConLai) // Bỏ '?? 0' vì SoLuongConLai là int không thể null
                                        ?? 0, // Giữ lại '?? 0' ở ngoài vì kết quả của Sum có thể là null nếu collection Sach là null
                    // Map danh sách Tác giả (nếu đã được Include từ DAL)
                    TacGias = entity.IdTacGia? // Navigation property cho Tacgia (M-N)
                       .Select(tg => new TacGiaDTO { Id = tg.Id, Matacgia = tg.Matacgia, TenTacGia = tg.TenTacGia })
                       .ToList() ?? new List<TacGiaDTO>() // Trả về list rỗng nếu null
                };
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping Tuasach Entity (ID: {EntityId}) to DTO.", entity.Id);
                return null!; // Trả về null nếu có lỗi mapping
            }
        }

        // Hàm mapping từ DTO sang Entity (giữ nguyên logic)
        private Tuasach MapToTuaSachEntity(TuaSachDTO dto) // Chỉ map scalar properties
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            return new Tuasach
            {
                Id = dto.Id,
                MaTuaSach = dto.MaTuaSach?.Trim(), // Cho phép Mã là null hoặc trim
                TenTuaSach = dto.TenTuaSach.Trim(), // Tên không nên null, trim để sạch
                IdTheLoai = dto.IdTheLoai,
                DaAn = dto.DaAn ?? 0 // Mặc định là 0 nếu DTO không có
                // KHÔNG map collections ở đây (IdTacGia, Sach)
            };
        }

        /// <summary>
        /// Lấy danh sách tất cả Tựa sách DTO chưa bị xóa mềm.
        /// Đã thêm logging chi tiết hơn.
        /// </summary>
        /// <returns>Danh sách TuaSachDTO.</returns>
        public async Task<List<TuaSachDTO>> GetAllTuaSachAsync()
        {
            _logger.LogInformation("BUSTuaSach: Attempting to get all TuaSach entities from DAL.");
            List<Tuasach> entities = new List<Tuasach>(); // Khởi tạo list rỗng
            try
            {
                // Gọi DAL để lấy danh sách Entities
                entities = await _dalTuaSach.GetAllAsync(); // Giả định DAL đã Include đủ các bảng liên quan
                _logger.LogInformation("BUSTuaSach: Received {Count} entities from DAL.", entities.Count);

                if (entities == null)
                {
                    _logger.LogWarning("BUSTuaSach: DAL returned a null list for GetAllAsync. Returning empty list.");
                    return new List<TuaSachDTO>(); // Trả về list rỗng nếu DAL trả về null
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BUSTuaSach: Error occurred while calling DAL.GetAllAsync.");
                // Ném lại lỗi hoặc trả về list rỗng tùy theo yêu cầu xử lý lỗi của ứng dụng
                throw new Exception("Lỗi khi lấy dữ liệu Tựa sách từ tầng DAL.", ex);
                // Hoặc: return new List<TuaSachDTO>();
            }

            try
            {
                _logger.LogInformation("BUSTuaSach: Mapping {Count} entities to DTOs.", entities.Count);
                // Map danh sách Entities sang danh sách DTOs
                var dtos = entities
                    .Select(e => MapToTuaSachDTO(e)) // Sử dụng hàm map đã định nghĩa
                    .Where(dto => dto != null) // Lọc bỏ các kết quả null từ hàm map (nếu có lỗi hoặc entity gốc là null)
                    .ToList();

                _logger.LogInformation("BUSTuaSach: Successfully mapped {DtoCount} entities to non-null DTOs. Returning the list.", dtos.Count);
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BUSTuaSach: Error occurred during mapping Entities to DTOs.");
                // Xử lý lỗi mapping nếu cần, ví dụ trả về list rỗng
                // Debug.WriteLine($"[BUSTuaSach] Error mapping entities to DTOs: {ex.ToString()}");
                return new List<TuaSachDTO>(); // Trả về list rỗng nếu có lỗi mapping
            }
        }

        // --- Các phương thức khác giữ nguyên logic nhưng có thể thêm logging tương tự nếu cần ---

        public async Task<TuaSachDTO?> GetTuaSachByIdAsync(int id)
        {
            _logger.LogInformation("BUSTuaSach: Getting TuaSach by ID {Id}", id);
            var entity = await _dalTuaSach.GetByIdAsync(id); // DAL đã Include đủ
            if (entity == null)
            {
                _logger.LogInformation("BUSTuaSach: TuaSach with ID {Id} not found in DAL.", id);
                return null;
            }
            _logger.LogInformation("BUSTuaSach: Mapping found entity (ID: {Id}) to DTO.", id);
            return MapToTuaSachDTO(entity);
        }

        public async Task<TuaSachDTO?> GetTuaSachByMaAsync(string maTuaSach)
        {
            _logger.LogInformation("BUSTuaSach: Getting TuaSach by MaTuaSach {MaTuaSach}", maTuaSach);
            var entity = await _dalTuaSach.GetByMaAsync(maTuaSach);
            if (entity == null)
            {
                _logger.LogInformation("BUSTuaSach: TuaSach with MaTuaSach {MaTuaSach} not found in DAL.", maTuaSach);
                return null;
            }
            _logger.LogInformation("BUSTuaSach: Mapping found entity (MaTuaSach: {MaTuaSach}) to DTO.", maTuaSach);
            return MapToTuaSachDTO(entity);
        }
        public async Task<TuaSachDTO?> GetTuaSachByTenAsync(string tenTuaSach)
        {
            _logger.LogInformation("BUSTuaSach: Getting TuaSach by TenTuaSach {TenTuaSach}", tenTuaSach);
            var entity = await _dalTuaSach.GetByTenAsync(tenTuaSach);
            if (entity == null)
            {
                _logger.LogInformation("BUSTuaSach: TuaSach with TenTuaSach {TenTuaSach} not found in DAL.", tenTuaSach);
                return null;
            }
            _logger.LogInformation("BUSTuaSach: Mapping found entity (TenTuaSach: {TenTuaSach}) to DTO.", tenTuaSach);
            return MapToTuaSachDTO(entity);
        }


        public async Task<TuaSachDTO?> AddTuaSachAsync(TuaSachDTO tuaSachDto)
        {
            _logger.LogInformation("BUSTuaSach: Attempting to add TuaSach: {TenTuaSach}", tuaSachDto?.TenTuaSach);
            // --- Validation ---
            if (tuaSachDto == null)
            {
                _logger.LogError("BUSTuaSach: AddTuaSachAsync called with null DTO.");
                throw new ArgumentNullException(nameof(tuaSachDto));
            }
            // Trim và Validate Tên
            tuaSachDto.TenTuaSach = tuaSachDto.TenTuaSach?.Trim();
            if (string.IsNullOrWhiteSpace(tuaSachDto.TenTuaSach))
            {
                _logger.LogWarning("BUSTuaSach: Validation failed - TenTuaSach is empty.");
                throw new ArgumentException("Tên tựa sách không được để trống.", nameof(tuaSachDto.TenTuaSach));
            }
            // Trim Mã (nếu có)
            tuaSachDto.MaTuaSach = tuaSachDto.MaTuaSach?.Trim();
            // Validate Thể loại ID
            _logger.LogInformation("BUSTuaSach: Validating TheLoai ID: {IdTheLoai}", tuaSachDto.IdTheLoai);
            if (tuaSachDto.IdTheLoai <= 0 || await _busTheLoai.GetTheLoaiByIdAsync(tuaSachDto.IdTheLoai) == null) // Kiểm tra TheLoai có tồn tại không
            {
                _logger.LogWarning("BUSTuaSach: Validation failed - Invalid TheLoai ID: {IdTheLoai}", tuaSachDto.IdTheLoai);
                throw new ArgumentException("Thể loại không hợp lệ.", nameof(tuaSachDto.IdTheLoai));
            }
            // Validate Tác giả IDs (nếu có)
            var tacGiaIds = tuaSachDto.TacGias?.Select(tg => tg.Id).Distinct().ToList() ?? new List<int>();
            _logger.LogInformation("BUSTuaSach: Validating TacGia IDs: {TacGiaIdsCount} IDs provided.", tacGiaIds.Count);
            if (tacGiaIds.Any(id => id <= 0))
            {
                _logger.LogWarning("BUSTuaSach: Validation failed - Invalid TacGia ID found in list.");
                throw new ArgumentException("Danh sách tác giả chứa ID không hợp lệ.", nameof(tuaSachDto.TacGias));
            }
            // (Tùy chọn) Kiểm tra tất cả Tác giả ID có tồn tại trong DB không
            // foreach (int tgId in tacGiaIds) { if (await _busTacGia.GetTacGiaByIdAsync(tgId) == null) throw new ArgumentException($"Tác giả với ID {tgId} không tồn tại."); }


            // --- Check Duplicates ---
            _logger.LogInformation("BUSTuaSach: Checking for duplicate MaTuaSach '{MaTuaSach}' and TenTuaSach '{TenTuaSach}'", tuaSachDto.MaTuaSach, tuaSachDto.TenTuaSach);
            if (!string.IsNullOrWhiteSpace(tuaSachDto.MaTuaSach) && await _dalTuaSach.IsMaTuaSachExistsAsync(tuaSachDto.MaTuaSach))
            {
                _logger.LogWarning("BUSTuaSach: Add failed - Duplicate MaTuaSach '{MaTuaSach}' found.", tuaSachDto.MaTuaSach);
                throw new InvalidOperationException($"Mã tựa sách '{tuaSachDto.MaTuaSach}' đã tồn tại.");
            }
            if (await _dalTuaSach.IsTenTuaSachExistsAsync(tuaSachDto.TenTuaSach))
            {
                _logger.LogWarning("BUSTuaSach: Add failed - Duplicate TenTuaSach '{TenTuaSach}' found.", tuaSachDto.TenTuaSach);
                throw new InvalidOperationException($"Tên tựa sách '{tuaSachDto.TenTuaSach}' đã tồn tại.");
            }

            // --- Add Logic ---
            try
            {
                _logger.LogInformation("BUSTuaSach: Mapping DTO to entity for adding.");
                var entityToAdd = MapToTuaSachEntity(tuaSachDto);
                // Sinh mã nếu cần
                if (string.IsNullOrWhiteSpace(entityToAdd.MaTuaSach))
                {
                    entityToAdd.MaTuaSach = $"TS{DateTime.Now:yyyyMMddHHmmssfff}"; // Ví dụ sinh mã
                    _logger.LogInformation("BUSTuaSach: Generated MaTuaSach: {GeneratedMa}", entityToAdd.MaTuaSach);
                }
                entityToAdd.DaAn = 0; // Đảm bảo khi thêm thì chưa bị xóa

                // Bước 1: Thêm Tựa Sách (chỉ scalar properties và FK IdTheLoai)
                _logger.LogInformation("BUSTuaSach: Calling DAL.AddAsync...");
                var addedEntity = await _dalTuaSach.AddAsync(entityToAdd);
                if (addedEntity == null)
                {
                    _logger.LogError("BUSTuaSach: DAL.AddAsync returned null.");
                    throw new Exception("Thêm tựa sách vào cơ sở dữ liệu thất bại (DAL trả về null).");
                }
                _logger.LogInformation("BUSTuaSach: Added entity with ID: {AddedId}. Proceeding to link authors if any.", addedEntity.Id);

                // Bước 2: Liên kết Tác giả (nếu có)
                if (tacGiaIds.Any())
                {
                    _logger.LogInformation("BUSTuaSach: Linking {TacGiaCount} authors to TuaSach ID {AddedId}.", tacGiaIds.Count, addedEntity.Id);
                    bool linkSuccess = await _dalTuaSach.UpdateTuaSachTacGiasAsync(addedEntity.Id, tacGiaIds);
                    if (!linkSuccess)
                    {
                        // Log warning hoặc xử lý lỗi liên kết tác giả (hiếm khi xảy ra nếu AddAsync thành công)
                        _logger.LogWarning("BUSTuaSach: Failed to link authors for newly added TuaSach ID: {AddedId}. Proceeding anyway.", addedEntity.Id);
                        // Debug.WriteLine($"[BUSTuaSach] Warning: Failed to link authors for newly added TuaSach ID: {addedEntity.Id}");
                    }
                    else
                    {
                        _logger.LogInformation("BUSTuaSach: Successfully linked authors for TuaSach ID {AddedId}.", addedEntity.Id);
                    }
                }

                // Lấy lại entity đầy đủ (bao gồm tác giả đã liên kết) để trả về DTO hoàn chỉnh
                _logger.LogInformation("BUSTuaSach: Fetching full entity with ID {AddedId} after add/link.", addedEntity.Id);
                var fullAddedEntity = await _dalTuaSach.GetByIdAsync(addedEntity.Id); // Sử dụng GetByIdAsync của DAL, không phải của BUS này để tránh vòng lặp vô hạn nếu GetByIdAsync của BUS cũng gọi lại GetByIdAsync của DAL
                if (fullAddedEntity == null)
                {
                    _logger.LogWarning("BUSTuaSach: Could not fetch the fully added entity (ID: {AddedId}) after linking authors. Returning null.", addedEntity.Id);
                    return null;
                }
                _logger.LogInformation("BUSTuaSach: Mapping full entity to DTO and returning.");
                return MapToTuaSachDTO(fullAddedEntity); // Map entity đầy đủ
            }
            catch (ArgumentException ex) // Catch specific validation errors
            {
                _logger.LogError(ex, "BUSTuaSach: Validation error during AddTuaSachAsync.");
                throw; // Re-throw validation errors
            }
            catch (InvalidOperationException ex) // Catch specific business rule errors (duplicates)
            {
                _logger.LogError(ex, "BUSTuaSach: Business rule violation during AddTuaSachAsync.");
                throw; // Re-throw business rule errors
            }
            catch (Exception ex) // Catch general errors
            {
                _logger.LogError(ex, "BUSTuaSach: Unexpected error during AddTuaSachAsync.");
                // Debug.WriteLine($"[BUSTuaSach] Error during AddTuaSachAsync: {ex.ToString()}");
                // Ném lại Exception hoặc một loại Exception tùy chỉnh của BUS
                throw new Exception($"Lỗi khi thêm tựa sách: {ex.Message}", ex);
            }
        }


        public async Task<bool> UpdateTuaSachAsync(TuaSachDTO tuaSachDto)
        {
            _logger.LogInformation("BUSTuaSach: Attempting to update TuaSach ID: {Id}", tuaSachDto?.Id);
            // --- Validation ---
            if (tuaSachDto == null)
            {
                _logger.LogError("BUSTuaSach: UpdateTuaSachAsync called with null DTO.");
                throw new ArgumentNullException(nameof(tuaSachDto));
            }
            if (tuaSachDto.Id <= 0)
            {
                _logger.LogWarning("BUSTuaSach: Validation failed - Invalid ID for update: {Id}", tuaSachDto.Id);
                throw new ArgumentException("ID tựa sách không hợp lệ để cập nhật.", nameof(tuaSachDto.Id));
            }

            // Trim và Validate Tên
            tuaSachDto.TenTuaSach = tuaSachDto.TenTuaSach?.Trim();
            if (string.IsNullOrWhiteSpace(tuaSachDto.TenTuaSach))
            {
                _logger.LogWarning("BUSTuaSach: Validation failed - TenTuaSach is empty for update ID: {Id}", tuaSachDto.Id);
                throw new ArgumentException("Tên tựa sách không được để trống.", nameof(tuaSachDto.TenTuaSach));
            }
            // Trim Mã (nếu có) - Lưu ý: Thường không cho sửa Mã sau khi tạo
            tuaSachDto.MaTuaSach = tuaSachDto.MaTuaSach?.Trim();

            // Validate Thể loại ID
            _logger.LogInformation("BUSTuaSach: Validating TheLoai ID: {IdTheLoai} for update ID: {Id}", tuaSachDto.IdTheLoai, tuaSachDto.Id);
            if (tuaSachDto.IdTheLoai <= 0 || await _busTheLoai.GetTheLoaiByIdAsync(tuaSachDto.IdTheLoai) == null)
            {
                _logger.LogWarning("BUSTuaSach: Validation failed - Invalid TheLoai ID: {IdTheLoai} for update ID: {Id}", tuaSachDto.IdTheLoai, tuaSachDto.Id);
                throw new ArgumentException("Thể loại không hợp lệ.", nameof(tuaSachDto.IdTheLoai));
            }

            // Validate Tác giả IDs
            var tacGiaIds = tuaSachDto.TacGias?.Select(tg => tg.Id).Distinct().ToList() ?? new List<int>();
            _logger.LogInformation("BUSTuaSach: Validating {TacGiaCount} TacGia IDs for update ID: {Id}.", tacGiaIds.Count, tuaSachDto.Id);
            if (tacGiaIds.Any(id => id <= 0))
            {
                _logger.LogWarning("BUSTuaSach: Validation failed - Invalid TacGia ID found in list for update ID: {Id}.", tuaSachDto.Id);
                throw new ArgumentException("Danh sách tác giả chứa ID không hợp lệ.", nameof(tuaSachDto.TacGias));
            }
            // (Tùy chọn) Kiểm tra Tác giả ID tồn tại


            // --- Check Duplicates (excluding self) ---
            _logger.LogInformation("BUSTuaSach: Checking for duplicate MaTuaSach '{MaTuaSach}' and TenTuaSach '{TenTuaSach}' excluding self (ID: {Id})", tuaSachDto.MaTuaSach, tuaSachDto.TenTuaSach, tuaSachDto.Id);
            if (!string.IsNullOrWhiteSpace(tuaSachDto.MaTuaSach) && await _dalTuaSach.IsMaTuaSachExistsExcludingIdAsync(tuaSachDto.MaTuaSach, tuaSachDto.Id))
            {
                _logger.LogWarning("BUSTuaSach: Update failed - Duplicate MaTuaSach '{MaTuaSach}' found for another TuaSach.", tuaSachDto.MaTuaSach);
                throw new InvalidOperationException($"Mã tựa sách '{tuaSachDto.MaTuaSach}' đã tồn tại cho tựa sách khác.");
            }
            if (await _dalTuaSach.IsTenTuaSachExistsExcludingIdAsync(tuaSachDto.TenTuaSach, tuaSachDto.Id))
            {
                _logger.LogWarning("BUSTuaSach: Update failed - Duplicate TenTuaSach '{TenTuaSach}' found for another TuaSach.", tuaSachDto.TenTuaSach);
                throw new InvalidOperationException($"Tên tựa sách '{tuaSachDto.TenTuaSach}' đã tồn tại cho tựa sách khác.");
            }

            // --- Update Logic ---
            try
            {
                // Lấy trạng thái DaAn hiện tại trước khi map (để không bị ghi đè bởi DTO)
                _logger.LogInformation("BUSTuaSach: Fetching current entity (ID: {Id}) before update.", tuaSachDto.Id);
                var currentEntity = await _dalTuaSach.GetByIdAsync(tuaSachDto.Id); // Lại dùng DAL trực tiếp
                if (currentEntity == null)
                {
                    _logger.LogError("BUSTuaSach: Update failed - TuaSach with ID {Id} not found.", tuaSachDto.Id);
                    // Không nên throw lỗi ở đây, trả về false để báo update không thành công do không tìm thấy
                    // throw new InvalidOperationException($"Không tìm thấy tựa sách với ID {tuaSachDto.Id} để cập nhật.");
                    return false; // Trả về false nếu không tìm thấy entity để cập nhật
                }
                _logger.LogInformation("BUSTuaSach: Mapping DTO to entity for update ID: {Id}.", tuaSachDto.Id);

                // Bước 1: Cập nhật thông tin cơ bản của Tựa Sách
                var entityToUpdate = MapToTuaSachEntity(tuaSachDto); // Map scalar properties từ DTO
                entityToUpdate.DaAn = currentEntity.DaAn; // Giữ nguyên trạng thái DaAn hiện có

                _logger.LogInformation("BUSTuaSach: Calling DAL.UpdateAsync for scalar properties of ID: {Id}.", tuaSachDto.Id);
                bool scalarUpdateSuccess = await _dalTuaSach.UpdateAsync(entityToUpdate);
                if (!scalarUpdateSuccess)
                {
                    // Có thể do lỗi tương tranh hoặc ID không đúng, mặc dù đã kiểm tra GetByIdAsync
                    _logger.LogWarning("BUSTuaSach: Scalar update for TuaSach ID {Id} returned false from DAL. Update might have failed or no changes detected.", tuaSachDto.Id);
                    // Không nên return false ngay, vì có thể việc update tác giả vẫn cần thực hiện
                    // return false;
                }
                else
                {
                    _logger.LogInformation("BUSTuaSach: Scalar update for TuaSach ID {Id} reported success from DAL.", tuaSachDto.Id);
                }


                // Bước 2: Cập nhật danh sách Tác giả liên kết
                _logger.LogInformation("BUSTuaSach: Calling DAL.UpdateTuaSachTacGiasAsync for ID: {Id} with {TacGiaCount} authors.", tuaSachDto.Id, tacGiaIds.Count);
                bool authorUpdateSuccess = await _dalTuaSach.UpdateTuaSachTacGiasAsync(tuaSachDto.Id, tacGiaIds);
                if (!authorUpdateSuccess)
                {
                    // Log warning, transaction sẽ rollback nếu DAL dùng transaction
                    _logger.LogWarning("BUSTuaSach: Author update for TuaSach ID {Id} returned false from DAL.", tuaSachDto.Id);
                    // Có thể cần throw lỗi ở đây nếu việc cập nhật tác giả là bắt buộc
                    // Hoặc xem xét logic trả về: chỉ cần scalar thành công hay cả hai phải thành công?
                }
                else
                {
                    _logger.LogInformation("BUSTuaSach: Author update for TuaSach ID {Id} reported success from DAL.", tuaSachDto.Id);
                }

                // Trả về true nếu ít nhất một trong hai thao tác (scalar hoặc author update) thành công
                // Hoặc thay đổi logic nếu cả hai đều phải thành công
                _logger.LogInformation("BUSTuaSach: Update process finished for ID: {Id}. Scalar success: {ScalarSuccess}, Author success: {AuthorSuccess}", tuaSachDto.Id, scalarUpdateSuccess, authorUpdateSuccess);
                return scalarUpdateSuccess || authorUpdateSuccess; // Hoặc scalarUpdateSuccess && authorUpdateSuccess tùy yêu cầu
            }
            catch (ArgumentException ex) // Catch specific validation errors
            {
                _logger.LogError(ex, "BUSTuaSach: Validation error during UpdateTuaSachAsync for ID: {Id}.", tuaSachDto.Id);
                throw; // Re-throw validation errors
            }
            catch (InvalidOperationException ex) // Catch specific business rule errors (duplicates)
            {
                _logger.LogError(ex, "BUSTuaSach: Business rule violation during UpdateTuaSachAsync for ID: {Id}.", tuaSachDto.Id);
                throw; // Re-throw business rule errors
            }
            catch (Exception ex) // Catch general errors
            {
                _logger.LogError(ex, "BUSTuaSach: Unexpected error during UpdateTuaSachAsync for ID: {Id}.", tuaSachDto.Id);
                // Debug.WriteLine($"[BUSTuaSach] Error during UpdateTuaSachAsync (ID: {tuaSachDto.Id}): {ex.ToString()}");
                throw new Exception($"Lỗi khi cập nhật tựa sách ID {tuaSachDto.Id}: {ex.Message}", ex);
            }

        }

        public async Task<bool> DeleteTuaSachAsync(int id) // Soft Delete
        {
            _logger.LogInformation("BUSTuaSach: Attempting to soft delete TuaSach ID: {Id}", id);
            if (id <= 0)
            {
                _logger.LogWarning("BUSTuaSach: Invalid ID for soft delete: {Id}", id);
                throw new ArgumentException("ID tựa sách không hợp lệ.", nameof(id));
            }
            try
            {
                // Có thể thêm kiểm tra nghiệp vụ phức tạp hơn ở đây nếu cần (ví dụ: kiểm tra sách đang được mượn)
                _logger.LogInformation("BUSTuaSach: Calling DAL.SoftDeleteAsync for ID: {Id}", id);
                bool success = await _dalTuaSach.SoftDeleteAsync(id);
                _logger.LogInformation("BUSTuaSach: Soft delete result for ID {Id}: {Success}", id, success);
                return success;
            }
            catch (InvalidOperationException ex) // Bắt lỗi nghiệp vụ từ DAL (ví dụ: còn sách)
            {
                _logger.LogWarning(ex, "BUSTuaSach: Business rule violation during soft delete for ID: {Id}", id);
                // Debug.WriteLine($"Business Rule Violation during Soft Delete (ID: {id}): {ex.Message}");
                throw; // Ném lại để GUI hiển thị
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BUSTuaSach: Unexpected error during soft delete for ID: {Id}", id);
                // Debug.WriteLine($"Error during Soft Delete (ID: {id}): {ex.ToString()}");
                throw new Exception($"Lỗi khi xóa mềm tựa sách ID {id}: {ex.Message}", ex);
            }
        }


        public async Task<bool> RestoreTuaSachAsync(int id)
        {
            _logger.LogInformation("BUSTuaSach: Attempting to restore TuaSach ID: {Id}", id);
            if (id <= 0)
            {
                _logger.LogWarning("BUSTuaSach: Invalid ID for restore: {Id}", id);
                throw new ArgumentException("ID tựa sách không hợp lệ.", nameof(id));
            }
            try
            {
                _logger.LogInformation("BUSTuaSach: Calling DAL.RestoreAsync for ID: {Id}", id);
                bool success = await _dalTuaSach.RestoreAsync(id);
                _logger.LogInformation("BUSTuaSach: Restore result for ID {Id}: {Success}", id, success);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BUSTuaSach: Unexpected error during restore for ID: {Id}", id);
                // Debug.WriteLine($"Error during Restore (ID: {id}): {ex.ToString()}");
                throw new Exception($"Lỗi khi phục hồi tựa sách ID {id}: {ex.Message}", ex);
            }
        }


        public async Task<bool> HardDeleteTuaSachAsync(int id)
        {
            _logger.LogWarning("BUSTuaSach: Attempting HARD DELETE for TuaSach ID: {Id}", id);
            if (id <= 0)
            {
                _logger.LogError("BUSTuaSach: Invalid ID for hard delete: {Id}", id);
                throw new ArgumentException("ID tựa sách không hợp lệ.", nameof(id));
            }
            // CẢNH BÁO: Rất nguy hiểm, chỉ dùng khi thực sự cần và hiểu rõ hậu quả.
            try
            {
                // Có thể thêm kiểm tra nghiệp vụ nghiêm ngặt trước khi Hard Delete ở đây
                // Ví dụ: Kiểm tra xem còn sách nào không? (Mặc dù DAL cũng kiểm tra)
                _logger.LogInformation("BUSTuaSach: Calling DAL.HardDeleteAsync for ID: {Id}", id);
                bool success = await _dalTuaSach.HardDeleteAsync(id);
                _logger.LogWarning("BUSTuaSach: Hard delete result for ID {Id}: {Success}", id, success); // Log warning vì đây là thao tác nguy hiểm
                return success;
            }
            catch (InvalidOperationException ex) // Lỗi ràng buộc FK hoặc nghiệp vụ từ DAL
            {
                _logger.LogWarning(ex, "BUSTuaSach: Business rule or FK violation during hard delete for ID: {Id}", id);
                // Debug.WriteLine($"FK Violation during Hard Delete (ID: {id}): {ex.Message}");
                throw; // Ném lại lỗi tường minh cho GUI
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BUSTuaSach: Unexpected error during hard delete for ID: {Id}", id);
                // Debug.WriteLine($"Error during Hard Delete (ID: {id}): {ex.ToString()}");
                throw new Exception($"Lỗi khi xóa vĩnh viễn tựa sách ID {id}: {ex.Message}", ex);
            }
        }

        // Hàm này không hiệu quả, nên bỏ hoặc tính toán ở DAL nếu thực sự cần tổng tất cả sách
        // Đánh dấu là không dùng hoặc xóa bỏ nếu không cần thiết
        [Obsolete("This method is inefficient. Calculate the sum in DAL if needed.")]
        public async Task<int> GetTotalSoLuongSachAsync()
        {
            _logger.LogWarning("BUSTuaSach: GetTotalSoLuongSachAsync called - this method is inefficient.");
            // Nên có hàm trong DALSach hoặc DAL CuonSach để tính tổng hiệu quả hơn
            var allTuaSach = await GetAllTuaSachAsync();
            return allTuaSach.Sum(ts => ts.SoLuongSach); // Tính tổng từ DTOs (không hiệu quả)
        }

        // --- PHƯƠNG THỨC TÌM KIẾM VÀ LỌC ---
        public async Task<List<TuaSachDTO>> SearchAndFilterTuaSachAsync(string? searchText, int theLoaiId, int tacGiaId)
        {
            _logger.LogInformation("BUSTuaSach: Searching/Filtering TuaSach. Search='{SearchText}', TheLoaiId={TheLoaiId}, TacGiaId={TacGiaId}", searchText, theLoaiId, tacGiaId);
            try
            {
                // Gọi phương thức tương ứng ở DAL
                var entities = await _dalTuaSach.SearchAndFilterAsync(searchText, theLoaiId, tacGiaId);

                if (entities == null)
                {
                    _logger.LogWarning("BUSTuaSach: DAL returned null list for SearchAndFilterAsync.");
                    return new List<TuaSachDTO>();
                }

                _logger.LogInformation("BUSTuaSach: Mapping {Count} filtered entities to DTOs.", entities.Count);
                // Map kết quả sang DTO
                var dtos = entities
                    .Select(e => MapToTuaSachDTO(e))
                    .Where(dto => dto != null) // Lọc bỏ DTO null nếu có lỗi mapping
                    .ToList();
                _logger.LogInformation("BUSTuaSach: Successfully mapped {DtoCount} filtered DTOs.", dtos.Count);
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BUSTuaSach: Error occurred during SearchAndFilterTuaSachAsync.");
                throw new Exception("Lỗi khi tìm kiếm và lọc tựa sách từ tầng BUS.", ex);
            }
        }
        // ---------------------------------

    } // End class BUSTuaSach
} // End namespace BUS
