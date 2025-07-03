// Project/Namespace: BUS

using BUS; // Cần cho interface IBUSThamSo mà class này implement
using DAL; // Cần cho interface IDALThamSo mà class này phụ thuộc
using DAL.Models; // Cần cho Entity Thamso để mapping
using DTO; // Cần cho ThamSoDTO để mapping
using System; // Cần cho Exception, ArgumentNullException, InvalidOperationException
using System.Collections.Generic; // Cần cho List
using System.Linq; // Cần cho LINQ (Select, ToList)
using System.Threading.Tasks; // Cần cho async/await Task


namespace BUS // Namespace của Business Logic Layer
{
    /// <summary>
    /// Business Logic Layer triển khai IBUSThamSo, xử lý nghiệp vụ cho Tham số hệ thống.
    /// Phụ thuộc vào IDALThamSo. Thực hiện mapping DTO/Entity và validation nghiệp vụ.
    /// </summary>
    public class BUSThamso : IBUSThamSo // <<< Implement interface IBUSThamSo
    {
        // --- DEPENDENCIES ---
        // BUS phụ thuộc vào DAL thông qua interface IDALThamso
        private readonly IDALThamSo _dalThamso;

        // --- CONSTRUCTOR (Sử dụng Dependency Injection) ---
        // Constructor nhận các DAL dependencies qua interface
        public BUSThamso(IDALThamSo dalThamso) // Nhận IDAL qua DI
        {
            _dalThamso = dalThamso ?? throw new ArgumentNullException(nameof(dalThamso));
        }

        // --- PRIVATE HELPER METHODS: MAPPING BETWEEN ENTITY AND DTO ---

        /// <summary>
        /// Ánh xạ từ Entity Thamso sang DTO ThamSoDTO.
        /// </summary>
        /// <param name="entity">Đối tượng Entity Thamso.</param>
        /// <returns>Đối tượng ThamSoDTO tương ứng hoặc null nếu entity là null.</returns>
        private ThamSoDTO MapToThamSoDTO(Thamso entity)
        {
            // Kiểm tra null trước khi mapping
            if (entity == null) return null!; // Sử dụng null-forgiving operator nếu bạn chắc chắn không trả về null
            // Hoặc trả về null: if (entity == null) return null;

            return new ThamSoDTO
            {
                Id = entity.Id,
                TuoiToiThieu = entity.TuoiToiThieu,
                TuoiToiDa = entity.TuoiToiDa,
                ThoiHanThe = entity.ThoiHanThe,
                KhoangCachXuatBan = entity.KhoangCachXuatBan,
                SoSachMuonToiDa = entity.SoSachMuonToiDa,
                SoNgayMuonToiDa = entity.SoNgayMuonToiDa,
                DonGiaPhat = entity.DonGiaPhat,
                AdQdkttienThu = entity.AdQdkttienThu,
                // Các thuộc tính khác nếu có trong DTO nhưng không có trong Entity sẽ giữ giá trị mặc định
            };
        }

        /// <summary>
        /// Ánh xạ từ DTO ThamSoDTO sang Entity Thamso (cho mục đích thêm/cập nhật).
        /// </summary>
        /// <param name="dto">Đối tượng ThamSoDTO.</param>
        /// <returns>Đối tượng Entity Thamso tương ứng.</returns>
        /// <exception cref="ArgumentNullException">Ném ra nếu dto là null.</exception>
        private Thamso MapToThamSoEntity(ThamSoDTO dto)
        {
            // Kiểm tra null DTO
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new Thamso
            {
                Id = dto.Id, // ID cần thiết cho việc cập nhật hoặc nếu ID được truyền từ UI khi thêm
                TuoiToiThieu = dto.TuoiToiThieu,
                TuoiToiDa = dto.TuoiToiDa,
                ThoiHanThe = dto.ThoiHanThe,
                KhoangCachXuatBan = dto.KhoangCachXuatBan,
                SoSachMuonToiDa = dto.SoSachMuonToiDa,
                SoNgayMuonToiDa = dto.SoNgayMuonToiDa,
                DonGiaPhat = dto.DonGiaPhat,
                AdQdkttienThu = dto.AdQdkttienThu,
                // Không gán các navigation properties (ICollection) ở đây khi mapping DTO -> Entity
            };
        }


        // --- METHOD IMPLEMENTATIONS (Triển khai các phương thức từ IBUSThamSo) ---

        /// <summary>
        /// Lấy bộ tham số hệ thống hiện tại dưới dạng DTO.
        /// Gọi DAL để lấy Entity, sau đó mapping sang DTO.
        /// </summary>
        /// <returns>Task chứa ThamSoDTO hoặc null nếu không tìm thấy bộ tham số nào.</returns>
        public async Task<ThamSoDTO?> GetThamSoAsync()
        {
            try
            {
                // Gọi đúng phương thức GetThamSoAsync từ DAL
                var entity = await _dalThamso.GetThamSoAsync(); // <<< Đã sửa thành GetThamSoAsync()
                return MapToThamSoDTO(entity);
            }
            catch (Exception ex) // Bắt lỗi từ tầng DAL hoặc lỗi hệ thống khác
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSThamso.GetThamSoAsync: {ex.Message}");
                throw; // Ném lại lỗi cho tầng GUI xử lý/hiển thị
            }
        }

