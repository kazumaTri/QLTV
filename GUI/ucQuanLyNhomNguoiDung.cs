// GUI/ucQuanLyNhomNguoiDung.cs
// *** PHIÊN BẢN VIẾT LẠI SAU KHI ĐÃ KHẮC PHỤC LỖI TẢI CHỨC NĂNG VÀ XỬ LÝ DOUBLE CLICK LƯU ***
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BUS; // Cần using BUS namespace
using DTO; // Cần using DTO namespace
using MaterialSkin.Controls; // Cần using MaterialSkin
using System.Diagnostics;

namespace GUI
{
    public partial class ucQuanLyNhomNguoiDung : UserControl, IRequiresDataLoading
    {
        private readonly IBUSNhomNguoiDung _busNhomNguoiDung;
        private readonly IBUSChucNang _busChucNang;
        private bool _isAdding = false;
        private bool _isEditingMode = false;
        private NhomNguoiDungDTO? _currentSelectedDTO = null;
        private List<int>? _originalChucNangIds = null; // Lưu trạng thái quyền ban đầu khi vào chế độ sửa

        public ucQuanLyNhomNguoiDung(IBUSNhomNguoiDung busNhomNguoiDung, IBUSChucNang busChucNang)
        {
            InitializeComponent();
            _busNhomNguoiDung = busNhomNguoiDung ?? throw new ArgumentNullException(nameof(busNhomNguoiDung));
            _busChucNang = busChucNang ?? throw new ArgumentNullException(nameof(busChucNang));

            // Gắn sự kiện
            this.dgvNhomNguoiDung.SelectionChanged += dgvNhomNguoiDung_SelectionChanged;
            this.dgvNhomNguoiDung.DoubleClick += dgvNhomNguoiDung_DoubleClick;
            this.btnThem.Click += btnThem_Click;
            this.btnSua.Click += btnSua_Click;
            this.btnXoa.Click += btnXoa_Click;
            this.btnLuu.Click += btnLuu_Click;
            this.btnBoQua.Click += btnBoQua_Click;
            // Gắn sự kiện Changed cho các input field để cập nhật trạng thái nút Lưu
            this.txtMaNhom.TextChanged += InputField_Changed;
            this.txtTenNhom.TextChanged += InputField_Changed;
            // Gắn sự kiện ItemCheck cho CheckedListBox để cập nhật trạng thái nút Lưu
            this.clbChucNang.ItemCheck += clbChucNang_ItemCheck;

            // Khởi tạo trạng thái UI ban đầu
            UpdateUIState(); // Gọi lần đầu để setup nút
        }

        // Phương thức được gọi khi UserControl được hiển thị lần đầu hoặc cần refresh toàn bộ
        public async Task InitializeDataAsync()
        {
            Debug.WriteLine("ucQuanLyNhomNguoiDung.InitializeDataAsync() called.");
            this.Cursor = Cursors.WaitCursor;
            try
            {
                // Reset trạng thái
                _isAdding = false;
                _isEditingMode = false;
                _currentSelectedDTO = null;
                _originalChucNangIds = null; // Đảm bảo quyền gốc cũng được reset

                ClearInputFields(); // Xóa sạch các trường nhập liệu
                await LoadChucNangListAsync(); // Tải danh sách tất cả chức năng
                await LoadDataGrid(); // Tải danh sách nhóm người dùng (sẽ gọi UpdateUIState ở cuối)
            }
            catch (Exception ex)
            {
                HandleError("khởi tạo dữ liệu", ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                // UpdateUIState được gọi ở cuối LoadDataGrid
            }
        }

        // Tải danh sách tất cả chức năng (quyền) vào CheckedListBox
        private async Task LoadChucNangListAsync()
        {
            Debug.WriteLine("LoadChucNangListAsync() called.");
            if (clbChucNang == null) return;

            // Bỏ gắn sự kiện tạm thời để tránh trigger ItemCheck khi load dữ liệu
            clbChucNang.ItemCheck -= clbChucNang_ItemCheck;

            try
            {
                var allChucNang = await _busChucNang.GetAllChucNangAsync();
                clbChucNang.DataSource = null; // Xóa DataSource cũ
                clbChucNang.Items.Clear(); // Xóa các mục cũ

                if (allChucNang != null && allChucNang.Any())
                {
                    // Gán danh sách DTO làm DataSource
                    clbChucNang.DisplayMember = nameof(ChucNangDTO.TenChucNang); // Tên thuộc tính hiển thị
                    clbChucNang.ValueMember = nameof(ChucNangDTO.Id); // Tên thuộc tính làm giá trị
                    clbChucNang.DataSource = allChucNang.ToList(); // Cần .ToList() nếu allChucNang là IEnumerable
                    Debug.WriteLine($"LoadChucNangListAsync: Loaded {clbChucNang.Items.Count} functions.");
                }
                else
                {
                    Debug.WriteLine("LoadChucNangListAsync: No functions loaded from BUS.");
                }

                UncheckAllChucNangItems(); // Đảm bảo tất cả đều unchecked ban đầu
            }
            catch (Exception ex)
            {
                HandleError("tải danh sách chức năng", ex);
                Debug.WriteLine($"LoadChucNangListAsync ERROR: {ex.Message}");
                clbChucNang.DataSource = null; // Đảm bảo trống nếu có lỗi
                clbChucNang.Items.Clear();
            }
            finally
            {
                // Gắn lại sự kiện ItemCheck sau khi load xong
                clbChucNang.ItemCheck += clbChucNang_ItemCheck;
            }
        }

        // Tải danh sách nhóm người dùng vào DataGridView
        private async Task LoadDataGrid()
        {
            Debug.WriteLine("LoadDataGrid() called.");
            // Lưu lại ID của dòng đang chọn trước khi load lại để có thể chọn lại sau
            int? selectedId = _currentSelectedDTO?.Id;

            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (dgvNhomNguoiDung == null) return;
                dgvNhomNguoiDung.DataSource = null; // Xóa DataSource cũ

                var danhSach = await _busNhomNguoiDung.GetAllNhomNguoiDungAsync();
                dgvNhomNguoiDung.DataSource = danhSach?.ToList(); // Gán danh sách DTO

                SetupDataGridViewColumns(); // Cấu hình hiển thị cột

                // Sau khi load xong grid, cố gắng chọn lại dòng đã được chọn trước đó
                if (selectedId.HasValue && danhSach != null && danhSach.Any(n => n.Id == selectedId.Value))
                {
                    SelectRowById(selectedId.Value); // SelectRowById sẽ trigger SelectionChanged -> DisplaySelectedRow
                }
                else
                {
                    // Nếu không có dòng nào được chọn trước đó, hoặc dòng đó không còn tồn tại,
                    // hoặc LoadDataGrid trả về rỗng, thì xóa trắng input fields và clear selection
                    dgvNhomNguoiDung.ClearSelection();
                    // ClearInputFields() sẽ được gọi bởi DisplaySelectedRow() khi không có dòng nào được chọn
                }
            }
            catch (Exception ex)
            {
                HandleError("tải danh sách nhóm người dùng", ex);
                ClearInputFields(); // Nếu có lỗi, cũng xóa trắng input
            }
            finally
            {
                this.Cursor = Cursors.Default;
                // Gọi UpdateUIState sau khi grid đã load xong và dòng đã được chọn (hoặc không)
                UpdateUIState();
            }
        }

