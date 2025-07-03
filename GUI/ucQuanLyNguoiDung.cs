// Project/Namespace: GUI
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

namespace GUI
{
    public partial class ucQuanLyNguoiDung : UserControl, IRequiresDataLoading
    {
        private readonly IBUSNguoiDung _busNguoiDung;
        private readonly IBUSNhomNguoiDung _busNhomNguoiDung;
        private readonly IServiceProvider _serviceProvider;

        // --- FIX: Thêm biến trạng thái _isEditing ---
        private bool _isAdding = false;
        private bool _isEditing = false; // Biến mới để theo dõi chế độ chỉnh sửa
        // --- KẾT THÚC FIX ---

        private List<NhomNguoiDungDTO> _nhomNguoiDungList = new List<NhomNguoiDungDTO>();

        /// <summary>
        /// Sự kiện được kích hoạt khi UserControl muốn yêu cầu đóng chính nó.
        /// Form cha (frmMain) sẽ lắng nghe sự kiện này.
        /// </summary>
        public event EventHandler? RequestClose;

        public ucQuanLyNguoiDung(IBUSNguoiDung busNguoiDung, IBUSNhomNguoiDung busNhomNguoiDung, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _busNguoiDung = busNguoiDung ?? throw new ArgumentNullException(nameof(busNguoiDung));
            _busNhomNguoiDung = busNhomNguoiDung ?? throw new ArgumentNullException(nameof(busNhomNguoiDung));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        private void ucQuanLyNguoiDung_Load(object sender, EventArgs e)
        {
            // Không gọi InitializeDataAsync ở đây nữa, frmMain sẽ gọi
        }

        /// <summary>
        /// Phương thức công khai để khởi tạo dữ liệu cho UserControl khi được load bởi frmMain.
        /// </summary>
        public async Task InitializeDataAsync()
        {
            // Không chạy logic trong Design Mode của Visual Studio
            if (!this.DesignMode)
            {
                // --- FIX: Đảm bảo reset cả _isEditing ---
                _isAdding = false;
                _isEditing = false;
                // --- KẾT THÚC FIX ---
                ClearInputFields();
                await LoadNhomNguoiDungComboBox();
                await LoadDataGrid();
                // UpdateUIState() sẽ được gọi trong finally của LoadDataGrid
            }
        }


        private async Task LoadNhomNguoiDungComboBox()
        {
            try
            {
                if (cboNhomNguoiDung == null) return;
                object? selectedValue = cboNhomNguoiDung.SelectedValue; // Lưu giá trị cũ
                _nhomNguoiDungList = await _busNhomNguoiDung.GetAllNhomNguoiDungAsync();
                if (_nhomNguoiDungList == null) { _nhomNguoiDungList = new List<NhomNguoiDungDTO>(); } // Handle null case
                cboNhomNguoiDung.DataSource = null;
                cboNhomNguoiDung.DataSource = _nhomNguoiDungList;
                cboNhomNguoiDung.DisplayMember = "TenNhomNguoiDung";
                cboNhomNguoiDung.ValueMember = "Id";

                // Chọn lại giá trị cũ hoặc bỏ chọn
                if (selectedValue != null && _nhomNguoiDungList.Any(nnd => nnd.Id == (int)selectedValue))
                {
                    cboNhomNguoiDung.SelectedValue = selectedValue;
                }
                else
                {
                    cboNhomNguoiDung.SelectedIndex = -1;
                }
            }
            catch (Exception ex) { HandleError("tải nhóm người dùng", ex); }
        }

        private async Task LoadDataGrid()
        {
            int? selectedId = null;
            if (dgvNguoiDung?.SelectedRows.Count > 0 && dgvNguoiDung.SelectedRows[0].DataBoundItem is NguoiDungDTO currentDto)
                selectedId = currentDto.Id;

            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (dgvNguoiDung == null) return;
                dgvNguoiDung.DataSource = null;
                List<NguoiDungDTO> danhSach = await _busNguoiDung.GetAllNguoiDungAsync();
                if (danhSach == null) { danhSach = new List<NguoiDungDTO>(); } // Handle null case
                dgvNguoiDung.DataSource = danhSach.ToList();
                SetupDataGridViewColumns();
                if (selectedId.HasValue) SelectRowById(selectedId.Value);
                else dgvNguoiDung.ClearSelection();
            }
            catch (Exception ex) { HandleError("tải danh sách người dùng", ex); }
            finally
            {
                this.Cursor = Cursors.Default;
                UpdateUIState(); // Cập nhật trạng thái UI sau khi tải xong
            }
        }

