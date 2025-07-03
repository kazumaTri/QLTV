// File: BUS/IBUSTheLoai.cs
// Đã hoàn chỉnh và đồng bộ với BUSTheLoai.cs (đã sửa)

using DTO; // Cần để định nghĩa các phương thức nhận/trả về TheLoaiDTO
using System; // Cần cho Exception (nếu các phương thức BUS có thể ném exception)
using System.Collections.Generic; // Cần cho List
using System.Threading.Tasks; // Cần cho Task cho các phương thức bất đồng bộ

namespace BUS // Namespace của Business Logic Layer
{
    /// <summary>
    /// Interface cho Business Logic Layer xử lý các nghiệp vụ liên quan đến Thể loại.
    /// Định nghĩa các phương thức mà tầng GUI sẽ gọi.
    /// </summary>
    public interface IBUSTheLoai
    {
        /// <summary>
        /// Lấy tất cả danh sách Thể loại dưới dạng DTO.
        /// </summary>
        /// <returns>Task chứa List các TheLoaiDTO.</returns>
        Task<List<TheLoaiDTO>> GetAllTheLoaiAsync();

        /// <summary>
        /// Lấy thông tin một Thể loại theo ID dưới dạng DTO.
        /// </summary>
        /// <param name="id">ID của Thể loại.</param>
        /// <returns>Task chứa TheLoaiDTO hoặc null nếu không tìm thấy.</returns>
        Task<TheLoaiDTO?> GetTheLoaiByIdAsync(int id);

        /// <summary>
        /// Thêm mới một Thể loại. Mã Thể loại sẽ được tự động sinh.
        /// Thực hiện validation nghiệp vụ trước khi gọi tầng DAL.
        /// </summary>
        /// <param name="theLoaiDto">Đối tượng TheLoaiDTO chứa thông tin cần thêm (MaTheLoai có thể là null).</param>
        /// <returns>Task chứa TheLoaiDTO đã được thêm (có ID và MaTheLoai mới) hoặc null nếu thêm thất bại.</returns>
        /// <exception cref="ArgumentException">Ném ra nếu dữ liệu nhập không hợp lệ.</exception>
        /// <exception cref="InvalidOperationException">Ném ra nếu vi phạm quy tắc nghiệp vụ (ví dụ: trùng tên, lỗi tạo mã).</exception>
        /// <exception cref="Exception">Ném ra nếu có lỗi hệ thống khác.</exception>
        Task<TheLoaiDTO?> AddTheLoaiAsync(TheLoaiDTO theLoaiDto);

        /// <summary>
        /// Cập nhật thông tin một Thể loại hiện có (không bao gồm Mã Thể Loại).
        /// Thực hiện validation nghiệp vụ trước khi gọi tầng DAL.
        /// </summary>
        /// <param name="theLoaiDto">Đối tượng TheLoaiDTO chứa thông tin cập nhật.</param>
        /// <returns>Task chứa bool, true nếu cập nhật thành công, false nếu không tìm thấy hoặc cập nhật thất bại.</returns>
        /// <exception cref="ArgumentException">Ném ra nếu dữ liệu nhập không hợp lệ.</exception>
        /// <exception cref="InvalidOperationException">Ném ra nếu vi phạm quy tắc nghiệp vụ (ví dụ: trùng tên).</exception>
        /// <exception cref="Exception">Ném ra nếu có lỗi hệ thống khác.</exception>
        Task<bool> UpdateTheLoaiAsync(TheLoaiDTO theLoaiDto);

        /// <summary>
        /// Xóa vĩnh viễn một Thể loại theo ID.
        /// BUS sẽ kiểm tra các ràng buộc nghiệp vụ trước khi cho phép xóa.
        /// </summary>
        /// <param name="id">ID của Thể loại cần xóa.</param>
        /// <returns>Task chứa bool, true nếu xóa thành công, false nếu không tìm thấy.</returns>
        /// <exception cref="ArgumentException">Ném ra nếu ID không hợp lệ.</exception>
        /// <exception cref="InvalidOperationException">Ném ra nếu không thể xóa do có ràng buộc dữ liệu liên quan.</exception>
        /// <exception cref="Exception">Ném ra nếu có lỗi hệ thống khác.</exception>
        Task<bool> DeleteTheLoaiAsync(int id);

        /// <summary>
        /// Kiểm tra xem một Thể loại có thể xóa vĩnh viễn được không (ví dụ: không có sách/tựa sách liên quan).
        /// Dùng để hỗ trợ UI hiển thị/ẩn nút Xóa hoặc đưa ra thông báo phù hợp.
        /// </summary>
        /// <param name="id">ID của Thể loại cần kiểm tra.</param>
        /// <returns>Task chứa bool, true nếu có thể xóa, false nếu ngược lại hoặc có lỗi.</returns>
        Task<bool> CanDeleteTheLoaiAsync(int id);

        // Bỏ CanHardDeleteTheLoaiAsync vì đã có CanDeleteTheLoaiAsync với ý nghĩa tương tự.
        // Bỏ các phương thức kiểm tra tồn tại (Is...Exists...) vì chúng là chi tiết triển khai của DAL
        // mà BUS sẽ sử dụng nội bộ khi cần (ví dụ khi validate tên hoặc kiểm tra mã mới tạo).
    }
}