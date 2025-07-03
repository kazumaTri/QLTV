// --- USING ---
using BUS;
using DTO;
using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
// using GUI.Interfaces; // Đảm bảo using đúng namespace chứa IRequiresDataLoading

namespace GUI
{
    // --- INTERFACES ---
    /// <summary>
    /// Interface cho các UserControl cần tải dữ liệu bất đồng bộ khi được hiển thị.
    /// </summary>
    public interface IRequiresDataLoading
    {
        /// <summary>
        /// Phương thức để tải dữ liệu cần thiết cho UserControl.
        /// </summary>
        Task InitializeDataAsync();
    }

    /// <summary>
    /// Interface cho các UserControl có thể yêu cầu đóng chính nó.
    /// </summary>
    public interface IRequestClose
    {
        /// <summary>
        /// Sự kiện được kích hoạt khi UserControl muốn được đóng.
        /// </summary>
        event EventHandler? RequestClose;
    }

    public partial class frmMain : MaterialForm
    {
        // --- P/Invoke (giữ nguyên) ---
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        private static extern bool DeleteObject(IntPtr hObject);

        // --- DEPENDENCIES (giữ nguyên) ---
        private readonly IServiceProvider _serviceProvider;
        private readonly IBUSNguoiDung _busNguoiDung;
        private readonly IBUSThongBao _busThongBao;
        private NguoiDungDTO? _loggedInUser;
        private readonly MaterialSkinManager _materialSkinManager;

        // --- UI STATE (giữ nguyên) ---
        private List<MaterialButton> _menuButtons = new List<MaterialButton>();
        private MaterialButton? _currentSelectedButton = null;
        private UserControl? _currentLoadedControl = null;
        private IntPtr _currentRegion = IntPtr.Zero;

        // --- NOTIFICATION STATE (giữ nguyên) ---
        private List<ThongBaoDTO> _currentNotifications = new List<ThongBaoDTO>();

        // --- PATHS (giữ nguyên) ---
        private readonly string _iconFolderPath;

        // --- LAYOUT CONSTANTS ---
        private const int FixedMenuWidth = 260;
        private const int CornerRadius = 20;
        private const int NotificationPanelWidth = 380;
        private const int NotificationPanelMaxHeight = 450;

        // --- CONSTRUCTOR ---
        public frmMain(IServiceProvider serviceProvider, IBUSNguoiDung busNguoiDung, IBUSThongBao busThongBao)
        {
            InitializeComponent();

            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _busNguoiDung = busNguoiDung ?? throw new ArgumentNullException(nameof(busNguoiDung));
            _busThongBao = busThongBao ?? throw new ArgumentNullException(nameof(busThongBao));

            _materialSkinManager = MaterialSkinManager.Instance;
            _materialSkinManager.AddFormToManage(this);

            _materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            _materialSkinManager.ColorScheme = new ColorScheme(
                Primary.BlueGrey800,
                Primary.BlueGrey900,
                Primary.BlueGrey500,
                Accent.LightBlue200,
                TextShade.WHITE
            );

            // Xác định đường dẫn Icons (giữ nguyên logic)
            _iconFolderPath = Path.Combine(Application.StartupPath, "..", "..", "..", "Icons");
            if (!Directory.Exists(_iconFolderPath))
            {
                _iconFolderPath = Path.Combine(Application.StartupPath, "Icons");
                if (!Directory.Exists(_iconFolderPath))
                {
                    _iconFolderPath = Application.StartupPath;
                }
            }

            // Gán sự kiện (giữ nguyên)
            this.SizeChanged += new System.EventHandler(this.frmMain_SizeChanged);
            InitializeNotificationComponents();

            // *** THÊM: Gán màu nền cho flowLayoutPanelMenu khi khởi tạo ***
            if (flowLayoutPanelMenu != null)
            {
                flowLayoutPanelMenu.BackColor = _materialSkinManager.BackgroundColor;
            }
        }

        // --- Phương thức áp dụng bo tròn góc (giữ nguyên) ---
        private void ApplyRoundedCorners()
        {
            if (!this.IsHandleCreated || this.DesignMode) return;
            try
            {
                if (_currentRegion != IntPtr.Zero) DeleteObject(_currentRegion);
                _currentRegion = CreateRoundRectRgn(0, 0, this.Width, this.Height, CornerRadius, CornerRadius);
                int result = SetWindowRgn(this.Handle, _currentRegion, true);
                if (result == 0)
                {
                    int error = Marshal.GetLastWin32Error(); Debug.WriteLine($"Lỗi khi SetWindowRgn cho frmMain: {error}");
                    DeleteObject(_currentRegion); _currentRegion = IntPtr.Zero;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi khi áp dụng góc bo tròn cho frmMain: {ex.Message}");
                if (_currentRegion != IntPtr.Zero) { DeleteObject(_currentRegion); _currentRegion = IntPtr.Zero; }
            }
        }

        // Set thông tin người dùng (giữ nguyên logic)
        public async void SetLoggedInUser(NguoiDungDTO user)
        {
            _loggedInUser = user ?? throw new ArgumentNullException(nameof(user));
            DisplayUserInfo();
            CreateMainMenu();
            // Gọi LoadUserControlByNameAsync để tải Trang Chủ ban đầu
            await LoadUserControlByNameAsync("trangchu");
            // Chọn nút Trang Chủ sau khi đã tải xong
            SelectMenuButtonByTag("trangchu");
            try
            {
                await LoadNotificationsAsync();
                if (timerNotifications != null && !timerNotifications.Enabled)
                {
                    timerNotifications.Start();
                }
            }
            catch (Exception exLoadNotify) { Debug.WriteLine($"Error initial notification load: {exLoadNotify.Message}"); }
        }

        // --- FORM LOAD EVENT ---
        private void frmMain_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                if (panelMenu != null)
                {
                    panelMenu.Width = FixedMenuWidth;
                    // Màu nền của panelMenu (Card) có thể giữ mặc định hoặc đặt trong suốt
                    // panelMenu.BackColor = Color.Transparent; // Hoặc giữ màu của theme
                }
                // *** THÊM: Đặt lại màu nền cho flowLayoutPanelMenu phòng trường hợp theme thay đổi ***
                if (flowLayoutPanelMenu != null)
                {
                    flowLayoutPanelMenu.BackColor = _materialSkinManager.BackgroundColor;
                }
                AdjustHeaderLayout();
                ApplyRoundedCorners();
                // Không cần gọi LoadUserControl ở đây nữa, đã gọi trong SetLoggedInUser
            }
        }

