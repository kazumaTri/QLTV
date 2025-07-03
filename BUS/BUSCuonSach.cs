// Project/Namespace: BUS
using DAL; // Cần cho IDALCuonSach, IDALSach (để validate IdSach)
using DAL.Models; // Cần để làm việc với Entity Cuonsach, Sach (cho mapping)
using DTO; // Cần cho CuonSachDTO, TuaSachDTO, TheLoaiDTO, TacGiaDTO
using System; // Cần cho Exception, ArgumentNullException, InvalidOperationException
using System.Collections.Generic; // Cần cho List
using System.Linq; // Cần cho LINQ (Select, Any)
using System.Threading.Tasks; // Cần cho async/await Task

namespace BUS
{
    /// <summary>
    /// Business Logic Layer triển khai IBUSCuonSach, xử lý nghiệp vụ cho Cuốn Sách.
    /// Phụ thuộc vào IDALCuonSach. Thực hiện mapping DTO/Entity.
    /// Xử lý các quy tắc nghiệp vụ liên quan đến tình trạng và xóa mềm cuốn sách.
    /// </summary>
    public class BUSCuonSach : IBUSCuonSach // <<< Implement interface IBUSCuonSach
    {
        // --- DEPENDENCIES ---
        private readonly IDALCuonSach _dalCuonSach;
        private readonly IDALSach _dalSach; // Cần inject DAL Sach để kiểm tra IdSach

        // --- CONSTRUCTOR (Sử dụng Dependency Injection) ---
        // Constructor nhận các DAL dependencies qua interface
        public BUSCuonSach(IDALCuonSach dalCuonSach, IDALSach dalSach) // Nhận IDAL qua DI
        {
            _dalCuonSach = dalCuonSach ?? throw new ArgumentNullException(nameof(dalCuonSach));
            _dalSach = dalSach ?? throw new ArgumentNullException(nameof(dalSach));
        }

        // --- PRIVATE HELPER METHOD: MAPPING ENTITY TO DTO ---
        // Ánh xạ từ Entity Cuonsach sang DTO CuonSachDTO
        private CuonSachDTO MapToCuonSachDTO(Cuonsach entity) // <<< Entity type is Cuonsach
        {
            if (entity == null) return null!;

            var dto = new CuonSachDTO
            {
                Id = entity.Id,
                MaCuonSach = entity.MaCuonSach, // Có thể là null
                IdSach = entity.IdSach,
                TinhTrang = entity.TinhTrang,
                TinhTrangText = GetTinhTrangText(entity.TinhTrang), // Mapping số sang text
                DaAn = entity.DaAn,
            };

            // Lấy thông tin từ Sach (IdSachNavigation) nếu đã được Include ở DAL
            if (entity.IdSachNavigation != null)
            {
                dto.MaSach = entity.IdSachNavigation.MaSach; // Mã Đầu sách từ Sach
                dto.DonGia = entity.IdSachNavigation.DonGia;
                dto.NamXb = entity.IdSachNavigation.NamXb;
                dto.NhaXb = entity.IdSachNavigation.NhaXb;

                // Lấy thông tin từ TuaSach (IdTuaSachNavigation) nếu đã được Include ở DAL
                if (entity.IdSachNavigation.IdTuaSachNavigation != null)
                {
                    dto.IdTuaSach = entity.IdSachNavigation.IdTuaSachNavigation.Id; // ID Tựa sách
                    dto.MaTuaSach = entity.IdSachNavigation.IdTuaSachNavigation.MaTuaSach; // Mã Tựa sách
                    dto.TenTuaSach = entity.IdSachNavigation.IdTuaSachNavigation.TenTuaSach; // Tên Tựa sách

                    // Lấy tên Thể loại từ TuaSach -> Theloai nếu đã Include
                    if (entity.IdSachNavigation.IdTuaSachNavigation.IdTheLoaiNavigation != null)
                    {
                        dto.IdTheLoai = entity.IdSachNavigation.IdTuaSachNavigation.IdTheLoaiNavigation.Id; // ID Thể loại
                        dto.TenTheLoai = entity.IdSachNavigation.IdTuaSachNavigation.IdTheLoaiNavigation.TenTheLoai; // Tên Thể loại
                    }


                    // Lấy tên Tác giả từ TuaSach -> Tacgia (collection IdTacGia) nếu đã Include
                    // Nếu nhiều tác giả, cần format thành string hoặc list DTO
                    if (entity.IdSachNavigation.IdTuaSachNavigation.IdTacGia != null && entity.IdSachNavigation.IdTuaSachNavigation.IdTacGia.Any())
                    {
                        // Ví dụ đơn giản lấy tên tác giả đầu tiên hoặc format thành string
                        // dto.TenTacGia = string.Join(", ", entity.IdSachNavigation.IdTuaSachNavigation.IdTacGia.Select(tg => tg.TenTacGia)); // Tên tác giả
                        // Hoặc nếu DTO TuaSachDTO cần List<TacGiaDTO>, có thể tạo List ở đây và gán vào TuaSachDTO (nếu có)
                        dto.TacGias = entity.IdSachNavigation.IdTuaSachNavigation.IdTacGia.Select(tg => new TacGiaDTO { Id = tg.Id, Matacgia = tg.Matacgia, TenTacGia = tg.TenTacGia }).ToList(); // Danh sách tác giả DTO
                    }

                }
            }

            return dto;
        }

