using BUS;
using DAL;
using DTO;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GUI.frmMain; // Để dùng IRequiresDataLoading

namespace GUI
{
    public partial class ucQuanLyThongBao : UserControl, IRequiresDataLoading // Implement interface
    {
        private readonly IBUSThongBao _busThongBao;
        private List<ThongBaoDTO> _listThongBao = new List<ThongBaoDTO>();
        private bool _isAdding = false;
        private int? _editingThongBaoId = null;

        // Danh sách các mức độ và trạng thái
        private readonly List<string> _mucDoOptions = new List<string> { "Thông tin", "Cảnh báo", "Quan trọng" };
        private readonly List<string> _trangThaiOptions = new List<string> { "Active", "Draft" }; // Chỉ cho phép tạo Active hoặc Draft

        public ucQuanLyThongBao(IBUSThongBao busThongBao)
        {
            InitializeComponent();
            _busThongBao = busThongBao ?? throw new ArgumentNullException(nameof(busThongBao));
        }

        // --- Implement IRequiresDataLoading ---
        public async Task InitializeDataAsync()
        {
            Debug.WriteLine("ucQuanLyThongBao initializing data...");
            SetupInputForm(); // Setup comboboxes, etc.
            await LoadDataAsync(); // Load data grid
            Debug.WriteLine("ucQuanLyThongBao data initialization complete.");
        }

        private void ucQuanLyThongBao_Load(object sender, EventArgs e)
        {
            // Không cần làm gì ở đây nếu đã dùng InitializeDataAsync
            // Nếu không dùng interface, bạn sẽ gọi SetupInputForm() và await LoadDataAsync() ở đây.
        }

        private void SetupInputForm()
        {
            // Load ComboBoxes
            cmbMucDo.DataSource = null;
            cmbMucDo.DataSource = _mucDoOptions;
            cmbMucDo.SelectedIndex = 0; // Mặc định là Thông tin

            cmbTrangThai.DataSource = null;
            cmbTrangThai.DataSource = _trangThaiOptions;
            cmbTrangThai.SelectedIndex = 0; // Mặc định là Active

            // Thiết lập DateTimePickers
            dtpNgayBatDau.Value = DateTime.Now;
            dtpNgayKetThuc.Value = DateTime.Now.AddDays(7);
            dtpNgayKetThuc.Format = DateTimePickerFormat.Short; // Hoặc Custom nếu muốn cả giờ
            dtpNgayBatDau.Format = DateTimePickerFormat.Short;

            // Ban đầu ẩn panel nhập liệu
            HideInputPanel();
            lblInputError.Text = ""; // Xóa thông báo lỗi cũ
            lblInputError.Visible = false;

            // Thiết lập ban đầu cho checkbox "Không hết hạn"
            chkKhongHetHan.Checked = false; // Mặc định là có ngày hết hạn
            dtpNgayKetThuc.Enabled = true; // Cho phép chọn ngày

            // Đăng ký sự kiện CheckedChanged cho checkbox
            chkKhongHetHan.CheckedChanged -= chkKhongHetHan_CheckedChanged; // Hủy đăng ký cũ (nếu có)
            chkKhongHetHan.CheckedChanged += chkKhongHetHan_CheckedChanged;
        }

