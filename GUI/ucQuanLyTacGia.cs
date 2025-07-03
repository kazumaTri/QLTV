// Project/Namespace: GUI
using BUS; // Cần cho IBUSTacGia
using DTO; // Cần cho TacGiaDTO
using MaterialSkin.Controls; // Cần cho các MaterialSkin controls
using System;
using System.Collections.Generic;
using System.Drawing; // <<< THÊM USING NÀY để dùng Color, Font
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

// *** THÊM DÒNG NÀY ***
using static GUI.frmMain;

namespace GUI
{
    /// <summary>
    /// UserControl quản lý thông tin Tác giả.
    /// Hỗ trợ Soft Delete: Ẩn/Hiện, Khôi phục, Tìm kiếm bao gồm cả đã ẩn.
    /// </summary>
    public partial class ucQuanLyTacGia : UserControl, IRequiresDataLoading, IRequestClose
    {
        // --- DEPENDENCIES ---
        private readonly IBUSTacGia _busTacGia;
        private readonly IServiceProvider _serviceProvider;

        // --- STATE ---
        private bool _isAdding = false;

        // --- EVENTS ---
        public event EventHandler? RequestClose;

        // --- CONSTRUCTOR ---
        public ucQuanLyTacGia(IBUSTacGia busTacGia, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _busTacGia = busTacGia ?? throw new ArgumentNullException(nameof(busTacGia));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            // Gắn sự kiện cho các controls (Designer cũng đã làm, nhưng thêm ở đây để chắc chắn)
            if (this.txtTimKiem != null)
            {
                this.txtTimKiem.TextChanged -= new System.EventHandler(this.txtTimKiem_TextChanged);
                this.txtTimKiem.TextChanged += new System.EventHandler(this.txtTimKiem_TextChanged);
            }
            if (this.chkHienThiDaXoa != null) // <<< Gắn sự kiện cho CheckBox mới
            {
                this.chkHienThiDaXoa.CheckedChanged -= new System.EventHandler(this.chkHienThiDaXoa_CheckedChanged);
                this.chkHienThiDaXoa.CheckedChanged += new System.EventHandler(this.chkHienThiDaXoa_CheckedChanged);
            }
            if (txtTenTacGia != null)
            {
                txtTenTacGia.TextChanged -= InputField_Changed;
                txtTenTacGia.TextChanged += InputField_Changed;
            }
            if (this.btnKhoiPhuc != null) // <<< Gắn sự kiện cho Button mới
            {
                this.btnKhoiPhuc.Click -= new System.EventHandler(this.btnKhoiPhuc_Click);
                this.btnKhoiPhuc.Click += new System.EventHandler(this.btnKhoiPhuc_Click);
            }
            // Gắn các sự kiện khác nếu cần (btnThem, btnSua, btnXoa, btnLuu, btnBoQua đã được gắn trong Designer)
        }

