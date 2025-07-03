namespace BUS.Utilities
{
    /// <summary>
    /// Interface cho dịch vụ băm và xác thực mật khẩu.
    /// </summary>
    public interface IPasswordHasherService
    {
        /// <summary>
        /// Băm mật khẩu thô.
        /// </summary>
        /// <param name="password">Mật khẩu dạng chuỗi (chưa băm).</param>
        /// <returns>Chuỗi mật khẩu đã được băm.</returns>
        string HashPassword(string password);

        /// <summary>
        /// Xác thực mật khẩu thô so với mật khẩu đã băm.
        /// </summary>
        /// <param name="rawPassword">Mật khẩu thô (chưa băm) nhập bởi người dùng.</param>
        /// <param name="hashedPassword">Mật khẩu đã băm lưu trong cơ sở dữ liệu.</param>
        /// <returns>True nếu mật khẩu khớp, False nếu không.</returns>
        bool VerifyPassword(string rawPassword, string hashedPassword);
    }
}