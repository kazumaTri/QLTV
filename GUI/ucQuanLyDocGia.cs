// File: GUI/ucQuanLyDocGia.cs
// --- Đảm bảo bạn đã có các using này hoặc tương đương ---
using BUS; // Namespace chứa các Interface BUS (IBUSDocGia, IBUSLoaiDocGia, IBUSPhieuMuonTra)
using DTO; // Namespace chứa các DTO (DocgiaDTO, LoaiDocGiaDTO, PhieuMuonTraDTO, PhieuThuPhatDTO)
using MaterialSkin.Controls; // Cho Material controls
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing; // Cho Point, Size (có thể đã có trong Designer)
using System.Linq;
using System.Text; // Cho StringBuilder
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection; // Cho IServiceProvider
using Microsoft.EntityFrameworkCore;
using GUI; // <<< ĐÃ THÊM using này

// *** THÊM USING STATIC CHO INTERFACE NẾU CẦN ***
// Ví dụ: Nếu IRequiresDataLoading định nghĩa trong frmMain
using static GUI.frmMain;

namespace GUI
{
    /// <summary>
    /// UserControl quản lý thông tin Độc giả.
    /// Hỗ trợ tìm kiếm, thêm, sửa, xóa mềm, khôi phục, xem lịch sử mượn trả, xem chi tiết nợ và thu phạt.
    /// </summary>
    public partial class ucQuanLyDocGia : UserControl, IRequiresDataLoading
    {
        // --- Dependencies ---
        private readonly IBUSDocGia _busDocGia;
        private readonly IBUSLoaiDocGia _busLoaiDocGia;
        private readonly IBUSPhieuMuonTra _busPhieuMuonTra;
        private readonly IServiceProvider _serviceProvider;

        // --- State ---
        private bool _isAdding = false;
        private DocgiaDTO? _currentSelectedDTO; // DTO gốc KHI BẮT ĐẦU SỬA
        private List<LoaiDocGiaDTO> _loaiDocGiaList = new List<LoaiDocGiaDTO>();
        private bool _showDeleted = false;

        // --- Constants for Filter ComboBoxes ---
        private const int FILTER_ID_TAT_CA = 0; // ID đại diện cho "Tất cả" loại độc giả
        private const string FILTER_TRANGTHAI_TAT_CA = "Tất cả trạng thái";
        private const string FILTER_TRANGTHAI_CON_HAN = "Còn hạn";
        private const string FILTER_TRANGTHAI_HET_HAN = "Hết hạn";

        // --- Events ---
        public event EventHandler? RequestClose; // Event để yêu cầu form cha đóng UC này

        // --- Constructor ---
        public ucQuanLyDocGia(
            IBUSDocGia busDocGia,
            IBUSLoaiDocGia busLoaiDocGia,
            IBUSPhieuMuonTra busPhieuMuonTra,
            IServiceProvider serviceProvider)
        {
            InitializeComponent(); // Hàm này do Designer tạo

            // Inject dependencies
            _busDocGia = busDocGia ?? throw new ArgumentNullException(nameof(busDocGia));
            _busLoaiDocGia = busLoaiDocGia ?? throw new ArgumentNullException(nameof(busLoaiDocGia));
            _busPhieuMuonTra = busPhieuMuonTra ?? throw new ArgumentNullException(nameof(busPhieuMuonTra));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            // Gắn các sự kiện
            AssignChangeEventHandlers();
            AssignFilterEventHandlers();
            AssignActionEventHandlers(); // <<< Gắn sự kiện cho nút mới
        }

        // Gán các sự kiện thay đổi để kiểm tra HasUnsavedChanges
        private void AssignChangeEventHandlers()
        {
            txtTenDocGia.TextChanged += InputField_Changed;
            txtDiaChi.TextChanged += InputField_Changed;
            txtDienThoai.TextChanged += InputField_Changed;
            txtEmail.TextChanged += InputField_Changed;
            txtTenDangNhap.TextChanged += InputField_Changed;
            txtMatKhau.TextChanged += InputField_Changed;
            dtpNgaySinh.ValueChanged += InputField_Changed;
            dtpNgayLapThe.ValueChanged += InputField_Changed;
            dtpNgayHetHan.ValueChanged += InputField_Changed;
            cboLoaiDocGia.SelectedIndexChanged += InputField_Changed;
            // Sự kiện cho ô nhập tiền phạt (chỉ cần nếu muốn validate real-time)
            // txtSoTienThu.TextChanged += InputField_Changed;
        }

        // Gán sự kiện cho ComboBox lọc
        private void AssignFilterEventHandlers()
        {
            cboFilterLoaiDocGia.SelectedIndexChanged += FilterComboBox_SelectedIndexChanged;
            cboFilterTrangThaiThe.SelectedIndexChanged += FilterComboBox_SelectedIndexChanged;
        }

        // <<< THÊM: Gán sự kiện cho các nút hành động mới >>>
        private void AssignActionEventHandlers()
        {
            // Gắn sự kiện click cho các nút mới (kiểm tra null để an toàn)
            if (btnXemChiTietNo != null)
                btnXemChiTietNo.Click += btnXemChiTietNo_Click;
            if (btnThuTienPhat != null)
                btnThuTienPhat.Click += btnThuTienPhat_Click;
        }

        // Sự kiện chung cho các thay đổi input
        private void InputField_Changed(object? sender, EventArgs e)
        {
            if (_isAdding || IsEditing())
            {
                UpdateSaveButtonState();
            }
        }

        // Sự kiện chung cho các ComboBox lọc thay đổi
        private async void FilterComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (!_isAdding && !IsEditing())
            {
                Debug.WriteLine("FilterComboBox_SelectedIndexChanged triggered.");
                await ApplyFiltersAndLoadDataGrid();
            }
            else
            {
                Debug.WriteLine("FilterComboBox_SelectedIndexChanged skipped (Adding/Editing).");
            }
        }

        // Helper để lấy filter và load lại grid
        private async Task ApplyFiltersAndLoadDataGrid()
        {
            string? keyword = txtTimKiem?.Text;
            var selectedLoai = cboFilterLoaiDocGia?.SelectedItem as LoaiDocGiaDTO;
            int filterLoaiId = selectedLoai?.Id ?? FILTER_ID_TAT_CA;
            string filterTrangThai = cboFilterTrangThaiThe?.SelectedItem?.ToString() ?? FILTER_TRANGTHAI_TAT_CA;
            bool includeDeleted = chkHienThiDaXoa?.Checked ?? false;
            await LoadDataGrid(keyword, includeDeleted, filterLoaiId, filterTrangThai);
        }

