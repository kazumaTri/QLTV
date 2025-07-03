// Project/Namespace: BUS

using DTO; // Cần để định nghĩa các phương thức nhận/trả về SachDTO
using System; // Cần cho Exception (nếu các phương thức BUS có thể ném exception)
using System.Collections.Generic; // Cần cho List
using System.Threading.Tasks; // Cần cho Task cho các phương thức bất đồng bộ


namespace BUS // Namespace của Business Logic Layer
{
    /// <summary>
    /// Interface cho Business Logic Layer xử lý các nghiệp vụ liên quan đến Sách (ấn bản).
    /// Định nghĩa các phương thức mà tầng GUI sẽ gọi.
    /// </summary>
    public interface IBUSSach
    {
        /// <summary>
        /// Lấy tất cả danh sách Sách (ấn bản) dưới dạng DTO.
        /// Theo cấu trúc DB, Entity Sach không có cột DaAn, nên phương thức này sẽ lấy tất cả các bản ghi hiện có.
        /// </summary>
        /// <returns>Task chứa List các SachDTO.</returns>
        Task<List<SachDTO>> GetAllSachAsync();

        /// <summary>
        /// Lấy thông tin một Sách (ấn bản) theo ID dưới dạng DTO.
        /// </summary>
        /// <param name="id">ID của Sách (ấn bản).</param>
        /// <returns>Task chứa SachDTO hoặc null nếu không tìm thấy.</returns>
        Task<SachDTO?> GetSachByIdAsync(int id);

        // Các phương thức lấy dữ liệu theo tiêu chí khác nếu cần cho UI hoặc Validation
        // Ví dụ:
        // Task<SachDTO?> GetSachByMaAsync(string maSach); // Lấy sách theo Mã ấn bản


        /// <summary>
        /// Thêm mới một Sách (ấn bản).
        /// Thực hiện validation nghiệp vụ trước khi gọi tầng DAL.
        /// </summary>
        /// <param name="sachDto">Đối tượng SachDTO chứa thông tin cần thêm.</param>
        /// <returns>Task chứa SachDTO đã được thêm (có ID mới) hoặc null nếu thêm thất bại.</returns>
        /// <exception cref="ArgumentException">Ném ra nếu dữ liệu nhập không hợp lệ (ví dụ: thiếu Tựa Sách, Đơn giá âm).</exception>
        /// <exception cref="InvalidOperationException">Ném ra nếu vi phạm quy tắc nghiệp vụ (ví dụ: trùng mã ấn bản nếu Mã không tự sinh).</exception>
        Task<SachDTO?> AddSachAsync(SachDTO sachDto);

        /// <summary>
        /// Cập nhật thông tin một Sách (ấn bản) hiện có.
        /// Thực hiện validation nghiệp vụ trước khi gọi tầng DAL.
        /// </summary>
        /// <param name="sachDto">Đối tượng SachDTO chứa thông tin cập nhật.</param>
        /// <returns>Task chứa bool, true nếu cập nhật thành công, false nếu không tìm thấy hoặc cập nhật thất bại.</returns>
        /// <exception cref="ArgumentException">Ném ra nếu dữ liệu nhập không hợp lệ.</exception>
        /// <exception cref="InvalidOperationException">Ném ra nếu vi phạm quy tắc nghiệp vụ (ví dụ: trùng mã ấn bản).</exception>
        Task<bool> UpdateSachAsync(SachDTO sachDto);

        /// <summary>
        /// Xóa vĩnh viễn một Sách (ấn bản) theo ID.
        /// BUS sẽ kiểm tra các ràng buộc nghiệp vụ (ví dụ: còn cuốn sách vật lý nào thuộc ấn bản này không) trước khi cho phép xóa.
        /// </summary>
        /// <param name="id">ID của Sách (ấn bản) cần xóa.</param>
        /// <returns>Task chứa bool, true nếu xóa thành công, false nếu không tìm thấy.</returns>
        /// <exception cref="InvalidOperationException">Ném ra nếu không thể xóa do có ràng buộc dữ liệu liên quan (còn cuốn sách vật lý).</exception>
        Task<bool> HardDeleteSachAsync(int id); // Sử dụng cho Hard Delete vì Entity Sach không có DaAn

        // Không có Soft Delete/Restore cho Entity Sach dựa trên cấu trúc DB.


        /// <summary>
        /// Kiểm tra xem một Sách (ấn bản) có thể xóa vĩnh viễn được không (ví dụ: không còn cuốn sách vật lý nào thuộc ấn bản này).
        /// Dùng để hỗ trợ UI hiển thị nút Xóa phù hợp.
        /// </summary>
        /// <param name="id">ID của Sách (ấn bản) cần kiểm tra.</param>
        /// <returns>Task chứa bool, true nếu có thể xóa, false nếu ngược lại.</returns>
        Task<bool> CanHardDeleteSachAsync(int id);
        Task<SachDTO?> AddSachMetadataAsync(SachDTO sachDto);


        // Các phương thức kiểm tra sự tồn tại/ràng buộc khác nếu cần cho UI hoặc Business Logic
        // Task<bool> IsMaSachExistsAsync(string maSach);
        // Task<bool> IsMaSachExistsExcludingIdAsync(string maSach, int excludeId);

        // Có thể thêm các phương thức nghiệp vụ phức tạp hơn liên quan đến sách nếu cần
        // Ví dụ: Task<List<SachDTO>> SearchSachAsync(string keyword);
    }
}