        private async Task LoadDataAsync()
        {
            try
            {
                _listThongBao = await _busThongBao.GetAllNotificationsAsync();
                dgvThongBao.DataSource = null; // Xóa binding cũ

                // Cấu hình DataGridView (chỉ làm lần đầu hoặc nếu cần)
                ConfigureDataGridView();

                // Binding dữ liệu
                dgvThongBao.DataSource = _listThongBao;

                Debug.WriteLine($"Loaded {_listThongBao.Count} notifications into grid.");

                // Cập nhật trạng thái nút Sửa/Xóa
                UpdateEditDeleteButtonStates();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading notifications: {ex.Message}");
                MaterialMessageBox.Show(this.FindForm(), $"Không thể tải danh sách thông báo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureDataGridView()
        {
            if (dgvThongBao.Columns.Count == 0) // Chỉ cấu hình nếu chưa có cột
            {
                dgvThongBao.AutoGenerateColumns = false;
                dgvThongBao.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Định nghĩa các cột bạn muốn hiển thị
                dgvThongBao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50, ReadOnly = true });
                dgvThongBao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TieuDe", HeaderText = "Tiêu đề", FillWeight = 30 });
                dgvThongBao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MucDo", HeaderText = "Mức độ", Width = 100 });
                dgvThongBao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NgayBatDau", HeaderText = "Ngày BĐ", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });
                dgvThongBao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NgayKetThuc", HeaderText = "Ngày KT", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });
                dgvThongBao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TrangThai", HeaderText = "Trạng thái", Width = 80 });
                dgvThongBao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NgayTao", HeaderText = "Ngày tạo", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });

                // Ẩn cột Nội dung vì nó dài
                // dgvThongBao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NoiDung", HeaderText = "Nội dung", Visible = false });

                dgvThongBao.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvThongBao.MultiSelect = false;
                dgvThongBao.AllowUserToAddRows = false;
                dgvThongBao.AllowUserToDeleteRows = false;
                dgvThongBao.ReadOnly = true; // Chỉ cho đọc trên grid
            }
            // Định dạng lại cột ngày kết thúc để hiển thị trống nếu là NULL
            var ngayKTCol = dgvThongBao.Columns.Cast<DataGridViewColumn>().FirstOrDefault(c => c.DataPropertyName == "NgayKetThuc");
            if (ngayKTCol != null)
            {
                ngayKTCol.DefaultCellStyle.NullValue = ""; // Hiển thị ô trống nếu giá trị là null
            }
        }


        // --- Xử lý Nút CRUD ---

        private void btnThem_Click(object sender, EventArgs e)
        {
            _isAdding = true;
            _editingThongBaoId = null;
            ClearInputFields();
            ShowInputPanel("Thêm Thông Báo Mới");
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvThongBao.SelectedRows.Count > 0)
            {
                var selectedRow = dgvThongBao.SelectedRows[0];
                if (selectedRow.DataBoundItem is ThongBaoDTO selectedThongBao)
                {
                    _isAdding = false;
                    _editingThongBaoId = selectedThongBao.Id;
                    LoadThongBaoToInput(selectedThongBao);
                    ShowInputPanel($"Sửa Thông Báo (ID: {selectedThongBao.Id})");
                }
            }
            else
            {
                MaterialMessageBox.Show(this.FindForm(), "Vui lòng chọn một thông báo để sửa.", "Chưa chọn thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvThongBao.SelectedRows.Count > 0)
            {
                var selectedRow = dgvThongBao.SelectedRows[0];
                if (selectedRow.DataBoundItem is ThongBaoDTO selectedThongBao)
                {
                    var result = MaterialMessageBox.Show(this.FindForm(),
                        $"Bạn có chắc chắn muốn xóa thông báo '{selectedThongBao.TieuDe}' (ID: {selectedThongBao.Id}) không?",
                        "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            var deleteResult = await _busThongBao.DeleteNotificationAsync(selectedThongBao.Id);
                            if (deleteResult.Success)
                            {
                                MaterialMessageBox.Show(this.FindForm(), "Xóa thông báo thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                await LoadDataAsync(); // Tải lại dữ liệu
                            }
                            else
                            {
                                MaterialMessageBox.Show(this.FindForm(), $"Xóa thất bại: {deleteResult.ErrorMessage}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error deleting notification: {ex.Message}");
                            MaterialMessageBox.Show(this.FindForm(), $"Đã xảy ra lỗi khi xóa: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MaterialMessageBox.Show(this.FindForm(), "Vui lòng chọn một thông báo để xóa.", "Chưa chọn thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // --- Xử lý Lưu / Hủy ---

        private async void btnLuu_Click(object sender, EventArgs e)
        {
            lblInputError.Visible = false; // Ẩn lỗi cũ
            var validation = ValidateInput();
            if (!validation.IsValid)
            {
                lblInputError.Text = validation.ErrorMessage;
                lblInputError.Visible = true;
                return;
            }

            // Tạo DTO từ input
            var thongBaoDto = new ThongBaoDTO
            {
                Id = _editingThongBaoId ?? 0, // Nếu là thêm mới thì Id là 0 hoặc không quan trọng
                TieuDe = txtTieuDe.Text.Trim(),
                NoiDung = txtNoiDung.Text.Trim(),
                NgayBatDau = dtpNgayBatDau.Value,
                NgayKetThuc = chkKhongHetHan.Checked ? (DateTime?)null : dtpNgayKetThuc.Value,
                MucDo = cmbMucDo.SelectedItem?.ToString() ?? _mucDoOptions[0], // Lấy giá trị đã chọn hoặc mặc định
                TrangThai = cmbTrangThai.SelectedItem?.ToString() ?? _trangThaiOptions[0]
                // NgayTao sẽ do CSDL tự xử lý hoặc BUS/DAL xử lý
            };

            try
            {
                bool success = false;
                string errorMessage = "";

                if (_isAdding) // Thêm mới
                {
                    var addResult = await _busThongBao.AddNotificationAsync(thongBaoDto);
                    success = addResult.Success;
                    errorMessage = addResult.ErrorMessage;
                    if (success) MaterialMessageBox.Show(this.FindForm(), "Thêm thông báo thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else // Cập nhật
                {
                    if (_editingThongBaoId.HasValue) // Đảm bảo có ID để sửa
                    {
                        thongBaoDto.Id = _editingThongBaoId.Value; // Gán ID vào DTO
                        var updateResult = await _busThongBao.UpdateNotificationAsync(thongBaoDto);
                        success = updateResult.Success;
                        errorMessage = updateResult.ErrorMessage;
                        if (success) MaterialMessageBox.Show(this.FindForm(), "Cập nhật thông báo thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        errorMessage = "Không xác định được thông báo cần sửa.";
                    }
                }

                if (success)
                {
                    HideInputPanel();
                    await LoadDataAsync(); // Tải lại dữ liệu
                }
                else
                {
                    lblInputError.Text = $"Lỗi khi lưu: {errorMessage}";
                    lblInputError.Visible = true;
                    // Không ẩn panel input để người dùng sửa lại
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving notification: {ex.Message}");
                lblInputError.Text = $"Lỗi hệ thống: {ex.Message}";
                lblInputError.Visible = true;
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            HideInputPanel();
        }

        // --- Helpers ---

        private void ShowInputPanel(string title)
        {
            // Có thể thay đổi Text của panelInput hoặc thêm một Label tiêu đề riêng
            // Ví dụ: this.Parent.Text = title; // Nếu muốn đổi tiêu đề của Form cha (không nên)
            // Hoặc thêm 1 MaterialLabel lblInputTitle vào panelInput
            panelInput.Visible = true;
            panelInput.BringToFront(); // Đưa panel lên trên DataGridView
            // Có thể vô hiệu hóa các nút Thêm/Sửa/Xóa và DataGridView khi panel hiện ra
            dgvThongBao.Enabled = false;
            btnThem.Enabled = false;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }

        private void HideInputPanel()
        {
            panelInput.Visible = false;
            ClearInputFields(); // Xóa input khi ẩn đi
            lblInputError.Text = "";
            lblInputError.Visible = false;
            _isAdding = false;
            _editingThongBaoId = null;
            // Kích hoạt lại các nút và DataGridView
            dgvThongBao.Enabled = true;
            btnThem.Enabled = true;
            UpdateEditDeleteButtonStates(); // Cập nhật lại trạng thái Sửa/Xóa dựa trên lựa chọn grid
        }

        private void ClearInputFields()
        {
            txtTieuDe.Clear();
            txtNoiDung.Clear();
            dtpNgayBatDau.Value = DateTime.Now;
            dtpNgayKetThuc.Value = DateTime.Now.AddDays(7);
            chkKhongHetHan.Checked = false;
            cmbMucDo.SelectedIndex = 0;
            cmbTrangThai.SelectedIndex = 0;
            lblInputError.Text = "";
            lblInputError.Visible = false;
        }

        private void LoadThongBaoToInput(ThongBaoDTO thongBao)
        {
            txtTieuDe.Text = thongBao.TieuDe;
            txtNoiDung.Text = thongBao.NoiDung;
            dtpNgayBatDau.Value = thongBao.NgayBatDau;

            if (thongBao.NgayKetThuc.HasValue)
            {
                chkKhongHetHan.Checked = false;
                dtpNgayKetThuc.Value = thongBao.NgayKetThuc.Value;
                dtpNgayKetThuc.Enabled = true;
            }
            else
            {
                chkKhongHetHan.Checked = true;
                dtpNgayKetThuc.Enabled = false; // Không cho chọn khi checkbox được tick
            }

            // Chọn đúng giá trị trong ComboBox
            cmbMucDo.SelectedItem = _mucDoOptions.FirstOrDefault(m => m.Equals(thongBao.MucDo, StringComparison.OrdinalIgnoreCase)) ?? _mucDoOptions[0];
            cmbTrangThai.SelectedItem = _trangThaiOptions.FirstOrDefault(t => t.Equals(thongBao.TrangThai, StringComparison.OrdinalIgnoreCase)) ?? _trangThaiOptions[0];
        }

        private (bool IsValid, string ErrorMessage) ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtTieuDe.Text))
            {
                return (false, "Tiêu đề không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(txtNoiDung.Text))
            {
                return (false, "Nội dung không được để trống.");
            }
            if (!chkKhongHetHan.Checked && dtpNgayKetThuc.Value.Date < dtpNgayBatDau.Value.Date) // Chỉ kiểm tra ngày nếu có ngày KT
            {
                return (false, "Ngày kết thúc không được trước ngày bắt đầu.");
            }
            if (cmbMucDo.SelectedIndex < 0)
            {
                return (false, "Vui lòng chọn mức độ.");
            }
            if (cmbTrangThai.SelectedIndex < 0)
            {
                return (false, "Vui lòng chọn trạng thái.");
            }

            // Thêm các kiểm tra khác nếu cần

            return (true, ""); // Hợp lệ
        }

        private void chkKhongHetHan_CheckedChanged(object sender, EventArgs e)
        {
            // Khi check vào "Không hết hạn", vô hiệu hóa DateTimePicker ngày kết thúc
            dtpNgayKetThuc.Enabled = !chkKhongHetHan.Checked;
        }

        private void dgvThongBao_SelectionChanged(object sender, EventArgs e)
        {
            UpdateEditDeleteButtonStates();
        }

        private void UpdateEditDeleteButtonStates()
        {
            // Chỉ bật nút Sửa/Xóa nếu có dòng được chọn và panel input đang không hiển thị
            bool rowSelected = dgvThongBao.SelectedRows.Count > 0;
            bool inputHidden = !panelInput.Visible;

            btnSua.Enabled = rowSelected && inputHidden;
            btnXoa.Enabled = rowSelected && inputHidden;
        }
    }
}