        private void SetupDataGridViewColumns()
        {
            if (dgvNguoiDung == null || dgvNguoiDung.DataSource == null || dgvNguoiDung.Columns.Count == 0) return;
            try
            {
                var columns = dgvNguoiDung.Columns;
                if (columns.Contains("Id")) columns["Id"].Visible = false;
                if (columns.Contains("IdNhomNguoiDung")) columns["IdNhomNguoiDung"].Visible = false;
                if (columns.Contains("MaNguoiDung")) { columns["MaNguoiDung"].HeaderText = "Mã User"; columns["MaNguoiDung"].Width = 80; }
                if (columns.Contains("TenDangNhap")) { columns["TenDangNhap"].HeaderText = "Tên Đăng Nhập"; columns["TenDangNhap"].Width = 120; }
                if (columns.Contains("TenHienThi")) { columns["TenHienThi"].HeaderText = "Tên Hiển Thị"; columns["TenHienThi"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; }
                if (columns.Contains("TenNhomNguoiDung")) { columns["TenNhomNguoiDung"].HeaderText = "Vai Trò"; columns["TenNhomNguoiDung"].Width = 100; }
                if (columns.Contains("NgaySinh")) { columns["NgaySinh"].HeaderText = "Ngày Sinh"; columns["NgaySinh"].Width = 100; columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy"; }
                if (columns.Contains("ChucVu")) { columns["ChucVu"].HeaderText = "Chức Vụ"; columns["ChucVu"].Width = 120; }

                dgvNguoiDung.ReadOnly = true;
                dgvNguoiDung.AllowUserToAddRows = false;
                dgvNguoiDung.AllowUserToDeleteRows = false;
                dgvNguoiDung.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvNguoiDung.MultiSelect = false;
            }
            catch (Exception ex) { Debug.WriteLine($"Lỗi cấu hình cột DGV Người dùng: {ex.Message}"); }
        }

        // --- FIX: Loại bỏ phương thức IsEditing() không cần thiết và có vấn đề ---
        // private bool IsEditing() => btnLuu != null && btnLuu.Enabled && !_isAdding; 
        // --- KẾT THÚC FIX ---

        private void ClearInputFields()
        {
            if (txtId != null) txtId.Clear();
            if (txtMaNguoiDung != null) txtMaNguoiDung.Clear();
            if (txtTenDangNhap != null) txtTenDangNhap.Clear();
            if (txtMatKhau != null) txtMatKhau.Clear();
            if (txtTenHienThi != null) txtTenHienThi.Clear();
            if (dtpNgaySinh != null) dtpNgaySinh.Value = DateTime.Now; // Or a sensible default
            if (txtChucVu != null) txtChucVu.Clear();
            if (cboNhomNguoiDung != null) cboNhomNguoiDung.SelectedIndex = -1;

            // Focus
            if (_isAdding)
            {
                if (txtMaNguoiDung != null && txtMaNguoiDung.Enabled) txtMaNguoiDung.Focus();
                else if (txtTenDangNhap != null && txtTenDangNhap.Enabled) txtTenDangNhap.Focus();
            }
            else if (_isEditing) // --- FIX: Focus khi đang chỉnh sửa ---
            {
                if (txtTenHienThi != null && txtTenHienThi.Enabled) txtTenHienThi.Focus();
            }
            // --- KẾT THÚC FIX ---
        }

        private void UpdateUIState()
        {
            bool rowSelected = dgvNguoiDung?.SelectedRows.Count > 0;
            // --- FIX: Sử dụng _isAdding và _isEditing trực tiếp ---
            bool isAddingOrEditing = _isAdding || _isEditing;
            // --- KẾT THÚC FIX ---

            if (btnThem != null) btnThem.Enabled = !isAddingOrEditing;
            if (btnSua != null) btnSua.Enabled = !isAddingOrEditing && rowSelected;
            if (btnXoa != null) btnXoa.Enabled = !isAddingOrEditing && rowSelected;
            if (btnKhoiPhuc != null) btnKhoiPhuc.Enabled = false; // Vẫn giữ logic cũ cho Khôi phục
            if (btnLuu != null) btnLuu.Enabled = isAddingOrEditing;
            if (btnBoQua != null) btnBoQua.Enabled = isAddingOrEditing;
            if (btnThoat != null) btnThoat.Enabled = true;

            // Kích hoạt/Vô hiệu hóa các trường nhập liệu
            if (txtId != null) txtId.Enabled = false; // ID luôn không cho sửa
            if (txtMaNguoiDung != null) txtMaNguoiDung.Enabled = _isAdding; // Chỉ cho nhập Mã khi Thêm
            if (txtTenDangNhap != null) txtTenDangNhap.Enabled = _isAdding; // Chỉ cho nhập Tên đăng nhập khi Thêm
            if (txtMatKhau != null) txtMatKhau.Enabled = _isAdding; // Chỉ cho nhập Mật khẩu khi Thêm
            if (txtTenHienThi != null) txtTenHienThi.Enabled = isAddingOrEditing;
            if (dtpNgaySinh != null) dtpNgaySinh.Enabled = isAddingOrEditing;
            if (txtChucVu != null) txtChucVu.Enabled = isAddingOrEditing;
            if (cboNhomNguoiDung != null) cboNhomNguoiDung.Enabled = isAddingOrEditing;

            // Kích hoạt/Vô hiệu hóa DataGridView
            if (dgvNguoiDung != null) dgvNguoiDung.Enabled = !isAddingOrEditing;

            // Cập nhật form chi tiết dựa trên dòng được chọn (chỉ khi không thêm/sửa)
            if (!isAddingOrEditing)
            {
                DisplaySelectedRow(); // Hiển thị dữ liệu hoặc xóa trắng form
            }
        }

        private void DisplaySelectedRow()
        {
            // Không cần kiểm tra isAdding/isEditing nữa vì UpdateUIState đã làm
            int id = 0; string maNguoiDung = "", tenDangNhap = "", tenHienThi = "", chucVu = "";
            DateTime? ngaySinh = null; int? nhomNguoiDungId = null;
            bool rowSelected = false;

            if (dgvNguoiDung?.SelectedRows.Count == 1 && dgvNguoiDung.SelectedRows[0].DataBoundItem is NguoiDungDTO dto)
            {
                id = dto.Id; maNguoiDung = dto.MaNguoiDung ?? ""; tenDangNhap = dto.TenDangNhap ?? "";
                tenHienThi = dto.TenHienThi ?? ""; ngaySinh = dto.NgaySinh; chucVu = dto.ChucVu ?? "";
                nhomNguoiDungId = dto.IdNhomNguoiDung;
                rowSelected = true;
            }

            // Gán giá trị vào các controls
            if (txtId != null) txtId.Text = id > 0 ? id.ToString() : "";
            if (txtMaNguoiDung != null) txtMaNguoiDung.Text = maNguoiDung;
            if (txtTenDangNhap != null) txtTenDangNhap.Text = tenDangNhap;
            if (txtMatKhau != null) txtMatKhau.Clear(); // Không hiển thị mật khẩu cũ
            if (txtTenHienThi != null) txtTenHienThi.Text = tenHienThi;
            if (dtpNgaySinh != null) dtpNgaySinh.Value = (ngaySinh.HasValue && ngaySinh.Value >= dtpNgaySinh.MinDate && ngaySinh.Value <= dtpNgaySinh.MaxDate) ? ngaySinh.Value : DateTime.Now;
            if (txtChucVu != null) txtChucVu.Text = chucVu;
            if (cboNhomNguoiDung != null)
            {
                // Kiểm tra ID có tồn tại trong danh sách hiện tại không
                if (nhomNguoiDungId.HasValue && _nhomNguoiDungList.Any(nnd => nnd.Id == nhomNguoiDungId))
                {
                    cboNhomNguoiDung.SelectedValue = (object)nhomNguoiDungId.Value;
                }
                else
                {
                    cboNhomNguoiDung.SelectedIndex = -1; // Hoặc thử tải lại nếu cần
                }
            }

            // Nếu không có dòng nào được chọn, xóa trắng các trường
            if (!rowSelected) { ClearInputFields(); }
            // Trạng thái nút được xử lý bởi UpdateUIState đã gọi phương thức này hoặc sẽ được gọi bởi SelectionChanged
        }

        private void SelectRowById(int id)
        {
            if (dgvNguoiDung == null || dgvNguoiDung.Rows.Count == 0 || dgvNguoiDung.DataSource == null) { dgvNguoiDung?.ClearSelection(); return; }
            dgvNguoiDung.ClearSelection();
            bool found = false;
            foreach (DataGridViewRow row in dgvNguoiDung.Rows)
            {
                if (row.DataBoundItem is NguoiDungDTO dto && dto.Id == id)
                {
                    row.Selected = true;
                    found = true;
                    try { if (row.Index >= 0 && row.Index < dgvNguoiDung.RowCount) dgvNguoiDung.FirstDisplayedScrollingRowIndex = row.Index; } catch { }
                    break;
                }
            }
            if (!found) { Debug.WriteLine($"SelectRowById - Row {id} not found."); }
            // UpdateUIState sẽ được gọi bởi sự kiện SelectionChanged khi ClearSelection hoàn tất (nếu lựa chọn thay đổi)
            // Hoặc nếu lựa chọn không đổi (đã trống), UpdateUIState đã được gọi bởi khối finally của LoadDataGrid.
        }

        // --- BUTTON EVENT HANDLERS ---

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (HasUnsavedChanges()) // Kiểm tra thay đổi chưa lưu trước khi chuyển chế độ
            {
                if (ShowConfirm("Dữ liệu chưa được lưu. Bạn có muốn tiếp tục Thêm mới không?") == DialogResult.No) return;
            }
            // --- FIX: Cập nhật trạng thái ---
            _isAdding = true;
            _isEditing = false;
            // --- KẾT THÚC FIX ---
            ClearInputFields();
            dgvNguoiDung?.ClearSelection(); // Bỏ chọn dòng hiện tại trên lưới
            UpdateUIState(); // Cập nhật giao diện cho chế độ Thêm
            // Focus vào trường nhập liệu đầu tiên có thể sửa
            if (txtMaNguoiDung != null && txtMaNguoiDung.Enabled) txtMaNguoiDung.Focus();
            else if (txtTenDangNhap != null && txtTenDangNhap.Enabled) txtTenDangNhap.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvNguoiDung?.SelectedRows.Count == 1 && !_isAdding && !_isEditing) // Chỉ cho sửa khi đang xem và có 1 dòng được chọn
            {
                if (HasUnsavedChanges()) // Kiểm tra thay đổi chưa lưu (dù không nên xảy ra ở trạng thái này)
                {
                    if (ShowConfirm("Dữ liệu có vẻ đã thay đổi. Bạn có muốn tiếp tục Sửa không?") == DialogResult.No) return;
                }
                // --- FIX: Cập nhật trạng thái ---
                _isAdding = false;
                _isEditing = true;
                // --- KẾT THÚC FIX ---
                UpdateUIState(); // Cập nhật giao diện cho chế độ Sửa
                // Focus vào trường nhập liệu đầu tiên có thể sửa (thường là Tên hiển thị)
                if (txtTenHienThi != null && txtTenHienThi.Enabled) txtTenHienThi.Focus();
            }
            else if (dgvNguoiDung?.SelectedRows.Count == 0) { ShowWarning("Vui lòng chọn người dùng cần sửa."); }
            else if (dgvNguoiDung?.SelectedRows.Count > 1) { ShowWarning("Chỉ có thể sửa một người dùng mỗi lần."); }
            // Trường hợp đang Thêm (_isAdding = true) hoặc đang Sửa (_isEditing = true), nút Sửa đã bị vô hiệu hóa bởi UpdateUIState
        }

