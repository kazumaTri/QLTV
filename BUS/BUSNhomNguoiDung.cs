// BUS/BUSNhomNguoiDung.cs
using DAL; // Cần để sử dụng IDALNhomNguoiDung
using DAL.Models; // Cần để sử dụng Entity Nhomnguoidung
using DTO; // Cần để sử dụng NhomNguoiDungDTO
using System;
using System.Collections.Generic;
using System.Linq; // Cần cho Select
using System.Threading.Tasks;
using System.Diagnostics; // Cho Debug.WriteLine

namespace BUS
{
    /// <summary>
    /// Lớp cài đặt logic nghiệp vụ cho Nhóm Người Dùng.
    /// </summary>
    public class BUSNhomNguoiDung : IBUSNhomNguoiDung // Implement interface
    {
        private readonly IDALNhomNguoiDung _dalNhomNguoiDung;

        // Constructor nhận IDALNhomNguoiDung qua Dependency Injection
        public BUSNhomNguoiDung(IDALNhomNguoiDung dalNhomNguoiDung)
        {
            _dalNhomNguoiDung = dalNhomNguoiDung ?? throw new ArgumentNullException(nameof(dalNhomNguoiDung));
        }

        // --- Hàm chuyển đổi (Mapping) ---

        /// <summary>
        /// Chuyển đổi từ Entity Nhomnguoidung sang NhomNguoiDungDTO.
        /// </summary>
        /// <param name="entity">Đối tượng Entity Nhomnguoidung.</param>
        /// <returns>Đối tượng NhomNguoiDungDTO hoặc null nếu entity là null.</returns>
        private NhomNguoiDungDTO? MapToDTO(Nhomnguoidung? entity)
        {
            if (entity == null)
            {
                return null;
            }
            return new NhomNguoiDungDTO
            {
                Id = entity.Id,
                MaNhomNguoiDung = entity.MaNhomNguoiDung,
                TenNhomNguoiDung = entity.TenNhomNguoiDung
                // Không map danh sách quyền ở đây để giữ DTO đơn giản
            };
        }

        /// <summary>
        /// Chuyển đổi từ NhomNguoiDungDTO sang Entity Nhomnguoidung.
        /// Chỉ map các thuộc tính cơ bản (Id, Mã, Tên).
        /// </summary>
        /// <param name="dto">Đối tượng NhomNguoiDungDTO.</param>
        /// <returns>Đối tượng Entity Nhomnguoidung.</returns>
        /// <exception cref="ArgumentNullException">Ném lỗi nếu dto là null.</exception>
        private Nhomnguoidung MapToEntity(NhomNguoiDungDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // Không cần gọi Validate ở đây nữa vì đã gọi trong Add/Update

            return new Nhomnguoidung
            {
                Id = dto.Id, // Id = 0 khi thêm mới, > 0 khi cập nhật
                MaNhomNguoiDung = dto.MaNhomNguoiDung?.Trim(), // Mã có thể null
                TenNhomNguoiDung = dto.TenNhomNguoiDung.Trim() // Tên đã được validate không rỗng
                // Không map quyền ở đây, quyền được xử lý riêng
            };
        }

        // --- Hàm kiểm tra dữ liệu (Validation) ---

