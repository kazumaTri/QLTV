using System; // Có thể không cần nhưng để sẵn
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    /// <summary>
    /// Data Transfer Object (DTO) cho một mục trong kết quả thống kê (ví dụ: tên và số lượng).
    /// </summary>
    public class ThongKeItemDTO // <<< Sửa thành public và đổi tên lớp
    {
        /// <summary>
        /// Tên của mục thống kê (ví dụ: Tên Tựa Sách, Tên Thể Loại).
        /// </summary>
        public string Ten { get; set; } = string.Empty;

        /// <summary>
        /// Số lượt mượn hoặc số lượng tương ứng.
        /// </summary>
        public int SoLuotMuon { get; set; }
    }
}