        private async void btnLuu_Click(object sender, EventArgs e)
        {
            int currentId = 0;
            if (_isEditing) // Chỉ cần lấy ID khi đang sửa
            {
                if (!int.TryParse(txtId?.Text, out currentId) || currentId <= 0) { ShowError("Không xác định được ID người dùng để cập nhật."); return; }
            }

            // Lấy dữ liệu từ các trường nhập liệu
            string? maNguoiDung = txtMaNguoiDung?.Text.Trim(); // Chỉ dùng khi thêm
            string tenDangNhap = txtTenDangNhap?.Text.Trim() ?? ""; // Chỉ dùng khi thêm
            string matKhau = txtMatKhau?.Text ?? ""; // Chỉ dùng khi thêm
            string tenHienThi = txtTenHienThi?.Text.Trim() ?? "";
            DateTime? ngaySinh = dtpNgaySinh?.Value;
            string? chucVu = txtChucVu?.Text.Trim();
            int? idNhomNguoiDung = (cboNhomNguoiDung?.SelectedValue as int?);

            // --- VALIDATION ---
            if (string.IsNullOrWhiteSpace(tenHienThi)) { ShowError("Tên hiển thị không được rỗng."); txtTenHienThi?.Focus(); return; }
            if (_isAdding && string.IsNullOrWhiteSpace(tenDangNhap)) { ShowError("Tên đăng nhập không được rỗng."); txtTenDangNhap?.Focus(); return; }
            if (_isAdding && string.IsNullOrWhiteSpace(matKhau)) { ShowError("Mật khẩu không được rỗng khi thêm mới."); txtMatKhau?.Focus(); return; }
            // Thêm kiểm tra Mã người dùng nếu đang thêm mới và trường này bắt buộc
            if (_isAdding && string.IsNullOrWhiteSpace(maNguoiDung)) { ShowError("Mã người dùng không được rỗng khi thêm mới."); txtMaNguoiDung?.Focus(); return; }
            if (!idNhomNguoiDung.HasValue || idNhomNguoiDung <= 0) { ShowError("Vui lòng chọn vai trò/nhóm người dùng."); cboNhomNguoiDung?.Focus(); return; }
            // --- KẾT THÚC VALIDATION ---

            NguoiDungDTO nguoiDungDto = new NguoiDungDTO
            {
                Id = currentId, // Sẽ là 0 nếu đang thêm mới
                MaNguoiDung = string.IsNullOrWhiteSpace(maNguoiDung) ? null : maNguoiDung, // Chỉ gán khi thêm
                TenDangNhap = tenDangNhap, // Chỉ gán khi thêm
                TenHienThi = tenHienThi,
                NgaySinh = ngaySinh,
                ChucVu = string.IsNullOrWhiteSpace(chucVu) ? null : chucVu,
                IdNhomNguoiDung = idNhomNguoiDung
            };

            bool success = false; string? errorMsg = null; NguoiDungDTO? savedDto = null;
            if (btnLuu != null) btnLuu.Enabled = false; // Vô hiệu hóa tạm thời nút Lưu/Bỏ qua
            if (btnBoQua != null) btnBoQua.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (_isAdding)
                {
                    // Gọi BUS để thêm, truyền cả mật khẩu
                    var addedDto = await _busNguoiDung.AddNguoiDungAsync(nguoiDungDto, matKhau);
                    success = addedDto != null;
                    if (success) savedDto = addedDto; // Lưu lại DTO đã thêm thành công
                }
                else // Đang chỉnh sửa (_isEditing = true)
                {
                    // Lấy thông tin gốc để đảm bảo không sửa Tên đăng nhập và Mã người dùng
                    NguoiDungDTO? originalDto = null;
                    if (dgvNguoiDung?.SelectedRows.Count == 1)
                        originalDto = dgvNguoiDung.SelectedRows[0].DataBoundItem as NguoiDungDTO;

                    if (originalDto != null)
                    {
                        nguoiDungDto.TenDangNhap = originalDto.TenDangNhap; // Giữ nguyên Tên đăng nhập
                        nguoiDungDto.MaNguoiDung = originalDto.MaNguoiDung;   // Giữ nguyên Mã người dùng
                    }
                    else
                    {
                        throw new InvalidOperationException("Không tìm thấy dữ liệu gốc để cập nhật.");
                    }

                    // Gọi BUS để cập nhật (không cần truyền mật khẩu)
                    success = await _busNguoiDung.UpdateNguoiDungAsync(nguoiDungDto);
                    if (success) savedDto = nguoiDungDto; // Lưu lại DTO đã gửi để cập nhật
                }
            }
            catch (InvalidOperationException invOpEx) { errorMsg = $"Lỗi nghiệp vụ: {invOpEx.Message}"; Debug.WriteLine($"BUS Error: {invOpEx}"); }
            catch (ArgumentException argEx) { errorMsg = $"Lỗi dữ liệu: {argEx.Message}"; Debug.WriteLine($"Validation Error: {argEx}"); }
            catch (Exception ex) { errorMsg = $"Lỗi hệ thống: {ex.Message}"; Debug.WriteLine($"System Error: {ex}"); }
            finally { this.Cursor = Cursors.Default; /* UpdateUIState sẽ được gọi sau LoadDataGrid hoặc ở khối else */ }

