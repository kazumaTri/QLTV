// Project/Namespace: DAL
using DAL.Models; // Cần để định nghĩa các phương thức nhận/trả về Entity Cuonsach
using System.Collections.Generic; // Cần cho List
using System.Threading.Tasks; // Cần cho Task

namespace DAL
{
    /// <summary>
    /// Interface cho Data Access Layer của Cuonsach (Cuốn Sách Cụ Thể).
    /// Bao gồm các phương thức CRUD, lấy dữ liệu theo tiêu chí, và kiểm tra ràng buộc/tồn tại.
    /// </summary>
    public interface IDALCuonSach
    {
        // Các phương thức lấy dữ liệu
        Task<List<Cuonsach>> GetAllAsync(); // Lấy tất cả cuốn sách (chưa xóa mềm)
        Task<Cuonsach?> GetByIdAsync(int id); // Lấy cuốn sách theo ID (chưa xóa mềm)
        Task<Cuonsach?> GetByIdIncludingDeletedAsync(int id); // Lấy cuốn sách theo ID (bao gồm cả đã xóa)
        Task<Cuonsach?> GetByMaAsync(string maCuonSach); // Lấy cuốn sách theo Mã (chưa xóa mềm)
        Task<Cuonsach?> GetByMaIncludingDeletedAsync(string maCuonSach); // Lấy theo Mã (bao gồm đã xóa)

        Task<List<Cuonsach>> GetBySachIdAsync(int sachId); // Lấy tất cả cuốn sách thuộc một Sach ID (chưa xóa mềm)


        // Các phương thức kiểm tra sự tồn tại/tình trạng
        Task<bool> IsMaCuonSachExistsAsync(string maCuonSach); // Kiểm tra mã cuốn sách tồn tại (bao gồm cả đã xóa)
        Task<bool> IsMaCuonSachExistsExcludingIdAsync(string maCuonSach, int excludeId); // Kiểm tra mã cuốn sách tồn tại (loại trừ 1 ID, bao gồm cả đã xóa khác)
        Task<bool> IsBorrowedAsync(int id); // Kiểm tra xem cuốn sách có đang được mượn không (dựa vào TinhTrang hoặc Phieumuontra)

        /// <summary>
        /// Kiểm tra xem cuốn sách có bất kỳ bản ghi liên quan nào trong các bảng khác không (phiếu mượn, báo cáo...).
        /// </summary>
        /// <param name="cuonSachId">ID của cuốn sách cần kiểm tra.</param>
        /// <returns>Task<bool> True nếu có liên quan, False nếu ngược lại.</returns>
        Task<bool> HasAnyRelatedReferencesAsync(int cuonSachId);

        /// <summary>
        /// Lấy tổng số lượng CuonSach thuộc một Sach (ấn bản) đang ở trạng thái 'Đang mượn'.
        /// </summary>
        /// <param name="sachId">ID của ấn bản sách (Sach.Id).</param>
        /// <returns>Task chứa số lượng cuốn sách đang mượn thuộc ấn bản đó.</returns>
        Task<int> GetSoLuongCuonSachDangMuonBySachIdAsync(int sachId);

        // Các phương thức thêm/sửa/xóa
        Task<Cuonsach?> AddAsync(Cuonsach cuonSach); // Thêm mới cuốn sách
        Task<bool> UpdateAsync(Cuonsach cuonSach); // Cập nhật cuốn sách (bao gồm IdSach, MaCuonSach, TinhTrang)
        Task<bool> SoftDeleteAsync(int id); // Xóa mềm cuốn sách (Set DaAn = 1)
        Task<bool> RestoreAsync(int id); // Phục hồi cuốn sách đã xóa mềm (Set DaAn = 0)
        Task<bool> HardDeleteAsync(int id); // Xóa vĩnh viễn cuốn sách (Cẩn thận với ràng buộc FK)

        // <<< ĐÃ ĐỔI TÊN TỪ HasAnyActiveCuonSachBySachIdAsync SANG HasActiveCopiesAsync >>>
        /// <summary>
        /// Kiểm tra tồn tại cuốn sách vật lý "active" theo ID Sách (chưa xóa mềm, có sẵn hoặc đang mượn).
        /// </summary>
        /// <param name="sachId">ID của ấn bản sách.</param>
        /// <returns>Task<bool> True nếu có bản sao active, False nếu ngược lại.</returns>
        Task<bool> HasActiveCopiesAsync(int sachId);


        Task<bool> HasAnyCuonSachBySachIdIncludingDeletedAsync(int sachId); // Kiểm tra tồn tại cuốn sách vật lý theo ID Sách (bao gồm cả đã xóa mềm)

        Task<bool> AddRangeAsync(List<Cuonsach> cuonSachs);
        // Các phương thức hỗ trợ khác (nếu cần)
        // Task<List<Cuonsach>> GetBySachIdIncludingDeletedAsync(int sachId); // Lấy cuốn sách theo Sach ID (bao gồm đã xóa)
        // Task<bool> UpdateTinhTrangAsync(int cuonSachId, int newTinhTrang); // Nếu cần DAL có phương thức cập nhật tình trạng riêng
    }
}