        // --- PRIVATE HELPER METHOD: MAPPING DTO TO ENTITY (For Add/Update) ---
        // Ánh xạ từ DTO CuonSachDTO sang Entity Cuonsach
        private Cuonsach MapToCuonSachEntity(CuonSachDTO dto) // <<< Entity type is Cuonsach
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var entity = new Cuonsach // <<< Entity type is Cuonsach
            {
                Id = dto.Id, // ID sẽ là 0 khi thêm mới, EF Core tự xử lý nếu identity
                MaCuonSach = dto.MaCuonSach?.Trim(), // Trim whitespace (nullable string)
                IdSach = dto.IdSach, // FK đến Sach
                TinhTrang = dto.TinhTrang, // Tình trạng số
                DaAn = dto.DaAn // Cờ xóa mềm
                // Không gán navigation properties ở đây khi mapping DTO -> Entity
            };

            return entity;
        }


        // --- BUSINESS RULE: Mapping TinhTrang number to Text ---
        public string GetTinhTrangText(int tinhTrang)
        {
            return tinhTrang switch
            {
                0 => "Có sẵn",
                1 => "Đang mượn",
                2 => "Hỏng / Mất",
                3 => "Đã thanh lý / Xóa mềm", // Thêm trạng thái cho xóa mềm nếu cần
                // Có thể thêm các trạng thái khác nếu có trong DB
                _ => "Không xác định" // Trạng thái mặc định cho các giá trị không khớp
            };
        }

        // --- METHOD IMPLEMENTATIONS (Từ IBUSCuonSach) ---

