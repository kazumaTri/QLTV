// File: GUI/ucQuanLyPhieuMuonTra.cs
// Nội dung đã được xem xét và điều chỉnh để tăng tính ổn định và khả năng debug.

// --- USING DIRECTIVES ---
using BUS;
using DTO;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

// Namespace của chính project GUI để refer các Forms/UserControl khác nếu cần (ví dụ: frmMain)
using GUI;


namespace GUI // Namespace của project GUI của bạn
{
    /// <summary>
    /// UserControl quản lý các Phiếu mượn trả sách.
    /// Hiển thị danh sách, cho phép lập phiếu mượn mới và ghi nhận trả sách.
    /// Sử dụng Dependency Injection và MaterialSkin.2.
    /// Đã sửa đổi để dùng InitializeDataAsync và áp dụng cách sửa lỗi 'unassigned variable' khác.
    /// </summary>
    public partial class ucQuanLyPhieuMuonTra : UserControl, IRequiresDataLoading
    {
        // --- DEPENDENCIES (Nhận qua Constructor Injection) ---
        private readonly IBUSPhieuMuonTra _busPhieuMuonTra;
        private readonly IBUSDocGia _busDocGia;
        private readonly IBUSCuonSach _busCuonSach;
        private readonly IBUSThamSo _busThamSo;
        private readonly IServiceProvider _serviceProvider;

        // --- STATE ---
        private bool _isAdding = false; // Cờ theo dõi trạng thái Lập phiếu mới
        private List<DocgiaDTO> _docGiaList = new List<DocgiaDTO>();
        private List<CuonSachDTO> _cuonSachAvailableList = new List<CuonSachDTO>();
        private ThamSoDTO? _thamSo;

        /// <summary>
        /// Sự kiện được kích hoạt khi UserControl muốn yêu cầu đóng chính nó.
        /// Form cha (frmMain) sẽ lắng nghe sự kiện này.
        /// </summary>
        public event EventHandler? RequestClose;


