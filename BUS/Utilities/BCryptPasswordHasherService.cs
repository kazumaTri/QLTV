using BCrypt.Net; // Cần cài gói NuGet BCrypt.Net-Core

namespace BUS.Utilities
{
    /// <summary>
    /// Triển khai IPasswordHasherService sử dụng thuật toán BCrypt.
    /// </summary>
    public class BCryptPasswordHasherService : IPasswordHasherService
    {
        // BCrypt.Net.BCrypt không có dependency nào cần inject, nên constructor mặc định là OK.
        // public BCryptPasswordHasherService() { } // Có thể có hoặc không cần viết rõ

        public string HashPassword(string password)
        {
            // Sử dụng BCrypt để băm mật khẩu. UseSalt=true (mặc định) tự động tạo salt.
            // WorkFactor (12) điều chỉnh độ khó, tăng lên an toàn hơn nhưng tốn CPU hơn.
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public bool VerifyPassword(string rawPassword, string hashedPassword)
        {
            // Sử dụng BCrypt để xác thực mật khẩu thô so với hash.
            return BCrypt.Net.BCrypt.Verify(rawPassword, hashedPassword);
        }
    }
}