        // --- UI HELPERS ---
        private void DisplayUserInfo()
        {
            if (lblUserInfo == null) return;
            if (_loggedInUser != null)
            {
                string displayName = !string.IsNullOrEmpty(_loggedInUser.TenHienThi) ? _loggedInUser.TenHienThi : _loggedInUser.TenDangNhap;
                lblUserInfo.FontType = MaterialSkinManager.fontType.Subtitle1;
                lblUserInfo.Text = $"Chào, {displayName ?? "User"}";
            }
            else { lblUserInfo.Text = "Khách"; }
            AdjustHeaderLayout();
        }

        // --- MENU MANAGEMENT ---
        private void CreateMainMenu()
        {
            // *** THAY ĐỔI: Kiểm tra flowLayoutPanelMenu thay vì panelMenu ***
            if (flowLayoutPanelMenu == null) { Debug.WriteLine("ERROR: flowLayoutPanelMenu is null."); return; }
            if (_loggedInUser == null) { Debug.WriteLine("ERROR: _loggedInUser is null."); return; }

            // *** THAY ĐỔI: Tạm dừng layout và xóa control của flowLayoutPanelMenu ***
            flowLayoutPanelMenu.SuspendLayout();
            flowLayoutPanelMenu.Controls.Clear();
            _menuButtons.Clear();
            Image? defaultIcon = LoadIconFromFile("default_icon_24.png");

            string currentUserRole = _loggedInUser?.TenNhomNguoiDung ?? string.Empty;
            if (string.IsNullOrEmpty(currentUserRole))
            {
                Debug.WriteLine("WARNING: Current user role is null or empty.");
                MaterialMessageBox.Show(this, "Không thể xác định vai trò người dùng.", "Lỗi Phân Quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                flowLayoutPanelMenu.ResumeLayout(); return; // Resume layout của flowLayoutPanelMenu
            }

            // Danh sách menu items (giữ nguyên logic)
            var menuItems = new List<dynamic> {
                 new { Text = " Trang Chủ", Name = "trangchu", IconFile = "home.png", AllowedRoles = new[] {"Quản trị viên", "Thủ thư", "Độc giả"} },
                 new { Text = "-", Name = "-", IconFile = "-", AllowedRoles = new[] {"Quản trị viên", "Thủ thư", "Độc giả"} },
                 new { Text = " Loại Độc Giả", Name = "qlloaidocgia", IconFile = "user_category.png", AllowedRoles = new[] {"Quản trị viên", "Thủ thư"} },
                 new { Text = " Độc Giả", Name = "qldocgia", IconFile = "reader.png", AllowedRoles = new[] {"Quản trị viên", "Thủ thư"} },
                 new { Text = "-", Name = "-", IconFile = "-", AllowedRoles = new[] {"Quản trị viên", "Thủ thư"} },
                 new { Text = " Thể Loại", Name = "qltheloai", IconFile = "category.png", AllowedRoles = new[] {"Quản trị viên", "Thủ thư"} },
                 new { Text = " Tác Giả", Name = "qltacgia", IconFile = "author.png", AllowedRoles = new[] {"Quản trị viên", "Thủ thư"} },
                 new { Text = " Nhà Xuất Bản", Name = "qlnxb", IconFile = "publisher.png", AllowedRoles = new[] {"Quản trị viên", "Thủ thư"} },
                 new { Text = " Tựa Sách", Name = "qltuasach", IconFile = "bookshelf.png", AllowedRoles = new[] {"Quản trị viên", "Thủ thư"} },
                 new { Text = " Sách", Name = "qlsach", IconFile = "book.png", AllowedRoles = new[] {"Quản trị viên", "Thủ thư", "Độc giả"} },
                 new { Text = " Cuốn Sách", Name = "qlcuonsach", IconFile = "barcode.png", AllowedRoles = new[] {"Quản trị viên", "Thủ thư"} },
                 new { Text = "-", Name = "-", IconFile = "-", AllowedRoles = new[] {"Quản trị viên", "Thủ thư"} },
                 new { Text = " Nhập Sách", Name = "qlnhapsach", IconFile = "import.png", AllowedRoles = new[] {"Quản trị viên", "Thủ thư"} },
                 new { Text = " Mượn/Trả Sách", Name = "qlphieumuontra", IconFile = "borrow_return.png", AllowedRoles = new[] {"Quản trị viên", "Thủ thư"} },
                 new { Text = "-", Name = "-", IconFile = "-", AllowedRoles = new[] {"Quản trị viên", "Thủ thư"} },
                 new { Text = " Người Dùng", Name = "qlnguoidung", IconFile = "admin_user.png", AllowedRoles = new[] {"Quản trị viên"} },
                 new { Text = " Nhóm Người Dùng", Name = "qlnhomnguoidung", IconFile = "user_group.png", AllowedRoles = new[] {"Quản trị viên"} },
                 new { Text = " Quản lý Thông báo", Name = "qlthongbao", IconFile = "notification_settings.png", AllowedRoles = new[] {"Quản trị viên", "Thủ thư"} },
                 new { Text = " Cấu Hình", Name = "cauHinh", IconFile = "settings.png", AllowedRoles = new[] {"Quản trị viên"} },
                 new { Text = "-", Name = "-", IconFile = "-", AllowedRoles = new[] {"Quản trị viên"} },
                 new { Text = " Thống Kê", Name = "thongke", IconFile = "statistics.png", AllowedRoles = new[] {"Quản trị viên", "Thủ thư"} },
                 new { Text = "-", Name = "-", IconFile = "-", AllowedRoles = new[] {"Quản trị viên", "Thủ thư", "Độc giả"} },
                 new { Text = " Đăng Xuất", Name = "dangxuat", IconFile = "logout.png", AllowedRoles = new[] {"Quản trị viên", "Thủ thư", "Độc giả"} }
            };

            AddMenuAppTitle("THƯ VIỆN"); // Thêm vào flowLayoutPanelMenu

            bool addedDocGiaGroup = false, addedSachGroup = false, addedNghiepVuGroup = false, addedHeThongGroup = false, addedBaoCaoGroup = false;

            foreach (var item in menuItems)
            {
                bool allowed = ((string[])item.AllowedRoles).Contains(currentUserRole, StringComparer.OrdinalIgnoreCase);
                if (allowed)
                {
                    if (item.Name == "qlloaidocgia" && !addedDocGiaGroup) { AddMenuGroupTitle("QUẢN LÝ ĐỘC GIẢ"); addedDocGiaGroup = true; }
                    else if (item.Name == "qltheloai" && !addedSachGroup) { AddMenuGroupTitle("QUẢN LÝ SÁCH"); addedSachGroup = true; }
                    else if (item.Name == "qlnhapsach" && !addedNghiepVuGroup) { AddMenuGroupTitle("NGHIỆP VỤ"); addedNghiepVuGroup = true; }
                    else if (item.Name == "qlnguoidung" && !addedHeThongGroup) { AddMenuGroupTitle("HỆ THỐNG"); addedHeThongGroup = true; }
                    else if (item.Name == "thongke" && !addedBaoCaoGroup) { AddMenuGroupTitle("BÁO CÁO"); addedBaoCaoGroup = true; }

                    if (item.Text == "-") { AddSeparator(); } // Thêm vào flowLayoutPanelMenu
                    else
                    {
                        Image? icon = LoadIconFromFile(item.IconFile) ?? defaultIcon;
                        var button = CreateMenuButton(item.Text, item.Name, item.IconFile, icon);
                        // *** THAY ĐỔI: Thêm button vào flowLayoutPanelMenu ***
                        flowLayoutPanelMenu.Controls.Add(button);
                        _menuButtons.Add(button);
                        // Không cần BringToFront trong FlowLayoutPanel
                    }
                }
            }

            UpdateMenuButtonAppearance();
            // *** THAY ĐỔI: Resume layout của flowLayoutPanelMenu ***
            flowLayoutPanelMenu.ResumeLayout(true);
        }

        private void AddMenuAppTitle(string title)
        {
            // *** THAY ĐỔI: Kiểm tra và thêm vào flowLayoutPanelMenu ***
            if (flowLayoutPanelMenu == null) return;
            var lblAppTitle = new MaterialLabel
            {
                Text = title.ToUpper(),
                // *** THAY ĐỔI: Không dùng Dock, đặt Width và AutoSize ***
                // Dock = DockStyle.Top,
                Width = flowLayoutPanelMenu.ClientSize.Width - 10, // Trừ padding/margin
                AutoSize = false, // Đặt chiều cao cố định
                Height = 60,
                FontType = MaterialSkinManager.fontType.H6,
                HighEmphasis = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 15, 0, 15),
                Margin = new Padding(5, 10, 5, 10),
            };
            flowLayoutPanelMenu.Controls.Add(lblAppTitle);
        }