        // Cấu hình hiển thị các cột trong DataGridView
        private void SetupDataGridViewColumns()
        {
            Debug.WriteLine("SetupDataGridViewColumns() called.");
            if (dgvNhomNguoiDung == null || dgvNhomNguoiDung.DataSource == null || dgvNhomNguoiDung.Columns.Count == 0) return;

            try
            {
                var columns = dgvNhomNguoiDung.Columns;
                // Ẩn cột Id (khóa chính)
                if (columns.Contains("Id")) columns["Id"].Visible = false;

                // Đặt HeaderText và chiều rộng cho các cột
                if (columns.Contains("MaNhomNguoiDung")) { columns["MaNhomNguoiDung"].HeaderText = "Mã Nhóm"; columns["MaNhomNguoiDung"].Width = 150; }
                if (columns.Contains("TenNhomNguoiDung")) { columns["TenNhomNguoiDung"].HeaderText = "Tên Nhóm Người Dùng"; columns["TenNhomNguoiDung"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; }
                // Có thể thêm cấu hình cho các cột khác nếu có

                // Cấu hình DataGridView
                dgvNhomNguoiDung.ReadOnly = true; // Không cho phép sửa trực tiếp trên grid
                dgvNhomNguoiDung.AllowUserToAddRows = false; // Không cho phép thêm dòng trống
                dgvNhomNguoiDung.AllowUserToDeleteRows = false; // Không cho phép xóa dòng bằng phím Delete
                dgvNhomNguoiDung.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Chọn toàn bộ dòng
                dgvNhomNguoiDung.MultiSelect = false; // Chỉ cho phép chọn một dòng
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi cấu hình cột DGV Nhóm Người Dùng: {ex.Message}");
                // HandleError("cấu hình lưới hiển thị", ex); // Không cần báo lỗi UI cho lỗi cấu hình cột nhỏ
            }
        }

        // Xóa trắng các trường nhập liệu
        private void ClearInputFields()
        {
            Debug.WriteLine("ClearInputFields() called.");
            if (txtId != null) txtId.Clear();
            if (txtMaNhom != null) txtMaNhom.Clear();
            if (txtTenNhom != null) txtTenNhom.Clear();
            UncheckAllChucNangItems(); // Bỏ tích tất cả các quyền
            _originalChucNangIds = null; // Reset quyền gốc
        }

        // Bỏ tích tất cả các mục trong CheckedListBox Quyền Chức Năng
        private void UncheckAllChucNangItems()
        {
            if (clbChucNang == null || clbChucNang.Items.Count == 0) return;

            // Bỏ gắn sự kiện tạm thời để tránh trigger ItemCheck khi uncheck
            clbChucNang.ItemCheck -= clbChucNang_ItemCheck;
            for (int i = 0; i < clbChucNang.Items.Count; i++)
            {
                clbChucNang.SetItemChecked(i, false);
            }
            // Gắn lại sự kiện sau khi hoàn thành
            clbChucNang.ItemCheck += clbChucNang_ItemCheck;
        }

        // Cập nhật trạng thái bật/tắt của các nút và trường nhập liệu dựa trên trạng thái hiện tại (_isAdding, _isEditingMode)
        private void UpdateUIState()
        {
            // Kiểm tra các control cần thiết có tồn tại không
            if (dgvNhomNguoiDung == null || btnThem == null || btnSua == null || btnXoa == null || btnLuu == null || btnBoQua == null || txtMaNhom == null || txtTenNhom == null || clbChucNang == null) return;

            bool rowSelected = dgvNhomNguoiDung.SelectedRows.Count > 0;
            bool isAddingOrEditing = _isAdding || _isEditingMode; // Đang ở chế độ thêm hoặc sửa

            // Các nút hành động chính
            btnThem.Enabled = !isAddingOrEditing; // Chỉ bật Thêm khi không thêm và không sửa
            btnSua.Enabled = !_isAdding && !_isEditingMode && rowSelected; // Chỉ bật Sửa khi không thêm, không sửa VÀ có dòng chọn
            btnXoa.Enabled = !_isAdding && !_isEditingMode && rowSelected; // Chỉ bật Xóa khi không thêm, không sửa VÀ có dòng chọn

            // Nút Lưu chỉ bật khi đang thêm/sửa VÀ có thay đổi
            btnLuu.Enabled = isAddingOrEditing && HasUnsavedChanges();
            // Nút Bỏ qua chỉ bật khi đang thêm hoặc sửa
            btnBoQua.Enabled = isAddingOrEditing;

            // Các trường nhập liệu và danh sách quyền chỉ bật khi đang thêm hoặc sửa
            txtMaNhom.Enabled = isAddingOrEditing;
            txtTenNhom.Enabled = isAddingOrEditing;
            clbChucNang.Enabled = isAddingOrEditing;

            // DataGridView chỉ bật khi không ở chế độ thêm hoặc sửa (để ngăn chọn dòng khác khi đang thao tác)
            dgvNhomNguoiDung.Enabled = !isAddingOrEditing;

            Debug.WriteLine($"UpdateUIState: Mode: {(isAddingOrEditing ? (_isAdding ? "Adding" : "Editing") : "Viewing")}, RowSelected: {rowSelected}, btnLuu.Enabled: {btnLuu.Enabled}");
        }

        // Hiển thị thông tin của nhóm được chọn trong DataGridView lên các trường nhập liệu
        private async void DisplaySelectedRow()
        {
            Debug.WriteLine("DisplaySelectedRow() called.");
            // Chỉ hiển thị dữ liệu lên các control nếu không đang trong chế độ thêm hoặc sửa
            // vì các control đó đang được sử dụng để nhập/sửa dữ liệu mới
            if (_isAdding || _isEditingMode)
            {
                Debug.WriteLine("DisplaySelectedRow: In add/edit mode, skipping data display.");
                return; // Không cập nhật UI input khi đang thêm/sửa
            }

            _currentSelectedDTO = null; // Reset DTO đang chọn
            _originalChucNangIds = null; // Reset quyền gốc

            // Kiểm tra xem có dòng nào được chọn và dữ liệu có hợp lệ không
            if (dgvNhomNguoiDung?.SelectedRows.Count == 1 && dgvNhomNguoiDung.SelectedRows[0].DataBoundItem is NhomNguoiDungDTO dto)
            {
                _currentSelectedDTO = dto; // Gán DTO của dòng đang chọn
                // Hiển thị thông tin cơ bản
                if (txtId != null) txtId.Text = dto.Id.ToString();
                if (txtMaNhom != null) txtMaNhom.Text = dto.MaNhomNguoiDung ?? "";
                if (txtTenNhom != null) txtTenNhom.Text = dto.TenNhomNguoiDung ?? "";

                // Tải và hiển thị phân quyền cho nhóm này
                await DisplayPhanQuyenAsync(dto.Id);
            }
            else
            {
                // Nếu không có dòng nào hợp lệ được chọn (ví dụ: clear selection), xóa trắng input
                Debug.WriteLine("DisplaySelectedRow: No valid row selected, clearing fields.");
                ClearInputFields(); // Sẽ uncheck clbChucNang và reset _originalChucNangIds
            }
            // UpdateUIState sẽ được gọi bởi SelectionChanged
        }

        // Tải và hiển thị phân quyền cho một nhóm cụ thể vào CheckedListBox
        private async Task DisplayPhanQuyenAsync(int nhomId)
        {
            Debug.WriteLine($"DisplayPhanQuyenAsync({nhomId}) called.");
            // Kiểm tra control và ID hợp lệ
            if (clbChucNang == null || nhomId <= 0)
            {
                UncheckAllChucNangItems();
                _originalChucNangIds = null;
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            List<int>? assignedIds = null;
            try
            {
                // Lấy danh sách ID các chức năng được gán cho nhóm này từ BUS
                assignedIds = await _busNhomNguoiDung.GetChucNangIdsByNhomAsync(nhomId);
                // Lưu danh sách ID này lại để kiểm tra thay đổi khi sửa
                _originalChucNangIds = assignedIds ?? new List<int>();

                // Bỏ gắn sự kiện tạm thời để tránh trigger ItemCheck
                clbChucNang.ItemCheck -= clbChucNang_ItemCheck;

                // Bỏ tích tất cả các mục trước khi tích lại theo phân quyền
                UncheckAllChucNangItems(); // Đảm bảo các mục đã bị uncheck hoàn toàn

                // Tích chọn các mục chức năng có trong danh sách assignedIds
                for (int i = 0; i < clbChucNang.Items.Count; i++)
                {
                    // Lấy ChucNangDTO từ mục trong CheckedListBox
                    if (clbChucNang.Items[i] is ChucNangDTO cnDto)
                    {
                        // Kiểm tra xem ID của chức năng này có trong danh sách assignedIds không
                        bool isChecked = assignedIds?.Contains(cnDto.Id) ?? false;
                        clbChucNang.SetItemChecked(i, isChecked);
                    }
                }
                Debug.WriteLine($"DisplayPhanQuyenAsync: Set checked state for {assignedIds?.Count ?? 0} functions for group {nhomId}.");
            }
            catch (Exception ex)
            {
                HandleError($"lấy danh sách quyền cho nhóm ID {nhomId}", ex);
                UncheckAllChucNangItems(); // Nếu lỗi, bỏ tích tất cả
                _originalChucNangIds = null;
            }
            finally
            {
                // Gắn lại sự kiện ItemCheck sau khi load xong
                clbChucNang.ItemCheck += clbChucNang_ItemCheck;
                this.Cursor = Cursors.Default;
                // UpdateUIState sẽ được gọi bởi SelectionChanged
            }
        }

        // Chọn một dòng trong DataGridView theo ID
        private void SelectRowById(int id)
        {
            Debug.WriteLine($"SelectRowById({id}) called.");
            // Kiểm tra control và dữ liệu
            if (dgvNhomNguoiDung == null || dgvNhomNguoiDung.Rows.Count == 0 || dgvNhomNguoiDung.DataSource == null)
            {
                dgvNhomNguoiDung?.ClearSelection(); // Trigger SelectionChanged -> ClearInputFields
                return;
            }

            dgvNhomNguoiDung.ClearSelection(); // Bỏ chọn tất cả để đảm bảo SelectionChanged event fired
            bool found = false;
            foreach (DataGridViewRow row in dgvNhomNguoiDung.Rows)
            {
                // Lấy DTO từ dòng và so sánh Id
                if (row.DataBoundItem is NhomNguoiDungDTO dto && dto.Id == id)
                {
                    row.Selected = true; // Chọn dòng (sẽ trigger SelectionChanged)
                    // Cuộn đến dòng được chọn nếu nó không hiển thị
                    if (!row.Displayed)
                    {
                        try { if (row.Index >= 0 && row.Index < dgvNhomNguoiDung.RowCount) dgvNhomNguoiDung.FirstDisplayedScrollingRowIndex = row.Index; }
                        catch (Exception ex) { Debug.WriteLine($"Error scrolling to row: {ex.Message}"); }
                    }
                    found = true;
                    break;
                }
            }
            Debug.WriteLine($"SelectRowById({id}): Found = {found}");
            // SelectionChanged sẽ được gọi dù tìm thấy hay không (khi ClearSelection hoặc khi set row.Selected = true)
        }

        // Kiểm tra xem có thay đổi nào chưa lưu không (dùng cho nút Lưu và Bỏ qua)
        private bool HasUnsavedChanges()
        {
            // Nếu đang thêm mới
            if (_isAdding)
            {
                // Có thay đổi nếu tên nhóm đã được nhập HOẶC có quyền đã được tích
                bool basicInfoEntered = !string.IsNullOrWhiteSpace(txtTenNhom?.Text);
                bool permissionsSelected = GetCheckedChucNangIds().Any();
                return basicInfoEntered || permissionsSelected;
            }

            // Nếu đang sửa
            if (_currentSelectedDTO == null || !_isEditingMode) return false; // Chỉ kiểm tra khi đang sửa thực sự và có DTO gốc

            // Kiểm tra thay đổi thông tin cơ bản
            bool basicInfoChanged = false;
            if (txtMaNhom != null && (txtMaNhom.Text.Trim() ?? "") != (_currentSelectedDTO.MaNhomNguoiDung ?? "")) basicInfoChanged = true;
            if (!basicInfoChanged && txtTenNhom != null && (txtTenNhom.Text.Trim() ?? "") != (_currentSelectedDTO.TenNhomNguoiDung ?? "")) basicInfoChanged = true;

            // Kiểm tra thay đổi phân quyền
            bool permissionsChanged = false;
            if (clbChucNang != null && _originalChucNangIds != null)
            {
                var currentCheckedIds = GetCheckedChucNangIds();
                // So sánh tập hợp các ID quyền hiện tại với tập hợp các ID quyền gốc
                permissionsChanged = !(new HashSet<int>(_originalChucNangIds).SetEquals(currentCheckedIds));
            }
            // Nếu _originalChucNangIds là null (trường hợp bất thường khi sửa), coi như có thay đổi nếu có bất kỳ quyền nào được tích
            else if (clbChucNang != null) permissionsChanged = GetCheckedChucNangIds().Any();


            Debug.WriteLine($"HasUnsavedChanges (Editing): BasicChanged={basicInfoChanged}, PermissionsChanged={permissionsChanged}");
            return basicInfoChanged || permissionsChanged; // Có thay đổi nếu thông tin cơ bản HOẶC phân quyền thay đổi
        }

        // Lấy danh sách ID các chức năng hiện đang được tích chọn trong CheckedListBox
        private List<int> GetCheckedChucNangIds()
        {
            var checkedIds = new List<int>();
            if (clbChucNang == null || clbChucNang.Items.Count == 0) return checkedIds;

            // Duyệt qua các mục được tích chọn
            foreach (var item in clbChucNang.CheckedItems)
            {
                // Ép kiểu sang ChucNangDTO và lấy Id
                if (item is ChucNangDTO cnDto)
                {
                    checkedIds.Add(cnDto.Id);
                }
            }
            return checkedIds;
        }

        // --- Event Handlers ---

        // Xử lý khi nội dung của các trường nhập liệu thay đổi
        private void InputField_Changed(object sender, EventArgs e)
        {
            // Chỉ cập nhật trạng thái UI (nút Lưu) nếu đang trong mode thêm hoặc sửa
            if (_isAdding || _isEditingMode)
            {
                UpdateUIState();
            }
        }

        // Xử lý khi trạng thái tích chọn của một mục trong CheckedListBox thay đổi
        private void clbChucNang_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            Debug.WriteLine($"clbChucNang_ItemCheck: Item Index={e.Index}, NewValue={e.NewValue}");
            // Dùng BeginInvoke để đảm bảo cập nhật UIState sau khi trạng thái check đã thực sự thay đổi
            this.BeginInvoke((MethodInvoker)delegate {
                // Chỉ cập nhật trạng thái UI (nút Lưu) nếu đang trong mode thêm hoặc sửa
                if (_isAdding || _isEditingMode)
                {
                    UpdateUIState();
                }
            });
        }

        // Xử lý khi nhấn nút "Thêm"
        private void btnThem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnThem_Click");
            // Kiểm tra xem có thay đổi chưa lưu ở chế độ sửa không
            if (_isEditingMode && HasUnsavedChanges())
            {
                if (ShowConfirm("Bạn có thay đổi chưa lưu. Thêm mới và hủy thay đổi?") == DialogResult.No) return;
            }
            // Chuyển sang chế độ thêm mới
            _isAdding = true;
            _isEditingMode = false;
            _currentSelectedDTO = null; // Không có DTO đang chọn ở chế độ thêm
            _originalChucNangIds = null; // Không có quyền gốc ở chế độ thêm
            ClearInputFields(); // Xóa trắng các trường
            dgvNhomNguoiDung?.ClearSelection(); // Bỏ chọn dòng trên grid
            UpdateUIState(); // Cập nhật trạng thái UI cho chế độ thêm
            if (txtTenNhom != null && txtTenNhom.Enabled) txtTenNhom.Focus(); // Focus vào ô tên nhóm nếu nó được bật
        }

