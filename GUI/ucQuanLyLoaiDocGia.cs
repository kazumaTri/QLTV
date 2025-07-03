// --- USING DIRECTIVES ---
using BUS; // Namespace chứa IBUSLoaiDocGia
using DTO; // Namespace chứa LoaiDocGiaDTO
using MaterialSkin.Controls; // Controls MaterialSkin
using System;
using System.Collections.Generic;
using System.ComponentModel; // Cần cho SortOrder
using System.Diagnostics; // Cần cho Debug
using System.Drawing;
using System.IO; // Cần cho File, SaveFileDialog
using System.Linq;
using System.Text; // Cần cho StringBuilder
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin; // Cần cho MaterialMessageBox
using static GUI.frmMain; // Sử dụng interface IRequiresDataLoading

namespace GUI
{
    /// <summary>
    /// UserControl quản lý thông tin Loại Độc Giả.
    /// Hiển thị danh sách, cho phép thêm, sửa, xóa, tìm kiếm, sắp xếp, xuất CSV.
    /// </summary>
    public partial class ucQuanLyLoaiDocGia : UserControl, IRequiresDataLoading
    {
        // --- KHAI BÁO BIẾN ---
        private readonly IBUSLoaiDocGia _busLoaiDocGia;
        private LoaiDocGiaDTO? _originalDto; // Dữ liệu gốc của dòng đang chọn
        public event EventHandler? RequestClose; // Sự kiện yêu cầu đóng UC (nếu cần)
        private bool _isAdding = false; // Cờ trạng thái Thêm mới

        // Biến cho Sắp xếp
        private string _sortColumn = nameof(LoaiDocGiaDTO.MaLoaiDocGia); // Cột sắp xếp mặc định
        private SortOrder _sortDirection = SortOrder.Ascending; // Hướng sắp xếp mặc định

        // --- KHỞI TẠO (CONSTRUCTOR) ---
        public ucQuanLyLoaiDocGia(IBUSLoaiDocGia busLoaiDocGia)
        {
            InitializeComponent();

            _busLoaiDocGia = busLoaiDocGia ?? throw new ArgumentNullException(nameof(busLoaiDocGia));

            // *** Đăng ký sự kiện bằng tay để tránh lỗi designer ***
            this.dgvLoaiDocGia.SelectionChanged += new System.EventHandler(this.dgvLoaiDocGia_SelectionChanged);
            this.dgvLoaiDocGia.DoubleClick += new System.EventHandler(this.dgvLoaiDocGia_DoubleClick);
            this.dgvLoaiDocGia.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvLoaiDocGia_ColumnHeaderMouseClick);
            //this.txtTimKiem.TextChanged += new System.EventHandler(this.txtTimKiem_TextChanged); // Đã gán trong Designer
            //this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click); // Đã gán trong Designer
        }

        // --- PHƯƠNG THỨC KHỞI TẠO DỮ LIỆU (IMPLEMENT TỪ IRequiresDataLoading) ---
        public async Task InitializeDataAsync()
        {
            Debug.WriteLine("ucQuanLyLoaiDocGia.InitializeDataAsync called.");
            if (!this.DesignMode)
            {
                _isAdding = false;
                _originalDto = null;
                _sortColumn = nameof(LoaiDocGiaDTO.MaLoaiDocGia); // Reset sort
                _sortDirection = SortOrder.Ascending;
                if (txtTimKiem != null) txtTimKiem.Clear(); // Reset search

                ClearInputFields();
                SetControlState(false, false); // Trạng thái xem ban đầu

                await LoadDataGrid(); // Tải dữ liệu ban đầu (có áp dụng sort mặc định)
            }
        }

        // --- SỰ KIỆN LOAD USERCONTROL ---
        private void ucQuanLyLoaiDocGia_Load(object sender, EventArgs e)
        {
            // Không cần gọi LoadDataGrid ở đây nữa
        }

        // --- HÀM TẢI DỮ LIỆU & CÀI ĐẶT GIAO DIỆN ---