        private MaterialButton CreateMenuButton(string text, string controlName, string iconFileName, Image? icon = null)
        {
            var button = new MaterialButton
            {
                Text = text,
                Tag = new { ControlName = controlName, OriginalText = text.TrimStart(), IconFileName = iconFileName },
                // *** THAY ĐỔI: Không dùng Dock ***
                // Dock = DockStyle.Top,
                Height = 52,
                AutoSize = false, // Giữ chiều cao cố định
                // *** THAY ĐỔI: Đặt Width cho button ***
                // Width sẽ được đặt khi thêm vào FlowLayoutPanel hoặc giữ cố định nếu muốn
                // Width = flowLayoutPanelMenu.ClientSize.Width - 20, // Ví dụ: Trừ margin
                Type = MaterialButton.MaterialButtonType.Text,
                HighEmphasis = false,
                UseAccentColor = false,
                Icon = icon,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 10, 0),
                Margin = new Padding(10, 4, 10, 4) // Giữ Margin
            };
            button.Click += MenuItem_Click;
            button.MouseEnter += MenuButton_MouseEnter;
            button.MouseLeave += MenuButton_MouseLeave;

            // *** THÊM: Đặt chiều rộng nút sau khi khởi tạo (quan trọng cho FlowLayoutPanel) ***
            // Cần đảm bảo flowLayoutPanelMenu đã được khởi tạo
            if (flowLayoutPanelMenu != null)
            {
                // Trừ đi Margin ngang của nút và Padding ngang của FlowLayoutPanel
                int availableWidth = flowLayoutPanelMenu.ClientSize.Width - button.Margin.Horizontal - flowLayoutPanelMenu.Padding.Horizontal;
                button.Width = Math.Max(100, availableWidth); // Đặt chiều rộng tối thiểu
            }
            else
            {
                button.Width = 200; // Giá trị mặc định nếu flowLayoutPanelMenu chưa sẵn sàng
            }

