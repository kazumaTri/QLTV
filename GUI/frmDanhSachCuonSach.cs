// File: GUI/frmDanhSachCuonSach.cs
// Project/Namespace: GUI

// --- USING DIRECTIVES ---
using BUS; // Cần cho IBUSCuonSach
using DTO; // Cần cho CuonSachDTO
using MaterialSkin.Controls; // Cần cho MaterialForm, MaterialMessageBox
using System; // Cần cho EventArgs, Exception, ArgumentNullException
using System.Collections.Generic; // Cần cho List
using System.Diagnostics; // Cần cho Debug
using System.Linq; // Cần cho LINQ (Select, ToList)
using System.Threading.Tasks; // Cần cho async/await Task
using System.Windows.Forms; // Cần cho Form, DataGridView, Cursor, MessageBoxButtons, MessageBoxIcon

namespace GUI
{
    /// <summary>
    /// Form hiển thị danh sách các cuốn sách cụ thể (CuonSach)
    /// thuộc về một ấn bản sách (Sach) nhất định.
    /// </summary>
    public partial class frmDanhSachCuonSach : MaterialForm // Kế thừa từ MaterialForm nếu bạn dùng MaterialSkin
    {
        // --- DEPENDENCIES & STATE ---
        private readonly IBUSCuonSach _busCuonSach; // Dependency Injection cho BUS layer
        private readonly int _sachId; // ID của ấn bản sách (Sach) đang xem
        private readonly string _tenSachAnBan; // Tên của ấn bản sách để hiển thị trên tiêu đề

        /// <summary>
        /// Constructor của Form.
        /// </summary>
        /// <param name="busCuonSach">Instance của IBUSCuonSach được inject.</param>
        /// <param name="sachId">ID của ấn bản sách (Sach) cần hiển thị danh sách cuốn sách.</param>
        /// <param name="tenSachAnBan">Tên hiển thị của ấn bản sách (để đặt tiêu đề Form).</param>
        /// <exception cref="ArgumentNullException">Ném ra nếu busCuonSach là null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Ném ra nếu sachId không hợp lệ (<= 0).</exception>
        public frmDanhSachCuonSach(IBUSCuonSach busCuonSach, int sachId, string tenSachAnBan)
        {
            InitializeComponent(); // Khởi tạo các control từ Designer

            // Gán dependencies và kiểm tra null/invalid
            _busCuonSach = busCuonSach ?? throw new ArgumentNullException(nameof(busCuonSach));
            if (sachId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sachId), "ID Sách không hợp lệ.");
            }
            _sachId = sachId;
            _tenSachAnBan = string.IsNullOrWhiteSpace(tenSachAnBan) ? $"ID Sách: {sachId}" : tenSachAnBan; // Đảm bảo có tên hiển thị

