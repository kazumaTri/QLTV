// Project/Namespace: DTO

using System;
using System.Collections.Generic; // Cần cho List nếu DTO có collection

namespace DTO
{
    /// <summary>
    /// DTO (Data Transfer Object) để truyền dữ liệu Độc giả giữa các tầng.
    /// Chứa các thuộc tính cần thiết cho việc hiển thị và thao tác trên giao diện.
    /// </summary>
    public class DocgiaDTO
    {
        /// <summary>
        /// ID của độc giả (khóa chính).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Mã độc giả (ví dụ: DG00001), có thể tự động tạo.
        /// Nullable vì có thể chưa được gán khi tạo mới.
        /// </summary>
        public string? MaDocGia { get; set; }

        /// <summary>
        /// Tên đầy đủ của độc giả (bắt buộc).
        /// Sử dụng null-forgiving operator (!) vì được coi là bắt buộc trong logic nghiệp vụ.
        /// </summary>
        public string TenDocGia { get; set; } = null!;

        /// <summary>
        /// Ngày sinh của độc giả.
        /// Nullable DateTime để cho phép không nhập hoặc giá trị không xác định.
        /// </summary>
        public DateTime? NgaySinh { get; set; }

        /// <summary>
        /// Địa chỉ liên lạc của độc giả.
        /// </summary>
        public string? DiaChi { get; set; }

        /// <summary>
        /// Số điện thoại liên lạc của độc giả.
        /// </summary>
        public string? DienThoai { get; set; }

        /// <summary>
        /// Địa chỉ email của độc giả.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Ngày cấp thẻ thư viện cho độc giả.
        /// Nullable DateTime.
        /// </summary>
        public DateTime? NgayLapThe { get; set; }

        /// <summary>
        /// Ngày thẻ thư viện hết hạn.
        /// Nullable DateTime.
        /// </summary>
        public DateTime? NgayHetHan { get; set; }

        /// <summary>
        /// Khóa ngoại liên kết đến bảng LoaiDocGia (ID của loại độc giả).
        /// </summary>
        public int IdLoaiDocGia { get; set; }

        /// <summary>
        /// Tổng số tiền nợ hiện tại của độc giả (ví dụ: tiền phạt trả sách trễ).
        /// Nullable decimal để thể hiện giá trị tiền tệ.
        /// </summary>
        public decimal? TongNoHienTai { get; set; }

        /// <summary>
        /// ID của tài khoản người dùng (Nguoidung) liên kết với độc giả này.
        /// </summary>
        public int IdNguoiDung { get; set; }

        /// <summary>
        /// Tên đăng nhập của tài khoản người dùng liên kết.
        /// </summary>
        public string? TenDangNhap { get; set; }

        /// <summary>
        /// ID của vai trò/nhóm người dùng (NhomNguoiDung) liên kết.
        /// Nullable int.
        /// </summary>
        public int? IdVaiTroNguoiDung { get; set; }

        /// <summary>
        /// Dùng để chứa mật khẩu MỚI khi người dùng muốn thay đổi hoặc khi tạo tài khoản mới.
        /// Luôn là null khi đọc dữ liệu từ DB lên. Không bao giờ lưu trữ mật khẩu đã hash ở đây.
        /// </summary>
        public string? MatKhauNguoiDung { get; set; }

        /// <summary>
        /// Tên của loại độc giả (ví dụ: Sinh viên, Giáo viên).
        /// Được lấy thông qua join/navigation từ IdLoaiDocGia.
        /// </summary>
        public string? TenLoaiDocGia { get; set; }

        /// <summary>
        /// Cờ đánh dấu độc giả đã bị xóa mềm (ẩn khỏi các giao diện thông thường).
        /// true: Đã xóa mềm, false: Đang hoạt động.
        /// </summary>
        public bool DaAn { get; set; }

        // --- Thuộc tính ví dụ được thêm vào ---

        /// <summary>
        /// Số lượng sách mà độc giả đang mượn (chưa trả).
        /// Thuộc tính này cần được tính toán và gán giá trị ở tầng BUS trước khi gửi về GUI.
        /// Mặc định là 0.
        /// </summary>
        public int SoSachDangMuon { get; set; } = 0;

        /// <summary>
        /// Trạng thái của thẻ độc giả (ví dụ: "Hoạt động", "Hết hạn", "Bị khóa").
        /// Thuộc tính này cần được xác định và gán giá trị ở tầng BUS dựa trên NgayHetHan và các quy định khác.
        /// </summary>
        public string? TrangThaiThe { get; set; }

        // ----------------------------------------
    }
}