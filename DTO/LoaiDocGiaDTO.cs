using System; // Cần cho các kiểu dữ liệu cơ bản như int, string

namespace DTO
{
    /// <summary>
    /// Data Transfer Object (DTO) cho Loại Độc Giả.
    /// Dùng để truyền dữ liệu Loại Độc Giả giữa các tầng ứng dụng (GUI, BUS, DAL).
    /// Bao gồm thông tin cơ bản và số lượng độc giả liên quan (nếu có).
    /// </summary>
    public class LoaiDocGiaDTO
    {
        /// <summary>
        /// ID của loại độc giả (Khóa chính).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Mã định danh của loại độc giả (Ví dụ: L001, VIP).
        /// Có thể là null nếu cơ sở dữ liệu cho phép.
        /// </summary>
        public string? MaLoaiDocGia { get; set; } // Nullable nếu DB cho phép null

        /// <summary>
        /// Tên của loại độc giả (Ví dụ: Độc giả thường, Độc giả VIP).
        /// Khởi tạo là string rỗng để tránh lỗi null reference.
        /// </summary>
        public string TenLoaiDocGia { get; set; } = string.Empty; // Khởi tạo để tránh null

        /// <summary>
        /// Số lượng độc giả hiện tại thuộc loại này.
        /// Thuộc tính này được tính toán và gán ở tầng BUS khi cần thiết (ví dụ: khi lấy danh sách có kèm số lượng).
        /// </summary>
        public int SoLuongDocGia { get; set; } // Thêm thuộc tính để chứa số lượng

        // --- Ghi chú ---
        // DTO thường không chứa các navigation properties (ví dụ: ICollection<Docgia>)
        // như trong lớp Entity Model của EF Core để giữ cho DTO đơn giản và tập trung vào việc truyền dữ liệu.
        // Các thuộc tính ở đây nên là các kiểu dữ liệu cơ bản (primitive types) hoặc các DTO khác nếu cần thiết.
    }
}