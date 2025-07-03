// File: GUI/ucQuanLyTheLoai.cs

using BUS; // Cần cho IBUSTheLoai
using DTO; // Cần cho TheLoaiDTO
using MaterialSkin.Controls; // Cần cho MaterialMessageBox
using System; // Cần cho Exception, ArgumentNullException, InvalidOperationException, EventArgs
using System.Collections.Generic; // Cần cho List
using System.Linq; // Cần cho LINQ (ToList)
using System.Threading.Tasks; // Cần cho async/await Task
using System.Windows.Forms; // Cần cho UserControl, DataGridView, EventArgs, DialogResult, Cursor, Panel, MessageBoxButtons, MessageBoxIcon, IWin32Window, BindingSource
using Microsoft.Extensions.DependencyInjection; // Cần cho IServiceProvider nếu mở Form/UC con
using System.Drawing; // Cần cho Point, Size, SizeF
using System.ComponentModel; // Cần cho IContainer, ComponentResourceManager
using System.Diagnostics; // Cần cho Debug.WriteLine
using MaterialSkin; // Cần cho MaterialMessageBox

// Giả sử có interface này nếu cần gọi từ frmMain
// Nếu IRequiresDataLoading nằm trong namespace GUI nhưng không static, bạn chỉ cần using GUI;
// Nếu IRequiresDataLoading nằm trong namespace khác, dùng using YourNamespace;
using static GUI.frmMain; // << GIỮ NGUYÊN nếu interface IRequiresDataLoading được định nghĩa bên trong frmMain

namespace GUI // Namespace của project GUI của bạn
{
    /// <summary>
    /// UserControl quản lý thông tin Thể loại.
    /// **Phiên bản sửa lỗi cú pháp toán tử ?? và các lỗi logic trước đó.**
    /// **Cập nhật: Truyền ID cần chọn vào LoadDataGrid.**
    /// **Cập nhật: Thêm chức năng tìm kiếm/lọc.**
    /// </summary>
    // *** THÊM IMPLEMENT INTERFACE IRequiresDataLoading ***
    public partial class ucQuanLyTheLoai : UserControl, IRequiresDataLoading // <-- Đã thêm ", IRequiresDataLoading"
    {
        // --- DEPENDENCIES ---
        private readonly IBUSTheLoai _busTheLoai;
        private readonly IServiceProvider _serviceProvider;

        // --- STATE ---
        private bool _isAdding = false;
        private TheLoaiDTO? _originalDto; // DTO gốc KHI BẮT ĐẦU SỬA
        private List<TheLoaiDTO> _danhSachGoc = new List<TheLoaiDTO>(); // <-- THÊM: Danh sách gốc để lọc

        // --- EVENTS ---
        public event EventHandler? RequestClose;

        // --- CONSTRUCTOR ---
        public ucQuanLyTheLoai(IBUSTheLoai busTheLoai, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _busTheLoai = busTheLoai ?? throw new ArgumentNullException(nameof(busTheLoai));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            // Gắn sự kiện TextChanged cho ô nhập liệu chính
            if (txtTenTheLoai != null)
            {
                txtTenTheLoai.TextChanged += InputField_Changed;
            }

            // *** THÊM: Gắn sự kiện TextChanged cho ô tìm kiếm (nếu chưa làm trong Designer) ***
            if (txtTimKiem != null)
            {
                // Gỡ bỏ trước để tránh gắn nhiều lần nếu constructor được gọi lại (hiếm khi)
                txtTimKiem.TextChanged -= txtTimKiem_TextChanged;
                txtTimKiem.TextChanged += txtTimKiem_TextChanged;
            }
        }

        // --- LOAD & INITIALIZE ---
        private void ucQuanLyTheLoai_Load(object sender, EventArgs e)
        {
            // Việc tải dữ liệu ban đầu sẽ được thực hiện qua InitializeDataAsync()
            // được gọi bởi form cha thông qua interface IRequiresDataLoading.
            // *** CÓ THỂ GẮN SỰ KIỆN Ở ĐÂY THAY VÌ CONSTRUCTOR NẾU MUỐN ***
            // if (txtTimKiem != null && !this.DesignMode) 
            // {
            //     txtTimKiem.TextChanged -= txtTimKiem_TextChanged; 
            //     txtTimKiem.TextChanged += txtTimKiem_TextChanged;
            // }
        }

        // *** IMPLEMENT TỪ IRequiresDataLoading ***
        public async Task InitializeDataAsync()
        {
            // Kiểm tra trạng thái của control trước khi thực hiện thao tác UI
            if (this.IsDisposed || !this.IsHandleCreated)
            {
                Debug.WriteLine("InitializeDataAsync: Control is disposed or handle not created. Skipping.");
                return; // Không làm gì nếu control đã bị hủy hoặc chưa tạo handle
            }
            Debug.WriteLine("InitializeDataAsync: Starting initial data load...");

            if (!this.DesignMode)
            {
                _isAdding = false;      // Luôn bắt đầu ở chế độ xem
                _originalDto = null;    // Reset DTO gốc
                ClearInputFields();     // Xóa trắng các ô nhập liệu
                if (txtTimKiem != null) txtTimKiem.Clear(); // Xóa ô tìm kiếm khi khởi tạo
                SetControlState(false, false); // Cài đặt trạng thái ban đầu: xem, chưa có dòng chọn

                await LoadDataGrid(); // Tải dữ liệu vào lưới (không cần chọn ID cụ thể ban đầu)
                Debug.WriteLine("InitializeDataAsync: Initial data load complete.");
            }
        }