        /// <summary>
        /// Tải hoặc tải lại dữ liệu Loại Độc Giả vào DataGridView, áp dụng tìm kiếm và sắp xếp.
        /// </summary>
        private async Task LoadDataGrid()
        {
            string? searchTerm = txtTimKiem?.Text.Trim(); // Lấy searchTerm từ textbox
            Debug.WriteLine($"--- LoadDataGrid START (Search: '{searchTerm}', Sort: '{_sortColumn}' {_sortDirection}) ---");
            int? selectedId = null;

            // Lưu ID dòng đang chọn (nếu có)
            if (dgvLoaiDocGia?.SelectedRows.Count == 1 && dgvLoaiDocGia.SelectedRows[0]?.DataBoundItem is LoaiDocGiaDTO currentDto)
            {
                selectedId = currentDto.Id;
                Debug.WriteLine($"LoadDataGrid: Selected ID to restore: {selectedId}");
            }
            else
            {
                Debug.WriteLine("LoadDataGrid: No single row selected or DataBoundItem invalid before load.");
            }

            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (dgvLoaiDocGia == null)
                {
                    Debug.WriteLine("ERROR: dgvLoaiDocGia is null in LoadDataGrid!");
                    return;
                }

                dgvLoaiDocGia.SelectionChanged -= dgvLoaiDocGia_SelectionChanged; // Tạm hủy event
                dgvLoaiDocGia.DataSource = null; // Xóa nguồn cũ

                // --- Gọi BUS để lấy dữ liệu đã lọc VÀ sắp xếp ---
                Debug.WriteLine($"LoadDataGrid: Calling GetLoaiDocGiaFilteredAndSortedAsync(searchTerm='{searchTerm}', sortColumn='{_sortColumn}', ascending={_sortDirection == SortOrder.Ascending})");
                var danhSach = await _busLoaiDocGia.GetLoaiDocGiaFilteredAndSortedAsync(
                                                    searchTerm,
                                                    _sortColumn,
                                                    _sortDirection == SortOrder.Ascending);
                int danhSachCount = danhSach?.Count ?? 0;
                Debug.WriteLine($"LoadDataGrid: Received danhSach from BUS. Count={danhSachCount}");
                // --- Kết thúc phần gọi BUS ---

                dgvLoaiDocGia.DataSource = danhSach; // Gán nguồn mới
                SetupDataGridViewColumns(); // Cấu hình cột SAU KHI gán DataSource

                // Cập nhật glyph sắp xếp trên header
                UpdateSortGlyph();

                // Chọn lại dòng đã lưu ID
                if (selectedId.HasValue)
                {
                    SelectRowById(selectedId.Value);
                }
                else
                {
                    // Xử lý trường hợp không có dòng nào được chọn lại
                    if (dgvLoaiDocGia.Rows.Count == 0)
                    {
                        ClearInputFields();
                        _originalDto = null;
                        SetControlState(false, false);
                    }
                    // Nếu có dòng nhưng không có selection, SelectionChanged sẽ xử lý khi dòng đầu tiên được chọn mặc định
                }

                dgvLoaiDocGia.Refresh(); // Đảm bảo hiển thị
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                Debug.WriteLine($"ERROR CAUGHT in LoadDataGrid: {ex.GetType().Name} - {ex.Message}");
                Debug.WriteLine($"StackTrace:\n{ex.StackTrace}");
                Debug.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                MaterialMessageBox.Show(this.FindForm(), $"Lỗi khi tải danh sách loại độc giả: {ex.Message}", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Reset DataSource nếu có lỗi nghiêm trọng
                if (dgvLoaiDocGia != null) dgvLoaiDocGia.DataSource = null;
                ClearInputFields();
                SetControlState(false, false);
            }
            finally
            {
                this.Cursor = Cursors.Default; // Khôi phục con trỏ
                // Đăng ký lại event
                if (dgvLoaiDocGia != null && !dgvLoaiDocGia.IsDisposed)
                {
                    try
                    {
                        dgvLoaiDocGia.SelectionChanged -= dgvLoaiDocGia_SelectionChanged; // Ensure only one subscriber
                        dgvLoaiDocGia.SelectionChanged += dgvLoaiDocGia_SelectionChanged;
                        Debug.WriteLine("LoadDataGrid: Re-subscribed to SelectionChanged.");

                        // Xử lý lại trạng thái nếu không có dòng nào được chọn sau cùng
                        if (dgvLoaiDocGia.SelectedRows.Count == 0 && !_isAdding && !IsEditing())
                        {
                            Debug.WriteLine("LoadDataGrid Finally: No row selected, clearing inputs.");
                            ClearInputFields();
                            _originalDto = null;
                            SetControlState(false, false);
                        }
                    }
                    catch (Exception ex) { Debug.WriteLine($"LoadDataGrid Finally Error: {ex.Message}"); }
                }
                Debug.WriteLine("--- LoadDataGrid FINALLY block END ---");
            }
            Debug.WriteLine("--- LoadDataGrid END ---");
        }