        // Cập nhật trạng thái nút Lưu và Bỏ qua
        private void UpdateSaveButtonState()
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            bool editingOrAdding = _isAdding || IsEditing();
            bool canSave = editingOrAdding && HasUnsavedChanges();
            if (btnLuu != null) btnLuu.Enabled = canSave;
            if (btnBoQua != null) btnBoQua.Enabled = editingOrAdding;
            Debug.WriteLine($"UpdateSaveButtonState: CanSave={canSave}, btnLuu.Enabled={btnLuu?.Enabled}, btnBoQua.Enabled={btnBoQua?.Enabled}");
        }

        // Phương thức được gọi bởi Form cha để tải dữ liệu ban đầu
        public async Task InitializeDataAsync()
        {
            Debug.WriteLine("ucQuanLyDocGia.InitializeDataAsync() called.");
            if (this.DesignMode) return;
            _isAdding = false;
            _currentSelectedDTO = null;
            if (txtTimKiem != null) txtTimKiem.Clear();
            if (chkHienThiDaXoa != null) chkHienThiDaXoa.Checked = false;
            _showDeleted = false;
            ClearInputFields();
            await LoadLoaiDocGiaComboBox();
            await LoadFilterComboBoxesAsync();
            await LoadDataGrid(includeDeleted: _showDeleted);
        }

        // Load UserControl
        private void ucQuanLyDocGia_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                Debug.WriteLine("ucQuanLyDocGia_Load fired (DesignMode=false). InitializeDataAsync should be called by parent.");
                dtpNgaySinh.Format = DateTimePickerFormat.Custom;
                dtpNgaySinh.CustomFormat = "dd/MM/yyyy";
                dtpNgayLapThe.Format = DateTimePickerFormat.Custom;
                dtpNgayLapThe.CustomFormat = "dd/MM/yyyy";
                dtpNgayHetHan.Format = DateTimePickerFormat.Custom;
                dtpNgayHetHan.CustomFormat = "dd/MM/yyyy";
            }
        }

        // Tải dữ liệu Loại Độc Giả vào ComboBox chi tiết
        private async Task LoadLoaiDocGiaComboBox()
        {
            Debug.WriteLine("LoadLoaiDocGiaComboBox() called.");
            try
            {
                if (cboLoaiDocGia == null) return;
                _loaiDocGiaList = await _busLoaiDocGia.GetAllLoaiDocGiaAsync();
                cboLoaiDocGia.DataSource = _loaiDocGiaList;
                cboLoaiDocGia.DisplayMember = "TenLoaiDocGia";
                cboLoaiDocGia.ValueMember = "Id";
                cboLoaiDocGia.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                HandleError("tải danh sách loại độc giả (chi tiết)", ex);
                if (cboLoaiDocGia != null) cboLoaiDocGia.DataSource = null;
                _loaiDocGiaList = new List<LoaiDocGiaDTO>();
            }
        }

        // Tải dữ liệu cho các ComboBox Lọc
        private async Task LoadFilterComboBoxesAsync()
        {
            Debug.WriteLine("LoadFilterComboBoxesAsync() called.");
            try
            {
                if (cboFilterLoaiDocGia != null)
                {
                    if (_loaiDocGiaList == null || !_loaiDocGiaList.Any())
                    {
                        _loaiDocGiaList = await _busLoaiDocGia.GetAllLoaiDocGiaAsync() ?? new List<LoaiDocGiaDTO>();
                    }
                    var filterList = new List<LoaiDocGiaDTO>(_loaiDocGiaList);
                    filterList.Insert(0, new LoaiDocGiaDTO { Id = FILTER_ID_TAT_CA, TenLoaiDocGia = "Tất cả loại" });
                    cboFilterLoaiDocGia.DataSource = filterList;
                    cboFilterLoaiDocGia.DisplayMember = "TenLoaiDocGia";
                    cboFilterLoaiDocGia.ValueMember = "Id";
                    cboFilterLoaiDocGia.SelectedValue = FILTER_ID_TAT_CA;
                }
                if (cboFilterTrangThaiThe != null)
                {
                    var trangThaiList = new List<string> { FILTER_TRANGTHAI_TAT_CA, FILTER_TRANGTHAI_CON_HAN, FILTER_TRANGTHAI_HET_HAN };
                    cboFilterTrangThaiThe.DataSource = trangThaiList;
                    cboFilterTrangThaiThe.SelectedItem = FILTER_TRANGTHAI_TAT_CA;
                }
            }
            catch (Exception ex)
            {
                HandleError("tải dữ liệu cho bộ lọc", ex);
                if (cboFilterLoaiDocGia != null) cboFilterLoaiDocGia.DataSource = null;
                if (cboFilterTrangThaiThe != null) cboFilterTrangThaiThe.DataSource = null;
            }
        }

        // Tải dữ liệu Độc Giả vào DataGridView
        private async Task LoadDataGrid(
            string? keyword = null,
            bool includeDeleted = false,
            int filterLoaiDocGiaId = FILTER_ID_TAT_CA,
            string filterTrangThaiThe = FILTER_TRANGTHAI_TAT_CA)
        {
            Debug.WriteLine($"LoadDataGrid(keyword: '{keyword}', includeDeleted: {includeDeleted}, filterLoaiId: {filterLoaiDocGiaId}, filterTrangThai: '{filterTrangThaiThe}') called.");
            int? selectedId = null;
            if (IsEditing() && _currentSelectedDTO != null) { selectedId = _currentSelectedDTO.Id; }
            else if (dgvDocGia?.SelectedRows.Count > 0 && dgvDocGia.SelectedRows[0].DataBoundItem is DocgiaDTO currentDto) { selectedId = currentDto.Id; }

            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (dgvDocGia == null) throw new InvalidOperationException("DataGridView dgvDocGia is null.");
                dgvDocGia.SelectionChanged -= dgvDocGia_SelectionChanged;
                dgvDocGia.DataSource = null;

                List<DocgiaDTO> danhSachFull;
                if (string.IsNullOrWhiteSpace(keyword)) { danhSachFull = await _busDocGia.GetAllDocGiaAsync(includeDeleted); }
                else { danhSachFull = await _busDocGia.SearchDocGiaAsync(keyword.Trim(), includeDeleted); }
                danhSachFull ??= new List<DocgiaDTO>();

                IEnumerable<DocgiaDTO> danhSachFiltered = danhSachFull;
                if (filterLoaiDocGiaId != FILTER_ID_TAT_CA) { danhSachFiltered = danhSachFiltered.Where(dg => dg.IdLoaiDocGia == filterLoaiDocGiaId); }
                DateTime today = DateTime.Today;
                if (filterTrangThaiThe == FILTER_TRANGTHAI_CON_HAN) { danhSachFiltered = danhSachFiltered.Where(dg => dg.NgayHetHan.HasValue && dg.NgayHetHan.Value.Date >= today); }
                else if (filterTrangThaiThe == FILTER_TRANGTHAI_HET_HAN) { danhSachFiltered = danhSachFiltered.Where(dg => !dg.NgayHetHan.HasValue || dg.NgayHetHan.Value.Date < today); }
                List<DocgiaDTO> danhSachFinal = danhSachFiltered.ToList();

                dgvDocGia.DataSource = danhSachFinal;
                SetupDataGridViewColumns();
                if (selectedId.HasValue) { SelectRowById(selectedId.Value); }
                else { dgvDocGia.ClearSelection(); if (!_isAdding && !IsEditing()) { ClearInputFields(); UpdateUIState(); } }
            }
            catch (Exception ex) { HandleError("tải danh sách độc giả", ex); if (dgvDocGia != null) dgvDocGia.DataSource = null; }
            finally
            {
                if (dgvDocGia != null && !dgvDocGia.IsDisposed)
                {
                    try
                    {
                        dgvDocGia.SelectionChanged -= dgvDocGia_SelectionChanged;
                        dgvDocGia.SelectionChanged += dgvDocGia_SelectionChanged;
                        if (dgvDocGia.SelectedRows.Count > 0 && !_isAdding && !IsEditing()) { DisplaySelectedRow(); }
                        else if (dgvDocGia.SelectedRows.Count == 0 && !_isAdding && !IsEditing()) { ClearInputFields(); UpdateUIState(); }
                    }
                    catch (Exception ex) { Debug.WriteLine($"LoadDataGrid Finally Error: {ex.Message}"); }
                }
                this.Cursor = Cursors.Default;
            }
        }

        // Cấu hình các cột DataGridView
        private void SetupDataGridViewColumns()
        {
            Debug.WriteLine("SetupDataGridViewColumns() called.");
            if (dgvDocGia == null || dgvDocGia.Columns.Count == 0) return;
            try
            {
                dgvDocGia.AutoGenerateColumns = false;
                var columns = dgvDocGia.Columns;
                if (columns.Contains("Id")) columns["Id"].Visible = false;
                if (columns.Contains("IdLoaiDocGia")) columns["IdLoaiDocGia"].Visible = false;
                if (columns.Contains("IdNguoiDung")) columns["IdNguoiDung"].Visible = false;
                if (columns.Contains("IdVaiTroNguoiDung")) columns["IdVaiTroNguoiDung"].Visible = false;
                if (columns.Contains("MatKhauNguoiDung")) columns["MatKhauNguoiDung"].Visible = false;
                if (columns.Contains("DaAn")) { columns["DaAn"].HeaderText = "Đã Xóa"; columns["DaAn"].Width = 60; columns["DaAn"].Visible = _showDeleted; columns["DaAn"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; }
                SetColumnProperties("MaDocGia", "Mã ĐG", 100);
                SetColumnProperties("TenDocGia", "Tên Độc Giả", 200, DataGridViewAutoSizeColumnMode.Fill);
                SetColumnProperties("NgaySinh", "Ngày Sinh", 100, format: "dd/MM/yyyy");
                SetColumnProperties("DiaChi", "Địa Chỉ", 250);
                SetColumnProperties("DienThoai", "Điện Thoại", 100);
                SetColumnProperties("Email", "Email", 150);
                SetColumnProperties("TenLoaiDocGia", "Loại Độc Giả", 120);
                SetColumnProperties("NgayLapThe", "Ngày Lập Thẻ", 100, format: "dd/MM/yyyy");
                SetColumnProperties("NgayHetHan", "Ngày Hết Hạn", 100, format: "dd/MM/yyyy");
                SetColumnProperties("TongNoHienTai", "Tiền Nợ", 100, alignment: DataGridViewContentAlignment.MiddleRight, format: "N0");
                SetColumnProperties("TenDangNhap", "Tên Đăng Nhập", 120);
                dgvDocGia.ReadOnly = true; dgvDocGia.AllowUserToAddRows = false; dgvDocGia.AllowUserToDeleteRows = false;
                dgvDocGia.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgvDocGia.MultiSelect = false;
                dgvDocGia.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; dgvDocGia.RowHeadersVisible = false;
                dgvDocGia.CellFormatting -= DgvDocGia_CellFormatting; dgvDocGia.CellFormatting += DgvDocGia_CellFormatting;
            }
            catch (Exception ex) { Debug.WriteLine($"Lỗi cấu hình cột DGV Độc giả: {ex.Message}"); HandleError("cấu hình lưới hiển thị", ex); }
        }

        // Helper cấu hình cột DGV
        private void SetColumnProperties(string columnName, string headerText, int width, DataGridViewAutoSizeColumnMode autoSizeMode = DataGridViewAutoSizeColumnMode.None, DataGridViewContentAlignment alignment = DataGridViewContentAlignment.MiddleLeft, string? format = null, bool visible = true)
        {
            if (dgvDocGia != null && dgvDocGia.Columns.Contains(columnName))
            {
                var column = dgvDocGia.Columns[columnName]; column.HeaderText = headerText; column.Width = width; column.AutoSizeMode = autoSizeMode; column.DefaultCellStyle.Alignment = alignment; if (!string.IsNullOrEmpty(format)) { column.DefaultCellStyle.Format = format; }
                column.Visible = visible;
            }
            else { Debug.WriteLine($"SetColumnProperties: Column '{columnName}' not found or dgvDocGia is null."); }
        }

        // Sự kiện định dạng cell
        private void DgvDocGia_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvDocGia == null || e.RowIndex < 0 || e.RowIndex >= dgvDocGia.Rows.Count) return;
            var row = dgvDocGia.Rows[e.RowIndex];
            if (row.DataBoundItem is DocgiaDTO dto)
            {
                if (dto.DaAn) { row.DefaultCellStyle.BackColor = Color.LightGray; row.DefaultCellStyle.ForeColor = Color.DarkGray; foreach (DataGridViewCell cell in row.Cells) { cell.ToolTipText = "Độc giả này đã bị xóa"; } }
                else { row.DefaultCellStyle.BackColor = SystemColors.Window; row.DefaultCellStyle.ForeColor = SystemColors.ControlText; foreach (DataGridViewCell cell in row.Cells) { cell.ToolTipText = string.Empty; } }
            }
        }

        // Kiểm tra xem có đang ở chế độ sửa không
        private bool IsEditing() => !_isAdding && _currentSelectedDTO != null;

        // Xóa trắng các ô nhập liệu
        private void ClearInputFields()
        {
            Debug.WriteLine("ClearInputFields() called.");
            txtId?.Clear(); txtMaDocGia?.Clear(); txtTenDocGia?.Clear();
            if (dtpNgaySinh != null) dtpNgaySinh.Value = DateTime.Now.AddYears(-18);
            txtDiaChi?.Clear(); txtDienThoai?.Clear(); txtEmail?.Clear();
            if (cboLoaiDocGia != null) cboLoaiDocGia.SelectedIndex = -1;
            if (dtpNgayLapThe != null) dtpNgayLapThe.Value = DateTime.Today;
            if (dtpNgayHetHan != null) dtpNgayHetHan.Value = DateTime.Today.AddYears(1);
            if (txtTienNo != null) txtTienNo.Text = "0";
            txtTenDangNhap?.Clear(); txtMatKhau?.Clear();
            // <<< THÊM: Xóa ô nhập tiền thu >>>
            if (txtSoTienThu != null) txtSoTienThu.Clear();
            _currentSelectedDTO = null;
            if (_isAdding) { txtTenDocGia?.Focus(); }
            UpdateSaveButtonState();
        }

        // Cập nhật trạng thái Enabled/Visible của các controls
        private void UpdateUIState()
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            if (!AreControlsInitialized())
            {
                Debug.WriteLine("UpdateUIState() - UI controls not fully initialized. Skipping.");
                return;
            }

            bool rowSelected = dgvDocGia.SelectedRows.Count > 0;
            bool isAddingOrEditing = _isAdding || IsEditing();
            bool isDeleted = false;
            DocgiaDTO? selectedDto = null;
            bool hasDebt = false;

            if (rowSelected && dgvDocGia.SelectedRows[0].DataBoundItem is DocgiaDTO dto)
            {
                selectedDto = dto;
                isDeleted = dto.DaAn;
                hasDebt = (selectedDto?.TongNoHienTai ?? 0) > 0; // <<< Kiểm tra nợ >>>
            }

            Debug.WriteLine($"UpdateUIState - isAdding: {_isAdding}, isEditing: {IsEditing()}, rowSelected: {rowSelected}, isDeleted: {isDeleted}, hasDebt: {hasDebt}");

            // Panel lưới và nút chính
            panelGrid.Enabled = !isAddingOrEditing;
            btnThem.Enabled = !isAddingOrEditing;
            btnSua.Enabled = !isAddingOrEditing && rowSelected && !isDeleted;
            btnXoa.Enabled = !isAddingOrEditing && rowSelected && !isDeleted;
            btnKhoiPhuc.Enabled = !isAddingOrEditing && rowSelected && isDeleted; // <<< ĐÃ SỬA
            txtTimKiem.Enabled = !isAddingOrEditing;
            btnTimKiem.Enabled = !isAddingOrEditing;
            chkHienThiDaXoa.Enabled = !isAddingOrEditing;
            cboFilterLoaiDocGia.Enabled = !isAddingOrEditing;
            cboFilterTrangThaiThe.Enabled = !isAddingOrEditing;
            if (btnXemLichSu != null) btnXemLichSu.Enabled = !isAddingOrEditing && rowSelected && !isDeleted;

            // <<< THÊM: Bật/tắt nút Xem Chi Tiết Nợ và các control Thu Phạt >>>
            if (btnXemChiTietNo != null) btnXemChiTietNo.Enabled = !isAddingOrEditing && rowSelected && !isDeleted && hasDebt;
            if (btnThuTienPhat != null) btnThuTienPhat.Enabled = !isAddingOrEditing && rowSelected && !isDeleted && hasDebt;
            if (txtSoTienThu != null)
            {
                txtSoTienThu.Enabled = !isAddingOrEditing && rowSelected && !isDeleted && hasDebt;
                // Xóa nội dung khi tắt
                if (!txtSoTienThu.Enabled) txtSoTienThu.Clear();
            }
            // <<< KẾT THÚC THÊM >>>


            // Panel chi tiết và nút Lưu/Bỏ qua
            panelDetails.Enabled = true;
            UpdateSaveButtonState();

            // Trạng thái ô nhập liệu
            txtId.Enabled = false; txtMaDocGia.Enabled = false;
            txtTenDocGia.Enabled = isAddingOrEditing; dtpNgaySinh.Enabled = isAddingOrEditing;
            txtDiaChi.Enabled = isAddingOrEditing; txtDienThoai.Enabled = isAddingOrEditing;
            txtEmail.Enabled = isAddingOrEditing; cboLoaiDocGia.Enabled = isAddingOrEditing;
            dtpNgayLapThe.Enabled = isAddingOrEditing; dtpNgayHetHan.Enabled = isAddingOrEditing;
            txtTienNo.Enabled = false;
            txtTenDangNhap.Enabled = _isAdding;
            txtMatKhau.Enabled = isAddingOrEditing;
        }

        // Helper kiểm tra controls đã khởi tạo chưa
        private bool AreControlsInitialized()
        {
            // <<< THÊM kiểm tra các control mới >>>
            return btnThem != null && btnSua != null && btnXoa != null && btnKhoiPhuc != null && btnXemLichSu != null && // <<< ĐÃ SỬA
                   btnXemChiTietNo != null && btnThuTienPhat != null && txtSoTienThu != null && // <<< Các control mới
                   chkHienThiDaXoa != null && txtTimKiem != null && btnTimKiem != null &&
                   cboFilterLoaiDocGia != null && cboFilterTrangThaiThe != null &&
                   txtId != null && txtMaDocGia != null && txtTenDocGia != null && dtpNgaySinh != null &&
                   txtDiaChi != null && txtDienThoai != null && txtEmail != null && cboLoaiDocGia != null &&
                   dtpNgayLapThe != null && dtpNgayHetHan != null && txtTienNo != null &&
                   txtTenDangNhap != null && txtMatKhau != null && panelGrid != null && panelDetails != null;
        }


        // Hiển thị dữ liệu dòng đang chọn lên các ô input
        private void DisplaySelectedRow()
        {
            Debug.WriteLine("DisplaySelectedRow() called.");
            if (_isAdding || IsEditing())
            { Debug.WriteLine("DisplaySelectedRow() - Skipping update while adding or editing."); return; }
            _currentSelectedDTO = null;
            if (dgvDocGia != null && dgvDocGia.SelectedRows.Count == 1 && dgvDocGia.SelectedRows[0].DataBoundItem is DocgiaDTO dto)
            { DisplaySelectedRowData(dto); }
            else { Debug.WriteLine("DisplaySelectedRow() - No single row selected. Clearing input fields."); ClearInputFields(); }
            UpdateUIState();
        }

        // Helper gán DateTime an toàn
        private DateTime SafeGetDateTime(DateTime? value, DateTime min, DateTime max)
        {
            if (!value.HasValue) return min; if (value.Value < min) return min; if (value.Value > max) return max; return value.Value;
        }

        // Chọn dòng trên lưới dựa theo ID
        private void SelectRowById(int id)
        {
            Debug.WriteLine($"SelectRowById({id}) called.");
            if (dgvDocGia == null || dgvDocGia.Rows.Count == 0) return;
            dgvDocGia.ClearSelection(); DataGridViewRow? rowToSelect = null;
            foreach (DataGridViewRow row in dgvDocGia.Rows) { if (row.DataBoundItem is DocgiaDTO dto && dto.Id == id) { rowToSelect = row; break; } }
            if (rowToSelect != null)
            {
                rowToSelect.Selected = true; Debug.WriteLine($"SelectRowById({id}) - Row found and selected at index {rowToSelect.Index}.");
                if (!IsRowFullyVisible(rowToSelect.Index)) { try { dgvDocGia.FirstDisplayedScrollingRowIndex = rowToSelect.Index; } catch (Exception ex) { Debug.WriteLine($"Error scrolling to row {rowToSelect.Index}: {ex.Message}"); } }
                if (!_isAdding && !IsEditing()) { DisplaySelectedRow(); }
            }
            else { Debug.WriteLine($"SelectRowById({id}) - Row not found."); if (!_isAdding && !IsEditing()) { ClearInputFields(); UpdateUIState(); } }
        }

        // Hàm helper kiểm tra dòng có hiển thị đầy đủ không
        private bool IsRowFullyVisible(int rowIndex)
        {
            if (dgvDocGia == null || rowIndex < 0 || rowIndex >= dgvDocGia.RowCount) return false; int firstVisibleRowIndex = dgvDocGia.FirstDisplayedScrollingRowIndex; if (firstVisibleRowIndex < 0) return false; int displayedRowCount = dgvDocGia.DisplayedRowCount(false); if (displayedRowCount <= 0) return false; int lastVisibleRowIndex = firstVisibleRowIndex + displayedRowCount - 1; if (lastVisibleRowIndex >= dgvDocGia.RowCount) lastVisibleRowIndex = dgvDocGia.RowCount - 1; return rowIndex >= firstVisibleRowIndex && rowIndex <= lastVisibleRowIndex;
        }

        // Kiểm tra xem có thay đổi chưa lưu hay không
        private bool HasUnsavedChanges()
        {
            if (_isAdding) { bool changed = !string.IsNullOrWhiteSpace(txtTenDocGia?.Text) || !string.IsNullOrWhiteSpace(txtTenDangNhap?.Text) || !string.IsNullOrWhiteSpace(txtMatKhau?.Text) || (cboLoaiDocGia?.SelectedIndex ?? -1) != -1 || !string.IsNullOrWhiteSpace(txtDiaChi?.Text) || !string.IsNullOrWhiteSpace(txtDienThoai?.Text) || !string.IsNullOrWhiteSpace(txtEmail?.Text) || dtpNgaySinh?.Value.Date != DateTime.Now.AddYears(-18).Date || dtpNgayLapThe?.Value.Date != DateTime.Today || dtpNgayHetHan?.Value.Date != DateTime.Today.AddYears(1); Debug.WriteLine($"HasUnsavedChanges (Adding): {changed}"); return changed; }
            if (IsEditing() && _currentSelectedDTO != null) { bool changed = false; if ((txtTenDocGia?.Text.Trim() ?? "") != (_currentSelectedDTO.TenDocGia ?? "")) changed = true; else if (dtpNgaySinh?.Value.Date != (_currentSelectedDTO.NgaySinh?.Date ?? DateTimePicker.MinimumDateTime.Date)) changed = true; else if ((txtDiaChi?.Text.Trim() ?? "") != (_currentSelectedDTO.DiaChi ?? "")) changed = true; else if ((txtDienThoai?.Text.Trim() ?? "") != (_currentSelectedDTO.DienThoai ?? "")) changed = true; else if ((txtEmail?.Text.Trim() ?? "") != (_currentSelectedDTO.Email ?? "")) changed = true; else if ((cboLoaiDocGia?.SelectedValue as int? ?? 0) != _currentSelectedDTO.IdLoaiDocGia) changed = true; else if (dtpNgayLapThe?.Value.Date != (_currentSelectedDTO.NgayLapThe?.Date ?? DateTimePicker.MinimumDateTime.Date)) changed = true; else if (dtpNgayHetHan?.Value.Date != (_currentSelectedDTO.NgayHetHan?.Date ?? DateTimePicker.MinimumDateTime.Date)) changed = true; else if (!string.IsNullOrWhiteSpace(txtMatKhau?.Text)) changed = true; Debug.WriteLine($"HasUnsavedChanges (Editing): {changed}"); return changed; }
            Debug.WriteLine("HasUnsavedChanges (Viewing): false"); return false;
        }

        // Lấy dữ liệu từ các ô nhập liệu thành DTO
        private DocgiaDTO GetDTOFromControls()
        {
            Debug.WriteLine("GetDTOFromControls() called.");
            if (!AreControlsInitialized()) throw new InvalidOperationException("Controls chưa được khởi tạo.");
            if (string.IsNullOrWhiteSpace(txtTenDocGia.Text)) throw new ArgumentException("Tên độc giả là bắt buộc.", nameof(txtTenDocGia));
            if (cboLoaiDocGia.SelectedValue == null || (cboLoaiDocGia.SelectedValue as int? ?? 0) <= 0) throw new ArgumentException("Vui lòng chọn loại độc giả.", nameof(cboLoaiDocGia));
            if (dtpNgayLapThe.Value.Date > dtpNgayHetHan.Value.Date) throw new ArgumentException("Ngày hết hạn không được trước ngày lập thẻ.", nameof(dtpNgayHetHan));
            if (_isAdding) { if (string.IsNullOrWhiteSpace(txtTenDangNhap.Text)) throw new ArgumentException("Tên đăng nhập là bắt buộc khi thêm mới.", nameof(txtTenDangNhap)); if (string.IsNullOrWhiteSpace(txtMatKhau.Text)) throw new ArgumentException("Mật khẩu là bắt buộc khi thêm mới.", nameof(txtMatKhau)); }
            int id = 0; int idNguoiDung = 0;
            if (!_isAdding && _currentSelectedDTO != null) { id = _currentSelectedDTO.Id; idNguoiDung = _currentSelectedDTO.IdNguoiDung; if (id <= 0) throw new InvalidOperationException("ID độc giả gốc không hợp lệ để cập nhật."); }
            string? maDocGia = _isAdding ? null : _currentSelectedDTO?.MaDocGia;
            string tenDocGia = txtTenDocGia.Text.Trim(); DateTime? ngaySinh = dtpNgaySinh.Value; string? diaChi = txtDiaChi.Text.Trim(); string? dienThoai = txtDienThoai.Text.Trim(); string? email = txtEmail.Text.Trim(); int idLoaiDocGia = (int)cboLoaiDocGia.SelectedValue; DateTime? ngayLapThe = dtpNgayLapThe.Value; DateTime? ngayHetHan = dtpNgayHetHan.Value; decimal? tienNo = _currentSelectedDTO?.TongNoHienTai ?? 0m;
            string? tenDangNhap = null; string? matKhau = null; int? idVaiTro = null;
            if (_isAdding) { tenDangNhap = txtTenDangNhap.Text.Trim(); matKhau = txtMatKhau.Text; idVaiTro = 15; }
            else { tenDangNhap = _currentSelectedDTO?.TenDangNhap; matKhau = string.IsNullOrWhiteSpace(txtMatKhau.Text) ? null : txtMatKhau.Text; idVaiTro = _currentSelectedDTO?.IdVaiTroNguoiDung; }
            DocgiaDTO dto = new DocgiaDTO { Id = id, MaDocGia = maDocGia, TenDocGia = tenDocGia, NgaySinh = ngaySinh, DiaChi = string.IsNullOrWhiteSpace(diaChi) ? null : diaChi, DienThoai = string.IsNullOrWhiteSpace(dienThoai) ? null : dienThoai, Email = string.IsNullOrWhiteSpace(email) ? null : email, IdLoaiDocGia = idLoaiDocGia, NgayLapThe = ngayLapThe, NgayHetHan = ngayHetHan, TongNoHienTai = tienNo, IdNguoiDung = idNguoiDung, TenDangNhap = tenDangNhap, IdVaiTroNguoiDung = idVaiTro, MatKhauNguoiDung = matKhau, DaAn = _currentSelectedDTO?.DaAn ?? false };
            return dto;
        }

        // --- BUTTON EVENT HANDLERS ---

        // Nút Thêm
        private void btnThem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnThem_Click");
            if (HasUnsavedChanges() && IsEditing()) { DialogResult confirm = ShowConfirm("Bạn có thay đổi chưa lưu khi đang sửa. Bạn có muốn hủy bỏ và thêm mới?"); if (confirm == DialogResult.No) return; }
            _isAdding = true; _currentSelectedDTO = null; dgvDocGia?.ClearSelection(); ClearInputFields(); UpdateUIState();
        }

        // Nút Sửa
        private void btnSua_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnSua_Click");
            if (dgvDocGia?.SelectedRows.Count == 1 && btnSua.Enabled) { if (dgvDocGia.SelectedRows[0].DataBoundItem is DocgiaDTO dtoToEdit) { _isAdding = false; _currentSelectedDTO = CreateDeepCopy(dtoToEdit); Debug.WriteLine($"btnSua_Click: Stored original DTO (ID: {_currentSelectedDTO.Id})."); DisplaySelectedRowData(dtoToEdit); UpdateUIState(); txtTenDocGia?.Focus(); } else { Debug.WriteLine("btnSua_Click: Could not cast DataBoundItem to DocgiaDTO."); ShowWarning("Không thể lấy thông tin độc giả để sửa."); } }
            else if (dgvDocGia?.SelectedRows.Count == 0) { ShowWarning("Vui lòng chọn độc giả cần sửa."); }
            else { Debug.WriteLine("btnSua_Click: Condition not met (Not 1 row selected or button disabled)."); }
        }

        // Helper tạo bản sao sâu
        private DocgiaDTO CreateDeepCopy(DocgiaDTO original) { return new DocgiaDTO { Id = original.Id, MaDocGia = original.MaDocGia, TenDocGia = original.TenDocGia, NgaySinh = original.NgaySinh, DiaChi = original.DiaChi, DienThoai = original.DienThoai, Email = original.Email, IdLoaiDocGia = original.IdLoaiDocGia, TenLoaiDocGia = original.TenLoaiDocGia, NgayLapThe = original.NgayLapThe, NgayHetHan = original.NgayHetHan, TongNoHienTai = original.TongNoHienTai, IdNguoiDung = original.IdNguoiDung, TenDangNhap = original.TenDangNhap, IdVaiTroNguoiDung = original.IdVaiTroNguoiDung, MatKhauNguoiDung = null, DaAn = original.DaAn }; }

        // Helper để hiển thị dữ liệu DTO lên form
        private void DisplaySelectedRowData(DocgiaDTO dto)
        {
            txtId.Text = dto.Id.ToString(); txtMaDocGia.Text = dto.MaDocGia ?? string.Empty; txtTenDocGia.Text = dto.TenDocGia; dtpNgaySinh.Value = SafeGetDateTime(dto.NgaySinh, dtpNgaySinh.MinDate, dtpNgaySinh.MaxDate); txtDiaChi.Text = dto.DiaChi ?? string.Empty; txtDienThoai.Text = dto.DienThoai ?? string.Empty; txtEmail.Text = dto.Email ?? string.Empty; if (dto.IdLoaiDocGia > 0 && _loaiDocGiaList.Any(l => l.Id == dto.IdLoaiDocGia)) cboLoaiDocGia.SelectedValue = dto.IdLoaiDocGia; else cboLoaiDocGia.SelectedIndex = -1; dtpNgayLapThe.Value = SafeGetDateTime(dto.NgayLapThe, dtpNgayLapThe.MinDate, dtpNgayLapThe.MaxDate); dtpNgayHetHan.Value = SafeGetDateTime(dto.NgayHetHan, dtpNgayHetHan.MinDate, dtpNgayHetHan.MaxDate); txtTienNo.Text = dto.TongNoHienTai?.ToString("N0") ?? "0"; txtTenDangNhap.Text = dto.TenDangNhap ?? string.Empty; txtMatKhau.Clear();
        }

        // Nút Lưu
        private async void btnLuu_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnLuu_Click started."); if (!btnLuu.Enabled) return;
            if (btnLuu != null) btnLuu.Enabled = false; this.Cursor = Cursors.WaitCursor;
            DocgiaDTO? docGiaDtoToSave = null; bool success = false; string? errorMsg = null; int? savedDocGiaId = null;
            try
            {
                docGiaDtoToSave = GetDTOFromControls();
                if (_isAdding) { DocgiaDTO? addedDocGiaResult = await _busDocGia.AddDocGiaAsync(docGiaDtoToSave); success = addedDocGiaResult != null; if (success) savedDocGiaId = addedDocGiaResult!.Id; else errorMsg ??= "Thêm độc giả thất bại (BUS trả về null)."; }
                else { if (docGiaDtoToSave.Id <= 0) throw new InvalidOperationException("Không thể cập nhật độc giả với ID không hợp lệ."); success = await _busDocGia.UpdateDocGiaAsync(docGiaDtoToSave); if (success) savedDocGiaId = docGiaDtoToSave.Id; else errorMsg ??= "Cập nhật thông tin độc giả thất bại."; }
            }
            catch (ArgumentException argEx) { errorMsg = argEx.Message; Debug.WriteLine($"Validation Error: {argEx.Message}"); }
            catch (InvalidOperationException invOpEx) { errorMsg = $"Lỗi logic: {invOpEx.Message}"; Debug.WriteLine($"Invalid Operation: {invOpEx}"); }
            catch (DbUpdateException dbEx) { errorMsg = $"Lỗi CSDL khi lưu: {dbEx.InnerException?.Message ?? dbEx.Message}"; Debug.WriteLine($"DB Update Error: {dbEx}"); }
            catch (Exception ex) { errorMsg = $"Lỗi hệ thống: {ex.Message}"; Debug.WriteLine($"Generic Error: {ex}"); }
            finally { this.Cursor = Cursors.Default; if (!success && (_isAdding || IsEditing())) { UpdateSaveButtonState(); } else if (success) { _isAdding = false; _currentSelectedDTO = null; } Debug.WriteLine("btnLuu_Click finally block finished."); }
            if (success) { ShowInfo(_isAdding ? "Thêm độc giả thành công!" : "Cập nhật thông tin độc giả thành công!"); await ApplyFiltersAndLoadDataGrid(); if (savedDocGiaId.HasValue) { SelectRowById(savedDocGiaId.Value); } }
            else { ShowError(errorMsg ?? (_isAdding ? "Thêm độc giả thất bại." : "Cập nhật thông tin độc giả thất bại.")); }
        }

        // Nút Bỏ qua
        private void btnBoQua_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnBoQua_Click fired."); bool wasAdding = _isAdding; bool wasEditing = IsEditing(); bool needsConfirm = HasUnsavedChanges();
            if ((wasAdding || wasEditing) && needsConfirm) { DialogResult confirm = ShowConfirm("Bạn có thay đổi chưa lưu. Bạn có chắc chắn muốn hủy bỏ?"); if (confirm == DialogResult.No) { Debug.WriteLine("btnBoQua_Click cancelled by user."); return; } Debug.WriteLine("btnBoQua_Click confirmed cancel unsaved changes."); }
            _isAdding = false;
            if (wasEditing && _currentSelectedDTO != null) { int originalId = _currentSelectedDTO.Id; SelectRowById(originalId); DisplaySelectedRowData(_currentSelectedDTO); _currentSelectedDTO = null; UpdateUIState(); }
            else { _currentSelectedDTO = null; DisplaySelectedRow(); UpdateUIState(); }
            Debug.WriteLine("btnBoQua_Click finished.");
        }

        // Nút Xóa
        private async void btnXoa_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnXoa_Click (Soft Delete)"); DocgiaDTO? dtoToDelete = null;
            if (dgvDocGia?.SelectedRows.Count == 1 && btnXoa.Enabled) { if (dgvDocGia.SelectedRows[0].DataBoundItem is DocgiaDTO dto) dtoToDelete = dto; }
            if (dtoToDelete == null) { if (dgvDocGia?.SelectedRows.Count == 0) ShowWarning("Vui lòng chọn độc giả cần xóa."); else Debug.WriteLine("Delete aborted: Cannot get DTO or button disabled."); return; }
            int idToDelete = dtoToDelete.Id; string tenDocGia = dtoToDelete.TenDocGia ?? "?"; if (idToDelete <= 0) { ShowWarning("ID độc giả không hợp lệ để xóa."); return; }
            DialogResult confirm = ShowConfirm($"Bạn chắc chắn muốn XÓA (ẩn) độc giả '{tenDocGia}' (ID: {idToDelete})?");
            if (confirm == DialogResult.Yes)
            {
                bool success = false; string? errorMsg = null; this.Cursor = Cursors.WaitCursor; ToggleButtonStateDuringOperation(false);
                try { success = await _busDocGia.SoftDeleteDocGiaAsync(idToDelete); } catch (InvalidOperationException invOpEx) { errorMsg = invOpEx.Message; Debug.WriteLine($"Business Rule Violation: {invOpEx}"); } catch (Exception ex) { errorMsg = $"Lỗi hệ thống khi xóa: {ex.Message}"; Debug.WriteLine($"System Error: {ex}"); } finally { this.Cursor = Cursors.Default; }
                if (success) { ShowInfo("Xóa (ẩn) độc giả thành công!"); await ApplyFiltersAndLoadDataGrid(); } else { ShowError(errorMsg ?? "Xóa (ẩn) độc giả thất bại."); UpdateUIState(); }
            }
        }

        // Nút Khôi phục
        private async void btnKhoiPhuc_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnKhoiPhuc_Click"); DocgiaDTO? dtoToRestore = null;
            if (dgvDocGia?.SelectedRows.Count == 1 && btnKhoiPhuc.Enabled) { if (dgvDocGia.SelectedRows[0].DataBoundItem is DocgiaDTO dto) dtoToRestore = dto; }
            if (dtoToRestore == null) { ShowWarning("Vui lòng chọn độc giả cần khôi phục."); return; }
            int idToRestore = dtoToRestore.Id; string tenDocGia = dtoToRestore.TenDocGia ?? "?"; if (idToRestore <= 0) { ShowWarning("ID độc giả không hợp lệ để khôi phục."); return; }
            DialogResult confirm = ShowConfirm($"Bạn chắc chắn muốn KHÔI PHỤC độc giả '{tenDocGia}' (ID: {idToRestore})?");
            if (confirm == DialogResult.Yes)
            {
                bool success = false; string? errorMsg = null; this.Cursor = Cursors.WaitCursor; ToggleButtonStateDuringOperation(false);
                try { success = await _busDocGia.RestoreDocGiaAsync(idToRestore); } catch (Exception ex) { errorMsg = $"Lỗi hệ thống khi khôi phục: {ex.Message}"; Debug.WriteLine($"System Error: {ex}"); } finally { this.Cursor = Cursors.Default; }
                if (success) { ShowInfo("Khôi phục độc giả thành công!"); await ApplyFiltersAndLoadDataGrid(); SelectRowById(idToRestore); } else { ShowError(errorMsg ?? "Khôi phục độc giả thất bại."); UpdateUIState(); }
            }
        }

        // Nút Xem Lịch Sử
        private async void btnXemLichSu_Click(object sender, EventArgs e)
        {
            if (dgvDocGia.SelectedRows.Count == 1 && dgvDocGia.SelectedRows[0].DataBoundItem is DocgiaDTO selectedDocGia)
            {
                int idDocGia = selectedDocGia.Id; string tenDocGia = selectedDocGia.TenDocGia ?? "Không rõ"; string maDocGia = selectedDocGia.MaDocGia ?? "N/A";
                Debug.WriteLine($"Xem lịch sử cho Độc giả ID: {idDocGia}, Tên: {tenDocGia}");
                this.Cursor = Cursors.WaitCursor; if (btnXemLichSu != null) btnXemLichSu.Enabled = false;
                try
                {
                    List<PhieuMuonTraDTO> lichSu = await _busPhieuMuonTra.GetHistoryByDocGiaIdAsync(idDocGia); Debug.WriteLine($"Tìm thấy {lichSu.Count} bản ghi lịch sử.");
                    using (var formLichSu = new frmLichSuMuonTra(lichSu, maDocGia, tenDocGia)) { formLichSu.ShowDialog(this.FindForm()); }
                }
                catch (ArgumentException argEx) { ShowError($"Lỗi dữ liệu khi xem lịch sử: {argEx.Message}"); }
                catch (Exception ex) { HandleError($"xem lịch sử mượn trả của độc giả '{tenDocGia}'", ex); }
                finally { this.Cursor = Cursors.Default; UpdateUIState(); } // <<< Gọi UpdateUIState để cập nhật lại nút
            }
            else if (dgvDocGia.SelectedRows.Count == 0) { ShowWarning("Vui lòng chọn một độc giả để xem lịch sử."); }
        }

        // <<< THÊM: Sự kiện Click cho nút Xem Chi Tiết Nợ >>>
        private async void btnXemChiTietNo_Click(object sender, EventArgs e)
        {
            if (dgvDocGia.SelectedRows.Count == 1 && dgvDocGia.SelectedRows[0].DataBoundItem is DocgiaDTO selectedDocGia)
            {
                int idDocGia = selectedDocGia.Id;
                string tenDocGia = selectedDocGia.TenDocGia ?? "Không rõ";
                decimal? tongNo = selectedDocGia.TongNoHienTai;

                Debug.WriteLine($"Xem chi tiết nợ cho Độc giả ID: {idDocGia}, Tên: {tenDocGia}");

                if (tongNo == null || tongNo <= 0)
                {
                    ShowInfo("Độc giả này không có nợ.");
                    return;
                }

                this.Cursor = Cursors.WaitCursor;
                if (btnXemChiTietNo != null) btnXemChiTietNo.Enabled = false;

                try
                {
                    List<PhieuMuonTraDTO> fineHistory = await _busPhieuMuonTra.GetFineHistoryByDocGiaIdAsync(idDocGia);

                    if (fineHistory == null || !fineHistory.Any())
                    {
                        ShowInfo("Không tìm thấy chi tiết các khoản phạt cho độc giả này.");
                    }
                    else
                    {
                        var detailsBuilder = new StringBuilder();
                        detailsBuilder.AppendLine($"Chi tiết nợ của độc giả: {tenDocGia} (Tổng nợ: {tongNo:N0} VNĐ)");
                        detailsBuilder.AppendLine("--------------------------------------------------");
                        foreach (var fine in fineHistory)
                        {
                            detailsBuilder.AppendLine($"* Sách: {fine.TenTuaSach ?? "N/A"} (Mã: {fine.MaCuonSach ?? "N/A"})");
                            detailsBuilder.AppendLine($"  Trả ngày: {fine.NgayTra:dd/MM/yyyy} (Hạn: {fine.HanTra:dd/MM/yyyy})");
                            detailsBuilder.AppendLine($"  Tiền phạt: {fine.SoTienPhat:N0} VNĐ");
                            detailsBuilder.AppendLine("---");
                        }
                        MaterialMessageBox.Show(detailsBuilder.ToString(), "Chi Tiết Nợ", MessageBoxButtons.OK, MessageBoxIcon.Information, false, FlexibleMaterialForm.ButtonsPosition.Center);
                    }
                }
                catch (ArgumentException argEx) { ShowError($"Lỗi dữ liệu khi xem chi tiết nợ: {argEx.Message}"); }
                catch (Exception ex) { HandleError($"xem chi tiết nợ của độc giả '{tenDocGia}'", ex); }
                finally
                {
                    this.Cursor = Cursors.Default;
                    UpdateUIState(); // Gọi lại để cập nhật trạng thái các nút
                }
            }
            else if (dgvDocGia.SelectedRows.Count == 0)
            {
                ShowWarning("Vui lòng chọn một độc giả để xem chi tiết nợ.");
            }
        }

        // <<< THÊM: Sự kiện Click cho nút Thu Tiền Phạt >>>
        private async void btnThuTienPhat_Click(object sender, EventArgs e)
        {
            if (dgvDocGia.SelectedRows.Count != 1 || !(dgvDocGia.SelectedRows[0].DataBoundItem is DocgiaDTO selectedDocGia))
            {
                ShowWarning("Vui lòng chọn độc giả cần thu tiền phạt.");
                return;
            }

            int idDocGia = selectedDocGia.Id;
            decimal currentDebtDecimal = selectedDocGia.TongNoHienTai ?? 0m;
            int currentDebt = Convert.ToInt32(currentDebtDecimal);

            if (currentDebt <= 0)
            {
                ShowInfo("Độc giả này không có nợ.");
                return;
            }

            if (!int.TryParse(txtSoTienThu?.Text, out int soTienThu) || soTienThu <= 0)
            {
                ShowWarning("Vui lòng nhập số tiền thu hợp lệ (số dương).");
                txtSoTienThu?.Focus();
                return;
            }

            if (soTienThu > currentDebt)
            {
                ShowWarning($"Số tiền thu không được vượt quá tổng nợ hiện tại ({currentDebt:N0} VNĐ).");
                txtSoTienThu?.Focus();
                return;
            }

            DialogResult confirm = ShowConfirm($"Bạn chắc chắn muốn thu {soTienThu:N0} VNĐ tiền phạt từ độc giả '{selectedDocGia.TenDocGia}'?");
            if (confirm == DialogResult.No) return;

            var phieuThuPhatDTO = new PhieuThuPhatDTO { IdDocGia = idDocGia, SoTienThu = soTienThu };
            bool success = false; string? errorMsg = null;
            if (btnThuTienPhat != null) btnThuTienPhat.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                success = await _busDocGia.LapPhieuThuPhatAsync(phieuThuPhatDTO);
            }
            catch (ArgumentException argEx) { errorMsg = $"Lỗi dữ liệu: {argEx.Message}"; }
            catch (InvalidOperationException opEx) { errorMsg = $"Lỗi nghiệp vụ: {opEx.Message}"; }
            catch (KeyNotFoundException keyEx) { errorMsg = $"Lỗi không tìm thấy: {keyEx.Message}"; }
            catch (Exception ex) { errorMsg = $"Lỗi hệ thống khi thu phạt: {ex.Message}"; Debug.WriteLine($"ERROR (LapPhieuThuPhatAsync): {ex}"); }
            finally
            {
                this.Cursor = Cursors.Default;
            }

            if (success)
            {
                ShowInfo("Thu tiền phạt thành công!");
                txtSoTienThu?.Clear();
                await ApplyFiltersAndLoadDataGrid();
                SelectRowById(idDocGia);
            }
            else
            {
                ShowError(errorMsg ?? "Thu tiền phạt thất bại do lỗi không xác định.");
                // UpdateUIState sẽ bật lại nút nếu lỗi và vẫn đủ điều kiện
                UpdateUIState();
            }
        }

        // Nút Thoát
        private void btnThoat_Click(object sender, EventArgs g)
        {
            Debug.WriteLine("btnThoat_Click");
            if (HasUnsavedChanges() && (_isAdding || IsEditing())) { DialogResult confirm = ShowConfirm("Bạn có thay đổi chưa lưu. Bạn có chắc chắn muốn thoát và hủy bỏ?"); if (confirm != DialogResult.Yes) return; }
            _isAdding = false; _currentSelectedDTO = null; RequestClose?.Invoke(this, EventArgs.Empty);
        }

        // --- DATAGRIDVIEW EVENT HANDLERS ---

        // Khi lựa chọn dòng trên lưới thay đổi
        private void dgvDocGia_SelectionChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("dgvDocGia_SelectionChanged triggered.");
            DisplaySelectedRow();
        }

        // Double click trên lưới = hành động Sửa
        private void dgvDocGia_DoubleClick(object sender, EventArgs e)
        {
            Debug.WriteLine("dgvDocGia_DoubleClick triggered."); if (btnSua.Enabled) { btnSua_Click(sender, e); }
        }

        // --- TÌM KIẾM VÀ HIỂN THỊ ĐÃ XÓA ---

        // Nút Tìm kiếm
        private async void btnTimKiem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnTimKiem_Click"); await ApplyFiltersAndLoadDataGrid();
        }

        // Nhấn Enter trong ô tìm kiếm
        private async void txtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { Debug.WriteLine("txtTimKiem_KeyDown Enter"); await ApplyFiltersAndLoadDataGrid(); e.SuppressKeyPress = true; e.Handled = true; }
        }

        // Checkbox Hiển thị đã xóa thay đổi
        private async void chkHienThiDaXoa_CheckedChanged(object sender, EventArgs e)
        {
            _showDeleted = chkHienThiDaXoa?.Checked ?? false; Debug.WriteLine($"chkHienThiDaXoa_CheckedChanged: {_showDeleted}"); await ApplyFiltersAndLoadDataGrid();
        }

        // --- HELPER METHODS (Thông báo lỗi,...) ---
        private void HandleError(string action, Exception ex) { var mainForm = this.FindForm() as frmMain; string message = $"Lỗi khi {action}: {ex.Message}"; Debug.WriteLine($"ERROR ({action}): {ex}"); var msgBox = mainForm ?? (Control)this; MaterialMessageBox.Show(msgBox, message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        private void ShowError(string message) { var mainForm = this.FindForm() as frmMain; Debug.WriteLine($"UI Error: {message}"); var msgBox = mainForm ?? (Control)this; MaterialMessageBox.Show(msgBox, message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        private void ShowWarning(string message) { var mainForm = this.FindForm() as frmMain; Debug.WriteLine($"UI Warning: {message}"); var msgBox = mainForm ?? (Control)this; MaterialMessageBox.Show(msgBox, message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        private void ShowInfo(string message) { var mainForm = this.FindForm() as frmMain; Debug.WriteLine($"UI Info: {message}"); var msgBox = mainForm ?? (Control)this; MaterialMessageBox.Show(msgBox, message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private DialogResult ShowConfirm(string message) { var mainForm = this.FindForm() as frmMain; Debug.WriteLine($"UI Confirm: {message}"); var msgBox = mainForm ?? (Control)this; return MaterialMessageBox.Show(msgBox, message, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question); }
        private void ToggleButtonStateDuringOperation(bool enabled)
        {
            if (btnThem != null) btnThem.Enabled = enabled; if (btnSua != null) btnSua.Enabled = enabled; if (btnXoa != null) btnXoa.Enabled = enabled; if (btnKhoiPhuc != null) btnKhoiPhuc.Enabled = enabled; if (btnXemLichSu != null) btnXemLichSu.Enabled = enabled; // <<< ĐÃ SỬA
            // <<< THÊM: Tắt/Bật các nút mới khi đang thực hiện thao tác dài >>>
            if (btnXemChiTietNo != null) btnXemChiTietNo.Enabled = enabled;
            if (btnThuTienPhat != null) btnThuTienPhat.Enabled = enabled;
            if (txtSoTienThu != null) txtSoTienThu.Enabled = enabled;
            // <<< KẾT THÚC THÊM >>>
            if (txtTimKiem != null) txtTimKiem.Enabled = enabled; if (btnTimKiem != null) btnTimKiem.Enabled = enabled; if (chkHienThiDaXoa != null) chkHienThiDaXoa.Enabled = enabled; if (cboFilterLoaiDocGia != null) cboFilterLoaiDocGia.Enabled = enabled; if (cboFilterTrangThaiThe != null) cboFilterTrangThaiThe.Enabled = enabled;
        }

    } // End class ucQuanLyDocGia
} // End namespace GUI