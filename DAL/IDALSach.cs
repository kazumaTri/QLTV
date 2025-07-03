// Project/Namespace: DAL
using DAL.Models; // Cần để định nghĩa các phương thức nhận/trả về Entity Sach
using System.Collections.Generic; // Cần cho List
using System.Threading.Tasks; // Cần cho Task

namespace DAL
{
    /// <summary>
    /// Interface cho Data Access Layer của Sach (Đầu sách/Phiên bản sách).
    /// </summary>
    public interface IDALSach
    {
        // Các phương thức lấy dữ liệu
        Task<List<Sach>> GetAllAsync(); // Lấy tất cả sách (chưa xóa mềm)
        Task<Sach?> GetByIdAsync(int id); // Lấy sách theo ID (chưa xóa mềm)
        Task<Sach?> GetByIdIncludingDeletedAsync(int id); // Lấy sách theo ID (bao gồm cả đã xóa)
        Task<Sach?> GetByMaAsync(string maSach); // Lấy sách theo Mã (chưa xóa mềm)
        Task<Sach?> GetByMaIncludingDeletedAsync(string maSach); // Lấy sách theo Mã (bao gồm cả đã xóa)
        Task<List<Sach>> GetByTuaSachIdAsync(int tuaSachId); // Lấy tất cả sách thuộc một Tựa sách (chưa xóa mềm)


        // Các phương thức kiểm tra sự tồn tại/ràng buộc
        Task<bool> IsMaSachExistsAsync(string maSach); // Kiểm tra mã sách tồn tại (bao gồm cả đã xóa)
        Task<bool> IsMaSachExistsExcludingIdAsync(string maSach, int excludeId); // Kiểm tra mã sách tồn tại (loại trừ 1 ID, bao gồm cả đã xóa khác)
        Task<bool> HasActiveCopiesAsync(int id); // Kiểm tra xem có cuốn sách nào (Cuonsach) thuộc Sach này chưa bị xóa mềm không. (Logic nghiệp vụ cho xóa mềm Sach)
        Task<bool> HasAnyCopiesAsync(int id); // Kiểm tra xem có cuốn sách nào (Cuonsach) thuộc Sach này không (bao gồm cả đã xóa). (Logic nghiệp vụ cho hard delete Sach)


        // Các phương thức thêm/sửa/xóa
        Task<Sach?> AddAsync(Sach sach); // Thêm mới sách
        Task<bool> UpdateAsync(Sach sach); // Cập nhật sách (bao gồm MaSach, IdTuaSach, SoLuong, NamXb, NhaXb, DonGia)

        /// <summary>
        /// Cập nhật thông tin cho nhiều đối tượng Sách cùng lúc.
        /// Thường dùng để cập nhật số lượng sau khi nhập sách.
        /// </summary>
        /// <param name="sachs">Danh sách các đối tượng Sách cần cập nhật.</param>
        /// <returns>Task chứa bool, true nếu cập nhật thành công, false nếu có lỗi.</returns>
        Task<bool> UpdateRangeAsync(List<Sach> sachs); // <<< PHƯƠNG THỨC ĐÃ ĐƯỢC THÊM

        Task<bool> SoftDeleteAsync(int id); // Xóa mềm sách (Set DaAn = 1)
        Task<bool> RestoreAsync(int id); // Phục hồi sách đã xóa mềm (Set DaAn = 0)
        Task<bool> HardDeleteAsync(int id); // Xóa vĩnh viễn sách (Cẩn thận với ràng buộc FK từ CtPhieunhap, Cuonsach)
        Task<int> GetTotalCountAsync();
        Task<int> GetSoLuongCuonSachDangMuonBySachIdAsync(int sachId);

        // Các phương thức nghiệp vụ đặc thù (nếu cần DAL xử lý)
        // Task<bool> UpdateSoLuongAsync(int id, int newSoLuong); // Cập nhật tổng số lượng
        // Task<bool> UpdateSoLuongConLaiAsync(int id, int newSoLuongConLai); // Cập nhật số lượng còn lại (thường do BUS tính)
    }
}