        // --- SỰ KIỆN LOAD USERCONTROL ---
        private void ucQuanLyTacGia_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                if (txtMaTacGia != null)
                {
                    txtMaTacGia.ReadOnly = true;
                }
                // Không gọi InitializeDataAsync ở đây
            }
        }

        // --- PHƯƠNG THỨC KHỞI TẠO DỮ LIỆU ---
        public async Task InitializeDataAsync()
        {
            System.Diagnostics.Debug.WriteLine("ucQuanLyTacGia.InitializeDataAsync called.");
            if (!this.DesignMode)
            {
                _isAdding = false;
                ClearInputFields();
                if (chkHienThiDaXoa != null) chkHienThiDaXoa.Checked = false; // Bỏ check khi load lại
                SetControlState(false, false);
                await LoadDataGrid(); // Load ban đầu không bao gồm đã xóa
            }
        }

        // --- HÀM TẢI DỮ LIỆU & CÀI ĐẶT GIAO DIỆN ---

        /// <summary>
        /// Tải hoặc làm mới dữ liệu cho DataGridView.
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm.</param>
        /// <param name="includeDeleted">True để bao gồm các tác giả đã bị ẩn.</param>
        private async Task LoadDataGrid(string? searchTerm = null, bool includeDeleted = false) // <<< THÊM includeDeleted
        {
            System.Diagnostics.Debug.WriteLine($"--- LoadDataGrid START (Search: '{searchTerm ?? ""}', IncludeDeleted: {includeDeleted}) ---");
            int? selectedId = null;
            if (dgvTacGia?.SelectedRows.Count > 0 && dgvTacGia.SelectedRows[0].DataBoundItem is TacGiaDTO currentDto)
            {
                selectedId = currentDto.Id;
                System.Diagnostics.Debug.WriteLine($"LoadDataGrid: Selected ID to restore: {selectedId}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("LoadDataGrid: No row selected before load.");
            }

            this.Cursor = Cursors.WaitCursor;
            List<TacGiaDTO> danhSach = new List<TacGiaDTO>();

            try
            {
                System.Diagnostics.Debug.WriteLine($"LoadDataGrid: Calling BUS (Search: '{searchTerm ?? ""}', IncludeDeleted: {includeDeleted})");

                // *** GỌI BUS VỚI THAM SỐ includeDeleted ***
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    danhSach = await _busTacGia.GetAllTacGiaAsync(includeDeleted);
                }
                else
                {
                    danhSach = await _busTacGia.SearchTacGiaAsync(searchTerm, includeDeleted);
                }
                System.Diagnostics.Debug.WriteLine($"LoadDataGrid: Received danhSach from BUS. Count={danhSach?.Count ?? 0}");

                await Task.Yield(); // Đảm bảo về luồng UI

                if (dgvTacGia == null) { System.Diagnostics.Debug.WriteLine("LoadDataGrid: dgvTacGia is null after Yield."); return; }

                dgvTacGia.DataSource = null;
                System.Diagnostics.Debug.WriteLine("LoadDataGrid: DataSource set to null.");

                if (danhSach == null)
                {
                    System.Diagnostics.Debug.WriteLine($"WARNING: BUS returned null. Assigning empty list.");
                    danhSach = new List<TacGiaDTO>();
                }

                dgvTacGia.DataSource = danhSach; // Bind List<DTO>
                System.Diagnostics.Debug.WriteLine($"LoadDataGrid: New DataSource assigned. RowCount={dgvTacGia.RowCount}");

                if (dgvTacGia.Columns.Count == 0 || !dgvTacGia.Columns.Contains("colMaTacGia"))
                {
                    System.Diagnostics.Debug.WriteLine("LoadDataGrid: Setting up DataGridView columns...");
                    SetupDataGridViewColumns();
                }
                else { System.Diagnostics.Debug.WriteLine("LoadDataGrid: Columns already setup."); }

                // *** THÊM LOGIC ĐỊNH DẠNG DÒNG ĐÃ XÓA ***
                foreach (DataGridViewRow row in dgvTacGia.Rows)
                {
                    if (row.DataBoundItem is TacGiaDTO rowDto && rowDto.DaAn)
                    {
                        // Áp dụng style cho dòng đã bị xóa mềm
                        row.DefaultCellStyle.BackColor = Color.LightGray;
                        row.DefaultCellStyle.ForeColor = Color.DarkGray;
                        // Tùy chọn: Thêm gạch ngang
                        // row.DefaultCellStyle.Font = new Font(dgvTacGia.Font, FontStyle.Strikeout);
                    }
                    else if (row.DataBoundItem is TacGiaDTO) // Reset style cho dòng không bị xóa
                    {
                        // Đảm bảo trả về style mặc định nếu dòng không bị xóa
                        row.DefaultCellStyle.BackColor = dgvTacGia.DefaultCellStyle.BackColor; // Hoặc màu mặc định của bạn
                        row.DefaultCellStyle.ForeColor = dgvTacGia.DefaultCellStyle.ForeColor;
                        // row.DefaultCellStyle.Font = dgvTacGia.Font; // Reset font nếu dùng Strikeout
                    }
                }
                System.Diagnostics.Debug.WriteLine("LoadDataGrid: Applied styling for deleted rows.");


                bool rowSelectedAfterLoad = false;
                if (selectedId.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine($"LoadDataGrid: Attempting to re-select row by ID: {selectedId.Value}");
                    rowSelectedAfterLoad = SelectRowById(selectedId.Value);
                    if (!rowSelectedAfterLoad) System.Diagnostics.Debug.WriteLine($"LoadDataGrid: Failed to re-select row with ID {selectedId.Value}.");
                }
                else { System.Diagnostics.Debug.WriteLine("LoadDataGrid: No ID to re-select."); }

                if (!rowSelectedAfterLoad)
                {
                    System.Diagnostics.Debug.WriteLine("LoadDataGrid: No row selected after re-selection attempt.");
                    if (!_isAdding && !IsEditing())
                    {
                        System.Diagnostics.Debug.WriteLine("LoadDataGrid: Not adding/editing, clearing input fields.");
                        ClearInputFields(clearSearchBox: false);
                    }
                    if (dgvTacGia.SelectedRows.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("LoadDataGrid: No rows selected, updating control state.");
                        SetControlState(_isAdding || IsEditing(), false);
                    }
                }

                dgvTacGia.Refresh();
                dgvTacGia.PerformLayout();
                System.Diagnostics.Debug.WriteLine("--- LoadDataGrid TRY block finished successfully. ---");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadDataGrid: Catch block - Awaiting Task.Yield() for error message...");
                await Task.Yield();
                System.Diagnostics.Debug.WriteLine("LoadDataGrid: Catch block - Task.Yield() completed.");
                ShowError($"Lỗi khi tải danh sách tác giả: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"ERROR (ucQuanLyTacGia - LoadDataGrid - Search: '{searchTerm}', IncludeDeleted: {includeDeleted}): {ex.ToString()}");
                ClearInputFields(clearSearchBox: false);
                SetControlState(false, false);
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine("--- LoadDataGrid FINALLY block START ---");
                System.Diagnostics.Debug.WriteLine($"LoadDataGrid: Finally block - Awaiting Task.Yield()...");
                await Task.Yield();
                System.Diagnostics.Debug.WriteLine("LoadDataGrid: Finally block - Task.Yield() completed.");
                this.Cursor = Cursors.Default;

                if (dgvTacGia != null && !dgvTacGia.IsDisposed)
                {
                    try
                    {
                        dgvTacGia.SelectionChanged -= dgvTacGia_SelectionChanged;
                        dgvTacGia.SelectionChanged += dgvTacGia_SelectionChanged;
                        System.Diagnostics.Debug.WriteLine("LoadDataGrid: Re-subscribed to SelectionChanged.");
                    }
                    catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"LoadDataGrid Finally Error: {ex.Message}"); }
                }
                else { System.Diagnostics.Debug.WriteLine("LoadDataGrid Finally: dgvTacGia is null or disposed."); }

                System.Diagnostics.Debug.WriteLine("--- LoadDataGrid FINALLY block END ---");
            }
            System.Diagnostics.Debug.WriteLine("--- LoadDataGrid END ---");
        }

        private void SetupDataGridViewColumns()
        {
            if (dgvTacGia == null || dgvTacGia.DataSource == null || dgvTacGia.Columns.Count > 0) return;
            System.Diagnostics.Debug.WriteLine("SetupDataGridViewColumns: Proceeding with setup.");
            try
            {
                dgvTacGia.AutoGenerateColumns = false;
                dgvTacGia.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Matacgia", HeaderText = "Mã Tác Giả", Name = "colMaTacGia", Width = 120 });
                dgvTacGia.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenTacGia", HeaderText = "Tên Tác Giả", Name = "colTenTacGia", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                dgvTacGia.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", Name = "colId", Visible = false });
                // Không thêm cột DaAn, sẽ dùng style để thể hiện
                // dgvTacGia.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "DaAn", HeaderText = "Đã ẩn", Name = "colDaAn", Width = 80 });

                System.Diagnostics.Debug.WriteLine($"SetupDataGridViewColumns: Added {dgvTacGia.Columns.Count} columns.");
                dgvTacGia.ReadOnly = true;
                dgvTacGia.AllowUserToAddRows = false;
                dgvTacGia.AllowUserToDeleteRows = false;
                dgvTacGia.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvTacGia.MultiSelect = false;
                dgvTacGia.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
                dgvTacGia.RowHeadersVisible = false;
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Lỗi cấu hình cột DataGridView Tác Giả: {ex.Message}"); }
        }

        // --- HÀM TIỆN ÍCH TRẠNG THÁI & GIAO DIỆN ---

        private bool IsEditing() => !_isAdding && (btnLuu?.Enabled == true || btnBoQua?.Enabled == true);

        private void ClearInputFields(bool clearSearchBox = true)
        {
            System.Diagnostics.Debug.WriteLine($"ClearInputFields: clearSearchBox={clearSearchBox}");
            if (txtId != null) txtId.Clear();
            if (txtMaTacGia != null) { txtMaTacGia.Clear(); txtMaTacGia.Text = _isAdding ? "(Tự động)" : ""; }
            if (txtTenTacGia != null) txtTenTacGia.Clear();
            if (clearSearchBox && txtTimKiem != null)
            {
                System.Diagnostics.Debug.WriteLine("ClearInputFields: Clearing search box.");
                this.txtTimKiem.TextChanged -= this.txtTimKiem_TextChanged;
                txtTimKiem.Clear();
                this.txtTimKiem.TextChanged += this.txtTimKiem_TextChanged;
            }
            if (_isAdding && txtTenTacGia != null && txtTenTacGia.Enabled) txtTenTacGia.Focus();
        }

        // *** CẬP NHẬT SetControlState ***
        private void SetControlState(bool isAddingOrEditing, bool rowSelected)
        {
            System.Diagnostics.Debug.WriteLine($"SetControlState: isAddingOrEditing={isAddingOrEditing}, rowSelected={rowSelected}");

            TacGiaDTO? selectedDto = null;
            bool isDeleted = false;
            if (rowSelected && dgvTacGia?.SelectedRows.Count == 1 && dgvTacGia.SelectedRows[0].DataBoundItem is TacGiaDTO dto)
            {
                selectedDto = dto;
                isDeleted = dto.DaAn; // Lấy trạng thái DaAn của dòng đang chọn
                System.Diagnostics.Debug.WriteLine($"SetControlState: Selected DTO ID={selectedDto.Id}, DaAn={isDeleted}");
            }
            else { System.Diagnostics.Debug.WriteLine($"SetControlState: No single row selected or invalid DTO."); }

            // Nút cơ bản
            if (btnThem != null) btnThem.Enabled = !isAddingOrEditing;
            // Sửa/Xóa (ẩn) chỉ bật khi không thêm/sửa, có dòng chọn VÀ dòng đó chưa bị ẩn
            if (btnSua != null) btnSua.Enabled = !isAddingOrEditing && rowSelected && !isDeleted;
            if (btnXoa != null) btnXoa.Enabled = !isAddingOrEditing && rowSelected && !isDeleted; // btnXoa là Soft Delete

            // Nút Lưu/Bỏ qua
            if (btnLuu != null) btnLuu.Enabled = isAddingOrEditing && HasUnsavedChanges();
            if (btnBoQua != null) btnBoQua.Enabled = isAddingOrEditing;

            // Nút Khôi phục chỉ bật khi không thêm/sửa, có dòng chọn VÀ dòng đó ĐÃ bị ẩn
            if (btnKhoiPhuc != null) btnKhoiPhuc.Enabled = !isAddingOrEditing && rowSelected && isDeleted;

            // Form chi tiết
            if (txtId != null) txtId.Enabled = false;
            if (txtMaTacGia != null) txtMaTacGia.ReadOnly = true;
            if (txtTenTacGia != null) txtTenTacGia.Enabled = isAddingOrEditing;

            // Lưới và Tìm kiếm
            if (dgvTacGia != null) dgvTacGia.Enabled = !isAddingOrEditing;
            if (txtTimKiem != null) txtTimKiem.Enabled = !isAddingOrEditing;
            if (chkHienThiDaXoa != null) chkHienThiDaXoa.Enabled = !isAddingOrEditing; // Checkbox cũng bị khóa khi thêm/sửa

            // Placeholder Mã Tác Giả
            if (isAddingOrEditing && _isAdding && txtMaTacGia != null && string.IsNullOrEmpty(txtMaTacGia.Text))
            { txtMaTacGia.Text = "(Tự động)"; }
            else if (!isAddingOrEditing && txtMaTacGia != null && txtMaTacGia.Text == "(Tự động)")
            { txtMaTacGia.Text = ""; }
            System.Diagnostics.Debug.WriteLine($"SetControlState finished. btnSua.Enabled={btnSua?.Enabled}, btnXoa.Enabled={btnXoa?.Enabled}, btnKhoiPhuc.Enabled={btnKhoiPhuc?.Enabled}");
        }

        // *** CẬP NHẬT DisplaySelectedRow ***
        private void DisplaySelectedRow()
        {
            System.Diagnostics.Debug.WriteLine($"DisplaySelectedRow triggered. IsAdding={_isAdding}, IsEditing={IsEditing()}. SelectedRows.Count = {dgvTacGia?.SelectedRows.Count}");
            if (_isAdding || IsEditing())
            {
                System.Diagnostics.Debug.WriteLine("DisplaySelectedRow: Skipping update because currently adding or editing.");
                return;
            }

            TacGiaDTO? dto = null;
            if (dgvTacGia?.SelectedRows.Count == 1 && dgvTacGia.SelectedRows[0].DataBoundItem is TacGiaDTO boundDto)
            {
                dto = boundDto;
                System.Diagnostics.Debug.WriteLine($"DisplaySelectedRow: Bound DTO found (ID: {dto.Id}, DaAn: {dto.DaAn}).");
            }
            else { System.Diagnostics.Debug.WriteLine($"DisplaySelectedRow: SelectedRows.Count is not 1 or invalid DTO."); }

            if (txtId != null) txtId.Text = dto?.Id.ToString() ?? string.Empty;
            if (txtMaTacGia != null) txtMaTacGia.Text = dto?.Matacgia ?? string.Empty;
            if (txtTenTacGia != null) txtTenTacGia.Text = dto?.TenTacGia ?? string.Empty;

            bool rowSelected = dto != null;
            System.Diagnostics.Debug.WriteLine($"DisplaySelectedRow: Calling SetControlState with rowSelected={rowSelected}.");
            // SetControlState sẽ tự xử lý trạng thái các nút dựa trên dto.DaAn
            SetControlState(false, rowSelected);
        }

        private bool HasUnsavedChanges()
        {
            if (_isAdding)
            {
                bool changes = !string.IsNullOrWhiteSpace(txtTenTacGia?.Text);
                System.Diagnostics.Debug.WriteLine($"HasUnsavedChanges (Add mode): {changes}");
                return changes;
            }
            else // Đang sửa
            {
                if (dgvTacGia?.SelectedRows.Count == 1 && dgvTacGia.SelectedRows[0].DataBoundItem is TacGiaDTO originalDtoFromGrid)
                {
                    string currentTen = txtTenTacGia?.Text.Trim() ?? "";
                    string originalTen = originalDtoFromGrid.TenTacGia?.Trim() ?? "";
                    bool tenChanged = (currentTen != originalTen);
                    System.Diagnostics.Debug.WriteLine($"HasUnsavedChanges (Edit mode): {tenChanged}");
                    return tenChanged;
                }
                System.Diagnostics.Debug.WriteLine($"HasUnsavedChanges (Edit mode): No single row selected or no DTO bound, returning false.");
                return false;
            }
        }

        // --- SỰ KIỆN CỦA CÁC BUTTON ---

        private void btnThem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnThem_Click fired.");
            if ((IsEditing() || _isAdding) && HasUnsavedChanges())
            {
                DialogResult confirm = ShowConfirm("Dữ liệu chưa được lưu. Bạn có muốn hủy bỏ thay đổi và thêm mới?");
                if (confirm == DialogResult.No) { System.Diagnostics.Debug.WriteLine("btnThem_Click cancelled."); return; }
                System.Diagnostics.Debug.WriteLine("btnThem_Click: Confirmed cancelling unsaved changes.");
            }

            _isAdding = true;
            dgvTacGia?.ClearSelection();
            ClearInputFields(clearSearchBox: false);
            SetControlState(true, false); // Đang thêm, không có dòng chọn
            if (txtTenTacGia != null) { System.Diagnostics.Debug.WriteLine("btnThem_Click: Focusing txtTenTacGia."); txtTenTacGia.Focus(); }
            System.Diagnostics.Debug.WriteLine("btnThem_Click finished. State: Adding.");
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnSua_Click fired.");
            // Chỉ cho sửa nếu đang View, có 1 dòng chọn VÀ dòng đó chưa bị xóa mềm
            if (dgvTacGia?.SelectedRows.Count == 1 && !_isAdding && !IsEditing())
            {
                if (dgvTacGia.SelectedRows[0].DataBoundItem is TacGiaDTO dto && !dto.DaAn) // <<< KIỂM TRA DaAn
                {
                    _isAdding = false;
                    SetControlState(true, true); // Đang sửa, có dòng chọn
                    if (txtTenTacGia != null && txtTenTacGia.Enabled) { System.Diagnostics.Debug.WriteLine("btnSua_Click: Focusing txtTenTacGia."); txtTenTacGia.Focus(); }
                    System.Diagnostics.Debug.WriteLine("btnSua_Click finished. State: Editing.");
                }
                else if (dgvTacGia.SelectedRows[0].DataBoundItem is TacGiaDTO deletedDto && deletedDto.DaAn)
                {
                    System.Diagnostics.Debug.WriteLine("btnSua_Click aborted: Selected row is already deleted.");
                    ShowWarning("Không thể sửa tác giả đã bị ẩn. Vui lòng khôi phục trước.");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("btnSua_Click aborted: Invalid DTO.");
                    ShowError("Không thể lấy thông tin tác giả để sửa.");
                }
            }
            else if (dgvTacGia?.SelectedRows.Count == 0) { System.Diagnostics.Debug.WriteLine("btnSua_Click aborted: No row selected."); ShowWarning("Vui lòng chọn tác giả cần sửa."); }
            else if (dgvTacGia?.SelectedRows.Count > 1) { System.Diagnostics.Debug.WriteLine("btnSua_Click aborted: Multiple rows selected."); ShowWarning("Chỉ có thể sửa một tác giả mỗi lần."); }
            else { System.Diagnostics.Debug.WriteLine($"btnSua_Click aborted: Already in Adding ({_isAdding}) or Editing ({IsEditing()}) mode."); }
        }

        private async void btnLuu_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnLuu_Click fired.");
            string tenTacGia = txtTenTacGia?.Text.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(tenTacGia))
            {
                System.Diagnostics.Debug.WriteLine("btnLuu_Click aborted: TenTacGia is empty.");
                ShowWarning("Tên tác giả không được rỗng.");
                txtTenTacGia?.Focus();
                return;
            }

            int currentId = 0;
            string? originalMaTacGia = null;
            bool originalDaAn = false; // Lưu trạng thái DaAn gốc khi sửa

            if (!_isAdding) // Đang sửa
            {
                if (dgvTacGia?.SelectedRows.Count == 1 && dgvTacGia.SelectedRows[0].DataBoundItem is TacGiaDTO originalDtoFromGrid)
                {
                    currentId = originalDtoFromGrid.Id;
                    if (currentId <= 0) { ShowError("ID không hợp lệ để cập nhật."); return; }
                    originalMaTacGia = originalDtoFromGrid.Matacgia;
                    originalDaAn = originalDtoFromGrid.DaAn; // Lấy trạng thái DaAn gốc
                    System.Diagnostics.Debug.WriteLine($"btnLuu_Click: Update mode. Original DTO ID={currentId}, Ma={originalMaTacGia}, DaAn={originalDaAn}.");
                }
                else { ShowError("Không xác định được dòng dữ liệu gốc để cập nhật."); return; }
            }
            else { System.Diagnostics.Debug.WriteLine("btnLuu_Click: Add mode."); }

            TacGiaDTO tacGiaDto = new TacGiaDTO
            {
                Id = currentId,
                Matacgia = originalMaTacGia, // Mã không cho sửa ở GUI này
                TenTacGia = tenTacGia,
                DaAn = originalDaAn // Giữ nguyên trạng thái DaAn khi lưu
            };

            bool success = false;
            string? errorMsg = null;
            TacGiaDTO? processedDto = null;

            if (btnLuu != null) btnLuu.Enabled = false;
            if (btnBoQua != null) btnBoQua.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (_isAdding)
                {
                    System.Diagnostics.Debug.WriteLine($"Calling BUS.AddTacGiaAsync with Ten='{tacGiaDto.TenTacGia}'");
                    processedDto = await _busTacGia.AddTacGiaAsync(tacGiaDto);
                    success = processedDto != null;
                    if (!success) errorMsg ??= "Thêm thất bại.";
                }
                else // Đang sửa
                {
                    System.Diagnostics.Debug.WriteLine($"Calling BUS.UpdateTacGiaAsync with ID={tacGiaDto.Id}, Ten='{tacGiaDto.TenTacGia}'");
                    success = await _busTacGia.UpdateTacGiaAsync(tacGiaDto); // BUS sẽ kiểm tra DaAn=false trước khi update
                    if (success) processedDto = tacGiaDto; // DTO gửi đi là DTO đã cập nhật
                    if (!success && string.IsNullOrEmpty(errorMsg)) errorMsg = "Cập nhật thất bại.";
                }
            }
            catch (ArgumentException argEx) { errorMsg = $"Lỗi dữ liệu nhập: {argEx.Message}"; }
            catch (InvalidOperationException invOpEx) { errorMsg = $"Lỗi nghiệp vụ: {invOpEx.Message}"; } // Bắt lỗi trùng tên/mã hoặc không tìm thấy/đã ẩn
            catch (Exception ex) { errorMsg = HandleGeneralError("lưu tác giả", ex); }
            finally
            {
                await Task.Yield(); this.Cursor = Cursors.Default;
            }

            await Task.Yield(); // Về UI thread xử lý kết quả

            if (success)
            {
                bool wasAddingBeforeSave = _isAdding;
                _isAdding = false;
                ShowInfo(wasAddingBeforeSave ? "Thêm tác giả thành công!" : "Cập nhật tác giả thành công!");

                string currentSearchTerm = txtTimKiem?.Text.Trim() ?? "";
                bool includeDeleted = chkHienThiDaXoa?.Checked ?? false; // Lấy trạng thái checkbox
                System.Diagnostics.Debug.WriteLine($"btnLuu_Click: Reloading grid with search term: '{currentSearchTerm}', includeDeleted: {includeDeleted}");
                await LoadDataGrid(currentSearchTerm, includeDeleted); // Tải lại lưới với trạng thái checkbox

                await Task.Yield(); // Về UI thread trước khi chọn dòng

                bool rowSelected = false;
                if (processedDto != null && processedDto.Id > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"btnLuu_Click: Attempting to select processed DTO with ID: {processedDto.Id}");
                    rowSelected = SelectRowById(processedDto.Id);
                }
                else { System.Diagnostics.Debug.WriteLine("btnLuu_Click: processedDto is null or has invalid ID."); }
                // SetControlState sẽ được gọi bởi SelectionChanged hoặc trong LoadDataGrid nếu không chọn được
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("btnLuu_Click: Save failed.");
                ShowError(errorMsg ?? "Lưu thất bại (lý do không xác định).");
                bool rowCurrentlySelected = dgvTacGia?.SelectedRows.Count > 0;
                SetControlState(_isAdding || IsEditing(), rowCurrentlySelected); // Giữ nguyên trạng thái thêm/sửa
                if (txtTenTacGia != null && txtTenTacGia.Enabled) txtTenTacGia.Focus();
            }
            System.Diagnostics.Debug.WriteLine("btnLuu_Click finished.");
        }

        private async void btnBoQua_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnBoQua_Click fired.");
            bool currentlyEditing = IsEditing();
            bool currentlyAdding = _isAdding;

            if ((currentlyAdding || currentlyEditing) && HasUnsavedChanges())
            {
                DialogResult confirm = ShowConfirm("Dữ liệu chưa được lưu. Bạn có muốn hủy bỏ thay đổi?");
                if (confirm == DialogResult.No) { System.Diagnostics.Debug.WriteLine("btnBoQua_Click cancelled."); return; }
                System.Diagnostics.Debug.WriteLine("btnBoQua_Click: Confirmed cancelling unsaved changes.");
            }
            else { System.Diagnostics.Debug.WriteLine("btnBoQua_Click: No unsaved changes or not in add/edit mode."); }

            int? selectedIdBeforeCancel = null;
            if (currentlyEditing && dgvTacGia?.SelectedRows.Count == 1 && dgvTacGia.SelectedRows[0].DataBoundItem is TacGiaDTO dto)
            { selectedIdBeforeCancel = dto.Id; System.Diagnostics.Debug.WriteLine($"btnBoQua_Click: Saving selected ID {selectedIdBeforeCancel} before cancel."); }
            else { System.Diagnostics.Debug.WriteLine("btnBoQua_Click: No selected ID to save."); }

            _isAdding = false;
            ClearInputFields(clearSearchBox: true); // Xóa cả ô tìm kiếm
            if (chkHienThiDaXoa != null) chkHienThiDaXoa.Checked = false; // Bỏ check khi bỏ qua

            System.Diagnostics.Debug.WriteLine("btnBoQua_Click: Calling LoadDataGrid to refresh data (includeDeleted=false).");
            await LoadDataGrid(); // Tải lại tất cả (không bao gồm đã xóa)

            await Task.Yield(); // Về UI thread

            bool rowSelectedAfterCancel = false;
            if (currentlyEditing && selectedIdBeforeCancel.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"btnBoQua_Click: Attempting to re-select original row ID: {selectedIdBeforeCancel.Value}");
                rowSelectedAfterCancel = SelectRowById(selectedIdBeforeCancel.Value);
                if (!rowSelectedAfterCancel) System.Diagnostics.Debug.WriteLine($"btnBoQua_Click: Failed to re-select row.");
            }
            else { System.Diagnostics.Debug.WriteLine("btnBoQua_Click: Not editing or no ID saved."); }
            // SetControlState đã được gọi bởi SelectionChanged/LoadDataGrid

            System.Diagnostics.Debug.WriteLine("btnBoQua_Click finished. State: View.");
        }

        // *** CẬP NHẬT btnXoa_Click thành Soft Delete ***
        private async void btnXoa_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnXoa_Click (Soft Delete) fired.");
            // Chỉ cho xóa mềm nếu đang View, có 1 dòng chọn VÀ dòng đó chưa bị xóa mềm
            if (dgvTacGia?.SelectedRows.Count == 1 && !_isAdding && !IsEditing())
            {
                if (dgvTacGia.SelectedRows[0].DataBoundItem is TacGiaDTO dto && !dto.DaAn) // <<< KIỂM TRA DaAn
                {
                    int idToDelete = dto.Id;
                    string tenTacGia = dto.TenTacGia ?? "?";

                    if (idToDelete <= 0) { ShowError("ID không hợp lệ để ẩn."); return; }

                    // *** THAY ĐỔI THÔNG BÁO XÁC NHẬN ***
                    DialogResult confirm = ShowConfirm($"Bạn có chắc chắn muốn ẩn tác giả '{tenTacGia}' (ID: {idToDelete})?");

                    if (confirm == DialogResult.Yes)
                    {
                        System.Diagnostics.Debug.WriteLine($"btnXoa_Click: User confirmed soft deletion of ID {idToDelete}.");
                        bool success = false;
                        string? errorMsg = null;

                        // Vô hiệu hóa các nút
                        if (btnXoa != null) btnXoa.Enabled = false;
                        if (btnSua != null) btnSua.Enabled = false;
                        if (btnThem != null) btnThem.Enabled = false;
                        if (btnKhoiPhuc != null) btnKhoiPhuc.Enabled = false;
                        if (dgvTacGia != null) dgvTacGia.Enabled = false;
                        this.Cursor = Cursors.WaitCursor;

                        try
                        {
                            // *** GỌI PHƯƠNG THỨC SOFT DELETE ***
                            System.Diagnostics.Debug.WriteLine($"Calling BUS.SoftDeleteTacGiaAsync({idToDelete})...");
                            success = await _busTacGia.SoftDeleteTacGiaAsync(idToDelete);
                            System.Diagnostics.Debug.WriteLine($"BUS.SoftDeleteTacGiaAsync returned success = {success}");
                            if (!success && string.IsNullOrEmpty(errorMsg)) { errorMsg = "Ẩn tác giả thất bại."; }
                        }
                        catch (Exception ex) { errorMsg = HandleGeneralError("ẩn tác giả", ex); }
                        finally
                        {
                            await Task.Yield(); this.Cursor = Cursors.Default;
                        }

                        await Task.Yield(); // Về UI thread xử lý kết quả

                        if (success)
                        {
                            // *** THAY ĐỔI THÔNG BÁO KẾT QUẢ ***
                            ShowInfo("Ẩn tác giả thành công!");
                            string currentSearchTerm = txtTimKiem?.Text.Trim() ?? "";
                            bool includeDeleted = chkHienThiDaXoa?.Checked ?? false;
                            System.Diagnostics.Debug.WriteLine($"btnXoa_Click: Reloading grid with search term: '{currentSearchTerm}', includeDeleted: {includeDeleted}");
                            await LoadDataGrid(currentSearchTerm, includeDeleted); // Tải lại lưới với trạng thái checkbox
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("btnXoa_Click: Soft deletion failed.");
                            ShowError(errorMsg ?? "Ẩn tác giả thất bại (lý do không xác định).");
                            // Bật lại các nút phù hợp (SetControlState sẽ làm khi LoadDataGrid xong)
                            bool rowStillSelected = dgvTacGia?.SelectedRows.Count > 0;
                            SetControlState(false, rowStillSelected);
                        }
                    }
                    else { System.Diagnostics.Debug.WriteLine("btnXoa_Click cancelled by user."); }
                }
                else if (dgvTacGia.SelectedRows[0].DataBoundItem is TacGiaDTO deletedDto && deletedDto.DaAn)
                {
                    System.Diagnostics.Debug.WriteLine("btnXoa_Click aborted: Selected row is already deleted.");
                    ShowWarning("Tác giả này đã được ẩn.");
                }
                else { System.Diagnostics.Debug.WriteLine("btnXoa_Click aborted: Invalid DTO."); ShowError("Không thể xác định đối tượng cần ẩn."); }
            }
            else if (dgvTacGia?.SelectedRows.Count == 0) { System.Diagnostics.Debug.WriteLine("btnXoa_Click aborted: No row selected."); ShowWarning("Vui lòng chọn tác giả cần ẩn."); }
            else if (dgvTacGia?.SelectedRows.Count > 1) { System.Diagnostics.Debug.WriteLine("btnXoa_Click aborted: Multiple rows selected."); ShowWarning("Chỉ có thể ẩn một tác giả mỗi lần."); }
            else { System.Diagnostics.Debug.WriteLine($"btnXoa_Click aborted: Already in Adding ({_isAdding}) or Editing ({IsEditing()}) mode."); }

            System.Diagnostics.Debug.WriteLine("btnXoa_Click finished.");
        }

        // *** THÊM SỰ KIỆN CHO NÚT KHÔI PHỤC ***
        private async void btnKhoiPhuc_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnKhoiPhuc_Click fired.");
            // Chỉ khôi phục khi đang View, có 1 dòng chọn VÀ dòng đó ĐÃ bị xóa mềm
            if (dgvTacGia?.SelectedRows.Count == 1 && !_isAdding && !IsEditing())
            {
                if (dgvTacGia.SelectedRows[0].DataBoundItem is TacGiaDTO dto && dto.DaAn) // <<< KIỂM TRA DaAn
                {
                    int idToRestore = dto.Id;
                    string tenTacGia = dto.TenTacGia ?? "?";

                    DialogResult confirm = ShowConfirm($"Bạn có chắc chắn muốn khôi phục tác giả '{tenTacGia}' (ID: {idToRestore})?");

                    if (confirm == DialogResult.Yes)
                    {
                        System.Diagnostics.Debug.WriteLine($"btnKhoiPhuc_Click: User confirmed restoration of ID {idToRestore}.");
                        bool success = false;
                        string? errorMsg = null;

                        // Vô hiệu hóa nút
                        if (btnKhoiPhuc != null) btnKhoiPhuc.Enabled = false;
                        if (dgvTacGia != null) dgvTacGia.Enabled = false;
                        this.Cursor = Cursors.WaitCursor;

                        try
                        {
                            System.Diagnostics.Debug.WriteLine($"Calling BUS.RestoreTacGiaAsync({idToRestore})...");
                            success = await _busTacGia.RestoreTacGiaAsync(idToRestore);
                            System.Diagnostics.Debug.WriteLine($"BUS.RestoreTacGiaAsync returned success = {success}");
                            if (!success && string.IsNullOrEmpty(errorMsg)) { errorMsg = "Khôi phục thất bại."; }
                        }
                        catch (Exception ex) { errorMsg = HandleGeneralError("khôi phục tác giả", ex); }
                        finally
                        {
                            await Task.Yield(); this.Cursor = Cursors.Default;
                        }

                        await Task.Yield(); // Về UI thread xử lý kết quả

                        if (success)
                        {
                            ShowInfo("Khôi phục tác giả thành công!");
                            string currentSearchTerm = txtTimKiem?.Text.Trim() ?? "";
                            bool includeDeleted = chkHienThiDaXoa?.Checked ?? false;
                            System.Diagnostics.Debug.WriteLine($"btnKhoiPhuc_Click: Reloading grid with search term: '{currentSearchTerm}', includeDeleted: {includeDeleted}");
                            await LoadDataGrid(currentSearchTerm, includeDeleted); // Tải lại lưới
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("btnKhoiPhuc_Click: Restoration failed.");
                            ShowError(errorMsg ?? "Khôi phục tác giả thất bại (lý do không xác định).");
                            // Bật lại các nút nếu cần (SetControlState sẽ làm khi LoadDataGrid xong)
                            bool rowStillSelected = dgvTacGia?.SelectedRows.Count > 0;
                            SetControlState(false, rowStillSelected);
                        }
                    }
                    else { System.Diagnostics.Debug.WriteLine("btnKhoiPhuc_Click cancelled by user."); }
                }
                else if (dgvTacGia.SelectedRows[0].DataBoundItem is TacGiaDTO notDeletedDto && !notDeletedDto.DaAn)
                {
                    System.Diagnostics.Debug.WriteLine("btnKhoiPhuc_Click aborted: Selected row is not deleted.");
                    ShowWarning("Tác giả này chưa bị ẩn.");
                }
                else { System.Diagnostics.Debug.WriteLine("btnKhoiPhuc_Click aborted: Invalid DTO."); ShowError("Không thể xác định đối tượng cần khôi phục."); }
            }
            else if (dgvTacGia?.SelectedRows.Count == 0) { System.Diagnostics.Debug.WriteLine("btnKhoiPhuc_Click aborted: No row selected."); ShowWarning("Vui lòng chọn tác giả đã bị ẩn để khôi phục."); }
            else if (dgvTacGia?.SelectedRows.Count > 1) { System.Diagnostics.Debug.WriteLine("btnKhoiPhuc_Click aborted: Multiple rows selected."); ShowWarning("Chỉ có thể khôi phục một tác giả mỗi lần."); }
            else { System.Diagnostics.Debug.WriteLine($"btnKhoiPhuc_Click aborted: Currently in Adding ({_isAdding}) or Editing ({IsEditing()}) mode."); }

            System.Diagnostics.Debug.WriteLine("btnKhoiPhuc_Click finished.");
        }


        // --- SỰ KIỆN CỦA DATAGRIDVIEW ---
        private void dgvTacGia_SelectionChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"dgvTacGia_SelectionChanged fired. IsAdding={_isAdding}, IsEditing={IsEditing()}. SelectedRows.Count = {dgvTacGia?.SelectedRows.Count}");
            if (!_isAdding && !IsEditing())
            {
                System.Diagnostics.Debug.WriteLine("dgvTacGia_SelectionChanged: Calling DisplaySelectedRow.");
                DisplaySelectedRow(); // Sẽ gọi SetControlState bên trong
            }
            else { System.Diagnostics.Debug.WriteLine("dgvTacGia_SelectionChanged: Skipping DisplaySelectedRow."); }
        }

        private void dgvTacGia_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("dgvTacGia_DoubleClick fired.");
            // Chỉ cho sửa nếu đang View, có 1 dòng chọn VÀ dòng đó chưa bị xóa mềm
            if (dgvTacGia?.SelectedRows.Count == 1 && !_isAdding && !IsEditing())
            {
                if (dgvTacGia.SelectedRows[0].DataBoundItem is TacGiaDTO dto && !dto.DaAn) // <<< KIỂM TRA DaAn
                {
                    System.Diagnostics.Debug.WriteLine("dgvTacGia_DoubleClick: Calling btnSua_Click.");
                    btnSua_Click(sender, e);
                }
                else { System.Diagnostics.Debug.WriteLine("dgvTacGia_DoubleClick: Skipping btnSua_Click because row is deleted."); }
            }
            else { System.Diagnostics.Debug.WriteLine($"dgvTacGia_DoubleClick: Skipping btnSua_Click."); }
        }

        // --- INPUT CHANGE HANDLER ---
        private void InputField_Changed(object? sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"InputField_Changed fired from {sender?.GetType().Name}.");
            if (_isAdding || IsEditing())
            {
                System.Diagnostics.Debug.WriteLine("InputField_Changed: Calling SetControlState.");
                SetControlState(true, dgvTacGia?.SelectedRows.Count > 0);
            }
            else { System.Diagnostics.Debug.WriteLine("InputField_Changed: Not in Add/Edit mode."); }
        }

        // --- SỰ KIỆN CỦA Ô TÌM KIẾM VÀ CHECKBOX ---
        private async void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("txtTimKiem_TextChanged fired.");
            string searchTerm = txtTimKiem?.Text.Trim() ?? "";
            bool includeDeleted = chkHienThiDaXoa?.Checked ?? false; // Lấy trạng thái checkbox
            System.Diagnostics.Debug.WriteLine($"txtTimKiem_TextChanged: Search term='{searchTerm}', includeDeleted={includeDeleted}. Calling LoadDataGrid.");
            await LoadDataGrid(searchTerm, includeDeleted); // Gọi với trạng thái checkbox
            System.Diagnostics.Debug.WriteLine("txtTimKiem_TextChanged finished.");
        }

        // *** THÊM SỰ KIỆN CHO CHECKBOX ***
        private async void chkHienThiDaXoa_CheckedChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("chkHienThiDaXoa_CheckedChanged fired.");
            string searchTerm = txtTimKiem?.Text.Trim() ?? "";
            bool includeDeleted = chkHienThiDaXoa?.Checked ?? false;
            System.Diagnostics.Debug.WriteLine($"chkHienThiDaXoa_CheckedChanged: includeDeleted={includeDeleted}. Calling LoadDataGrid.");
            dgvTacGia?.ClearSelection(); // Bỏ chọn dòng cũ trước khi tải lại
            await LoadDataGrid(searchTerm, includeDeleted); // Tải lại lưới với trạng thái mới
            System.Diagnostics.Debug.WriteLine("chkHienThiDaXoa_CheckedChanged finished.");
        }


        // --- HÀM HỖ TRỢ KHÁC ---
        private bool SelectRowById(int id)
        {
            System.Diagnostics.Debug.WriteLine($"SelectRowById called with ID: {id}");
            if (dgvTacGia == null || dgvTacGia.Rows.Count == 0 || dgvTacGia.DataSource == null)
            {
                System.Diagnostics.Debug.WriteLine("SelectRowById: Grid is null, empty, or has no DataSource.");
                dgvTacGia?.ClearSelection();
                return false;
            }

            bool found = false;
            dgvTacGia.SelectionChanged -= dgvTacGia_SelectionChanged;
            System.Diagnostics.Debug.WriteLine("SelectRowById: Temporarily unsubscribed from SelectionChanged.");

            try
            {
                System.Diagnostics.Debug.WriteLine("SelectRowById: Calling ClearSelection.");
                dgvTacGia.ClearSelection();

                DataGridViewRow? rowToSelect = null;
                foreach (DataGridViewRow row in dgvTacGia.Rows)
                {
                    if (row.DataBoundItem is TacGiaDTO dto && dto.Id == id)
                    {
                        rowToSelect = row;
                        found = true;
                        System.Diagnostics.Debug.WriteLine($"SelectRowById: Found row at index {row.Index} for ID {id}.");
                        break;
                    }
                }

                if (rowToSelect != null)
                {
                    rowToSelect.Selected = true;
                    System.Diagnostics.Debug.WriteLine($"SelectRowById: Row {rowToSelect.Index} selected.");
                    try
                    {
                        if (rowToSelect.Index >= 0 && rowToSelect.Index < dgvTacGia.RowCount && !IsRowDisplayed(rowToSelect.Index))
                        {
                            System.Diagnostics.Debug.WriteLine($"SelectRowById: Scrolling to row {rowToSelect.Index}.");
                            dgvTacGia.FirstDisplayedScrollingRowIndex = rowToSelect.Index;
                        }
                        else { System.Diagnostics.Debug.WriteLine($"SelectRowById: Row {rowToSelect.Index} is already displayed or index invalid."); }
                    }
                    catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Lỗi khi cuộn DGV đến dòng ID {id}: {ex.Message}"); }
                }
                else { System.Diagnostics.Debug.WriteLine($"SelectRowById: Row with ID {id} not found."); }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SelectRowById Try/Catch Error: {ex.Message}");
                found = false;
                if (dgvTacGia != null) dgvTacGia.ClearSelection();
            }
            finally
            {
                if (dgvTacGia != null && !dgvTacGia.IsDisposed)
                {
                    dgvTacGia.SelectionChanged += dgvTacGia_SelectionChanged;
                    System.Diagnostics.Debug.WriteLine("SelectRowById: Re-subscribed to SelectionChanged.");
                }
                else { System.Diagnostics.Debug.WriteLine("SelectRowById Finally: dgvTacGia is null or disposed."); }
            }
            System.Diagnostics.Debug.WriteLine($"SelectRowById finished. Found: {found}.");
            return found;
        }

        private bool IsRowDisplayed(int rowIndex)
        {
            if (dgvTacGia == null || rowIndex < 0 || rowIndex >= dgvTacGia.RowCount) return false;
            try { return dgvTacGia.GetRowDisplayRectangle(rowIndex, true).IntersectsWith(dgvTacGia.ClientRectangle); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"IsRowDisplayed Error for row {rowIndex}: {ex.Message}"); return false; }
        }

        // --- MESSAGE BOX HELPERS ---
        private DialogResult ShowConfirm(string message)
        {
            System.Diagnostics.Debug.WriteLine("ShowConfirm called.");
            try
            {
                Form? parentForm = this.FindForm();
                return parentForm != null
                    ? MaterialMessageBox.Show(parentForm, message, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    : MessageBox.Show(message, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question); // Fallback
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"ShowConfirm Error: {ex.Message}"); return MessageBox.Show(message, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question); }
        }
        private void ShowWarning(string message)
        {
            System.Diagnostics.Debug.WriteLine($"ShowWarning called: {message}");
            try
            {
                Form? parentForm = this.FindForm();
                if (parentForm != null) MaterialMessageBox.Show(parentForm, message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"ShowWarning Error: {ex.Message}"); MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
        private void ShowInfo(string message)
        {
            System.Diagnostics.Debug.WriteLine($"ShowInfo called: {message}");
            try
            {
                Form? parentForm = this.FindForm();
                if (parentForm != null) MaterialMessageBox.Show(parentForm, message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"ShowInfo Error: {ex.Message}"); MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }
        private void ShowError(string message)
        {
            System.Diagnostics.Debug.WriteLine($"ShowError called: {message}");
            try
            {
                Form? parentForm = this.FindForm();
                if (parentForm != null) MaterialMessageBox.Show(parentForm, message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"ShowError Error: {ex.Message}"); MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private string HandleGeneralError(string action, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR ({action}): {ex}");
            return $"Lỗi hệ thống khi {action}: {ex.Message}";
        }

        // --- REQUEST CLOSE ---
        protected virtual void OnRequestClose()
        {
            System.Diagnostics.Debug.WriteLine("ucQuanLyTacGia: OnRequestClose called.");
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

    } // Kết thúc class ucQuanLyTacGia
} // Kết thúc namespace GUI
