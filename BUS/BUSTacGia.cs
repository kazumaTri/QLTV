// Project/Namespace: BUS
using DAL; // Cần cho IDALTacGia
using DAL.Models; // Cần để làm việc với Entity Tacgia (cho mapping)
using DTO; // Cần cho TacGiaDTO
using System; // Cần cho Exception, ArgumentNullException, InvalidOperationException
using System.Collections.Generic; // Cần cho List
using System.Linq; // Cần cho LINQ (Select)
using System.Threading.Tasks; // Cần cho async/await Task

namespace BUS
{
    /// <summary>
    /// Business Logic Layer triển khai IBUSTacGia, xử lý nghiệp vụ cho Tác giả.
    /// Phụ thuộc vào IDALTacGia. Thực hiện mapping DTO/Entity.
    /// Đã cập nhật để hỗ trợ Soft Delete, Restore và lấy dữ liệu bao gồm cả đã xóa.
    /// </summary>
    public class BUSTacGia : IBUSTacGia // <<< Implement interface
    {
        // --- DEPENDENCIES ---
        private readonly IDALTacGia _dalTacGia;

        // --- CONSTRUCTOR (Sử dụng Dependency Injection) ---
        public BUSTacGia(IDALTacGia dalTacGia)
        {
            _dalTacGia = dalTacGia ?? throw new ArgumentNullException(nameof(dalTacGia));
        }

        // --- PRIVATE HELPER METHOD: MAPPING ENTITY TO DTO ---
        private TacGiaDTO MapToTacGiaDTO(Tacgia entity)
        {
            if (entity == null) return null!;

            var dto = new TacGiaDTO
            {
                Id = entity.Id,
                Matacgia = entity.Matacgia,
                TenTacGia = entity.TenTacGia,
                // *** CẬP NHẬT MAPPING: Thêm DaAn ***
                DaAn = entity.DaAn
            };
            return dto;
        }

        // --- PRIVATE HELPER METHOD: MAPPING DTO TO ENTITY (For Add/Update) ---
        private Tacgia MapToTacGiaEntity(TacGiaDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var entity = new Tacgia
            {
                Id = dto.Id,
                Matacgia = dto.Matacgia?.Trim(),
                TenTacGia = dto.TenTacGia.Trim(),
                // *** CẬP NHẬT MAPPING: Thêm DaAn (DAL sẽ xử lý giá trị mặc định khi Add) ***
                DaAn = dto.DaAn // Cần thiết khi Update, Restore
            };
            return entity;
        }


        // --- METHOD IMPLEMENTATIONS (Từ IBUSTacGia) ---

        // Lấy tác giả dưới dạng DTO (có tùy chọn bao gồm đã xóa)
        public async Task<List<TacGiaDTO>> GetAllTacGiaAsync(bool includeDeleted = false) // <<< THÊM THAM SỐ
        {
            try
            {
                List<Tacgia> entities;
                // *** GỌI DAL PHÙ HỢP ***
                if (includeDeleted)
                {
                    entities = await _dalTacGia.GetAllIncludingDeletedAsync();
                }
                else
                {
                    entities = await _dalTacGia.GetAllAsync(); // DAL này đã lọc DaAn = false
                }
                return entities.Select(e => MapToTacGiaDTO(e)).Where(dto => dto != null).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSTacGia.GetAllTacGiaAsync (includeDeleted={includeDeleted}): {ex.Message}");
                throw;
            }
        }

        // Tìm kiếm tác giả dưới dạng DTO (có tùy chọn bao gồm đã xóa)
        public async Task<List<TacGiaDTO>> SearchTacGiaAsync(string searchTerm, bool includeDeleted = false) // <<< THÊM THAM SỐ
        {
            try
            {
                List<Tacgia> entities;
                // *** GỌI DAL PHÙ HỢP ***
                if (includeDeleted)
                {
                    entities = await _dalTacGia.SearchIncludingDeletedAsync(searchTerm);
                }
                else
                {
                    entities = await _dalTacGia.SearchAsync(searchTerm); // DAL này đã lọc DaAn = false
                }
                return entities.Select(e => MapToTacGiaDTO(e)).Where(dto => dto != null).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSTacGia.SearchTacGiaAsync (SearchTerm: '{searchTerm}', includeDeleted={includeDeleted}): {ex.Message}");
                throw;
            }
        }

        // Lấy tác giả theo ID dưới dạng DTO (chỉ trả về nếu chưa bị xóa mềm)
        public async Task<TacGiaDTO?> GetTacGiaByIdAsync(int id)
        {
            if (id <= 0) return null;
            try
            {
                // Gọi DAL để lấy Entity theo ID (DAL đã lọc DaAn = false)
                var entity = await _dalTacGia.GetByIdAsync(id);
                return entity != null ? MapToTacGiaDTO(entity) : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSTacGia.GetTacGiaByIdAsync (ID: {id}): {ex.Message}");
                throw;
            }
        }