        // --- CONSTRUCTOR (Sử dụng Dependency Injection) ---
        public ucQuanLyPhieuMuonTra(IBUSPhieuMuonTra busPhieuMuonTra, IBUSDocGia busDocGia, IBUSCuonSach busCuonSach, IBUSThamSo busThamSo, IServiceProvider serviceProvider)
        {
            InitializeComponent(); // Phương thức được sinh tự động

            _busPhieuMuonTra = busPhieuMuonTra ?? throw new ArgumentNullException(nameof(busPhieuMuonTra));
            _busDocGia = busDocGia ?? throw new ArgumentNullException(nameof(busDocGia));
            _busCuonSach = busCuonSach ?? throw new ArgumentNullException(nameof(busCuonSach));
            _busThamSo = busThamSo ?? throw new ArgumentNullException(nameof(busThamSo));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Phương thức công khai để khởi tạo dữ liệu cho UserControl khi được load bởi frmMain.
        /// </summary>
        public async Task InitializeDataAsync()
        {
            // Không chạy logic trong Design Mode của Visual Studio
            if (this.DesignMode) return;

            Debug.WriteLine("ucQuanLyPhieuMuonTra.InitializeDataAsync() called.");
            _isAdding = false;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                ClearInputFields(); // Xóa input trước
                await LoadThamSo();
                await LoadDocGiaComboBox();
                await LoadCuonSachComboBox();
                await LoadDataGrid(); // LoadDataGrid sẽ gọi UpdateUIState ở finally
            }
            catch (Exception ex)
            {
                HandleError("khởi tạo dữ liệu ban đầu", ex);
                // Cập nhật UI về trạng thái lỗi nếu có thể
                UpdateUIState(); // Gọi để đảm bảo UI phản ánh trạng thái hiện tại (có thể là lỗi)
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }


        // --- SỰ KIỆN LOAD USERCONTROL ---
        private void ucQuanLyPhieuMuonTra_Load(object sender, EventArgs e)
        {
            // Logic trong Load chỉ nên là các cài đặt ban đầu không phụ thuộc dữ liệu async
            if (!this.DesignMode)
            {
                Debug.WriteLine("ucQuanLyPhieuMuonTra_Load fired (DesignMode=false). InitializeDataAsync should be called by parent.");
                // Ví dụ: Cài đặt định dạng DateTimePicker nếu cần
                // dtpNgayMuon.CustomFormat = "dd/MM/yyyy";
                // dtpNgayMuon.Format = DateTimePickerFormat.Custom;
                // dtpHanTra.CustomFormat = "dd/MM/yyyy";
                // dtpHanTra.Format = DateTimePickerFormat.Custom;
                // dtpNgayTra.CustomFormat = "dd/MM/yyyy";
                // dtpNgayTra.Format = DateTimePickerFormat.Custom;
            }
            else
            {
                Debug.WriteLine("ucQuanLyPhieuMuonTra_Load fired (DesignMode=true).");
            }
        }

        // --- HÀM TẢI DỮ LIỆU & CÀI ĐẶT GIAO DIỆN ---

        private async Task LoadThamSo()
        {
            Debug.WriteLine("LoadThamSo() called.");
            try
            {
                _thamSo = await _busThamSo.GetThamSoAsync();
                if (_thamSo == null)
                {
                    ShowWarning("Không tải được tham số hệ thống. Sử dụng giá trị mặc định.");
                }
            }
            catch (Exception ex)
            {
                HandleError("tải tham số hệ thống", ex);
                _thamSo = null; // Đảm bảo thamSo là null nếu có lỗi
            }
        }

        private async Task LoadDocGiaComboBox()
        {
            Debug.WriteLine("LoadDocGiaComboBox() called.");
            try
            {
                if (cboDocGia == null) { Debug.WriteLine("WARNING: cboDocGia is null."); return; }

                object? selectedValue = cboDocGia.SelectedValue; // Lưu giá trị đang chọn (nếu có)
                _docGiaList = await _busDocGia.GetAllDocGiaAsync(includeDeleted: false); // Chỉ tải độc giả chưa xóa
                if (_docGiaList == null)
                {
                    Debug.WriteLine("WARNING: _busDocGia.GetAllDocGiaAsync() returned null.");
                    _docGiaList = new List<DocgiaDTO>();
                }

                cboDocGia.DataSource = null; // Xóa nguồn cũ
                cboDocGia.DataSource = _docGiaList;
                cboDocGia.DisplayMember = nameof(DocgiaDTO.TenDocGia); // Dùng nameof để tránh lỗi chính tả
                cboDocGia.ValueMember = nameof(DocgiaDTO.Id);

                // Cố gắng chọn lại giá trị cũ nếu nó vẫn tồn tại trong danh sách mới
                if (selectedValue != null && selectedValue is int id && _docGiaList.Any(dg => dg.Id == id))
                {
                    cboDocGia.SelectedValue = selectedValue;
                }
                else
                {
                    cboDocGia.SelectedIndex = -1; // Bỏ chọn nếu không tìm thấy
                }
                Debug.WriteLine($"LoadDocGiaComboBox completed. Count: {_docGiaList.Count}");
            }
            catch (Exception ex)
            {
                HandleError("tải danh sách độc giả", ex);
                if (cboDocGia != null) cboDocGia.DataSource = null; // Reset DataSource nếu lỗi
                _docGiaList = new List<DocgiaDTO>();
            }
        }

        private async Task LoadCuonSachComboBox()
        {
            Debug.WriteLine("LoadCuonSachComboBox() called.");
            try
            {
                if (cboCuonSach == null) { Debug.WriteLine("WARNING: cboCuonSach is null."); return; }

                object? selectedValue = cboCuonSach.SelectedValue; // Lưu giá trị đang chọn (nếu có)
                var allCuonSach = await _busCuonSach.GetAllCuonSachAsync();
                if (allCuonSach == null) { Debug.WriteLine("WARNING: _busCuonSach.GetAllCuonSachAsync() returned null."); allCuonSach = new List<CuonSachDTO>(); }

                // Chỉ hiển thị sách có tình trạng là 0 (Sẵn sàng cho mượn) trong combobox khi thêm mới
                _cuonSachAvailableList = allCuonSach.Where(cs => cs.TinhTrang == 0).ToList();

                cboCuonSach.DataSource = null; // Xóa nguồn cũ
                cboCuonSach.DataSource = _cuonSachAvailableList;
                cboCuonSach.DisplayMember = nameof(CuonSachDTO.TenTuaSach); // Dùng nameof
                cboCuonSach.ValueMember = nameof(CuonSachDTO.Id);

                // Cố gắng chọn lại giá trị cũ nếu nó vẫn tồn tại trong danh sách mới
                if (selectedValue != null && selectedValue is int id && _cuonSachAvailableList.Any(cs => cs.Id == id))
                {
                    cboCuonSach.SelectedValue = selectedValue;
                }
                else
                {
                    cboCuonSach.SelectedIndex = -1; // Bỏ chọn nếu không tìm thấy
                }
                Debug.WriteLine($"LoadCuonSachComboBox completed. Available Count: {_cuonSachAvailableList.Count}");
            }
            catch (Exception ex)
            {
                HandleError("tải danh sách cuốn sách có sẵn", ex);
                if (cboCuonSach != null) cboCuonSach.DataSource = null; // Reset DataSource nếu lỗi
                _cuonSachAvailableList = new List<CuonSachDTO>();
            }
        }

        private async Task LoadDataGrid()
        {
            Debug.WriteLine("LoadDataGrid() called.");
            int? selectedId = GetSelectedPhieuMuonTraId(); // Lấy ID đang chọn (nếu có)

            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (dgvPhieuMuonTra == null) { Debug.WriteLine("ERROR: dgvPhieuMuonTra is null."); return; }

                dgvPhieuMuonTra.DataSource = null; // Xóa dữ liệu cũ
                List<PhieuMuonTraDTO> danhSach = await _busPhieuMuonTra.GetAllPhieuMuonTraAsync();

                if (danhSach == null)
                {
                    Debug.WriteLine("WARNING: _busPhieuMuonTra.GetAllPhieuMuonTraAsync() returned null.");
                    danhSach = new List<PhieuMuonTraDTO>();
                }

                dgvPhieuMuonTra.DataSource = danhSach; // Gán nguồn dữ liệu
                SetupDataGridViewColumns(); // Cấu hình cột sau khi gán DataSource

                // Chọn lại dòng trước đó nếu có
                if (selectedId.HasValue)
                {
                    SelectRowById(selectedId.Value);
                }
                else
                {
                    dgvPhieuMuonTra.ClearSelection();
                    // Chỉ xóa input nếu không đang thêm mới (tránh xóa khi đang nhập liệu)
                    if (!_isAdding)
                    {
                        ClearInputFields();
                    }
                }
                Debug.WriteLine($"LoadDataGrid completed. Row count: {danhSach.Count}");
            }
            catch (Exception ex)
            {
                HandleError("tải danh sách phiếu mượn trả", ex);
                if (dgvPhieuMuonTra != null) dgvPhieuMuonTra.DataSource = null; // Đảm bảo lưới rỗng khi lỗi
            }
            finally
            {
                // Quan trọng: Gọi UpdateUIState *sau khi* đã tải xong và chọn lại dòng (hoặc bỏ chọn)
                UpdateUIState();
                this.Cursor = Cursors.Default;
            }
        }

        private int? GetSelectedPhieuMuonTraId()
        {
            if (dgvPhieuMuonTra?.SelectedRows.Count > 0 &&
                dgvPhieuMuonTra.SelectedRows[0].DataBoundItem is PhieuMuonTraDTO currentDto)
            {
                return currentDto.SoPhieuMuonTra;
            }
            return null;
        }