        // Xử lý khi nhấn nút "Sửa"
        private void btnSua_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnSua_Click");
            // Chỉ xử lý nếu có đúng một dòng được chọn và nút Sửa đang enabled
            if (dgvNhomNguoiDung?.SelectedRows.Count == 1 && btnSua.Enabled)
            {
                // Lấy DTO từ dòng được chọn (nếu chưa có)
                if (_currentSelectedDTO == null || !(_currentSelectedDTO.Id == (dgvNhomNguoiDung.SelectedRows[0].DataBoundItem as NhomNguoiDungDTO)?.Id))
                {
                    // Nếu DTO hiện tại không khớp với dòng được chọn mới, hoặc chưa có DTO
                    // Cần gọi lại DisplaySelectedRow để load đúng dữ liệu và quyền gốc
                    // Mặc dù DisplaySelectedRow là async void, nhưng nó được gọi từ SelectionChanged
                    // khi một dòng được chọn. Khi bấm Sửa, dòng đã được chọn rồi, nên DisplaySelectedRow
                    // đã chạy xong hoặc đang chạy. Tuy nhiên, để đảm bảo, ta kiểm tra _currentSelectedDTO
                    // và _originalChucNangIds có được load đầy đủ chưa.

                    if (dgvNhomNguoiDung.SelectedRows[0].DataBoundItem is NhomNguoiDungDTO dtoToEdit)
                    {
                        // Nếu DisplaySelectedRow chưa chạy xong hoặc không chạy, load lại thủ công
                        _currentSelectedDTO = dtoToEdit;
                        if (txtId != null) txtId.Text = dtoToEdit.Id.ToString();
                        if (txtMaNhom != null) txtMaNhom.Text = dtoToEdit.MaNhomNguoiDung ?? "";
                        if (txtTenNhom != null) txtTenNhom.Text = dtoToEdit.TenNhomNguoiDung ?? "";
                        // Tải quyền gốc và chờ hoàn thành
                        try
                        {
                            // *** CẨN THẬN VỚI .Wait() - Có thể gây Deadlock trong môi trường WinForms nếu không đúng context ***
                            // Cách an toàn hơn là refactor DisplayPhanQuyenAsync để không phụ thuộc vào UI thread
                            // hoặc đảm bảo luồng gọi là background thread. Tạm giữ lại .Wait() như phiên bản trước
                            // nếu nó hoạt động, nhưng lưu ý đây là điểm cần cải thiện.
                            Task.Run(async () => await DisplayPhanQuyenAsync(dtoToEdit.Id)).Wait();
                        }
                        catch (Exception ex)
                        {
                            HandleError($"tải quyền gốc cho nhóm ID {dtoToEdit.Id}", ex);
                            // Không thể sửa nếu không tải được quyền
                            _currentSelectedDTO = null;
                            _originalChucNangIds = null;
                            ClearInputFields();
                            UpdateUIState(); // Cập nhật lại nút
                            return;
                        }

                    }
                    else
                    {
                        ShowError("Không thể lấy thông tin nhóm để sửa.");
                        UpdateUIState(); // Cập nhật lại nút
                        return;
                    }
                }

                // Nếu đã đảm bảo _currentSelectedDTO và _originalChucNangIds đã được load
                if (_currentSelectedDTO != null && _originalChucNangIds != null)
                {
                    // Chuyển sang chế độ sửa
                    _isAdding = false;
                    _isEditingMode = true;
                    UpdateUIState(); // Cập nhật trạng thái UI cho chế độ sửa
                    if (txtTenNhom != null && txtTenNhom.Enabled) txtTenNhom.Focus(); // Focus vào ô tên nhóm nếu nó được bật
                }
                else
                {
                    // Trường hợp bất thường
                    ShowError("Không xác định được thông tin nhóm cần sửa.");
                    UpdateUIState();
                }
            }
            else if (dgvNhomNguoiDung?.SelectedRows.Count == 0)
            {
                ShowWarning("Vui lòng chọn nhóm người dùng cần sửa.");
            }
        }