        // --- DATA LOADING & GRID SETUP ---
        // *** CẬP NHẬT: Thêm tham số idToSelect ***
        private async Task LoadDataGrid(int? idToSelect = null)
        {
            Debug.WriteLine($"--- LoadDataGrid START (Selecting ID: {idToSelect?.ToString() ?? "None"}) ---");

            // Kiểm tra control trước khi thực hiện các thao tác UI nặng
            if (dgvTheLoai == null || dgvTheLoai.IsDisposed || !dgvTheLoai.IsHandleCreated)
            {
                Debug.WriteLine("LoadDataGrid: DataGridView is null, disposed, or handle not created. Aborting.");
                this.Cursor = Cursors.Default; // Đảm bảo con trỏ không bị kẹt
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            List<TheLoaiDTO>? danhSach = null;
            _danhSachGoc.Clear(); // Xóa danh sách gốc trước khi tải mới

            try
            {
                // Tạm thời hủy đăng ký sự kiện SelectionChanged để tránh fire không cần thiết khi thay đổi DataSource
                Debug.WriteLine("LoadDataGrid: Unsubscribing from SelectionChanged.");
                dgvTheLoai.SelectionChanged -= dgvTheLoai_SelectionChanged;

                dgvTheLoai.DataSource = null; // Xóa DataSource cũ
                Debug.WriteLine("LoadDataGrid: DataSource set to null.");

                // Lấy dữ liệu mới
                Debug.WriteLine("LoadDataGrid: Calling BUS GetAllTheLoaiAsync...");
                danhSach = await _busTheLoai.GetAllTheLoaiAsync() ?? new List<TheLoaiDTO>();
                Debug.WriteLine($"LoadDataGrid: Received {danhSach.Count} items from BUS.");

                // *** THÊM: Lưu danh sách gốc ***
                _danhSachGoc = danhSach.ToList(); // Tạo bản sao mới
                Debug.WriteLine($"LoadDataGrid: Stored {_danhSachGoc.Count} items to _danhSachGoc.");


                // Kiểm tra lại control trước khi gán DataSource mới
                if (dgvTheLoai.IsDisposed || !dgvTheLoai.IsHandleCreated)
                {
                    Debug.WriteLine("LoadDataGrid: DataGridView became disposed or lost handle before assigning new DataSource. Aborting.");
                    throw new ObjectDisposedException("DataGridView", "DataGridView was disposed during data loading."); // Ném lỗi rõ ràng
                }

                // *** THAY ĐỔI: Gọi hàm lọc thay vì gán trực tiếp ***
                // dgvTheLoai.DataSource = danhSach.ToList(); // <-- BỎ DÒNG NÀY
                FilterAndDisplayData(); // <-- THÊM DÒNG NÀY (Sẽ hiển thị toàn bộ ban đầu nếu txtTimKiem rỗng)
                Debug.WriteLine("LoadDataGrid: Called FilterAndDisplayData for initial display.");

                // Gán DataSource mới và cấu hình cột
                //dgvTheLoai.DataSource = danhSach.ToList(); // Sử dụng ToList() để đảm bảo binding // <<-- ĐÃ CHUYỂN SANG FilterAndDisplayData()
                //Debug.WriteLine("LoadDataGrid: New DataSource assigned."); // <<-- ĐÃ CHUYỂN SANG FilterAndDisplayData()
                SetupDataGridViewColumns(); // Gọi sau khi DataSource đã được gán bởi FilterAndDisplayData

                // Chọn lại dòng dựa trên ID được truyền vào
                bool rowReSelected = false;
                if (idToSelect.HasValue && idToSelect.Value > 0)
                {
                    Debug.WriteLine($"LoadDataGrid: Attempting to re-select ID: {idToSelect.Value}");
                    rowReSelected = SelectRowById(idToSelect.Value); // Hàm này sẽ ClearSelection trước khi chọn
                    Debug.WriteLine($"LoadDataGrid: SelectRowById result for ID {idToSelect.Value}: {rowReSelected}");
                }
                else
                {
                    Debug.WriteLine($"LoadDataGrid: No specific ID to select.");
                }

                // Sau khi Load và có thể đã chọn lại dòng
                // Nếu không tìm thấy dòng để chọn lại HOẶC không có ID nào được truyền vào để chọn
                if (!rowReSelected)
                {
                    Debug.WriteLine("LoadDataGrid: Row was not re-selected. Clearing selection and potentially inputs.");
                    // FilterAndDisplayData đã ClearSelection và cập nhật state nếu ở View mode
                    // dgvTheLoai.ClearSelection(); // Đảm bảo không có dòng nào được chọn trên lưới // <<-- ĐÃ LÀM TRONG FilterAndDisplayData
                    // Chỉ xóa input và cập nhật UI về trạng thái xem nếu đang ở chế độ xem
                    if (!_isAdding && !IsEditing())
                    {
                        // Đã làm trong FilterAndDisplayData nếu ở View Mode
                        // Debug.WriteLine("LoadDataGrid: Not Adding/Editing, calling ClearInputFields and SetControlState(false, false).");
                        // ClearInputFields();
                        // SetControlState(false, false);
                    }
                    else
                    {
                        Debug.WriteLine($"LoadDataGrid: State is Adding({_isAdding}) or Editing({IsEditing()}), inputs/state retained after FilterAndDisplay.");
                    }
                }
                // Nếu dòng được chọn lại (rowReSelected == true), SelectionChanged sẽ được gọi bởi SelectRowById
                // và sẽ gọi DisplaySelectedRow/SetControlState
                else
                {
                    Debug.WriteLine("LoadDataGrid: Row re-selection successful, SelectionChanged handler should update UI.");
                }
                Debug.WriteLine("--- LoadDataGrid TRY block finished successfully. ---");
            }
            catch (ObjectDisposedException objDispEx)
            {
                // Lỗi này xảy ra nếu control bị hủy trong quá trình load, không cần báo người dùng
                Debug.WriteLine($"*** LoadDataGrid ABORTED due to ObjectDisposedException: {objDispEx.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR in LoadDataGrid: {ex.ToString()}"); // Log stack trace
                HandleError("tải danh sách thể loại", ex);
                _danhSachGoc.Clear(); // Xóa danh sách gốc khi có lỗi tải
                if (dgvTheLoai != null && !dgvTheLoai.IsDisposed)
                {
                    try
                    {
                        // Gán DataSource rỗng khi lỗi
                        BindingSource bsError = new BindingSource();
                        bsError.DataSource = typeof(TheLoaiDTO); // Cung cấp kiểu để cột không biến mất
                        dgvTheLoai.DataSource = bsError;
                    }
                    catch { /* Ignore */ } // Cố gắng xóa data source khi lỗi
                }
                // Đảm bảo UI về trạng thái xem, không chọn dòng khi có lỗi tải và không ở trạng thái thêm/sửa
                if (!_isAdding && !IsEditing())
                {
                    ClearInputFields();
                    SetControlState(false, false);
                }
            }
            finally
            {
                Debug.WriteLine("--- LoadDataGrid FINALLY block START ---");
                this.Cursor = Cursors.Default; // Luôn khôi phục con trỏ chuột

                // Đăng ký lại sự kiện SelectionChanged *một cách an toàn* (Đã làm trong FilterAndDisplayData)
                if (dgvTheLoai != null && !dgvTheLoai.IsDisposed && dgvTheLoai.IsHandleCreated)
                {
                    // Đã làm trong FilterAndDisplayData và SelectRowById
                    // try
                    // {
                    //     Debug.WriteLine("LoadDataGrid Finally: Re-subscribing to SelectionChanged.");
                    //     // Hủy đăng ký nhiều lần không sao, đảm bảo chỉ có một handler được gắn
                    //     dgvTheLoai.SelectionChanged -= dgvTheLoai_SelectionChanged;
                    //     dgvTheLoai.SelectionChanged += dgvTheLoai_SelectionChanged;
                    //     ... (phần BeginInvoke kiểm tra UI) ...
                    // } ...
                }
                else
                {
                    Debug.WriteLine("LoadDataGrid Finally: DataGridView is null, disposed, or handle not created. Skipping final checks.");
                }
                Debug.WriteLine("--- LoadDataGrid FINALLY block END ---");
            }
            Debug.WriteLine($"--- LoadDataGrid END ---");
        }


        private void SetupDataGridViewColumns()
        {
            // Kiểm tra trước khi truy cập Columns
            if (dgvTheLoai == null) // Chỉ cần kiểm tra grid null
            {
                Debug.WriteLine("SetupDataGridViewColumns: DataGridView is null. Skipping setup.");
                return;
            }
            // Kiểm tra DataSource có phải BindingSource không và có dữ liệu không
            BindingSource? bs = dgvTheLoai.DataSource as BindingSource;
            if (bs == null)
            {
                Debug.WriteLine("SetupDataGridViewColumns: DataSource is not a BindingSource or null. Skipping setup.");
                return;
            }
            // Nếu DataSource là BindingSource nhưng list bên trong rỗng, cột vẫn tồn tại nếu AutoGenerateColumns = true (mặc định)
            // Hoặc nếu AutoGenerateColumns = false và chưa Add cột thủ công -> Columns.Count = 0
            if (dgvTheLoai.Columns.Count == 0 && dgvTheLoai.AutoGenerateColumns)
            {
                // Có thể cần đợi hoặc ép tạo cột nếu AutoGenerateColumns=true và list rỗng ban đầu
                Debug.WriteLine("SetupDataGridViewColumns: Columns count is 0 but AutoGenerateColumns=true. Forcing column generation (may not work if list is empty).");
                // dgvTheLoai.AutoGenerateColumns = false; // Tắt đi nếu bạn định nghĩa cột thủ công
                // dgvTheLoai.AutoGenerateColumns = true; // Bật lại để thử ép tạo cột
                // Hoặc thêm cột thủ công nếu bạn muốn kiểm soát hoàn toàn:
                // dgvTheLoai.Columns.Add("MaTheLoai", "Mã Thể Loại");
                // dgvTheLoai.Columns.Add("TenTheLoai", "Tên Thể Loại");
                // ... cấu hình các cột vừa thêm ...
                // return; // Thoát nếu bạn thêm cột thủ công và không muốn chạy code dưới
            }
            else if (dgvTheLoai.Columns.Count == 0)
            {
                Debug.WriteLine("SetupDataGridViewColumns: Columns count is 0 and AutoGenerateColumns=false. Skipping setup.");
                return;
            }

            Debug.WriteLine("SetupDataGridViewColumns: Setting up columns...");

            try
            {
                dgvTheLoai.ReadOnly = true;
                dgvTheLoai.AllowUserToAddRows = false;
                dgvTheLoai.AllowUserToDeleteRows = false;
                dgvTheLoai.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvTheLoai.MultiSelect = false;
                dgvTheLoai.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; // Set None first
                dgvTheLoai.RowHeadersVisible = false;
                dgvTheLoai.AutoGenerateColumns = false; // *** QUAN TRỌNG: Tắt AutoGenerate nếu bạn cấu hình cột thủ công ***

                // Xóa cột cũ trước khi thêm (để tránh trùng lặp nếu gọi lại)
                dgvTheLoai.Columns.Clear();

                // Thêm và cấu hình cột thủ công
                dgvTheLoai.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "MaTheLoai",
                    DataPropertyName = "MaTheLoai", // Phải khớp tên thuộc tính trong TheLoaiDTO
                    HeaderText = "Mã Thể Loại",
                    Width = 120
                });
                dgvTheLoai.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TenTheLoai",
                    DataPropertyName = "TenTheLoai", // Phải khớp tên thuộc tính trong TheLoaiDTO
                    HeaderText = "Tên Thể Loại",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill // Fill sau khi các cột khác đã set Width
                });