        // Lấy tất cả cuốn sách dưới dạng DTO (Chỉ lấy các cuốn sách chưa xóa mềm)
        public async Task<List<CuonSachDTO>> GetAllCuonSachAsync()
        {
            try
            {
                // Gọi DAL để lấy Entity list (DAL đã lọc DaAn=0 và Include các Navigation Property cần thiết)
                var entities = await _dalCuonSach.GetAllAsync();
                // Mapping từ Entity list sang DTO list
                return entities.Select(e => MapToCuonSachDTO(e)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSCuonSach.GetAllCuonSachAsync: {ex.Message}");
                throw; // Ném lại lỗi cho tầng GUI xử lý/hiển thị
            }
        }

        // Lấy cuốn sách theo ID dưới dạng DTO
        public async Task<CuonSachDTO?> GetCuonSachByIdAsync(int id)
        {
            if (id <= 0) return null;
            try
            {
                // Gọi DAL để lấy Entity theo ID (DAL đã kiểm tra DaAn=0 và Include)
                var entity = await _dalCuonSach.GetByIdAsync(id);
                // Mapping Entity sang DTO và trả về
                return entity != null ? MapToCuonSachDTO(entity) : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSCuonSach.GetCuonSachByIdAsync (ID: {id}): {ex.Message}");
                throw;
            }
        }

        // Lấy cuốn sách theo Mã dưới dạng DTO (Chỉ lấy cuốn sách chưa xóa mềm)
        public async Task<CuonSachDTO?> GetCuonSachByMaAsync(string maCuonSach)
        {
            if (string.IsNullOrWhiteSpace(maCuonSach)) return null;
            try
            {
                // Gọi DAL để lấy Entity theo Mã (DAL đã lọc DaAn=0)
                var entity = await _dalCuonSach.GetByMaAsync(maCuonSach);
                // Mapping Entity sang DTO và trả về
                return entity != null ? MapToCuonSachDTO(entity) : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSCuonSach.GetCuonSachByMaAsync (Ma: {maCuonSach}): {ex.Message}");
                throw;
            }
        }

        // Lấy tất cả cuốn sách thuộc một Sach ID dưới dạng DTO (Chỉ lấy cuốn sách chưa xóa mềm)
        public async Task<List<CuonSachDTO>> GetCuonSachBySachIdAsync(int sachId)
        {
            if (sachId <= 0) return new List<CuonSachDTO>();
            try
            {
                // Gọi DAL để lấy Entity list theo Sach ID (DAL đã lọc DaAn=0 và Include)
                var entities = await _dalCuonSach.GetBySachIdAsync(sachId); // <<< Gọi phương thức DAL
                // Mapping từ Entity list sang DTO list
                return entities.Select(e => MapToCuonSachDTO(e)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSCuonSach.GetCuonSachBySachIdAsync (SachId: {sachId}): {ex.Message}");
                throw;
            }
        }


        // Kiểm tra cuốn sách có trạng thái 'Có sẵn' không
        public async Task<bool> IsCuonSachAvailableAsync(int id)
        {
            if (id <= 0) return false;
            try
            {
                // Gọi DAL để lấy cuốn sách
                var cuonSach = await _dalCuonSach.GetByIdAsync(id); // GetByIdAsync đã lọc DaAn=0
                // Kiểm tra tình trạng
                return cuonSach != null && cuonSach.TinhTrang == 0; // Giả định 0 là Có sẵn
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSCuonSach.IsCuonSachAvailableAsync (ID: {id}): {ex.Message}");
                throw;
            }
        }

        // Kiểm tra cuốn sách có đang được mượn không
        public async Task<bool> IsCuonSachBorrowedAsync(int id)
        {
            if (id <= 0) return false;
            try
            {
                // Gọi DAL để kiểm tra tình trạng mượn (DAL đã thêm logic kiểm tra trong IsBorrowedAsync)
                return await _dalCuonSach.IsBorrowedAsync(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSCuonSach.IsCuonSachBorrowedAsync (ID: {id}): {ex.Message}");
                throw;
            }
        }


        // Kiểm tra mã cuốn sách tồn tại (bao gồm cả đã xóa)
        public async Task<bool> IsMaCuonSachExistsAsync(string maCuonSach)
        {
            if (string.IsNullOrWhiteSpace(maCuonSach)) return false;
            try
            {
                // Sử dụng hàm kiểm tra tồn tại từ DAL (bao gồm cả đã xóa)
                var entity = await _dalCuonSach.GetByMaIncludingDeletedAsync(maCuonSach.Trim());
                return entity != null;
                // Hoặc dùng hàm AnyAsync nếu có trong DAL
                // return await _dalCuonSach.AnyByMaAsync(maCuonSach.Trim()); // Nếu DAL có hàm AnyByMaAsync không lọc DaAn
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSCuonSach.IsMaCuonSachExistsAsync (Ma: {maCuonSach}): {ex.Message}");
                throw;
            }
        }

        // Kiểm tra mã cuốn sách tồn tại (loại trừ 1 ID)
        public async Task<bool> IsMaCuonSachExistsExcludingIdAsync(string maCuonSach, int excludeId)
        {
            if (string.IsNullOrWhiteSpace(maCuonSach)) return false;
            if (excludeId <= 0) return await IsMaCuonSachExistsAsync(maCuonSach); // Nếu excludeId không hợp lệ, kiểm tra tất cả

            try
            {
                // Sử dụng hàm kiểm tra tồn tại loại trừ ID từ DAL
                return await _dalCuonSach.IsMaCuonSachExistsExcludingIdAsync(maCuonSach.Trim(), excludeId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSCuonSach.IsMaCuonSachExistsExcludingIdAsync (Ma: {maCuonSach}, ExcludeId: {excludeId}): {ex.Message}");
                throw;
            }
        }


        // Thêm mới cuốn sách từ DTO
        public async Task<CuonSachDTO?> AddCuonSachAsync(CuonSachDTO cuonSachDto)
        {
            if (cuonSachDto == null) throw new ArgumentNullException(nameof(cuonSachDto));

            // *** BUSINESS VALIDATION ***
            // Validate IdSach
            if (cuonSachDto.IdSach <= 0)
            {
                throw new ArgumentException("Vui lòng chọn đầu sách cho cuốn sách.", nameof(cuonSachDto.IdSach));
            }
            // Kiểm tra IdSach có tồn tại trong DB không (cần IDALSach)
            var sachExists = await _dalSach.GetByIdIncludingDeletedAsync(cuonSachDto.IdSach); // Giả định IDALSach có GetByIdIncludingDeletedAsync
            if (sachExists == null)
            {
                throw new ArgumentException($"Đầu sách với ID {cuonSachDto.IdSach} không tồn tại.", nameof(cuonSachDto.IdSach));
            }


            // Kiểm tra trùng Mã Cuốn Sách (nếu Mã không tự sinh ở DAL)
            if (!string.IsNullOrWhiteSpace(cuonSachDto.MaCuonSach))
            {
                var exists = await IsMaCuonSachExistsAsync(cuonSachDto.MaCuonSach.Trim());
                if (exists)
                {
                    throw new InvalidOperationException($"Mã cuốn sách '{cuonSachDto.MaCuonSach.Trim()}' đã tồn tại.");
                }
            }

            // Validate TinhTrang ban đầu (ví dụ: chỉ cho phép thêm với TinhTrang Có sẵn = 0)
            if (cuonSachDto.TinhTrang != 0)
            {
                throw new ArgumentException("Tình trạng ban đầu của cuốn sách phải là 'Có sẵn'.", nameof(cuonSachDto.TinhTrang));
            }

            try
            {
                // Mapping DTO sang Entity (lúc này Entity chưa có ID và các collection navigation property)
                var entityToAdd = MapToCuonSachEntity(cuonSachDto);

                // Gán các giá trị mặc định hoặc do hệ thống sinh ra ở tầng nghiệp vụ (nếu cần)
                // entityToAdd.MaCuonSach = ... // Tự sinh mã ở đây nếu không nhập từ UI
                entityToAdd.DaAn = 0; // Mặc định chưa xóa mềm

                // Gọi DAL để thêm vào DB
                var addedEntity = await _dalCuonSach.AddAsync(entityToAdd);

                // Mapping Entity đã thêm (có ID mới) sang DTO và trả về
                return addedEntity != null ? MapToCuonSachDTO(addedEntity) : null;
            }
            catch (ArgumentException) { throw; } // Ném lại lỗi validation
            catch (InvalidOperationException) { throw; } // Ném lại lỗi nghiệp vụ (trùng mã)
            catch (Exception ex) // Bắt các lỗi hệ thống/DAL khác
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSCuonSach.AddCuonSachAsync: {ex.Message}");
                throw new Exception($"Lỗi khi thêm mới cuốn sách: {ex.Message}", ex);
            }
        }

        // Cập nhật cuốn sách từ DTO
        public async Task<bool> UpdateCuonSachAsync(CuonSachDTO cuonSachDto)
        {
            if (cuonSachDto == null) throw new ArgumentNullException(nameof(cuonSachDto));
            if (cuonSachDto.Id <= 0) throw new ArgumentException("Invalid CuonSach ID for update.", nameof(cuonSachDto.Id));

            // *** BUSINESS VALIDATION ***
            // Validate IdSach (ít khi thay đổi, nhưng nếu DTO cho phép sửa thì cần validate)
            if (cuonSachDto.IdSach <= 0)
            {
                throw new ArgumentException("Vui lòng chọn đầu sách cho cuốn sách.", nameof(cuonSachDto.IdSach));
            }
            // Kiểm tra IdSach có tồn tại trong DB không
            var sachExists = await _dalSach.GetByIdIncludingDeletedAsync(cuonSachDto.IdSach); // Giả định IDALSach có GetByIdIncludingDeletedAsync
            if (sachExists == null)
            {
                throw new ArgumentException($"Đầu sách với ID {cuonSachDto.IdSach} không tồn tại.", nameof(cuonSachDto.IdSach));
            }


            // Kiểm tra trùng Mã Cuốn Sách (loại trừ chính nó, nếu Mã cho sửa)
            if (!string.IsNullOrWhiteSpace(cuonSachDto.MaCuonSach))
            {
                var exists = await IsMaCuonSachExistsExcludingIdAsync(cuonSachDto.MaCuonSach.Trim(), cuonSachDto.Id);
                if (exists)
                {
                    throw new InvalidOperationException($"Mã cuốn sách '{cuonSachDto.MaCuonSach.Trim()}' đã tồn tại.");
                }
            }

            // Validate TinhTrang mới (ví dụ: không cho phép đặt TinhTrang = Đang mượn trực tiếp nếu chưa tạo phiếu mượn)
            if (cuonSachDto.TinhTrang < 0) // Giả định TinhTrang không âm
            {
                throw new ArgumentException("Tình trạng cuốn sách không hợp lệ.", nameof(cuonSachDto.TinhTrang));
            }

            try
            {
                // Cần lấy entity gốc từ DAL để giữ lại các field không có trong DTO hoặc không được phép sửa
                var currentEntity = await _dalCuonSach.GetByIdAsync(cuonSachDto.Id); // GetByIdAsync đã lọc DaAn=0
                if (currentEntity == null) throw new InvalidOperationException($"Không tìm thấy cuốn sách với ID {cuonSachDto.Id} để cập nhật.");

                // Mapping DTO sang currentEntity cho các thuộc tính được phép sửa
                currentEntity.IdSach = cuonSachDto.IdSach;
                currentEntity.MaCuonSach = cuonSachDto.MaCuonSach?.Trim(); // Cập nhật mã nếu cho sửa
                currentEntity.TinhTrang = cuonSachDto.TinhTrang;
                // currentEntity.DaAn = cuonSachDto.DaAn; // Xóa mềm dùng hàm riêng


                // Gọi DAL để cập nhật
                bool success = await _dalCuonSach.UpdateAsync(currentEntity); // Truyền entity đã được cập nhật

                return success;
            }
            catch (ArgumentException) { throw; } // Ném lại lỗi validation
            catch (InvalidOperationException) { throw; } // Ném lại lỗi nghiệp vụ (trùng mã, không tìm thấy entity)
            catch (Exception ex) // Bắt các lỗi hệ thống/DAL khác
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSCuonSach.UpdateCuonSachAsync (ID: {cuonSachDto.Id}): {ex.Message}");
                throw new Exception($"Lỗi khi cập nhật cuốn sách: {ex.Message}", ex);
            }
        }

        // Cập nhật tình trạng cuốn sách cụ thể (Business Rule)
        public async Task<bool> UpdateTinhTrangAsync(int cuonSachId, int newTinhTrang)
        {
            if (cuonSachId <= 0) throw new ArgumentException("Invalid CuonSach ID.", nameof(cuonSachId));
            if (newTinhTrang < 0) throw new ArgumentException("Tình trạng mới không hợp lệ.", nameof(newTinhTrang));

            // *** BUSINESS VALIDATION ***
            // Có thể cần kiểm tra TinhTrang cũ có cho phép chuyển sang newTinhTrang không (StateMachine)
            // Ví dụ: Không thể chuyển từ 'Mất' sang 'Có sẵn' trực tiếp.
            // var currentCuonSach = await _dalCuonSach.GetByIdAsync(cuonSachId);
            // if (currentCuonSach == null) throw new InvalidOperationException($"Không tìm thấy cuốn sách với ID {cuonSachId}.");
            // if (!CanChangeStatus(currentCuonSach.TinhTrang, newTinhTrang)) { ... throw InvalidOperationException ... }

            // Logic cụ thể cho từng trạng thái (ví dụ: khi chuyển sang Đang mượn, cần kiểm tra số lượng mượn tối đa)
            // if (newTinhTrang == 1) // Đang mượn
            // {
            //      // Cần kiểm tra xem độc giả có được mượn thêm sách không (cần IDALDocGia, IDALThamSo)
            // }


            try
            {
                // Lấy entity để cập nhật tình trạng
                var cuonSachToUpdate = await _dalCuonSach.GetByIdAsync(cuonSachId); // GetByIdAsync đã lọc DaAn=0
                if (cuonSachToUpdate == null) throw new InvalidOperationException($"Không tìm thấy cuốn sách với ID {cuonSachId} để cập nhật tình trạng.");

                // Cập nhật chỉ tình trạng
                cuonSachToUpdate.TinhTrang = newTinhTrang;

                // Gọi DAL để cập nhật
                bool success = await _dalCuonSach.UpdateAsync(cuonSachToUpdate); // Sử dụng hàm Update chung hoặc hàm UpdateStatus riêng nếu có

                return success;
            }
            catch (ArgumentException) { throw; }
            catch (InvalidOperationException) { throw; } // Ném lại lỗi nghiệp vụ
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSCuonSach.UpdateTinhTrangAsync (ID: {cuonSachId}, Status: {newTinhTrang}): {ex.Message}");
                throw new Exception($"Lỗi khi cập nhật tình trạng cuốn sách: {ex.Message}", ex);
            }
        }

        // Xóa mềm cuốn sách theo ID
        public async Task<bool> SoftDeleteCuonSachAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid CuonSach ID for soft delete.", nameof(id));

            // *** BUSINESS VALIDATION: Kiểm tra ràng buộc trước khi xóa mềm ***
            // Cần kiểm tra xem cuốn sách có đang được mượn không.
            // Logic kiểm tra này đã được thêm vào DAL.SoftDeleteAsync và ném InvalidOperationException.
            // BUS chỉ cần gọi DAL và bắt Exception đó.
            // Hoặc có thể gọi CanSoftDeleteCuonSachAsync ở đây:
            // if (!await CanSoftDeleteCuonSachAsync(id))
            // {
            //      throw new InvalidOperationException($"Không thể xóa mềm cuốn sách ID {id} vì đang được mượn.");
            // }

            try
            {
                // Gọi DAL để xóa mềm
                bool success = await _dalCuonSach.SoftDeleteAsync(id);
                return success;
            }
            catch (InvalidOperationException ex) { throw; } // Ném lại lỗi nghiệp vụ từ DAL
            catch (Exception ex) // Dòng 453
            {
                System.Diagnostics.Debug.WriteLine($"Some error message: {ex.ToString()}"); // Sử dụng ex.ToString()
                throw;
            }
        }

