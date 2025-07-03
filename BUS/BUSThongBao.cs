// BUS/BUSThongBao.cs
using DAL;
using DAL.Models; // <<< THÊM: Using cho ThongBao Entity
using DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics; // <<< THÊM: Using cho Debug
using System.Linq;
using System.Threading.Tasks;

namespace BUS
{
    public class BUSThongBao : IBUSThongBao // Đảm bảo implement IBUSThongBao đầy đủ
    {
        private readonly IDALThongBao _dalThongBao;
        // Có thể inject thêm các IDAL khác nếu cần logic phức tạp hơn

        public BUSThongBao(IDALThongBao dalThongBao)
        {
            _dalThongBao = dalThongBao ?? throw new ArgumentNullException(nameof(dalThongBao));
        }

        // --- Phương thức hiện có ---
        public async Task<List<ThongBaoDTO>> GetActiveNotificationsAsync()
        {
            try
            {
                var thongBaos = await _dalThongBao.GetAllActiveAsync();
                // Map từ Entity sang DTO
                return MapListEntityToListDto(thongBaos);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BUSThongBao.GetActiveNotificationsAsync] Error: {ex.Message}");
                return new List<ThongBaoDTO>(); // Trả về rỗng nếu lỗi
            }
        }

        // --- Triển khai các phương thức quản lý mới ---