        private void SetupDataGridViewColumns()
        {
            Debug.WriteLine("SetupDataGridViewColumns() called.");
            if (dgvPhieuMuonTra == null || dgvPhieuMuonTra.Columns.Count == 0)
            {
                Debug.WriteLine("WARNING: DGV or Columns are null/empty in SetupDataGridViewColumns.");
                return;
            }
            try
            {
                dgvPhieuMuonTra.AutoGenerateColumns = false; // Rất quan trọng!
                var columns = dgvPhieuMuonTra.Columns;

                // --- Cấu hình từng cột ---
                // Sử dụng nameof để tránh lỗi chính tả khi tên thuộc tính DTO thay đổi
                SetColumnProperties(columns, nameof(PhieuMuonTraDTO.SoPhieuMuonTra), "Số Phiếu", 80);
                SetColumnProperties(columns, nameof(PhieuMuonTraDTO.TenDocGia), "Độc Giả", 150);
                SetColumnProperties(columns, nameof(PhieuMuonTraDTO.MaCuonSach), "Mã Sách", 120);
                SetColumnProperties(columns, nameof(PhieuMuonTraDTO.NgayMuon), "Ngày Mượn", 100, format: "dd/MM/yyyy");
                SetColumnProperties(columns, nameof(PhieuMuonTraDTO.HanTra), "Hạn Trả", 100, format: "dd/MM/yyyy");
                SetColumnProperties(columns, nameof(PhieuMuonTraDTO.NgayTra), "Ngày Trả", 100, format: "dd/MM/yyyy");
                SetColumnProperties(columns, nameof(PhieuMuonTraDTO.SoTienPhat), "Tiền Phạt", 100, alignment: DataGridViewContentAlignment.MiddleRight, format: "N0");
                // --- Ẩn các cột không cần thiết ---
                SetColumnVisibility(columns, nameof(PhieuMuonTraDTO.IdDocGia), false);
                SetColumnVisibility(columns, nameof(PhieuMuonTraDTO.IdCuonSach), false);

                // --- Cài đặt chung cho DataGridView ---
                dgvPhieuMuonTra.ReadOnly = true;
                dgvPhieuMuonTra.AllowUserToAddRows = false;
                dgvPhieuMuonTra.AllowUserToDeleteRows = false;
                dgvPhieuMuonTra.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvPhieuMuonTra.MultiSelect = false;
                dgvPhieuMuonTra.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; // Đặt None để Width có tác dụng
                dgvPhieuMuonTra.RowHeadersVisible = false;

                // Gắn sự kiện định dạng Cell (ví dụ: tô màu dòng quá hạn) - nếu cần
                // dgvPhieuMuonTra.CellFormatting -= DgvPhieuMuonTra_CellFormatting; // Hủy nếu đã gắn trước đó
                // dgvPhieuMuonTra.CellFormatting += DgvPhieuMuonTra_CellFormatting;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in SetupDataGridViewColumns: {ex.Message}");
                // Không nên throw lỗi ở đây để tránh crash chương trình
            }
        }

        // Helper cấu hình cột
        private void SetColumnProperties(DataGridViewColumnCollection columns, string columnName, string headerText, int width,
                                         DataGridViewAutoSizeColumnMode autoSizeMode = DataGridViewAutoSizeColumnMode.None,
                                         DataGridViewContentAlignment alignment = DataGridViewContentAlignment.MiddleLeft,
                                         string? format = null)
        {
            if (columns.Contains(columnName))
            {
                var column = columns[columnName];
                column.HeaderText = headerText;
                column.Width = width;
                column.AutoSizeMode = autoSizeMode;
                column.DefaultCellStyle.Alignment = alignment;
                if (!string.IsNullOrEmpty(format))
                {
                    column.DefaultCellStyle.Format = format;
                }
                column.Visible = true; // Mặc định là hiển thị
            }
            else
            {
                Debug.WriteLine($"WARNING: Column '{columnName}' not found during setup.");
            }
        }

        // Helper ẩn/hiện cột
        private void SetColumnVisibility(DataGridViewColumnCollection columns, string columnName, bool visible)
        {
            if (columns.Contains(columnName))
            {
                columns[columnName].Visible = visible;
            }
            else
            {
                Debug.WriteLine($"WARNING: Column '{columnName}' not found for visibility setting.");
            }
        }

        // --- HÀM TIỆN ÍCH TRẠNG THÁI & GIAO DIỆN ---

        private bool IsEditing() { return false; } // Giả định không sửa phiếu trong phiên bản này

        private void ClearInputFields()
        {
            Debug.WriteLine("ClearInputFields() called.");
            if (txtSoPhieuMuonTra != null) txtSoPhieuMuonTra.Clear();
            if (dtpNgayMuon != null) dtpNgayMuon.Value = DateTime.Now; // Mặc định ngày hiện tại
            // Tính hạn trả mặc định dựa trên tham số (nếu có)
            int soNgayMuon = _thamSo?.SoNgayMuonToiDa ?? 14; // Mặc định 14 ngày nếu không có tham số
            if (dtpHanTra != null) { dtpHanTra.Value = DateTime.Now.AddDays(soNgayMuon); }

            if (cboDocGia != null) cboDocGia.SelectedIndex = -1;
            if (cboCuonSach != null) cboCuonSach.SelectedIndex = -1;

            // Reset phần thông tin trả sách
            if (dtpNgayTra != null) dtpNgayTra.Value = DateTime.Now; // Hiển thị ngày hiện tại
            if (txtSoTienPhat != null) txtSoTienPhat.Text = "0"; // Reset về 0

            // Focus vào ô đầu tiên khi bắt đầu thêm mới
            if (_isAdding && cboDocGia != null && cboDocGia.Enabled) cboDocGia.Focus();
        }

        /// <summary>
        /// Cập nhật trạng thái giao diện dựa trên trạng thái hiện tại (_isAdding, rowSelected).
        /// Gọi sau mỗi lần thay đổi trạng thái hoặc tải dữ liệu.
        /// </summary>
        private void UpdateUIState()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(UpdateUIState);
                return;
            }

            bool rowSelected = dgvPhieuMuonTra?.SelectedRows.Count > 0;
            Debug.WriteLine($"UpdateUIState called. IsAdding: {_isAdding}, RowSelected: {rowSelected}");

