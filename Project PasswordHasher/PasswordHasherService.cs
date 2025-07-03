using BCryptNet = BCrypt.Net.BCrypt; // Sử dụng alias để rõ ràng
using System;
using System.Diagnostics; // Thêm using này để sử dụng Debug

namespace PasswordHasher // <-- Đổi tên namespace cho khớp với tên Project (thường là PasswordHasher)
{
    /// <summary>
    /// Service để xử lý việc băm và xác thực mật khẩu bằng BCrypt.
    /// </summary>
    public class PasswordHasherService // <--- Lớp xử lý chính, tên này đã đúng
    {
        // Constructor mặc định (cần thiết nếu BUS tạo bằng new PasswordHasherService())
        public PasswordHasherService()
        {
            // Không cần logic khởi tạo đặc biệt ở đây cho BCrypt.Net
        }

        /// <summary>
        /// Băm mật khẩu bằng BCrypt.
        /// </summary>
        /// <param name="password">Mật khẩu gốc.</param>
        /// <returns>Chuỗi mật khẩu đã được băm.</returns>
        public string HashPassword(string password)
        {
            // Độ phức tạp (work factor) có thể điều chỉnh, mặc định thường là đủ tốt (ví dụ: 10-12)
            // Thêm work factor tường minh nếu muốn: BCryptNet.HashPassword(password, BCryptNet.GenerateSalt(12));
            return BCryptNet.HashPassword(password);
        }

        /// <summary>
        /// Xác thực mật khẩu gốc với mật khẩu đã được băm.
        /// </summary>
        /// <param name="password">Mật khẩu gốc người dùng nhập.</param>
        /// <param name="hashedPassword">Mật khẩu đã băm lấy từ cơ sở dữ liệu.</param>
        /// <returns>True nếu mật khẩu khớp, False nếu không khớp hoặc có lỗi.</returns>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            // Kiểm tra các chuỗi có giá trị không trước khi xác thực
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            {
                Debug.WriteLine("VerifyPassword failed: password or hashedPassword is null or empty.");
                return false;
            }

            try
            {
                // *** DÒNG QUAN TRỌNG: Sử dụng BCrypt.Net để so sánh mật khẩu gốc và mật khẩu đã hash ***
                return BCryptNet.Verify(password, hashedPassword); // 
            }
            // Bắt các lỗi cụ thể hơn nếu cần xử lý riêng (ví dụ: định dạng hash sai)
            catch (BCrypt.Net.SaltParseException ex)
            {
                // Ghi lại lỗi (quan trọng khi debug)
                // Sử dụng Debug.WriteLine thay vì Console.WriteLine cho ứng dụng WinForms/WPF
                Debug.WriteLine($"Error parsing salt in hash string: {ex.Message}");
                Debug.WriteLine($"VerifyPassword SaltParseException: {ex}");
                return false; // Chuỗi hash không hợp lệ
            }
            catch (Exception ex)
            {
                // Bắt các lỗi không xác định khác trong quá trình xác thực
                Debug.WriteLine($"Unknown error during password verification: {ex.Message}");
                Debug.WriteLine($"VerifyPassword Exception: {ex}");
                return false;
            }
        }

        // *** Thêm constructor có tham số nếu muốn dùng DI sau này ***
        // public PasswordHasherService(SomeDependency dependency) { ... }
    }
}