        // Lấy tác giả theo Mã dưới dạng DTO (chỉ trả về nếu chưa bị xóa mềm)
        public async Task<TacGiaDTO?> GetTacGiaByMaAsync(string maTacGia)
        {
            if (string.IsNullOrWhiteSpace(maTacGia)) return null;
            try
            {
                // Gọi DAL để lấy Entity theo Mã (DAL đã lọc DaAn = false)
                var entity = await _dalTacGia.GetByMaAsync(maTacGia);
                return entity != null ? MapToTacGiaDTO(entity) : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSTacGia.GetTacGiaByMaAsync (Ma: {maTacGia}): {ex.Message}");
                throw;
            }
        }

        // Lấy tác giả theo Tên dưới dạng DTO (chỉ trả về nếu chưa bị xóa mềm)
        public async Task<TacGiaDTO?> GetTacGiaByTenAsync(string tenTacGia)
        {
            if (string.IsNullOrWhiteSpace(tenTacGia)) return null;
            try
            {
                // Gọi DAL để lấy Entity theo Tên (DAL đã lọc DaAn = false)
                var entity = await _dalTacGia.GetByTenAsync(tenTacGia);
                return entity != null ? MapToTacGiaDTO(entity) : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSTacGia.GetTacGiaByTenAsync (Ten: {tenTacGia}): {ex.Message}");
                throw;
            }
        }

        // Thêm mới tác giả từ DTO
        public async Task<TacGiaDTO?> AddTacGiaAsync(TacGiaDTO tacGiaDto)
        {
            if (tacGiaDto == null) throw new ArgumentNullException(nameof(tacGiaDto));

            // *** BUSINESS VALIDATION ***
            if (string.IsNullOrWhiteSpace(tacGiaDto.TenTacGia))
            {
                throw new ArgumentException("Tên tác giả không được để trống.", nameof(tacGiaDto.TenTacGia));
            }
            string tenTacGiaTrimmed = tacGiaDto.TenTacGia.Trim();

            // Kiểm tra trùng Mã (nếu có) trong các bản ghi chưa bị xóa mềm
            if (!string.IsNullOrWhiteSpace(tacGiaDto.Matacgia))
            {
                string maTacGiaTrimmed = tacGiaDto.Matacgia.Trim();
                var exists = await _dalTacGia.IsMaTacGiaExistsAsync(maTacGiaTrimmed); // DAL kiểm tra DaAn=false
                if (exists)
                {
                    throw new InvalidOperationException($"Mã tác giả '{maTacGiaTrimmed}' đã tồn tại.");
                }
                tacGiaDto.Matacgia = maTacGiaTrimmed;
            }

            // Kiểm tra trùng Tên trong các bản ghi chưa bị xóa mềm
            var tenExists = await _dalTacGia.IsTenTacGiaExistsAsync(tenTacGiaTrimmed); // DAL kiểm tra DaAn=false
            if (tenExists)
            {
                throw new InvalidOperationException($"Tên tác giả '{tenTacGiaTrimmed}' đã tồn tại.");
            }
            tacGiaDto.TenTacGia = tenTacGiaTrimmed;

            try
            {
                // Mapping DTO sang Entity (DaAn sẽ là false do mặc định của bool hoặc do DAL gán)
                var entityToAdd = MapToTacGiaEntity(tacGiaDto);
                // entityToAdd.DaAn = false; // Đảm bảo DaAn là false (DAL cũng làm điều này)

                // Tự sinh mã nếu cần
                if (string.IsNullOrWhiteSpace(entityToAdd.Matacgia))
                {
                    entityToAdd.Matacgia = $"TG{DateTime.Now:yyyyMMddHHmmss}";
                }

                var addedEntity = await _dalTacGia.AddAsync(entityToAdd);
                return addedEntity != null ? MapToTacGiaDTO(addedEntity) : null;
            }
            catch (ArgumentException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSTacGia.AddTacGiaAsync: {ex.Message}");
                throw new Exception($"Lỗi khi thêm mới tác giả: {ex.Message}", ex);
            }
        }