            try
            {
                SetControlState(_isAdding, rowSelected);

                // Chỉ hiển thị dữ liệu chi tiết nếu *không* đang thêm mới
                if (!_isAdding)
                {
                    DisplaySelectedRow();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in UpdateUIState: {ex}");
                HandleError("cập nhật trạng thái giao diện", ex);
            }
        }


        /// <summary>
        /// Cập nhật trạng thái Enabled/Disabled và Visible của các controls.
        /// Được gọi bởi UpdateUIState.
        /// </summary>
        private void SetControlState(bool isAdding, bool rowSelected)
        {
            Debug.WriteLine($"--- SetControlState START (isAdding: {isAdding}, rowSelected: {rowSelected}) ---");

            // Kiểm tra sự tồn tại của các controls quan trọng
            if (btnThem == null || btnLuu == null || btnBoQua == null || btnThoat == null ||
                txtSoPhieuMuonTra == null || dtpNgayMuon == null || dtpHanTra == null ||
                cboDocGia == null || cboCuonSach == null || dtpNgayTra == null ||
                txtSoTienPhat == null || dgvPhieuMuonTra == null || btnTraSach == null)
            {
                Debug.WriteLine("SetControlState - CRITICAL: One or more essential controls are null. Aborting UI update.");
                return; // Thoát nếu controls chưa sẵn sàng
            }

            // 1. Trạng thái cơ bản dựa trên isAdding (Chế độ Lập phiếu / Xem danh sách)
            btnThem.Enabled = !isAdding;
            btnLuu.Enabled = isAdding;
            btnBoQua.Enabled = isAdding;
            btnThoat.Enabled = true; // Luôn bật nút Thoát

            // Các ô nhập liệu cho phiếu mới
            txtSoPhieuMuonTra.Enabled = false; // Số phiếu luôn tắt (tự sinh hoặc hiển thị)
            dtpNgayMuon.Enabled = isAdding;
            dtpHanTra.Enabled = isAdding;
            cboDocGia.Enabled = isAdding;
            cboCuonSach.Enabled = isAdding;

            // Các ô hiển thị chi tiết trả sách (luôn tắt vì chỉ hiển thị)
            dtpNgayTra.Enabled = false;
            txtSoTienPhat.Enabled = false;

            // DataGridView chỉ bật khi xem danh sách
            dgvPhieuMuonTra.Enabled = !isAdding;

            // 2. Trạng thái phụ thuộc vào dòng được chọn (khi không ở chế độ isAdding)
            bool isReturned = false;
            bool canReturn = false;
            PhieuMuonTraDTO? currentDto = null;
            bool showReturnDetailsPanel = false;

            if (!isAdding && rowSelected)
            {
                currentDto = dgvPhieuMuonTra.SelectedRows[0].DataBoundItem as PhieuMuonTraDTO;
                if (currentDto != null)
                {
                    isReturned = currentDto.NgayTra.HasValue; // Kiểm tra đã trả chưa
                    canReturn = !isReturned; // Có thể trả nếu chưa trả
                    showReturnDetailsPanel = true; // Hiển thị panel chi tiết khi có dòng chọn
                    Debug.WriteLine($"SetControlState - Selected DTO: SoPhieu={currentDto.SoPhieuMuonTra}, IsReturned={isReturned}");
                }
                else
                {
                    Debug.WriteLine("SetControlState - Row selected but failed to cast DataBoundItem to PhieuMuonTraDTO.");
                    // Giữ giá trị mặc định (isReturned=false, canReturn=false, showReturnDetailsPanel=false)
                }
            }
            else if (!isAdding && !rowSelected) // Không thêm mới và không có dòng nào chọn
            {
                Debug.WriteLine("SetControlState - Not adding and no row selected.");
                // Giữ giá trị mặc định
            }

            // Nút Trả sách: Bật khi (không thêm mới) VÀ (có dòng chọn hợp lệ) VÀ (chưa trả)
            btnTraSach.Enabled = !isAdding && currentDto != null && canReturn;

            // Panel chi tiết trả sách (`panelReturnDetails`)
            try
            {
                // Sử dụng Controls.Find để tìm panel an toàn
                var panel = this.Controls.Find("panelReturnDetails", true).FirstOrDefault() as Panel;
                if (panel != null)
                {
                    // Hiển thị khi (không thêm mới) VÀ (có dòng được chọn hợp lệ)
                    panel.Visible = showReturnDetailsPanel;
                    Debug.WriteLine($"SetControlState - panelReturnDetails.Visible = {panel.Visible}");
                }
                else
                {
                    Debug.WriteLine("SetControlState - WARNING: panelReturnDetails not found by Controls.Find.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SetControlState - ERROR finding/setting visibility for panelReturnDetails: {ex.Message}");
                // Bỏ qua lỗi nếu không tìm thấy panel để tránh crash
            }

            // Ghi log trạng thái các nút chính
            Debug.WriteLine($"SetControlState - btnThem.Enabled: {btnThem.Enabled}");
            Debug.WriteLine($"SetControlState - btnLuu.Enabled: {btnLuu.Enabled}");
            Debug.WriteLine($"SetControlState - btnBoQua.Enabled: {btnBoQua.Enabled}");
            Debug.WriteLine($"SetControlState - btnTraSach.Enabled: {btnTraSach.Enabled}");
            Debug.WriteLine($"SetControlState - btnThoat.Enabled: {btnThoat.Enabled}");
            Debug.WriteLine($"--- SetControlState END ---");
        }


        /// <summary>
        /// Hiển thị dữ liệu của dòng được chọn lên các controls nhập liệu (chỉ khi không isAdding).
        /// </summary>
        private async void DisplaySelectedRow()
        {
            Debug.WriteLine("DisplaySelectedRow() called.");
            // Đảm bảo không chạy khi đang thêm mới
            if (_isAdding)
            {
                Debug.WriteLine("DisplaySelectedRow() - Skipping because IsAdding is true.");
                return;
            }

            PhieuMuonTraDTO? dto = null;

            // Lấy DTO từ dòng đang chọn một cách an toàn
            if (dgvPhieuMuonTra?.SelectedRows.Count == 1)
            {
                dto = dgvPhieuMuonTra.SelectedRows[0].DataBoundItem as PhieuMuonTraDTO;
            }

            // Nếu không lấy được DTO hợp lệ -> Xóa trắng input và thoát
            if (dto == null)
            {
                Debug.WriteLine("DisplaySelectedRow: No valid row/DTO selected, clearing input fields.");
                ClearInputFields();
                // Không cần gọi UpdateUIState ở đây vì nó đã được gọi trước khi vào DisplaySelectedRow
                return;
            }

            // --- Nếu có DTO hợp lệ, hiển thị dữ liệu ---
            Debug.WriteLine($"DisplaySelectedRow - Displaying data for SoPhieu: {dto.SoPhieuMuonTra}");
            try
            {
                // Gán giá trị cơ bản
                if (txtSoPhieuMuonTra != null) txtSoPhieuMuonTra.Text = dto.SoPhieuMuonTra.ToString();

                // Độc giả: Chọn trong ComboBox hoặc tải nếu chưa có
                if (cboDocGia != null)
                {
                    if (_docGiaList.Any(dg => dg.Id == dto.IdDocGia))
                    {
                        cboDocGia.SelectedValue = dto.IdDocGia;
                    }
                    else
                    {
                        await AddAndSelectDocGia(dto.IdDocGia); // Tải và chọn độc giả nếu cần
                    }
                }

                // Cuốn sách: Hiển thị mã sách (không cho chọn lại)
                if (cboCuonSach != null)
                {
                    // Không chọn trong combobox sách available
                    cboCuonSach.SelectedIndex = -1;
                    await DisplayBorrowedCuonSachInfo(dto.IdCuonSach); // Hiển thị thông tin sách đã mượn
                }

                // Ngày tháng (kiểm tra giá trị hợp lệ cho DateTimePicker)
                if (dtpNgayMuon != null) dtpNgayMuon.Value = SafeGetDateTime(dto.NgayMuon, dtpNgayMuon.MinDate, dtpNgayMuon.MaxDate);
                if (dtpHanTra != null) dtpHanTra.Value = SafeGetDateTime(dto.HanTra, dtpHanTra.MinDate, dtpHanTra.MaxDate);

                // Thông tin trả sách
                bool daTra = dto.NgayTra.HasValue;
                if (daTra)
                {
                    if (dtpNgayTra != null) dtpNgayTra.Value = SafeGetDateTime(dto.NgayTra.Value, dtpNgayTra.MinDate, dtpNgayTra.MaxDate);
                    if (txtSoTienPhat != null) txtSoTienPhat.Text = (dto.SoTienPhat ?? 0).ToString("N0");
                }
                else // Nếu chưa trả
                {
                    if (dtpNgayTra != null) dtpNgayTra.Value = DateTime.Now; // Hiển thị ngày hiện tại (control disabled)
                    if (txtSoTienPhat != null) txtSoTienPhat.Text = "0"; // Hiển thị 0 (control disabled)
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in DisplaySelectedRow: {ex}");
                HandleError("hiển thị chi tiết phiếu mượn", ex);
                ClearInputFields(); // Xóa input nếu có lỗi khi hiển thị
            }
        }

        // Helper gán DateTime an toàn cho DateTimePicker
        private DateTime SafeGetDateTime(DateTime value, DateTime min, DateTime max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }


        // Helper để xử lý load và chọn độc giả không có trong list ban đầu (ví dụ độc giả đã bị xóa)
        private async Task AddAndSelectDocGia(int idDocGia)
        {
            if (cboDocGia == null) return;
            Debug.WriteLine($"AddAndSelectDocGia attempting for ID: {idDocGia}");

            // Kiểm tra lại lần nữa nếu độc giả đã có trong list (có thể do tải đồng thời)
            if (_docGiaList.Any(d => d.Id == idDocGia))
            {
                // Đã có, chỉ cần chọn
                if (cboDocGia.Items.Cast<DocgiaDTO>().Any(d => d.Id == idDocGia)) // Kiểm tra trong items thực tế
                {
                    cboDocGia.SelectedValue = idDocGia;
                    Debug.WriteLine($"DocGia ID {idDocGia} found in existing list/items. Selected.");
                }
                else // Có trong _docGiaList nhưng chưa có trong items? -> Reload DataSource
                {
                    Debug.WriteLine($"DocGia ID {idDocGia} found in list but not items. Reloading DataSource.");
                    cboDocGia.DataSource = null;
                    cboDocGia.DataSource = _docGiaList;
                    cboDocGia.SelectedValue = idDocGia;
                }
                return;
            }

            // Nếu chưa có, thử tải thông tin độc giả (bao gồm cả đã xóa để hiển thị lịch sử)
            try
            {
                var docGiaInfo = await _busDocGia.GetDocGiaByIdAsync(idDocGia, includeDeleted: true);
                if (docGiaInfo != null)
                {
                    _docGiaList.Add(docGiaInfo); // Thêm vào danh sách nguồn
                    // Cập nhật lại DataSource của ComboBox
                    cboDocGia.DataSource = null;
                    cboDocGia.DataSource = _docGiaList;
                    cboDocGia.DisplayMember = nameof(DocgiaDTO.TenDocGia);
                    cboDocGia.ValueMember = nameof(DocgiaDTO.Id);
                    cboDocGia.SelectedValue = idDocGia; // Chọn độc giả vừa thêm
                    Debug.WriteLine($"Successfully added and selected missing DocGia ID {idDocGia}.");
                }
                else
                {
                    cboDocGia.SelectedIndex = -1; // Không tìm thấy, bỏ chọn
                    Debug.WriteLine($"WARNING: Could not find DocGia ID {idDocGia} via BUS.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in AddAndSelectDocGia for ID {idDocGia}: {ex}");
                HandleError($"tải thông tin độc giả ID {idDocGia}", ex);
                cboDocGia.SelectedIndex = -1;
            }
        }

        // Helper để hiển thị thông tin sách đã mượn (thay vì chọn trong combobox available)
        private async Task DisplayBorrowedCuonSachInfo(int idCuonSach)
        {
            // Có thể thêm một MaterialTextBox2 chỉ đọc (ví dụ: txtMaSachDisplay) vào form designer
            // để hiển thị mã sách đã mượn thay vì để trống ComboBox.

            MaterialTextBox2? txtMaSachDisplay = this.Controls.Find("txtMaSachDisplay", true).FirstOrDefault() as MaterialTextBox2;

            if (txtMaSachDisplay == null)
            {
                Debug.WriteLine("DisplayBorrowedCuonSachInfo - txtMaSachDisplay control not found.");
                // Nếu không có control hiển thị, chỉ cần đảm bảo combobox không chọn gì
                if (cboCuonSach != null) cboCuonSach.SelectedIndex = -1;
                return;
            }

            // Luôn ẩn combobox và hiện textbox khi hiển thị sách đã mượn
            if (cboCuonSach != null) cboCuonSach.Visible = false;
            txtMaSachDisplay.Visible = true;
            txtMaSachDisplay.ReadOnly = true; // Đảm bảo chỉ đọc

            try
            {
                var cuonSach = await _busCuonSach.GetCuonSachByIdAsync(idCuonSach);
                if (cuonSach != null)
                {
                    txtMaSachDisplay.Text = cuonSach.MaCuonSach ?? "Không rõ";
                    txtMaSachDisplay.Hint = "Mã sách đã mượn"; // Cập nhật Hint
                    Debug.WriteLine($"DisplayBorrowedCuonSachInfo - Displaying MaCuonSach: {cuonSach.MaCuonSach}");
                }
                else
                {
                    txtMaSachDisplay.Text = $"Không tìm thấy (ID: {idCuonSach})";
                    Debug.WriteLine($"WARNING: Could not find CuonSach ID {idCuonSach} via BUS.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR loading CuonSach ID {idCuonSach}: {ex}");
                HandleError($"tải thông tin cuốn sách ID {idCuonSach}", ex);
                txtMaSachDisplay.Text = "Lỗi tải dữ liệu";
            }
        }

        // Helper để chuyển đổi giữa ComboBox và TextBox hiển thị sách
        private void ShowAvailableSachInput(bool showComboBox)
        {
            MaterialTextBox2? txtMaSachDisplay = this.Controls.Find("txtMaSachDisplay", true).FirstOrDefault() as MaterialTextBox2;

            if (cboCuonSach != null) cboCuonSach.Visible = showComboBox;
            if (txtMaSachDisplay != null) txtMaSachDisplay.Visible = !showComboBox;

            if (showComboBox && cboCuonSach != null)
            {
                cboCuonSach.SelectedIndex = -1; // Reset lựa chọn khi quay lại combobox
            }
            else if (!showComboBox && txtMaSachDisplay != null)
            {
                txtMaSachDisplay.Clear(); // Xóa textbox khi ẩn đi
            }
        }


        private bool HasUnsavedChanges()
        {
            // Chỉ kiểm tra khi đang thêm mới
            if (!_isAdding) return false;

            // Có thay đổi nếu đã chọn độc giả HOẶC đã chọn cuốn sách
            bool docGiaSelected = (cboDocGia?.SelectedIndex ?? -1) > -1;
            bool cuonSachSelected = (cboCuonSach?.SelectedIndex ?? -1) > -1;

            // Có thể kiểm tra thêm ngày tháng nếu giá trị mặc định khác DateTime.Now
            // bool dateChanged = (dtpNgayMuon?.Value.Date != DateTime.Today) || ...

            return docGiaSelected || cuonSachSelected;
        }

        // --- BUTTON EVENT HANDLERS ---

        private void btnThem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnThem_Click called.");
            _isAdding = true;
            ShowAvailableSachInput(true); // Hiển thị ComboBox sách khi thêm
            ClearInputFields();
            dgvPhieuMuonTra?.ClearSelection();
            UpdateUIState(); // Cập nhật UI cho trạng thái thêm mới
            cboDocGia?.Focus();
        }

        private async void btnTraSach_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnTraSach_Click called.");
            int? soPhieuToReturn = GetSelectedPhieuMuonTraId();

            // Chỉ thực hiện nếu nút Trả sách đang bật VÀ lấy được ID phiếu hợp lệ
            if (btnTraSach?.Enabled == true && soPhieuToReturn.HasValue)
            {
                // Xác nhận lại
                var confirm = MaterialMessageBox.Show(
                    text: $"Bạn chắc chắn muốn ghi nhận trả sách cho phiếu {soPhieuToReturn.Value}?",
                    caption: "Xác nhận Trả sách",
                    buttons: MessageBoxButtons.YesNo,
                    icon: MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    bool success = false;
                    string? errorMsg = null;
                    if (btnTraSach != null) btnTraSach.Enabled = false; // Tạm thời vô hiệu hóa nút
                    this.Cursor = Cursors.WaitCursor;

                    try
                    {
                        success = await _busPhieuMuonTra.ProcessReturnAsync(soPhieuToReturn.Value);
                    }
                    catch (InvalidOperationException ex) { errorMsg = $"Lỗi nghiệp vụ: {ex.Message}"; }
                    catch (Exception ex) { errorMsg = $"Lỗi hệ thống: {ex.Message}"; Debug.WriteLine($"ERROR (ProcessReturnAsync): {ex}"); }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                        // Không cần bật lại nút ở đây, LoadDataGrid sẽ gọi UpdateUIState
                    }

                    if (success)
                    {
                        ShowInfo("Trả sách thành công!");
                        await LoadDataGrid(); // Tải lại lưới (sẽ gọi UpdateUIState)
                        SelectRowById(soPhieuToReturn.Value); // Chọn lại dòng vừa trả
                    }
                    else // Nếu thất bại
                    {
                        ShowError(errorMsg ?? "Trả sách thất bại do lỗi không xác định.");
                        // Cần cập nhật lại UI để bật lại nút Trả sách nếu cần
                        UpdateUIState();
                    }
                }
            }
            else if (soPhieuToReturn == null) // Không có dòng nào được chọn
            {
                ShowWarning("Vui lòng chọn phiếu cần ghi nhận trả sách.");
            }
            else // Nút Trả sách không bật (có thể do phiếu đã trả rồi)
            {
                Debug.WriteLine("btnTraSach_Click ignored - button disabled or invalid state.");
            }
        }

        private async void btnLuu_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnLuu_Click called.");
            if (!_isAdding || !(btnLuu?.Enabled ?? false))
            {
                Debug.WriteLine("btnLuu_Click ignored - not adding or button disabled.");
                return;
            }

            // --- Lấy dữ liệu từ controls ---
            int idDocGia = (cboDocGia?.SelectedValue as int?) ?? 0;
            int idCuonSach = (cboCuonSach?.SelectedValue as int?) ?? 0;
            DateTime ngayMuon = dtpNgayMuon?.Value ?? DateTime.Now;
            DateTime hanTra = dtpHanTra?.Value ?? DateTime.Now;

            // --- Validate dữ liệu ---
            string? validationError = ValidateInputForAdd(idDocGia, idCuonSach, ngayMuon, hanTra);
            if (validationError != null)
            {
                ShowWarning(validationError);
                // Focus vào control bị lỗi (có thể cải thiện bằng cách trả về control từ hàm validate)
                if (validationError.Contains("độc giả")) cboDocGia?.Focus();
                else if (validationError.Contains("cuốn sách")) cboCuonSach?.Focus();
                else if (validationError.Contains("Ngày mượn")) dtpNgayMuon?.Focus();
                else if (validationError.Contains("Hạn trả")) dtpHanTra?.Focus();
                return;
            }

            // --- Tạo DTO và gọi BUS ---
            PhieuMuonTraDTO phieuMuonTraDto = new PhieuMuonTraDTO
            {
                IdDocGia = idDocGia,
                IdCuonSach = idCuonSach,
                NgayMuon = ngayMuon.Date, // Chỉ lấy phần ngày
                HanTra = hanTra.Date,     // Chỉ lấy phần ngày
                NgayTra = null,
                SoTienPhat = 0
            };

            bool success = false; string? errorMsg = null; int? savedPhieuId = null;
            if (btnLuu != null) btnLuu.Enabled = false; if (btnBoQua != null) btnBoQua.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            try
            {
                var addedDto = await _busPhieuMuonTra.AddPhieuMuonTraAsync(phieuMuonTraDto);
                success = addedDto != null;
                if (success) savedPhieuId = addedDto!.SoPhieuMuonTra;
            }
            catch (ArgumentException ex) { errorMsg = $"Lỗi dữ liệu nhập: {ex.Message}"; }
            catch (InvalidOperationException ex) { errorMsg = $"Lỗi nghiệp vụ: {ex.Message}"; }
            catch (Exception ex) { errorMsg = $"Lỗi hệ thống khi lưu phiếu: {ex.Message}"; Debug.WriteLine($"ERROR (AddPhieuMuonTraAsync): {ex}"); }
            finally
            {
                this.Cursor = Cursors.Default;
                // Không bật lại nút Lưu/Bỏ qua ở đây nếu thành công, vì sẽ thoát chế độ Add
            }

            // --- Xử lý kết quả ---
            if (success)
            {
                ShowInfo("Lập phiếu thành công!");
                _isAdding = false; // Thoát khỏi chế độ thêm
                await LoadCuonSachComboBox(); // Tải lại sách available
                await LoadDataGrid(); // Tải lại lưới (sẽ gọi UpdateUIState)
                if (savedPhieuId.HasValue) { SelectRowById(savedPhieuId.Value); } // Chọn dòng vừa thêm
            }
            else // Nếu có lỗi
            {
                ShowError(errorMsg ?? "Lập phiếu thất bại do lỗi không xác định.");
                // Bật lại các nút Lưu, Bỏ qua để người dùng thử lại
                if (btnLuu != null) btnLuu.Enabled = true;
                if (btnBoQua != null) btnBoQua.Enabled = true;
            }
        }