            if (success)
            {
                ShowInfo(_isAdding ? "Thêm người dùng thành công!" : "Cập nhật người dùng thành công!");
                // --- FIX: Reset trạng thái sau khi Lưu thành công ---
                _isAdding = false;
                _isEditing = false;
                // --- KẾT THÚC FIX ---
                await LoadDataGrid(); // Tải lại lưới, sẽ tự động gọi UpdateUIState
                if (savedDto != null && savedDto.Id > 0) SelectRowById(savedDto.Id); // Chọn lại dòng vừa lưu
            }
            else
            {
                ShowError(errorMsg ?? (_isAdding ? "Thêm thất bại." : "Cập nhật thất bại."));
                // --- FIX: Kích hoạt lại nút Lưu/Bỏ qua nếu Lưu thất bại ---
                UpdateUIState(); // Cập nhật lại UI để kích hoạt lại nút Lưu/Bỏ qua
                                 // --- KẾT THÚC FIX ---
            }
        }

        private void btnBoQua_Click(object sender, EventArgs e)
        {
            if (HasUnsavedChanges()) // Kiểm tra thay đổi chưa lưu trước khi bỏ qua
            {
                if (ShowConfirm("Dữ liệu chưa được lưu. Bạn có muốn hủy bỏ thay đổi không?") == DialogResult.No) return;
            }

            // --- FIX: Reset trạng thái ---
            _isAdding = false;
            _isEditing = false;
            // --- KẾT THÚC FIX ---

            // Không cần ClearInputFields vì DisplaySelectedRow sẽ làm việc đó nếu không có dòng nào được chọn
            // hoặc hiển thị lại dữ liệu của dòng đang chọn.
            UpdateUIState(); // Quay lại chế độ xem, hiển thị dòng đang chọn hoặc xóa trắng form
        }