        /// <summary>
        /// Thêm mới một bộ tham số.
        /// Thường chỉ gọi khi khởi tạo database lần đầu hoặc nếu có nhiều bộ tham số.
        /// </summary>
        /// <param name="thamSoDto">Đối tượng ThamSoDTO chứa thông tin cần thêm.</param>
        /// <returns>Task chứa ThamSoDTO đã được thêm (có ID mới) hoặc null nếu thêm thất bại.</returns>
        /// <exception cref="ArgumentException">Ném ra nếu dữ liệu tham số không hợp lệ.</exception>
        /// <exception cref="InvalidOperationException">Ném ra nếu vi phạm quy tắc nghiệp vụ.</exception>
        public async Task<ThamSoDTO?> AddThamSoAsync(ThamSoDTO thamSoDto)
        {
            // Kiểm tra DTO đầu vào
            if (thamSoDto == null) throw new ArgumentNullException(nameof(thamSoDto));

            // *** BUSINESS VALIDATION (Validation nghiệp vụ) ***
            if (thamSoDto.TuoiToiThieu < 0 || thamSoDto.TuoiToiDa < 0 || thamSoDto.ThoiHanThe <= 0 ||
                thamSoDto.KhoangCachXuatBan < 0 || thamSoDto.SoSachMuonToiDa <= 0 || thamSoDto.SoNgayMuonToiDa <= 0 ||
                thamSoDto.DonGiaPhat < 0)
            {
                throw new ArgumentException("Các giá trị tham số không hợp lệ (âm hoặc bằng không).");
            }
            if (thamSoDto.TuoiToiThieu > thamSoDto.TuoiToiDa)
            {
                throw new ArgumentException("Tuổi tối thiểu không được lớn hơn tuổi tối đa.");
            }
            if (thamSoDto.AdQdkttienThu != 0 && thamSoDto.AdQdkttienThu != 1)
            {
                throw new ArgumentException("Tham số Áp dụng quy định tiền thu chỉ nhận giá trị 0 hoặc 1.");
            }

            // Kiểm tra tồn tại bằng phương thức đúng
            var existing = await _dalThamso.GetThamSoAsync(); // <<< Đã sửa thành GetThamSoAsync()
            if (existing != null)
            {
                throw new InvalidOperationException("Đã tồn tại bộ tham số trong hệ thống. Không thể thêm mới.");
            }
            // *** HẾT BUSINESS VALIDATION ***

            try
            {
                // Mapping DTO sang Entity
                var entityToAdd = MapToThamSoEntity(thamSoDto);

                // Gọi DAL để thêm vào DB
                // !!! YÊU CẦU: IDALThamSo và lớp triển khai phải có phương thức AddAsync(Thamso thamso) !!!
                var addedEntity = await _dalThamso.AddAsync(entityToAdd);

                // Mapping Entity đã thêm (có ID mới nếu tự sinh) sang DTO và trả về
                return MapToThamSoDTO(addedEntity);
            }
            catch (ArgumentException) { throw; } // Ném lại lỗi validation
            catch (InvalidOperationException) { throw; } // Ném lại lỗi nghiệp vụ
            catch (Exception ex) // Bắt các lỗi khác từ DAL
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSThamso.AddThamSoAsync: {ex.Message}");
                throw new Exception($"Lỗi khi thêm mới tham số: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Cập nhật bộ tham số hệ thống.
        /// Thực hiện validation nghiệp vụ, sau đó gọi tầng DAL để lưu.
        /// </summary>
        /// <param name="thamSoDto">Đối tượng ThamSoDTO chứa thông tin tham số cần cập nhật.</param>
        /// <returns>Task chứa bool, true nếu cập nhật thành công, false nếu không tìm thấy hoặc cập nhật thất bại.</returns>
        /// <exception cref="ArgumentException">Ném ra nếu dữ liệu tham số không hợp lệ (ví dụ: giá trị âm, tuổi min > tuổi max).</exception>
        /// <exception cref="InvalidOperationException">Ném ra nếu không tìm thấy bộ tham số để cập nhật hoặc vi phạm quy tắc nghiệp vụ khác.</exception>
        public async Task<bool> UpdateThamSoAsync(ThamSoDTO thamSoDto)
        {
            // Kiểm tra DTO đầu vào
            if (thamSoDto == null) throw new ArgumentNullException(nameof(thamSoDto));

            // Kiểm tra ID tham số hợp lệ (thường là ID của bộ tham số duy nhất, ví dụ: ID=1)
            if (thamSoDto.Id <= 0) // Hoặc if (thamSoDto.Id != 1) nếu bạn chắc chắn ID luôn là 1
            {
                throw new ArgumentException("ID tham số không hợp lệ để cập nhật.", nameof(thamSoDto.Id));
            }


            // *** BUSINESS VALIDATION (Validation nghiệp vụ) ***
            // 1. Kiểm tra các giá trị không âm (nếu logic nghiệp vụ yêu cầu)
            if (thamSoDto.TuoiToiThieu < 0 || thamSoDto.TuoiToiDa < 0 || thamSoDto.ThoiHanThe <= 0 ||
                thamSoDto.KhoangCachXuatBan < 0 || thamSoDto.SoSachMuonToiDa <= 0 || thamSoDto.SoNgayMuonToiDa <= 0 ||
                thamSoDto.DonGiaPhat < 0)
            {
                throw new ArgumentException("Các giá trị tham số không được âm. Thời hạn thẻ, số sách, số ngày mượn tối đa phải dương.");
            }

            // 2. Kiểm tra ràng buộc giữa các tham số
            if (thamSoDto.TuoiToiThieu > thamSoDto.TuoiToiDa)
            {
                throw new ArgumentException("Tuổi tối thiểu không được lớn hơn tuổi tối đa.");
            }

            // 3. Kiểm tra cờ AdQdkttienThu chỉ có giá trị 0 hoặc 1
            if (thamSoDto.AdQdkttienThu != 0 && thamSoDto.AdQdkttienThu != 1)
            {
                throw new ArgumentException("Tham số Áp dụng quy định tiền thu chỉ nhận giá trị 0 hoặc 1.");
            }
            // *** HẾT BUSINESS VALIDATION ***

            try
            {
                // Mapping DTO sang Entity để truyền xuống DAL
                var entityToUpdate = MapToThamSoEntity(thamSoDto);

                // Gọi DAL để cập nhật vào DB
                bool success = await _dalThamso.UpdateAsync(entityToUpdate);

                // Nếu DAL báo không tìm thấy bản ghi (UpdateAsync trả về false)
                if (!success)
                {
                    // Ném lỗi nghiệp vụ rõ ràng
                    throw new InvalidOperationException($"Không tìm thấy bộ tham số với ID {thamSoDto.Id} để cập nhật.");
                }

                return success; // Trả về true nếu cập nhật thành công ở DAL
            }
            catch (ArgumentException) { throw; } // Ném lại lỗi validation
            catch (InvalidOperationException) { throw; } // Ném lại lỗi nghiệp vụ
            catch (Exception ex) // Bắt các lỗi khác từ DAL
            {
                System.Diagnostics.Debug.WriteLine($"Error in BUSThamso.UpdateThamSoAsync (ID: {thamSoDto.Id}): {ex.Message}");
                // Wrap lại exception
                throw new Exception($"Lỗi khi cập nhật tham số hệ thống (ID: {thamSoDto.Id}): {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Xóa vĩnh viễn bộ tham số.
        /// Thường không được phép từ GUI cho bộ tham số hệ thống.
        /// </summary>
        /// <param name="id">ID của bộ tham số cần xóa.</param>
        /// <returns>Task chứa bool.</returns>
        /// <exception cref="InvalidOperationException">Luôn ném lỗi vì không cho phép xóa tham số hệ thống.</exception>
        public async Task<bool> DeleteThamSoAsync(int id)
        {
            // Logic nghiệp vụ: Thường không cho phép xóa bộ tham số hệ thống.
            if (id == 1) // Nếu ID=1 là bộ tham số mặc định không được xóa
            {
                throw new InvalidOperationException("Không được xóa bộ tham số mặc định của hệ thống.");
            }

            // Dựa trên IDALThamSo không có DeleteAsync, ném lỗi
            throw new InvalidOperationException($"Không hỗ trợ xóa tham số hệ thống với ID {id} từ giao diện.");
            // Hoặc trả về false nếu muốn im lặng: return false;
        }

        /// <summary>
        /// Kiểm tra xem một bộ tham số có thể xóa vĩnh viễn được không.
        /// Dựa trên logic DeleteThamSoAsync, phương thức này sẽ luôn trả về false nếu là bộ tham số mặc định.
        /// </summary>
        /// <param name="id">ID của bộ tham số cần kiểm tra.</param>
        /// <returns>Task chứa bool, true nếu có thể xóa, false nếu ngược lại.</returns>
        public async Task<bool> CanDeleteThamSoAsync(int id)
        {
            // Logic nghiệp vụ: Không cho xóa bộ tham số mặc định (ID=1)
            if (id == 1) return false;

            // Dựa trên IDALThamSo không có Delete, coi như không cho xóa từ GUI
            return false; // Luôn trả về false nếu không có logic xóa ở DAL/BUS
        }

        // Các phương thức khác từ IBUSThamSo (nếu có) sẽ được triển khai tại đây.
    }
}