        // Hàm kiểm tra dữ liệu nhập khi thêm phiếu
        private string? ValidateInputForAdd(int idDocGia, int idCuonSach, DateTime ngayMuon, DateTime hanTra)
        {
            if (idDocGia <= 0) return "Vui lòng chọn độc giả.";
            if (idCuonSach <= 0) return "Vui lòng chọn cuốn sách.";
            if (ngayMuon.Date > DateTime.Today) return "Ngày mượn không được trong tương lai.";
            if (hanTra.Date < ngayMuon.Date) return "Hạn trả không được trước ngày mượn.";

            // Kiểm tra hạn trả so với quy định
            int soNgayMuonMax = _thamSo?.SoNgayMuonToiDa ?? 14; // Lấy từ tham số hoặc mặc định
            if ((hanTra.Date - ngayMuon.Date).TotalDays > soNgayMuonMax)
            {
                return $"Thời hạn mượn không được quá {soNgayMuonMax} ngày.";
            }
            return null; // Hợp lệ
        }

        private void btnBoQua_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnBoQua_Click called.");
            if (!_isAdding || !(btnBoQua?.Enabled ?? false)) return;

            // Xác nhận nếu có thay đổi chưa lưu
            if (HasUnsavedChanges())
            {
                var confirm = MaterialMessageBox.Show(
                    text: "Dữ liệu chưa được lưu. Bạn có muốn hủy bỏ thao tác lập phiếu không?",
                    caption: "Xác nhận Hủy",
                    buttons: MessageBoxButtons.YesNo,
                    icon: MessageBoxIcon.Question);

                if (confirm == DialogResult.No)
                {
                    return; // Người dùng không muốn hủy
                }
            }

