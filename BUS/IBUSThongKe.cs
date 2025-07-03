// BUS/IBUSThongKe.cs
using DTO; // Cần để sử dụng các lớp DTO báo cáo và ThongKeItemDTO
using System;
using System.Collections.Generic; // Cần cho List<>
using System.Threading.Tasks;

namespace BUS
{
    /// <summary>
    /// Interface cho lớp logic nghiệp vụ Thống kê - Báo cáo.
    /// Định nghĩa các phương thức để lấy dữ liệu thống kê.
    /// </summary>
    public interface IBUSThongKe
    {
        /// <summary>
        /// Lấy dữ liệu thống kê số lượt mượn sách theo thể loại trong một khoảng thời gian.
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu thống kê.</param>
        /// <param name="endDate">Ngày kết thúc thống kê.</param>
        /// <returns>Danh sách các đối tượng DTO chứa thông tin thống kê chi tiết theo từng thể loại.</returns>
        /// <exception cref="Exception">Ném ra nếu có lỗi trong quá trình xử lý thống kê.</exception>
        Task<List<CtBaoCaoLuotMuonTheoTheLoaiDTO>> GetThongKeLuotMuonTheoTheLoai(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Lấy danh sách Top N Tựa Sách được mượn nhiều nhất.
        /// </summary>
        /// <param name="topN">Số lượng kết quả cần lấy.</param>
        /// <returns>Danh sách ThongKeItemDTO.</returns>
        Task<List<ThongKeItemDTO>> GetTopBorrowedTuaSachAsync(int topN);

        /// <summary>
        /// Lấy danh sách Top N Thể Loại được mượn nhiều nhất.
        /// </summary>
        /// <param name="topN">Số lượng kết quả cần lấy.</param>
        /// <returns>Danh sách ThongKeItemDTO.</returns>
        Task<List<ThongKeItemDTO>> GetTopBorrowedTheLoaiAsync(int topN);

        // --- THÊM PHƯƠNG THỨC MỚI CHO BIỂU ĐỒ THÁNG ---
        /// <summary>
        /// Lấy số lượt mượn sách theo từng tháng trong một năm cụ thể.
        /// </summary>
        /// <param name="year">Năm cần thống kê.</param>
        /// <returns>Danh sách MonthlyBorrowCountsDTO chứa thông tin tháng và số lượt mượn.</returns>
        /// <exception cref="Exception">Ném ra nếu có lỗi trong quá trình xử lý thống kê.</exception>
        Task<List<MonthlyBorrowCountsDTO>> GetMonthlyBorrowCountsAsync(int year);
        // --- KẾT THÚC THÊM ---

        // --- Các phương thức thống kê khác (ví dụ) ---
        // Task<List<BaoCaoSachTraTreDTO>> GetThongKeSachTraTreAsync(DateTime ngayThongKe);
        // Task<ThongKeTongHopDTO> GetThongKeTongHopAsync(DateTime startDate, DateTime endDate);
    }

    /// <summary>
    /// DTO chứa thông tin số lượt mượn theo tháng.
    /// (Bạn có thể đặt class này trong project DTO nếu muốn)
    /// </summary>
    public class MonthlyBorrowCountsDTO
    {
        /// <summary>
        /// Số thứ tự của tháng (1-12).
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Tổng số lượt mượn trong tháng.
        /// </summary>
        public int BorrowCount { get; set; }
    }
}