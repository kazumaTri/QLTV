// --- USING ---
using DAL; // Namespace chứa các lớp DAL (IDALLoaiDocGia)
using DAL.Models; // Namespace chứa các Entity Model (Loaidocgia)
using DTO; // Namespace chứa các lớp Data Transfer Objects (LoaiDocGiaDTO)
using System; // Cần cho ArgumentNullException, InvalidOperationException, Exception, Console.WriteLine, KeyNotFoundException
using System.Collections.Generic; // Cần cho List
using System.Diagnostics; // Cần cho Debug
using System.Linq; // Cần cho LINQ (Select)
using System.Threading.Tasks; // Cần cho Task

namespace BUS
{
    /// <summary>
    /// Business Logic Layer cho Loại Độc Giả.
    /// Chứa logic nghiệp vụ, validation, và phối hợp với tầng DAL.
    /// Đã nhất quán với Dependency Injection (nhận interface IDALLoaiDocGia).
    /// Triển khai đầy đủ IBUSLoaiDocGia (đã cập nhật Add/Update trả về DTO).
    /// </summary>
    public class BUSLoaiDocGia : IBUSLoaiDocGia // <<< Triển khai interface IBUSLoaiDocGia
    {
        // --- DEPENDENCIES ---
        private readonly IDALLoaiDocGia _dalLoaiDocGia;

        // --- CONSTRUCTOR (Sử dụng Dependency Injection) ---
        public BUSLoaiDocGia(IDALLoaiDocGia dalLoaiDocGia)
        {
            _dalLoaiDocGia = dalLoaiDocGia ?? throw new ArgumentNullException(nameof(dalLoaiDocGia));
        }

        // --- Mapping Functions ---

        /// <summary>
        /// Ánh xạ từ Entity Loaidocgia sang LoaiDocGiaDTO.
        /// </summary>
        /// <param name="entity">Entity Loaidocgia (có thể null).</param>
        /// <param name="count">Số lượng độc giả liên quan (tùy chọn).</param>
        /// <returns>LoaiDocGiaDTO hoặc null.</returns>
        private LoaiDocGiaDTO? MapToDTO(Loaidocgia? entity, int? count = null) // Thêm tham số count tùy chọn
        {
            if (entity == null) return null;
            return new LoaiDocGiaDTO
            {
                Id = entity.Id,
                MaLoaiDocGia = entity.MaLoaiDocGia,
                TenLoaiDocGia = entity.TenLoaiDocGia,
                SoLuongDocGia = count ?? 0 // Gán số lượng nếu được cung cấp
            };
        }

        /// <summary>
        /// Ánh xạ từ LoaiDocGiaDTO sang Entity Loaidocgia.
        /// Ném ArgumentException nếu dữ liệu không hợp lệ (vd: Tên rỗng).
        /// </summary>
        /// <param name="dto">LoaiDocGiaDTO.</param>
        /// <returns>Entity Loaidocgia.</returns>
        /// <exception cref="ArgumentException">Khi tên loại độc giả rỗng hoặc chỉ chứa khoảng trắng.</exception>
        private Loaidocgia MapToEntity(LoaiDocGiaDTO dto)
        {
            string tenLoai = dto.TenLoaiDocGia.Trim();
            string? maLoai = dto.MaLoaiDocGia?.Trim();

            if (string.IsNullOrWhiteSpace(tenLoai))
            {
                throw new ArgumentException("Tên loại độc giả không được để trống.", nameof(dto.TenLoaiDocGia));
            }

            return new Loaidocgia
            {
                Id = dto.Id,
                MaLoaiDocGia = string.IsNullOrEmpty(maLoai) ? null : maLoai,
                TenLoaiDocGia = tenLoai
            };
        }

        // --- Business Logic Methods (Triển khai từ IBUSLoaiDocGia) ---

