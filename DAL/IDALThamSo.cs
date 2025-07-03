// File: DAL/IDALThamSo.cs
using DAL.Models; // Cần để làm việc với Entity Thamso
using System;
using System.Threading.Tasks; // Cần cho Task

namespace DAL // Namespace của Data Access Layer
{
    /// <summary>
    /// Interface cho Data Access Layer xử lý các thao tác trên bảng THAMSO.
    /// Cung cấp các phương thức để lấy và cập nhật các quy định hệ thống.
    /// </summary>
    public interface IDALThamSo // Đảm bảo là public interface
    {
        /// <summary>
        /// Lấy toàn bộ Entity Thamso (thường chỉ có 1 record).
        /// </summary>
        /// <returns>Task chứa Entity Thamso hoặc null nếu không tìm thấy.</returns>
        Task<Thamso?> GetThamSoAsync(); // Lấy bộ tham số chính

        /// <summary>
        /// Cập nhật thông tin bộ tham số hiện có trong cơ sở dữ liệu.
        /// </summary>
        /// <param name="thamso">Entity Thamso chứa thông tin cập nhật. ID thường là cố định (ví dụ: 1).</param>
        /// <returns>Task chứa bool, true nếu cập nhật thành công, false nếu không tìm thấy hoặc lỗi.</returns>
        Task<bool> UpdateAsync(Thamso thamso);

        // --- Các phương thức lấy giá trị tham số cụ thể ---
        // (Tên phương thức và kiểu trả về phụ thuộc vào các cột trong bảng Thamso của bạn)

        /// <summary>
        /// Lấy quy định tuổi tối thiểu của độc giả.
        /// </summary>
        /// <returns>Tuổi tối thiểu.</returns>
        Task<int> GetMinimumReaderAgeAsync();

        /// <summary>
        /// Lấy quy định tuổi tối đa của độc giả.
        /// </summary>
        /// <returns>Tuổi tối đa.</returns>
        Task<int> GetMaximumReaderAgeAsync();

        /// <summary>
        /// Lấy quy định thời hạn hiệu lực của thẻ độc giả (tính bằng năm).
        /// </summary>
        /// <returns>Số năm hiệu lực.</returns>
        Task<int> GetCardValidityDurationAsync();

        /// <summary>
        /// Lấy quy định số ngày mượn tối đa cho một lần mượn.
        /// </summary>
        /// <returns>Số ngày mượn tối đa.</returns>
        Task<int> GetMaxBorrowDaysAsync();

        /// <summary>
        /// Lấy quy định số sách tối đa được mượn trong một lần.
        /// </summary>
        /// <returns>Số sách mượn tối đa.</returns>
        Task<int> GetMaxBooksPerLoanAsync();

        /// <summary>
        /// Lấy quy định số tiền phạt cho mỗi ngày trả sách trễ.
        /// Kiểu trả về (int hoặc decimal) phụ thuộc vào cột trong DB.
        /// </summary>
        /// <returns>Số tiền phạt mỗi ngày.</returns>
        Task<int> GetFinePerDayAsync(); // Hoặc Task<decimal>

        /// <summary>
        /// Lấy quy định về khoảng cách năm xuất bản cho phép khi nhập sách.
        /// </summary>
        /// <returns>Số năm tối đa cho phép.</returns>
        Task<int> GetPublishingYearGapAsync();
        Task<Thamso?> AddAsync(Thamso thamso);

        // --- Thêm các phương thức khác cho các quy định khác nếu có ---
        // Ví dụ:
        // Task<bool> IsFineReceiptRequiredAsync(); // Quy định có cần lập phiếu thu tiền phạt không?

    }
}