// File: GUI/ucQuanLyCuonSach.cs
// Project/Namespace: GUI

using BUS;
using DTO;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection; // If using DI for forms/controls

namespace GUI
{
    /// <summary>
    /// UserControl quản lý thông tin các Cuốn Sách cụ thể (bản copy vật lý).
    /// Hiển thị danh sách, cho phép thêm mới và cập nhật tình trạng sách.
    /// </summary>
    public partial class ucQuanLyCuonSach : UserControl, IRequiresDataLoading // <-- Thêm vào đây
    {
        // --- DEPENDENCIES ---
        private readonly IBUSCuonSach _busCuonSach;
        private readonly IBUSSach _busSach;
        // private readonly IServiceProvider _serviceProvider; // Inject if needed

        // --- STATE ---
        private bool _isAdding = false; // Trạng thái Thêm mới
        private List<SachDTO> _sachList = new List<SachDTO>(); // DS Sách (ấn bản) cho ComboBox

        // --- CONSTANTS for TinhTrang (int) ---
        // Đảm bảo các giá trị này khớp với quy định trong hệ thống của bạn (BUS/DAL/DB)
        private const int TINH_TRANG_ID_SAN_CO = 0;
        private const int TINH_TRANG_ID_DA_CHO_MUON = 1;
        private const int TINH_TRANG_ID_BI_MAT = 2;
        private const int TINH_TRANG_ID_HU_HONG = 3;
        // Add other statuses if needed

        /// <summary>
        /// Sự kiện yêu cầu đóng UserControl.
        /// </summary>
        public event EventHandler? RequestClose;

        // --- CONSTRUCTOR ---
        // Modify constructor if you use DI for forms/controls via IServiceProvider
        public ucQuanLyCuonSach(IBUSCuonSach busCuonSach, IBUSSach busSach)
        {
            InitializeComponent();
            _busCuonSach = busCuonSach ?? throw new ArgumentNullException(nameof(busCuonSach));
            _busSach = busSach ?? throw new ArgumentNullException(nameof(busSach));
            // _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        // --- SỰ KIỆN LOAD USERCONTROL ---
        private void ucQuanLyCuonSach_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                // frmMain should call InitializeDataAsync()
            }
        }

        // --- PHƯƠNG THỨC KHỞI TẠO DỮ LIỆU ---
        public async Task InitializeDataAsync()
        {
            if (!this.DesignMode)
            {
                _isAdding = false;
                ClearInputFields(); // Gọi ClearInputFields
                await LoadSachComboBox();
                await LoadDataGrid();
                // SetControlState will be called from LoadDataGrid's finally block
            }
        }

        // --- HÀM TẢI DỮ LIỆU & CÀI ĐẶT GIAO DIỆN ---

