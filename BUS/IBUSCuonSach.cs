// Project/Namespace: BUS
using DTO; // Cần để định nghĩa các phương thức nhận/trả về CuonSachDTO
using System; // Cần cho DateTime
using System.Collections.Generic; // Cần cho List
using System.Threading.Tasks; // Cần cho Task

namespace BUS
{
    /// <summary>
    /// Interface cho Business Logic Layer của Cuonsach.
    /// </summary>
    public interface IBUSCuonSach
    {
        Task<List<CuonSachDTO>> GetAllCuonSachAsync(); // Lấy tất cả cuốn sách dưới dạng DTO (chưa xóa mềm)
        Task<CuonSachDTO?> GetCuonSachByIdAsync(int id); // Lấy cuốn sách theo ID dưới dạng DTO (chưa xóa mềm)
        Task<CuonSachDTO?> GetCuonSachByMaAsync(string maCuonSach); // Lấy cuốn sách theo Mã dưới dạng DTO (chưa xóa mềm)
        Task<List<CuonSachDTO>> GetCuonSachBySachIdAsync(int sachId); // Lấy tất cả cuốn sách thuộc một Sach ID dưới dạng DTO (chưa xóa mềm)


        // Các phương thức kiểm tra sự tồn tại/tình trạng
        Task<bool> IsCuonSachAvailableAsync(int id); // Kiểm tra cuốn sách có trạng thái 'Có sẵn' không
        Task<bool> IsCuonSachBorrowedAsync(int id); // Kiểm tra cuốn sách có đang được mượn không
        Task<bool> IsMaCuonSachExistsAsync(string maCuonSach); // Kiểm tra mã cuốn sách tồn tại (bao gồm cả đã xóa)
        Task<bool> IsMaCuonSachExistsExcludingIdAsync(string maCuonSach, int excludeId); // Kiểm tra mã cuốn sách tồn tại (loại trừ 1 ID, bao gồm cả đã xóa khác)
        Task<bool> CanSoftDeleteCuonSachAsync(int id); // Kiểm tra ràng buộc nghiệp vụ trước khi xóa mềm (còn cuốn sách chưa xóa mềm không?)
        Task<bool> CanHardDeleteCuonSachAsync(int id); // Kiểm tra ràng buộc nghiệp vụ trước khi xóa vĩnh viễn (còn chi tiết phiếu nhập/cuốn sách không?)


        // Các phương thức thêm/sửa/xóa
        Task<CuonSachDTO?> AddCuonSachAsync(CuonSachDTO cuonSachDto); // Thêm mới cuốn sách từ DTO
        Task<bool> UpdateCuonSachAsync(CuonSachDTO cuonSachDto); // Cập nhật cuốn sách từ DTO (bao gồm TinhTrang)
        Task<bool> SoftDeleteCuonSachAsync(int id); // Xóa mềm cuốn sách theo ID
        Task<bool> RestoreCuonSachAsync(int id); // Phục hồi cuốn sách đã xóa mềm theo ID
        Task<bool> HardDeleteCuonSachAsync(int id); // Xóa vĩnh viễn cuốn sách theo ID (Cẩn thận)

        // Các phương thức nghiệp vụ khác (ví dụ: Cập nhật tình trạng khi mượn/trả)
        Task<bool> UpdateTinhTrangAsync(int cuonSachId, int newTinhTrang); // Cập nhật tình trạng cụ thể

        // Hàm mapping int TinhTrang sang string (Business Rule)
        string GetTinhTrangText(int tinhTrang);
    }
}