        // Phục hồi cuốn sách đã xóa mềm theo ID
        public async Task<bool> RestoreCuonSachAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid CuonSach ID for restore.", nameof(id));
            try
            {
                // Gọi DAL để phục hồi
                return await _dalCuonSach.RestoreAsync(id);
            }
            catch (Exception ex) // Bắt các lỗi hệ thống/DAL
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSCuonSach.RestoreCuonSachAsync (ID: {id}): {ex.Message}");
                throw new Exception($"Lỗi khi phục hồi cuốn sách (ID: {id}): {ex.Message}", ex); // Ném lại lỗi hệ thống
            }
        }

        // Xóa vĩnh viễn cuốn sách theo ID (Hard Delete)
        public async Task<bool> HardDeleteCuonSachAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid CuonSach ID for hard delete.", nameof(id));

            // *** BUSINESS VALIDATION: Kiểm tra ràng buộc trước khi xóa vĩnh viễn ***
            // Cần đảm bảo không có bất kỳ record liên quan nào còn tồn tại trong DB (Phieumuontra, Bcsachtratre)
            // Logic kiểm tra này đã được thêm vào DAL.HardDeleteAsync và ném InvalidOperationException.
            // BUS chỉ cần gọi DAL và bắt Exception đó.
            // Hoặc có thể gọi CanHardDeleteCuonSachAsync ở đây:
            // if (!await CanHardDeleteCuonSachAsync(id))
            // {
            //      // Cần thông báo chi tiết hơn lý do không thể xóa
            //      throw new InvalidOperationException($"Không thể xóa vĩnh viễn cuốn sách ID {id} vì còn dữ liệu liên quan.");
            // }

            try
            {
                // Gọi DAL để xóa vĩnh viễn
                bool success = await _dalCuonSach.HardDeleteAsync(id);
                return success;
            }
            catch (InvalidOperationException ex) { throw; } // Ném lại lỗi nghiệp vụ/ràng buộc từ DAL (đã ném từ DAL)
            catch (Exception ex) // Bắt các lỗi hệ thống/DAL khác
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSCuonSach.HardDeleteCuonSachAsync (ID: {id}): {ex.Message}");
                throw new Exception($"Lỗi khi xóa vĩnh viễn cuốn sách (ID: {id}): {ex.Message}", ex); // Ném lại lỗi hệ thống
            }
        }

        // --- NEW METHODS REQUIRED BY IBUSCuonSach INTERFACE ---

        /// <summary>
        /// Kiểm tra xem cuốn sách có thể xóa mềm được không (ví dụ: không đang được mượn).
        /// Phương thức này dùng để kiểm tra điều kiện trước khi gọi SoftDeleteCuonSachAsync.
        /// </summary>
        /// <param name="id">ID của cuốn sách.</param>
        /// <returns>Task<bool> True nếu có thể xóa mềm, False nếu ngược lại.</returns>
        public async Task<bool> CanSoftDeleteCuonSachAsync(int id)
        {
            if (id <= 0) return false; // ID không hợp lệ thì không thể xóa

            try
            {
                // Logic nghiệp vụ: Không thể xóa mềm nếu cuốn sách đang được mượn.
                // Sử dụng phương thức đã có để kiểm tra tình trạng mượn.
                bool isBorrowed = await IsCuonSachBorrowedAsync(id);
                return !isBorrowed; // Trả về true nếu KHÔNG đang được mượn
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking if CuonSach ID {id} can be soft deleted: {ex.Message}");
                // Nếu có lỗi khi kiểm tra, coi như không thể xóa để an toàn.
                // Hoặc ném exception nếu lỗi nghiêm trọng.
                throw new Exception($"Lỗi khi kiểm tra khả năng xóa mềm cuốn sách (ID: {id}): {ex.Message}", ex);
                // return false; // Alternative: return false silently on error
            }
        }

        /// <summary>
        /// Kiểm tra xem cuốn sách có thể xóa vĩnh viễn được không (ví dụ: không có bất kỳ phiếu mượn/trả liên quan nào).
        /// Phương thức này dùng để kiểm tra điều kiện trước khi gọi HardDeleteCuonSachAsync.
        /// </summary>
        /// <param name="id">ID của cuốn sách.</param>
        /// <returns>Task<bool> True nếu có thể xóa vĩnh viễn, False nếu ngược lại.</returns>
        public async Task<bool> CanHardDeleteCuonSachAsync(int id)
        {
            if (id <= 0) return false; // ID không hợp lệ thì không thể xóa

            try
            {
                // Logic nghiệp vụ: Không thể xóa vĩnh viễn nếu cuốn sách còn liên quan đến
                // bất kỳ record nào trong DB (phiếu mượn, báo cáo sách trễ, v.v.).
                // Cần một phương thức DAL để kiểm tra các ràng buộc này.
                // Giả sử IDALCuonSach có phương thức HasAnyRelatedReferencesAsync(int cuonSachId)
                // Nếu không có, bạn sẽ cần thêm phương thức này vào IDALCuonSach và triển khai trong DALCuonSach.
                // Hoặc kiểm tra thủ công bằng cách gọi các DAL khác (phiếu mượn, báo cáo...).
                bool hasRelated = await _dalCuonSach.HasAnyRelatedReferencesAsync(id); // <<< CHÚ Ý: Phương thức này cần được thêm vào IDALCuonSach và DALCuonSach
                return !hasRelated; // Trả về true nếu KHÔNG có record liên quan nào

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking if CuonSach ID {id} can be hard deleted: {ex.Message}");
                // Nếu có lỗi khi kiểm tra, coi như không thể xóa để an toàn.
                // Hoặc ném exception nếu lỗi nghiêm trọng.
                throw new Exception($"Lỗi khi kiểm tra khả năng xóa vĩnh viễn cuốn sách (ID: {id}): {ex.Message}", ex);
                // return false; // Alternative: return false silently on error
            }
        }


        // Có thể thêm các phương thức nghiệp vụ khác
        // Ví dụ: Hàm kiểm tra xem có thể chuyển từ trạng thái X sang trạng thái Y không (StateMachine)
        // private bool CanChangeStatus(int currentStatus, int newStatus) { ... }
    } // Kết thúc class BUSCuonSach

} // Kết thúc namespace BUS