        // Xử lý khi nhấn nút "Lưu"
        private async void btnLuu_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnLuu_Click");

            // *** Bước 1: Vô hiệu hóa nút Lưu và các nút khác ngay lập tức ***
            // Điều này ngăn chặn việc click đúp hoặc click liên tục
            // Lưu lại trạng thái các control để khôi phục nếu có lỗi
            bool originalState_btnLuu = btnLuu?.Enabled ?? false;
            bool originalState_btnBoQua = btnBoQua?.Enabled ?? false;
            bool originalState_txtMaNhom = txtMaNhom?.Enabled ?? false;
            bool originalState_txtTenNhom = txtTenNhom?.Enabled ?? false;
            bool originalState_clbChucNang = clbChucNang?.Enabled ?? false;
            bool originalState_dgv = dgvNhomNguoiDung?.Enabled ?? false;

            if (btnLuu != null) btnLuu.Enabled = false;
            if (btnBoQua != null) btnBoQua.Enabled = false; // Vô hiệu hóa luôn bỏ qua
            if (txtMaNhom != null) txtMaNhom.Enabled = false; // Vô hiệu hóa input
            if (txtTenNhom != null) txtTenNhom.Enabled = false;
            if (clbChucNang != null) clbChucNang.Enabled = false;
            if (dgvNhomNguoiDung != null) dgvNhomNguoiDung.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            // --- Kết thúc vô hiệu hóa UI tạm thời ---

