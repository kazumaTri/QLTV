// Trong file GUI/frmResetPassword.cs
using BUS; // Namespace chứa IBUSNguoiDung
using System;
using System.Diagnostics; // Cho Debug.WriteLine
using System.Windows.Forms;
using System.Threading.Tasks; // Cho Task

namespace GUI // Đảm bảo đúng namespace của project GUI
{
    // Giả sử bạn đặt tên Form là frmResetPassword trong trình thiết kế
    // Nếu bạn dùng MaterialSkin, hãy kế thừa từ MaterialForm thay vì Form
    public partial class frmResetPassword : Form
    {
        // Biến private để giữ tham chiếu đến BUS layer service
        private readonly IBUSNguoiDung _busNguoiDung;

        // === Constructor cần được sửa đổi để nhận IBUSNguoiDung ===
        // Điều này yêu cầu bạn phải đăng ký IBUSNguoiDung và frmResetPassword
        // trong Dependency Injection Container ở Program.cs
        public frmResetPassword(IBUSNguoiDung busNguoiDung)
        {
            InitializeComponent(); // Hàm này do Designer tạo ra, giữ nguyên

            // Lưu lại service được inject vào
            _busNguoiDung = busNguoiDung ?? throw new ArgumentNullException(nameof(busNguoiDung));

            // (Tùy chọn) Cài đặt thêm cho Form nếu cần
            this.AcceptButton = btnXacNhanReset; // Cho phép nhấn Enter để Xác nhận
            this.CancelButton = btnHuyBoReset;   // Cho phép nhấn Esc để Hủy bỏ
            this.StartPosition = FormStartPosition.CenterParent; // Hiển thị form ở giữa form cha
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // Ngăn thay đổi kích thước
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        // === Xử lý sự kiện khi nhấn nút Xác nhận ===
        private async void btnXacNhanReset_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu từ các TextBox (giả sử tên là txtTenDangNhapReset, txtMatKhauMoiReset, txtXacNhanMatKhauReset)
            string tenDangNhap = txtTenDangNhapReset.Text.Trim();
            string matKhauMoi = txtMatKhauMoiReset.Text; // Không .Trim() mật khẩu
            string xacNhanMatKhau = txtXacNhanMatKhauReset.Text; // Không .Trim() mật khẩu

            // --- Các kiểm tra đầu vào cơ bản ---
            if (string.IsNullOrWhiteSpace(tenDangNhap))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenDangNhapReset.Focus(); // Đưa con trỏ vào ô tên đăng nhập
                return; // Dừng xử lý
            }
            if (string.IsNullOrEmpty(matKhauMoi))
            {
                MessageBox.Show("Mật khẩu mới không được để trống.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhauMoiReset.Focus(); // Đưa con trỏ vào ô mật khẩu mới
                return; // Dừng xử lý
            }
            if (matKhauMoi.Length < 6) // Ví dụ kiểm tra độ dài tối thiểu
            {
                MessageBox.Show("Mật khẩu mới phải có ít nhất 6 ký tự.", "Mật khẩu yếu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhauMoiReset.Focus();
                return;
            }
            if (matKhauMoi != xacNhanMatKhau)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtXacNhanMatKhauReset.Focus(); // Đưa con trỏ vào ô xác nhận
                txtXacNhanMatKhauReset.SelectAll(); // Chọn hết text để người dùng dễ sửa
                return; // Dừng xử lý
            }
            // (Tùy chọn) Bạn có thể thêm các kiểm tra độ mạnh mật khẩu phức tạp hơn ở đây

            // --- Gọi lớp BUS để thực hiện đặt lại mật khẩu ---
            try
            {
                // Vô hiệu hóa nút và hiển thị con trỏ chờ để người dùng biết đang xử lý
                btnXacNhanReset.Enabled = false;
                btnHuyBoReset.Enabled = false; // Cũng nên vô hiệu hóa nút Hủy
                this.UseWaitCursor = true;

                Debug.WriteLine($"[ResetPassword] Attempting to reset password for user: {tenDangNhap}"); // Ghi log (chỉ thấy khi chạy Debug)

                // Gọi phương thức bất đồng bộ từ BUS layer
                bool ketQua = await _busNguoiDung.DatLaiMatKhauAsync(tenDangNhap, matKhauMoi);

                // Xử lý kết quả trả về từ BUS
                if (ketQua)
                {
                    Debug.WriteLine($"[ResetPassword] SUCCESS for user: {tenDangNhap}"); // Ghi log
                    MessageBox.Show("Đặt lại mật khẩu thành công!\nVui lòng đăng nhập lại bằng mật khẩu mới.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK; // Đặt kết quả trả về của Dialog là OK
                    this.Close(); // Đóng form đặt lại mật khẩu
                }
                else
                {
                    Debug.WriteLine($"[ResetPassword] FAILED for user: {tenDangNhap}"); // Ghi log
                    // Thông báo lỗi chung chung hơn cho người dùng
                    MessageBox.Show("Đặt lại mật khẩu thất bại.\nNguyên nhân có thể do tên đăng nhập không đúng hoặc đã có lỗi xảy ra.\nVui lòng kiểm tra lại thông tin và thử lại.", "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTenDangNhapReset.Focus(); // Focus lại vào ô tên đăng nhập để người dùng sửa
                }
            }
            catch (Exception ex) // Bắt các lỗi không mong muốn khác
            {
                Debug.WriteLine($"[ResetPassword] EXCEPTION during password reset for {tenDangNhap}: {ex}"); // Ghi log lỗi chi tiết
                // Hiển thị thông báo lỗi chung cho người dùng
                MessageBox.Show($"Đã xảy ra lỗi không mong muốn trong quá trình đặt lại mật khẩu.\nVui lòng thử lại sau.\nChi tiết lỗi: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            finally // Khối này luôn được thực thi dù có lỗi hay không
            {
                // Kích hoạt lại các nút và tắt con trỏ chờ
                btnXacNhanReset.Enabled = true;
                btnHuyBoReset.Enabled = true;
                this.UseWaitCursor = false;
            }
        }

        // === Xử lý sự kiện khi nhấn nút Hủy bỏ ===
        private void btnHuyBoReset_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel; // Đặt kết quả trả về là Cancel
            this.Close(); // Đóng form
        }

    } // End class frmResetPassword
} // End namespace GUI