        /// <summary>
        /// Tải danh sách Sách (ấn bản) vào ComboBox.
        /// </summary>
        private async Task LoadSachComboBox()
        {
            try
            {
                if (cboSach == null) return;

                object? selectedValue = cboSach.SelectedValue;

                _sachList = await _busSach.GetAllSachAsync(); // Lấy danh sách SachDTO

                if (_sachList == null)
                {
                    System.Diagnostics.Debug.WriteLine("WARNING: _busSach.GetAllSachAsync() returned null.");
                    _sachList = new List<SachDTO>();
                }

                // Tạo một danh sách hiển thị mô tả hơn cho ComboBox
                var displayList = _sachList.Select(s => new
                {
                    Id = s.Id,
                    DisplayText = $"{s.MaSach ?? "N/A"} - {s.TenTuaSach ?? "Không có tựa"} ({s.NamXb})" // Customize display text
                }).ToList();


                cboSach.DataSource = null;
                cboSach.DataSource = displayList;
                cboSach.DisplayMember = "DisplayText";
                cboSach.ValueMember = "Id";

                if (selectedValue != null && _sachList.Any(s => s.Id == (int)selectedValue))
                {
                    cboSach.SelectedValue = selectedValue;
                }
                else
                {
                    cboSach.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show(this.FindForm(), $"Lỗi khi tải danh sách ấn bản sách: {ex.Message}", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"ERROR (ucQuanLyCuonSach - LoadSachComboBox): {ex.ToString()}");
                if (cboSach != null) cboSach.DataSource = null;
            }
        }

        /// <summary>
        /// Tải danh sách Cuốn Sách vào DataGridView.
        /// </summary>
        private async Task LoadDataGrid()
        {
            int? selectedId = null;
            if (dgvCuonSach != null && dgvCuonSach.SelectedRows.Count > 0 && dgvCuonSach.SelectedRows[0].DataBoundItem != null)
            {
                // Cố gắng lấy ID từ item đang chọn
                try
                {
                    object currentItem = dgvCuonSach.SelectedRows[0].DataBoundItem;
                    selectedId = (int?)currentItem.GetType().GetProperty("Id")?.GetValue(currentItem, null);
                }
                catch
                {
                    selectedId = null; // Không lấy được ID
                }
            }

            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (dgvCuonSach == null) return;

                dgvCuonSach.DataSource = null;

                //*** CẦN ĐẢM BẢO BUS TRẢ VỀ DTO CÓ TinhTrangText ***
                List<CuonSachDTO> danhSachCuonSach = await _busCuonSach.GetAllCuonSachAsync(); // Gọi BUS

                if (danhSachCuonSach == null)
                {
                    System.Diagnostics.Debug.WriteLine("WARNING: _busCuonSach.GetAllCuonSachAsync() returned null.");
                    danhSachCuonSach = new List<CuonSachDTO>();
                }

                // Tạo datasource với thông tin bổ sung nếu cần (Ví dụ lấy từ SachList đã load)
                // BUS layer nên là nơi xử lý việc này để trả về DTO hoàn chỉnh
                var dataSource = danhSachCuonSach.Select(cs => {
                    var sachInfo = _sachList.FirstOrDefault(s => s.Id == cs.IdSach);
                    return new
                    {
                        cs.Id,
                        cs.MaCuonSach,
                        MaSachAnBan = sachInfo?.MaSach ?? "N/A",
                        TenTuaSach = sachInfo?.TenTuaSach ?? "Không rõ",
                        cs.TinhTrang, // Giữ lại TinhTrang int cho logic
                        cs.TinhTrangText, // Dùng TinhTrangText để hiển thị
                        IdSach = cs.IdSach
                    };
                }).ToList();


                dgvCuonSach.DataSource = dataSource;

                SetupDataGridViewColumns();

                if (selectedId.HasValue)
                {
                    SelectRowById(selectedId.Value);
                }
                else
                {
                    dgvCuonSach.ClearSelection();
                    // ClearInputFields(); // Sẽ được gọi trong finally nếu cần
                }
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show(this.FindForm(), $"Lỗi khi tải danh sách cuốn sách: {ex.Message}", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"ERROR (ucQuanLyCuonSach - LoadDataGrid): {ex.ToString()}");
            }
            finally
            {
                this.Cursor = Cursors.Default;
                bool rowSelectedAfterLoad = dgvCuonSach?.SelectedRows.Count > 0;
                SetControlState(_isAdding, rowSelectedAfterLoad);

                if (!rowSelectedAfterLoad && !_isAdding)
                {
                    ClearInputFields(); // Gọi ClearInputFields nếu không có dòng chọn và không đang thêm
                }
                // SelectionChanged sẽ gọi DisplaySelectedRow nếu có dòng chọn lại
            }
        }