        /// <summary>
        /// Kiểm tra tính hợp lệ của dữ liệu trong NhomNguoiDungDTO.
        /// </summary>
        /// <param name="dto">Đối tượng DTO cần kiểm tra.</param>
        /// <param name="isAdding">True nếu đang kiểm tra cho thao tác thêm mới (có thể có logic khác).</param>
        /// <exception cref="ArgumentNullException">Ném lỗi nếu dto là null.</exception>
        /// <exception cref="ArgumentException">Ném lỗi nếu dữ liệu không hợp lệ.</exception>
        private void ValidateNhomNguoiDungDTO(NhomNguoiDungDTO dto, bool isAdding = false)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.TenNhomNguoiDung))
            {
                errors.Add("Tên nhóm người dùng không được để trống.");
            }
            // Thêm các kiểm tra khác nếu cần (ví dụ: độ dài tối đa)
            // if (dto.TenNhomNguoiDung.Length > 100) errors.Add("Tên nhóm quá dài.");
            // if (!string.IsNullOrEmpty(dto.MaNhomNguoiDung) && dto.MaNhomNguoiDung.Length > 50) errors.Add("Mã nhóm quá dài.");

            // Việc kiểm tra trùng lặp Mã/Tên sẽ được thực hiện trong Add/Update Async

            if (errors.Any())
            {
                // Ném lỗi với danh sách các vấn đề
                throw new ArgumentException("Dữ liệu Nhóm người dùng không hợp lệ:\n- " + string.Join("\n- ", errors));
            }
        }


        // --- Cài đặt các phương thức từ Interface IBUSNhomNguoiDung ---

        /// <summary>
        /// Lấy tất cả các nhóm người dùng.
        /// </summary>
        /// <returns>Danh sách NhomNguoiDungDTO.</returns>
        public async Task<List<NhomNguoiDungDTO>> GetAllNhomNguoiDungAsync()
        {
            try
            {
                var entities = await _dalNhomNguoiDung.GetAllAsync();
                // Chuyển đổi danh sách Entity sang DTO, bỏ qua các kết quả null (nếu có)
                return entities.Select(MapToDTO).Where(dto => dto != null).ToList()!;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong BUSNhomNguoiDung.GetAllNhomNguoiDungAsync: {ex.Message}");
                // Ném lại lỗi để tầng trên xử lý
                throw new Exception("Đã xảy ra lỗi khi lấy danh sách nhóm người dùng.", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin nhóm người dùng theo ID.
        /// </summary>
        /// <param name="id">ID của nhóm cần lấy.</param>
        /// <returns>NhomNguoiDungDTO hoặc null nếu không tìm thấy.</returns>
        public async Task<NhomNguoiDungDTO?> GetNhomNguoiDungByIdAsync(int id)
        {
            if (id <= 0) return null; // ID không hợp lệ
            try
            {
                var entity = await _dalNhomNguoiDung.GetByIdAsync(id);
                return MapToDTO(entity); // MapToDTO đã xử lý trường hợp entity null
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong BUSNhomNguoiDung.GetNhomNguoiDungByIdAsync (ID: {id}): {ex.Message}");
                throw new Exception($"Đã xảy ra lỗi khi lấy thông tin nhóm người dùng (ID: {id}).", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin nhóm người dùng theo Mã nhóm.
        /// </summary>
        /// <param name="maNhom">Mã của nhóm cần lấy.</param>
        /// <returns>NhomNguoiDungDTO hoặc null nếu không tìm thấy hoặc mã không hợp lệ.</returns>
        public async Task<NhomNguoiDungDTO?> GetNhomNguoiDungByMaAsync(string maNhom)
        {
            if (string.IsNullOrWhiteSpace(maNhom))
            {
                return null; // Mã không hợp lệ
            }
            try
            {
                var entity = await _dalNhomNguoiDung.GetByMaAsync(maNhom.Trim());
                return MapToDTO(entity);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong BUSNhomNguoiDung.GetNhomNguoiDungByMaAsync (Ma: {maNhom}): {ex.Message}");
                throw new Exception($"Đã xảy ra lỗi khi lấy thông tin nhóm người dùng (Mã: {maNhom}).", ex);
            }
        }

        /// <summary>
        /// Thêm một nhóm người dùng mới (chỉ thông tin cơ bản).
        /// Việc gán quyền sẽ được thực hiện qua UpdatePhanQuyenAsync sau khi có ID.
        /// </summary>
        /// <param name="newNhomDTO">DTO chứa thông tin nhóm mới.</param>
        /// <returns>DTO của nhóm đã được thêm (với ID) hoặc null nếu thất bại.</returns>
        /// <exception cref="ArgumentNullException">Ném lỗi nếu newNhomDTO là null.</exception>
        /// <exception cref="ArgumentException">Ném lỗi nếu dữ liệu DTO không hợp lệ.</exception>
        /// <exception cref="InvalidOperationException">Ném lỗi nếu Mã nhóm đã tồn tại.</exception>
        public async Task<NhomNguoiDungDTO?> AddNhomNguoiDungAsync(NhomNguoiDungDTO newNhomDTO)
        {
            // 1. Kiểm tra đầu vào
            if (newNhomDTO == null) throw new ArgumentNullException(nameof(newNhomDTO));
            ValidateNhomNguoiDungDTO(newNhomDTO, isAdding: true); // Kiểm tra dữ liệu cơ bản

            try
            {
                // 2. Kiểm tra logic nghiệp vụ (trùng lặp)
                if (!string.IsNullOrWhiteSpace(newNhomDTO.MaNhomNguoiDung))
                {
                    var existingByMa = await _dalNhomNguoiDung.GetByMaAsync(newNhomDTO.MaNhomNguoiDung.Trim());
                    if (existingByMa != null)
                    {
                        // Ném lỗi cụ thể để GUI có thể xử lý
                        throw new InvalidOperationException($"Mã nhóm người dùng '{newNhomDTO.MaNhomNguoiDung}' đã tồn tại.");
                    }
                }
                // TODO: Thêm kiểm tra trùng Tên nếu cần thiết
                // var existingByName = await _dalNhomNguoiDung.GetByNameAsync(newNhomDTO.TenNhomNguoiDung.Trim());
                // if(existingByName != null) { throw new InvalidOperationException($"Tên nhóm '{newNhomDTO.TenNhomNguoiDung}' đã tồn tại."); }

                // 3. Chuyển đổi sang Entity
                var entity = MapToEntity(newNhomDTO);
                entity.Id = 0; // Đảm bảo ID là 0 khi thêm

                // 4. Gọi DAL để thêm vào CSDL
                var addedEntity = await _dalNhomNguoiDung.AddAsync(entity);

                // 5. Chuyển đổi kết quả trả về DTO
                return MapToDTO(addedEntity); // Trả về DTO với ID đã được gán
            }
            catch (InvalidOperationException ex) // Bắt lỗi trùng mã từ bước 2 hoặc lỗi DB từ DAL
            {
                Debug.WriteLine($"Lỗi nghiệp vụ khi thêm nhóm: {ex.Message}");
                throw; // Ném lại lỗi nghiệp vụ để GUI xử lý
            }
            catch (Exception ex) // Bắt các lỗi khác
            {
                Debug.WriteLine($"Lỗi không xác định trong BUSNhomNguoiDung.AddNhomNguoiDungAsync: {ex.Message}");
                throw new Exception("Đã xảy ra lỗi khi thêm nhóm người dùng.", ex); // Ném lỗi chung
            }
        }

        /// <summary>
        /// Cập nhật thông tin cơ bản (Mã, Tên) của một nhóm người dùng.
        /// Việc cập nhật quyền được thực hiện qua UpdatePhanQuyenAsync.
        /// </summary>
        /// <param name="updatedNhomDTO">DTO chứa thông tin cần cập nhật (phải có ID).</param>
        /// <returns>True nếu cập nhật thành công, False nếu không tìm thấy nhóm hoặc cập nhật thất bại.</returns>
        /// <exception cref="ArgumentNullException">Ném lỗi nếu updatedNhomDTO là null.</exception>
        /// <exception cref="ArgumentException">Ném lỗi nếu ID hoặc dữ liệu không hợp lệ.</exception>
        /// <exception cref="InvalidOperationException">Ném lỗi nếu Mã nhóm bị trùng với nhóm khác.</exception>
        /// <exception cref="KeyNotFoundException">Ném lỗi nếu không tìm thấy nhóm với ID cung cấp (từ DAL).</exception>
        public async Task<bool> UpdateNhomNguoiDungAsync(NhomNguoiDungDTO updatedNhomDTO)
        {
            // 1. Kiểm tra đầu vào
            if (updatedNhomDTO == null) throw new ArgumentNullException(nameof(updatedNhomDTO));
            if (updatedNhomDTO.Id <= 0) throw new ArgumentException("ID nhóm người dùng không hợp lệ để cập nhật.", nameof(updatedNhomDTO.Id));
            ValidateNhomNguoiDungDTO(updatedNhomDTO); // Kiểm tra dữ liệu cơ bản

            try
            {
                // 2. Kiểm tra logic nghiệp vụ (trùng lặp với nhóm khác)
                if (!string.IsNullOrWhiteSpace(updatedNhomDTO.MaNhomNguoiDung))
                {
                    var existingByMa = await _dalNhomNguoiDung.GetByMaAsync(updatedNhomDTO.MaNhomNguoiDung.Trim());
                    // Nếu tìm thấy nhóm có cùng Mã VÀ ID khác với nhóm đang sửa -> lỗi trùng lặp
                    if (existingByMa != null && existingByMa.Id != updatedNhomDTO.Id)
                    {
                        throw new InvalidOperationException($"Mã nhóm người dùng '{updatedNhomDTO.MaNhomNguoiDung}' đã được sử dụng bởi nhóm khác.");
                    }
                }
                // TODO: Thêm kiểm tra trùng Tên nếu cần

                // 3. Chuyển đổi sang Entity (chỉ chứa thông tin cơ bản)
                var entityToUpdate = MapToEntity(updatedNhomDTO);

                // 4. Gọi DAL để cập nhật
                return await _dalNhomNguoiDung.UpdateAsync(entityToUpdate);
            }
            catch (InvalidOperationException ex) // Bắt lỗi trùng mã từ bước 2 hoặc lỗi DB từ DAL
            {
                Debug.WriteLine($"Lỗi nghiệp vụ khi cập nhật nhóm (ID: {updatedNhomDTO.Id}): {ex.Message}");
                throw; // Ném lại lỗi nghiệp vụ
            }
            catch (KeyNotFoundException ex) // Bắt lỗi không tìm thấy từ DAL.UpdateAsync
            {
                Debug.WriteLine($"Lỗi không tìm thấy khi cập nhật nhóm (ID: {updatedNhomDTO.Id}): {ex.Message}");
                throw; // Ném lại lỗi không tìm thấy
            }
            catch (Exception ex) // Bắt các lỗi khác
            {
                Debug.WriteLine($"Lỗi không xác định trong BUSNhomNguoiDung.UpdateNhomNguoiDungAsync (ID: {updatedNhomDTO.Id}): {ex.Message}");
                throw new Exception($"Đã xảy ra lỗi khi cập nhật thông tin nhóm người dùng (ID: {updatedNhomDTO.Id}).", ex);
            }
        }

        /// <summary>
        /// Xóa một nhóm người dùng (xóa cứng).
        /// </summary>
        /// <param name="id">ID của nhóm cần xóa.</param>
        /// <returns>True nếu xóa thành công, False nếu không tìm thấy.</returns>
        /// <exception cref="ArgumentException">Ném lỗi nếu ID không hợp lệ.</exception>
        /// <exception cref="InvalidOperationException">Ném lỗi nếu nhóm còn người dùng (từ DAL).</exception>
        public async Task<bool> DeleteNhomNguoiDungAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID nhóm người dùng không hợp lệ để xóa.", nameof(id));

            try
            {
                // Gọi DAL để xóa, DAL đã có kiểm tra ràng buộc người dùng
                return await _dalNhomNguoiDung.DeleteAsync(id);
            }
            catch (InvalidOperationException ex) // Bắt lỗi ràng buộc từ DAL
            {
                Debug.WriteLine($"Lỗi ràng buộc khi xóa nhóm (ID: {id}): {ex.Message}");
                throw; // Ném lại lỗi nghiệp vụ rõ ràng
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi không xác định trong BUSNhomNguoiDung.DeleteNhomNguoiDungAsync (ID: {id}): {ex.Message}");
                throw new Exception($"Đã xảy ra lỗi khi xóa nhóm người dùng (ID: {id}).", ex);
            }
        }

        // === PHƯƠNG THỨC QUẢN LÝ PHÂN QUYỀN ===

        /// <summary>
        /// Lấy danh sách ID các Chức năng đã được gán cho một Nhóm người dùng.
        /// </summary>
        /// <param name="nhomId">ID của nhóm người dùng.</param>
        /// <returns>Danh sách các ID chức năng.</returns>
        /// <exception cref="ArgumentException">Ném lỗi nếu nhomId không hợp lệ.</exception>
        public async Task<List<int>> GetChucNangIdsByNhomAsync(int nhomId)
        {
            if (nhomId <= 0) throw new ArgumentException("ID nhóm người dùng không hợp lệ.", nameof(nhomId));

            try
            {
                // Gọi thẳng phương thức tương ứng của DAL
                return await _dalNhomNguoiDung.GetChucNangIdsByNhomIdAsync(nhomId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong BUSNhomNguoiDung.GetChucNangIdsByNhomAsync (ID: {nhomId}): {ex.Message}");
                // Ném lại lỗi để lớp gọi (GUI) xử lý
                throw new Exception($"Đã xảy ra lỗi khi lấy danh sách quyền của nhóm người dùng (ID: {nhomId}).", ex);
            }
        }

        /// <summary>
        /// Cập nhật danh sách Chức năng được gán cho một Nhóm người dùng.
        /// Phương thức này sẽ xóa hết quyền cũ và gán lại quyền mới dựa trên danh sách ID cung cấp.
        /// </summary>
        /// <param name="nhomId">ID của nhóm cần cập nhật quyền.</param>
        /// <param name="chucNangIds">Danh sách ID các chức năng mới cần gán. Nếu rỗng hoặc null, tất cả quyền sẽ bị xóa.</param>
        /// <returns>True nếu cập nhật thành công, False nếu không tìm thấy nhóm.</returns>
        /// <exception cref="ArgumentException">Ném lỗi nếu nhomId không hợp lệ.</exception>
        /// <exception cref="InvalidOperationException">Ném lỗi nếu có lỗi CSDL khi cập nhật.</exception>
        public async Task<bool> UpdatePhanQuyenAsync(int nhomId, List<int> chucNangIds)
        {
            if (nhomId <= 0) throw new ArgumentException("ID nhóm người dùng không hợp lệ.", nameof(nhomId));
            // chucNangIds có thể null hoặc rỗng, DAL sẽ xử lý việc này

            try
            {
                // Có thể thêm logic kiểm tra nghiệp vụ ở đây nếu cần
                // Ví dụ: Kiểm tra xem các chucNangIds có hợp lệ không (tồn tại trong bảng CHUCNANG?)
                // Tuy nhiên, DAL.UpdatePhanQuyenAsync đã xử lý việc chỉ thêm các ID tồn tại.

                // Gọi thẳng phương thức tương ứng của DAL
                // Đảm bảo truyền vào một list (có thể rỗng) thay vì null
                return await _dalNhomNguoiDung.UpdatePhanQuyenAsync(nhomId, chucNangIds ?? new List<int>());
            }
            catch (InvalidOperationException ex) // Bắt lỗi CSDL từ DAL
            {
                Debug.WriteLine($"Lỗi CSDL khi cập nhật quyền cho nhóm (ID: {nhomId}): {ex.Message}");
                throw; // Ném lại lỗi CSDL
            }
            catch (Exception ex) // Bắt các lỗi khác
            {
                Debug.WriteLine($"Lỗi không xác định trong BUSNhomNguoiDung.UpdatePhanQuyenAsync (ID: {nhomId}): {ex.Message}");
                // Ném lại lỗi để lớp gọi (GUI) xử lý
                throw new Exception($"Đã xảy ra lỗi khi cập nhật quyền cho nhóm người dùng (ID: {nhomId}).", ex);
            }
        }

    } // End class BUSNhomNguoiDung
} // End namespace BUS
