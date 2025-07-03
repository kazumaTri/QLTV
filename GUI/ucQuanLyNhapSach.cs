using BUS;
using DTO;
using MaterialSkin.Controls;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class ucQuanLyNhapSach : UserControl, IRequiresDataLoading
    {
        private readonly IBUSPhieuNhapSach _busPhieuNhapSach;
        private readonly IBUSSach _busSach; // Để lấy danh sách sách
        private readonly ILogger<ucQuanLyNhapSach> _logger;
        private BindingList<CtPhieuNhapDTO> _chiTietPhieuNhapList; // DataSource cho DataGridView
        private List<SachDTO> _sachList; // Cache danh sách sách

        public ucQuanLyNhapSach(IBUSPhieuNhapSach busPhieuNhapSach, IBUSSach busSach, ILogger<ucQuanLyNhapSach> logger)
        {
            InitializeComponent();

            _busPhieuNhapSach = busPhieuNhapSach ?? throw new ArgumentNullException(nameof(busPhieuNhapSach));
            _busSach = busSach ?? throw new ArgumentNullException(nameof(busSach));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _chiTietPhieuNhapList = new BindingList<CtPhieuNhapDTO>();
            // Đăng ký sự kiện ListChanged để cập nhật tổng tiền khi danh sách thay đổi
            _chiTietPhieuNhapList.ListChanged += ChiTietPhieuNhapList_ListChanged;
        }

        // Hàm này sẽ được gọi từ frmMain
        public async Task InitializeDataAsync()
        {
            _logger.LogInformation("Initializing data for ucQuanLyNhapSach...");
            await LoadSachListAsync();
            SetupDataGridView();
            ResetForm(); // Đặt lại form về trạng thái ban đầu
            _logger.LogInformation("Data initialization complete for ucQuanLyNhapSach.");
        }

        private void ucQuanLyNhapSach_Load(object sender, EventArgs e)
        {
            // Có thể gọi InitializeDataAsync ở đây nếu không gọi từ frmMain
            // hoặc thực hiện các cài đặt UI ban đầu khác
            ConfigureUI();
        }


        private void ConfigureUI()
        {
            // Cấu hình giao diện ban đầu
            dgvChiTietPhieuNhap.DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("vi-VN");
            dgvChiTietPhieuNhap.Columns[nameof(colDonGia)].DefaultCellStyle.Format = "N0"; // Định dạng tiền tệ
            dgvChiTietPhieuNhap.Columns[nameof(colThanhTien)].DefaultCellStyle.Format = "N0"; // Định dạng tiền tệ

            // Thiết lập AutoComplete cho ComboBox sách (nếu muốn)
            // cboSach.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            // cboSach.AutoCompleteSource = AutoCompleteSource.ListItems;
        }


        private async Task LoadSachListAsync()
        {
            try
            {
                _sachList = await _busSach.GetAllSachAsync(); // Lấy tất cả sách DTO
                if (_sachList != null && _sachList.Any())
                {
                    cboSach.DataSource = _sachList;
                    cboSach.DisplayMember = nameof(SachDTO.DisplaySach); // Hiển thị Mã + Tên Tựa Sách
                    cboSach.ValueMember = nameof(SachDTO.Id); // Giá trị là Id Sách
                    cboSach.SelectedIndex = -1; // Không chọn gì ban đầu
                    cboSach.Hint = "Chọn sách hoặc tìm kiếm...";
                }
                else
                {
                    cboSach.DataSource = null;
                    cboSach.Items.Clear();
                    cboSach.Hint = "Không có sách nào";
                    _logger.LogWarning("No books found to populate ComboBox.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading book list.");
                MessageBox.Show($"Lỗi khi tải danh sách sách: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDataGridView()
        {
            dgvChiTietPhieuNhap.AutoGenerateColumns = false; // Đã thiết lập trong Designer
            dgvChiTietPhieuNhap.DataSource = _chiTietPhieuNhapList; // Gán BindingList làm nguồn dữ liệu

            // Thiết lập lại tên cột HeaderText (nếu cần)
            dgvChiTietPhieuNhap.Columns[nameof(colMaSach)].HeaderText = "Mã Sách";
            dgvChiTietPhieuNhap.Columns[nameof(colTenSach)].HeaderText = "Tên Sách";
            dgvChiTietPhieuNhap.Columns[nameof(colSoLuongNhap)].HeaderText = "Số Lượng";
            dgvChiTietPhieuNhap.Columns[nameof(colDonGia)].HeaderText = "Đơn Giá (VNĐ)";
            dgvChiTietPhieuNhap.Columns[nameof(colThanhTien)].HeaderText = "Thành Tiền (VNĐ)";

            // Định dạng cột số
            dgvChiTietPhieuNhap.Columns[nameof(colSoLuongNhap)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvChiTietPhieuNhap.Columns[nameof(colDonGia)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvChiTietPhieuNhap.Columns[nameof(colThanhTien)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Set Width (Optional)
            // dgvChiTietPhieuNhap.Columns[nameof(colMaSach)].Width = 100;
            // ...
        }

        private void ResetForm()
        {
            dtpNgayNhap.Value = DateTime.Now;
            lblSoPhieuValue.Text = "(Tự động)";
            cboSach.SelectedIndex = -1;
            numSoLuongNhap.Value = 1;
            txtDonGia.Text = "0"; // Hoặc string.Empty
            _chiTietPhieuNhapList.Clear(); // Xóa danh sách chi tiết
                                           // UpdateTongTien(); // Hàm Clear() sẽ trigger ListChanged -> UpdateTongTien()
            btnXoaSachKhoiPhieu.Enabled = false;
            cboSach.Focus();
            _logger.LogInformation("Form reset to initial state.");
        }

        private void UpdateTongTien()
        {
            decimal tongTien = _chiTietPhieuNhapList.Sum(ct => ct.ThanhTien);
            lblTongTienValue.Text = tongTien.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " VNĐ";
            _logger.LogTrace("Total amount updated: {TongTien}", tongTien);
        }


        private void ChiTietPhieuNhapList_ListChanged(object? sender, ListChangedEventArgs e)
        {
            // Cập nhật lại tổng tiền mỗi khi danh sách chi tiết thay đổi (thêm, xóa)
            UpdateTongTien();
        }


        private void btnLapPhieuMoi_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void btnThemSachVaoPhieu_Click(object sender, EventArgs e)
        {
            _logger.LogInformation("Attempting to add book to the slip details.");
            // 1. Validate Input
            if (cboSach.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một sách.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboSach.Focus();
                return;
            }

            if (!decimal.TryParse(txtDonGia.Text, NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out decimal donGia) || donGia < 0)
            {
                MessageBox.Show("Đơn giá không hợp lệ. Vui lòng nhập số lớn hơn hoặc bằng 0.", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDonGia.Focus();
                return;
            }

            int soLuong = (int)numSoLuongNhap.Value;
            if (soLuong <= 0)
            {
                MessageBox.Show("Số lượng nhập phải lớn hơn 0.", "Không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numSoLuongNhap.Focus();
                return;
            }

            var selectedSach = (SachDTO)cboSach.SelectedItem;

            // 2. Check if book already exists in the list
            var existingItem = _chiTietPhieuNhapList.FirstOrDefault(ct => ct.IdSach == selectedSach.Id);
            if (existingItem != null)
            {
                // Option 1: Update quantity and price (Ask user?)
                var result = MessageBox.Show($"Sách '{selectedSach.DisplaySach}' đã có trong phiếu. Bạn có muốn cập nhật số lượng và đơn giá không?",
                                             "Sách đã tồn tại", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    existingItem.SoLuongNhap += soLuong;
                    existingItem.DonGia = donGia; // Cập nhật đơn giá mới
                    _chiTietPhieuNhapList.ResetBindings(); // Refresh DataGridView để hiển thị thay đổi
                    _logger.LogInformation("Updated existing item: IdSach={IdSach}, New Quantity={Quantity}, New Price={Price}", existingItem.IdSach, existingItem.SoLuongNhap, existingItem.DonGia);
                }
                else
                {
                    _logger.LogInformation("User chose not to update existing item.");
                }

                // Option 2: Prevent adding duplicates
                // MessageBox.Show($"Sách '{selectedSach.DisplaySach}' đã có trong phiếu.", "Trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // return;
            }
            else
            {
                // 3. Create CtPhieuNhapDTO
                var newItem = new CtPhieuNhapDTO
                {
                    IdSach = selectedSach.Id,
                    MaSach = selectedSach.MaSach,
                    TenSach = selectedSach.TenTuaSach, // Lấy tên từ SachDTO
                    SoLuongNhap = soLuong,
                    DonGia = donGia
                    // ThanhTien được tính tự động
                };

                // 4. Add to BindingList (triggers UI update and ListChanged event)
                _chiTietPhieuNhapList.Add(newItem);
                _logger.LogInformation("Added new item: IdSach={IdSach}, Quantity={Quantity}, Price={Price}", newItem.IdSach, newItem.SoLuongNhap, newItem.DonGia);
            }


            // 5. Clear input fields for next entry
            cboSach.SelectedIndex = -1;
            numSoLuongNhap.Value = 1;
            txtDonGia.Text = "0";
            cboSach.Focus();
        }


        private void btnXoaSachKhoiPhieu_Click(object sender, EventArgs e)
        {
            if (dgvChiTietPhieuNhap.SelectedRows.Count > 0)
            {
                var selectedRow = dgvChiTietPhieuNhap.SelectedRows[0];
                var itemToRemove = (CtPhieuNhapDTO)selectedRow.DataBoundItem;

                var confirmResult = MessageBox.Show($"Bạn có chắc muốn xóa sách '{itemToRemove.TenSach}' khỏi phiếu nhập không?",
                                                   "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    _chiTietPhieuNhapList.Remove(itemToRemove);
                    _logger.LogInformation("Removed item: IdSach={IdSach}", itemToRemove.IdSach);
                    btnXoaSachKhoiPhieu.Enabled = false; // Disable button after removal or if list becomes empty
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sách cần xóa từ danh sách.", "Chưa chọn sách", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvChiTietPhieuNhap_SelectionChanged(object sender, EventArgs e)
        {
            // Enable/disable nút xóa dựa trên việc có dòng nào được chọn hay không
            btnXoaSachKhoiPhieu.Enabled = dgvChiTietPhieuNhap.SelectedRows.Count > 0;
        }

        private async void btnLuuPhieu_Click(object sender, EventArgs e)
        {
            _logger.LogInformation("Attempting to save the PhieuNhapSach.");
            // 1. Validate if there are details
            if (!_chiTietPhieuNhapList.Any())
            {
                MessageBox.Show("Phiếu nhập phải có ít nhất một sách.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Create PhieuNhapSachDTO
            var phieuNhapDTO = new PhieuNhapSachDTO
            {
                NgayNhap = dtpNgayNhap.Value,
                ChiTietPhieuNhap = _chiTietPhieuNhapList.ToList() // Chuyển BindingList thành List
                // TongTien sẽ được tính trong BUS hoặc có thể tính ở đây nếu muốn
                // TongTien = _chiTietPhieuNhapList.Sum(ct => ct.ThanhTien)
            };


            // 3. Call BUS to save
            try
            {
                this.Cursor = Cursors.WaitCursor; // Show wait cursor
                btnLuuPhieu.Enabled = false; // Disable button during save

                int soPhieuMoi = await _busPhieuNhapSach.LapPhieuNhapAsync(phieuNhapDTO);

                _logger.LogInformation("PhieuNhapSach saved successfully with SoPhieuNhap: {SoPhieuNhap}", soPhieuMoi);
                MessageBox.Show($"Lập phiếu nhập thành công!\nSố phiếu: {soPhieuMoi}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 4. Reset form for new entry
                ResetForm();
                await LoadSachListAsync(); // Tải lại danh sách sách vì số lượng có thể đã thay đổi
            }
            catch (InvalidOperationException invOpEx) // Lỗi nghiệp vụ hoặc validation từ BUS
            {
                _logger.LogWarning(invOpEx, "Business logic error while saving PhieuNhapSach.");
                MessageBox.Show($"Lỗi khi lưu phiếu nhập: {invOpEx.Message}", "Lỗi nghiệp vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) // Lỗi hệ thống hoặc DB
            {
                _logger.LogError(ex, "Error saving PhieuNhapSach.");
                MessageBox.Show($"Đã xảy ra lỗi không mong muốn khi lưu phiếu nhập:\n{ex.Message}\nChi tiết xem trong file log.", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default; // Restore cursor
                btnLuuPhieu.Enabled = true; // Re-enable button
            }
        }

        private void btnHuyPhieu_Click(object sender, EventArgs e)
        {
            if (_chiTietPhieuNhapList.Any())
            {
                var confirmResult = MessageBox.Show("Bạn có chắc muốn hủy phiếu nhập đang lập không? Mọi thay đổi sẽ bị mất.",
                                                    "Xác nhận hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmResult == DialogResult.Yes)
                {
                    ResetForm();
                }
            }
            else
            {
                ResetForm(); // Nếu chưa có gì thì reset luôn
            }
        }

        // Chỉ cho phép nhập số vào ô Đơn giá
        private void txtDonGia_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Cho phép nhập số, dấu phẩy (cho thập phân theo VN) và phím Backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // Chỉ cho phép một dấu phẩy
            if ((e.KeyChar == ',') && ((sender as MaterialTextBox2).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }
    }
}