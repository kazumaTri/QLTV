// Project/Namespace: DTO
using System;
using System.Collections.Generic;

namespace DTO // Namespace cho các Data Transfer Objects (DTOs)
{
    /// <summary>
    /// Data Transfer Object (DTO) cho thực thể Thể loại.
    /// Dùng để truyền dữ liệu về Thể loại giữa các tầng BUS và GUI.
    /// </summary>
    public class TheLoaiDTO
    {
        // ID của Thể loại (thường là khóa chính)
        // Cần thiết cho các thao tác Sửa/Xóa và xác định đối tượng
        public int Id { get; set; }

        // Mã Thể loại (có thể là duy nhất, có thể tự sinh hoặc nhập)
        // Sử dụng string? để cho phép giá trị null nếu MaTheLoai trong DB có thể null
        public string? MaTheLoai { get; set; }

        // Tên Thể loại (bắt buộc nhập, không thể null)
        public string TenTheLoai { get; set; } = null!; // Khai báo = null! để thể hiện rằng nó không nên null về mặt logic/nghiệp vụ

        // Có thể thêm các thuộc tính khác nếu cần hiển thị thông tin tổng hợp ở UI
        // Ví dụ: Số lượng tựa sách thuộc thể loại này (nếu được tính toán ở tầng BUS/DAL)
        // public int SoLuongTuaSach { get; set; }

        // Không nên đưa các Navigation Properties (ICollection<...>) từ Entity vào DTO cơ bản này
        // trừ khi UI thực sự cần hiển thị danh sách chi tiết của các đối tượng liên quan
        // (Thường thì chỉ cần hiển thị các thông tin tổng hợp hoặc điều hướng sang màn hình khác)


        /// <summary>
        /// Ghi đè phương thức ToString() để hiển thị TenTheLoai khi đối tượng được chuyển đổi thành chuỗi.
        /// Điều này giúp các control như ComboBox, ListBox, DataGridView hiển thị tên thay vì tên kiểu dữ liệu.
        /// </summary>
        /// <returns>Tên thể loại hoặc chuỗi rỗng nếu TenTheLoai là null.</returns>
        public override string ToString()
        {
            // Trả về tên thể loại. Sử dụng ?? string.Empty để tránh lỗi nếu TenTheLoai là null.
            // Bạn cũng có thể trả về ID nếu tên bị thiếu, ví dụ: return TenTheLoai ?? $"ID_{Id}";
            return TenTheLoai ?? string.Empty;
        }
    }
}