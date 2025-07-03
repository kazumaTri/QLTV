using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Cần để sử dụng Attributes nếu muốn
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class PhieuNhapSachDTO
    {
        public int SoPhieuNhap { get; set; } // Thường là identity, chỉ đọc

        [Required(ErrorMessage = "Ngày nhập không được để trống")]
        public DateTime NgayNhap { get; set; } = DateTime.Now; // Mặc định ngày hiện tại

        public decimal TongTien { get; set; } // Sẽ được tính toán dựa trên chi tiết

        [MinLength(1, ErrorMessage = "Phiếu nhập phải có ít nhất một sách")]
        public List<CtPhieuNhapDTO> ChiTietPhieuNhap { get; set; } = new List<CtPhieuNhapDTO>(); // Danh sách chi tiết

        // Thêm constructor hoặc các thuộc tính khác nếu cần
        // Ví dụ: Id người lập phiếu (nếu có hệ thống người dùng)
        // public int IdNguoiLap { get; set; }
    }
}