        private async void btnXoa_Click(object sender, EventArgs e)
        {
            // Chỉ cho phép xóa khi đang ở chế độ xem và có một dòng được chọn
            if (dgvNguoiDung?.SelectedRows.Count == 1 && !_isAdding && !_isEditing)
            {
                if (dgvNguoiDung.SelectedRows[0].DataBoundItem is NguoiDungDTO dto)
                {
                    int idToDelete = dto.Id;
                    string tenNguoiDung = dto.TenHienThi ?? dto.TenDangNhap ?? "?"; // Ưu tiên Tên hiển thị
                    if (idToDelete <= 0) return; // Không có ID hợp lệ

                    // Có thể thêm kiểm tra không cho xóa chính mình ở đây nếu cần
                    // Ví dụ: if (dto.Id == _currentlyLoggedInUserId) { ShowWarning("Không thể xóa tài khoản đang đăng nhập."); return; }

                    if (ShowConfirm($"Bạn có chắc chắn muốn XÓA VĨNH VIỄN người dùng '{tenNguoiDung}' không?") == DialogResult.Yes)
                    {
                        // Gọi hàm thực hiện thao tác DB (đã có sẵn)
                        await PerformDatabaseOperation(async () => await _busNguoiDung.HardDeleteNguoiDungAsync(idToDelete), "Xóa vĩnh viễn người dùng");
                        // LoadDataGrid sẽ được gọi bên trong PerformDatabaseOperation nếu thành công
                    }
                }
                else { ShowError("Không thể lấy thông tin người dùng để xóa."); }
            }
            else if (dgvNguoiDung?.SelectedRows.Count == 0) { ShowWarning("Vui lòng chọn người dùng cần xóa."); }
            // Trường hợp đang Thêm/Sửa, nút Xóa đã bị vô hiệu hóa
        }

