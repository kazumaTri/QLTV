// BUS/BUSChucNang.cs
using DAL; // Cần để sử dụng IDALChucNang
using DAL.Models; // Cần để sử dụng Entity 'Chucnang'
using DTO; // Cần để sử dụng ChucNangDTO
using System;
using System.Collections.Generic;
using System.Diagnostics; // Cho Debug.WriteLine
using System.Linq; // Cần cho Select (LINQ)
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    /// <summary>
    /// Business Logic Layer class for ChucNang (Function/Permission).
    /// Implements the IBUSChucNang interface to provide business logic methods.
    /// </summary>
    public class BUSChucNang : IBUSChucNang // Implement interface đã tạo
    {
        private readonly IDALChucNang _dalChucNang; // Biến để giữ DAL instance

        /// <summary>
        /// Constructor that accepts the DAL interface via Dependency Injection.
        /// </summary>
        /// <param name="dalChucNang">The data access layer instance for ChucNang.</param>
        public BUSChucNang(IDALChucNang dalChucNang)
        {
            _dalChucNang = dalChucNang ?? throw new ArgumentNullException(nameof(dalChucNang));
        }

        /// <summary>
        /// Retrieves all available functions as DTOs asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of ChucNangDTO objects.</returns>
        public async Task<List<ChucNangDTO>> GetAllChucNangAsync()
        {
            try
            {
                // Gọi DAL để lấy danh sách Entity
                var entities = await _dalChucNang.GetAllAsync();

                // Chuyển đổi (map) danh sách Entity sang danh sách DTO
                // Sử dụng LINQ Select và hàm MapToDTO helper
                // Lọc bỏ các kết quả null từ MapToDTO nếu có thể xảy ra (dù ở đây ít khả năng)
                var dtos = entities.Select(MapToDTO).Where(dto => dto != null).ToList();

                return dtos!; // Sử dụng null-forgiving vì đã lọc null
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in BUSChucNang.GetAllChucNangAsync: {ex.Message}");
                // Ném lại lỗi để lớp gọi (GUI) có thể xử lý hoặc hiển thị thông báo
                // Có thể gói lại lỗi trong một Exception cụ thể hơn của lớp BUS nếu muốn
                throw new Exception("An error occurred while retrieving the list of functions.", ex);
                // Hoặc trả về danh sách rỗng: return new List<ChucNangDTO>();
            }
        }

        // --- Implement các phương thức khác từ IBUSChucNang nếu có ---
        // Ví dụ:
        // public async Task<ChucNangDTO?> GetChucNangByIdAsync(int id)
        // {
        //    // ... gọi DAL.GetByIdAsync sau đó MapToDTO ...
        // }


        // --- Helper method for mapping Entity to DTO ---
        /// <summary>
        /// Maps a Chucnang entity to a ChucNangDTO.
        /// </summary>
        /// <param name="entity">The Chucnang entity to map.</param>
        /// <returns>A ChucNangDTO object, or null if the entity is null.</returns>
        private ChucNangDTO? MapToDTO(Chucnang? entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new ChucNangDTO
            {
                Id = entity.Id,
                MaChucNang = entity.MaChucNang, // Giữ nguyên nullable
                TenChucNang = entity.TenChucNang, // Entity đảm bảo không null
                TenManHinh = entity.TenManHinh   // Entity đảm bảo không null
            };
        }

        // --- Helper method for mapping DTO to Entity (nếu cần cho Add/Update) ---
        /*
        private Chucnang MapToEntity(ChucNangDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            // Thêm validation cho DTO nếu cần
            if (string.IsNullOrWhiteSpace(dto.TenChucNang))
                 throw new ArgumentException("Tên chức năng không được để trống.", nameof(dto.TenChucNang));
             if (string.IsNullOrWhiteSpace(dto.TenManHinh))
                 throw new ArgumentException("Tên màn hình không được để trống.", nameof(dto.TenManHinh));

            return new Chucnang
            {
                Id = dto.Id, // Id = 0 khi thêm mới
                MaChucNang = dto.MaChucNang?.Trim(),
                TenChucNang = dto.TenChucNang.Trim(),
                TenManHinh = dto.TenManHinh.Trim()
            };
        }
        */
    }
}