            return button;
        }

        private Image? LoadIconFromFile(string iconFileName)
        {
            // Giữ nguyên logic
            if (string.IsNullOrEmpty(iconFileName) || iconFileName == "-") return null;
            string iconPath = Path.Combine(_iconFolderPath, iconFileName);
            try
            {
                if (File.Exists(iconPath))
                {
                    byte[] fileBytes = File.ReadAllBytes(iconPath);
                    using (var ms = new MemoryStream(fileBytes))
                    {
                        Image originalImage = Image.FromStream(ms);
                        Bitmap bmp = new Bitmap(originalImage.Width, originalImage.Height, PixelFormat.Format32bppArgb);
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.Clear(Color.Transparent);
                            g.DrawImage(originalImage, 0, 0, originalImage.Width, originalImage.Height);
                        }
                        originalImage.Dispose();
                        return bmp;
                    }
                }
                else { Debug.WriteLine($"Icon file not found: {iconPath}"); return null; }
            }
            catch (Exception ex) { Debug.WriteLine($"Error loading icon file '{iconFileName}': {ex.Message}"); return null; }
        }

        private void AddSeparator()
        {
            // *** THAY ĐỔI: Kiểm tra và thêm vào flowLayoutPanelMenu ***
            if (flowLayoutPanelMenu == null) return;
            var separator = new Panel
            {
                Height = 1,
                // *** THAY ĐỔI: Không dùng Dock, đặt Width ***
                // Dock = DockStyle.Top,
                Width = flowLayoutPanelMenu.ClientSize.Width - 40, // Trừ margin
                BackColor = _materialSkinManager.DividersColor,
                Margin = new Padding(20, 10, 20, 10)
            };
            flowLayoutPanelMenu.Controls.Add(separator);
        }

        private void AddMenuGroupTitle(string title)
        {
            // *** THAY ĐỔI: Kiểm tra và thêm vào flowLayoutPanelMenu ***
            if (flowLayoutPanelMenu == null) return;
            var lblGroupTitle = new MaterialLabel
            {
                Text = title.ToUpper(),
                // *** THAY ĐỔI: Không dùng Dock, đặt Width ***
                // Dock = DockStyle.Top,
                Width = flowLayoutPanelMenu.ClientSize.Width - 30, // Trừ margin/padding
                FontType = MaterialSkinManager.fontType.Button,
                HighEmphasis = false,
                ForeColor = _materialSkinManager.ColorScheme.AccentColor,
                Padding = new Padding(20, 12, 0, 6),
                Margin = new Padding(10, 8, 10, 2),
                AutoSize = false,
                Height = 35,
                TextAlign = ContentAlignment.MiddleLeft
            };
            flowLayoutPanelMenu.Controls.Add(lblGroupTitle);
        }

        private async void MenuItem_Click(object? sender, EventArgs e)
        {
            // Giữ nguyên logic
            if (sender is MaterialButton clickedButton && clickedButton.Tag is { } tagData)
            {
                string? controlName = null;
                try { controlName = (tagData as dynamic)?.ControlName?.ToString(); } catch { }

                if (!string.IsNullOrEmpty(controlName))
                {
                    if (controlName.Equals("dangxuat", StringComparison.OrdinalIgnoreCase)) { PerformLogout(); }
                    else
                    {
                        // Chọn nút trước khi tải để người dùng thấy phản hồi ngay
                        SelectMenuButton(clickedButton);
                        // Gọi hàm tải UserControl
                        await LoadUserControlByNameAsync(controlName);
                    }
                }
            }
        }

        private void SelectMenuButton(MaterialButton? buttonToSelect)
        {
            // *** THAY ĐỔI: Cập nhật màu nền dựa trên flowLayoutPanelMenu ***
            if (_currentSelectedButton != null && _currentSelectedButton != buttonToSelect)
            {
                _currentSelectedButton.HighEmphasis = false;
                _currentSelectedButton.UseAccentColor = false;
                _currentSelectedButton.Type = MaterialButton.MaterialButtonType.Text;
                _currentSelectedButton.BackColor = flowLayoutPanelMenu?.BackColor ?? _materialSkinManager.BackgroundColor; // Lấy màu nền của FlowLayoutPanel
                _currentSelectedButton.Invalidate();
            }
            _currentSelectedButton = buttonToSelect;
            if (_currentSelectedButton != null)
            {
                _currentSelectedButton.HighEmphasis = true;
                _currentSelectedButton.UseAccentColor = true;
                _currentSelectedButton.Type = MaterialButton.MaterialButtonType.Contained;
                _currentSelectedButton.Invalidate();
            }
        }

        public void SelectMenuButtonByTag(string controlName)
        {
            // Giữ nguyên logic
            MaterialButton? buttonToSelect = null;
            try
            {
                buttonToSelect = _menuButtons.FirstOrDefault(btn =>
                    btn?.Tag is { } tag &&
                    (tag as dynamic)?.ControlName?.Equals(controlName, StringComparison.OrdinalIgnoreCase) == true);
            }
            catch (Exception ex) { Debug.WriteLine($"Error finding button by tag '{controlName}': {ex.Message}"); }

            if (buttonToSelect != null) { SelectMenuButton(buttonToSelect); }
            else { Debug.WriteLine($"Button with tag '{controlName}' not found."); SelectMenuButton(null); }
        }

        private void MenuButton_MouseEnter(object? sender, EventArgs e)
        {
            // *** THAY ĐỔI: Cập nhật màu nền dựa trên flowLayoutPanelMenu ***
            if (sender is MaterialButton hoveredButton && hoveredButton != _currentSelectedButton)
            {
                Color menuBg = flowLayoutPanelMenu?.BackColor ?? _materialSkinManager.BackgroundColor; // Lấy màu nền FlowLayoutPanel
                int adjustment = _materialSkinManager.Theme == MaterialSkinManager.Themes.DARK ? 20 : -15;
                Color hoverColor = Color.FromArgb(
                    Math.Max(0, Math.Min(255, menuBg.R + adjustment)),
                    Math.Max(0, Math.Min(255, menuBg.G + adjustment)),
                    Math.Max(0, Math.Min(255, menuBg.B + adjustment))
                );
                hoveredButton.BackColor = hoverColor;
                hoveredButton.Invalidate();
            }
        }

        private void MenuButton_MouseLeave(object? sender, EventArgs e)
        {
            // *** THAY ĐỔI: Cập nhật màu nền dựa trên flowLayoutPanelMenu ***
            if (sender is MaterialButton hoveredButton && hoveredButton != _currentSelectedButton)
            {
                hoveredButton.BackColor = flowLayoutPanelMenu?.BackColor ?? _materialSkinManager.BackgroundColor; // Lấy màu nền FlowLayoutPanel
                hoveredButton.Invalidate();
            }
        }


        // --- USER CONTROL LOADING ---
        /// <summary>
        /// Lấy và hiển thị UserControl dựa trên tên (key).
        /// </summary>
        /// <param name="controlName">Tên (key) của UserControl cần tải.</param>
        public async Task LoadUserControlByNameAsync(string controlName)
        {
            UserControl? uc = null;
            string normalizedName = controlName?.Trim().ToLowerInvariant() ?? "";

            if (panelContent == null || _serviceProvider == null)
            {
                Debug.WriteLine("ERROR: panelContent or ServiceProvider is null.");
                MaterialMessageBox.Show(this, "Lỗi khởi tạo giao diện.", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SelectMenuButtonByTag(""); // Bỏ chọn nút menu hiện tại
                return;
            }

            this.UseWaitCursor = true; // Hiển thị con trỏ chờ
            panelContent.Enabled = false; // Vô hiệu hóa panel nội dung tạm thời

            try
            {
                // Lấy instance UserControl từ Dependency Injection Container
                switch (normalizedName)
                {
                    case "trangchu": uc = _serviceProvider.GetRequiredService<ucTrangChu>(); break;
                    case "qlloaidocgia": uc = _serviceProvider.GetRequiredService<ucQuanLyLoaiDocGia>(); break;
                    case "qldocgia": uc = _serviceProvider.GetRequiredService<ucQuanLyDocGia>(); break;
                    case "qltheloai": uc = _serviceProvider.GetRequiredService<ucQuanLyTheLoai>(); break;
                    case "qltacgia": uc = _serviceProvider.GetRequiredService<ucQuanLyTacGia>(); break;
                    case "qlnxb": uc = _serviceProvider.GetRequiredService<ucQuanLyNhaXuatBan>(); break;
                    case "qltuasach": uc = _serviceProvider.GetRequiredService<ucQuanLyTuaSach>(); break; // <<< ucQuanLyTuaSach đây
                    case "qlsach": uc = _serviceProvider.GetRequiredService<ucQuanLySach>(); break;
                    case "qlcuonsach": uc = _serviceProvider.GetRequiredService<ucQuanLyCuonSach>(); break;
                    case "qlnhapsach": uc = _serviceProvider.GetRequiredService<ucQuanLyNhapSach>(); break;
                    case "qlphieumuontra": uc = _serviceProvider.GetRequiredService<ucQuanLyPhieuMuonTra>(); break;
                    case "qlnguoidung": uc = _serviceProvider.GetRequiredService<ucQuanLyNguoiDung>(); break;
                    case "qlnhomnguoidung": uc = _serviceProvider.GetRequiredService<ucQuanLyNhomNguoiDung>(); break;
                    case "qlthongbao": uc = _serviceProvider.GetRequiredService<ucQuanLyThongBao>(); break;
                    case "cauhinh": uc = _serviceProvider.GetRequiredService<ucCaiDat>(); break;
                    case "thongke": uc = _serviceProvider.GetRequiredService<ucThongKe>(); break;
                    default:
                        string buttonTextNotFound = GetOriginalMenuButtonText(_currentSelectedButton) ?? controlName;
                        MaterialMessageBox.Show(this, $"Chức năng '{buttonTextNotFound}' chưa được đăng ký hoặc tên không đúng.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SelectMenuButtonByTag(""); // Bỏ chọn nút menu
                        uc = null; // Không có UserControl nào được tải
                        break;
                }
            }
            catch (InvalidOperationException ioEx) // Lỗi không tìm thấy Service (chưa đăng ký DI)
            {
                string buttonTextDIError = GetOriginalMenuButtonText(_currentSelectedButton) ?? controlName;
                MaterialMessageBox.Show(this, $"Lỗi khởi tạo chức năng '{buttonTextDIError}'.\nChi tiết: {ioEx.Message}\n\nVui lòng kiểm tra lại cấu hình Dependency Injection.", "Lỗi Cấu Hình", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"DI Error loading '{controlName}': {ioEx}");
                SelectMenuButtonByTag(""); // Bỏ chọn nút menu
                uc = null;
            }
            catch (Exception ex) // Các lỗi khác khi tạo instance
            {
                string buttonTextGeneralError = GetOriginalMenuButtonText(_currentSelectedButton) ?? controlName;
                MaterialMessageBox.Show(this, $"Lỗi không xác định khi tải chức năng '{buttonTextGeneralError}': {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"Error creating UC instance '{controlName}': {ex}");
                SelectMenuButtonByTag(""); // Bỏ chọn nút menu
                uc = null;
            }

            // Gọi hàm helper để thực sự hiển thị và tải dữ liệu (nếu cần)
            await LoadUserControlAsync(uc);

            // Kích hoạt lại panel nội dung và trả lại con trỏ chuột
            panelContent.Enabled = true;
            this.UseWaitCursor = false;
        }

        /// <summary>
        /// Hàm helper để xóa control cũ, thêm control mới và gọi InitializeDataAsync nếu cần.
        /// </summary>
        /// <param name="userControl">UserControl cần hiển thị.</param>
        private async Task LoadUserControlAsync(UserControl? userControl)
        {
            if (this.panelContent == null) { Debug.WriteLine("CRITICAL ERROR: panelContent is null in LoadUserControlAsync."); return; }

            // Dọn dẹp control cũ
            var controlToDispose = _currentLoadedControl;
            _currentLoadedControl = null; // Đặt là null trước khi Dispose

            UnsubscribeFromControlEvents(controlToDispose);
            this.panelContent.Controls.Clear(); // Xóa tất cả controls cũ trong panel
            if (controlToDispose != null && !controlToDispose.IsDisposed)
            {
                controlToDispose.Dispose(); // Giải phóng tài nguyên control cũ
                Debug.WriteLine($"Disposed previous control: {controlToDispose.GetType().Name}");
            }

            // Thêm và khởi tạo control mới (nếu có)
            if (userControl != null)
            {
                if (userControl.IsDisposed)
                {
                    Debug.WriteLine($"ERROR: UserControl {userControl.GetType().Name} was disposed before adding.");
                    return;
                }

                userControl.Dock = DockStyle.Fill; // Đảm bảo control mới fill panel
                this.panelContent.Controls.Add(userControl); // Thêm control mới vào panel
                userControl.BringToFront(); // Đưa control mới lên trên cùng
                _currentLoadedControl = userControl; // Lưu lại tham chiếu control hiện tại
                SubscribeToControlEvents(userControl); // Đăng ký sự kiện cho control mới
                Debug.WriteLine($"Added and subscribed to new control: {userControl.GetType().Name}");

                // *** QUAN TRỌNG: Kiểm tra và gọi InitializeDataAsync ***
                if (userControl is IRequiresDataLoading dataLoader)
                {
                    Debug.WriteLine($"Control {userControl.GetType().Name} requires data loading. Calling InitializeDataAsync...");
                    try
                    {
                        // Gọi phương thức tải dữ liệu của UserControl
                        await dataLoader.InitializeDataAsync();
                        Debug.WriteLine($"InitializeDataAsync completed for {userControl.GetType().Name}.");
                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi nếu InitializeDataAsync thất bại
                        Debug.WriteLine($"*** ERROR during InitializeDataAsync for {userControl.GetType().Name}: {ex}");
                        MaterialMessageBox.Show(this, $"Lỗi tải dữ liệu cho chức năng '{GetOriginalMenuButtonText(_currentSelectedButton)}':\n{ex.Message}\n\nVui lòng thử lại hoặc liên hệ quản trị viên.", "Lỗi Tải Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // Dọn dẹp control bị lỗi
                        UnsubscribeFromControlEvents(userControl);
                        this.panelContent.Controls.Remove(userControl);
                        if (!userControl.IsDisposed) userControl.Dispose();
                        if (_currentLoadedControl == userControl) _currentLoadedControl = null; // Đảm bảo không giữ tham chiếu lỗi

                        // Có thể quay về trang chủ hoặc bỏ chọn menu
                        SelectMenuButtonByTag(""); // Bỏ chọn nút menu hiện tại
                        // await LoadUserControlByNameAsync("trangchu"); // Tùy chọn: Tự động quay về trang chủ
                        return; // Thoát khỏi phương thức sau khi xử lý lỗi
                    }
                }
                else
                {
                    Debug.WriteLine($"Control {userControl.GetType().Name} does not require data loading.");
                }
            }
            else
            {
                Debug.WriteLine("LoadUserControlAsync called with null UserControl.");
            }
        }


        // --- Event Subscription/Unsubscription (giữ nguyên logic) ---
        private void SubscribeToControlEvents(UserControl? control)
        {
            if (control == null || control.IsDisposed) return;
            if (control is IRequestClose closable) { closable.RequestClose -= HandleUserControlRequestClose; closable.RequestClose += HandleUserControlRequestClose; }
            if (control is ucTrangChu trangChu) { trangChu.RequestNavigate -= HandleTrangChuNavigateRequest; trangChu.RequestNavigate += HandleTrangChuNavigateRequest; }
        }
        private void UnsubscribeFromControlEvents(UserControl? control)
        {
            if (control == null) return;
            // Sử dụng try-catch để tránh lỗi nếu sự kiện đã bị hủy đăng ký trước đó
            if (control is IRequestClose closable) { try { closable.RequestClose -= HandleUserControlRequestClose; } catch { } }
            if (control is ucTrangChu trangChu) { try { trangChu.RequestNavigate -= HandleTrangChuNavigateRequest; } catch { } }
        }
        private async void HandleUserControlRequestClose(object? sender, EventArgs e)
        {
            // Khi UserControl yêu cầu đóng, quay về Trang Chủ
            await LoadUserControlByNameAsync("trangchu");
            SelectMenuButtonByTag("trangchu");
        }
        private async void HandleTrangChuNavigateRequest(string targetScreenKey)
        {
            // Khi Trang Chủ yêu cầu điều hướng đến màn hình khác
            await LoadUserControlByNameAsync(targetScreenKey);
            SelectMenuButtonByTag(targetScreenKey);
        }


        // --- Cập nhật giao diện nút menu, Logout, Layout, Closing (giữ nguyên logic) ---
        private void UpdateMenuButtonAppearance()
        {
            // *** THAY ĐỔI: Cập nhật màu nền dựa trên flowLayoutPanelMenu ***
            if (flowLayoutPanelMenu == null || !_menuButtons.Any()) return;
            flowLayoutPanelMenu.SuspendLayout(); // Tạm dừng layout của FlowLayoutPanel
            foreach (var btn in _menuButtons)
            {
                if (btn.IsDisposed) continue;
                string originalText = GetOriginalMenuButtonText(btn) ?? ""; btn.Text = " " + originalText;
                btn.ImageAlign = ContentAlignment.MiddleLeft; btn.TextAlign = ContentAlignment.MiddleLeft;

                // Cập nhật lại Width của button phòng trường hợp ClientSize của FlowLayoutPanel thay đổi
                int availableWidth = flowLayoutPanelMenu.ClientSize.Width - btn.Margin.Horizontal - flowLayoutPanelMenu.Padding.Horizontal;
                btn.Width = Math.Max(100, availableWidth);

                if (btn != _currentSelectedButton)
                {
                    btn.HighEmphasis = false; btn.UseAccentColor = false; btn.Type = MaterialButton.MaterialButtonType.Text;
                    btn.BackColor = flowLayoutPanelMenu.BackColor; // Lấy màu nền FlowLayoutPanel
                }
                else
                {
                    btn.HighEmphasis = true; btn.UseAccentColor = true; btn.Type = MaterialButton.MaterialButtonType.Contained;
                }
                btn.Invalidate();
            }
            flowLayoutPanelMenu.ResumeLayout(true); // Tiếp tục layout của FlowLayoutPanel
        }
        private string? GetOriginalMenuButtonText(MaterialButton? btn) { if (btn?.Tag == null) return null; try { return (btn.Tag as dynamic)?.OriginalText?.ToString(); } catch { return null; } }
        private string? GetOriginalMenuButtonIconName(MaterialButton? btn) { if (btn?.Tag == null) return null; try { return (btn.Tag as dynamic)?.IconFileName?.ToString(); } catch { return null; } }
        private void btnLogout_Click(object? sender, EventArgs e) { PerformLogout(); }
        private void PerformLogout()
        {
            var result = MaterialMessageBox.Show(this, "Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question, UseRichTextBox: false);
            if (result == DialogResult.Yes)
            {
                _loggedInUser = null; _currentSelectedButton = null;
                UnsubscribeFromControlEvents(_currentLoadedControl);
                if (_currentLoadedControl != null && !_currentLoadedControl.IsDisposed) _currentLoadedControl.Dispose(); _currentLoadedControl = null;
                if (timerNotifications != null && timerNotifications.Enabled) { timerNotifications.Stop(); }
                // *** THAY ĐỔI: Xóa control của flowLayoutPanelMenu ***
                if (flowLayoutPanelMenu != null) flowLayoutPanelMenu.Controls.Clear();
                if (panelContent != null) panelContent.Controls.Clear();
                _menuButtons.Clear();
                if (lblUserInfo != null) lblUserInfo.Text = ""; if (btnNotifications != null) { btnNotifications.Text = ""; btnNotifications.Icon = LoadIconFromFile("notification_read.png"); btnNotifications.BackColor = Color.Transparent; }
                if (panelNotificationList != null) panelNotificationList.Visible = false;

                this.Hide();
                var loginForm = _serviceProvider.GetRequiredService<frmLogin>();
                loginForm.Show();
            }
        }
        private void AdjustHeaderLayout()
        {
            // Giữ nguyên logic đã cập nhật
            if (panelUserInfoHeader == null || lblUserInfo == null || btnNotifications == null) return;
            try
            {
                panelUserInfoHeader.SuspendLayout();
                const int spacing = 12;
                const int rightMargin = 20;

                int btnNotifY = (panelUserInfoHeader.Height - btnNotifications.Height) / 2;
                int btnNotifX = panelUserInfoHeader.Width - btnNotifications.Width - rightMargin;
                btnNotifications.Location = new Point(btnNotifX, btnNotifY);

                lblUserInfo.AutoSize = true;
                int lblUserY = (panelUserInfoHeader.Height - lblUserInfo.Height) / 2;
                int lblUserX = btnNotifications.Left - lblUserInfo.Width - spacing;
                if (lblUserX < panelUserInfoHeader.Padding.Left) lblUserX = panelUserInfoHeader.Padding.Left;
                lblUserInfo.Location = new Point(lblUserX, lblUserY);

                panelUserInfoHeader.ResumeLayout(true);
                panelUserInfoHeader.BringToFront();
            }
            catch (Exception ex) { Debug.WriteLine($"Error in AdjustHeaderLayout: {ex.Message}"); }
        }

        private void frmMain_SizeChanged(object? sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                AdjustHeaderLayout();
                ApplyRoundedCorners();
                // *** THÊM: Cập nhật lại Width của các control trong menu khi Form resize ***
                UpdateMenuControlsWidth();
            }
        }

        // *** THÊM: Phương thức cập nhật Width cho các control trong menu ***
        private void UpdateMenuControlsWidth()
        {
            if (flowLayoutPanelMenu == null || !flowLayoutPanelMenu.IsHandleCreated) return;

            flowLayoutPanelMenu.SuspendLayout(); // Tạm dừng layout

            foreach (Control ctrl in flowLayoutPanelMenu.Controls)
            {
                if (ctrl is MaterialButton btn)
                {
                    int availableWidth = flowLayoutPanelMenu.ClientSize.Width - btn.Margin.Horizontal - flowLayoutPanelMenu.Padding.Horizontal;
                    btn.Width = Math.Max(100, availableWidth);
                }
                else if (ctrl is MaterialLabel lbl)
                {
                    // Chỉ cập nhật Width cho các label không AutoSize (tiêu đề)
                    if (!lbl.AutoSize)
                    {
                        int availableWidth = flowLayoutPanelMenu.ClientSize.Width - lbl.Margin.Horizontal - flowLayoutPanelMenu.Padding.Horizontal;
                        lbl.Width = Math.Max(100, availableWidth);
                    }
                }
                else if (ctrl is Panel separator)
                {
                    int availableWidth = flowLayoutPanelMenu.ClientSize.Width - separator.Margin.Horizontal - flowLayoutPanelMenu.Padding.Horizontal;
                    separator.Width = Math.Max(50, availableWidth);
                }
            }
            flowLayoutPanelMenu.ResumeLayout(true); // Tiếp tục layout
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Giữ nguyên logic
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var result = MaterialMessageBox.Show(this, "Bạn có chắc chắn muốn thoát?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question, UseRichTextBox: false);
                if (result == DialogResult.No) { e.Cancel = true; }
                else { CleanupBeforeExit(); }
            }
            else { CleanupBeforeExit(); }
        }

        private void CleanupBeforeExit()
        {
            // Giữ nguyên logic
            if (timerNotifications != null && timerNotifications.Enabled) timerNotifications.Stop();
            UnsubscribeFromControlEvents(_currentLoadedControl);
            if (_currentLoadedControl != null && !_currentLoadedControl.IsDisposed) _currentLoadedControl.Dispose();
            if (_currentRegion != IntPtr.Zero) DeleteObject(_currentRegion);
        }


        // --- NOTIFICATION HANDLING (giữ nguyên logic) ---
        private void InitializeNotificationComponents()
        {
            if (this.timerNotifications != null) { this.timerNotifications.Interval = 300000; this.timerNotifications.Tick -= timerNotifications_Tick; this.timerNotifications.Tick += timerNotifications_Tick; } else { Debug.WriteLine("ERROR: timerNotifications is null!"); }
            if (this.btnNotifications != null)
            {
                this.btnNotifications.Click -= btnNotifications_Click; this.btnNotifications.Click += btnNotifications_Click;
                this.btnNotifications.Icon = LoadIconFromFile("notification_read.png");
                //this.btnNotifications.Type = MaterialButton.MaterialButtonType.Icon;
                this.btnNotifications.AutoSize = false;
                this.btnNotifications.Size = new Size(40, 40);
            }
            else { Debug.WriteLine("ERROR: btnNotifications is null!"); }

            if (panelNotificationList == null) { Debug.WriteLine("ERROR: panelNotificationList is null!"); }
            else { panelNotificationList.Padding = new Padding(10); }

            if (flowLayoutPanelNotifications == null) { Debug.WriteLine("ERROR: flowLayoutPanelNotifications is null!"); }

            this.Click -= HandleFormClickToHideNotification; this.Click += HandleFormClickToHideNotification;
            SubscribeClickRecursively(this.panelMenu); // Vẫn cần để bắt click trên Card nền
            SubscribeClickRecursively(this.flowLayoutPanelMenu); // *** THÊM: Bắt click trên FlowLayoutPanel ***
            SubscribeClickRecursively(this.panelContent);
        }

        private void SubscribeClickToHide(Control? control) { if (control != null) { control.Click -= HandleFormClickToHideNotification; control.Click += HandleFormClickToHideNotification; } }
        private void SubscribeClickRecursively(Control parentControl) { if (parentControl == null) return; SubscribeClickToHide(parentControl); foreach (Control childControl in parentControl.Controls) { SubscribeClickRecursively(childControl); } }

        private async void timerNotifications_Tick(object? sender, EventArgs e) { if (_loggedInUser != null) { await LoadNotificationsAsync(); } else { timerNotifications?.Stop(); } }

        private async Task LoadNotificationsAsync()
        {
            if (_busThongBao == null) { Debug.WriteLine("LoadNotificationsAsync Error: _busThongBao is null."); return; }
            try
            {
                _currentNotifications = await _busThongBao.GetActiveNotificationsAsync();
                if (this.IsHandleCreated && !this.IsDisposed) { this.Invoke((MethodInvoker)delegate { UpdateNotificationButtonState(); if (panelNotificationList != null && panelNotificationList.Visible) { UpdateNotificationPanel(); } }); }
            }
            catch (Exception ex) { Debug.WriteLine($"Error loading notifications: {ex.Message}"); }
        }

        private void UpdateNotificationButtonState()
        {
            if (btnNotifications == null) return;
            int unreadCount = _currentNotifications?.Count ?? 0;
            if (unreadCount > 0)
            {
                btnNotifications.Icon = LoadIconFromFile("notification_read.png");
                btnNotifications.Text = unreadCount.ToString();
            }
            else
            {
                btnNotifications.Icon = LoadIconFromFile("notification_read.png");
                btnNotifications.Text = "";
            }
            btnNotifications.Invalidate();
        }

        private void btnNotifications_Click(object? sender, EventArgs e)
        {
            if (panelNotificationList == null || btnNotifications == null || panelUserInfoHeader == null) return;

            if (panelNotificationList.Visible)
            {
                panelNotificationList.Visible = false;
            }
            else
            {
                Point buttonScreenLocation = btnNotifications.PointToScreen(Point.Empty);
                int xPos = buttonScreenLocation.X + btnNotifications.Width - panelNotificationList.Width;
                int yPos = buttonScreenLocation.Y + btnNotifications.Height + 5;

                Screen currentScreen = Screen.FromPoint(Cursor.Position);
                if (xPos < currentScreen.WorkingArea.Left) xPos = currentScreen.WorkingArea.Left + 5;
                if (xPos + panelNotificationList.Width > currentScreen.WorkingArea.Right) xPos = currentScreen.WorkingArea.Right - panelNotificationList.Width - 5;
                if (yPos + panelNotificationList.Height > currentScreen.WorkingArea.Bottom) yPos = currentScreen.WorkingArea.Bottom - panelNotificationList.Height - 5;
                if (yPos < currentScreen.WorkingArea.Top) yPos = currentScreen.WorkingArea.Top + 5;

                panelNotificationList.Location = this.PointToClient(new Point(xPos, yPos));
                panelNotificationList.Size = new Size(NotificationPanelWidth, NotificationPanelMaxHeight);

                panelNotificationList.BringToFront();
                UpdateNotificationPanel();
                panelNotificationList.Visible = true;
            }
        }

        private void UpdateNotificationPanel()
        {
            if (flowLayoutPanelNotifications == null || panelNotificationList == null) return;
            flowLayoutPanelNotifications.SuspendLayout();
            flowLayoutPanelNotifications.Controls.Clear();

            if (_currentNotifications == null || !_currentNotifications.Any())
            {
                var lblEmpty = new MaterialLabel
                {
                    Text = "Không có thông báo mới.",
                    AutoSize = false,
                    Width = flowLayoutPanelNotifications.ClientSize.Width - 10,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Top, // Dock vẫn ổn trong trường hợp này
                    Padding = new Padding(10, 30, 10, 30),
                    Margin = new Padding(0, 10, 0, 0),
                    FontType = MaterialSkinManager.fontType.Body1
                };
                flowLayoutPanelNotifications.Controls.Add(lblEmpty);
            }
            else
            {
                foreach (var notification in _currentNotifications.OrderByDescending(n => n.NgayTao))
                {
                    var itemCard = new MaterialCard
                    {
                        Width = flowLayoutPanelNotifications.ClientSize.Width - 6,
                        AutoSize = true,
                        MinimumSize = new Size(0, 65),
                        Margin = new Padding(0, 0, 0, 8),
                        Padding = new Padding(12),
                        Tag = notification,
                        MouseState = MouseState.HOVER
                    };

                    var lblTitle = new MaterialLabel
                    {
                        Text = notification.TieuDe ?? "Không có tiêu đề",
                        FontType = MaterialSkinManager.fontType.Subtitle2,
                        HighEmphasis = true,
                        Dock = DockStyle.Top,
                        AutoSize = false,
                        Width = itemCard.ClientSize.Width, // Cần tính toán lại khi card thay đổi size?
                        Height = 20,
                        Padding = new Padding(0, 0, 0, 4)
                    };

                    var lblDate = new MaterialLabel
                    {
                        Text = $"{notification.MucDo ?? "Thông tin"} - {notification.NgayTao:dd/MM/yy HH:mm}",
                        FontType = MaterialSkinManager.fontType.Caption,
                        ForeColor = _materialSkinManager.TextMediumEmphasisColor,
                        Dock = DockStyle.Top,
                        AutoSize = false,
                        Width = itemCard.ClientSize.Width, // Cần tính toán lại khi card thay đổi size?
                        Height = 15,
                        TextAlign = ContentAlignment.MiddleLeft
                    };

                    itemCard.Click += NotificationItem_Click;
                    lblTitle.Click += NotificationItem_Click;
                    lblDate.Click += NotificationItem_Click;

                    itemCard.Controls.Add(lblDate);
                    itemCard.Controls.Add(lblTitle);

                    flowLayoutPanelNotifications.Controls.Add(itemCard);
                }
            }

            int requiredHeight = flowLayoutPanelNotifications.Padding.Vertical;
            foreach (Control c in flowLayoutPanelNotifications.Controls)
            {
                requiredHeight += c.Height + c.Margin.Vertical;
            }
            requiredHeight += 10;

            panelNotificationList.Height = Math.Min(NotificationPanelMaxHeight, requiredHeight + panelNotificationList.Padding.Vertical);

            flowLayoutPanelNotifications.ResumeLayout(true);

            if (flowLayoutPanelNotifications.Controls.Count > 0)
            {
                flowLayoutPanelNotifications.ScrollControlIntoView(flowLayoutPanelNotifications.Controls[0]);
            }
        }

        private void NotificationItem_Click(object? sender, EventArgs e)
        {
            // Giữ nguyên logic
            ThongBaoDTO? notification = null;
            if (sender is Control control)
            {
                Control? current = control;
                while (current != null && !(current is MaterialCard)) { current = current.Parent; }
                if (current is MaterialCard card && card.Tag is ThongBaoDTO dto) { notification = dto; }
            }

            if (notification != null)
            {
                ShowNotificationDetail(notification);
                panelNotificationList.Visible = false;
            }
            else { Debug.WriteLine("Could not retrieve notification DTO from clicked control."); }
        }

        private void ShowNotificationDetail(ThongBaoDTO notification)
        {
            // Giữ nguyên logic đã sửa
            MaterialMessageBox.Show(this,
                notification.NoiDung ?? "Không có nội dung.",
                $"Thông báo: {notification.TieuDe ?? "N/A"}",
                MessageBoxButtons.OK,
                false // useRichTextBox = false
            );
        }

        private void HandleFormClickToHideNotification(object? sender, EventArgs e)
        {
            // Giữ nguyên logic
            if (panelNotificationList != null && panelNotificationList.Visible)
            {
                Point clientCursorPos = this.PointToClient(Cursor.Position);
                bool clickedOnButton = btnNotifications != null && btnNotifications.ClientRectangle.Contains(btnNotifications.PointToClient(Cursor.Position));
                // *** THAY ĐỔI: Kiểm tra click trên panelNotificationList ***
                bool clickedOnPanel = panelNotificationList.ClientRectangle.Contains(panelNotificationList.PointToClient(Cursor.Position));

                if (!clickedOnButton && !clickedOnPanel)
                {
                    panelNotificationList.Visible = false;
                }
            }
        }

        // --- Các hàm xử lý sự kiện rỗng (giữ nguyên) ---
        private void txtPassword_KeyDown(object? sender, KeyEventArgs e) { }
        private void chkShowPassword_CheckedChanged(object? sender, EventArgs e) { }

    } // Kết thúc class frmMain
} // Kết thúc namespace
