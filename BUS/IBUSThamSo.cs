// Project/Namespace: BUS

using DTO; // Cần để định nghĩa các phương thức nhận/trả về ThamSoDTO
using System; // Cần cho Exception (nếu các phương thức BUS có thể ném exception)
using System.Collections.Generic; // Cần cho List (nếu có phương thức trả về danh sách tham số, mặc dù thường chỉ có 1 bộ tham số)
using System.Threading.Tasks; // Cần cho Task cho các phương thức bất đồng bộ


namespace BUS // Namespace của Business Logic Layer
{
    /// <summary>
    /// Interface cho Business Logic Layer xử lý các nghiệp vụ liên quan đến Tham số hệ thống.
    /// Định nghĩa các phương thức mà tầng GUI (ví dụ: Form Cài đặt) sẽ gọi.
    /// </summary>
    public interface IBUSThamSo
    {
        /// <summary>
        /// Lấy bộ tham số hệ thống hiện tại dưới dạng DTO.
        /// Hệ thống thường chỉ có một bộ tham số (ví dụ: với ID cố định như 1).
        /// </summary>
        /// <returns>Task chứa ThamSoDTO hoặc null nếu không tìm thấy bộ tham số nào.</returns>
        Task<ThamSoDTO?> GetThamSoAsync();

        /// <summary>
        /// Cập nhật bộ tham số hệ thống.
        /// Thực hiện validation nghiệp vụ trước khi gọi tầng DAL.
        /// </summary>
        /// <param name="thamSoDto">Đối tượng ThamSoDTO chứa thông tin tham số cần cập nhật.</param>
        /// <returns>Task chứa bool, true nếu cập nhật thành công.</returns>
        /// <exception cref="ArgumentException">Ném ra nếu dữ liệu tham số không hợp lệ (ví dụ: Tuổi tối thiểu > Tuổi tối đa, giá trị âm).</exception>
        /// <exception cref="InvalidOperationException">Ném ra nếu không tìm thấy bộ tham số để cập nhật hoặc vi phạm quy tắc nghiệp vụ khác.</exception>
        Task<bool> UpdateThamSoAsync(ThamSoDTO thamSoDto);

        // Thêm phương thức Thêm mới nếu hệ thống cho phép thêm các bộ tham số khác nhau
        // Task<ThamSoDTO?> AddThamSoAsync(ThamSoDTO thamSoDto);

        // Thường không có phương thức Xóa cho bộ tham số hệ thống từ GUI
        // Task<bool> DeleteThamSoAsync(int id);

        // Các phương thức kiểm tra hoặc lấy giá trị tham số cụ thể nếu cần cho validation nghiệp vụ ở các BUS khác
        // Ví dụ:
        // Task<int> GetSoNgayMuonToiDaAsync();
        // Task<int> GetSoSachMuonToiDaAsync();
        // Task<bool> IsAdQdkttienThuEnabledAsync(); // Kiểm tra cờ AdQdkttienThu


        // Có thể thêm các phương thức khác nếu có nghiệp vụ phức tạp liên quan đến tham số
    }
}