        /// <summary>
        /// Cấu hình tên cột, độ rộng, chế độ hiển thị cho DataGridView.
        /// </summary>
        private void SetupDataGridViewColumns()
        {
            if (dgvLoaiDocGia == null || dgvLoaiDocGia.DataSource == null || dgvLoaiDocGia.Columns.Count == 0) return;

            try
            {
                dgvLoaiDocGia.SuspendLayout(); // Tạm dừng layout để tăng tốc

                var columns = dgvLoaiDocGia.Columns;

                if (columns.Contains(nameof(LoaiDocGiaDTO.Id))) columns[nameof(LoaiDocGiaDTO.Id)].Visible = false;

                if (columns.Contains(nameof(LoaiDocGiaDTO.MaLoaiDocGia)))
                {
                    columns[nameof(LoaiDocGiaDTO.MaLoaiDocGia)].HeaderText = "Mã Loại";
                    columns[nameof(LoaiDocGiaDTO.MaLoaiDocGia)].Width = 150;
                }
                if (columns.Contains(nameof(LoaiDocGiaDTO.TenLoaiDocGia)))
                {
                    columns[nameof(LoaiDocGiaDTO.TenLoaiDocGia)].HeaderText = "Tên Loại Độc Giả";
                    columns[nameof(LoaiDocGiaDTO.TenLoaiDocGia)].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                // --- THÊM CỘT SỐ LƯỢNG ---
                if (columns.Contains(nameof(LoaiDocGiaDTO.SoLuongDocGia)))
                {
                    columns[nameof(LoaiDocGiaDTO.SoLuongDocGia)].HeaderText = "Số ĐG";
                    columns[nameof(LoaiDocGiaDTO.SoLuongDocGia)].Width = 80; // Điều chỉnh độ rộng
                    columns[nameof(LoaiDocGiaDTO.SoLuongDocGia)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Căn giữa
                    columns[nameof(LoaiDocGiaDTO.SoLuongDocGia)].ToolTipText = "Số lượng độc giả thuộc loại này";
                }
                // --- KẾT THÚC THÊM CỘT SỐ LƯỢNG ---

                // Cài đặt chung
                dgvLoaiDocGia.ReadOnly = true;
                dgvLoaiDocGia.AllowUserToAddRows = false;
                dgvLoaiDocGia.AllowUserToDeleteRows = false;
                dgvLoaiDocGia.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvLoaiDocGia.MultiSelect = false;
                dgvLoaiDocGia.AllowUserToResizeRows = false; // Tắt resize chiều cao dòng

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi cấu hình cột DataGridView Loại ĐG: {ex.Message}");
            }
            finally
            {
                if (dgvLoaiDocGia != null) dgvLoaiDocGia.ResumeLayout(); // Bật lại layout
            }
        }

        /// <summary>
        /// Cập nhật biểu tượng sắp xếp trên tiêu đề cột.
        /// </summary>
        private void UpdateSortGlyph()
        {
            if (dgvLoaiDocGia == null) return;
            foreach (DataGridViewColumn column in dgvLoaiDocGia.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
                if (column.DataPropertyName == _sortColumn)
                {
                    column.HeaderCell.SortGlyphDirection = _sortDirection;
                }
            }
        }


        // --- HÀM TIỆN ÍCH TRẠNG THÁI & GIAO DIỆN ---

        private bool IsEditing()
        {
            return !_isAdding && (btnLuu?.Enabled ?? false);
        }

        private void ClearInputFields()
        {
            if (txtId != null) txtId.Clear();
            if (txtMaLoaiDocGia != null) txtMaLoaiDocGia.Clear();
            if (txtTenLoaiDocGia != null) txtTenLoaiDocGia.Clear();

            if (_isAdding)
            {
                txtMaLoaiDocGia?.Focus();
            }
        }

        private void SetControlState(bool isAddingOrEditing, bool rowSelected)
        {
            Debug.WriteLine($"SetControlState: isAddingOrEditing={isAddingOrEditing}, rowSelected={rowSelected}");

            // Actions trên Grid
            if (btnThem != null) btnThem.Enabled = !isAddingOrEditing;
            if (btnSua != null) btnSua.Enabled = !isAddingOrEditing && rowSelected;
            if (btnXoa != null) btnXoa.Enabled = !isAddingOrEditing && rowSelected;
            if (txtTimKiem != null) txtTimKiem.Enabled = !isAddingOrEditing; // Bật/tắt tìm kiếm
            if (btnExportCsv != null) btnExportCsv.Enabled = !isAddingOrEditing && (dgvLoaiDocGia?.Rows.Count > 0); // Bật/tắt xuất

            // Actions trong Details Panel
            if (btnLuu != null) btnLuu.Enabled = isAddingOrEditing;
            if (btnBoQua != null) btnBoQua.Enabled = isAddingOrEditing;

            // Input Fields
            if (txtId != null) txtId.Enabled = false; // ID luôn tắt
            if (txtMaLoaiDocGia != null) txtMaLoaiDocGia.Enabled = false;
            if (txtTenLoaiDocGia != null) txtTenLoaiDocGia.Enabled = isAddingOrEditing; // Tên bật khi Thêm/Sửa

            // DataGridView và Panel chứa nó
            if (dgvLoaiDocGia != null) dgvLoaiDocGia.Enabled = !isAddingOrEditing; // Khóa lưới khi Thêm/Sửa
            if (panelGrid != null) panelGrid.Enabled = !isAddingOrEditing; // Khóa cả panel lưới
        }

        private void DisplaySelectedRow()
        {
            if (_isAdding || IsEditing())
            { Debug.WriteLine("DisplaySelectedRow: Skipping update because currently adding or editing."); return; }

            string idText = string.Empty;
            string maText = string.Empty;
            string tenText = string.Empty;
            _originalDto = null;

            if (dgvLoaiDocGia?.SelectedRows.Count == 1)
            {
                DataGridViewRow selectedRow = dgvLoaiDocGia.SelectedRows[0];
                if (selectedRow.DataBoundItem is LoaiDocGiaDTO dto)
                {
                    idText = dto.Id.ToString();
                    maText = dto.MaLoaiDocGia ?? string.Empty;
                    tenText = dto.TenLoaiDocGia ?? string.Empty;
                    _originalDto = new LoaiDocGiaDTO { Id = dto.Id, MaLoaiDocGia = dto.MaLoaiDocGia, TenLoaiDocGia = dto.TenLoaiDocGia };
                    Debug.WriteLine($"DisplaySelectedRow: _originalDto set (ID: {_originalDto.Id}).");
                }
                else
                {
                    Debug.WriteLine($"Lỗi DisplaySelectedRow: DataBoundItem không phải {typeof(LoaiDocGiaDTO).Name}.");
                    ClearInputFields();
                }
            }
            else
            {
                Debug.WriteLine("DisplaySelectedRow: No single row selected. Clearing input fields.");
                ClearInputFields();
            }

            if (txtId != null) txtId.Text = idText;
            if (txtMaLoaiDocGia != null) txtMaLoaiDocGia.Text = maText;
            if (txtTenLoaiDocGia != null) txtTenLoaiDocGia.Text = tenText;

            // Cập nhật trạng thái controls dựa trên việc có dòng được chọn hay không
            SetControlState(false, dgvLoaiDocGia?.SelectedRows.Count > 0);
        }


        // --- SỰ KIỆN CỦA CÁC BUTTON ---

        private void btnThem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnThem_Click fired.");
            _isAdding = true;
            _originalDto = null;
            ClearInputFields();
            SetControlState(true, false);
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnSua_Click fired.");
            if (!_isAdding && !IsEditing() && dgvLoaiDocGia?.SelectedRows.Count == 1)
            {
                // Đảm bảo _originalDto được lấy đúng
                if (_originalDto == null) DisplaySelectedRow();
                if (_originalDto == null)
                {
                    MaterialMessageBox.Show(this.FindForm(), "Không thể lấy dữ liệu gốc để sửa. Vui lòng chọn lại dòng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.WriteLine("btnSua_Click aborted: Could not get original DTO.");
                    return;
                }

                _isAdding = false; // Đảm bảo không phải đang Thêm
                SetControlState(true, true);
                txtTenLoaiDocGia?.Focus(); // Focus vào Tên để sửa
                Debug.WriteLine("btnSua_Click state set to editing.");
            }
            else if (dgvLoaiDocGia?.SelectedRows.Count != 1)
            {
                MaterialMessageBox.Show(this.FindForm(), "Vui lòng chọn đúng một loại độc giả cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Debug.WriteLine("btnSua_Click aborted: No single row selected.");
            }
        }

        private async void btnLuu_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnLuu_Click fired.");

            // CHỈ cần đọc Tên Loại từ input
            string tenLoai = txtTenLoaiDocGia?.Text.Trim() ?? "";
            // KHÔNG cần đọc Mã Loại ở đây nữa khi thêm mới

            // --- VALIDATION (Phần GUI) ---
            var errors = new List<string>();
            // KHÔNG kiểm tra Mã Loại rỗng khi thêm nữa
            // if (_isAdding && string.IsNullOrWhiteSpace(maLoai)) errors.Add("Mã loại độc giả không được để trống.");
            if (string.IsNullOrWhiteSpace(tenLoai)) errors.Add("Tên loại độc giả không được để trống.");

            // Kiểm tra độ dài Tên
            const int MAX_TEN_LENGTH = 50;
            if (tenLoai.Length > MAX_TEN_LENGTH) errors.Add($"Tên loại không quá {MAX_TEN_LENGTH} ký tự.");

            // Kiểm tra độ dài Mã chỉ khi SỬA (nếu bạn cho phép hiển thị mã gốc khi sửa)
            // const int MAX_MA_LENGTH = 10;
            // string? currentMaDisplay = txtMaLoaiDocGia?.Text; // Lấy mã đang hiển thị (chỉ dùng khi sửa)
            // if (!_isAdding && currentMaDisplay != null && currentMaDisplay.Length > MAX_MA_LENGTH) errors.Add($"Mã loại không quá {MAX_MA_LENGTH} ký tự.");


            if (errors.Count > 0)
            {
                MaterialMessageBox.Show(this.FindForm(), "Vui lòng sửa các lỗi sau:\n- " + string.Join("\n- ", errors), "Lỗi Nhập Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // Focus vào control lỗi đầu tiên
                if (errors[0].Contains("Tên")) txtTenLoaiDocGia?.Focus();
                // else if (errors[0].Contains("Mã")) txtMaLoaiDocGia?.Focus(); // Không focus vào Mã vì đã disable
                return;
            }
            // --- END VALIDATION (GUI) ---

            int currentId = _isAdding ? 0 : (_originalDto?.Id ?? 0);
            // Kiểm tra lại ID khi sửa
            if (!_isAdding && currentId == 0)
            {
                MaterialMessageBox.Show(this.FindForm(), "Không xác định được ID để cập nhật.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tạo DTO để gửi đi
            LoaiDocGiaDTO loaiDocGiaDto = new LoaiDocGiaDTO
            {
                Id = currentId,
                // *** QUAN TRỌNG: Khi thêm mới, không cần gán MaLoaiDocGia ở đây ***
                // MaLoaiDocGia sẽ được sinh ra ở tầng BUS.
                // Khi sửa, Mã gốc đã được lưu trong _originalDto và không cần gửi đi trong DTO này
                // vì tầng BUS sẽ lấy mã gốc khi cập nhật (nếu cần) hoặc DAL không cho sửa Mã.
                MaLoaiDocGia = _isAdding ? null : _originalDto?.MaLoaiDocGia, // Chỉ dùng mã gốc khi sửa để tham khảo (hoặc null khi thêm)
                TenLoaiDocGia = tenLoai
            };


            bool success = false;
            string? errorMsg = null;
            LoaiDocGiaDTO? savedDto = null; // Biến lưu DTO trả về TỪ BUS sau khi lưu thành công

            if (btnLuu != null) btnLuu.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (_isAdding)
                {
                    // *** SỬA ĐỔI QUAN TRỌNG: Giả sử AddLoaiDocGiaAsync trả về DTO đã lưu ***
                    // Cần sửa IBUSLoaiDocGia và BUSLoaiDocGia để Add trả về Task<LoaiDocGiaDTO?>
                    // Task<bool> addResult = await _busLoaiDocGia.AddLoaiDocGiaAsync(loaiDocGiaDto); // Cũ
                    savedDto = await _busLoaiDocGia.AddLoaiDocGiaAsync(loaiDocGiaDto); // Mới (Giả định đã sửa BUS)
                    success = (savedDto != null); // Thành công nếu DTO trả về không null
                }
                else // Đang sửa
                {
                    // *** SỬA ĐỔI QUAN TRỌNG: Giả sử UpdateLoaiDocGiaAsync trả về DTO đã lưu ***
                    // Cần sửa IBUSLoaiDocGia và BUSLoaiDocGia để Update trả về Task<LoaiDocGiaDTO?>
                    // bool updateResult = await _busLoaiDocGia.UpdateLoaiDocGiaAsync(loaiDocGiaDto); // Cũ
                    // Giữ nguyên Mã gốc khi gọi Update
                    loaiDocGiaDto.MaLoaiDocGia = _originalDto?.MaLoaiDocGia;
                    savedDto = await _busLoaiDocGia.UpdateLoaiDocGiaAsync(loaiDocGiaDto); // Mới (Giả định đã sửa BUS)
                    success = (savedDto != null);
                }
            }
            catch (ArgumentException argEx) { errorMsg = $"Dữ liệu nhập không hợp lệ: {argEx.Message}"; }
            catch (InvalidOperationException invOpEx) { errorMsg = $"Lỗi nghiệp vụ: {invOpEx.Message}"; } // Lỗi trùng lặp
            catch (KeyNotFoundException knfEx) { errorMsg = $"Lỗi cập nhật: {knfEx.Message}"; } // Lỗi không tìm thấy ID
            catch (Exception ex) { errorMsg = $"Lỗi hệ thống khi lưu: {ex.Message}"; Debug.WriteLine($"ERROR (btnLuu_Click): {ex.ToString()}"); }
            finally
            {
                this.Cursor = Cursors.Default;
                if (!success && btnLuu != null && !btnLuu.IsDisposed)
                {
                    try { btnLuu.Enabled = true; } catch { /* Ignore */ }
                }
            }

            // Xử lý kết quả
            if (success && savedDto != null) // *** KIỂM TRA savedDto KHÔNG NULL ***
            {
                MaterialMessageBox.Show(this.FindForm(), _isAdding ? "Thêm thành công!" : "Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _isAdding = false;
                _originalDto = null;
                await LoadDataGrid(); // Tải lại lưới

                // Chọn lại dòng vừa thêm/sửa DỰA TRÊN DTO TRẢ VỀ TỪ BUS
                if (savedDto.Id > 0)
                {
                    Debug.WriteLine($"Attempting to select saved row by ID: {savedDto.Id}");
                    SelectRowById(savedDto.Id);
                }
                else if (!string.IsNullOrEmpty(savedDto.MaLoaiDocGia)) // Dự phòng nếu ID không có nhưng Mã có
                {
                    Debug.WriteLine($"Attempting to select saved row by Ma: {savedDto.MaLoaiDocGia}");
                    SelectRowByMa(savedDto.MaLoaiDocGia);
                }
                else
                {
                    Debug.WriteLine("Saved DTO is missing both ID and Ma. Cannot select row.");
                }
            }
            else if (!string.IsNullOrEmpty(errorMsg))
            {
                MaterialMessageBox.Show(this.FindForm(), errorMsg, "Lỗi Lưu Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Focus lại control gây lỗi (đơn giản hóa)
                if (errorMsg.Contains("Tên")) txtTenLoaiDocGia?.Focus();
                // Không focus vào Mã vì nó bị disable
            }
            else
            {
                MaterialMessageBox.Show(this.FindForm(), _isAdding ? "Thêm thất bại." : "Cập nhật thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnBoQua_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnBoQua_Click fired.");

            if ((_isAdding || IsEditing()) && HasUnsavedChanges())
            {
                DialogResult confirm = MaterialMessageBox.Show(this.FindForm(), "Dữ liệu chưa được lưu. Bạn có muốn hủy bỏ thay đổi?", "Xác nhận Hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.No) return;
            }

            _isAdding = false;
            if (btnLuu != null && !btnLuu.IsDisposed)
            {
                try { btnLuu.Enabled = false; } catch { /* Ignore */ }
            }

            DisplaySelectedRow();
        }

        private bool HasUnsavedChanges()
        {
            if (_isAdding)
            {
                return !string.IsNullOrWhiteSpace(txtMaLoaiDocGia?.Text) || !string.IsNullOrWhiteSpace(txtTenLoaiDocGia?.Text);
            }
            else if (IsEditing() && _originalDto != null)
            {
                // Chỉ cần so sánh Tên vì Mã không cho sửa
                return (txtTenLoaiDocGia?.Text.Trim() ?? "") != (_originalDto.TenLoaiDocGia?.Trim() ?? "");
            }
            return false;
        }

        private async void btnXoa_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnXoa_Click fired.");
            if (!_isAdding && !IsEditing() && dgvLoaiDocGia?.SelectedRows.Count == 1)
            {
                if (dgvLoaiDocGia.SelectedRows[0].DataBoundItem is LoaiDocGiaDTO dtoToDelete)
                {
                    if (dtoToDelete.Id <= 0)
                    {
                        MaterialMessageBox.Show(this.FindForm(), "ID không hợp lệ để xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Hỏi xác nhận
                    DialogResult confirm = MaterialMessageBox.Show(
                        this.FindForm(),
                        $"Bạn có chắc chắn muốn xóa loại độc giả '{dtoToDelete.TenLoaiDocGia}' (ID: {dtoToDelete.Id})?",
                        "Xác nhận Xóa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (confirm == DialogResult.Yes)
                    {
                        bool success = false;
                        string? errorMsg = null;
                        SetControlState(false, false); // Khóa tạm thời
                        if (btnXoa != null) btnXoa.Enabled = false;
                        this.Cursor = Cursors.WaitCursor;

                        try
                        {
                            // BUS sẽ kiểm tra ràng buộc trước khi gọi DAL.Delete
                            success = await _busLoaiDocGia.DeleteLoaiDocGiaAsync(dtoToDelete.Id);
                        }
                        catch (InvalidOperationException invalidOpEx) { errorMsg = $"Không thể xóa: {invalidOpEx.Message}"; } // Lỗi ràng buộc
                        catch (Exception ex) { errorMsg = $"Lỗi hệ thống khi xóa: {ex.Message}"; Debug.WriteLine($"ERROR (btnXoa_Click): {ex.ToString()}"); }
                        finally { this.Cursor = Cursors.Default; }

                        // Xử lý kết quả
                        if (success)
                        {
                            MaterialMessageBox.Show(this.FindForm(), "Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            _originalDto = null;
                            await LoadDataGrid(); // Tải lại lưới
                        }
                        else if (!string.IsNullOrEmpty(errorMsg))
                        {
                            MaterialMessageBox.Show(this.FindForm(), errorMsg, "Lỗi Xóa", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            // State sẽ được cập nhật lại bởi LoadDataGrid hoặc DisplaySelectedRow
                        }
                        else
                        {
                            MaterialMessageBox.Show(this.FindForm(), "Xóa thất bại (lý do không xác định).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MaterialMessageBox.Show(this.FindForm(), "Không thể xác định đối tượng cần xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (dgvLoaiDocGia?.SelectedRows.Count != 1)
            {
                MaterialMessageBox.Show(this.FindForm(), "Vui lòng chọn đúng một loại độc giả cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // --- SỰ KIỆN CỦA DATAGRIDVIEW ---

        private void dgvLoaiDocGia_SelectionChanged(object? sender, EventArgs e)
        {
            Debug.WriteLine($"SelectionChanged Fired. IsAdding={_isAdding}, IsEditing={IsEditing()}. SelectedRows.Count={dgvLoaiDocGia?.SelectedRows.Count}");
            if (dgvLoaiDocGia != null && !_isAdding && !IsEditing())
            {
                DisplaySelectedRow();
            }
            else if (dgvLoaiDocGia != null && (_isAdding || IsEditing()))
            {
                Debug.WriteLine("Skipping DisplaySelectedRow because IsAdding or IsEditing is true.");
            }
        }

        private void dgvLoaiDocGia_DoubleClick(object? sender, EventArgs e)
        {
            Debug.WriteLine("dgvLoaiDocGia_DoubleClick fired.");
            // Gọi sự kiện nút Sửa nếu hợp lệ
            if (btnSua != null && btnSua.Enabled) // Kiểm tra nút Sửa có bật không
            {
                Debug.WriteLine("Calling btnSua_Click from DoubleClick.");
                btnSua_Click(sender, e);
            }
        }

        // --- SỰ KIỆN SẮP XẾP ---
        private async void dgvLoaiDocGia_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (_isAdding || IsEditing()) return; // Không sắp xếp khi đang sửa/thêm

            string newSortColumn = dgvLoaiDocGia.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(newSortColumn)) return; // Bỏ qua cột không có DataPropertyName

            Debug.WriteLine($"ColumnHeaderMouseClick: Clicked on '{newSortColumn}'");

            if (_sortColumn == newSortColumn)
            {
                _sortDirection = (_sortDirection == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                _sortColumn = newSortColumn;
                _sortDirection = SortOrder.Ascending;
            }

            await LoadDataGrid(); // Tải lại dữ liệu với sắp xếp mới
        }

        // --- SỰ KIỆN TÌM KIẾM ---
        private async void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            // Chỉ tìm kiếm khi không ở chế độ Thêm/Sửa
            // Cân nhắc thêm debounce timer ở đây để tránh tìm kiếm liên tục
            if (!_isAdding && !IsEditing())
            {
                await LoadDataGrid(); // LoadDataGrid sẽ đọc giá trị từ txtTimKiem
            }
        }

        // --- SỰ KIỆN XUẤT CSV ---
        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            if (dgvLoaiDocGia.Rows.Count == 0)
            {
                MaterialMessageBox.Show(this.FindForm(), "Không có dữ liệu để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV UTF-8 (*.csv)|*.csv",
                Title = "Lưu danh sách Loại Độc Giả",
                FileName = $"DanhSachLoaiDocGia_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(saveFileDialog.FileName))
            {
                try
                {
                    StringBuilder csvContent = new StringBuilder();
                    var visibleColumns = dgvLoaiDocGia.Columns.Cast<DataGridViewColumn>().Where(col => col.Visible).ToList();

                    // Header
                    csvContent.AppendLine(string.Join(",", visibleColumns.Select(col => EscapeCsvField(col.HeaderText))));

                    // Data Rows
                    foreach (DataGridViewRow row in dgvLoaiDocGia.Rows)
                    {
                        if (!row.IsNewRow) // Bỏ qua dòng mới nếu có
                        {
                            csvContent.AppendLine(string.Join(",", visibleColumns.Select(col => EscapeCsvField(row.Cells[col.Index].FormattedValue?.ToString() ?? string.Empty))));
                        }
                    }

                    // Write using UTF8 encoding
                    File.WriteAllText(saveFileDialog.FileName, csvContent.ToString(), Encoding.UTF8);
                    MaterialMessageBox.Show(this.FindForm(), $"Xuất file thành công:\n{saveFileDialog.FileName}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (IOException ioEx) { MaterialMessageBox.Show(this.FindForm(), $"Lỗi ghi file: {ioEx.Message}", "Lỗi Xuất File", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                catch (Exception ex) { MaterialMessageBox.Show(this.FindForm(), $"Lỗi khi xuất CSV: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        // Helper function for CSV export
        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field)) return "";
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }

        // --- HÀM HỖ TRỢ KHÁC ---

        private void SelectRowById(int id)
        {
            Debug.WriteLine($"SelectRowById called with ID: {id}");
            if (dgvLoaiDocGia == null || id <= 0) return;

            dgvLoaiDocGia.ClearSelection();
            DataGridViewRow? rowToSelect = dgvLoaiDocGia.Rows
                .Cast<DataGridViewRow>()
                .FirstOrDefault(row => row.DataBoundItem is LoaiDocGiaDTO dto && dto.Id == id);

            if (rowToSelect != null)
            {
                rowToSelect.Selected = true;
                Debug.WriteLine($"SelectRowById: Row {rowToSelect.Index} selected.");
                ScrollToRow(rowToSelect.Index); // Cuộn đến dòng
                // SelectionChanged sẽ tự động gọi DisplaySelectedRow nếu không ở chế độ Add/Edit
            }
            else
            {
                Debug.WriteLine($"SelectRowById: Row with ID {id} not found.");
                // Nếu không tìm thấy và đang không Add/Edit, xóa input và cập nhật state
                if (!_isAdding && !IsEditing())
                {
                    ClearInputFields();
                    _originalDto = null;
                    SetControlState(false, false);
                }
            }
        }

        private void SelectRowByMa(string? ma)
        {
            Debug.WriteLine($"SelectRowByMa called with Ma: '{ma ?? "null"}'");
            if (dgvLoaiDocGia == null || string.IsNullOrEmpty(ma)) return;

            dgvLoaiDocGia.ClearSelection();
            DataGridViewRow? rowToSelect = dgvLoaiDocGia.Rows
               .Cast<DataGridViewRow>()
               .FirstOrDefault(row => row.DataBoundItem is LoaiDocGiaDTO dto && string.Equals(dto.MaLoaiDocGia, ma, StringComparison.OrdinalIgnoreCase));

            if (rowToSelect != null)
            {
                rowToSelect.Selected = true;
                Debug.WriteLine($"SelectRowByMa: Row {rowToSelect.Index} selected.");
                ScrollToRow(rowToSelect.Index); // Cuộn đến dòng
            }
            else
            {
                Debug.WriteLine($"SelectRowByMa: Row with Ma '{ma}' not found.");
                if (!_isAdding && !IsEditing())
                {
                    ClearInputFields();
                    _originalDto = null;
                    SetControlState(false, false);
                }
            }
        }

        /// <summary>
        /// Cuộn DataGridView đến dòng được chỉ định.
        /// </summary>
        /// <param name="rowIndex">Index của dòng cần cuộn đến.</param>
        private void ScrollToRow(int rowIndex)
        {
            if (dgvLoaiDocGia == null || rowIndex < 0 || rowIndex >= dgvLoaiDocGia.RowCount) return;

            try
            {
                // Đảm bảo dòng không bị che khuất hoàn toàn
                if (!IsRowFullyVisible(rowIndex))
                {
                    // Sử dụng FirstDisplayedScrollingRowIndex là cách an toàn
                    dgvLoaiDocGia.FirstDisplayedScrollingRowIndex = rowIndex;
                    Debug.WriteLine($"ScrollToRow: Scrolled to row {rowIndex}.");
                }
                else
                {
                    Debug.WriteLine($"ScrollToRow: Row {rowIndex} is already visible.");
                }

                // EnsureVisible có thể không hoạt động tốt nếu dòng quá xa
                // dgvLoaiDocGia.CurrentCell = dgvLoaiDocGia.Rows[rowIndex].Cells[0]; // Chọn cell đầu tiên của dòng
                // dgvLoaiDocGia.FirstDisplayedScrollingRowIndex = rowIndex;
            }
            catch (Exception ex) { Debug.WriteLine($"Lỗi khi cuộn DGV đến dòng {rowIndex}: {ex.Message}"); }
        }


        // Hàm helper để kiểm tra dòng có hiển thị đầy đủ không
        private bool IsRowFullyVisible(int rowIndex)
        {
            if (dgvLoaiDocGia == null || rowIndex < 0 || rowIndex >= dgvLoaiDocGia.RowCount) return false;
            // Kiểm tra xem control có đang hiển thị không
            if (!dgvLoaiDocGia.Visible || dgvLoaiDocGia.Width <= 0 || dgvLoaiDocGia.Height <= 0) return false;
            try
            {
                Rectangle displayedRect = dgvLoaiDocGia.DisplayRectangle;
                Rectangle rowRect = dgvLoaiDocGia.GetRowDisplayRectangle(rowIndex, false); // Lấy rect của cả dòng
                return displayedRect.Contains(rowRect); // Kiểm tra xem rect dòng có nằm hoàn toàn trong rect hiển thị không
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in IsRowFullyVisible for index {rowIndex}: {ex.Message}");
                return false; // Giả định là không thấy nếu có lỗi
            }
        }


    } // Kết thúc class ucQuanLyLoaiDocGia
} // Kết thúc namespace GUI