        private void btnKhoiPhuc_Click(object sender, EventArgs e) { /* Logic khôi phục (nếu có) sẽ ở đây */ }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            // Kiểm tra thay đổi chưa lưu trước khi thoát
            if (HasUnsavedChanges() && (_isAdding || _isEditing))
            {
                if (ShowConfirm("Dữ liệu chưa được lưu. Bạn có muốn thoát không?") == DialogResult.No) return;
            }
            // --- FIX: Reset trạng thái trước khi thoát ---
            _isAdding = false;
            _isEditing = false;
            // --- KẾT THÚC FIX ---
            // Kích hoạt sự kiện để form cha xử lý việc đóng UserControl
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        private void dgvNguoiDung_SelectionChanged(object sender, EventArgs e)
        {
            // Chỉ cập nhật UI nếu không đang trong quá trình thêm/sửa
            // Vì khi thêm/sửa, việc chọn dòng không nên thay đổi trạng thái nút chính
            if (!_isAdding && !_isEditing)
            {
                UpdateUIState(); // Cập nhật UI dựa trên lựa chọn mới (hoặc không có lựa chọn)
            }
        }

        private void dgvNguoiDung_DoubleClick(object sender, EventArgs e)
        {
            // Nếu đang ở chế độ xem và có 1 dòng được chọn, hành động DoubleClick tương đương nhấn nút Sửa
            if (dgvNguoiDung?.SelectedRows.Count == 1 && !_isAdding && !_isEditing)
            {
                btnSua_Click(sender, e); // Gọi lại logic của nút Sửa
            }
        }