            // Cập nhật tiêu đề Form để hiển thị thông tin sách đang xem
            this.Text = $"Các cuốn sách của: {_tenSachAnBan}";
        }

        /// <summary>
        /// Xử lý sự kiện khi Form được tải lần đầu.
        /// Gọi phương thức để tải danh sách cuốn sách.
        /// </summary>
        private async void frmDanhSachCuonSach_Load(object sender, EventArgs e)
        {
            // Chỉ thực hiện tải dữ liệu khi không ở chế độ Design Mode của Visual Studio
            if (!this.DesignMode)
            {
                await LoadCuonSachListAsync();
            }
        }

        /// <summary>
        /// Tải danh sách các cuốn sách (CuonSach) từ BUS layer dựa trên _sachId
        /// và hiển thị lên DataGridView.
        /// </summary>
        private async Task LoadCuonSachListAsync()
        {
            // Kiểm tra lại _sachId (mặc dù đã kiểm tra ở constructor)
            if (_sachId <= 0)
            {
                MaterialMessageBox.Show(this, "ID Sách không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Xóa dữ liệu cũ và hiển thị con trỏ chờ
            dgvCuonSachList.DataSource = null;
            this.Cursor = Cursors.WaitCursor;
            btnDong.Enabled = false; // Tạm thời vô hiệu hóa nút Đóng

            try
            {
                Debug.WriteLine($"frmDanhSachCuonSach: Loading CuonSach for Sach ID: {_sachId}");

                // Gọi phương thức từ BUS layer để lấy danh sách CuonSachDTO
                List<CuonSachDTO> cuonSachList = await _busCuonSach.GetCuonSachBySachIdAsync(_sachId);

                // Xử lý trường hợp BUS trả về null (mặc dù không nên)
                if (cuonSachList == null)
                {
                    cuonSachList = new List<CuonSachDTO>();
                    Debug.WriteLine($"frmDanhSachCuonSach: WARNING - GetCuonSachBySachIdAsync returned null for Sach ID: {_sachId}");
                }

                // Tạo danh sách mới chỉ chứa các cột cần hiển thị trên DataGridView
                // Sử dụng TinhTrangText đã được BUS tính toán sẵn
                var displayList = cuonSachList.Select(cs => new
                {
                    MaCuonSach = cs.MaCuonSach ?? "N/A", // Hiển thị "N/A" nếu Mã cuốn sách null
                    TinhTrang = cs.TinhTrangText // Sử dụng trường Text đã được BUS xử lý
                    // Bạn có thể thêm các cột khác vào đây nếu muốn hiển thị thêm thông tin
                    // Ví dụ: Id = cs.Id (thường ẩn đi)
                }).ToList();


                // Gán danh sách đã xử lý làm nguồn dữ liệu cho DataGridView
                dgvCuonSachList.DataSource = displayList;

                // Gọi hàm cấu hình cột sau khi đã gán DataSource
                SetupDataGridViewColumns();

                Debug.WriteLine($"frmDanhSachCuonSach: Loaded {displayList.Count} CuonSach items for Sach ID: {_sachId}.");

            }
            catch (Exception ex) // Bắt lỗi từ BUS hoặc các lỗi hệ thống khác
            {
                Debug.WriteLine($"*** ERROR loading CuonSach list in frmDanhSachCuonSach (SachID: {_sachId}): {ex}");
                // Hiển thị thông báo lỗi thân thiện cho người dùng
                MaterialMessageBox.Show(this, $"Lỗi khi tải danh sách cuốn sách: {ex.Message}", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Luôn trả lại con trỏ chuột và bật lại nút Đóng sau khi hoàn tất (thành công hoặc lỗi)
                this.Cursor = Cursors.Default;
                btnDong.Enabled = true;
            }
        }

        /// <summary>
        /// Cấu hình các cột hiển thị trong DataGridView (dgvCuonSachList).
        /// Đặt tiêu đề, độ rộng, và chế độ tự động giãn cột.
        /// </summary>
        private void SetupDataGridViewColumns()
        {
            // Kiểm tra xem DataGridView và các cột đã được tạo chưa
            if (dgvCuonSachList == null || dgvCuonSachList.Columns.Count == 0)
            {
                Debug.WriteLine("SetupDataGridViewColumns skipped: DataGridView or Columns not ready.");
                return;
            }

            try
            {
                // Cấu hình cột "MaCuonSach"
                if (dgvCuonSachList.Columns.Contains("MaCuonSach"))
                {
                    dgvCuonSachList.Columns["MaCuonSach"].HeaderText = "Mã Cuốn Sách"; // Đặt tiêu đề cột
                    dgvCuonSachList.Columns["MaCuonSach"].Width = 200; // Đặt độ rộng cố định
                    dgvCuonSachList.Columns["MaCuonSach"].DataPropertyName = "MaCuonSach"; // Đảm bảo đúng tên thuộc tính nguồn
                }
                else { Debug.WriteLine("SetupDataGridViewColumns: Column 'MaCuonSach' not found."); }

                // Cấu hình cột "TinhTrang" (hiển thị TinhTrangText)
                if (dgvCuonSachList.Columns.Contains("TinhTrang")) // Tên cột có thể là "TinhTrang" do mapping từ displayList
                {
                    dgvCuonSachList.Columns["TinhTrang"].HeaderText = "Tình Trạng"; // Đặt tiêu đề cột
                    // Cho phép cột này tự động giãn ra để lấp đầy khoảng trống còn lại
                    dgvCuonSachList.Columns["TinhTrang"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dgvCuonSachList.Columns["TinhTrang"].DataPropertyName = "TinhTrang"; // Đảm bảo đúng tên thuộc tính nguồn ("TinhTrang" trong displayList)
                }
                else { Debug.WriteLine("SetupDataGridViewColumns: Column 'TinhTrang' not found."); }


                // --- Các cấu hình chung cho DataGridView ---
                dgvCuonSachList.ReadOnly = true; // Không cho phép người dùng sửa trực tiếp trên lưới
                dgvCuonSachList.AllowUserToAddRows = false; // Không cho phép thêm dòng mới
                dgvCuonSachList.AllowUserToDeleteRows = false; // Không cho phép xóa dòng
                dgvCuonSachList.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Chọn cả dòng khi click
                dgvCuonSachList.MultiSelect = false; // Không cho phép chọn nhiều dòng cùng lúc
                //dgvCuonSachList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Chế độ tự động giãn cột (có thể đặt None nếu muốn tự set Width)
                dgvCuonSachList.RowHeadersVisible = false; // Ẩn cột header mặc định bên trái

                Debug.WriteLine("SetupDataGridViewColumns completed.");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu có vấn đề khi cấu hình cột
                Debug.WriteLine($"*** ERROR setting up DataGridView columns in frmDanhSachCuonSach: {ex}");
                // Có thể hiển thị thông báo lỗi nếu cần, nhưng thường chỉ cần log
                // MaterialMessageBox.Show(this, $"Lỗi cấu hình hiển thị: {ex.Message}", "Lỗi Giao Diện", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        /// <summary>
        /// Xử lý sự kiện khi người dùng nhấn nút "Đóng".
        /// Đóng Form hiện tại.
        /// </summary>
        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close(); // Đóng Form (do DialogResult=Cancel đã được đặt ở Designer)
        }
    }
}