        public async Task<List<ThongBaoDTO>> GetAllNotificationsAsync()
        {
            try
            {
                var thongBaos = await _dalThongBao.GetAllAsync();
                return MapListEntityToListDto(thongBaos);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BUSThongBao.GetAllNotificationsAsync] Error: {ex.Message}");
                return new List<ThongBaoDTO>();
            }
        }

        public async Task<ThongBaoDTO?> GetNotificationByIdAsync(int id)
        {
            if (id <= 0) return null;
            try
            {
                var thongBao = await _dalThongBao.GetByIdAsync(id);
                return MapEntityToDto(thongBao); // Trả về null nếu không tìm thấy
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BUSThongBao.GetNotificationByIdAsync] Error for Id {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool Success, string ErrorMessage)> AddNotificationAsync(ThongBaoDTO thongBaoDto)
        {
            if (thongBaoDto == null)
            {
                return (false, "Dữ liệu thông báo không hợp lệ.");
            }

            // --- Validation cơ bản ---
            var validationResult = ValidateThongBaoDto(thongBaoDto);
            if (!validationResult.IsValid)
            {
                return (false, validationResult.ErrorMessage);
            }
            // --- Kết thúc Validation ---

            try
            {
                var thongBaoEntity = MapDtoToEntity(thongBaoDto);
                // Đảm bảo NgayTao được set bởi DAL hoặc DB, không cần gán ở đây trừ khi có logic đặc biệt
                // thongBaoEntity.NgayTao = DateTime.Now;

                bool success = await _dalThongBao.AddAsync(thongBaoEntity);

                if (success)
                {
                    return (true, "Thêm thông báo thành công.");
                }
                else
                {
                    return (false, "Không thể lưu thông báo vào cơ sở dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BUSThongBao.AddNotificationAsync] Error: {ex.Message}");
                return (false, $"Đã xảy ra lỗi hệ thống khi thêm: {ex.Message}");
            }
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateNotificationAsync(ThongBaoDTO thongBaoDto)
        {
            if (thongBaoDto == null || thongBaoDto.Id <= 0)
            {
                return (false, "Dữ liệu thông báo hoặc ID không hợp lệ để cập nhật.");
            }

            // --- Validation cơ bản ---
            var validationResult = ValidateThongBaoDto(thongBaoDto);
            if (!validationResult.IsValid)
            {
                return (false, validationResult.ErrorMessage);
            }
            // --- Kết thúc Validation ---

            try
            {
                // Kiểm tra xem thông báo có tồn tại không trước khi map và update
                var existingEntity = await _dalThongBao.GetByIdAsync(thongBaoDto.Id);
                if (existingEntity == null)
                {
                    return (false, $"Không tìm thấy thông báo với ID {thongBaoDto.Id} để cập nhật.");
                }

                // Map DTO sang Entity đã tồn tại (hoặc tạo entity mới và gọi DAL.Update)
                var thongBaoEntity = MapDtoToEntity(thongBaoDto, existingEntity); // Truyền entity cũ để cập nhật

                bool success = await _dalThongBao.UpdateAsync(thongBaoEntity);

                if (success)
                {
                    return (true, "Cập nhật thông báo thành công.");
                }
                else
                {
                    return (false, "Không thể cập nhật thông báo trong cơ sở dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BUSThongBao.UpdateNotificationAsync] Error for Id {thongBaoDto.Id}: {ex.Message}");
                return (false, $"Đã xảy ra lỗi hệ thống khi cập nhật: {ex.Message}");
            }
        }

        public async Task<(bool Success, string ErrorMessage)> DeleteNotificationAsync(int id)
        {
            if (id <= 0)
            {
                return (false, "ID thông báo không hợp lệ.");
            }
            try
            {
                // Optional: Kiểm tra xem thông báo có tồn tại không trước khi xóa
                var existing = await _dalThongBao.GetByIdAsync(id);
                if (existing == null)
                {
                    return (false, $"Không tìm thấy thông báo với ID {id} để xóa.");
                }

                bool success = await _dalThongBao.DeleteAsync(id);
                if (success)
                {
                    return (true, "Xóa thông báo thành công.");
                }
                else
                {
                    return (false, "Không thể xóa thông báo khỏi cơ sở dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BUSThongBao.DeleteNotificationAsync] Error for Id {id}: {ex.Message}");
                // Kiểm tra InnerException nếu là lỗi liên quan đến khóa ngoại
                string detailError = ex.InnerException?.Message ?? ex.Message;
                return (false, $"Đã xảy ra lỗi hệ thống khi xóa: {detailError}");
            }
        }


        // *** START: THÊM CHO HOẠT ĐỘNG GẦN ĐÂY ***

        /// <summary>
        /// Lấy một số lượng giới hạn các thông báo gần đây nhất cho Trang Chủ.
        /// </summary>
        /// <param name="count">Số lượng thông báo cần lấy.</param>
        /// <returns>Danh sách các ThongBaoDTO gần đây.</returns>
        public async Task<List<ThongBaoDTO>> GetRecentNotificationsAsync(int count)
        {
            if (count <= 0) return new List<ThongBaoDTO>();
            try
            {
                var thongBaos = await _dalThongBao.GetRecentAsync(count);
                // Map từ Entity sang DTO
                return MapListEntityToListDto(thongBaos);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BUSThongBao.GetRecentNotificationsAsync] Error getting recent {count}: {ex.Message}");
                return new List<ThongBaoDTO>(); // Trả về rỗng nếu lỗi
            }
        }

        /// <summary>
        /// Tạo một thông báo mới cho mục đích ghi log hoạt động.
        /// Phương thức này đơn giản hóa việc tạo thông báo chỉ với nội dung.
        /// </summary>
        /// <param name="noiDung">Nội dung của hoạt động.</param>
        /// <param name="loaiThongBao">Phân loại thông báo (ví dụ: "Hoạt động", "Hệ thống").</param>
        /// <returns>True nếu tạo thành công, False nếu thất bại.</returns>
        public async Task<bool> CreateActivityLogAsync(string noiDung, string loaiThongBao = "Hoạt động")
        {
            if (string.IsNullOrWhiteSpace(noiDung))
            {
                Debug.WriteLine("[BUSThongBao.CreateActivityLogAsync] Error: Nội dung không được để trống.");
                return false;
            }

            var activityLog = new ThongBao
            {
                TieuDe = loaiThongBao, // Sử dụng loại thông báo làm tiêu đề đơn giản
                NoiDung = noiDung.Trim(),
                NgayBatDau = DateTime.Now.Date, // Hoạt động xảy ra ngay bây giờ
                NgayKetThuc = null, // Không có ngày kết thúc cho log
                MucDo = "Thông tin", // Mặc định là thông tin
                TrangThai = "Internal", // Đánh dấu là log nội bộ, không hiển thị qua GetAllActiveAsync thông thường
                NgayTao = DateTime.Now // DAL sẽ ghi đè nhưng để đây cho rõ
                // Id sẽ tự tạo
            };

            try
            {
                bool success = await _dalThongBao.AddAsync(activityLog);
                if (!success)
                {
                    Debug.WriteLine("[BUSThongBao.CreateActivityLogAsync] Error: DAL.AddAsync failed.");
                }
                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BUSThongBao.CreateActivityLogAsync] Error: {ex.Message}");
                return false;
            }
        }
        // *** END: THÊM CHO HOẠT ĐỘNG GẦN ĐÂY ***

        // --- Private Helper Methods ---

        private (bool IsValid, string ErrorMessage) ValidateThongBaoDto(ThongBaoDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TieuDe))
                return (false, "Tiêu đề không được để trống.");
            if (dto.TieuDe.Length > 200) // Giả sử giới hạn 200 ký tự
                return (false, "Tiêu đề không được vượt quá 200 ký tự.");
            if (string.IsNullOrWhiteSpace(dto.NoiDung))
                return (false, "Nội dung không được để trống.");
            if (dto.NgayKetThuc.HasValue && dto.NgayKetThuc.Value.Date < dto.NgayBatDau.Date) // Chỉ so sánh ngày nếu có ngày KT
                return (false, "Ngày kết thúc không được trước ngày bắt đầu.");
            if (string.IsNullOrWhiteSpace(dto.MucDo))
                return (false, "Vui lòng chọn mức độ.");
            if (string.IsNullOrWhiteSpace(dto.TrangThai))
                return (false, "Vui lòng chọn trạng thái.");

            // Thêm các quy tắc validation khác nếu cần

            return (true, ""); // Hợp lệ
        }

        // Hàm map từ Entity sang DTO
        private ThongBaoDTO? MapEntityToDto(ThongBao? entity)
        {
            if (entity == null) return null;
            return new ThongBaoDTO
            {
                Id = entity.Id,
                TieuDe = entity.TieuDe,
                NoiDung = entity.NoiDung,
                NgayBatDau = entity.NgayBatDau,
                NgayKetThuc = entity.NgayKetThuc,
                MucDo = entity.MucDo,
                TrangThai = entity.TrangThai,
                NgayTao = entity.NgayTao
            };
        }

        // Hàm map từ List<Entity> sang List<DTO>
        private List<ThongBaoDTO> MapListEntityToListDto(List<ThongBao> entities)
        {
            return entities.Select(e => MapEntityToDto(e)!) // Dùng ! để báo hiệu không null ở đây vì Select sẽ bỏ qua null
                           .Where(dto => dto != null) // Lọc lại những DTO không null (dù MapEntityToDto đã kiểm tra)
                           .ToList();
        }

        // Hàm map từ DTO sang Entity (dùng cho Add)
        private ThongBao MapDtoToEntity(ThongBaoDTO dto)
        {
            return new ThongBao
            {
                // Không map Id vì đây là thêm mới, DB sẽ tự tạo
                TieuDe = dto.TieuDe.Trim(), // Trim() để loại bỏ khoảng trắng thừa
                NoiDung = dto.NoiDung.Trim(),
                NgayBatDau = dto.NgayBatDau,
                NgayKetThuc = dto.NgayKetThuc,
                MucDo = dto.MucDo,
                TrangThai = dto.TrangThai
                // NgayTao sẽ được DAL hoặc DB xử lý
            };
        }

        // Hàm map từ DTO sang Entity đã tồn tại (dùng cho Update)
        private ThongBao MapDtoToEntity(ThongBaoDTO dto, ThongBao existingEntity)
        {
            // Chỉ cập nhật các trường cần thiết từ DTO vào entity đã tồn tại
            existingEntity.TieuDe = dto.TieuDe.Trim();
            existingEntity.NoiDung = dto.NoiDung.Trim();
            existingEntity.NgayBatDau = dto.NgayBatDau;
            existingEntity.NgayKetThuc = dto.NgayKetThuc;
            existingEntity.MucDo = dto.MucDo;
            existingEntity.TrangThai = dto.TrangThai;
            // Không cập nhật Id và NgayTao
            return existingEntity;
        }

    }
}