                // Cột ID ẩn (vẫn cần để lấy ID khi chọn dòng)
                dgvTheLoai.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Id",
                    DataPropertyName = "Id",
                    Visible = false
                });


                // var columns = dgvTheLoai.Columns; // Lấy lại danh sách cột sau khi thêm thủ công

                // // Cấu hình các cột cụ thể (Ví dụ nếu dùng AutoGenerate=true)
                // if (columns.Contains("Id")) columns["Id"].Visible = false;
                // if (columns.Contains("MaTheLoai")) { columns["MaTheLoai"].HeaderText = "Mã Thể Loại"; columns["MaTheLoai"].Width = 120; }
                // if (columns.Contains("TenTheLoai")) { columns["TenTheLoai"].HeaderText = "Tên Thể Loại"; columns["TenTheLoai"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; } // Fill sau khi các cột khác đã set Width

                // // Đảm bảo các cột khác (nếu có từ AutoGenerate) không hiển thị nếu không được cấu hình
                // foreach (DataGridViewColumn col in columns)
                // {
                //     if (col.Name != "Id" && col.Name != "MaTheLoai" && col.Name != "TenTheLoai")
                //     {
                //         col.Visible = false;
                //     }
                // }

                Debug.WriteLine("SetupDataGridViewColumns: Manual columns setup complete.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR setting up DataGridView columns: {ex.ToString()}"); // Log cả stacktrace
                // Không cần báo lỗi này cho người dùng cuối, nhưng cần ghi log
            }
        }
        #region // --- PHƯƠNG THỨC LỌC VÀ HIỂN THỊ ---

        /// <summary>
        /// Lọc danh sách thể loại gốc dựa trên ô tìm kiếm và cập nhật DataGridView.
        /// </summary>
        private void FilterAndDisplayData()
        {
            // Kiểm tra control và danh sách gốc
            if (dgvTheLoai == null || dgvTheLoai.IsDisposed || !dgvTheLoai.IsHandleCreated || _danhSachGoc == null)
            {
                Debug.WriteLine("FilterAndDisplayData: Grid invalid or _danhSachGoc is null. Skipping.");
                return;
            }

            string searchTerm = txtTimKiem?.Text.Trim().ToLowerInvariant() ?? ""; // Lấy text tìm kiếm, chuẩn hóa
            Debug.WriteLine($"FilterAndDisplayData: Filtering with searchTerm='{searchTerm}'");

            List<TheLoaiDTO> filteredList;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // Hiển thị toàn bộ danh sách gốc nếu không có từ khóa tìm kiếm
                filteredList = _danhSachGoc.ToList(); // Tạo bản sao
                Debug.WriteLine($"FilterAndDisplayData: Search term is empty. Displaying all {_danhSachGoc.Count} items.");
            }
            else
            {
                // Thực hiện lọc trên danh sách gốc
                filteredList = _danhSachGoc
                    .Where(tl =>
                        (tl.MaTheLoai != null && tl.MaTheLoai.ToLowerInvariant().Contains(searchTerm)) ||
                        (tl.TenTheLoai != null && tl.TenTheLoai.ToLowerInvariant().Contains(searchTerm))
                    )
                    .ToList();
                Debug.WriteLine($"FilterAndDisplayData: Found {filteredList.Count} items matching the search term.");
            }

            // Sử dụng BindingSource để quản lý dữ liệu hiệu quả hơn
            BindingSource bs = new BindingSource();
            bs.DataSource = filteredList;


            // Tạm thời hủy sự kiện SelectionChanged
            Debug.WriteLine("FilterAndDisplayData: Unsubscribing from SelectionChanged before changing DataSource.");
            dgvTheLoai.SelectionChanged -= dgvTheLoai_SelectionChanged;

            dgvTheLoai.DataSource = null; // Xóa datasource cũ để refresh hoàn toàn
            dgvTheLoai.DataSource = bs; // Gán BindingSource mới
            Debug.WriteLine("FilterAndDisplayData: Assigned filtered list to DataGridView DataSource via BindingSource.");


            // // Cấu hình lại cột nếu cần (thường chỉ cần làm 1 lần trong LoadDataGrid)
            // SetupDataGridViewColumns(); 

            // Xóa lựa chọn hiện tại vì danh sách đã thay đổi
            if (dgvTheLoai.Rows.Count > 0)
            {
                dgvTheLoai.ClearSelection();
                Debug.WriteLine("FilterAndDisplayData: Cleared selection.");
            }
            else
            {
                Debug.WriteLine("FilterAndDisplayData: No rows after filtering, selection not cleared.");
            }


            // Cập nhật trạng thái control về chế độ xem, không chọn dòng (nếu đang ở chế độ xem)
            if (!_isAdding && !IsEditing())
            {
                Debug.WriteLine("FilterAndDisplayData: In View mode, calling ClearInputFields and SetControlState(false, false).");
                // Chỉ xóa input nếu không có dòng nào trên lưới sau khi lọc
                if (dgvTheLoai.Rows.Count == 0)
                {
                    ClearInputFields();
                }
                SetControlState(false, false); // Luôn cập nhật nút về trạng thái không chọn dòng
            }
            else
            {
                Debug.WriteLine("FilterAndDisplayData: In Add/Edit mode, state/inputs retained.");
                // Đảm bảo nút Lưu/Bỏ qua vẫn đúng trạng thái (không thay đổi bởi việc lọc)
                SetControlState(true, dgvTheLoai.SelectedRows.Count > 0); // Giữ isAddingOrEditing = true
            }

            // Đăng ký lại sự kiện SelectionChanged *sau khi* DataSource đã được cập nhật hoàn toàn
            // và đảm bảo control vẫn hợp lệ
            if (!dgvTheLoai.IsDisposed && dgvTheLoai.IsHandleCreated)
            {
                Debug.WriteLine("FilterAndDisplayData: Re-subscribing to SelectionChanged.");
                // Hủy đăng ký lại để đảm bảo chỉ có 1 handler
                dgvTheLoai.SelectionChanged -= dgvTheLoai_SelectionChanged;
                dgvTheLoai.SelectionChanged += dgvTheLoai_SelectionChanged;

                // Nếu sau khi lọc có dòng và đang ở view mode, tự động chọn dòng đầu tiên? (Tùy chọn)
                // if (!_isAdding && !IsEditing() && dgvTheLoai.Rows.Count > 0)
                // {
                //     dgvTheLoai.Rows[0].Selected = true;
                //     // SelectionChanged sẽ tự động kích hoạt DisplaySelectedRow
                // }
            }
            else
            {
                Debug.WriteLine("FilterAndDisplayData: Grid became invalid before re-subscribing SelectionChanged.");
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nội dung ô tìm kiếm thay đổi.
        /// </summary>
        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            // Chỉ lọc khi đang ở chế độ xem để không ảnh hưởng Thêm/Sửa
            if (!_isAdding && !IsEditing())
            {
                FilterAndDisplayData(); // Gọi hàm lọc
            }
            else
            {
                Debug.WriteLine("txtTimKiem_TextChanged: In Add/Edit mode. Filtering skipped.");
            }
        }

        #endregion // --- KẾT THÚC PHẦN LỌC ---

        // --- UI STATE & DISPLAY ---
        private bool IsEditing()
        {
            return !_isAdding && _originalDto != null;
        }

        private void ClearInputFields()
        {
            Debug.WriteLine("ClearInputFields called.");
            if (txtId != null) txtId.Clear();
            if (txtMaTheLoai != null) txtMaTheLoai.Clear();
            if (txtTenTheLoai != null) txtTenTheLoai.Clear();

            // Reset DTO gốc khi xóa input hoặc chuyển sang trạng thái không phải sửa
            if (!IsEditing()) // Chỉ reset nếu không đang trong trạng thái sửa
            {
                _originalDto = null;
                Debug.WriteLine("ClearInputFields: _originalDto reset to null.");
            }

        }

        private void SetControlState(bool isAddingOrEditing, bool rowSelected)
        {
            // Thêm log chi tiết hơn
            if (this.IsDisposed || !this.IsHandleCreated)
            {
                Debug.WriteLine($"*** SetControlState SKIPPED: IsDisposed={this.IsDisposed}, IsHandleCreated={this.IsHandleCreated}");
                return;
            }
            // Kiểm tra các control tồn tại trước khi sử dụng
            bool controlsAvailable = btnThem != null && btnSua != null && btnXoa != null &&
                                  btnLuu != null && btnBoQua != null && txtTenTheLoai != null &&
                                  dgvTheLoai != null && txtTimKiem != null; // Thêm txtTimKiem

            if (!controlsAvailable)
            {
                Debug.WriteLine("*** SetControlState SKIPPED: One or more controls are null.");
                return;
            }


            Debug.WriteLine($"--- SetControlState START: isAddingOrEditing={isAddingOrEditing}, rowSelected={rowSelected} ---");

            try
            {
                // Các nút thao tác trên lưới
                bool viewMode = !isAddingOrEditing;
                //if (btnThem != null) // Đã kiểm tra ở trên
                //{
                bool shouldEnableThem = viewMode;
                if (btnThem.Enabled != shouldEnableThem)
                { // Chỉ thay đổi nếu khác trạng thái hiện tại
                    Debug.WriteLine($"Setting btnThem.Enabled = {shouldEnableThem} (Current: {btnThem.Enabled})");
                    btnThem.Enabled = shouldEnableThem;
                }
                //}
                //if (btnSua != null)
                //{
                bool shouldEnableSua = viewMode && rowSelected;
                if (btnSua.Enabled != shouldEnableSua)
                {
                    Debug.WriteLine($"Setting btnSua.Enabled = {shouldEnableSua} (Current: {btnSua.Enabled})");
                    btnSua.Enabled = shouldEnableSua;
                }
                //}
                //if (btnXoa != null)
                //{
                bool shouldEnableXoa = viewMode && rowSelected;
                if (btnXoa.Enabled != shouldEnableXoa)
                {
                    Debug.WriteLine($"Setting btnXoa.Enabled = {shouldEnableXoa} (Current: {btnXoa.Enabled})");
                    btnXoa.Enabled = shouldEnableXoa;
                }
                //}

                // Các nút thao tác lưu/hủy
                bool hasChanges = HasUnsavedChanges(); // Tính toán trước khi dùng
                Debug.WriteLine($"SetControlState: HasUnsavedChanges = {hasChanges}");
                //if (btnLuu != null)
                //{
                bool shouldEnableLuu = isAddingOrEditing && hasChanges;
                if (btnLuu.Enabled != shouldEnableLuu)
                {
                    Debug.WriteLine($"Setting btnLuu.Enabled = {shouldEnableLuu} (Current: {btnLuu.Enabled})");
                    btnLuu.Enabled = shouldEnableLuu;
                }
                //}
                //if (btnBoQua != null)
                //{
                bool shouldEnableBoQua = isAddingOrEditing;
                if (btnBoQua.Enabled != shouldEnableBoQua)
                {
                    Debug.WriteLine($"Setting btnBoQua.Enabled = {shouldEnableBoQua} (Current: {btnBoQua.Enabled})");
                    btnBoQua.Enabled = shouldEnableBoQua;
                }
                //}


                // Các ô nhập liệu
                if (txtId != null) // ID và Mã luôn ReadOnly=true và Enabled=false
                {
                    // txtId.ReadOnly = true; // Nên set trong Designer
                    if (txtId.Enabled) txtId.Enabled = false;
                }
                if (txtMaTheLoai != null)
                {
                    // txtMaTheLoai.ReadOnly = true; // Nên set trong Designer
                    if (txtMaTheLoai.Enabled) txtMaTheLoai.Enabled = false;
                }
                //if (txtTenTheLoai != null) // Đã kiểm tra ở trên
                //{
                bool shouldEnableTen = isAddingOrEditing;
                if (txtTenTheLoai.Enabled != shouldEnableTen)
                {
                    Debug.WriteLine($"Setting txtTenTheLoai.Enabled = {shouldEnableTen} (Current: {txtTenTheLoai.Enabled})");
                    txtTenTheLoai.Enabled = shouldEnableTen;
                }
                //}

                // DataGridView và Ô tìm kiếm
                //if (dgvTheLoai != null) // Đã kiểm tra ở trên
                //{
                bool shouldEnableGridAndSearch = viewMode;
                if (dgvTheLoai.Enabled != shouldEnableGridAndSearch)
                {
                    Debug.WriteLine($"Setting dgvTheLoai.Enabled = {shouldEnableGridAndSearch} (Current: {dgvTheLoai.Enabled})");
                    dgvTheLoai.Enabled = shouldEnableGridAndSearch;
                }
                //}
                //if (txtTimKiem != null) // Đã kiểm tra ở trên
                //{
                if (txtTimKiem.Enabled != shouldEnableGridAndSearch)
                {
                    Debug.WriteLine($"Setting txtTimKiem.Enabled = {shouldEnableGridAndSearch} (Current: {txtTimKiem.Enabled})");
                    txtTimKiem.Enabled = shouldEnableGridAndSearch;
                }
                //}


                // Focus vào ô tên sau khi đã enable nó (nếu đang thêm/sửa)
                if (isAddingOrEditing && txtTenTheLoai.Enabled && !txtTenTheLoai.Focused)
                {
                    Debug.WriteLine("SetControlState: Focusing txtTenTheLoai.");
                    // Dùng BeginInvoke để đảm bảo focus sau khi tất cả các thay đổi state hoàn tất
                    this.BeginInvoke(new Action(() => {
                        if (txtTenTheLoai != null && !txtTenTheLoai.IsDisposed && txtTenTheLoai.Enabled)
                        {
                            txtTenTheLoai.Focus();
                        }
                    }));

                }

                Debug.WriteLine($"--- SetControlState END ---");
            }
            catch (ObjectDisposedException)
            {
                Debug.WriteLine("*** SetControlState caught ObjectDisposedException. Skipping further updates. ***");
                /* Bỏ qua nếu control bị dispose trong lúc cập nhật */
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** UNEXPECTED ERROR in SetControlState: {ex.ToString()}");
                // Cân nhắc ném lại lỗi nếu đây là lỗi nghiêm trọng
                // throw;
            }
        }

        private void DisplaySelectedRow()
        {
            // Thêm log và kiểm tra trạng thái
            if (this.IsDisposed || !this.IsHandleCreated || dgvTheLoai == null) // Thêm kiểm tra dgvTheLoai
            {
                Debug.WriteLine($"DisplaySelectedRow: Control invalid/disposed or Grid null. Skipping. IsDisposed={this.IsDisposed}, IsHandleCreated={this.IsHandleCreated}, IsGridNull={dgvTheLoai == null}");
                return;
            }
            Debug.WriteLine($"DisplaySelectedRow triggered. IsAdding={_isAdding}, IsEditing={IsEditing()}");

            // Chỉ xử lý khi đang ở chế độ xem (logic này đã được kiểm tra trong SelectionChanged)
            if (_isAdding || IsEditing())
            {
                Debug.WriteLine("DisplaySelectedRow: Called while Adding or Editing. Skipping.");
                return;
            }


            TheLoaiDTO? dtoToDisplay = null;
            bool rowSelected = false;

            if (dgvTheLoai.SelectedRows.Count == 1 && dgvTheLoai.SelectedRows[0].DataBoundItem is TheLoaiDTO dto)
            {
                dtoToDisplay = dto;
                rowSelected = true;
                Debug.WriteLine($"DisplaySelectedRow: Single row selected. ID: {dto.Id}");
            }
            else
            {
                Debug.WriteLine($"DisplaySelectedRow: No single row selected (Count: {dgvTheLoai.SelectedRows.Count}).");
                // Nếu không có dòng nào được chọn, đảm bảo _originalDto là null ở chế độ xem
                _originalDto = null;
                // Xóa input nếu không có dòng nào được chọn
                //ClearInputFields(); // <- Đã chuyển vào DisplayDtoData
            }

            // Hiển thị dữ liệu hoặc xóa trắng form
            DisplayDtoData(dtoToDisplay); // Hàm này sẽ gọi ClearInputFields nếu dtoToDisplay là null

            // Cập nhật trạng thái nút dựa trên selection mới
            // Luôn là trạng thái xem (isAddingOrEditing = false) khi gọi hàm này
            Debug.WriteLine($"DisplaySelectedRow: Calling SetControlState(false, {rowSelected})");
            // Hàm SetControlState(false,...) này sẽ đặt dgvTheLoai.Enabled = true
            SetControlState(false, rowSelected);
        }

        private bool HasUnsavedChanges()
        {
            // Kiểm tra controls nhập liệu có tồn tại không
            if (txtTenTheLoai == null)
            {
                Debug.WriteLine("HasUnsavedChanges: txtTenTheLoai is null. Returning false.");
                return false;
            }

            if (_isAdding)
            {
                // Trong chế độ thêm, có thay đổi nếu ô Tên thể loại có nhập liệu
                bool changes = !string.IsNullOrWhiteSpace(txtTenTheLoai.Text);
                // Debug.WriteLine($"HasUnsavedChanges (Adding Mode): Text='{txtTenTheLoai.Text}', HasChanges={changes}");
                return changes;
            }
            else if (IsEditing() && _originalDto != null)
            {
                // Trong chế độ sửa, có thay đổi nếu Tên thể loại hiện tại khác với Tên thể loại gốc
                string currentTen = txtTenTheLoai.Text.Trim();
                string originalTen = _originalDto.TenTheLoai?.Trim() ?? "";
                bool changes = currentTen != originalTen;
                // Debug.WriteLine($"HasUnsavedChanges (Editing Mode): Current='{currentTen}', Original='{originalTen}', HasChanges={changes}");
                return changes;
            }

            // Debug.WriteLine("HasUnsavedChanges: Not Adding or Editing. Returning false.");
            return false; // Không đang thêm hoặc sửa
        }

        // Helper hiển thị dữ liệu DTO lên form
        private void DisplayDtoData(TheLoaiDTO? dto) // Chấp nhận DTO null
        {
            // Kiểm tra controls có tồn tại không
            if (txtId == null || txtMaTheLoai == null || txtTenTheLoai == null)
            {
                Debug.WriteLine("DisplayDtoData: One or more input controls are null. Skipping.");
                return;
            }

            if (dto == null)
            {
                Debug.WriteLine("DisplayDtoData: DTO is null.");
                // Nếu DTO null (thường do không có dòng nào được chọn), xóa input
                Debug.WriteLine("DisplayDtoData: Calling ClearInputFields because DTO is null.");
                ClearInputFields();
                return;
            }

            Debug.WriteLine($"DisplayDtoData: Displaying DTO ID: {dto.Id}, Name: '{dto.TenTheLoai}'");
            txtId.Text = dto.Id.ToString();
            txtMaTheLoai.Text = dto.MaTheLoai ?? "";

            // Tạm thời ngắt sự kiện TextChanged để tránh trigger HasUnsavedChanges khi set giá trị
            // Và tránh gọi SetControlState không cần thiết từ InputField_Changed
            txtTenTheLoai.TextChanged -= InputField_Changed;
            txtTenTheLoai.Text = dto.TenTheLoai ?? "";
            txtTenTheLoai.TextChanged += InputField_Changed;

        }

        // --- BUTTON EVENT HANDLERS ---
        private void btnThem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnThem_Click triggered.");
            // Nếu đang ở chế độ xem, xóa ô tìm kiếm để hiển thị lại toàn bộ danh sách khi Thêm
            if (!_isAdding && !IsEditing() && txtTimKiem != null && !string.IsNullOrEmpty(txtTimKiem.Text))
            {
                Debug.WriteLine("btnThem_Click: Clearing search term before adding.");
                txtTimKiem.Clear(); // Thao tác này sẽ trigger FilterAndDisplayData
                                    // FilterAndDisplayData sẽ tự động ClearSelection nếu cần
            }


            // Hỏi xác nhận nếu đang sửa và có thay đổi
            if (IsEditing() && HasUnsavedChanges())
            {
                Debug.WriteLine("btnThem_Click: Currently editing with unsaved changes. Asking confirmation.");
                if (ShowConfirm("Bạn có thay đổi chưa lưu khi đang sửa. Bạn có muốn hủy bỏ và thêm mới?") == DialogResult.No)
                {
                    Debug.WriteLine("btnThem_Click: User chose not to discard editing changes.");
                    return;
                }
                Debug.WriteLine("btnThem_Click: User chose to discard editing changes.");
            }
            // Nếu đang thêm mà bấm thêm lần nữa và có thay đổi
            else if (_isAdding && HasUnsavedChanges())
            {
                Debug.WriteLine("btnThem_Click: Currently adding with unsaved changes. Asking confirmation.");
                if (ShowConfirm("Bạn có thay đổi chưa lưu khi đang thêm. Bạn có muốn hủy bỏ và bắt đầu lại?") == DialogResult.No)
                {
                    Debug.WriteLine("btnThem_Click: User chose not to discard adding changes.");
                    return;
                }
                Debug.WriteLine("btnThem_Click: User chose to discard adding changes.");
            }

            _isAdding = true;       // Chuyển sang trạng thái Thêm
            _originalDto = null;    // Reset DTO gốc
            Debug.WriteLine("btnThem_Click: State set to Adding. _originalDto = null.");

            // Bỏ chọn dòng trên lưới (FilterAndDisplayData có thể đã làm nếu txtTimKiem bị xóa)
            dgvTheLoai?.ClearSelection();
            Debug.WriteLine("btnThem_Click: Cleared DataGridView selection (again).");

            ClearInputFields();     // Xóa trắng các ô input
            Debug.WriteLine("btnThem_Click: Cleared input fields.");

            // Bây giờ mới gọi SetControlState sau khi logic đã cập nhật
            SetControlState(true, false); // Trạng thái: đang thêm, không có dòng chọn
            Debug.WriteLine("btnThem_Click: SetControlState(true, false) called.");

            // Focus đã được xử lý trong SetControlState
        }


        private void btnSua_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnSua_Click triggered.");
            // Chỉ sửa nếu có 1 dòng chọn VÀ đang ở chế độ xem (nút Sửa Enabled)
            if (dgvTheLoai?.SelectedRows.Count == 1 && btnSua != null && btnSua.Enabled)
            {
                Debug.WriteLine("btnSua_Click: Conditions met (Single row selected, button enabled).");
                // Hỏi xác nhận nếu đang thêm và có thay đổi
                if (_isAdding && HasUnsavedChanges())
                {
                    Debug.WriteLine("btnSua_Click: Currently adding with unsaved changes. Asking confirmation.");
                    DialogResult confirm = ShowConfirm("Bạn có thay đổi chưa lưu khi đang thêm. Bạn có muốn hủy bỏ và sửa thể loại đang chọn?");
                    if (confirm == DialogResult.No)
                    {
                        Debug.WriteLine("btnSua_Click: User chose not to discard adding changes.");
                        return;
                    }
                    Debug.WriteLine("btnSua_Click: User chose to discard adding changes.");
                    // Nếu đồng ý hủy Thêm, cần reset lại state trước khi vào Sửa
                    _isAdding = false; // Thoát khỏi trạng thái Thêm
                    ClearInputFields(); // Xóa input của trạng thái Thêm cũ
                }

                if (dgvTheLoai.SelectedRows[0]?.DataBoundItem is TheLoaiDTO dtoToEdit)
                {
                    Debug.WriteLine($"btnSua_Click: DTO to edit found. ID: {dtoToEdit.Id}");
                    _isAdding = false; // Đảm bảo đang ở trạng thái Sửa

                    // Quan trọng: Tạo bản sao DTO gốc vào _originalDto
                    _originalDto = new TheLoaiDTO
                    {
                        Id = dtoToEdit.Id,
                        MaTheLoai = dtoToEdit.MaTheLoai,
                        TenTheLoai = dtoToEdit.TenTheLoai
                    };
                    Debug.WriteLine($"btnSua_Click: _originalDto created. ID: {_originalDto.Id}, Name: '{_originalDto.TenTheLoai}'");

                    DisplayDtoData(dtoToEdit); // Hiển thị dữ liệu lên form
                    Debug.WriteLine("btnSua_Click: Displayed DTO data.");

                    SetControlState(true, true); // Cập nhật UI sang chế độ sửa
                    Debug.WriteLine("btnSua_Click: SetControlState(true, true) called.");
                    // Focus đã được xử lý trong SetControlState
                }
                else
                {
                    Debug.WriteLine("btnSua_Click: Could not get TheLoaiDTO from selected row's DataBoundItem.");
                    ShowError("Không thể lấy dữ liệu thể loại từ dòng đã chọn.");
                    // Quay về trạng thái xem nếu không lấy được DTO
                    _isAdding = false; _originalDto = null;
                    SetControlState(false, dgvTheLoai.SelectedRows.Count > 0); // Vẫn đang chọn dòng (nếu có) nhưng không vào được chế độ sửa
                }
            }
            else if (dgvTheLoai?.SelectedRows.Count != 1)
            {
                Debug.WriteLine($"btnSua_Click: Condition not met - SelectedRows.Count = {dgvTheLoai?.SelectedRows.Count ?? -1}");
                ShowWarning("Vui lòng chọn một thể loại cần sửa.");
            }
            else if (btnSua == null || !btnSua.Enabled)
            {
                Debug.WriteLine($"btnSua_Click: Condition not met - Button is null or disabled (Enabled={btnSua?.Enabled}).");
                // Không cần thông báo
            }
        }

        private async void btnXoa_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnXoa_Click triggered.");
            // Chỉ xóa nếu có 1 dòng chọn VÀ đang ở chế độ xem (nút Xóa Enabled)
            if (!_isAdding && !IsEditing() && dgvTheLoai?.SelectedRows.Count == 1 && btnXoa != null && btnXoa.Enabled)
            {
                Debug.WriteLine("btnXoa_Click: Conditions met.");
                if (dgvTheLoai.SelectedRows[0]?.DataBoundItem is TheLoaiDTO dtoToDelete)
                {
                    Debug.WriteLine($"btnXoa_Click: DTO to delete found. ID: {dtoToDelete.Id}");
                    if (ShowConfirm($"Bạn chắc chắn muốn XÓA thể loại '{dtoToDelete.TenTheLoai ?? "?"}' (ID: {dtoToDelete.Id})?") == DialogResult.Yes)
                    {
                        Debug.WriteLine("btnXoa_Click: User confirmed deletion.");
                        this.Cursor = Cursors.WaitCursor;
                        bool success = false;
                        string? errorMsg = null;

                        // Tạm thời tắt các nút thao tác dữ liệu
                        SetAllActionButtonsEnabled(false); // Helper function để tắt/bật nút
                        Debug.WriteLine("btnXoa_Click: Action buttons temporarily disabled.");

                        try
                        {
                            Debug.WriteLine($"btnXoa_Click: Calling BUS CanDeleteTheLoaiAsync({dtoToDelete.Id})...");
                            bool canDelete = await _busTheLoai.CanDeleteTheLoaiAsync(dtoToDelete.Id);
                            Debug.WriteLine($"btnXoa_Click: CanDeleteTheLoaiAsync result: {canDelete}");
                            if (canDelete)
                            {
                                Debug.WriteLine($"btnXoa_Click: Calling BUS DeleteTheLoaiAsync({dtoToDelete.Id})...");
                                success = await _busTheLoai.DeleteTheLoaiAsync(dtoToDelete.Id);
                                Debug.WriteLine($"btnXoa_Click: DeleteTheLoaiAsync result: {success}");
                            }
                            else
                            {
                                errorMsg = $"Không thể xóa thể loại '{dtoToDelete.TenTheLoai ?? "?"}' vì còn sách liên quan.";
                                Debug.WriteLine($"btnXoa_Click: Deletion prevented by business logic: {errorMsg}");
                            }
                            if (!success && canDelete) // Nếu CanDelete=true nhưng Delete vẫn false
                            {
                                errorMsg = errorMsg ?? "Xóa thất bại do lỗi không xác định.";
                                Debug.WriteLine($"btnXoa_Click: Deletion failed despite CanDelete=true. Error: {errorMsg}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"*** ERROR during deletion process: {ex.ToString()}");
                            HandleError("xóa thể loại", ex);
                            errorMsg = ex.Message; // Cập nhật errorMsg nếu có ngoại lệ
                        }
                        finally
                        {
                            this.Cursor = Cursors.Default;
                            Debug.WriteLine("btnXoa_Click: Cursor restored.");
                            // Kích hoạt lại các nút sẽ được xử lý bởi LoadDataGrid hoặc SetControlState ở dưới
                            // Hoặc gọi lại SetAllActionButtonsEnabled(true); nếu không có LoadDataGrid
                        }

                        if (success)
                        {
                            Debug.WriteLine("btnXoa_Click: Deletion successful.");
                            ShowInfo("Xóa thành công!");
                            // Reset trạng thái logic về xem
                            _isAdding = false;
                            _originalDto = null;
                            // Xóa ô tìm kiếm để tải lại toàn bộ danh sách sau khi xóa
                            if (txtTimKiem != null) txtTimKiem.Clear();
                            Debug.WriteLine("btnXoa_Click: State reset to view mode. Search cleared.");
                            await LoadDataGrid(); // Tải lại lưới, LoadDataGrid sẽ gọi FilterAndDisplayData
                        }
                        else
                        {
                            Debug.WriteLine($"btnXoa_Click: Deletion failed. Error: {errorMsg ?? "Unknown"}");
                            ShowError(errorMsg ?? "Xóa thất bại."); // Hiển thị lỗi
                                                                    // Bật lại các nút nhưng giữ nguyên trạng thái xem
                            SetAllActionButtonsEnabled(true); // Bật lại nút
                            SetControlState(false, dgvTheLoai.SelectedRows.Count > 0); // Cập nhật trạng thái nút Thêm/Sửa/Xóa dựa trên selection
                            Debug.WriteLine("btnXoa_Click: Deletion failed, restoring view mode button state.");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("btnXoa_Click: User cancelled deletion.");
                    }
                }
                else
                {
                    Debug.WriteLine("btnXoa_Click: Could not get TheLoaiDTO from selected row's DataBoundItem.");
                    ShowError("Không thể lấy dữ liệu thể loại từ dòng đã chọn.");
                }
            }
            else
            {
                Debug.WriteLine($"btnXoa_Click: Conditions not met. IsAdding={_isAdding}, IsEditing={IsEditing()}, SelectedCount={dgvTheLoai?.SelectedRows.Count ?? -1}, IsButtonEnabled={btnXoa?.Enabled}");
            }
        }

        private async void btnLuu_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnLuu_Click triggered.");
            // Kiểm tra các controls có tồn tại không trước khi tiếp tục
            if (btnLuu == null || btnBoQua == null || txtTenTheLoai == null || txtId == null || txtMaTheLoai == null)
            {
                Debug.WriteLine("*** btnLuu_Click ERROR: One or more required controls are null.");
                ShowError("Lỗi: Một số thành phần giao diện chưa sẵn sàng.");
                return;
            }

            // Nút Lưu chỉ nên được Enabled khi có thay đổi và đang thêm/sửa.
            if (!btnLuu.Enabled)
            {
                Debug.WriteLine("btnLuu_Click: Save button is disabled. Exiting.");
                return;
            }
            // Kiểm tra logic lại: Phải đang ở chế độ thêm hoặc sửa
            if (!_isAdding && !IsEditing())
            {
                Debug.WriteLine($"*** btnLuu_Click LOGIC ERROR: Save clicked but not in Add or Edit mode? _isAdding={_isAdding}, IsEditing={IsEditing()}. Exiting.");
                SetControlState(false, dgvTheLoai?.SelectedRows.Count > 0);
                return;
            }
            Debug.WriteLine($"btnLuu_Click: In Add({_isAdding}) or Edit({IsEditing()}) mode. Proceeding...");


            string tenTheLoai = txtTenTheLoai.Text.Trim();
            // Validate tên thể loại
            if (string.IsNullOrWhiteSpace(tenTheLoai))
            {
                Debug.WriteLine("btnLuu_Click: Validation failed - TenTheLoai is empty.");
                ShowWarning("Tên thể loại không được rỗng.");
                txtTenTheLoai?.Focus();
                return;
            }
            Debug.WriteLine($"btnLuu_Click: TenTheLoai validated: '{tenTheLoai}'");

            // Xác định ID và kiểm tra ID hợp lệ khi cập nhật
            int currentId = 0; // Mặc định cho thêm mới
            string? currentMa = null; // Mặc định cho thêm mới
            if (!_isAdding) // Đang sửa
            {
                if (_originalDto == null || _originalDto.Id <= 0)
                {
                    Debug.WriteLine($"*** btnLuu_Click ERROR: Edit mode but _originalDto is null or has invalid ID (ID={_originalDto?.Id}).");
                    ShowError("Lỗi dữ liệu: Không xác định được thể loại gốc để cập nhật. Vui lòng hủy và thử lại.");
                    btnBoQua_Click(sender, e); // Tự động Bỏ qua
                    return;
                }
                currentId = _originalDto.Id;
                currentMa = _originalDto.MaTheLoai; // Lấy mã gốc để truyền đi khi cập nhật
                Debug.WriteLine($"btnLuu_Click: Preparing for UPDATE. ID={currentId}, Ma={currentMa ?? "null"}");
            }
            else
            {
                Debug.WriteLine($"btnLuu_Click: Preparing for ADD.");
            }


            // Tạo DTO để gửi đi
            TheLoaiDTO dtoToSave = new TheLoaiDTO
            {
                Id = currentId,
                MaTheLoai = currentMa, // Sẽ là null khi thêm, là mã gốc khi sửa
                TenTheLoai = tenTheLoai
            };

            // Vô hiệu hóa nút Lưu và Bỏ qua trong quá trình xử lý
            Debug.WriteLine("btnLuu_Click: Disabling Save and Cancel buttons.");
            btnLuu.Enabled = false;
            btnBoQua.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            bool success = false;
            string? errorMsg = null;
            TheLoaiDTO? savedDto = null; // DTO trả về từ BUS (quan trọng khi thêm mới để lấy ID/Mã)
            int? idToSelectAfterSave = null; // ID để chọn lại dòng sau khi lưu

            try
            {
                if (_isAdding)
                {
                    Debug.WriteLine("btnLuu_Click: Calling BUS AddTheLoaiAsync...");
                    savedDto = await _busTheLoai.AddTheLoaiAsync(dtoToSave);
                    success = savedDto != null;
                    if (success)
                    {
                        idToSelectAfterSave = savedDto!.Id; // Lấy ID từ kết quả trả về
                        Debug.WriteLine($"btnLuu_Click: Add successful. Returned ID: {idToSelectAfterSave}");
                    }
                    else
                    {
                        // Thử lấy thông điệp lỗi từ Exception nếu BUS ném ra lỗi cụ thể
                        // errorMsg = BUS layer exception message if available
                        errorMsg ??= "Thêm thể loại thất bại."; // Thông báo chung
                        Debug.WriteLine($"btnLuu_Click: Add failed (BUS returned null or threw exception). Error: {errorMsg}");
                    }
                }
                else // Đang sửa
                {
                    Debug.WriteLine($"btnLuu_Click: Calling BUS UpdateTheLoaiAsync (ID: {dtoToSave.Id})...");
                    success = await _busTheLoai.UpdateTheLoaiAsync(dtoToSave);
                    if (success)
                    {
                        idToSelectAfterSave = dtoToSave.Id; // ID không đổi khi cập nhật
                        Debug.WriteLine($"btnLuu_Click: Update successful for ID: {idToSelectAfterSave}");
                    }
                    else
                    {
                        // errorMsg = BUS layer exception message if available
                        errorMsg ??= "Cập nhật thể loại thất bại."; // Thông báo chung
                        Debug.WriteLine($"btnLuu_Click: Update failed (BUS returned false or threw exception). Error: {errorMsg}");
                    }
                }
            }
            // Bắt các loại lỗi cụ thể từ BUS/DAL
            catch (ArgumentException argEx) { Debug.WriteLine($"btnLuu_Click: Caught ArgumentException: {argEx.Message}"); errorMsg = argEx.Message; success = false; }
            catch (InvalidOperationException invOpEx) { Debug.WriteLine($"btnLuu_Click: Caught InvalidOperationException: {invOpEx.Message}"); errorMsg = invOpEx.Message; success = false; }
            // catch (DbUpdateException dbEx) { errorMsg = HandleDbUpdateException(dbEx); success = false; } // Nếu muốn xử lý lỗi DB cụ thể
            catch (Exception ex)
            {
                Debug.WriteLine($"*** btnLuu_Click: Caught unexpected Exception: {ex.ToString()}");
                HandleError("lưu thể loại", ex); // Hiển thị lỗi chung
                errorMsg = ex.Message;
                success = false;
            }
            finally
            {
                this.Cursor = Cursors.Default;
                Debug.WriteLine("btnLuu_Click: Cursor restored.");
                // Nút Lưu/Bỏ qua sẽ được cập nhật bởi SetControlState dựa trên kết quả 'success'
            }

            // Xử lý kết quả
            if (success)
            {
                Debug.WriteLine("btnLuu_Click: Operation successful.");
                bool wasAddingBeforeSave = _isAdding;

                // Reset trạng thái logic về chế độ xem *trước khi* tải lại lưới
                _isAdding = false;
                _originalDto = null;
                Debug.WriteLine("btnLuu_Click: State reset to view mode.");

                ShowInfo(wasAddingBeforeSave ? "Thêm thành công!" : "Cập nhật thành công!");

                // Xóa ô tìm kiếm để đảm bảo thấy dòng mới/vừa sửa
                if (txtTimKiem != null) txtTimKiem.Clear();
                Debug.WriteLine("btnLuu_Click: Search term cleared.");

                // Tải lại lưới và truyền ID của dòng vừa lưu để chọn lại
                Debug.WriteLine($"btnLuu_Click: Calling LoadDataGrid with idToSelect={idToSelectAfterSave}");
                await LoadDataGrid(idToSelectAfterSave); // LoadDataGrid sẽ gọi FilterAndDisplayData và xử lý chọn dòng
            }
            else // Thao tác thất bại
            {
                Debug.WriteLine($"btnLuu_Click: Operation failed. Error: {errorMsg ?? "Unknown"}");
                ShowError(errorMsg ?? (_isAdding ? "Thêm thất bại." : "Cập nhật thất bại."));
                // Giữ nguyên trạng thái thêm/sửa nếu thất bại
                // Kích hoạt lại các nút Lưu/Bỏ qua (nút Lưu sẽ dựa vào HasUnsavedChanges)
                Debug.WriteLine("btnLuu_Click: Operation failed, restoring Add/Edit mode button state.");
                SetControlState(_isAdding || IsEditing(), dgvTheLoai?.SelectedRows.Count > 0); // isAddingOrEditing vẫn là true
                                                                                               // Focus lại ô Tên nếu còn bật
                if (txtTenTheLoai != null && txtTenTheLoai.Enabled) txtTenTheLoai.Focus();
            }
        }

        private void btnBoQua_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnBoQua_Click triggered.");
            bool wasAdding = _isAdding;
            bool wasEditing = IsEditing();
            Debug.WriteLine($"btnBoQua_Click: State before cancel - wasAdding={wasAdding}, wasEditing={wasEditing}");

            // Hỏi xác nhận nếu đang thêm/sửa VÀ có thay đổi
            if ((wasAdding || wasEditing) && HasUnsavedChanges())
            {
                Debug.WriteLine("btnBoQua_Click: Has unsaved changes. Asking confirmation.");
                if (ShowConfirm("Bạn có thay đổi chưa lưu. Bạn có chắc chắn muốn hủy bỏ?") == DialogResult.No)
                {
                    Debug.WriteLine("btnBoQua_Click: User chose not to cancel.");
                    return; // Không làm gì nếu người dùng không muốn hủy
                }
                Debug.WriteLine("btnBoQua_Click: User confirmed cancellation.");
            }

            // Lưu lại ID dòng đang được chọn trước khi reset UI (nếu đang sửa)
            int? selectedIdBeforeCancel = null;
            if (wasEditing && dgvTheLoai?.SelectedRows.Count == 1 && dgvTheLoai.SelectedRows[0].DataBoundItem is TheLoaiDTO currentDto)
            {
                selectedIdBeforeCancel = currentDto.Id;
            }
            else if (wasEditing && _originalDto != null)
            { // Dự phòng nếu selection bị mất
                selectedIdBeforeCancel = _originalDto.Id;
            }

            // Reset trạng thái logic về xem
            _isAdding = false;
            _originalDto = null; // Luôn reset _originalDto khi bỏ qua
            Debug.WriteLine("btnBoQua_Click: State reset to view mode (_isAdding=false, _originalDto=null).");

            // Xóa ô tìm kiếm nếu có nội dung, để đảm bảo thấy lại dòng gốc nếu nó bị ẩn bởi bộ lọc
            if (txtTimKiem != null && !string.IsNullOrEmpty(txtTimKiem.Text))
            {
                Debug.WriteLine("btnBoQua_Click: Clearing search term.");
                txtTimKiem.Clear(); // Sẽ trigger FilterAndDisplayData, hiển thị lại toàn bộ
                                    // FilterAndDisplayData sẽ tự động ClearInput và SetControlState(false, false)
            }


            // Reset UI:
            // FilterAndDisplayData (được gọi bởi txtTimKiem.Clear() hoặc gọi trực tiếp nếu không clear search)
            // sẽ xử lý việc hiển thị lại danh sách và reset trạng thái control.
            if (string.IsNullOrEmpty(txtTimKiem?.Text)) // Nếu ô tìm kiếm đã rỗng sẵn
            {
                Debug.WriteLine("btnBoQua_Click: Search term was already empty. Calling FilterAndDisplayData directly.");
                FilterAndDisplayData(); // Gọi để reset UI về trạng thái xem, không chọn dòng
            }

            // Cố gắng chọn lại dòng đã chọn trước khi Bỏ qua (nếu đang Sửa)
            if (wasEditing && selectedIdBeforeCancel.HasValue)
            {
                Debug.WriteLine($"btnBoQua_Click: Attempting to re-select row ID: {selectedIdBeforeCancel.Value} after filter reset.");
                // Dùng BeginInvoke để đảm bảo việc chọn lại xảy ra sau khi FilterAndDisplayData hoàn tất
                this.BeginInvoke(new Action(() => {
                    if (!this.IsDisposed && this.IsHandleCreated)
                    { // Kiểm tra lại control
                        SelectRowById(selectedIdBeforeCancel.Value);
                    }
                }));
            }


            Debug.WriteLine("btnBoQua_Click finished.");
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnThoat_Click triggered.");
            // Hỏi xác nhận nếu đang thêm/sửa VÀ có thay đổi
            if ((_isAdding || IsEditing()) && HasUnsavedChanges())
            {
                Debug.WriteLine("btnThoat_Click: Has unsaved changes. Asking confirmation.");
                if (ShowConfirm("Bạn có thay đổi chưa lưu. Bạn có chắc chắn muốn thoát và hủy bỏ?") == DialogResult.No)
                {
                    Debug.WriteLine("btnThoat_Click: User chose not to exit.");
                    return; // Không thoát nếu người dùng không muốn hủy
                }
                Debug.WriteLine("btnThoat_Click: User confirmed exit with discard.");
            }

            // Reset trạng thái logic trước khi thoát (an toàn)
            _isAdding = false;
            _originalDto = null;
            Debug.WriteLine("btnThoat_Click: State reset. Invoking RequestClose.");

            // Gửi yêu cầu đóng UC này đến form cha
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        // --- DATAGRIDVIEW EVENT HANDLERS ---
        private void dgvTheLoai_SelectionChanged(object? sender, EventArgs e)
        {
            // Thêm log để theo dõi sự kiện này
            if (dgvTheLoai == null || dgvTheLoai.IsDisposed || !dgvTheLoai.IsHandleCreated)
            {
                Debug.WriteLine($"dgvTheLoai_SelectionChanged: Grid invalid. Skipping. IsNull={dgvTheLoai == null}, IsDisposed={dgvTheLoai?.IsDisposed}, IsHandleCreated={dgvTheLoai?.IsHandleCreated}");
                return;
            }

            Debug.WriteLine($"dgvTheLoai_SelectionChanged fired. SelectedRows.Count = {dgvTheLoai.SelectedRows.Count}. IsAdding={_isAdding}, IsEditing={IsEditing()}, GridEnabled={dgvTheLoai.Enabled}");

            // Chỉ xử lý nếu đang ở chế độ xem (không thêm/sửa).
            if (!_isAdding && !IsEditing())
            {
                Debug.WriteLine("dgvTheLoai_SelectionChanged: In View Mode. Calling DisplaySelectedRow.");
                DisplaySelectedRow(); // Hàm này sẽ cập nhật input và SetControlState
            }
            else
            {
                // Log lý do bỏ qua
                if (_isAdding) Debug.WriteLine("dgvTheLoai_SelectionChanged: Currently Adding. Skipping DisplaySelectedRow call.");
                else if (IsEditing()) Debug.WriteLine("dgvTheLoai_SelectionChanged: Currently Editing. Skipping DisplaySelectedRow call.");
                // else Debug.WriteLine("dgvTheLoai_SelectionChanged: Not in View Mode (State logic error?). Skipping DisplaySelectedRow call."); // Trường hợp này không nên xảy ra
            }
        }

        private void dgvTheLoai_DoubleClick(object? sender, EventArgs e)
        {
            Debug.WriteLine("dgvTheLoai_DoubleClick fired.");
            // Kiểm tra null an toàn và đảm bảo nút Sửa đang bật
            if (dgvTheLoai != null && dgvTheLoai.SelectedRows.Count == 1 && btnSua != null && btnSua.Enabled)
            {
                Debug.WriteLine("dgvTheLoai_DoubleClick: Conditions met. Calling btnSua_Click.");
                btnSua_Click(sender, e); // Gọi lại handler của nút Sửa
            }
            else
            {
                Debug.WriteLine($"dgvTheLoai_DoubleClick: Conditions not met. SelectedCount={dgvTheLoai?.SelectedRows.Count ?? -1}, IsSuaEnabled={btnSua?.Enabled}");
            }
        }

        // --- INPUT CHANGE HANDLER (Cho ô Tên Thể Loại) ---
        private void InputField_Changed(object? sender, EventArgs e) // Cho phép sender là null
        {
            // Chỉ cập nhật trạng thái nút Lưu khi đang thêm hoặc sửa
            if ((_isAdding || IsEditing()) && btnLuu != null)
            {
                // Chỉ cập nhật trạng thái Enabled của nút Lưu dựa trên HasUnsavedChanges.
                bool shouldBeEnabled = HasUnsavedChanges();
                if (btnLuu.Enabled != shouldBeEnabled)
                {
                    // Debug.WriteLine($"InputField_Changed: Updating btnLuu.Enabled to {shouldBeEnabled}");
                    btnLuu.Enabled = shouldBeEnabled;
                }
            }
        }


        // --- HELPER METHODS ---
        /// <summary>
        /// Bật hoặc tắt tất cả các nút hành động chính (Thêm, Sửa, Xóa, Lưu, Bỏ qua).
        /// </summary>
        private void SetAllActionButtonsEnabled(bool enabled)
        {
            if (btnThem != null) btnThem.Enabled = enabled;
            if (btnSua != null) btnSua.Enabled = enabled;
            if (btnXoa != null) btnXoa.Enabled = enabled;
            if (btnLuu != null) btnLuu.Enabled = enabled;
            if (btnBoQua != null) btnBoQua.Enabled = enabled;
            // Có thể thêm cả txtTimKiem nếu muốn tắt/bật nó cùng lúc
            // if (txtTimKiem != null) txtTimKiem.Enabled = enabled;
        }
        private bool SelectRowById(int id)
        {
            Debug.WriteLine($"SelectRowById: Attempting to select ID: {id}");
            // Kiểm tra grid hợp lệ
            if (dgvTheLoai == null || dgvTheLoai.IsDisposed || !dgvTheLoai.IsHandleCreated)
            {
                Debug.WriteLine($"SelectRowById: Grid invalid. Cannot select ID {id}.");
                return false;
            }
            // Kiểm tra có DataSource không
            if (dgvTheLoai.DataSource == null)
            {
                Debug.WriteLine($"SelectRowById: DataSource is null. Cannot select ID {id}.");
                return false;
            }
            // Kiểm tra có dòng nào không
            if (dgvTheLoai.Rows.Count == 0)
            {
                Debug.WriteLine($"SelectRowById: Grid has no rows. Cannot select ID {id}.");
                // Đảm bảo không có dòng nào được chọn nếu lưới rỗng
                try { dgvTheLoai.ClearSelection(); } catch { /* Ignore */ }
                return false;
            }


            bool found = false;
            // Tạm ngắt SelectionChanged
            Debug.WriteLine($"SelectRowById: Unsubscribing from SelectionChanged for programmatic selection.");
            dgvTheLoai.SelectionChanged -= dgvTheLoai_SelectionChanged;

            try
            {
                Debug.WriteLine($"SelectRowById: Clearing existing selection.");
                dgvTheLoai.ClearSelection(); // Bỏ chọn tất cả trước khi tìm

                // Tìm dòng theo ID trong DataSource hiện tại (có thể đã được lọc)
                DataGridViewRow? rowToSelect = null;
                foreach (DataGridViewRow row in dgvTheLoai.Rows) // Duyệt qua các dòng đang hiển thị
                {
                    if (row.DataBoundItem is TheLoaiDTO dto && dto.Id == id)
                    {
                        rowToSelect = row;
                        break;
                    }
                }

                if (rowToSelect != null)
                {
                    Debug.WriteLine($"SelectRowById: Row found at index {rowToSelect.Index} for ID {id}.");
                    // Chọn dòng tìm thấy.
                    try
                    {
                        // Đảm bảo dòng có thể chọn được (Visible)
                        if (!rowToSelect.Visible)
                        {
                            Debug.WriteLine($"SelectRowById: Row {rowToSelect.Index} is not visible. Cannot select.");
                            // Có thể cần xóa bộ lọc và thử lại nếu muốn? Hoặc báo lỗi?
                        }
                        else
                        {
                            rowToSelect.Selected = true;
                            Debug.WriteLine($"SelectRowById: Row {rowToSelect.Index} selected.");

                            // Cuộn đến dòng nếu cần
                            if (!IsRowFullyVisible(rowToSelect.Index))
                            {
                                Debug.WriteLine($"SelectRowById: Row {rowToSelect.Index} not fully visible. Scrolling.");
                                // Đảm bảo FirstDisplayedScrollingRowIndex hợp lệ
                                if (rowToSelect.Index >= 0 && rowToSelect.Index < dgvTheLoai.RowCount)
                                {
                                    dgvTheLoai.FirstDisplayedScrollingRowIndex = rowToSelect.Index;
                                }
                            }
                            found = true;
                        }
                    }
                    catch (Exception selectEx) when (selectEx is InvalidOperationException || selectEx is ArgumentOutOfRangeException)
                    {
                        Debug.WriteLine($"*** ERROR setting Selected=true or scrolling for row index {rowToSelect?.Index}: {selectEx.Message}");
                        found = false;
                    }
                }
                else
                {
                    Debug.WriteLine($"SelectRowById: Row with ID {id} not found in the current grid view.");
                    // Đảm bảo không có dòng nào được chọn nếu không tìm thấy
                    // dgvTheLoai.ClearSelection(); // Đã clear ở đầu try
                }
            }
            catch (Exception ex) // Bắt lỗi chung
            {
                Debug.WriteLine($"*** ERROR during SelectRowById ({id}): {ex.ToString()}");
                found = false;
                try { dgvTheLoai.ClearSelection(); } catch { /* Ignore */ } // Cố gắng clear selection khi lỗi
            }
            finally
            {
                // Gắn lại sự kiện SelectionChanged *một cách an toàn*
                if (dgvTheLoai != null && !dgvTheLoai.IsDisposed && dgvTheLoai.IsHandleCreated)
                {
                    Debug.WriteLine($"SelectRowById: Re-subscribing to SelectionChanged.");
                    dgvTheLoai.SelectionChanged -= dgvTheLoai_SelectionChanged; // Đảm bảo chỉ có 1
                    dgvTheLoai.SelectionChanged += dgvTheLoai_SelectionChanged;

                    // Nếu tìm thấy và chọn thành công, gọi lại handler một cách tường minh
                    // để đảm bảo UI state được cập nhật ngay lập tức
                    if (found)
                    {
                        Debug.WriteLine($"SelectRowById: Row selection successful (found=true). Manually triggering SelectionChanged handler via BeginInvoke.");
                        if (this.IsHandleCreated && !this.IsDisposed)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                // Kiểm tra lại grid và selection trong BeginInvoke
                                if (dgvTheLoai != null && !dgvTheLoai.IsDisposed && dgvTheLoai.IsHandleCreated && dgvTheLoai.SelectedRows.Count == 1)
                                {
                                    // Lấy ID từ dòng đang được chọn thực sự
                                    if (dgvTheLoai.SelectedRows[0].DataBoundItem is TheLoaiDTO selectedDto && selectedDto.Id == id)
                                    {
                                        Debug.WriteLine($"SelectRowById (BeginInvoke): Calling dgvTheLoai_SelectionChanged for selected ID {id}.");
                                        dgvTheLoai_SelectionChanged(dgvTheLoai, EventArgs.Empty);
                                    }
                                    else
                                    {
                                        Debug.WriteLine($"SelectRowById (BeginInvoke): Selected row ID does not match target ID {id}. Skipping manual trigger.");
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine($"SelectRowById (BeginInvoke): State changed or selection lost before handler execution. Skipping manual trigger for ID {id}.");
                                }
                            }));
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"SelectRowById: Row selection failed (found=false). Not manually triggering SelectionChanged handler.");
                        // Nếu không chọn được, UI state nên được cập nhật bởi FilterAndDisplayData hoặc LoadDataGrid trước đó
                        // Nếu đang ở chế độ xem, đảm bảo về trạng thái không chọn gì
                        if (!_isAdding && !IsEditing())
                        {
                            SetControlState(false, false);
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"SelectRowById: Grid became invalid before re-subscribing/triggering event.");
                }
            }
            Debug.WriteLine($"SelectRowById: Finished for ID {id}. Result (found): {found}");
            return found;
        }

        // Hàm SelectRowByMa không cần thiết nữa nếu chỉ tìm theo ID
        // private bool SelectRowByMa(string? maTheLoai) { ... } 


        private bool IsRowFullyVisible(int rowIndex)
        {
            if (dgvTheLoai == null || rowIndex < 0 || rowIndex >= dgvTheLoai.RowCount) return false;
            if (dgvTheLoai.IsDisposed || !dgvTheLoai.IsHandleCreated) return false; // Thêm kiểm tra

            try
            {
                Rectangle rowRect = dgvTheLoai.GetRowDisplayRectangle(rowIndex, true); // Lấy cả phần header của dòng
                // Kiểm tra xem toàn bộ hình chữ nhật của dòng có nằm trong vùng hiển thị của client không
                // ClientRectangle không bao gồm thanh cuộn, nên đây là cách kiểm tra tương đối chính xác
                return dgvTheLoai.ClientRectangle.Contains(rowRect);

                // Hoặc kiểm tra Top và Bottom một cách tường minh hơn:
                //return rowRect.Top >= dgvTheLoai.ClientRectangle.Top && 
                //       rowRect.Bottom <= dgvTheLoai.ClientRectangle.Bottom;
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentOutOfRangeException) // Bắt lỗi cụ thể hơn
            {
                Debug.WriteLine($"*** Error in IsRowFullyVisible for index {rowIndex}: {ex.Message}");
                return false; // Coi như không thấy nếu có lỗi
            }
        }


        // --- HELPER METHODS FOR MESSAGE BOXES ---
        private IWin32Window GetMessageBoxOwner()
        {
            // Ưu tiên form cha
            IWin32Window? owner = this.FindForm();
            if (owner != null && owner is Form form && !form.IsDisposed && form.IsHandleCreated)
            {
                return owner;
            }
            // Nếu không có form cha, dùng UserControl
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                return this;
            }

            // Fallback cuối cùng: Tìm form đang mở bất kỳ (ưu tiên form chính nếu có)
            Form? mainForm = Application.OpenForms.OfType<frmMain>().FirstOrDefault(f => f.IsHandleCreated && !f.IsDisposed);
            if (mainForm != null) return mainForm;

            Form? fallbackForm = Application.OpenForms.Cast<Form>().FirstOrDefault(f => f.IsHandleCreated && !f.IsDisposed);
            if (fallbackForm != null) return fallbackForm;

            // Nếu không có gì hợp lệ
            Debug.WriteLine("*** GetMessageBoxOwner WARNING: No valid owner or fallback form found. Returning 'this' as last resort (might be disposed). ***");
            return this; // Fallback cuối cùng
        }

        // Các hàm Show Message giữ nguyên
        private void HandleError(string action, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR ({action}): {ex}");
            string message = $"Lỗi khi {action}: {ex.Message}";
            try { MaterialMessageBox.Show(GetMessageBoxOwner(), message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            catch { MessageBox.Show(message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ShowError(string message)
        {
            try { MaterialMessageBox.Show(GetMessageBoxOwner(), message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            catch { MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ShowWarning(string message)
        {
            try { MaterialMessageBox.Show(GetMessageBoxOwner(), message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            catch { MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void ShowInfo(string message)
        {
            try { MaterialMessageBox.Show(GetMessageBoxOwner(), message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            catch { MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }

        private DialogResult ShowConfirm(string message)
        {
            try { return MaterialMessageBox.Show(GetMessageBoxOwner(), message, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question); }
            catch { return MessageBox.Show(message, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question); }
        }

    } // End class ucQuanLyTheLoai
} // End namespace GUI