        /// <summary>
        /// Cấu hình các cột cho DataGridView.
        /// </summary>
        private void SetupDataGridViewColumns()
        {
            if (dgvCuonSach == null || dgvCuonSach.DataSource == null || dgvCuonSach.Columns.Count == 0) return;

            try
            {
                var columns = dgvCuonSach.Columns;

                // Ẩn các cột không cần thiết
                if (columns.Contains("Id")) columns["Id"].Visible = false;
                if (columns.Contains("IdSach")) columns["IdSach"].Visible = false;
                if (columns.Contains("TinhTrang")) columns["TinhTrang"].Visible = false; // Ẩn cột TinhTrang (int)

                // Cấu hình cột hiển thị
                if (columns.Contains("MaCuonSach")) { columns["MaCuonSach"].HeaderText = "Mã Cuốn Sách"; columns["MaCuonSach"].Width = 150; }
                if (columns.Contains("MaSachAnBan")) { columns["MaSachAnBan"].HeaderText = "Mã Sách (Ấn bản)"; columns["MaSachAnBan"].Width = 150; }
                if (columns.Contains("TenTuaSach")) { columns["TenTuaSach"].HeaderText = "Tên Tựa Sách"; columns["TenTuaSach"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; }
                // Hiển thị cột TinhTrangText
                if (columns.Contains("TinhTrangText")) { columns["TinhTrangText"].HeaderText = "Tình Trạng"; columns["TinhTrangText"].Width = 120; }


                dgvCuonSach.ReadOnly = true;
                dgvCuonSach.AllowUserToAddRows = false;
                dgvCuonSach.AllowUserToDeleteRows = false;
                dgvCuonSach.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvCuonSach.MultiSelect = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi cấu hình cột DataGridView Cuốn Sách: {ex.Message}");
            }
        }

        // --- HÀM TIỆN ÍCH TRẠNG THÁI & GIAO DIỆN ---

        /// <summary>
        /// Xóa trắng nội dung của các ô nhập liệu.
        /// **ĐỊNH NGHĨA PHƯƠNG THỨC NẰM Ở ĐÂY**
        /// </summary>
        private void ClearInputFields()
        {
            if (txtId != null) txtId.Clear();
            if (txtMaCuonSach != null) txtMaCuonSach.Clear();
            if (txtTinhTrang != null) txtTinhTrang.Clear();
            if (cboSach != null) cboSach.SelectedIndex = -1;

            if (_isAdding && cboSach != null && cboSach.Enabled)
            {
                cboSach.Focus();
            }
        }

        /// <summary>
        /// Cập nhật trạng thái Enabled/Disabled của các controls.
        /// </summary>
        /// <param name="isAdding">True nếu đang ở trạng thái Thêm.</param>
        /// <param name="rowSelected">True nếu có dòng đang được chọn.</param>
        private void SetControlState(bool isAdding, bool rowSelected)
        {
            if (btnThem != null) btnThem.Enabled = !isAdding;

            int currentTinhTrangId = -1;
            if (rowSelected && dgvCuonSach?.SelectedRows.Count == 1 && dgvCuonSach.SelectedRows[0].DataBoundItem != null)
            {
                object selectedDataBoundItem = dgvCuonSach.SelectedRows[0].DataBoundItem;
                try
                {
                    var propInfo = selectedDataBoundItem.GetType().GetProperty("TinhTrang"); // Lấy TinhTrang (int)
                    if (propInfo != null && propInfo.PropertyType == typeof(int))
                    {
                        currentTinhTrangId = (int)propInfo.GetValue(selectedDataBoundItem, null);
                    }
                }
                catch { /* Ignore */ }
            }

            bool canUpdateStatus = !isAdding && rowSelected && currentTinhTrangId != TINH_TRANG_ID_DA_CHO_MUON && currentTinhTrangId != -1;
            if (btnCapNhatTrangThai != null) btnCapNhatTrangThai.Enabled = canUpdateStatus;

            if (btnLuu != null) btnLuu.Enabled = isAdding;
            if (btnBoQua != null) btnBoQua.Enabled = isAdding;

            if (txtId != null) txtId.Enabled = false;
            if (txtMaCuonSach != null) txtMaCuonSach.Enabled = false;
            if (txtTinhTrang != null) txtTinhTrang.Enabled = false;
            if (cboSach != null) cboSach.Enabled = isAdding;

            if (dgvCuonSach != null) dgvCuonSach.Enabled = !isAdding;
        }

        /// <summary>
        /// Hiển thị thông tin của dòng đang chọn lên các ô nhập liệu.
        /// </summary>
        private void DisplaySelectedRow()
        {
            if (_isAdding) return;

            object? selectedDataBoundItem = null;
            bool rowSelected = false;

            if (dgvCuonSach != null && dgvCuonSach.SelectedRows.Count == 1)
            {
                selectedDataBoundItem = dgvCuonSach.SelectedRows[0].DataBoundItem;
                rowSelected = true;
            }

            int idCuonSach = 0;
            string? maCuonSach = string.Empty;
            string? tinhTrangText = string.Empty;
            int idSach = 0;

            if (selectedDataBoundItem != null)
            {
                try
                {
                    idCuonSach = (int?)selectedDataBoundItem.GetType().GetProperty("Id")?.GetValue(selectedDataBoundItem, null) ?? 0;
                    maCuonSach = selectedDataBoundItem.GetType().GetProperty("MaCuonSach")?.GetValue(selectedDataBoundItem, null) as string;
                    tinhTrangText = selectedDataBoundItem.GetType().GetProperty("TinhTrangText")?.GetValue(selectedDataBoundItem, null) as string; // Lấy TinhTrangText
                    idSach = (int?)selectedDataBoundItem.GetType().GetProperty("IdSach")?.GetValue(selectedDataBoundItem, null) ?? 0;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi khi đọc thuộc tính từ DataBoundItem: {ex.Message}");
                    rowSelected = false;
                }
            }

            if (txtId != null) txtId.Text = rowSelected ? idCuonSach.ToString() : string.Empty;
            if (txtMaCuonSach != null) txtMaCuonSach.Text = rowSelected ? (maCuonSach ?? string.Empty) : string.Empty;
            if (txtTinhTrang != null) txtTinhTrang.Text = rowSelected ? (tinhTrangText ?? "Không xác định") : string.Empty; // Hiển thị Text

            if (cboSach != null)
            {
                if (rowSelected && idSach > 0 && _sachList.Any(s => s.Id == idSach))
                {
                    cboSach.SelectedValue = idSach;
                }
                else
                {
                    cboSach.SelectedIndex = -1;
                }
            }

            SetControlState(false, rowSelected);
        }


        // --- SỰ KIỆN CỦA CÁC BUTTON ---

        private void btnThem_Click(object sender, EventArgs e)
        {
            _isAdding = true;
            SetControlState(true, false);
            if (dgvCuonSach != null) dgvCuonSach.ClearSelection();
            ClearInputFields(); // Gọi ClearInputFields
            if (txtTinhTrang != null)
            {
                // Hiển thị text mặc định, giá trị int sẽ được gán khi lưu
                txtTinhTrang.Text = "Sẵn có";
            }
            if (cboSach != null) cboSach.Focus();
        }

        private async void btnLuu_Click(object sender, EventArgs e)
        {
            if (!_isAdding) return;

            int idSach = (cboSach?.SelectedValue as int?) ?? 0;

            if (idSach <= 0)
            {
                MaterialMessageBox.Show(this.FindForm(), "Vui lòng chọn ấn bản sách.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboSach?.Focus();
                return;
            }

            CuonSachDTO cuonSachDto = new CuonSachDTO
            {
                Id = 0,
                MaCuonSach = null, // BUS/DAL sẽ xử lý
                IdSach = idSach,
                TinhTrang = TINH_TRANG_ID_SAN_CO // Gán ID int mặc định
            };

            bool success = false;
            string? errorMsg = null;
            CuonSachDTO? addedDto = null;

            if (btnLuu != null) btnLuu.Enabled = false;
            if (btnBoQua != null) btnBoQua.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                addedDto = await _busCuonSach.AddCuonSachAsync(cuonSachDto); // Gọi BUS
                success = addedDto != null && addedDto.Id > 0;
            }
            catch (ArgumentException argEx) { errorMsg = $"Lỗi dữ liệu: {argEx.Message}"; System.Diagnostics.Debug.WriteLine($"ArgumentException saving CuonSach: {argEx}"); }
            catch (InvalidOperationException invOpEx) { errorMsg = $"Lỗi nghiệp vụ: {invOpEx.Message}"; System.Diagnostics.Debug.WriteLine($"InvalidOperationException saving CuonSach: {invOpEx}"); }
            catch (Exception ex) { errorMsg = $"Lỗi hệ thống khi thêm cuốn sách: {ex.Message}"; System.Diagnostics.Debug.WriteLine($"Exception saving CuonSach: {ex}"); }
            finally
            {
                this.Cursor = Cursors.Default;
                if (!success && _isAdding)
                {
                    if (btnLuu != null) btnLuu.Enabled = true;
                    if (btnBoQua != null) btnBoQua.Enabled = true;
                }
            }

            if (success)
            {
                MaterialMessageBox.Show(this.FindForm(), "Thêm cuốn sách thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _isAdding = false;
                await LoadDataGrid();
                if (addedDto != null) SelectRowById(addedDto.Id);
            }
            else if (!string.IsNullOrEmpty(errorMsg))
            {
                MaterialMessageBox.Show(this.FindForm(), errorMsg, "Lỗi Lưu Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboSach?.Focus();
            }
            else
            {
                MaterialMessageBox.Show(this.FindForm(), "Thêm cuốn sách thất bại (lý do không xác định).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBoQua_Click(object sender, EventArgs e)
        {
            if (!_isAdding) return;
            _isAdding = false;
            DisplaySelectedRow();
        }

        private async void btnCapNhatTrangThai_Click(object sender, EventArgs e)
        {
            if (dgvCuonSach?.SelectedRows.Count == 1 && !_isAdding)
            {
                object? selectedDataBoundItem = dgvCuonSach.SelectedRows[0].DataBoundItem;
                if (selectedDataBoundItem == null) return;

                int idToUpdate = 0;
                string? currentMaCuonSach = null;
                int currentTinhTrangId = -1;

                try
                {
                    idToUpdate = (int?)selectedDataBoundItem.GetType().GetProperty("Id")?.GetValue(selectedDataBoundItem, null) ?? 0;
                    currentMaCuonSach = selectedDataBoundItem.GetType().GetProperty("MaCuonSach")?.GetValue(selectedDataBoundItem, null) as string;
                    var propTinhTrangInt = selectedDataBoundItem.GetType().GetProperty("TinhTrang"); // Lấy TinhTrang (int)
                    if (propTinhTrangInt != null && propTinhTrangInt.PropertyType == typeof(int))
                        currentTinhTrangId = (int)propTinhTrangInt.GetValue(selectedDataBoundItem, null);
                }
                catch { /* Ignore */ }

                if (idToUpdate <= 0 || currentTinhTrangId == -1)
                {
                    MaterialMessageBox.Show(this.FindForm(), "Không xác định được thông tin cuốn sách để cập nhật.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (currentTinhTrangId == TINH_TRANG_ID_DA_CHO_MUON)
                {
                    MaterialMessageBox.Show(this.FindForm(), $"Không thể cập nhật trạng thái cho cuốn sách '{currentMaCuonSach}' đang được mượn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var result = MaterialMessageBox.Show(this.FindForm(),
                    $"Chọn trạng thái mới cho cuốn sách '{currentMaCuonSach}':\n\n - YES = Bị mất\n - NO = Hư hỏng\n - CANCEL = Bỏ qua",
                    "Cập Nhật Trạng Thái", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, true);

                int? newStatusId = null;
                if (result == DialogResult.Yes) newStatusId = TINH_TRANG_ID_BI_MAT;
                else if (result == DialogResult.No) newStatusId = TINH_TRANG_ID_HU_HONG;
                else return; // Cancel

                bool success = false;
                string? errorMsg = null;
                if (btnCapNhatTrangThai != null) btnCapNhatTrangThai.Enabled = false;
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    // Giả sử BUS có phương thức UpdateTinhTrangAsync (cần kiểm tra)
                    success = await _busCuonSach.UpdateTinhTrangAsync(idToUpdate, newStatusId.Value);
                }
                catch (ArgumentException argEx) { errorMsg = $"Lỗi dữ liệu: {argEx.Message}"; }
                catch (InvalidOperationException invOpEx) { errorMsg = $"Lỗi nghiệp vụ: {invOpEx.Message}"; }
                catch (Exception ex) { errorMsg = $"Lỗi hệ thống khi cập nhật: {ex.Message}"; System.Diagnostics.Debug.WriteLine($"Exception updating TinhTrang: {ex}"); }
                finally
                {
                    this.Cursor = Cursors.Default;
                }

                if (success)
                {
                    MaterialMessageBox.Show(this.FindForm(), "Cập nhật trạng thái thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataGrid();
                    SelectRowById(idToUpdate);
                }
                else if (!string.IsNullOrEmpty(errorMsg))
                {
                    MaterialMessageBox.Show(this.FindForm(), errorMsg, "Lỗi Cập Nhật", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Bật lại nút nếu cần (kiểm tra lại trạng thái sau lỗi)
                    SetControlState(false, dgvCuonSach?.SelectedRows.Count > 0);
                }
                else
                {
                    MaterialMessageBox.Show(this.FindForm(), "Cập nhật trạng thái thất bại (lý do không xác định).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SetControlState(false, dgvCuonSach?.SelectedRows.Count > 0);
                }
            }
            else if (dgvCuonSach?.SelectedRows.Count == 0 && !_isAdding)
            {
                MaterialMessageBox.Show(this.FindForm(), "Vui lòng chọn cuốn sách cần cập nhật trạng thái.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            _isAdding = false;
            RequestClose?.Invoke(this, EventArgs.Empty);
        }


        // --- SỰ KIỆT CỦA DATAGRIDVIEW ---

        private void dgvCuonSach_SelectionChanged(object sender, EventArgs e)
        {
            if (!_isAdding)
            {
                DisplaySelectedRow();
            }
        }

        // --- HÀM HỖ TRỢ KHÁC ---

        /// <summary>
        /// Chọn một dòng trong DataGridView dựa trên ID của CuonSach.
        /// </summary>
        private void SelectRowById(int id)
        {
            if (dgvCuonSach == null || id <= 0) return;

            dgvCuonSach.ClearSelection();

            bool found = false;
            foreach (DataGridViewRow row in dgvCuonSach.Rows)
            {
                object? dataItem = row.DataBoundItem;
                if (dataItem != null)
                {
                    try
                    {
                        int rowId = (int?)dataItem.GetType().GetProperty("Id")?.GetValue(dataItem, null) ?? 0;
                        if (rowId == id)
                        {
                            row.Selected = true;
                            found = true;

                            try
                            {
                                if (!row.Displayed)
                                {
                                    int firstDisplayed = Math.Max(0, row.Index - dgvCuonSach.DisplayedRowCount(false) / 2);
                                    if (firstDisplayed >= 0 && firstDisplayed < dgvCuonSach.RowCount)
                                        dgvCuonSach.FirstDisplayedScrollingRowIndex = firstDisplayed;
                                    else
                                        dgvCuonSach.FirstDisplayedScrollingRowIndex = Math.Max(0, row.Index);
                                }
                            }
                            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Lỗi khi cuộn DGV Cuốn Sách đến ID {id}: {ex.Message}"); }

                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Lỗi khi đọc ID từ DataBoundItem để chọn dòng: {ex.Message}");
                        continue;
                    }
                }
            }

            // Không cần gọi SetControlState ở đây vì SelectionChanged sẽ được kích hoạt nếu tìm thấy dòng
            // Nếu không tìm thấy, LoadDataGrid đã gọi SetControlState(false, false) trong finally block
        }

    } // <<<< Kết thúc lớp ucQuanLyCuonSach
} // <<<< Kết thúc namespace GUI