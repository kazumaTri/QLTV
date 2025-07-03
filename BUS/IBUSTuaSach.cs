// Project/Namespace: BUS
using DTO; // Cần để định nghĩa các phương thức nhận/trả về TuaSachDTO
using System.Collections.Generic; // Cần cho List
using System.Threading.Tasks; // Cần cho Task

namespace BUS
{
    // Interface cho Business Logic Layer của Tựa Sách
    public interface IBUSTuaSach
    {
        Task<List<TuaSachDTO>> GetAllTuaSachAsync(); // Lấy tất cả tựa sách dưới dạng DTO
        Task<TuaSachDTO?> GetTuaSachByIdAsync(int id); // Lấy tựa sách theo ID dưới dạng DTO
        Task<TuaSachDTO?> GetTuaSachByMaAsync(string maTuaSach); // Lấy tựa sách theo Mã dưới dạng DTO
        Task<TuaSachDTO?> GetTuaSachByTenAsync(string tenTuaSach); // Lấy tựa sách theo Tên dưới dạng DTO


        Task<TuaSachDTO?> AddTuaSachAsync(TuaSachDTO tuaSachDto); // Thêm mới tựa sách từ DTO
        Task<bool> UpdateTuaSachAsync(TuaSachDTO tuaSachDto); // Cập nhật tựa sách từ DTO
        Task<bool> DeleteTuaSachAsync(int id); // Xóa mềm tựa sách theo ID
        Task<bool> RestoreTuaSachAsync(int id); // Phục hồi tựa sách theo ID
        Task<bool> HardDeleteTuaSachAsync(int id); // Xóa vĩnh viễn tựa sách theo ID

        // Các phương thức nghiệp vụ khác
        Task<int> GetTotalSoLuongSachAsync(); // Lấy tổng số lượng tất cả các cuốn sách (trên tất cả các tựa sách)
        // Có thể thêm các phương thức tìm kiếm/lọc phức tạp hơn
        Task<List<TuaSachDTO>> SearchAndFilterTuaSachAsync(string? searchText, int theLoaiId, int tacGiaId);
    }
}