            // *** Bước 2: Lấy dữ liệu từ UI và validate ***
            string? maNhom = txtMaNhom?.Text.Trim();
            string tenNhom = txtTenNhom?.Text.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(tenNhom))
            {
                ShowWarning("Tên nhóm người dùng không được để trống.");
                txtTenNhom?.Focus();
                // Khôi phục UI và thoát
                RestoreUIState(originalState_btnLuu, originalState_btnBoQua, originalState_txtMaNhom, originalState_txtTenNhom, originalState_clbChucNang, originalState_dgv);
                return;
            }

            int currentId = 0;
            bool isUpdating = _isEditingMode;
            if (isUpdating)
            {
                // Nếu đang sửa nhưng không có DTO gốc, đây là lỗi logic
                if (_currentSelectedDTO == null)
                {
                    ShowError("Lỗi hệ thống: Không xác định được nhóm người dùng cần cập nhật.");
                    RestoreUIState(originalState_btnLuu, originalState_btnBoQua, originalState_txtMaNhom, originalState_txtTenNhom, originalState_clbChucNang, originalState_dgv);
                    return;
                }
                currentId = _currentSelectedDTO.Id;
            }

            NhomNguoiDungDTO dtoToSave = new NhomNguoiDungDTO
            {
                Id = currentId,
                MaNhomNguoiDung = string.IsNullOrWhiteSpace(maNhom) ? null : maNhom,
                TenNhomNguoiDung = tenNhom
            };

            List<int> checkedChucNangIds = GetCheckedChucNangIds();

            // *** Bước 3: Thực hiện lưu/cập nhật bất đồng bộ ***
            bool basicInfoSuccess = false;
            bool permissionSuccess = false; // Sẽ cập nhật lại sau
            string? basicInfoError = null;
            string? permissionError = null;
            int? savedId = null; // ID của nhóm sau khi lưu thành công

            try
            {
                if (_isAdding) // --- THÊM MỚI ---
                {
                    Debug.WriteLine("Attempting to add new group.");
                    var addedDto = await _busNhomNguoiDung.AddNhomNguoiDungAsync(dtoToSave);
                    if (addedDto != null)
                    {
                        basicInfoSuccess = true;
                        savedId = addedDto.Id; // Lấy ID mới được gán
                        Debug.WriteLine($"Added group with ID: {savedId}");
                    }
                    else
                    {
                        basicInfoError = "Thêm thông tin nhóm thất bại.";
                        // Nếu thêm nhóm thất bại, không thể lưu quyền
                        permissionSuccess = false;
                        permissionError = "Không thể cập nhật quyền do thêm nhóm thất bại.";
                    }
                }
                else if (_isEditingMode) // --- CẬP NHẬT ---
                {
                    Debug.WriteLine($"Attempting to update group ID: {currentId}");
                    savedId = currentId; // ID đã có sẵn khi cập nhật

                    // --- CẬP NHẬT THÔNG TIN CƠ BẢN (Chỉ khi có thay đổi) ---
                    bool hasBasicChange = false;
                    if (_currentSelectedDTO != null)
                    {
                        if ((txtMaNhom?.Text.Trim() ?? "") != (_currentSelectedDTO.MaNhomNguoiDung ?? "")) hasBasicChange = true;
                        if (!hasBasicChange && (txtTenNhom?.Text.Trim() ?? "") != (_currentSelectedDTO.TenNhomNguoiDung ?? "")) hasBasicChange = true;
                    }

                    if (hasBasicChange)
                    {
                        Debug.WriteLine("Updating basic info...");
                        basicInfoSuccess = await _busNhomNguoiDung.UpdateNhomNguoiDungAsync(dtoToSave);
                        if (!basicInfoSuccess)
                        {
                            basicInfoError = "Cập nhật thông tin nhóm thất bại.";
                        }
                    }
                    else
                    {
                        Debug.WriteLine("No basic info changes detected, skipping update.");
                        basicInfoSuccess = true; // Coi như thành công vì không có gì để làm
                    }
                }
                // --- CẬP NHẬT PHÂN QUYỀN (Luôn thực hiện khi lưu thành công thông tin cơ bản hoặc đang ở chế độ sửa) ---
                // Chỉ cố gắng cập nhật quyền nếu savedId đã có (tức là đã thêm thành công hoặc đang sửa)
                if (savedId.HasValue)
                {
                    Debug.WriteLine($"Updating permissions for group {savedId}...");
                    try
                    {
                        // Luôn gọi UpdatePhanQuyenAsync để đồng bộ hóa trạng thái checked list với database
                        // Hàm BUS/DAL cần xử lý trường hợp list rỗng (xóa hết quyền)
                        permissionSuccess = await _busNhomNguoiDung.UpdatePhanQuyenAsync(savedId.Value, checkedChucNangIds);
                        if (!permissionSuccess)
                        {
                            // Lỗi này ít khả năng xảy ra nếu nhóm tồn tại, trừ khi có lỗi DB khác
                            permissionError = "Cập nhật quyền thất bại.";
                        }
                        Debug.WriteLine($"Permission update for group {savedId} result: {permissionSuccess}");
                    }
                    catch (Exception pEx)
                    {
                        permissionSuccess = false;
                        permissionError = $"Lỗi khi cập nhật quyền: {pEx.Message}";
                        Debug.WriteLine($"ERROR updating permissions: {pEx}");
                    }
                }
                else
                {
                    // Trường hợp này xảy ra nếu _isAdding là true nhưng basicInfoSuccess là false
                    permissionSuccess = false; // Không thể cập nhật quyền nếu thêm nhóm thất bại
                    permissionError = (permissionError == null) ? "Không thể cập nhật quyền do lỗi trước đó." : permissionError;
                }

            } // Kết thúc try block lớn
            catch (Exception ex) // Bắt các lỗi khác ngoài logic xử lý từng bước
            {
                string generalError = $"Lỗi hệ thống trong quá trình lưu: {ex.Message}";
                if (ex.InnerException != null) generalError += $"\nChi tiết: {ex.InnerException.Message}";
                ShowError(generalError);
                Debug.WriteLine($"btnLuu_Click UNHANDLED ERROR: {ex}");

                // Khôi phục UI và thoát
                RestoreUIState(originalState_btnLuu, originalState_btnBoQua, originalState_txtMaNhom, originalState_txtTenNhom, originalState_clbChucNang, originalState_dgv);
                this.Cursor = Cursors.Default; // Đảm bảo cursor trở lại trạng thái bình thường
                return; // Thoát khỏi hàm
            }

            // *** Bước 4: Xử lý kết quả và cập nhật UI cuối cùng ***

            // Xác định kết quả chung: thành công nếu cả thông tin cơ bản và quyền đều thành công
            // (hoặc các bước không cần làm cũng coi là thành công)
            bool overallSuccess = basicInfoSuccess && permissionSuccess;

            if (overallSuccess)
            {
                ShowInfo(_isAdding ? "Thêm nhóm người dùng và quyền thành công!" : "Cập nhật nhóm người dùng và quyền thành công!");

                // Reset trạng thái về xem/chọn
                _isAdding = false;
                _isEditingMode = false;
                _currentSelectedDTO = null;
                _originalChucNangIds = null;

                ClearInputFields(); // Xóa trắng các trường sau khi lưu thành công
                await LoadDataGrid(); // Tải lại DataGridView (sẽ gọi UpdateUIState cuối cùng)

                // Chọn lại dòng vừa lưu nếu ID đã có
                if (savedId.HasValue)
                {
                    SelectRowById(savedId.Value);
                }
                // Không cần gọi UpdateUIState() ở đây vì LoadDataGrid đã gọi
            }
            else // Nếu có bất kỳ bước nào thất bại
            {
                // Xây dựng thông báo lỗi chi tiết hơn
                StringBuilder finalErrorMessage = new StringBuilder("Lưu thất bại.");
                if (basicInfoError != null) finalErrorMessage.AppendLine($"\n- Lỗi thông tin cơ bản: {basicInfoError}");
                if (permissionError != null) finalErrorMessage.AppendLine($"\n- Lỗi phân quyền: {permissionError}");
                // Nếu cả hai đều không có lỗi cụ thể, dùng thông báo chung
                if (basicInfoError == null && permissionError == null) finalErrorMessage.AppendLine("\n- Không xác định được nguyên nhân lỗi.");


                ShowError(finalErrorMessage.ToString());

                // Khôi phục trạng thái UI để người dùng có thể sửa lại
                RestoreUIState(originalState_btnLuu, originalState_btnBoQua, originalState_txtMaNhom, originalState_txtTenNhom, originalState_clbChucNang, originalState_dgv);
                this.Cursor = Cursors.Default; // Đảm bảo cursor trở lại
                // Không cần gọi UpdateUIState() ở đây vì RestoreUIState đã bật lại các control theo trạng thái gốc
            }
        }

        // Helper để khôi phục trạng thái UI sau khi có lỗi lưu
        private void RestoreUIState(bool btnLuuEnabled, bool btnBoQuaEnabled, bool txtMaNhomEnabled, bool txtTenNhomEnabled, bool clbChucNangEnabled, bool dgvEnabled)
        {
            Debug.WriteLine("Restoring UI State after Save Failure.");
            if (btnLuu != null) btnLuu.Enabled = btnLuuEnabled;
            if (btnBoQua != null) btnBoQua.Enabled = btnBoQuaEnabled;
            if (txtMaNhom != null) txtMaNhom.Enabled = txtMaNhomEnabled;
            if (txtTenNhom != null) txtTenNhom.Enabled = txtTenNhomEnabled;
            if (clbChucNang != null) clbChucNang.Enabled = clbChucNangEnabled;
            if (dgvNhomNguoiDung != null) dgvNhomNguoiDung.Enabled = dgvEnabled;
            this.Cursor = Cursors.Default;
        }


        // Xử lý khi nhấn nút "Bỏ qua"
        private async void btnBoQua_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnBoQua_Click");
            // Kiểm tra xem có thay đổi chưa lưu không
            if ((_isAdding || _isEditingMode) && HasUnsavedChanges())
            {
                if (ShowConfirm("Bạn có thay đổi chưa lưu. Hủy bỏ thay đổi?") == DialogResult.No) return;
            }

            bool wasAdding = _isAdding; // Lưu lại trạng thái trước khi reset

            // Reset trạng thái
            _isAdding = false;
            _isEditingMode = false;
            _originalChucNangIds = null; // Reset quyền gốc

            if (wasAdding) // Nếu đang thêm mà bấm bỏ qua
            {
                _currentSelectedDTO = null; // Không có DTO cũ
                ClearInputFields(); // Xóa trắng input
                dgvNhomNguoiDung?.ClearSelection(); // Bỏ chọn trên grid
            }
            else // Nếu đang sửa hoặc đang xem mà bấm bỏ qua (chỉ bật khi đang sửa)
            {
                // Hiển thị lại thông tin gốc của nhóm đang chọn
                // DisplaySelectedRow sẽ tải lại dữ liệu và quyền gốc nếu _currentSelectedDTO không null
                if (_currentSelectedDTO != null)
                {
                    // Để đảm bảo DisplaySelectedRow chạy, có thể cần bỏ chọn rồi chọn lại
                    // hoặc gọi DisplaySelectedRow trực tiếp
                    // dgvNhomNguoiDung?.ClearSelection(); // ClearSelection có thể gây lỗi nếu _currentSelectedDTO = null
                    DisplaySelectedRow(); // Gọi DisplaySelectedRow để hiển thị lại dữ liệu gốc của _currentSelectedDTO
                }
                else
                {
                    // Trường hợp không có DTO đang chọn (ví dụ bấm Bỏ qua khi màn hình mới load và chưa chọn gì)
                    ClearInputFields();
                    dgvNhomNguoiDung?.ClearSelection();
                }
            }
            UpdateUIState(); // Cập nhật UI cho trạng thái mới (xem/chọn)
        }

        // Xử lý khi nhấn nút "Xóa" (Xóa cứng)
        private async void btnXoa_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnXoa_Click (Hard Delete)");
            // Chỉ xử lý nếu có đúng một dòng được chọn và nút Xóa đang enabled
            if (dgvNhomNguoiDung?.SelectedRows.Count == 1 && btnXoa.Enabled)
            {
                if (dgvNhomNguoiDung.SelectedRows[0].DataBoundItem is NhomNguoiDungDTO dto)
                {
                    int idToDelete = dto.Id;
                    string tenNhom = dto.TenNhomNguoiDung ?? "?";

                    // Kiểm tra ID hợp lệ
                    if (idToDelete <= 0) { ShowWarning("ID nhóm không hợp lệ."); return; }

                    // Xác nhận xóa từ người dùng
                    DialogResult confirm = ShowConfirm($"Bạn chắc chắn muốn XÓA VĨNH VIỄN nhóm '{tenNhom}' (ID: {idToDelete})?\nHành động này sẽ xóa cả phân quyền và có thể ảnh hưởng đến người dùng thuộc nhóm!");

                    if (confirm == DialogResult.Yes)
                    {
                        bool success = false;
                        string? errorMsg = null;
                        this.Cursor = Cursors.WaitCursor;

                        // Vô hiệu hóa các nút thao tác chính trong quá trình xóa
                        if (btnXoa != null) btnXoa.Enabled = false;
                        if (btnSua != null) btnSua.Enabled = false;
                        if (btnThem != null) btnThem.Enabled = false;
                        if (btnLuu != null) btnLuu.Enabled = false; // Vô hiệu hóa luôn Lưu/Bỏ qua
                        if (btnBoQua != null) btnBoQua.Enabled = false;
                        if (dgvNhomNguoiDung != null) dgvNhomNguoiDung.Enabled = false; // Vô hiệu hóa grid

                        try
                        {
                            // Gọi BUS để thực hiện xóa
                            success = await _busNhomNguoiDung.DeleteNhomNguoiDungAsync(idToDelete);
                        }
                        catch (InvalidOperationException invOpEx) // Bắt lỗi ràng buộc (ví dụ còn người dùng)
                        {
                            errorMsg = $"Lỗi ràng buộc: {invOpEx.Message}";
                            Debug.WriteLine($"Delete failed due to constraint: {invOpEx.Message}");
                        }
                        catch (Exception ex) // Bắt các lỗi hệ thống khác
                        {
                            errorMsg = $"Lỗi hệ thống: {ex.Message}";
                            if (ex.InnerException != null) errorMsg += $"\nChi tiết: {ex.InnerException.Message}";
                            Debug.WriteLine($"ERROR btnXoa_Click: {ex}");
                        }
                        finally
                        {
                            this.Cursor = Cursors.Default;
                            // Không bật lại nút ở đây, LoadDataGrid (nếu thành công) hoặc UpdateUIState (nếu thất bại) sẽ làm
                        }

                        if (success)
                        {
                            ShowInfo("Xóa nhóm người dùng thành công!");
                            // Reset trạng thái sau khi xóa thành công
                            _isAdding = false;
                            _isEditingMode = false;
                            _currentSelectedDTO = null;
                            _originalChucNangIds = null;
                            ClearInputFields(); // Xóa trắng input
                            await LoadDataGrid(); // Tải lại grid (sẽ gọi UpdateUIState)
                        }
                        else
                        {
                            ShowError(errorMsg ?? "Xóa nhóm thất bại.");
                            // Nếu xóa thất bại, khôi phục lại trạng thái UI
                            UpdateUIState(); // UpdateUIState sẽ bật lại các nút Sửa, Xóa, Thêm nếu vẫn có dòng được chọn
                            if (dgvNhomNguoiDung != null) dgvNhomNguoiDung.Enabled = true; // Đảm bảo grid được bật lại
                        }
                    }
                }
                else
                {
                    ShowError("Không thể xác định thông tin nhóm người dùng để xóa.");
                    UpdateUIState(); // Cập nhật lại nút nếu có lỗi
                }
            }
            else if (dgvNhomNguoiDung?.SelectedRows.Count == 0)
            {
                ShowWarning("Vui lòng chọn nhóm người dùng cần xóa.");
            }
        }

        // Xử lý khi lựa chọn dòng trên DataGridView thay đổi
        private void dgvNhomNguoiDung_SelectionChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("dgvNhomNguoiDung_SelectionChanged");

            // Chỉ hiển thị dữ liệu của dòng được chọn lên các control nếu không đang trong chế độ thêm hoặc sửa
            // vì các control đó đang được sử dụng để nhập/sửa dữ liệu
            if (!_isAdding && !_isEditingMode)
            {
                Debug.WriteLine("SelectionChanged: Not adding/editing, calling DisplaySelectedRow.");
                DisplaySelectedRow(); // Gọi hàm hiển thị dữ liệu của dòng được chọn
            }
            else
            {
                Debug.WriteLine($"SelectionChanged: In {_isAdding} (add) or {_isEditingMode} (edit) mode, skipping data display for selection change.");
            }

            // Luôn gọi UpdateUIState sau khi xử lý xong việc chọn dòng
            // để đảm bảo trạng thái các nút và control khác đúng
            UpdateUIState();
        }

        // Xử lý khi double click vào một dòng trên DataGridView
        private void dgvNhomNguoiDung_DoubleClick(object sender, EventArgs e)
        {
            Debug.WriteLine("dgvNhomNguoiDung_DoubleClick");
            // Nếu double click vào một dòng và nút Sửa đang enabled (tức là đang ở trạng thái xem và có dòng được chọn),
            // thì kích hoạt sự kiện click của nút Sửa để chuyển sang chế độ sửa
            if (btnSua.Enabled)
            {
                btnSua_Click(sender, e);
            }
        }

        // --- Helper Methods ---
        // Các hàm hiển thị thông báo (giữ nguyên)
        private void HandleError(string action, Exception ex)
        {
            var mainForm = this.FindForm() as frmMain;
            string message = $"Lỗi khi {action}: {ex.Message}";
            if (ex.InnerException != null) message += $"\nChi tiết: {ex.InnerException.Message}";

            if (mainForm != null) MaterialMessageBox.Show(mainForm, message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Debug.WriteLine($"ERROR {action}: {ex}");
        }
        private void ShowError(string message)
        {
            var mainForm = this.FindForm() as frmMain;
            if (mainForm != null) MaterialMessageBox.Show(mainForm, message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Debug.WriteLine($"UI Error: {message}");
        }
        private void ShowWarning(string message)
        {
            var mainForm = this.FindForm() as frmMain;
            if (mainForm != null) MaterialMessageBox.Show(mainForm, message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Debug.WriteLine($"UI Warning: {message}");
        }
        private void ShowInfo(string message)
        {
            var mainForm = this.FindForm() as frmMain;
            if (mainForm != null) MaterialMessageBox.Show(mainForm, message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Debug.WriteLine($"UI Info: {message}");
        }
        private DialogResult ShowConfirm(string message)
        {
            var mainForm = this.FindForm() as frmMain;
            if (mainForm != null) return MaterialMessageBox.Show(mainForm, message, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            else return MessageBox.Show(message, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

    }
}