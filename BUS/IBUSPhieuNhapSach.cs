using DTO; // Cần using DTO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public interface IBUSPhieuNhapSach
    {
        /// <summary>
        /// Lập phiếu nhập sách mới bao gồm thông tin phiếu và chi tiết phiếu.
        /// Đồng thời cập nhật số lượng sách và tạo các cuốn sách mới.
        /// </summary>
        /// <param name="phieuNhapSachDTO">DTO chứa thông tin phiếu nhập cần tạo.</param>
        /// <returns>Số phiếu nhập vừa tạo thành công.</returns>
        /// <exception cref="ArgumentNullException">Khi DTO đầu vào là null.</exception>
        /// <exception cref="InvalidOperationException">Khi dữ liệu không hợp lệ hoặc có lỗi trong quá trình xử lý nghiệp vụ.</exception>
        /// <exception cref="Exception">Lỗi hệ thống hoặc database.</exception>
        Task<int> LapPhieuNhapAsync(PhieuNhapSachDTO phieuNhapSachDTO);

        /// <summary>
        /// Lấy danh sách tất cả phiếu nhập sách (thông tin cơ bản).
        /// </summary>
        /// <returns>Danh sách PhieuNhapSachDTO.</returns>
        Task<List<PhieuNhapSachDTO>> GetAllPhieuNhapDTOAsync();

        /// <summary>
        /// Lấy thông tin chi tiết của một phiếu nhập sách theo số phiếu.
        /// </summary>
        /// <param name="soPhieuNhap">Số phiếu nhập cần xem.</param>
        /// <returns>PhieuNhapSachDTO chứa đầy đủ chi tiết, hoặc null nếu không tìm thấy.</returns>
        Task<PhieuNhapSachDTO?> GetPhieuNhapDTOByIdAsync(int soPhieuNhap);

        // Có thể thêm các phương thức khác như tìm kiếm phiếu nhập, ...
    }
}