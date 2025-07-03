// File: GUI/ucQuanLySach.cs
// Project/Namespace: GUI

// --- USING DIRECTIVES ---
using BUS; // Cần cho IBUSSach, IBUSTuaSach, IBUSCuonSach
using DTO; // Cần cho SachDTO, TuaSachDTO
using MaterialSkin.Controls; // Cần cho MaterialMessageBox, MaterialTextBox2, MaterialButton
using System; // Cần cho Exception, ArgumentNullException, InvalidOperationException, EventArgs, DateTime, int, decimal
using System.Collections.Generic; // Cần cho List
using System.Globalization; // Thêm để định dạng số (CultureInfo)
using System.Linq; // Cần cho LINQ (ToList, Any, FirstOrDefault)
using System.Threading.Tasks; // Cần cho async/await Task
using System.Windows.Forms; // Cần cho UserControl, DataGridView, Panel, MessageBoxButtons, MessageBoxIcon, Cursor, DialogResult, ComboBox
using Microsoft.Extensions.DependencyInjection; // Cần cho IServiceProvider
using System.Diagnostics; // Cho Debug

// Namespace của chính project GUI để refer các Forms/UserControl khác nếu cần
using GUI;
// Đảm bảo interface IRequiresDataLoading và IRequestClose được định nghĩa đúng (ví dụ trong frmMain.cs hoặc namespace chung)
using static GUI.frmMain; // Giả sử interfaces được định nghĩa trong frmMain

namespace GUI // Namespace của project GUI của bạn
{
    /// <summary>
    /// UserControl quản lý thông tin Sách (ấn bản cụ thể của một Tựa sách).
    /// Hiển thị danh sách, cho phép thêm, sửa, xóa vĩnh viễn thông tin sách.
    /// Có chức năng xem danh sách các cuốn sách cụ thể thuộc ấn bản đang chọn.
    /// </summary>
    public partial class ucQuanLySach : UserControl, IRequiresDataLoading
    {
        // --- DEPENDENCIES (Nhận qua Constructor Injection) ---
        private readonly IBUSSach _busSach;
        private readonly IBUSTuaSach _busTuaSach;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBUSCuonSach _busCuonSach; // Dependency cho BUS Cuốn Sách

        // --- STATE ---
        private bool _isAdding = false; // Cờ theo dõi trạng thái Thêm mới
        private List<TuaSachDTO> _tuaSachList = new List<TuaSachDTO>(); // Danh sách tựa sách cho ComboBox

        /// <summary>
        /// Sự kiện được kích hoạt khi UserControl muốn yêu cầu đóng chính nó.
        /// </summary>
        public event EventHandler? RequestClose;