            _isAdding = false; // Thoát chế độ thêm
            ShowAvailableSachInput(false); // Ẩn ComboBox sách, hiện lại Textbox nếu có

            // Tải lại trạng thái UI dựa trên dòng đang chọn (nếu có) hoặc xóa trắng
            UpdateUIState();

            // Nếu sau khi UpdateUIState mà không có dòng nào được chọn, thì xóa trắng input
            if (dgvPhieuMuonTra?.SelectedRows.Count == 0)
            {
                ClearInputFields();
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnThoat_Click called.");
            // Xác nhận nếu đang thêm và có thay đổi chưa lưu
            if (_isAdding && HasUnsavedChanges())
            {
                var confirm = MaterialMessageBox.Show(
                    text: "Dữ liệu lập phiếu chưa được lưu. Bạn có chắc chắn muốn thoát không?",
                    caption: "Xác nhận Thoát",
                    buttons: MessageBoxButtons.YesNo,
                    icon: MessageBoxIcon.Warning);

                if (confirm == DialogResult.No)
                {
                    return; // Không thoát
                }
            }
            _isAdding = false; // Đảm bảo thoát khỏi chế độ thêm
            // Kích hoạt sự kiện để thông báo cho frmMain yêu cầu đóng UserControl này
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        // --- DATAGRIDVIEW EVENT HANDLERS ---

        private void dgvPhieuMuonTra_SelectionChanged(object sender, EventArgs e)
        {
            // Chỉ xử lý khi không đang thêm mới để tránh ghi đè dữ liệu đang nhập
            if (!_isAdding)
            {
                Debug.WriteLine("dgvPhieuMuonTra_SelectionChanged called (not adding). Updating UI State.");
                // Quan trọng: Phải gọi UpdateUIState để cập nhật nút Trả sách và panelReturnDetails
                UpdateUIState();
            }
            else
            {
                Debug.WriteLine("dgvPhieuMuonTra_SelectionChanged called (is adding - ignored).");
            }
        }

        private void dgvPhieuMuonTra_DoubleClick(object sender, EventArgs e)
        {
            Debug.WriteLine("dgvPhieuMuonTra_DoubleClick called.");
            // Hành động double click tương đương với nhấn nút Trả sách (nếu nút đó đang bật)
            if (btnTraSach != null && btnTraSach.Enabled)
            {
                Debug.WriteLine("Simulating btnTraSach_Click on DoubleClick.");
                btnTraSach_Click(sender, e);
            }
            else
            {
                Debug.WriteLine("Ignoring DoubleClick, Return button disabled or null.");
            }
        }

        // --- HELPER METHODS ---
        private void SelectRowById(int soPhieuMuonTra)
        {
            Debug.WriteLine($"SelectRowById({soPhieuMuonTra}) called.");
            if (dgvPhieuMuonTra == null || dgvPhieuMuonTra.Rows.Count == 0)
            {
                Debug.WriteLine("DGV not ready or empty for selection.");
                dgvPhieuMuonTra?.ClearSelection(); // Đảm bảo bỏ chọn nếu lưới rỗng
                return;
            }

            dgvPhieuMuonTra.ClearSelection(); // Xóa lựa chọn cũ trước khi tìm
            bool found = false;
            foreach (DataGridViewRow row in dgvPhieuMuonTra.Rows)
            {
                if (row.DataBoundItem is PhieuMuonTraDTO dto && dto.SoPhieuMuonTra == soPhieuMuonTra)
                {
                    row.Selected = true; // Chọn dòng
                    found = true;
                    // Cuộn đến dòng được chọn (đảm bảo index hợp lệ và không lỗi)
                    try
                    {
                        // Chỉ cuộn nếu row index hợp lệ
                        if (row.Index >= 0 && row.Index < dgvPhieuMuonTra.RowCount)
                        {
                            // Đảm bảo FirstDisplayedScrollingRowIndex không âm
                            int scrollIndex = Math.Max(0, row.Index - dgvPhieuMuonTra.DisplayedRowCount(false) / 2);
                            // Kiểm tra lần nữa trước khi gán
                            if (scrollIndex < dgvPhieuMuonTra.RowCount)
                            {
                                dgvPhieuMuonTra.FirstDisplayedScrollingRowIndex = scrollIndex;
                            }
                            else // Nếu scrollIndex tính ra bị sai, cuộn về đầu
                            {
                                dgvPhieuMuonTra.FirstDisplayedScrollingRowIndex = 0;
                            }
                        }
                    }
                    catch (Exception ex) { Debug.WriteLine($"Error scrolling to selected row (Index: {row.Index}): {ex.Message}"); }
                    Debug.WriteLine($"Row with ID {soPhieuMuonTra} found and selected at index {row.Index}.");
                    break; // Thoát vòng lặp khi tìm thấy
                }
            }

            if (!found)
            {
                Debug.WriteLine($"Row with ID {soPhieuMuonTra} not found.");
                // Không cần ClearSelection nữa vì đã làm ở đầu
                // Nếu không tìm thấy, trạng thái UI sẽ tự cập nhật (không có dòng chọn)
                // thông qua UpdateUIState được gọi sau LoadDataGrid hoặc các thao tác khác.
            }
            // Sự kiện SelectionChanged sẽ được kích hoạt (hoặc không nếu không tìm thấy)
            // và gọi UpdateUIState để hoàn tất cập nhật giao diện.
        }

        // --- Các hàm hiển thị thông báo ---
        private void HandleError(string action, Exception ex)
        {
            var mainForm = this.FindForm();
            string message = $"Lỗi khi {action}: {ex.Message}";
            Debug.WriteLine($"ERROR ({action}): {ex}");
        }
        private void ShowError(string message)
        {
            var mainForm = this.FindForm();
            Debug.WriteLine($"UI Error: {message}");
        }
        private void ShowWarning(string message)
        {
            var mainForm = this.FindForm();
            Debug.WriteLine($"UI Warning: {message}");
        }
        private void ShowInfo(string message)
        {
            var mainForm = this.FindForm();
            Debug.WriteLine($"UI Info: {message}");
        }


    } // End class ucQuanLyPhieuMuonTra
} // End namespace GUI