        /// <inheritdoc/>
        public async Task<List<LoaiDocGiaDTO>> GetAllLoaiDocGiaAsync()
        {
            try
            {
                var entities = await _dalLoaiDocGia.GetAllAsync();
                // MapToDTO sẽ gán SoLuongDocGia = 0 vì DAL.GetAllAsync không trả về count
                return entities.Select(e => MapToDTO(e)!).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting all LoaiDocGia in BUS: {ex.Message}");
                throw new Exception("Lỗi khi lấy danh sách loại độc giả.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<LoaiDocGiaDTO?> GetLoaiDocGiaByIdAsync(int id)
        {
            try
            {
                var entity = await _dalLoaiDocGia.GetByIdAsync(id);
                // MapToDTO sẽ gán SoLuongDocGia = 0
                return MapToDTO(entity);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting LoaiDocGia by ID {id} in BUS: {ex.Message}");
                throw new Exception($"Lỗi khi lấy loại độc giả (ID: {id}).", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<LoaiDocGiaDTO?> GetLoaiDocGiaByMaAsync(string ma)
        {
            if (string.IsNullOrWhiteSpace(ma)) return null;
            try
            {
                var entity = await _dalLoaiDocGia.GetByMaAsync(ma);
                // MapToDTO sẽ gán SoLuongDocGia = 0
                return MapToDTO(entity);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting LoaiDocGia by Ma {ma} in BUS: {ex.Message}");
                throw new Exception($"Lỗi khi lấy loại độc giả (Mã: {ma}).", ex);
            }
        }

        // ****** ĐÂY LÀ PHIÊN BẢN ĐÚNG CỦA AddLoaiDocGiaAsync ******
        /// <inheritdoc/>
        public async Task<LoaiDocGiaDTO?> AddLoaiDocGiaAsync(LoaiDocGiaDTO loaiDocGiaDto) // <<< ĐÃ SỬA KIỂU TRẢ VỀ
        {
            if (loaiDocGiaDto == null)
                throw new ArgumentNullException(nameof(loaiDocGiaDto));

            try
            {
                // Tự động sinh mã
                string newMaLoaiDocGia = await GenerateNewMaLoaiDocGiaAsync(); // Gọi hàm sinh mã
                loaiDocGiaDto.MaLoaiDocGia = newMaLoaiDocGia; // Gán mã vừa sinh vào DTO
                Debug.WriteLine($"Generated MaLoaiDocGia: {newMaLoaiDocGia}");

                // Kiểm tra trùng lặp
                if (await CheckMaLoaiExistsAsync(loaiDocGiaDto.MaLoaiDocGia))
                { throw new InvalidOperationException($"Mã loại '{loaiDocGiaDto.MaLoaiDocGia}' tự sinh bị trùng."); }
                if (await CheckTenLoaiExistsAsync(loaiDocGiaDto.TenLoaiDocGia))
                { throw new InvalidOperationException($"Tên loại '{loaiDocGiaDto.TenLoaiDocGia}' đã tồn tại."); }

                // Map DTO to Entity (sẽ validate Tên rỗng)
                var entity = MapToEntity(loaiDocGiaDto);
                entity.Id = 0; // Đảm bảo ID là 0 khi thêm

                // Gọi DAL để thêm
                bool success = await _dalLoaiDocGia.AddAsync(entity);

                if (success) // Nếu DAL báo thêm thành công
                {
                    // *** KHÔNG GỌI GetByMaAsync NỮA ***
                    // Trả về DTO từ entity đã có sẵn Mã (ID sẽ là 0 hoặc giá trị db sinh ra nếu AddAsync cập nhật entity)
                    // UI sẽ dùng ID thật từ lưới sau khi LoadDataGrid()
                    return MapToDTO(entity); // <<< Chỉ cần Map từ entity đã chuẩn bị
                }
                else // Nếu DAL báo thêm thất bại
                {
                    Debug.WriteLine($"DAL.AddAsync failed for Ma: {newMaLoaiDocGia}");
                    return null; // Trả về null nếu DAL thêm thất bại
                }
            }
            catch (ArgumentException argEx) // Lỗi validation từ MapToEntity
            {
                Debug.WriteLine($"Validation Error in AddLoaiDocGiaAsync: {argEx.Message}");
                throw;
            }
            catch (InvalidOperationException ioEx) // Lỗi trùng lặp hoặc lỗi sinh mã
            {
                Debug.WriteLine($"Business Logic Error in AddLoaiDocGiaAsync: {ioEx.Message}");
                throw;
            }
            catch (Exception ex) // Lỗi hệ thống khác
            {
                Debug.WriteLine($"DAL/System Error adding LoaiDocGia in BUS: {ex.ToString()}");
                throw new Exception("Lỗi hệ thống khi thêm loại độc giả.", ex);
            }
        }

        // ****** ĐÂY LÀ PHIÊN BẢN ĐÚNG CỦA UpdateLoaiDocGiaAsync ******
        /// <inheritdoc/>
        public async Task<LoaiDocGiaDTO?> UpdateLoaiDocGiaAsync(LoaiDocGiaDTO loaiDocGiaDto) // <<< ĐÃ SỬA KIỂU TRẢ VỀ
        {
            if (loaiDocGiaDto == null)
                throw new ArgumentNullException(nameof(loaiDocGiaDto));
            if (loaiDocGiaDto.Id <= 0)
                throw new ArgumentException("ID không hợp lệ để cập nhật.", nameof(loaiDocGiaDto.Id));

            // Kiểm tra trùng Tên (loại trừ chính nó)
            if (await CheckTenLoaiExistsAsync(loaiDocGiaDto.TenLoaiDocGia, loaiDocGiaDto.Id))
            { throw new InvalidOperationException($"Tên loại '{loaiDocGiaDto.TenLoaiDocGia}' đã được sử dụng."); }

            try
            {
                // Lấy entity hiện có để đảm bảo tồn tại và lấy Mã gốc
                var existingEntity = await _dalLoaiDocGia.GetByIdAsync(loaiDocGiaDto.Id);
                if (existingEntity == null)
                { throw new KeyNotFoundException($"Không tìm thấy loại độc giả ID {loaiDocGiaDto.Id}."); }

                // Map DTO sang Entity mới để cập nhật (MapToEntity sẽ validate Tên)
                var entityToUpdate = MapToEntity(loaiDocGiaDto);
                // Đảm bảo Mã không bị thay đổi - Gán lại Mã từ entity gốc
                entityToUpdate.MaLoaiDocGia = existingEntity.MaLoaiDocGia;

                // Gọi DAL để cập nhật
                bool success = await _dalLoaiDocGia.UpdateAsync(entityToUpdate);

                if (success)
                {
                    // Trả về DTO đã cập nhật (lấy lại từ DB hoặc dùng DTO đã gửi đi)
                    // var updatedEntity = await _dalLoaiDocGia.GetByIdAsync(entityToUpdate.Id);
                    // return MapToDTO(updatedEntity);
                    return loaiDocGiaDto; // Trả về DTO đã gửi đi là đủ vì Mã không đổi
                }
                else
                {
                    Debug.WriteLine($"DAL.UpdateAsync failed for ID: {loaiDocGiaDto.Id}");
                    // Có thể xảy ra nếu UpdateAsync trả về false (ví dụ: lỗi concurrency không bắt được)
                    return null;
                }
            }
            catch (ArgumentException argEx) // Lỗi validation từ MapToEntity
            {
                Debug.WriteLine($"Validation Error in UpdateLoaiDocGiaAsync: {argEx.Message}");
                throw;
            }
            catch (KeyNotFoundException knfEx) // Lỗi không tìm thấy ID
            {
                Debug.WriteLine($"Key Not Found Error in UpdateLoaiDocGiaAsync: {knfEx.Message}");
                throw; // Ném lại lỗi
            }
            catch (InvalidOperationException ioEx) // Lỗi trùng lặp tên
            {
                Debug.WriteLine($"Business Logic Error (Duplicate) in UpdateLoaiDocGiaAsync: {ioEx.Message}");
                throw;
            }
            catch (Exception ex) // Bắt các lỗi khác
            {
                Debug.WriteLine($"DAL/System Error updating LoaiDocGia ID {loaiDocGiaDto.Id} in BUS: {ex.ToString()}");
                throw new Exception($"Lỗi hệ thống khi cập nhật loại độc giả (ID: {loaiDocGiaDto.Id}).", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteLoaiDocGiaAsync(int id) // Delete vẫn trả về bool
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID của loại độc giả không hợp lệ để xóa.", nameof(id));
            }

            try
            {
                // Kiểm tra ràng buộc nghiệp vụ
                int relatedReaderCount = await CountDocGiaByLoaiAsync(id);
                if (relatedReaderCount > 0)
                {
                    throw new InvalidOperationException($"Không thể xóa vì còn {relatedReaderCount} độc giả thuộc loại này.");
                }

                // Gọi DAL để xóa
                return await _dalLoaiDocGia.DeleteAsync(id);
            }
            catch (InvalidOperationException ex) // Bắt lỗi ràng buộc
            {
                Debug.WriteLine($"Business Rule Violation (Delete) in BUS for ID {id}: {ex.Message}");
                throw;
            }
            catch (Exception ex) // Bắt các lỗi khác từ DAL
            {
                Debug.WriteLine($"DAL/System Error deleting LoaiDocGia ID {id} in BUS: {ex.ToString()}");
                throw new Exception($"Lỗi hệ thống khi xóa loại độc giả (ID: {id}).", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<List<LoaiDocGiaDTO>> SearchLoaiDocGiaAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllLoaiDocGiaAsync();
            }
            try
            {
                var entities = await _dalLoaiDocGia.SearchAsync(searchTerm);
                // MapToDTO sẽ gán SoLuongDocGia = 0
                return entities.Select(e => MapToDTO(e)!).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error searching LoaiDocGia in BUS (term: {searchTerm}): {ex.Message}");
                throw new Exception("Lỗi trong quá trình tìm kiếm loại độc giả.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<List<LoaiDocGiaDTO>> GetLoaiDocGiaFilteredAndSortedAsync(string? searchTerm, string sortColumn, bool ascending)
        {
            try
            {
                // Gọi hàm DAL trả về cả Entity và Số lượng
                var results = await _dalLoaiDocGia.GetFilteredAndSortedWithCountAsync(searchTerm, sortColumn, ascending);
                // Map kết quả sang DTO, bao gồm cả số lượng
                return results.Select(r => MapToDTO(r.Entity, r.Count)!).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting filtered/sorted/counted LoaiDocGia in BUS: {ex.Message}");
                throw new Exception("Lỗi khi lấy danh sách loại độc giả kèm số lượng.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> CheckMaLoaiExistsAsync(string maLoai, int currentId = 0)
        {
            if (string.IsNullOrWhiteSpace(maLoai)) return false;
            try
            {
                return await _dalLoaiDocGia.AnyAsync(ldg => ldg.Id != currentId && ldg.MaLoaiDocGia != null && ldg.MaLoaiDocGia.ToLower() == maLoai.Trim().ToLower());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking MaLoaiDocGia existence in BUS (Ma: {maLoai}): {ex.Message}");
                return false; // Trả về false khi có lỗi kiểm tra
            }
        }

        /// <inheritdoc/>
        public async Task<bool> CheckTenLoaiExistsAsync(string tenLoai, int currentId = 0)
        {
            if (string.IsNullOrWhiteSpace(tenLoai)) return false;
            try
            {
                return await _dalLoaiDocGia.AnyAsync(ldg => ldg.Id != currentId && ldg.TenLoaiDocGia != null && ldg.TenLoaiDocGia.ToLower() == tenLoai.Trim().ToLower());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking TenLoaiDocGia existence in BUS (Ten: {tenLoai}): {ex.Message}");
                return false; // Trả về false khi có lỗi kiểm tra
            }
        }

        /// <inheritdoc/>
        public async Task<int> CountDocGiaByLoaiAsync(int loaiDocGiaId)
        {
            if (loaiDocGiaId <= 0) return 0;
            try
            {
                return await _dalLoaiDocGia.CountRelatedDocGiaAsync(loaiDocGiaId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error counting DocGia for LoaiDocGia ID {loaiDocGiaId} in BUS: {ex.Message}");
                return -1; // Trả về -1 báo lỗi
            }
        }

        // --- Hàm sinh mã tự động ---
        /// <summary>
        /// Hàm sinh Mã Loại Độc Giả mới một cách tự động.
        /// Ví dụ: Lấy mã cuối cùng dạng "LDGxxx" và tăng số lên.
        /// </summary>
        /// <returns>Mã Loại Độc Giả mới.</returns>
        /// <exception cref="Exception">Ném lỗi nếu không thể sinh mã.</exception>
        private async Task<string> GenerateNewMaLoaiDocGiaAsync()
        {
            const string prefix = "LDG";
            int nextNumber = 1;

            try
            {
                string? lastCode = await _dalLoaiDocGia.GetLastMaLoaiDocGiaAsync(prefix);

                if (!string.IsNullOrEmpty(lastCode) && lastCode.StartsWith(prefix))
                {
                    string numberPart = lastCode.Substring(prefix.Length);
                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }
                // Format mã mới với 3 chữ số (ví dụ: LDG001)
                return $"{prefix}{nextNumber:D3}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error generating new MaLoaiDocGia: {ex.ToString()}");
                throw new Exception("Không thể tạo mã loại độc giả tự động.", ex);
            }
        }

        // --- Hàm Validation cũ (không cần thiết nếu validation đã tích hợp vào Add/Update) ---
        // private bool IsValid(LoaiDocGiaDTO dto, out List<string> validationErrors) { ... }

    } // Kết thúc class BUSLoaiDocGia
} // Kết thúc namespace BUS