        // --- HELPER METHODS ---
        private void HandleError(string action, Exception ex) { MaterialMessageBox.Show(this.FindForm(), $"Lỗi khi {action}: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); Debug.WriteLine($"ERROR {action}: {ex}"); }
        private void ShowError(string message) => MaterialMessageBox.Show(this.FindForm(), message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void ShowWarning(string message) => MaterialMessageBox.Show(this.FindForm(), message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void ShowInfo(string message) => MaterialMessageBox.Show(this.FindForm(), message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private DialogResult ShowConfirm(string message) => MaterialMessageBox.Show(this.FindForm(), message, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        /// <summary>
        /// Thực hiện một thao tác cơ sở dữ liệu (Thêm/Sửa/Xóa) và xử lý kết quả, lỗi, con trỏ chuột, trạng thái nút.
        /// </summary>
        /// <param name="operation">Hàm async thực hiện thao tác DB, trả về true nếu thành công.</param>
        /// <param name="actionName">Tên hành động (ví dụ: "Thêm", "Xóa") để hiển thị thông báo.</param>
        private async Task PerformDatabaseOperation(Func<Task<bool>> operation, string actionName)
        {
            bool success = false; string? errorMsg = null;
            this.Cursor = Cursors.WaitCursor;
            ToggleButtonState(false); // Vô hiệu hóa các nút chính tạm thời
            try
            {
                success = await operation();
            }
            catch (InvalidOperationException ex) { errorMsg = $"Lỗi nghiệp vụ ({actionName}): {ex.Message}"; Debug.WriteLine($"BUS Error ({actionName}): {ex}"); }
            catch (ArgumentException ex) { errorMsg = $"Lỗi dữ liệu ({actionName}): {ex.Message}"; Debug.WriteLine($"Validation Error ({actionName}): {ex}"); }
            catch (Exception ex) { errorMsg = $"Lỗi hệ thống ({actionName}): {ex.Message}"; Debug.WriteLine($"System Error ({actionName}): {ex}"); }
            finally
            {
                this.Cursor = Cursors.Default;
                // Không gọi ToggleButtonState(true) ở đây vì LoadDataGrid/UpdateUIState sẽ xử lý trạng thái cuối cùng
            }

            if (success)
            {
                ShowInfo($"{actionName} thành công!");
                // --- FIX: Reset trạng thái sau thao tác DB thành công (Xóa) ---
                _isAdding = false;
                _isEditing = false;
                // --- KẾT THÚC FIX ---
                await LoadDataGrid(); // Tải lại lưới, sẽ tự động gọi UpdateUIState và đặt lại trạng thái nút
            }
            else
            {
                ShowError(errorMsg ?? $"{actionName} thất bại.");
                UpdateUIState(); // Đảm bảo các nút được cập nhật đúng trạng thái sau lỗi (ví dụ: kích hoạt lại Lưu/Bỏ qua nếu đang sửa mà lỗi)
            }
        }

        /// <summary>
        /// Tạm thời vô hiệu hóa/kích hoạt các nút thao tác chính (Thêm, Sửa, Xóa) trong khi thực hiện thao tác DB.
        /// </summary>
        /// <param name="enabled">True để kích hoạt, False để vô hiệu hóa.</param>
        private void ToggleButtonState(bool enabled)
        {
            // Chỉ kích hoạt lại nếu thao tác đã xong VÀ không đang ở chế độ thêm/sửa
            bool canInteract = enabled && !_isAdding && !_isEditing;
            bool rowSelected = dgvNguoiDung?.SelectedRows.Count > 0;

            if (btnThem != null) btnThem.Enabled = canInteract;
            if (btnSua != null) btnSua.Enabled = canInteract && rowSelected;
            if (btnXoa != null) btnXoa.Enabled = canInteract && rowSelected;
            if (btnKhoiPhuc != null) btnKhoiPhuc.Enabled = false; // Vẫn giữ là false
            // Nút Lưu, Bỏ qua sẽ được quản lý bởi UpdateUIState dựa trên _isAdding/_isEditing
        }

        /// <summary>
        /// Kiểm tra xem có thay đổi chưa được lưu trong các trường nhập liệu hay không.
        /// </summary>
        /// <returns>True nếu có thay đổi chưa lưu, False nếu không.</returns>
        private bool HasUnsavedChanges()
        {
            // Nếu không ở chế độ thêm hoặc sửa, không có gì để kiểm tra
            if (!_isAdding && !_isEditing) return false;

            // Nếu đang thêm mới, kiểm tra xem có trường nào đã được nhập chưa
            if (_isAdding)
            {
                return !string.IsNullOrWhiteSpace(txtMaNguoiDung?.Text) ||
                       !string.IsNullOrWhiteSpace(txtTenDangNhap?.Text) ||
                       !string.IsNullOrWhiteSpace(txtMatKhau?.Text) || // Kiểm tra cả mật khẩu
                       !string.IsNullOrWhiteSpace(txtTenHienThi?.Text) ||
                       (dtpNgaySinh?.Value.Date != DateTime.Now.Date) || // Kiểm tra nếu ngày sinh khác ngày hiện tại
                       !string.IsNullOrWhiteSpace(txtChucVu?.Text) ||
                       cboNhomNguoiDung?.SelectedIndex != -1;
            }
            else // Nếu đang chỉnh sửa, so sánh giá trị hiện tại với dữ liệu gốc của dòng đang chọn
            {
                if (dgvNguoiDung?.SelectedRows.Count == 1 && dgvNguoiDung.SelectedRows[0].DataBoundItem is NguoiDungDTO originalDto)
                {
                    // So sánh từng trường có thể sửa đổi
                    return (txtTenHienThi?.Text.Trim() != (originalDto.TenHienThi ?? "")) ||
                           (dtpNgaySinh?.Value.Date != (originalDto.NgaySinh?.Date ?? DateTime.MinValue.Date)) || // So sánh phần Date
                           (txtChucVu?.Text.Trim() != (originalDto.ChucVu ?? "")) ||
                           ((cboNhomNguoiDung?.SelectedValue as int?) != originalDto.IdNhomNguoiDung);
                    // Không cần so sánh Mã, Tên đăng nhập, Mật khẩu vì không cho sửa
                }
                return false; // Không thể so sánh nếu không có dữ liệu gốc
            }
        }
    }
}