        // Cập nhật thông tin tác giả từ DTO
        public async Task<bool> UpdateTacGiaAsync(TacGiaDTO tacGiaDto)
        {
            if (tacGiaDto == null) throw new ArgumentNullException(nameof(tacGiaDto));
            if (tacGiaDto.Id <= 0) throw new ArgumentException("ID tác giả không hợp lệ để cập nhật.", nameof(tacGiaDto.Id));

            // *** BUSINESS VALIDATION ***
            if (string.IsNullOrWhiteSpace(tacGiaDto.TenTacGia))
            {
                throw new ArgumentException("Tên tác giả không được để trống.", nameof(tacGiaDto.TenTacGia));
            }
            string tenTacGiaTrimmed = tacGiaDto.TenTacGia.Trim();

            // Kiểm tra trùng Mã (loại trừ chính nó, trong các bản ghi chưa bị xóa mềm)
            if (!string.IsNullOrWhiteSpace(tacGiaDto.Matacgia))
            {
                string maTacGiaTrimmed = tacGiaDto.Matacgia.Trim();
                var exists = await _dalTacGia.IsMaTacGiaExistsExcludingIdAsync(maTacGiaTrimmed, tacGiaDto.Id); // DAL kiểm tra DaAn=false
                if (exists)
                {
                    throw new InvalidOperationException($"Mã tác giả '{maTacGiaTrimmed}' đã tồn tại.");
                }
                tacGiaDto.Matacgia = maTacGiaTrimmed;
            }

            // Kiểm tra trùng Tên (loại trừ chính nó, trong các bản ghi chưa bị xóa mềm)
            var tenExists = await _dalTacGia.IsTenTacGiaExistsExcludingIdAsync(tenTacGiaTrimmed, tacGiaDto.Id); // DAL kiểm tra DaAn=false
            if (tenExists)
            {
                throw new InvalidOperationException($"Tên tác giả '{tenTacGiaTrimmed}' đã tồn tại.");
            }
            tacGiaDto.TenTacGia = tenTacGiaTrimmed;

            try
            {
                // Kiểm tra xem tác giả có tồn tại và chưa bị xóa mềm trước khi cập nhật không
                var existingEntity = await _dalTacGia.GetByIdAsync(tacGiaDto.Id); // DAL đã lọc DaAn=false
                if (existingEntity == null)
                {
                    // Có thể đã bị xóa mềm hoặc không tồn tại
                    throw new InvalidOperationException($"Không tìm thấy tác giả với ID {tacGiaDto.Id} hoặc tác giả đã bị ẩn để cập nhật.");
                }

                // Mapping DTO sang Entity (Giữ nguyên trạng thái DaAn = false từ existingEntity)
                var entityToUpdate = MapToTacGiaEntity(tacGiaDto);
                entityToUpdate.DaAn = existingEntity.DaAn; // Đảm bảo không ghi đè DaAn

                bool success = await _dalTacGia.UpdateAsync(entityToUpdate);
                return success;
            }
            catch (ArgumentException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSTacGia.UpdateTacGiaAsync (ID: {tacGiaDto.Id}): {ex.Message}");
                throw new Exception($"Lỗi khi cập nhật tác giả: {ex.Message}", ex);
            }
        }

        // Thực hiện xóa mềm tác giả theo ID
        public async Task<bool> SoftDeleteTacGiaAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID tác giả không hợp lệ để xóa.", nameof(id));
            try
            {
                // Gọi DAL để thực hiện Soft Delete
                return await _dalTacGia.SoftDeleteAsync(id);
            }
            catch (Exception ex) // Bắt lỗi chung từ DAL
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSTacGia.SoftDeleteTacGiaAsync (ID: {id}): {ex.Message}");
                throw new Exception($"Lỗi khi ẩn tác giả (ID: {id}): {ex.Message}", ex);
            }
        }

        // *** THÊM PHƯƠNG THỨC KHÔI PHỤC (RESTORE) ***
        public async Task<bool> RestoreTacGiaAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID tác giả không hợp lệ để khôi phục.", nameof(id));
            try
            {
                // Gọi DAL để thực hiện Restore
                return await _dalTacGia.RestoreAsync(id);
            }
            catch (Exception ex) // Bắt lỗi chung từ DAL
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSTacGia.RestoreTacGiaAsync (ID: {id}): {ex.Message}");
                throw new Exception($"Lỗi khi khôi phục tác giả (ID: {id}): {ex.Message}", ex);
            }
        }

        // *** ĐỔI TÊN PHƯƠNG THỨC HARD DELETE ***
        // Xóa vĩnh viễn tác giả theo ID (Hard Delete) - Sử dụng cẩn thận!
        public async Task<bool> HardDeleteTacGiaAsync(int id) // <<< ĐÃ ĐỔI TÊN
        {
            if (id <= 0) throw new ArgumentException("ID tác giả không hợp lệ để xóa vĩnh viễn.", nameof(id));
            try
            {
                // Gọi DAL để xóa vĩnh viễn (DAL đã kiểm tra ràng buộc)
                bool success = await _dalTacGia.HardDeleteAsync(id);
                return success;
            }
            catch (InvalidOperationException ex) // Bắt lỗi nghiệp vụ/ràng buộc từ DAL
            {
                System.Diagnostics.Debug.WriteLine($"Business rule violation (FK) in BUSTacGia.HardDeleteTacGiaAsync (ID: {id}): {ex.Message}");
                throw; // Ném lại lỗi nghiệp vụ
            }
            catch (Exception ex) // Bắt các lỗi hệ thống/DAL khác
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSTacGia.HardDeleteTacGiaAsync (ID: {id}): {ex.Message}");
                throw new Exception($"Lỗi khi xóa vĩnh viễn tác giả (ID: {id}): {ex.Message}", ex);
            }
        }

    } // Kết thúc class BUSTacGia
} // Kết thúc namespace BUS