        // --- CONSTRUCTOR (Sử dụng Dependency Injection) ---
        public ucQuanLySach(IBUSSach busSach, IBUSTuaSach busTuaSach, IServiceProvider serviceProvider, IBUSCuonSach busCuonSach)
        {
            InitializeComponent(); // Khởi tạo các controls từ Designer

            try
            {
                // Kiểm tra và gán từng dependency
                _busSach = busSach ?? throw new ArgumentNullException(nameof(busSach));
                _busTuaSach = busTuaSach ?? throw new ArgumentNullException(nameof(busTuaSach));
                _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
                _busCuonSach = busCuonSach ?? throw new ArgumentNullException(nameof(busCuonSach));
                Debug.WriteLine(">>> ucQuanLySach: Dependencies assigned OK.");
            }
            catch (ArgumentNullException ex) // Bắt lỗi nếu dependency bị null
            {
                // Ghi log lỗi chi tiết, bao gồm tên dependency bị lỗi
                Debug.WriteLine($"*** FATAL ERROR in ucQuanLySach Constructor: Dependency Injection Failed for '{ex.ParamName}'. Service was null. Error: {ex}");
                // Hiển thị thông báo lỗi thân thiện cho người dùng
                MaterialMessageBox.Show($"Lỗi khởi tạo dịch vụ quan trọng ('{ex.ParamName}'). Chức năng Quản lý Sách sẽ không hoạt động. Vui lòng kiểm tra cấu hình Dependency Injection trong Program.cs.", "Lỗi Dịch Vụ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Vô hiệu hóa UserControl này để ngăn lỗi tiếp theo
                this.Enabled = false;
            }
            catch (Exception ex) // Bắt các lỗi không mong muốn khác trong constructor
            {
                Debug.WriteLine($"*** UNEXPECTED FATAL ERROR in ucQuanLySach Constructor: {ex}");
                MaterialMessageBox.Show($"Lỗi không xác định trong constructor ucQuanLySach: {ex.Message}", "Lỗi Khởi Tạo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Enabled = false; // Vô hiệu hóa control
            }

            // Chỉ gán sự kiện nếu không có lỗi DI nghiêm trọng (Control vẫn Enabled)
            if (this.Enabled)
            {
                // Gắn sự kiện kiểm tra thay đổi cho các control nhập liệu
                if (txtMaSach != null) txtMaSach.TextChanged += InputField_Changed;
                else Debug.WriteLine("WARNING: txtMaSach is null during constructor.");
                if (cboTuaSach != null) cboTuaSach.SelectedValueChanged += InputField_Changed;
                else Debug.WriteLine("WARNING: cboTuaSach is null during constructor.");
                if (txtDonGia != null) txtDonGia.TextChanged += InputField_Changed;
                else Debug.WriteLine("WARNING: txtDonGia is null during constructor.");
                if (txtNamXb != null) txtNamXb.TextChanged += InputField_Changed;
                else Debug.WriteLine("WARNING: txtNamXb is null during constructor.");
                if (txtNhaXb != null) txtNhaXb.TextChanged += InputField_Changed;
                else Debug.WriteLine("WARNING: txtNhaXb is null during constructor.");
            }

            Debug.WriteLine(">>> ucQuanLySach: Constructor finished.");
        }

        // --- SỰ KIỆN LOAD USERCONTROL ---
        private void ucQuanLySach_Load(object sender, EventArgs e)
        {
            Debug.WriteLine(">>> Entering ucQuanLySach_Load...");
            // Kiểm tra xem có đang ở chế độ Design của Visual Studio hay không
            // và control có đang được bật (không bị lỗi constructor) không
            if (!this.DesignMode && this.Enabled)
            {
                // Thông báo rằng việc khởi tạo dữ liệu sẽ được gọi bởi form cha (frmMain)
                Debug.WriteLine(">>> ucQuanLySach_Load: In Runtime Mode and Enabled. Initialization will be triggered by parent (frmMain).");
            }
            else if (!this.Enabled)
            {
                // Ghi log nếu control bị vô hiệu hóa (thường do lỗi DI)
                Debug.WriteLine(">>> ucQuanLySach_Load: Skipped because control is disabled (likely due to constructor DI error).");
            }
            else
            {
                // Ghi log nếu đang ở chế độ Design
                Debug.WriteLine(">>> ucQuanLySach_Load: In Design Mode.");
            }
        }

        // --- PHƯƠNG THỨC KHỞI TẠO DỮ LIỆU (Triển khai từ IRequiresDataLoading) ---
        public async Task InitializeDataAsync()
        {
            // Log trạng thái thực tế khi vào hàm
            Debug.WriteLine($">>> Entering InitializeDataAsync (DesignMode={this.DesignMode}, Enabled={this.Enabled})");

            // Chỉ kiểm tra DesignMode, không kiểm tra Enabled nữa
            if (this.DesignMode)
            {
                Debug.WriteLine($"InitializeDataAsync skipped: DesignMode=True");
                return; // Không làm gì nếu ở chế độ Design
            }

            // Bỏ phần kiểm tra Enabled ở đây vì việc tải dữ liệu nên diễn ra
            // ngay cả khi control tạm thời bị disable bởi form cha.

            _isAdding = false; // Reset trạng thái thêm/sửa
            ClearInputFields(); // Xóa các ô nhập liệu

            Debug.WriteLine("InitializeDataAsync: Starting data loads...");
            this.Cursor = Cursors.WaitCursor; // Hiển thị con trỏ chờ
            try
            {
                // Tải dữ liệu cần thiết cho ComboBox và DataGridView
                await LoadTuaSachComboBox(); // Tải ComboBox trước
                await LoadDataGrid(); // Sau đó tải DataGridView
                // SetControlState sẽ được gọi trong finally của LoadDataGrid
                Debug.WriteLine("InitializeDataAsync: Data loads completed.");
            }
            catch (Exception ex) // Bắt lỗi trong quá trình tải dữ liệu
            {
                Debug.WriteLine($"*** FATAL ERROR during InitializeDataAsync: {ex}");
                // Đảm bảo không lỗi nếu FindForm() trả về null (dù ít khả năng)
                var owner = this.FindForm();
                MaterialMessageBox.Show(owner, $"Lỗi nghiêm trọng khi khởi tạo dữ liệu: {ex.Message}", "Lỗi Khởi Tạo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Có thể cân nhắc this.Enabled = false; ở đây nếu lỗi quá nghiêm trọng
                // this.Enabled = false;
            }
            finally
            {
                // Không cần Task.Yield() ở đây vì Cursor và SetControlState là UI operations
                this.Cursor = Cursors.Default; // Trả lại con trỏ chuột mặc định

                // Cập nhật trạng thái control cuối cùng SAU KHI đã tải và thử chọn lại dòng
                bool finalRowSelected = dgvSach?.SelectedRows.Count > 0;
                // IsEditing() có thể đã bị thay đổi bởi logic trong LoadDataGrid/SelectRowById
                // Cần kiểm tra lại _isAdding trước khi gọi IsEditing()
                bool isCurrentlyEditing = _isAdding || IsEditing();
                SetControlState(isCurrentlyEditing, finalRowSelected);

                // Nếu không có dòng nào được chọn VÀ không đang thêm/sửa -> xóa trắng input (Chỉ khi không có lỗi)
                if (!isCurrentlyEditing && (dgvSach?.Rows.Count == 0 || !finalRowSelected))
                {
                    ClearInputFields();
                }
                Debug.WriteLine("InitializeDataAsync: Exiting finally block.");
            }
            Debug.WriteLine(">>> Exiting InitializeDataAsync.");
        }


        // --- HÀM TẢI DỮ LIỆU & CÀI ĐẶT GIAO DIỆN ---

        /// <summary>
        /// Tải danh sách Tựa Sách từ tầng BUS và hiển thị lên ComboBox cboTuaSach.
        /// </summary>
        private async Task LoadTuaSachComboBox()
        {
            Debug.WriteLine("--- Starting LoadTuaSachComboBox ---");
            object? selectedValue = null; // Lưu giá trị đang chọn (ID)

            // Kiểm tra các control và dependency cần thiết
            if (cboTuaSach == null) { Debug.WriteLine("ERROR: LoadTuaSachComboBox skipped because cboTuaSach is NULL."); return; }
            if (_busTuaSach == null) { Debug.WriteLine("ERROR: LoadTuaSachComboBox skipped because _busTuaSach is NULL."); return; }

            List<TuaSachDTO> fetchedTuaSachList = new List<TuaSachDTO>(); // Lưu kết quả từ BUS

            try
            {
                // Lưu giá trị đang chọn của ComboBox (nếu có)
                try { if (cboTuaSach.SelectedValue != null && cboTuaSach.SelectedValue is int) { selectedValue = cboTuaSach.SelectedValue; } } catch { /* Bỏ qua lỗi nhỏ */ }

                // Gọi BUS để lấy dữ liệu
                Debug.WriteLine("LoadTuaSachComboBox: >>> Calling _busTuaSach.GetAllTuaSachAsync()...");
                fetchedTuaSachList = await _busTuaSach.GetAllTuaSachAsync();
                Debug.WriteLine($"LoadTuaSachComboBox: <<< Call completed. Result is {(fetchedTuaSachList == null ? "NULL" : $"a list with {fetchedTuaSachList.Count} raw items")}.");

                // Đảm bảo quay lại luồng UI trước khi cập nhật ComboBox
                await Task.Yield();
                Debug.WriteLine("LoadTuaSachComboBox: Switched back to UI thread.");

                // Kiểm tra lại ComboBox sau khi await
                if (cboTuaSach == null || cboTuaSach.IsDisposed) { Debug.WriteLine("ERROR: LoadTuaSachComboBox aborted after Task.Yield because cboTuaSach became null or disposed."); return; }

                // Xử lý kết quả từ BUS (lọc null nếu cần)
                if (fetchedTuaSachList == null) { _tuaSachList = new List<TuaSachDTO>(); }
                else { _tuaSachList = fetchedTuaSachList.Where(dto => dto != null).ToList(); }

                // Gán DataSource và cấu hình ComboBox
                Debug.WriteLine($"LoadTuaSachComboBox: Setting DataSource for cboTuaSach with {_tuaSachList.Count} items...");
                cboTuaSach.DataSource = null; // Xóa nguồn cũ
                cboTuaSach.DataSource = _tuaSachList; // Gán danh sách mới
                cboTuaSach.DisplayMember = "TenTuaSach"; // Thuộc tính hiển thị
                cboTuaSach.ValueMember = "Id"; // Thuộc tính giá trị
                Debug.WriteLine($"LoadTuaSachComboBox: DataSource set. Items.Count = {cboTuaSach.Items.Count}");

                // Chọn lại giá trị cũ hoặc bỏ chọn
                Debug.WriteLine($"LoadTuaSachComboBox: Attempting to restore selection to ID: {selectedValue ?? "N/A"}...");
                if (selectedValue != null && selectedValue is int currentId && _tuaSachList.Any(ts => ts.Id == currentId))
                {
                    try { cboTuaSach.SelectedValue = selectedValue; } catch { cboTuaSach.SelectedIndex = -1; } // Reset nếu lỗi
                }
                else { cboTuaSach.SelectedIndex = -1; } // Bỏ chọn
                Debug.WriteLine($"LoadTuaSachComboBox: Final SelectedIndex = {cboTuaSach.SelectedIndex}");
            }
            catch (Exception ex) // Bắt lỗi chung
            {
                Debug.WriteLine($"*** FATAL ERROR in LoadTuaSachComboBox: {ex}");
                await Task.Yield(); // Đảm bảo ở luồng UI để hiển thị MessageBox
                MaterialMessageBox.Show(this.FindForm(), $"Lỗi nghiêm trọng khi tải danh sách tựa sách: {ex.Message}", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Reset ComboBox nếu lỗi
                if (cboTuaSach != null && !cboTuaSach.IsDisposed) { cboTuaSach.DataSource = null; cboTuaSach.Items.Clear(); }
                _tuaSachList = new List<TuaSachDTO>(); // Reset danh sách
            }
            finally { Debug.WriteLine("--- Exiting LoadTuaSachComboBox ---"); }
        }

        /// <summary>
        /// Tải danh sách sách (Sach) từ tầng BUS và hiển thị lên DataGridView dgvSach.
        /// </summary>
        private async Task LoadDataGrid()
        {
            Debug.WriteLine("--- Starting LoadDataGrid ---");
            int? selectedId = null; // Lưu ID dòng đang chọn

            // Lấy ID dòng đang chọn (nếu có)
            if (dgvSach != null && dgvSach.SelectedRows.Count == 1 && dgvSach.SelectedRows[0].DataBoundItem is SachDTO currentDto) { selectedId = currentDto.Id; }

            // Kiểm tra control và dependency
            if (dgvSach == null) { Debug.WriteLine("ERROR: LoadDataGrid skipped: dgvSach is NULL."); return; }
            if (_busSach == null) { Debug.WriteLine("ERROR: LoadDataGrid skipped: _busSach is NULL."); return; }

            //this.Cursor = Cursors.WaitCursor; // Đã set ở InitializeDataAsync
            List<SachDTO> danhSach = new List<SachDTO>(); // Khởi tạo danh sách

            try
            {
                // Gọi BUS để lấy dữ liệu
                Debug.WriteLine("LoadDataGrid: >>> Calling _busSach.GetAllSachAsync()...");
                danhSach = await _busSach.GetAllSachAsync();
                Debug.WriteLine($"LoadDataGrid: <<< Call completed. Result is {(danhSach == null ? "NULL" : $"a list with {danhSach.Count} raw items")}.");

                // Đảm bảo quay lại luồng UI
                await Task.Yield();
                Debug.WriteLine("LoadDataGrid: Switched back to UI thread.");

                // Kiểm tra lại DataGridView sau khi await
                if (dgvSach == null || dgvSach.IsDisposed) { Debug.WriteLine("ERROR: LoadDataGrid aborted after Task.Yield because dgvSach became null or disposed."); return; }

                // Xử lý kết quả từ BUS (lọc null nếu cần)
                dgvSach.DataSource = null; // Xóa nguồn cũ
                if (danhSach == null) { danhSach = new List<SachDTO>(); }
                else { danhSach = danhSach.Where(dto => dto != null).ToList(); }

                // Gán DataSource và cấu hình cột (nếu chưa có)
                Debug.WriteLine($"LoadDataGrid: Setting DataSource with {danhSach.Count} items...");
                dgvSach.DataSource = danhSach;
                // Đảm bảo cột được setup SAU KHI có DataSource
                if (dgvSach.DataSource != null && dgvSach.Columns.Count == 0) // Chỉ setup cột lần đầu HOẶC khi DataSource được gán lại
                {
                    SetupDataGridViewColumns();
                }
                else if (dgvSach.DataSource == null)
                {
                    Debug.WriteLine("LoadDataGrid: DataSource is null, skipping column setup.");
                }
                else
                {
                    Debug.WriteLine($"LoadDataGrid: Columns already exist ({dgvSach.Columns.Count}), skipping setup.");
                }
                Debug.WriteLine($"LoadDataGrid: DataSource assigned. Columns: {dgvSach.Columns.Count}. Rows: {dgvSach.Rows.Count}");

                // Chọn lại dòng hoặc xóa lựa chọn
                bool rowSelectedAfterLoad = false;
                if (selectedId.HasValue)
                {
                    Debug.WriteLine($"LoadDataGrid: Attempting to re-select row with ID: {selectedId.Value}...");
                    rowSelectedAfterLoad = SelectRowById(selectedId.Value); // Hàm này sẽ trigger SelectionChanged
                    Debug.WriteLine($"LoadDataGrid: Re-selection result: {rowSelectedAfterLoad}");
                }
                else { dgvSach.ClearSelection(); Debug.WriteLine("LoadDataGrid: Cleared selection."); }

                // Nếu không chọn lại được dòng nào VÀ không đang thêm/sửa -> Xóa input
                if (!rowSelectedAfterLoad && !_isAdding && !IsEditing())
                {
                    ClearInputFields();
                }
            }
            catch (Exception ex) // Bắt lỗi chung
            {
                Debug.WriteLine($"*** FATAL ERROR in LoadDataGrid: {ex}");
                await Task.Yield(); // Đảm bảo ở luồng UI
                MaterialMessageBox.Show(this.FindForm(), $"Lỗi khi tải danh sách sách: {ex.Message}", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (dgvSach != null && !dgvSach.IsDisposed) dgvSach.DataSource = null; // Xóa lưới nếu lỗi
            }
            finally // Khối finally KHÔNG nên gọi SetControlState nữa vì đã gọi ở InitializeDataAsync
            {
                // await Task.Yield(); // Không cần Yield ở đây nữa
                // this.Cursor = Cursors.Default; // Đã gọi ở InitializeDataAsync
                // bool finalRowSelected = dgvSach?.SelectedRows.Count > 0;
                // bool isCurrentlyEditing = _isAdding || IsEditing();
                // SetControlState(isCurrentlyEditing, finalRowSelected); // <<< BỎ DÒNG NÀY
                // if (!finalRowSelected && !isCurrentlyEditing) { ClearInputFields(); } // <<< BỎ DÒNG NÀY
                Debug.WriteLine("--- Exiting LoadDataGrid (finally block, state set in InitializeDataAsync) ---");
            }
        }


        /// <summary>
        /// Cấu hình các cột hiển thị trong DataGridView dgvSach.
        /// </summary>
        private void SetupDataGridViewColumns()
        {
            // Kiểm tra các điều kiện cần thiết
            if (dgvSach == null || dgvSach.DataSource == null)
            {
                Debug.WriteLine("SetupDataGridViewColumns skipped: Grid or DataSource is null.");
                return;
            }
            // Kiểm tra xem cột đã được cấu hình chưa (dựa vào sự tồn tại của một cột cụ thể)
            // Bỏ kiểm tra này để đảm bảo cột được setup lại nếu DataSource thay đổi
            // if (dgvSach.Columns.Count > 0 && dgvSach.Columns.Contains("colMaSach")) return;
            Debug.WriteLine("--- Starting SetupDataGridViewColumns ---");

            try
            {
                dgvSach.Columns.Clear(); // Xóa các cột cũ (nếu có) trước khi thêm cột mới
                dgvSach.AutoGenerateColumns = false; // Tắt tự động tạo cột

                // Thêm và cấu hình từng cột mong muốn
                dgvSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaSach", HeaderText = "Mã Sách", Name = "colMaSach", Width = 100 });
                dgvSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenTuaSach", HeaderText = "Tên Tựa Sách", Name = "colTenTuaSach", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill }); // Tự động giãn cột này
                dgvSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DonGia", HeaderText = "Đơn Giá", Name = "colDonGia", Width = 100, DefaultCellStyle = { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } }); // Định dạng số và căn phải
                dgvSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NamXb", HeaderText = "Năm XB", Name = "colNamXb", Width = 80, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } }); // Căn giữa
                dgvSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NhaXb", HeaderText = "Nhà XB", Name = "colNhaXb", Width = 150 });
                dgvSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoLuong", HeaderText = "Tổng SL", Name = "colSoLuong", Width = 70, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight } }); // Căn phải
                dgvSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoLuongConLai", HeaderText = "SL Còn", Name = "colSoLuongConLai", Width = 70, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight } }); // Căn phải

                // Ẩn các cột không cần hiển thị (như ID)
                dgvSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", Name = "colId", Visible = false });
                dgvSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IdTuaSach", Name = "colIdTuaSach", Visible = false });

                // Các thuộc tính chung khác cho DataGridView
                dgvSach.ReadOnly = true; // Chỉ đọc
                dgvSach.AllowUserToAddRows = false; // Không cho thêm dòng
                dgvSach.AllowUserToDeleteRows = false; // Không cho xóa dòng
                dgvSach.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Chọn cả dòng
                dgvSach.MultiSelect = false; // Chỉ chọn 1 dòng
                dgvSach.RowHeadersVisible = false; // Ẩn cột header mặc định bên trái
                dgvSach.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells; // Tự chỉnh chiều cao dòng
                Debug.WriteLine("SetupDataGridViewColumns finished.");
            }
            catch (Exception ex) // Bắt lỗi nếu có vấn đề khi cấu hình cột
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi cấu hình cột DataGridView Sách: {ex.Message}");
                MaterialMessageBox.Show(this.FindForm(), $"Lỗi hiển thị dữ liệu Sách: {ex.Message}", "Lỗi Hiển Thị", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Debug.WriteLine("--- Exiting SetupDataGridViewColumns ---");
        }


        /// <summary>
        /// Kiểm tra xem UserControl có đang ở trạng thái Sửa (không phải Thêm) hay không.
        /// </summary>
        private bool IsEditing()
        {
            // Đang sửa nếu nút Lưu bật VÀ không phải đang Thêm
            // Cần kiểm tra null cho btnLuu
            return btnLuu != null && btnLuu.Enabled && !_isAdding;
        }

        /// <summary>
        /// Xóa trắng nội dung các ô nhập liệu trong panel chi tiết.
        /// </summary>
        private void ClearInputFields()
        {
            // Xóa text hoặc reset selection cho các controls
            if (txtId != null) txtId.Clear();
            if (txtMaSach != null) txtMaSach.Clear();
            if (txtDonGia != null) txtDonGia.Clear();
            if (txtNamXb != null) txtNamXb.Clear();
            if (txtNhaXb != null) txtNhaXb.Clear();
            if (cboTuaSach != null) cboTuaSach.SelectedIndex = -1; // Bỏ chọn ComboBox

            // Focus vào ô nhập liệu đầu tiên nếu đang ở chế độ Thêm
            if (_isAdding)
            {
                if (txtMaSach != null && txtMaSach.Enabled) txtMaSach.Focus();
                else if (cboTuaSach != null && cboTuaSach.Enabled) cboTuaSach.Focus();
            }
            Debug.WriteLine("Input fields cleared.");
        }

        /// <summary>
        /// Cập nhật trạng thái Enabled/Disabled của các controls dựa trên trạng thái hiện tại.
        /// </summary>
        /// <param name="isAddingOrEditing">True nếu đang ở chế độ Thêm hoặc Sửa.</param>
        /// <param name="rowSelected">True nếu có một dòng đang được chọn trong DataGridView.</param>
        private void SetControlState(bool isAddingOrEditing, bool rowSelected)
        {
            // Bật/tắt các nút chức năng chính
            if (btnThem != null) btnThem.Enabled = !isAddingOrEditing;
            if (btnSua != null) btnSua.Enabled = !isAddingOrEditing && rowSelected;
            if (btnXoa != null) btnXoa.Enabled = !isAddingOrEditing && rowSelected;
            if (btnXemCuonSach != null) btnXemCuonSach.Enabled = !isAddingOrEditing && rowSelected; // Quản lý nút mới

            // Kiểm tra xem có thay đổi chưa lưu không để bật/tắt nút Lưu
            bool hasChanges = HasUnsavedChanges();
            if (btnLuu != null) btnLuu.Enabled = isAddingOrEditing && hasChanges;
            if (btnBoQua != null) btnBoQua.Enabled = isAddingOrEditing; // Nút Bỏ qua bật khi đang thêm/sửa

            // Bật/tắt các ô nhập liệu
            if (txtId != null) txtId.Enabled = false; // ID luôn tắt
            if (txtMaSach != null) txtMaSach.Enabled = _isAdding; // Mã sách chỉ bật khi Thêm
            if (cboTuaSach != null) cboTuaSach.Enabled = isAddingOrEditing;
            if (txtDonGia != null) txtDonGia.Enabled = isAddingOrEditing;
            if (txtNamXb != null) txtNamXb.Enabled = isAddingOrEditing;
            if (txtNhaXb != null) txtNhaXb.Enabled = isAddingOrEditing;

            // Bật/tắt DataGridView (không cho tương tác khi đang thêm/sửa)
            if (dgvSach != null) dgvSach.Enabled = !isAddingOrEditing;
            Debug.WriteLine($"SetControlState: isAddingOrEditing={isAddingOrEditing}, rowSelected={rowSelected}, hasChanges={hasChanges}, btnLuu.Enabled={btnLuu?.Enabled}");
        }


        /// <summary>
        /// Hiển thị thông tin của dòng đang được chọn trong DataGridView lên các ô nhập liệu.
        /// </summary>
        // ***** ĐÃ SỬA Ở ĐÂY: Bỏ kiểm tra IsAdding/IsEditing ở đầu *****
        private void DisplaySelectedRow()
        {
            Debug.WriteLine($"Entering DisplaySelectedRow. (Previous state: IsAdding={_isAdding}, IsEditing={IsEditing()})");
            // OLD CHECK (REMOVED): if (_isAdding || IsEditing()) { Debug.WriteLine("DisplaySelectedRow skipped: Currently in Add/Edit mode."); return; }

            SachDTO? selectedDto = null;
            bool rowSelected = false;

            // Lấy DTO từ dòng đang chọn
            if (dgvSach != null && dgvSach.SelectedRows.Count == 1)
            {
                if (dgvSach.SelectedRows[0].DataBoundItem is SachDTO dto) { selectedDto = dto; rowSelected = true; }
                else { Debug.WriteLine($"ERROR: DisplaySelectedRow - DataBoundItem is not SachDTO."); }
            }
            else if (dgvSach?.SelectedRows.Count > 1)
            {
                Debug.WriteLine("DisplaySelectedRow: Multiple rows selected. Clearing fields.");
                ClearInputFields();
                SetControlState(false, false); // Cập nhật state không có dòng nào chọn
                return; // Thoát sớm vì không rõ dòng nào để hiển thị
            }
            else // Không có dòng nào được chọn
            {
                Debug.WriteLine("DisplaySelectedRow: No row selected. Clearing fields.");
                ClearInputFields();
                SetControlState(false, false); // Cập nhật state không có dòng nào chọn
                return; // Thoát sớm
            }

            // Hiển thị thông tin lên các controls nếu có DTO hợp lệ
            if (selectedDto != null)
            {
                if (txtId != null) txtId.Text = selectedDto.Id.ToString();
                if (txtMaSach != null) txtMaSach.Text = selectedDto.MaSach ?? string.Empty;

                // Chọn giá trị trong ComboBox Tựa sách
                if (cboTuaSach != null)
                {
                    int idTuaSach = selectedDto.IdTuaSach;
                    if (idTuaSach > 0 && _tuaSachList != null && _tuaSachList.Any(ts => ts.Id == idTuaSach))
                    {
                        try { cboTuaSach.SelectedValue = idTuaSach; } catch { cboTuaSach.SelectedIndex = -1; } // Reset nếu lỗi
                    }
                    else { cboTuaSach.SelectedIndex = -1; } // Bỏ chọn nếu không tìm thấy
                }

                // Hiển thị Đơn giá (định dạng tiền tệ)
                if (txtDonGia != null)
                {
                    // SachDTO.DonGia là int, không cần parse lại từ decimal
                    int donGia = selectedDto.DonGia;
                    txtDonGia.Text = donGia.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"));
                }
                // Hiển thị Năm XB và Nhà XB
                if (txtNamXb != null) txtNamXb.Text = (selectedDto.NamXb > 0) ? selectedDto.NamXb.ToString() : string.Empty;
                if (txtNhaXb != null) txtNhaXb.Text = selectedDto.NhaXb ?? string.Empty;
                Debug.WriteLine($"DisplaySelectedRow: Displayed data for Sach ID {selectedDto.Id}");
            }
            else // Trường hợp không lấy được DTO (dù rowSelected=true) -> lỗi logic đâu đó
            {
                Debug.WriteLine("DisplaySelectedRow: Row selected but DTO is null. Clearing fields.");
                ClearInputFields();
                rowSelected = false; // Coi như không có dòng nào được chọn hợp lệ
            }

            // Cập nhật trạng thái controls về chế độ xem SAU KHI hiển thị xong
            // Quan trọng: Gọi SetControlState ở đây chỉ khi DisplaySelectedRow được gọi từ ngữ cảnh không phải là Bỏ qua/Lưu.
            // Trong btnBoQua_Click, SetControlState nên được gọi riêng sau khi DisplaySelectedRow hoàn tất.
            // Tuy nhiên, để đơn giản hóa, ta vẫn gọi SetControlState ở đây, nhưng đảm bảo _isAdding đã là false.
            SetControlState(_isAdding, rowSelected); // _isAdding đã được set false bởi btnBoQua_Click

            Debug.WriteLine("Exiting DisplaySelectedRow.");
        }
        // ***** KẾT THÚC SỬA ĐỔI *****

        /// <summary>
        /// Kiểm tra xem có thay đổi nào chưa được lưu trong các ô nhập liệu hay không.
        /// </summary>
        /// <returns>True nếu có thay đổi, False nếu không.</returns>
        private bool HasUnsavedChanges()
        {
            // Chỉ kiểm tra khi đang thêm hoặc sửa
            if (!_isAdding && !IsEditing()) return false;

            SachDTO? originalDto = null;
            // Lấy DTO gốc nếu đang sửa
            if (!_isAdding && dgvSach?.SelectedRows.Count == 1) { originalDto = dgvSach.SelectedRows[0].DataBoundItem as SachDTO; }

            // Lấy giá trị hiện tại từ các controls
            string? currentMaSach = txtMaSach?.Text.Trim();
            int currentIdTuaSach = (cboTuaSach?.SelectedValue as int?) ?? 0;
            int currentDonGia = 0; // DTO.DonGia là int
            // Parse từ text box vào biến int
            if (txtDonGia != null && !string.IsNullOrWhiteSpace(txtDonGia.Text))
            {
                // Bỏ dấu phân cách ngàn trước khi parse
                string cleanedDonGia = txtDonGia.Text.Trim().Replace(".", "");
                int.TryParse(cleanedDonGia, NumberStyles.Any, CultureInfo.GetCultureInfo("vi-VN"), out currentDonGia);
            }

            int currentNamXb = 0;
            int.TryParse(txtNamXb?.Text.Trim(), out currentNamXb);
            string? currentNhaXb = txtNhaXb?.Text.Trim();

            // So sánh giá trị
            if (_isAdding) // Đang thêm: Chỉ cần 1 trường có giá trị là coi như có thay đổi
            {
                bool changed = (txtMaSach != null && txtMaSach.Enabled && !string.IsNullOrWhiteSpace(currentMaSach)) ||
                       currentIdTuaSach > 0 || currentDonGia > 0 || currentNamXb > 0 || !string.IsNullOrWhiteSpace(currentNhaXb);
                // Debug.WriteLine($"HasUnsavedChanges (Adding): {changed}");
                return changed;
            }
            else if (originalDto != null) // Đang sửa: So sánh với giá trị gốc
            {
                bool changed = (txtMaSach != null && txtMaSach.Enabled && currentMaSach != (originalDto.MaSach ?? "")) || // Chỉ so Mã nếu ô Mã bật
                       currentIdTuaSach != originalDto.IdTuaSach ||
                       currentDonGia != originalDto.DonGia || // So sánh int với int
                       currentNamXb != originalDto.NamXb ||
                       currentNhaXb != (originalDto.NhaXb ?? "");
                // Debug.WriteLine($"HasUnsavedChanges (Editing): {changed}");
                return changed;
            }

            // Debug.WriteLine("HasUnsavedChanges (Viewing or Error): false");
            return false; // Mặc định không có thay đổi
        }


        /// <summary>
        /// Xử lý sự kiện khi nội dung của một ô nhập liệu thay đổi.
        /// Cập nhật trạng thái nút Lưu nếu đang ở chế độ thêm/sửa.
        /// </summary>
        private void InputField_Changed(object? sender, EventArgs e)
        {
            if (_isAdding || IsEditing())
            {
                // Gọi lại SetControlState để kiểm tra HasUnsavedChanges và cập nhật nút Lưu
                SetControlState(true, dgvSach?.SelectedRows.Count > 0);
            }
        }

        // --- SỰ KIỆN CỦA CÁC BUTTON ---

        /// <summary>
        /// Xử lý sự kiện nhấn nút Thêm. Chuyển sang chế độ Thêm mới.
        /// </summary>
        private void btnThem_Click(object sender, EventArgs e)
        {
            // Hỏi xác nhận nếu có thay đổi chưa lưu
            if (IsEditing() && HasUnsavedChanges()) // Chỉ hỏi khi đang SỬA mà có thay đổi
            {
                DialogResult confirm = MaterialMessageBox.Show(this.FindForm(), "Dữ liệu đang sửa chưa được lưu. Bạn có muốn hủy bỏ thay đổi và thêm mới?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.No) return; // Không làm gì nếu người dùng không muốn hủy
            }
            _isAdding = true; // Đặt trạng thái đang Thêm
            if (dgvSach != null) dgvSach.ClearSelection(); // Bỏ chọn dòng trên lưới
            ClearInputFields(); // Xóa trắng ô nhập liệu
            SetControlState(true, false); // Cập nhật trạng thái controls cho chế độ Thêm
            if (txtMaSach != null && txtMaSach.Enabled) txtMaSach.Focus(); // Focus vào ô Mã sách
            Debug.WriteLine("Entered Add mode.");
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút Sửa. Chuyển sang chế độ Sửa.
        /// </summary>
        private void btnSua_Click(object sender, EventArgs e)
        {
            // Hỏi xác nhận nếu có thay đổi chưa lưu (ít khi xảy ra vì nút Sửa tắt khi đang sửa)
            // if (HasUnsavedChanges()) // Bỏ kiểm tra này vì nút Sửa bị disable khi đang sửa
            // {
            //     DialogResult confirm = MaterialMessageBox.Show(this.FindForm(), "Dữ liệu chưa được lưu. Bạn có muốn hủy bỏ thay đổi và bắt đầu sửa?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //     if (confirm == DialogResult.No) return;
            // }

            // Chỉ cho phép sửa nếu đang chọn đúng 1 dòng và nút Sửa đang bật
            if (dgvSach?.SelectedRows.Count == 1 && btnSua != null && btnSua.Enabled)
            {
                _isAdding = false; // Đảm bảo không phải trạng thái Thêm
                SetControlState(true, true); // Cập nhật trạng thái controls cho chế độ Sửa
                // Focus vào ô nhập liệu đầu tiên có thể sửa (thường là Tựa sách)
                if (cboTuaSach != null && cboTuaSach.Enabled) cboTuaSach.Focus();
                else if (txtDonGia != null && txtDonGia.Enabled) txtDonGia.Focus();
                Debug.WriteLine("Entered Edit mode.");
            }
            else if (dgvSach?.SelectedRows.Count == 0) { MaterialMessageBox.Show(this.FindForm(), "Vui lòng chọn sách cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            else { MaterialMessageBox.Show(this.FindForm(), "Chỉ có thể sửa một sách mỗi lần.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        /// <summary>
        /// Tải lại DataGridView và chọn dòng có ID cụ thể.
        /// </summary>
        /// <param name="idToSelect">ID của dòng cần chọn sau khi tải lại.</param>
        private async Task LoadDataGridAndSelect(int idToSelect)
        {
            Debug.WriteLine($"LoadDataGridAndSelect (Sach) called with idToSelect: {idToSelect}");
            await LoadDataGrid(); // Tải lại dữ liệu
            // Không cần Yield ở đây vì SelectRowById sẽ xử lý UI
            // await Task.Yield(); // Đảm bảo quay lại luồng UI
            Debug.WriteLine("LoadDataGridAndSelect (Sach): LoadDataGrid completed.");
            if (idToSelect > 0)
            {
                Debug.WriteLine($"LoadDataGridAndSelect (Sach): Attempting to select row by ID: {idToSelect}");
                bool rowSelected = SelectRowById(idToSelect); // Chọn dòng
                if (!rowSelected) Debug.WriteLine($"LoadDataGridAndSelect (Sach): SelectRowById failed for ID {idToSelect}.");
            }
            else { Debug.WriteLine("LoadDataGridAndSelect (Sach): idToSelect is invalid (<=0), skipping selection."); }
            Debug.WriteLine("LoadDataGridAndSelect (Sach) finished.");
        }


        /// <summary>
        /// Xử lý sự kiện nhấn nút Lưu. Thực hiện Thêm hoặc Cập nhật dữ liệu.
        /// </summary>
        private async void btnLuu_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("--- Entering btnLuu_Click ---");
            // Không làm gì nếu nút Lưu đang tắt
            if (btnLuu == null || !btnLuu.Enabled) { Debug.WriteLine("btnLuu_Click skipped: Button is null or disabled."); return; }

            // Lấy dữ liệu từ các ô nhập liệu
            string? maSach = txtMaSach?.Text.Trim();
            int idTuaSach = (cboTuaSach?.SelectedValue as int?) ?? 0;
            string donGiaText = txtDonGia?.Text.Trim() ?? "0";
            // ***** SỬA Ở ĐÂY: Parse vào biến int vì DTO.DonGia là int *****
            int donGia = 0;
            string namXbText = txtNamXb?.Text.Trim() ?? "0";
            string nhaXb = txtNhaXb?.Text.Trim() ?? "";

            // --- VALIDATION DỮ LIỆU ---
            if (idTuaSach <= 0) { MaterialMessageBox.Show(this.FindForm(), "Vui lòng chọn tựa sách.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning); cboTuaSach?.Focus(); return; }
            if (_isAdding && txtMaSach != null && txtMaSach.Enabled && string.IsNullOrWhiteSpace(maSach)) { MaterialMessageBox.Show(this.FindForm(), "Mã sách không được để trống khi thêm mới.", "Lỗi Nhập Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtMaSach?.Focus(); return; }
            // Parse đơn giá an toàn hơn vào biến int
            if (!int.TryParse(donGiaText.Replace(".", "").Replace(",", ""), NumberStyles.Any, CultureInfo.GetCultureInfo("vi-VN"), out donGia) || donGia < 0)
            {
                MaterialMessageBox.Show(this.FindForm(), "Đơn giá không hợp lệ.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtDonGia?.Focus(); return;
            }
            if (string.IsNullOrWhiteSpace(nhaXb)) { MaterialMessageBox.Show(this.FindForm(), "Nhà xuất bản không được rỗng.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtNhaXb?.Focus(); return; }
            if (!int.TryParse(namXbText, out int namXb) || namXb <= 1900 || namXb > DateTime.Now.Year + 1) { MaterialMessageBox.Show(this.FindForm(), "Năm xuất bản không hợp lệ.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtNamXb?.Focus(); return; }
            Debug.WriteLine("btnLuu_Click: Validation passed.");

            // --- TẠO DTO ĐỂ GỬI ĐẾN BUS ---
            int currentId = 0;
            // Lấy ID nếu đang sửa
            if (!_isAdding && txtId != null && int.TryParse(txtId.Text, out int parsedId)) { currentId = parsedId; }
            else if (!_isAdding) { Debug.WriteLine("ERROR: Cannot get valid Sach ID for update."); MaterialMessageBox.Show(this.FindForm(), "Không xác định được ID sách để cập nhật.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            // Xác định Mã sách cuối cùng (lấy từ lưới nếu đang sửa và ô Mã bị tắt)
            string? finalMaSach = maSach;
            if (!_isAdding && txtMaSach != null && !txtMaSach.Enabled)
            {
                if (dgvSach?.SelectedRows.Count == 1 && dgvSach.SelectedRows[0].DataBoundItem is SachDTO originalDtoFromGrid)
                { finalMaSach = originalDtoFromGrid.MaSach; }
                else
                { Debug.WriteLine("ERROR: Cannot get original MaSach from grid for update."); MaterialMessageBox.Show(this.FindForm(), "Không lấy được mã sách gốc để cập nhật.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            }
            // Kiểm tra lại Mã sách không rỗng (kể cả khi lấy từ lưới)
            if (string.IsNullOrWhiteSpace(finalMaSach)) { MaterialMessageBox.Show(this.FindForm(), "Mã sách không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); if (txtMaSach != null && txtMaSach.Enabled) txtMaSach.Focus(); return; }

            // Tạo DTO Sach (chỉ chứa thông tin metadata)
            SachDTO sachDto = new SachDTO
            {
                Id = currentId,
                MaSach = finalMaSach,
                IdTuaSach = idTuaSach,
                DonGia = donGia, // Gán giá trị int đã parse
                NamXb = namXb,
                NhaXb = nhaXb
                // SoLuong và SoLuongConLai KHÔNG được quản lý ở đây khi thêm/sửa metadata
            };
            Debug.WriteLine($"btnLuu_Click: Created SachDTO (for Metadata) - ID={sachDto.Id}, MaSach='{sachDto.MaSach}'");

            // --- GỌI BUS LAYER ---
            bool success = false; string? errorMsg = null; SachDTO? processedDto = null;
            // Tắt nút và hiển thị con trỏ chờ
            if (btnLuu != null) btnLuu.Enabled = false; if (btnBoQua != null) btnBoQua.Enabled = false; this.Cursor = Cursors.WaitCursor;

            try
            {
                if (_isAdding) // Gọi hàm thêm metadata
                {
                    Debug.WriteLine("btnLuu_Click: >>> Calling _busSach.AddSachMetadataAsync...");
                    processedDto = await _busSach.AddSachMetadataAsync(sachDto);
                    success = processedDto != null;
                    Debug.WriteLine($"btnLuu_Click: <<< AddSachMetadataAsync completed. Success: {success}. ID: {processedDto?.Id ?? -1}");
                    if (success && processedDto != null) currentId = processedDto.Id; // Lấy ID mới nếu thêm thành công
                    else if (string.IsNullOrEmpty(errorMsg)) errorMsg = "Thêm thông tin sách thất bại (BUS không trả về kết quả).";
                }
                else // Gọi hàm cập nhật metadata
                {
                    Debug.WriteLine($"btnLuu_Click: >>> Calling _busSach.UpdateSachAsync for ID {sachDto.Id}...");
                    success = await _busSach.UpdateSachAsync(sachDto); // Update thường trả về bool
                    Debug.WriteLine($"btnLuu_Click: <<< UpdateSachAsync completed. Success: {success}");
                    if (success) processedDto = sachDto; // Dùng DTO cũ để lấy ID chọn lại dòng
                    else if (string.IsNullOrEmpty(errorMsg)) errorMsg = "Cập nhật sách thất bại.";
                }
            }
            catch (ArgumentException argEx) { errorMsg = $"Lỗi dữ liệu nhập: {argEx.Message}"; Debug.WriteLine($"ArgumentException saving Sach: {argEx}"); }
            catch (InvalidOperationException invOpEx) { errorMsg = $"Lỗi nghiệp vụ: {invOpEx.Message}"; Debug.WriteLine($"InvalidOperationException saving Sach: {invOpEx}"); }
            catch (Exception ex) { errorMsg = $"Lỗi hệ thống khi lưu sách: {ex.Message}"; Debug.WriteLine($"Exception saving Sach: {ex}"); }
            finally // Khối finally để trả lại trạng thái UI
            {
                // Không cần Yield ở đây
                this.Cursor = Cursors.Default; // Trả lại con trỏ
                // Bật lại nút nếu thất bại VÀ còn đang trong mode thêm/sửa
                if (!success && (_isAdding || IsEditing()))
                {
                    Debug.WriteLine("btnLuu_Click finally: Operation failed, re-evaluating state.");
                    if (btnBoQua != null && !btnBoQua.IsDisposed) btnBoQua.Enabled = true; // Luôn bật Bỏ qua
                    SetControlState(true, dgvSach?.SelectedRows.Count > 0); // Cập nhật lại state (có thể bật Lưu nếu còn thay đổi)
                }
                else if (success) { Debug.WriteLine("btnLuu_Click finally: Operation succeeded. State will be reset after reload."); }
                else { /* Lỗi có errorMsg */ if (btnBoQua != null && !btnBoQua.IsDisposed) btnBoQua.Enabled = true; SetControlState(true, dgvSach?.SelectedRows.Count > 0); Debug.WriteLine("btnLuu_Click finally: Operation failed with specific error."); }
            }

            // --- XỬ LÝ KẾT QUẢ ---
            if (success)
            {
                Debug.WriteLine($"btnLuu_Click: Success. Reloading grid, selecting ID: {currentId}");
                MaterialMessageBox.Show(this.FindForm(), _isAdding ? "Thêm thông tin sách thành công!" : "Cập nhật sách thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _isAdding = false; // Reset trạng thái Thêm
                await LoadDataGridAndSelect(currentId); // Tải lại lưới và chọn dòng vừa xử lý
            }
            else // Lưu thất bại
            {
                Debug.WriteLine($"btnLuu_Click: Failed. Error: {errorMsg ?? "Unknown"}");
                errorMsg ??= _isAdding ? "Thêm thất bại (lý do không xác định)." : "Cập nhật thất bại (lý do không xác định).";
                MaterialMessageBox.Show(this.FindForm(), errorMsg, "Lỗi Lưu Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // Có thể focus lại ô lỗi nếu xác định được từ errorMsg
            }
            Debug.WriteLine("--- Exiting btnLuu_Click ---");
        }


        /// <summary>
        /// Xử lý sự kiện nhấn nút Bỏ qua. Hủy bỏ thay đổi và quay về trạng thái xem.
        /// </summary>
        // ***** ĐÃ SỬA Ở ĐÂY: Đảm bảo gọi SetControlState sau DisplaySelectedRow *****
        private void btnBoQua_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("--- Entering btnBoQua_Click ---");
            bool wasAdding = _isAdding;
            bool wasEditing = IsEditing();

            if (!wasAdding && !wasEditing) { Debug.WriteLine("btnBoQua_Click skipped: Not in Add/Edit mode."); return; }

            if (HasUnsavedChanges())
            {
                DialogResult confirm = MaterialMessageBox.Show(this.FindForm(), "Dữ liệu chưa được lưu. Bạn có muốn hủy bỏ thay đổi?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.No) { Debug.WriteLine("btnBoQua_Click: User cancelled."); return; }
                Debug.WriteLine("btnBoQua_Click: User confirmed discarding changes.");
            }
            else { Debug.WriteLine("btnBoQua_Click: No unsaved changes detected."); }

            _isAdding = false; // Reset state FIRST
            Debug.WriteLine("btnBoQua_Click: State reset (_isAdding=false). Calling DisplaySelectedRow...");
            DisplaySelectedRow(); // Call display function (which will now execute fully and call SetControlState inside)
            // SetControlState is called inside DisplaySelectedRow, no need to call again here.
            Debug.WriteLine("--- Exiting btnBoQua_Click ---");
        }


        /// <summary>
        /// Xử lý sự kiện nhấn nút Xóa. Thực hiện xóa vĩnh viễn ấn bản sách.
        /// </summary>
        private async void btnXoa_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("--- Entering btnXoa_Click ---");
            // Không cho xóa khi đang thêm/sửa
            if (_isAdding || IsEditing()) { Debug.WriteLine("btnXoa_Click skipped: Cannot delete while editing."); MaterialMessageBox.Show(this.FindForm(), "Không thể xóa khi đang thêm hoặc sửa.", "Thao tác không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            // Kiểm tra dependency
            if (_busSach == null) { Debug.WriteLine("ERROR: btnXoa_Click skipped: _busSach is NULL."); MaterialMessageBox.Show(this.FindForm(), "Lỗi dịch vụ Sách, không thể xóa.", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            // Chỉ xóa khi chọn đúng 1 dòng
            if (dgvSach?.SelectedRows.Count == 1 && dgvSach.SelectedRows[0].DataBoundItem is SachDTO dto)
            {
                int idToDelete = dto.Id;
                string tenSach = dto.MaSach ?? dto.TenTuaSach ?? $"ID {idToDelete}"; // Tên để hiển thị xác nhận
                if (idToDelete <= 0) { Debug.WriteLine($"ERROR: Invalid Sach ID for deletion: {idToDelete}"); MaterialMessageBox.Show(this.FindForm(), "ID sách không hợp lệ để xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                Debug.WriteLine($"btnXoa_Click: Attempting to delete Sach ID: {idToDelete}, Name: '{tenSach}'");

                // 1. Kiểm tra ràng buộc trước khi hỏi xác nhận
                Debug.WriteLine($"btnXoa_Click: >>> Checking CanHardDeleteSachAsync for ID {idToDelete}...");
                bool canDelete = false; string? checkErrorMsg = null; this.Cursor = Cursors.WaitCursor;
                try { canDelete = await _busSach.CanHardDeleteSachAsync(idToDelete); } // Gọi BUS kiểm tra
                catch (Exception ex) { checkErrorMsg = $"Lỗi khi kiểm tra khả năng xóa: {ex.Message}"; Debug.WriteLine($"Exception checking CanHardDeleteSachAsync: {ex}"); }
                finally { this.Cursor = Cursors.Default; } // Trả lại con trỏ ngay sau khi kiểm tra
                Debug.WriteLine($"btnXoa_Click: <<< Check completed. Result: {canDelete}, Error: '{checkErrorMsg ?? "None"}'");

                // Xử lý kết quả kiểm tra
                if (!string.IsNullOrEmpty(checkErrorMsg)) { MaterialMessageBox.Show(this.FindForm(), checkErrorMsg, "Lỗi Kiểm Tra", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (!canDelete) { MaterialMessageBox.Show(this.FindForm(), $"Không thể xóa ấn bản sách '{tenSach}' (ID: {idToDelete}) vì còn tồn tại các cuốn sách vật lý hoặc dữ liệu mượn trả liên quan.", "Ràng buộc Xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                // 2. Hỏi xác nhận xóa VĨNH VIỄN
                DialogResult confirm = MaterialMessageBox.Show(this.FindForm(), $"Bạn có chắc chắn muốn xóa vĩnh viễn ấn bản sách '{tenSach}' (ID: {idToDelete})?\nHành động này KHÔNG thể hoàn tác và sẽ xóa mọi dữ liệu liên quan đến ấn bản này.", "Xác nhận Xóa VĨNH VIỄN", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

                if (confirm == DialogResult.Yes)
                {
                    Debug.WriteLine($"btnXoa_Click: User confirmed HARD DELETE for ID {idToDelete}.");
                    // 3. Thực hiện xóa vĩnh viễn
                    bool success = false; string? errorMsg = null;
                    if (btnXoa != null) btnXoa.Enabled = false; // Tạm tắt nút Xóa
                    this.Cursor = Cursors.WaitCursor;
                    try
                    {
                        Debug.WriteLine($"btnXoa_Click: >>> Calling _busSach.HardDeleteSachAsync for ID {idToDelete}...");
                        success = await _busSach.HardDeleteSachAsync(idToDelete); // Gọi BUS xóa cứng
                        Debug.WriteLine($"btnXoa_Click: <<< HardDeleteSachAsync completed. Success: {success}");
                    }
                    catch (InvalidOperationException invalidOpEx) { errorMsg = $"Không thể xóa: {invalidOpEx.Message}"; Debug.WriteLine($"InvalidOperationException deleting Sach: {invalidOpEx}"); }
                    catch (Exception ex) { errorMsg = $"Lỗi hệ thống khi xóa: {ex.Message}"; Debug.WriteLine($"Exception deleting Sach: {ex}"); }
                    finally { this.Cursor = Cursors.Default; } // Trả lại con trỏ

                    // 4. Xử lý kết quả xóa
                    if (success)
                    {
                        Debug.WriteLine($"btnXoa_Click: Hard delete successful for ID {idToDelete}. Reloading grid.");
                        MaterialMessageBox.Show(this.FindForm(), "Xóa ấn bản sách thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadDataGrid(); // Tải lại lưới (sẽ tự cập nhật state)
                    }
                    else // Xóa thất bại
                    {
                        Debug.WriteLine($"btnXoa_Click: Hard delete failed for ID {idToDelete}. Error: {errorMsg ?? "Unknown"}");
                        errorMsg ??= "Xóa ấn bản sách thất bại (lý do không xác định).";
                        MaterialMessageBox.Show(this.FindForm(), errorMsg, "Lỗi Xóa", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // Cập nhật lại state controls vì xóa thất bại
                        SetControlState(false, dgvSach?.SelectedRows.Count > 0);
                    }
                }
                else { Debug.WriteLine($"btnXoa_Click: User cancelled HARD DELETE for ID {idToDelete}."); }
            }
            // Thông báo nếu chưa chọn dòng hoặc chọn nhiều dòng
            else if (dgvSach?.SelectedRows.Count == 0) { MaterialMessageBox.Show(this.FindForm(), "Vui lòng chọn sách cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            else { MaterialMessageBox.Show(this.FindForm(), "Chỉ có thể xóa một sách mỗi lần.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            Debug.WriteLine("--- Exiting btnXoa_Click ---");
        }


        /// <summary>
        /// Xử lý sự kiện nhấn nút Thoát. Yêu cầu đóng UserControl.
        /// </summary>
        private void btnThoat_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("--- Entering btnThoat_Click ---");
            // Hỏi xác nhận nếu có thay đổi chưa lưu
            if ((_isAdding || IsEditing()) && HasUnsavedChanges())
            {
                DialogResult confirm = MaterialMessageBox.Show(this.FindForm(), "Dữ liệu chưa được lưu. Bạn có chắc chắn muốn hủy bỏ thay đổi và thoát?", "Xác nhận Thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (confirm == DialogResult.No) { Debug.WriteLine("btnThoat_Click: User cancelled exit."); return; }
                Debug.WriteLine("btnThoat_Click: User confirmed exit despite unsaved changes.");
            }
            _isAdding = false; // Reset trạng thái phòng ngừa
            Debug.WriteLine("btnThoat_Click: Invoking RequestClose event.");
            RequestClose?.Invoke(this, EventArgs.Empty); // Kích hoạt sự kiện để form cha xử lý
            Debug.WriteLine("--- Exiting btnThoat_Click ---");
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút "Xem Cuốn Sách". Mở Form frmDanhSachCuonSach.
        /// </summary>
        private void btnXemCuonSach_Click(object sender, EventArgs e)
        {
            // Chỉ xử lý khi chọn đúng 1 dòng trên lưới
            if (dgvSach != null && dgvSach.SelectedRows.Count == 1 && dgvSach.SelectedRows[0].DataBoundItem is SachDTO selectedSach)
            {
                // Kiểm tra ID sách hợp lệ
                if (selectedSach.Id <= 0)
                {
                    MaterialMessageBox.Show(this.FindForm(), "Không thể xem cuốn sách cho ấn bản chưa có ID hợp lệ.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // Kiểm tra dependency _busCuonSach
                if (_busCuonSach == null)
                {
                    Debug.WriteLine("*** ERROR: _busCuonSach is null in btnXemCuonSach_Click. Check DI registration.");
                    MaterialMessageBox.Show(this.FindForm(), "Lỗi khởi tạo chức năng xem cuốn sách (BUS not found).", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                try
                {
                    // Lấy tên sách để hiển thị trên tiêu đề form con
                    string tenSachHienThi = $"{selectedSach.MaSach ?? "N/A"} - {selectedSach.TenTuaSach ?? "Không tên"}";
                    // Tạo và hiển thị form dialog, truyền dependency và thông tin cần thiết
                    using (var frm = new frmDanhSachCuonSach(_busCuonSach, selectedSach.Id, tenSachHienThi))
                    {
                        frm.ShowDialog(this.FindForm()); // Hiển thị form dialog
                    }
                }
                catch (InvalidOperationException ioEx) // Bắt lỗi DI nếu có
                {
                    Debug.WriteLine($"*** DI ERROR when showing frmDanhSachCuonSach: {ioEx}");
                    MaterialMessageBox.Show(this.FindForm(), $"Lỗi khởi tạo chức năng xem cuốn sách: {ioEx.Message}\nKiểm tra đăng ký Dependency Injection.", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex) // Bắt các lỗi khác
                {
                    Debug.WriteLine($"*** ERROR showing frmDanhSachCuonSach: {ex}");
                    MaterialMessageBox.Show(this.FindForm(), $"Lỗi khi mở danh sách cuốn sách: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // Thông báo nếu chưa chọn dòng hoặc chọn nhiều dòng
            else if (dgvSach?.SelectedRows.Count == 0)
            {
                MaterialMessageBox.Show(this.FindForm(), "Vui lòng chọn một ấn bản sách để xem danh sách cuốn sách.", "Chưa chọn sách", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MaterialMessageBox.Show(this.FindForm(), "Vui lòng chỉ chọn một ấn bản sách.", "Chọn quá nhiều", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        // --- SỰ KIỆN CỦA DATAGRIDVIEW ---

        /// <summary>
        /// Xử lý sự kiện khi lựa chọn dòng trong DataGridView thay đổi.
        /// Hiển thị thông tin của dòng được chọn lên các ô nhập liệu (nếu không đang thêm/sửa).
        /// </summary>
        private void dgvSach_SelectionChanged(object sender, EventArgs e)
        {
            // Chỉ hiển thị khi không ở chế độ thêm/sửa
            if (!_isAdding && !IsEditing())
            {
                DisplaySelectedRow();
            }
            else
            {
                Debug.WriteLine($"dgvSach_SelectionChanged skipped DisplaySelectedRow: IsAdding={_isAdding}, IsEditing={IsEditing()}");
            }
        }

        /// <summary>
        /// Xử lý sự kiện double-click trên DataGridView.
        /// Kích hoạt chế độ Sửa cho dòng được double-click (nếu hợp lệ).
        /// </summary>
        private void dgvSach_DoubleClick(object sender, EventArgs e)
        {
            // Chỉ kích hoạt sửa nếu đang chọn 1 dòng, không thêm/sửa, và nút Sửa đang bật
            if (dgvSach?.SelectedRows.Count == 1 && !_isAdding && !IsEditing() && btnSua != null && btnSua.Enabled)
            {
                Debug.WriteLine($"dgvSach_DoubleClick: Triggering Edit for selected row ID: {(dgvSach.SelectedRows[0].DataBoundItem as SachDTO)?.Id ?? -1}");
                btnSua_Click(sender, e); // Gọi sự kiện click của nút Sửa
            }
            else
            {
                Debug.WriteLine($"dgvSach_DoubleClick skipped: SelectedRows={dgvSach?.SelectedRows.Count ?? -1}, IsAdding={_isAdding}, IsEditing={IsEditing()}, BtnSuaEnabled={btnSua?.Enabled ?? false}");
            }
        }

        // --- HÀM HỖ TRỢ KHÁC ---

        /// <summary>
        /// Chọn một dòng trong DataGridView dựa trên ID của Sach.
        /// </summary>
        /// <param name="id">ID của sách cần chọn.</param>
        /// <returns>True nếu tìm thấy và chọn được dòng, False nếu không.</returns>
        private bool SelectRowById(int id)
        {
            Debug.WriteLine($"--- Entering SelectRowById({id}) ---");
            // Kiểm tra điều kiện đầu vào
            if (dgvSach == null || dgvSach.Rows.Count == 0 || id <= 0) { Debug.WriteLine($"SelectRowById({id}) skipped: Grid null/empty or invalid ID."); return false; }

            bool found = false;
            // Tạm thời ngắt sự kiện SelectionChanged để tránh trigger không mong muốn
            dgvSach.SelectionChanged -= dgvSach_SelectionChanged;
            try
            {
                dgvSach.ClearSelection(); // Xóa lựa chọn hiện tại
                Debug.WriteLine($"SelectRowById({id}): Cleared selection. Searching rows...");
                // Duyệt qua từng dòng để tìm ID khớp
                foreach (DataGridViewRow row in dgvSach.Rows)
                {
                    if (row.DataBoundItem is SachDTO dto && dto.Id == id)
                    {
                        Debug.WriteLine($"SelectRowById({id}): Found row at index {row.Index}. Setting Selected=true.");
                        row.Selected = true; // Chọn dòng
                        found = true;
                        // Cố gắng cuộn đến dòng được chọn nếu nó không hiển thị
                        try
                        {
                            // Kiểm tra chỉ số hợp lệ trước khi cuộn
                            if (row.Index >= 0 && row.Index < dgvSach.RowCount && !row.Displayed)
                            {
                                int firstDisplayed = Math.Max(0, row.Index - dgvSach.DisplayedRowCount(false) / 2);
                                if (firstDisplayed >= 0 && firstDisplayed < dgvSach.RowCount) // Kiểm tra firstDisplayed hợp lệ
                                {
                                    dgvSach.FirstDisplayedScrollingRowIndex = firstDisplayed; // Cuộn đến dòng
                                    Debug.WriteLine($"SelectRowById({id}): Scrolled to make row {row.Index} visible (FirstDisplayed: {firstDisplayed}).");
                                }
                                else
                                {
                                    Debug.WriteLine($"SelectRowById({id}): Calculated FirstDisplayedScrollingRowIndex ({firstDisplayed}) is out of range. Cannot scroll.");
                                }
                            }
                            else if (row.Displayed) { Debug.WriteLine($"SelectRowById({id}): Row {row.Index} is already displayed."); }
                            else { Debug.WriteLine($"SelectRowById({id}): Cannot scroll to row {row.Index} (Index: {row.Index}, Displayed: {row.Displayed})."); }
                        }
                        catch (Exception exScroll) { Debug.WriteLine($"WARNING: ScrollToRow Error (ID: {id}, Index: {row.Index}): {exScroll.Message}"); }
                        break; // Thoát vòng lặp khi tìm thấy
                    }
                }
                if (!found) Debug.WriteLine($"SelectRowById({id}): Row not found in grid.");
            }
            catch (Exception exSelect) { Debug.WriteLine($"ERROR during SelectRowById({id}): {exSelect.Message}"); found = false; }
            finally
            {
                // Gắn lại sự kiện SelectionChanged
                dgvSach.SelectionChanged += dgvSach_SelectionChanged;
                Debug.WriteLine($"SelectRowById({id}): Re-attached SelectionChanged event.");
                // Kích hoạt SelectionChanged thủ công để đảm bảo DisplaySelectedRow được gọi
                // Chỉ gọi nếu tìm thấy HOẶC không tìm thấy và không có dòng nào đang chọn
                if (found || (!found && dgvSach.SelectedRows.Count == 0))
                {
                    Debug.WriteLine($"SelectRowById({id}): Manually triggering SelectionChanged (found={found}, selectedCount={dgvSach.SelectedRows.Count}).");
                    dgvSach_SelectionChanged(dgvSach, EventArgs.Empty);
                }
                else
                {
                    Debug.WriteLine($"SelectRowById({id}): Not triggering SelectionChanged manually (found={found}, selectedCount={dgvSach.SelectedRows.Count}).");
                }
            }
            Debug.WriteLine($"--- Exiting SelectRowById({id}). Found={found} ---");
            return found;
        }


    } // Kết thúc class ucQuanLySach
} // Kết thúc namespace GUI
