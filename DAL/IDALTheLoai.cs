// File: DAL/IDALTheLoai.cs
// Đã hoàn chỉnh, đồng bộ với DALTheLoai.cs và IBUSTheLoai.cs (đã sửa)

using DAL.Models; // Cần để làm việc với Entity Theloai
using DTO; // Cần cho các phương thức trả về DTO nếu có (ví dụ nếu AddAsync trả về DTO thay vì Entity)
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL
{
    /// <summary>
    /// Interface cho Data Access Layer của Thể loại.
    /// Định nghĩa các phương thức CRUD cơ bản và các phương thức hỗ trợ kiểm tra.
    /// Các phương thức thường làm việc với Entity (Theloai) thay vì DTO.
    /// </summary>
    public interface IDALTheLoai
    {
        // --- Các phương thức lấy dữ liệu (Thường trả về Entity) ---
        /// <summary>
        /// Lấy tất cả danh sách Thể loại.
        /// </summary>
        /// <returns>Task chứa List các Entity Theloai.</returns>
        Task<List<Theloai>> GetAllAsync();

        /// <summary>
        /// Lấy thông tin một Thể loại theo ID.
        /// </summary>
        /// <param name="id">ID của Thể loại.</param>
        /// <returns>Task chứa Entity Theloai hoặc null nếu không tìm thấy.</returns>
        Task<Theloai?> GetByIdAsync(int id);

        // --- Các phương thức kiểm tra sự tồn tại/ràng buộc ---

        /// <summary>
        /// Kiểm tra xem Mã Thể Loại đã tồn tại trong CSDL hay chưa.
        /// </summary>
        /// <param name="maTheLoai">Mã Thể Loại cần kiểm tra.</param>
        /// <returns>Task chứa bool, true nếu mã đã tồn tại, false nếu chưa.</returns>
        Task<bool> MaTheLoaiExistsAsync(string maTheLoai);

        /// <summary>
        /// Kiểm tra xem Tên Thể Loại đã tồn tại trong CSDL hay chưa.
        /// </summary>
        /// <param name="tenTheLoai">Tên Thể Loại cần kiểm tra.</param>
        /// <returns>Task chứa bool, true nếu tên đã tồn tại, false nếu chưa.</returns>
        Task<bool> IsTenTheLoaiExistsAsync(string tenTheLoai);

        /// <summary>
        /// Kiểm tra xem Tên Thể Loại đã tồn tại trong CSDL (loại trừ một ID cụ thể).
        /// Dùng khi cập nhật để đảm bảo tên mới không trùng với các thể loại khác.
        /// </summary>
        /// <param name="tenTheLoai">Tên Thể Loại cần kiểm tra.</param>
        /// <param name="excludeId">ID của thể loại đang được cập nhật (để loại trừ khỏi việc kiểm tra).</param>
        /// <returns>Task chứa bool, true nếu tên đã tồn tại ở thể loại khác, false nếu chưa.</returns>
        Task<bool> IsTenTheLoaiExistsExcludingIdAsync(string tenTheLoai, int excludeId);

        /// <summary>
        /// Kiểm tra xem thể loại có thể xóa được không (dựa trên các ràng buộc khóa ngoại, ví dụ: TuaSach).
        /// </summary>
        /// <param name="id">ID của thể loại cần kiểm tra.</param>
        /// <returns>Task<bool> True nếu CÓ THỂ xóa (không có ràng buộc), False nếu KHÔNG THỂ xóa (có ràng buộc).</returns>
        Task<bool> CanDeleteAsync(int id); // Đổi tên từ HasAnyRelatedReferencesAsync và đảo ngược logic trả về

        // --- Các phương thức hỗ trợ tạo mã ---
        /// <summary>
        /// Lấy số thứ tự lớn nhất từ các Mã Thể Loại hiện có (theo định dạng "TLxxx").
        /// Dùng để hỗ trợ việc tạo mã tự động ở tầng BUS.
        /// </summary>
        /// <returns>Task chứa số nguyên là số lớn nhất tìm được, hoặc 0 nếu không có mã nào hợp lệ.</returns>
        Task<int> GetMaxMaTheLoaiNumberAsync(); // <<< THÊM PHƯƠNG THỨC NÀY

        // --- Các phương thức thêm/sửa/xóa (Thường làm việc với Entity) ---

        /// <summary>
        /// Thêm mới một Entity Theloai vào CSDL.
        /// </summary>
        /// <param name="theLoai">Entity Theloai cần thêm (ID nên là 0).</param>
        /// <returns>Task chứa Entity Theloai đã được thêm (bao gồm ID mới do CSDL tạo) hoặc null nếu thất bại.</returns>
        Task<Theloai?> AddAsync(Theloai theLoai);

        /// <summary>
        /// Cập nhật thông tin một Entity Theloai trong CSDL.
        /// Lưu ý: Mã Thể loại không nên được cập nhật ở đây.
        /// </summary>
        /// <param name="theLoai">Entity Theloai chứa thông tin cập nhật.</param>
        /// <returns>Task chứa bool, true nếu cập nhật thành công, false nếu không.</returns>
        Task<bool> UpdateAsync(Theloai theLoai);

        /// <summary>
        /// Xóa vĩnh viễn một Entity Theloai khỏi CSDL theo ID.
        /// Tầng BUS nên gọi CanDeleteAsync trước khi gọi phương thức này.
        /// </summary>
        /// <param name="id">ID của thể loại cần xóa.</param>
        /// <returns>Task chứa bool, true nếu xóa thành công, false nếu không tìm thấy.</returns>
        Task<bool> DeleteAsync(int id); // <<< Đổi tên từ HardDeleteAsync

        // Bỏ các phương thức GetByMaAsync, GetByTenAsync, IsMaTheLoaiExistsExcludingIdAsync
        // để đồng bộ với interface IBUSTheLoai và DALTheLoai đã sửa trước đó.
        // Nếu bạn thực sự cần chúng cho logic khác, hãy thêm lại.
    }
}