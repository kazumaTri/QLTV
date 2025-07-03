// --- USING ---
using BUS; // Đảm bảo có các using cần thiết cho BUS và DTO
using DTO;
using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices; // <<< THÊM using này cho P/Invoke
using System.IO; // <<< Cần cho Path và File

// Namespace của project GUI của bạn
namespace GUI
{
    // partial class để cho phép định nghĩa class frmLogin được chia thành nhiều file (frmLogin.cs và frmLogin.Designer.cs)
    public partial class frmLogin : MaterialForm // Kế thừa từ MaterialForm và là partial
    {
        // --- P/Invoke Khai báo cho việc bo tròn góc ---
        // Các hàm này được sử dụng để gọi các API của Windows
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);
        // --- Kết thúc P/Invoke ---


        // --- DEPENDENCIES (Được inject qua Constructor) ---
        private readonly IBUSNguoiDung _busNguoiDung; // Để xử lý nghiệp vụ đăng nhập
        private readonly IServiceProvider _serviceProvider; // Để lấy các Forms khác (như frmMain, frmResetPassword) qua DI

        // --- CONSTRUCTOR (Hàm khởi tạo Form Login) ---
        // *** NHẬN DEPENDENCIES QUA CONSTRUCTOR (SỬ DỤNG DEPENDENCY INJECTION) ***
        public frmLogin(IBUSNguoiDung busNguoiDung, IServiceProvider serviceProvider)
        {
            InitializeComponent(); // Khởi tạo các control UI (nằm trong Designer.cs) - PHẢI GỌI ĐẦU TIÊN

            // Gán dependencies nhận được từ DI Container
            _busNguoiDung = busNguoiDung ?? throw new ArgumentNullException(nameof(busNguoiDung));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)); // Đảm bảo ServiceProvider không null

            // Cấu hình MaterialSkin
            InitializeMaterialSkin();

            // *** GỌI HÀM BO TRÒN GÓC SAU KHI KHỞI TẠO UI ***
            // Đảm bảo các control đã được khởi tạo (sau InitializeComponent) trước khi gọi
            ApplyRoundedCorners();

            // Gắn sự kiện FormClosing (nếu chưa có trong Designer.cs)
            // Đảm bảo sự kiện này được đăng ký để xử lý việc đóng form đúng cách
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLogin_FormClosing);
        }

        // --- Phương thức áp dụng bo tròn góc ---
        // Phương thức này sử dụng P/Invoke để thay đổi hình dạng của cửa sổ Form
        private void ApplyRoundedCorners()
        {
            try
            {
                // Lấy handle (hWnd) của cửa sổ Form hiện tại
                IntPtr hWnd = this.Handle;

                // Tạo một vùng bo tròn với bán kính 25x25
                // nLeftRect, nTopRect, nRightRect, nBottomRect xác định kích thước của vùng
                // this.Width và this.Height là kích thước hiện tại của Form
                IntPtr hRgn = CreateRoundRectRgn(0, 0, this.Width, this.Height, 25, 25); // <<< Điều chỉnh 25, 25 để thay đổi độ cong

                // Áp dụng vùng bo tròn này cho cửa sổ Form
                // bRedraw = true để yêu cầu hệ thống vẽ lại cửa sổ
                int result = SetWindowRgn(hWnd, hRgn, true);

                // Kiểm tra xem API SetWindowRgn có thành công không
                if (result == 0) // 0 thường là lỗi (tùy thuộc vào tài liệu chi tiết của API)
                {
                    // Lấy mã lỗi Win32 cuối cùng nếu có lỗi
                    int error = Marshal.GetLastWin32Error();
                    Debug.WriteLine($"Lỗi khi SetWindowRgn: {error}");
                    // Có thể hiển thị thông báo lỗi hoặc xử lý khác tùy yêu cầu
                }
                else
                {
                    Debug.WriteLine("Đã áp dụng góc bo tròn cho Form.");
                }
            }
            catch (Exception ex)
            {
                // Bắt các Exception khác có thể xảy ra trong quá trình P/Invoke
                Debug.WriteLine($"Lỗi khi áp dụng góc bo tròn: {ex.Message}");
                // Có thể hiển thị thông báo lỗi cho người dùng nếu cần
            }
        }
        // --- Kết thúc phương thức bo tròn góc ---


        // --- Phương thức cấu hình MaterialSkin ---
        private void InitializeMaterialSkin()
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            // Thiết lập chủ đề và bảng màu
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT; // Hoặc MaterialSkinManager.Themes.DARK
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Indigo500, Primary.Indigo700, // Màu chính (Toolbar, Sidebar)
                Primary.Indigo100,                   // Màu chính nhạt
                Accent.Pink200,                      // Màu nhấn (Button, Checkbox, Switch...)
                TextShade.WHITE                      // Màu chữ trên nền màu chính
            );
            // Hoặc bạn có thể sử dụng một bảng màu khác:
            /*
            materialSkinManager.ColorScheme = new ColorScheme(
                 Primary.BlueGrey800, Primary.BlueGrey900,
                 Primary.BlueGrey500, Accent.LightBlue200,
                 TextShade.WHITE
             );
             */
        }

        // --- EVENT HANDLERS ---

        // Event handler khi Form được tải (Load)
        // Đây là nơi lý tưởng để đặt logic tải ảnh nền
        private void frmLogin_Load(object sender, EventArgs e)
        {
            // --- TẢI ẢNH TỪ THƯ MỤC "Images" ---
            // Đoạn code này chạy khi Form được tải lên và hiển thị,
            // đảm bảo PictureBox đã được khởi tạo.
            try
            {
                // Tên file ảnh nền
                string imageName = "Thu_vien.jpg"; // <<< THAY TÊN FILE ẢNH THỰC TẾ CỦA BẠN

                // Lấy đường dẫn đầy đủ đến file ảnh
                // AppDomain.CurrentDomain.BaseDirectory là thư mục chứa file .exe
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imagePath = Path.Combine(appDirectory, "Images", imageName);

                // Ghi log đường dẫn tìm ảnh (hữu ích cho debug)
                Debug.WriteLine($"Đang tìm ảnh nền tại: {imagePath}");

                // Kiểm tra xem file ảnh có tồn tại không
                if (File.Exists(imagePath))
                {
                    // Tải ảnh từ file và gán cho PictureBox
                    // Sử dụng 'using' để đảm bảo file handle được giải phóng ngay sau khi tải
                    // Tạo một bản sao của ảnh (Bitmap) để PictureBox không khóa file gốc
                    using (var img = Image.FromFile(imagePath))
                    {
                        this.pictureBoxLoginImage.Image = new Bitmap(img);
                    }
                    Debug.WriteLine($"Đã tải ảnh nền thành công từ: {imagePath}");
                }
                else
                {
                    // Xử lý nếu không tìm thấy file ảnh
                    Debug.WriteLine($"LỖI: Không tìm thấy ảnh nền tại đường dẫn: {imagePath}");
                    // Có thể đặt ảnh mặc định hoặc chỉ đặt màu nền báo lỗi
                    this.pictureBoxLoginImage.BackColor = Color.DarkSlateGray; // Ví dụ: đặt màu nền xám tối
                    // Optional: Hiển thị thông báo cho người dùng
                    // MessageBox.Show($"Không tìm thấy file ảnh nền: {imageName}", "Lỗi Tải Ảnh", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi khác có thể xảy ra khi tải ảnh (file hỏng, không có quyền, v.v.)
                Debug.WriteLine($"LỖI khi tải ảnh nền '{pictureBoxLoginImage.Name}': {ex.Message}");
                // Đặt màu nền báo lỗi nếu không tải được ảnh
                this.pictureBoxLoginImage.BackColor = Color.IndianRed; // Ví dụ: màu đỏ
                // Optional: Hiển thị thông báo lỗi chi tiết cho người dùng
                MessageBox.Show($"Không thể tải ảnh nền. Chi tiết: {ex.Message}", "Lỗi Tải Ảnh", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // --- KẾT THÚC TẢI ẢNH ---

            // Focus vào ô tên đăng nhập khi form mở lên để người dùng có thể nhập ngay
            this.ActiveControl = txtUsername;
        }


        // Event handler cho nút Đăng nhập
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim(); // Trim() để loại bỏ khoảng trắng ở đầu và cuối
            string password = txtPassword.Text;

            // Kiểm tra rỗng
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                // Sử dụng MaterialMessageBox để hiển thị thông báo theo phong cách Material Design
                MaterialMessageBox.Show(this, "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus(); // Trả focus về ô tên đăng nhập
                return; // Dừng xử lý
            }

            // Vô hiệu hóa UI tạm thời trong lúc xử lý
            btnLogin.Enabled = false;
            btnThoat.Enabled = false;
            btnQuenMatKhau.Enabled = false;
            txtUsername.Enabled = false;
            txtPassword.Enabled = false;
            chkShowPassword.Enabled = false;
            this.UseWaitCursor = true; // Thay đổi con trỏ chuột thành chờ

            NguoiDungDTO? loggedInUser = null; // DTO để lưu thông tin người dùng nếu đăng nhập thành công
            try
            {
                // Gọi BUS layer để xử lý logic đăng nhập (có thể là asynchronous)
                loggedInUser = await _busNguoiDung.DangNhapAsync(username, password);
            }
            catch (Exception ex)
            {
                // Bắt các lỗi hệ thống (ví dụ: mất kết nối DB)
                Debug.WriteLine($"Login Exception for '{username}': {ex}");
                MaterialMessageBox.Show(this, $"Đã xảy ra lỗi trong quá trình đăng nhập:\n{ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Kích hoạt lại UI (luôn thực hiện dù có lỗi hay không, trừ khi form bị Dispose)
                if (!this.IsDisposed) // Kiểm tra form còn tồn tại không
                {
                    btnLogin.Enabled = true;
                    btnThoat.Enabled = true;
                    btnQuenMatKhau.Enabled = true;
                    txtUsername.Enabled = true;
                    txtPassword.Enabled = true;
                    chkShowPassword.Enabled = true;
                    this.UseWaitCursor = false; // Trả lại con trỏ chuột bình thường
                }
            }

            // Xử lý kết quả sau khi gọi BUS
            if (loggedInUser != null)
            {
                // Đăng nhập thành công
                Debug.WriteLine($"Đăng nhập thành công cho: {loggedInUser.TenDangNhap}");
                try
                {
                    // Mở Form chính (frmMain) sử dụng DI Container
                    // Điều này đảm bảo frmMain cũng nhận được các dependencies của nó
                    var mainForm = _serviceProvider.GetRequiredService<frmMain>();
                    mainForm.SetLoggedInUser(loggedInUser); // Truyền thông tin người dùng sang Form chính (cần có phương thức này trong frmMain)

                    this.Hide(); // Ẩn form login hiện tại
                    mainForm.Show(); // Hiển thị form main
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu không thể khởi tạo hoặc hiển thị frmMain
                    Debug.WriteLine($"Lỗi khi khởi tạo hoặc hiển thị frmMain: {ex}");
                    MaterialMessageBox.Show(this, $"Không thể khởi tạo giao diện chính: {ex.Message}", "Lỗi nghiêm trọng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit(); // Thoát ứng dụng nếu không thể mở form chính
                }
            }
            else
            {
                // Đăng nhập thất bại (thông tin không đúng)
                // Chỉ hiển thị nếu không có lỗi hệ thống ở khối try...catch trên
                if (!this.IsDisposed)
                {
                    MaterialMessageBox.Show(this, "Tên đăng nhập hoặc mật khẩu không đúng.", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Clear(); // Xóa nội dung ô mật khẩu để nhập lại
                    txtPassword.Focus(); // Đặt focus lại vào ô mật khẩu
                }
            }
        }

        // Event handler cho nút Thoát
        private void btnThoat_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại xác nhận thoát
            var confirmResult = MaterialMessageBox.Show(this, "Bạn có chắc chắn muốn thoát ứng dụng?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmResult == DialogResult.Yes)
            {
                Application.Exit(); // Thoát toàn bộ ứng dụng một cách an toàn
            }
        }

        // Event handler cho nút/link Quên mật khẩu
        private void btnQuenMatKhau_Click(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Nút/Link Quên mật khẩu được nhấn.");
                // Lấy form ResetPassword từ DI Container
                // Điều này yêu cầu frmResetPassword phải được đăng ký trong DI Container
                var resetForm = _serviceProvider.GetRequiredService<frmResetPassword>();
                Debug.WriteLine("Đã tạo frmResetPassword thành công.");

                // Hiển thị form đặt lại mật khẩu dưới dạng Dialog
                // showDialog(this) để form mới hiện ở trung tâm của form Login và khóa form Login
                resetForm.ShowDialog(this);
                Debug.WriteLine($"frmResetPassword đã đóng với DialogResult: {resetForm.DialogResult}");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu không tạo được form ResetPassword (ví dụ: thiếu đăng ký DI, lỗi trong constructor của resetForm)
                Debug.WriteLine($"LỖI khi mở frmResetPassword: {ex}");
                MessageBox.Show($"Không thể mở chức năng đặt lại mật khẩu. Vui lòng liên hệ quản trị viên.\nChi tiết lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event handler cho sự kiện FormClosing (khi form sắp đóng)
        private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Logic này giúp ứng dụng thoát hoàn toàn khi form Login đóng
            // chỉ khi không có form nào khác đang hiển thị.
            // Điều này hữu ích khi bạn mở form Login từ Program.cs, rồi mở form Main,
            // sau đó đóng form Login. Nếu không có logic này, ứng dụng vẫn chạy vì form Main còn mở.
            // Nếu bạn đóng form Main trước, rồi đóng Login, nó sẽ thoát bình thường.

            // e.CloseReason == CloseReason.UserClosing: Kiểm tra xem người dùng có chủ động đóng form không (ví dụ: nhấn nút X)
            // Application.OpenForms.Cast<Form>().Count(form => form.Visible) <= 1:
            // Đếm số lượng form đang MỞ VÀ HIỂN THỊ. Nếu chỉ có form Login đang hiển thị (<= 1),
            // tức là không có form Main hay form khác nào đang được người dùng nhìn thấy, thì thoát hẳn.

            if (e.CloseReason == CloseReason.UserClosing && Application.OpenForms.Cast<Form>().Count(form => form.Visible) <= 1)
            {
                // Application.Exit() sẽ kết thúc tất cả các luồng và thoát ứng dụng
                Application.Exit();
            }
            // Nếu có form khác đang hiển thị, hoặc lý do đóng không phải do người dùng, form sẽ không thoát hẳn
            // (e.Cancel = true có thể cần ở đây tùy logic, nhưng với Application.Exit() ở trên thì thường không cần)
        }

        // Event handler khi nhấn phím trong ô Mật khẩu
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            // Nếu người dùng nhấn phím Enter
            if (e.KeyCode == Keys.Enter)
            {
                // Và nút Đăng nhập đang được kích hoạt
                if (btnLogin.Enabled)
                {
                    // Kích hoạt sự kiện Click của nút Đăng nhập
                    btnLogin.PerformClick();
                    // Ngăn không cho ký tự Enter xuất hiện trong textbox và ngăn xử lý tiếp theo của phím Enter
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }

        // Event handler khi check/uncheck ô Hiện mật khẩu
        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            // Thay đổi thuộc tính PasswordChar của MaterialTextBox2
            // Nếu checkbox được check, PasswordChar là '\0' (hiển thị ký tự thực)
            // Nếu checkbox không được check, PasswordChar là '●' (ký tự ẩn)
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '●';
        }

        // --- Các phương thức và event handler không cần thiết khác có thể thêm vào đây ---

    } // Kết thúc class frmLogin
} // Kết thúc namespace GUI