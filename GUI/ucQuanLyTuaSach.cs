// Project/Namespace: GUI
using BUS;
using DTO;
// using MaterialSkin; // Bỏ using này nếu không dùng MaterialSkinManager ở đâu khác
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms; // Đảm bảo using này đã có
using Microsoft.Extensions.DependencyInjection;
using System.Drawing; // Cần cho Point, Size etc.
// using GUI.Interfaces; // Giả sử interface nằm ở đây, bạn cần using đúng namespace

namespace GUI
{
    // Giả sử bạn có interface IRequiresDataLoading như sau:
    /*
    namespace GUI.Interfaces // Hoặc namespace phù hợp
    {
        public interface IRequiresDataLoading
        {
            Task InitializeDataAsync();
        }
    }
    */

    // Thêm kế thừa interface (ví dụ: IRequiresDataLoading) vào khai báo lớp
    public partial class ucQuanLyTuaSach : UserControl, IRequiresDataLoading // <<< Đảm bảo đã bỏ comment nếu dùng interface
    {
        // --- Dependencies ---
        private readonly IBUSTuaSach _busTuaSach;
        private readonly IBUSTheLoai _busTheLoai;
        private readonly IBUSTacGia _busTacGia;
        // Bỏ IServiceProvider nếu không dùng cho mục đích khác
        // private readonly IServiceProvider _serviceProvider;

        // --- State ---
        private bool _isAdding = false; // True khi đang ở chế độ Thêm mới
        private System.Windows.Forms.Timer? _searchDebounceTimer; // Khai báo Timer với namespace đầy đủ
        private int? _lastSelectedId = null; // Lưu ID dòng được chọn cuối cùng
        private bool _isDataInitialized = false; // Cờ đánh dấu đã load data lần đầu chưa

        // --- Constructor ---
        // Bỏ IServiceProvider nếu không dùng
        public ucQuanLyTuaSach(IBUSTuaSach busTuaSach, IBUSTheLoai busTheLoai, IBUSTacGia busTacGia /*, IServiceProvider serviceProvider */)
        {
            Debug.WriteLine(">>> Entering ucQuanLyTuaSach Constructor (Modernized)...");

            try
            {
                // Gán các dependencies được inject
                _busTuaSach = busTuaSach ?? throw new ArgumentNullException(nameof(busTuaSach));
                _busTheLoai = busTheLoai ?? throw new ArgumentNullException(nameof(busTheLoai));
                _busTacGia = busTacGia ?? throw new ArgumentNullException(nameof(busTacGia));
                // _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
                Debug.WriteLine(">>> Dependencies assigned SUCCESSFULLY.");
            }
            catch (ArgumentNullException argNullEx)
            {
                // Ghi log lỗi nghiêm trọng nếu dependency injection thất bại
                Debug.WriteLine($"*** CRITICAL ERROR: Dependency Injection Failed: {argNullEx.ToString()}");
                // Hiển thị thông báo lỗi cho người dùng
                MaterialMessageBox.Show(null, $"Lỗi nghiêm trọng khi khởi tạo dịch vụ cho Quản lý Tựa sách:\n{argNullEx.Message}\n\nVui lòng kiểm tra cấu hình Dependency Injection.", "Lỗi Khởi Tạo Dịch Vụ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Enabled = false; // Vô hiệu hóa UserControl nếu có lỗi nghiêm trọng
                throw; // Ném lại ngoại lệ để ngăn chặn việc tiếp tục thực thi
            }

            Debug.WriteLine(">>> Calling InitializeComponent()...");
            try
            {
                // Khởi tạo các thành phần giao diện được định nghĩa trong file Designer
                InitializeComponent();
                Debug.WriteLine(">>> InitializeComponent() Finished SUCCESSFULLY.");
            }
            catch (Exception exInit)
            {
                // Ghi log lỗi nghiêm trọng nếu InitializeComponent thất bại
                Debug.WriteLine($"*** CRITICAL ERROR DURING InitializeComponent() for ucQuanLyTuaSach: {exInit.ToString()}");
                // Hiển thị thông báo lỗi cho người dùng
                MaterialMessageBox.Show(null, $"Lỗi nghiêm trọng khi khởi tạo giao diện Quản lý Tựa sách:\n{exInit.Message}\n\nXem chi tiết trong Output Debug.", "Lỗi Khởi Tạo Giao Diện", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Enabled = false; // Vô hiệu hóa UserControl
                throw; // Ném lại ngoại lệ
            }

            // Khởi tạo các ComboBox và CheckedListBox ban đầu
            InitializeComboBoxesAndLists();
            Debug.WriteLine(">>> InitializeComboBoxesAndLists() called.");

            // Khởi tạo Timer cho việc trì hoãn tìm kiếm
            InitializeSearchDebounceTimer();
            Debug.WriteLine(">>> InitializeSearchDebounceTimer() called.");

            Debug.WriteLine(">>> Exiting ucQuanLyTuaSach Constructor (Modernized).");
        }

        // --- Interface Implementation (IRequiresDataLoading) ---

        /// <summary>
        /// Tải dữ liệu ban đầu cần thiết cho UserControl.
        /// Được gọi bởi form cha hoặc cơ chế quản lý UserControl.
        /// </summary>
        public async Task InitializeDataAsync()
        {
            // Chỉ load data lần đầu tiên
            if (_isDataInitialized)
            {
                Debug.WriteLine("InitializeDataAsync skipped: Data already initialized.");
                return;
            }

            Debug.WriteLine(">>> Entering InitializeDataAsync...");
            // Kiểm tra control trước khi load data
            if (!CheckRequiredControls())
            {
                MaterialMessageBox.Show(this.FindForm(), "Lỗi nghiêm trọng: Không thể tải dữ liệu do thiếu thành phần giao diện.", "Lỗi Control", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Enabled = false;
                Debug.WriteLine("<<< Exiting InitializeDataAsync due to missing controls.");
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            try
            {
                // Load dữ liệu cho các bộ lọc
                await LoadFilterDataAsync();
                Debug.WriteLine(">>> InitializeDataAsync: Filter data loaded.");

                // Load dữ liệu ban đầu cho DataGridView
                await LoadFilteredDataGrid(); // Load với bộ lọc mặc định (Tất cả)
                Debug.WriteLine(">>> InitializeDataAsync: Initial DataGrid loaded.");

                // Đặt trạng thái control ban đầu (chế độ xem, rowSelected tùy thuộc vào kết quả load)
                SetControlState(isEditingModeActive: false, rowSelected: dgvTuaSach?.SelectedRows.Count > 0);
                Debug.WriteLine(">>> InitializeDataAsync: Initial control state set.");

                _isDataInitialized = true; // Đánh dấu đã load xong
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR during InitializeDataAsync: {ex.ToString()}");
                MaterialMessageBox.Show(this.FindForm(), $"Lỗi nghiêm trọng khi tải dữ liệu ban đầu: {ex.Message}", "Lỗi Tải Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Enabled = false; // Vô hiệu hóa nếu load lỗi
            }
            finally
            {
                // Đảm bảo con trỏ chuột được đặt lại ngay cả khi có lỗi
                if (this.IsHandleCreated)
                {
                    this.BeginInvoke((MethodInvoker)delegate { this.Cursor = Cursors.Default; });
                }
                else
                {
                    this.Cursor = Cursors.Default; // Thử đặt trực tiếp nếu handle chưa tạo
                }
            }
            Debug.WriteLine("<<< Exiting InitializeDataAsync.");
        }


        // --- Initialization Methods ---

        /// <summary>
        /// Khởi tạo các ComboBox và CheckedListBox.
        /// </summary>
        private void InitializeComboBoxesAndLists()
        {
            // Cấu hình ComboBox Thể loại trong phần Details
            if (cbTheLoai != null)
            {
                cbTheLoai.DisplayMember = nameof(TheLoaiDTO.TenTheLoai);
                cbTheLoai.ValueMember = nameof(TheLoaiDTO.Id);
                cbTheLoai.DataSource = null; // Sẽ load dữ liệu sau
            }
            else { Debug.WriteLine("WARNING: cbTheLoai (Details) is null during InitializeComboBoxesAndLists!"); }

            // Xóa các mục cũ trong CheckedListBox Tác giả trong phần Details
            if (clbTacGia != null)
            {
                clbTacGia.Items.Clear();
            }
            else { Debug.WriteLine("WARNING: clbTacGia (Details) is null during InitializeComboBoxesAndLists!"); }

            // Cấu hình ComboBox lọc Thể loại
            if (cbFilterTheLoai != null)
            {
                cbFilterTheLoai.DisplayMember = nameof(TheLoaiDTO.TenTheLoai);
                cbFilterTheLoai.ValueMember = nameof(TheLoaiDTO.Id);
                cbFilterTheLoai.DataSource = null; // Sẽ load dữ liệu sau
            }
            else { Debug.WriteLine("WARNING: cbFilterTheLoai is null during InitializeComboBoxesAndLists!"); }

            // Cấu hình ComboBox lọc Tác giả
            if (cbFilterTacGia != null)
            {
                cbFilterTacGia.DisplayMember = nameof(TacGiaDTO.TenTacGia);
                cbFilterTacGia.ValueMember = nameof(TacGiaDTO.Id);
                cbFilterTacGia.DataSource = null; // Sẽ load dữ liệu sau
            }
            else { Debug.WriteLine("WARNING: cbFilterTacGia is null during InitializeComboBoxesAndLists!"); }
        }

        /// <summary>
        /// Khởi tạo Timer cho việc trì hoãn tìm kiếm.
        /// </summary>
        private void InitializeSearchDebounceTimer()
        {
            _searchDebounceTimer = new System.Windows.Forms.Timer();
            _searchDebounceTimer.Interval = 500; // Đặt khoảng thời gian chờ (ms)
            _searchDebounceTimer.Tick += async (timerSender, timerE) =>
            {
                // Dừng Timer và thực hiện tìm kiếm/lọc
                _searchDebounceTimer.Stop();
                Debug.WriteLine("Search debounce timer ticked. Loading filtered data...");
                await LoadFilteredDataGrid();
                // *** FIX: Gọi SetControlState sau khi LoadFilteredDataGrid hoàn tất ***
                if (this.IsHandleCreated) { this.BeginInvoke((MethodInvoker)delegate { SetControlState(false, dgvTuaSach?.SelectedRows.Count > 0); }); }
            };
        }

        /// <summary>
        /// Kiểm tra sự tồn tại của các control quan trọng.
        /// </summary>
        /// <returns>True nếu tất cả control cần thiết tồn tại, ngược lại False.</returns>
        private bool CheckRequiredControls()
        {
            // Kiểm tra các control chính
            bool controlsOk = true;
            if (dgvTuaSach == null) { Debug.WriteLine("CRITICAL ERROR: dgvTuaSach is null!"); controlsOk = false; }
            if (cbTheLoai == null) { Debug.WriteLine("CRITICAL ERROR: cbTheLoai (Details) is null!"); controlsOk = false; }
            if (clbTacGia == null) { Debug.WriteLine("CRITICAL ERROR: clbTacGia (Details) is null!"); controlsOk = false; }
            if (txtId == null) { Debug.WriteLine("CRITICAL ERROR: txtId is null!"); controlsOk = false; }
            if (txtMaTuaSach == null) { Debug.WriteLine("CRITICAL ERROR: txtMaTuaSach is null!"); controlsOk = false; }
            if (txtTenTuaSach == null) { Debug.WriteLine("CRITICAL ERROR: txtTenTuaSach is null!"); controlsOk = false; }
            if (btnLuu == null || btnBoQua == null || btnThem == null || btnSua == null || btnXoa == null ) { Debug.WriteLine("CRITICAL ERROR: One or more main buttons are null!"); controlsOk = false; }

            // Kiểm tra các panel/layout chứa control
            if (panelGridAndButtons == null) { Debug.WriteLine("CRITICAL ERROR: panelGridAndButtons is null!"); controlsOk = false; }
            if (panelFilterSearch == null) { Debug.WriteLine("CRITICAL ERROR: panelFilterSearch is null!"); controlsOk = false; }
            if (materialCardDetails == null) { Debug.WriteLine("CRITICAL ERROR: materialCardDetails is null!"); controlsOk = false; }
            if (tableLayoutPanelDetails == null) { Debug.WriteLine("CRITICAL ERROR: tableLayoutPanelDetails is null!"); controlsOk = false; }
            if (flowLayoutPanelFilter == null) { Debug.WriteLine("CRITICAL ERROR: flowLayoutPanelFilter is null!"); controlsOk = false; }
            if (flowLayoutPanelButtonsGrid == null) { Debug.WriteLine("CRITICAL ERROR: flowLayoutPanelButtonsGrid is null!"); controlsOk = false; }
            if (flowLayoutPanelButtonsDetails == null) { Debug.WriteLine("CRITICAL ERROR: flowLayoutPanelButtonsDetails is null!"); controlsOk = false; }


            // Kiểm tra các control lọc (cảnh báo nếu thiếu)
            if (txtSearch == null) { Debug.WriteLine("WARNING: txtSearch (Filter) is null!"); }
            if (cbFilterTheLoai == null) { Debug.WriteLine("WARNING: cbFilterTheLoai is null!"); }
            if (cbFilterTacGia == null) { Debug.WriteLine("WARNING: cbFilterTacGia is null!"); }
            if (btnResetFilter == null) { Debug.WriteLine("WARNING: btnResetFilter is null!"); }

            return controlsOk;
        }


        // --- Event Handlers ---

        /// <summary>
        /// Xử lý sự kiện Load của UserControl. Chỉ thực hiện các thiết lập ban đầu không liên quan đến tải dữ liệu.
        /// </summary>
        private void ucQuanLyTuaSach_Load(object sender, EventArgs e)
        {
            Debug.WriteLine(">>> Entering ucQuanLyTuaSach_Load (Modernized)...");
            // Bỏ qua nếu đang ở chế độ Design hoặc UserControl bị vô hiệu hóa
            if (this.DesignMode || !this.Enabled)
            {
                Debug.WriteLine($"<<< Exiting ucQuanLyTuaSach_Load early (DesignMode: {this.DesignMode}, Enabled: {this.Enabled}).");
                return;
            }

            // Kiểm tra control cần thiết cho UI cơ bản
            if (!CheckRequiredControls())
            {
                // Lỗi đã được báo trong InitializeDataAsync nếu nó được gọi trước Load
                // Nếu InitializeDataAsync chưa được gọi, báo lỗi ở đây
                if (!_isDataInitialized)
                {
                    MaterialMessageBox.Show(this.FindForm(), "Lỗi nghiêm trọng: Không thể khởi tạo các thành phần giao diện cần thiết.", "Lỗi Control", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Enabled = false;
                }
                Debug.WriteLine("<<< Exiting ucQuanLyTuaSach_Load due to missing controls.");
                return;
            }

            // Đặt trạng thái ban đầu (có thể đã được set trong InitializeDataAsync nếu nó chạy trước)
            if (!_isDataInitialized)
            {
                _isAdding = false;
                ClearInputFields();
                SetControlState(isEditingModeActive: false, rowSelected: false);
                Debug.WriteLine(">>> ucQuanLyTuaSach_Load: Initial state set (because data not yet initialized).");
            }
            else
            {
                Debug.WriteLine(">>> ucQuanLyTuaSach_Load: Initial state already set by InitializeDataAsync.");
            }

            // Không gọi tải dữ liệu ở đây nữa, nó đã được chuyển sang InitializeDataAsync
            Debug.WriteLine("<<< Exiting ucQuanLyTuaSach_Load (Modernized) - Data loading moved to InitializeDataAsync.");
        }

        // --- Data Loading Methods ---

        /// <summary>
        /// Load dữ liệu cho các ComboBox lọc (Thể loại, Tác giả).
        /// </summary>
        private async Task LoadFilterDataAsync()
        {
            Debug.WriteLine("--- Starting LoadFilterDataAsync ---");
            try
            {
                // Load Thể loại cho ComboBox lọc
                if (cbFilterTheLoai != null && _busTheLoai != null)
                {
                    var allTheLoai = await _busTheLoai.GetAllTheLoaiAsync() ?? new List<TheLoaiDTO>();
                    // Tạo danh sách với mục "Tất cả"
                    var filterTheLoaiList = new List<TheLoaiDTO> { new TheLoaiDTO { Id = 0, TenTheLoai = "Tất cả Thể loại" } };
                    filterTheLoaiList.AddRange(allTheLoai.Where(tl => tl != null)); // Lọc bỏ các DTO null
                    cbFilterTheLoai.DataSource = filterTheLoaiList;
                    cbFilterTheLoai.SelectedValue = 0; // Chọn "Tất cả" làm mặc định
                    Debug.WriteLine($"Filter TheLoai loaded: {filterTheLoaiList.Count} items.");
                }
                else { Debug.WriteLine("Skipping Filter TheLoai load (control or BUS is null)."); }

                // Load Tác giả cho ComboBox lọc
                if (cbFilterTacGia != null && _busTacGia != null)
                {
                    var allTacGia = await _busTacGia.GetAllTacGiaAsync() ?? new List<TacGiaDTO>();
                    // Tạo danh sách với mục "Tất cả"
                    var filterTacGiaList = new List<TacGiaDTO> { new TacGiaDTO { Id = 0, TenTacGia = "Tất cả Tác giả" } };
                    // Chỉ thêm các tác giả chưa bị xóa mềm (DaAn == false)
                    filterTacGiaList.AddRange(allTacGia.Where(tg => tg != null && tg.DaAn == false));
                    cbFilterTacGia.DataSource = filterTacGiaList;
                    cbFilterTacGia.SelectedValue = 0; // Chọn "Tất cả" làm mặc định
                    Debug.WriteLine($"Filter TacGia loaded: {filterTacGiaList.Count} items.");
                }
                else { Debug.WriteLine("Skipping Filter TacGia load (control or BUS is null)."); }
            }
            catch (Exception ex)
            {
                // Ghi log và hiển thị lỗi nếu có vấn đề khi tải dữ liệu lọc
                Debug.WriteLine($"*** ERROR loading filter data: {ex.ToString()}");
                MaterialMessageBox.Show(this.FindForm(), $"Lỗi tải dữ liệu bộ lọc: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Ném lại lỗi để InitializeDataAsync có thể bắt và xử lý
                throw;
            }
            Debug.WriteLine("--- Exiting LoadFilterDataAsync ---");
        }

        /// <summary>
        /// Load dữ liệu cho các control trong phần nhập liệu chi tiết (ComboBox Thể loại, CheckedListBox Tác giả).
        /// </summary>
        private async Task LoadEditorDataAsync()
        {
            Debug.WriteLine("--- Starting LoadEditorDataAsync (Details section) ---");
            // Sử dụng Task.WhenAll để chạy song song việc load Thể loại và Tác giả
            await Task.WhenAll(
                LoadTheLoaiComboBoxData(),
                LoadTacGiaCheckedListData()
            );
            Debug.WriteLine("--- Exiting LoadEditorDataAsync (Details section) ---");
        }

        /// <summary>
        /// Load dữ liệu cho ComboBox Thể loại trong phần Details.
        /// </summary>
        private async Task LoadTheLoaiComboBoxData()
        {
            Debug.WriteLine("+++ Starting LoadTheLoaiComboBoxData (Details) +++");
            if (cbTheLoai == null || _busTheLoai == null)
            {
                Debug.WriteLine("LoadTheLoaiComboBoxData skipped: Control or BUS is null.");
                return;
            }

            try
            {
                // Lưu lại giá trị đang được chọn (nếu có)
                object? currentSelection = cbTheLoai.SelectedValue;

                cbTheLoai.DataSource = null; // Xóa nguồn dữ liệu cũ

                Debug.WriteLine("LoadTheLoaiComboBoxData: Calling _busTheLoai.GetAllTheLoaiAsync()...");
                List<TheLoaiDTO>? danhSachTheLoai = null;
                try
                {
                    danhSachTheLoai = await _busTheLoai.GetAllTheLoaiAsync();
                    Debug.WriteLine($"LoadTheLoaiComboBoxData: Call completed. Result is {(danhSachTheLoai == null ? "NULL" : $"a list with {danhSachTheLoai.Count} items")}.");
                }
                catch (Exception busEx)
                {
                    Debug.WriteLine($"*** ERROR calling _busTheLoai.GetAllTheLoaiAsync(): {busEx.ToString()}");
                    MaterialMessageBox.Show(this.FindForm(), $"Lỗi khi gọi service thể loại: {busEx.Message}", "Lỗi Service", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    danhSachTheLoai = new List<TheLoaiDTO>(); // Khởi tạo list rỗng nếu lỗi
                }

                // Lọc bỏ các DTO null
                danhSachTheLoai = danhSachTheLoai?.Where(dto => dto != null).ToList() ?? new List<TheLoaiDTO>();
                Debug.WriteLine($"LoadTheLoaiComboBoxData: Processed list has {danhSachTheLoai.Count} non-null DTOs.");

                // Gán DataSource mới
                cbTheLoai.DataSource = danhSachTheLoai;

                // Khôi phục lại lựa chọn trước đó nếu có thể
                if (currentSelection != null && currentSelection is int currentId && danhSachTheLoai.Any(tl => tl.Id == currentId))
                {
                    try
                    {
                        cbTheLoai.SelectedValue = currentId;
                        Debug.WriteLine($"LoadTheLoaiComboBoxData: Restored selection to ID {currentId}.");
                    }
                    catch (Exception exSetSelVal)
                    {
                        Debug.WriteLine($"Warning setting SelectedValue to ID {currentId}: {exSetSelVal.Message}. Setting to -1.");
                        cbTheLoai.SelectedIndex = -1;
                    }
                }
                else
                {
                    cbTheLoai.SelectedIndex = -1; // Không chọn gì nếu không khôi phục được
                    Debug.WriteLine("LoadTheLoaiComboBoxData: Could not restore selection or list is empty. Setting SelectedIndex to -1.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** Unexpected ERROR in LoadTheLoaiComboBoxData: {ex.ToString()}");
                MaterialMessageBox.Show(this.FindForm(), $"Lỗi tải danh sách thể loại: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Debug.WriteLine("+++ Exiting LoadTheLoaiComboBoxData (Details) +++");
        }

        /// <summary>
        /// Load dữ liệu cho CheckedListBox Tác giả trong phần Details.
        /// </summary>
        private async Task LoadTacGiaCheckedListData()
        {
            Debug.WriteLine("+++ Starting LoadTacGiaCheckedListData (Details) +++");
            if (clbTacGia == null || _busTacGia == null)
            {
                Debug.WriteLine("LoadTacGiaCheckedListData skipped: Control or BUS is null.");
                return;
            }

            try
            {
                // Lưu lại các ID đang được check trên UI
                var checkedIds = GetSelectedTacGiaIdsFromUI();

                clbTacGia.Items.Clear(); // Xóa các mục cũ

                Debug.WriteLine("LoadTacGiaCheckedListData: Calling _busTacGia.GetAllTacGiaAsync()...");
                List<TacGiaDTO>? danhSachTacGia = null;
                try
                {
                    danhSachTacGia = await _busTacGia.GetAllTacGiaAsync();
                    Debug.WriteLine($"LoadTacGiaCheckedListData: Call completed. Result is {(danhSachTacGia == null ? "NULL" : $"a list with {danhSachTacGia.Count} items")}.");
                }
                catch (Exception busEx)
                {
                    Debug.WriteLine($"*** ERROR calling _busTacGia.GetAllTacGiaAsync(): {busEx.ToString()}");
                    MaterialMessageBox.Show(this.FindForm(), $"Lỗi khi gọi service tác giả: {busEx.Message}", "Lỗi Service", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    danhSachTacGia = new List<TacGiaDTO>(); // Khởi tạo list rỗng nếu lỗi
                }

                // Lọc bỏ các DTO null và các tác giả đã bị xóa mềm (DaAn == false)
                danhSachTacGia = danhSachTacGia?.Where(dto => dto != null && dto.DaAn == false).ToList() ?? new List<TacGiaDTO>();
                Debug.WriteLine($"LoadTacGiaCheckedListData: Processed list has {danhSachTacGia.Count} non-null, non-deleted DTOs.");

                // Thêm các MaterialCheckbox vào CheckedListBox
                foreach (var tacGia in danhSachTacGia)
                {
                    var item = new MaterialCheckbox
                    {
                        Text = tacGia.TenTacGia ?? $"ID_{tacGia.Id}_NULL_NAME",
                        Tag = tacGia.Id, // Lưu ID vào Tag để lấy lại sau
                        AutoSize = true,
                        Checked = checkedIds.Contains(tacGia.Id) // Check lại dựa trên ID đã lưu
                    };
                    clbTacGia.Items.Add(item);
                }
                Debug.WriteLine($"LoadTacGiaCheckedListData: Added {clbTacGia.Items.Count} checkboxes.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** Unexpected ERROR in LoadTacGiaCheckedListData: {ex.ToString()}");
                MaterialMessageBox.Show(this.FindForm(), $"Lỗi tải danh sách tác giả: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Debug.WriteLine("+++ Exiting LoadTacGiaCheckedListData (Details) +++");
        }


        /// <summary>
        /// Load dữ liệu vào DataGridView dựa trên các bộ lọc hiện tại.
        /// </summary>
        private async Task LoadFilteredDataGrid()
        {
            if (dgvTuaSach == null || _busTuaSach == null)
            {
                Debug.WriteLine("LoadFilteredDataGrid skipped: dgvTuaSach or _busTuaSach is null.");
                return;
            }

            Debug.WriteLine("--- Starting LoadFilteredDataGrid ---");
            this.Cursor = Cursors.WaitCursor; // Hiển thị con trỏ chờ

            try
            {
                dgvTuaSach.DataSource = null; // Xóa dữ liệu cũ

                // Lấy giá trị từ các control lọc
                string? searchText = txtSearch?.Text;
                int theLoaiId = (int)(cbFilterTheLoai?.SelectedValue ?? 0);
                int tacGiaId = (int)(cbFilterTacGia?.SelectedValue ?? 0);

                Debug.WriteLine($"LoadFilteredDataGrid: Calling BUS.SearchAndFilterTuaSachAsync with Search='{searchText}', TheLoaiId={theLoaiId}, TacGiaId={tacGiaId}");

                List<TuaSachDTO>? danhSach = null;
                try
                {
                    // Gọi BUS để lấy dữ liệu đã lọc
                    danhSach = await _busTuaSach.SearchAndFilterTuaSachAsync(searchText, theLoaiId, tacGiaId);
                    Debug.WriteLine($"LoadFilteredDataGrid: BUS call completed. Result is {(danhSach == null ? "NULL" : $"{danhSach.Count} items")}.");
                }
                catch (Exception busEx)
                {
                    Debug.WriteLine($"*** ERROR calling _busTuaSach.SearchAndFilterTuaSachAsync(): {busEx.ToString()}");
                    MaterialMessageBox.Show(this.FindForm(), $"Lỗi khi tìm kiếm/lọc tựa sách: {busEx.Message}", "Lỗi Service", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    danhSach = new List<TuaSachDTO>(); // Gán list rỗng nếu lỗi
                    // Ném lại lỗi để InitializeDataAsync có thể bắt và xử lý (nếu đây là lần load đầu)
                    if (!_isDataInitialized) throw;
                }

                // Lọc bỏ DTO null (phòng trường hợp lỗi mapping)
                danhSach = danhSach?.Where(dto => dto != null).ToList() ?? new List<TuaSachDTO>();
                Debug.WriteLine($"LoadFilteredDataGrid: Processed list has {danhSach.Count} non-null DTOs.");

                // Gán dữ liệu vào DataGridView và cấu hình cột
                dgvTuaSach.DataSource = danhSach;
                SetupDataGridViewColumns();

                // Chọn lại dòng đã chọn trước đó nếu có và không ở chế độ sửa
                bool isCurrentlyEditing = _isAdding || (btnLuu != null && btnLuu.Enabled); // Kiểm tra lại trạng thái sửa
                if (!isCurrentlyEditing && _lastSelectedId.HasValue)
                {
                    SelectRowById(_lastSelectedId.Value);
                }
                else if (!isCurrentlyEditing) // Nếu không sửa và không có ID lưu -> xóa chọn, xóa input
                {
                    dgvTuaSach.ClearSelection();
                    ClearInputFields();
                    Debug.WriteLine($"LoadFilteredDataGrid: Cleared selection & input (Not Editing, LastSelectedId: {_lastSelectedId}).");
                }
                else // Nếu đang sửa -> chỉ xóa chọn grid
                {
                    dgvTuaSach.ClearSelection();
                    Debug.WriteLine($"LoadFilteredDataGrid: Cleared grid selection (Editing: {isCurrentlyEditing}).");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR loading filtered DataGrid: {ex.ToString()}");
                // Chỉ hiển thị MessageBox nếu không phải lỗi từ lần load đầu tiên (đã được xử lý trong InitializeDataAsync)
                if (_isDataInitialized)
                {
                    MaterialMessageBox.Show(this.FindForm(), $"Lỗi khi tải danh sách tựa sách đã lọc: {ex.Message}", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Ném lại lỗi để InitializeDataAsync xử lý
                    throw;
                }
            }
            finally
            {
                // Chỉ trả lại con trỏ chuột, không set state ở đây nữa
                if (this.IsHandleCreated)
                {
                    this.BeginInvoke((MethodInvoker)delegate {
                        this.Cursor = Cursors.Default;
                        Debug.WriteLine($"--- Exiting LoadFilteredDataGrid (Row Selected After Load: {dgvTuaSach?.SelectedRows.Count > 0}) ---");
                    });
                }
                else
                {
                    Debug.WriteLine($"--- Exiting LoadFilteredDataGrid (Handle not created) ---");
                    this.Cursor = Cursors.Default; // Thử đặt lại trực tiếp
                }
            }
        }


        /// <summary>
        /// Cấu hình giao diện và hiển thị các cột trong DataGridView.
        /// </summary>
        private void SetupDataGridViewColumns()
        {
            if (dgvTuaSach == null || dgvTuaSach.Columns.Count == 0)
            {
                Debug.WriteLine("SetupDataGridViewColumns skipped: dgvTuaSach is null or has no columns.");
                return;
            }
            Debug.WriteLine("--- Starting SetupDataGridViewColumns ---");

            try
            {
                // Tắt tự động tạo cột nếu chưa tắt
                dgvTuaSach.AutoGenerateColumns = false;

                // Xóa các cột hiện có nếu cần thiết (để tránh trùng lặp khi gọi lại)
                // dgvTuaSach.Columns.Clear(); // Cân nhắc nếu bạn thêm cột thủ công

                // Thiết lập thuộc tính chung cho DataGridView (đã làm trong Designer)
                // dgvTuaSach.EnableHeadersVisualStyles = false;
                // dgvTuaSach.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                // ... (các thuộc tính khác đã set trong Designer)

                // Định dạng các cột hiện có (được tạo tự động từ DataSource hoặc thêm thủ công)
                var columns = dgvTuaSach.Columns;

                // Cột ID (Ẩn)
                var idCol = columns[nameof(TuaSachDTO.Id)];
                if (idCol != null) idCol.Visible = false;

                // Cột Mã Tựa Sách
                var maTuaSachCol = columns[nameof(TuaSachDTO.MaTuaSach)];
                if (maTuaSachCol != null)
                {
                    maTuaSachCol.HeaderText = "Mã Tựa Sách";
                    maTuaSachCol.Width = 120;
                    maTuaSachCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // Đặt kích thước cố định
                }

                // Cột Tên Tựa Sách
                var tenTuaSachCol = columns[nameof(TuaSachDTO.TenTuaSach)];
                if (tenTuaSachCol != null)
                {
                    tenTuaSachCol.HeaderText = "Tên Tựa Sách";
                    tenTuaSachCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // Chiếm phần còn lại
                }

                // Cột ID Thể Loại (Ẩn)
                var idTheLoaiCol = columns[nameof(TuaSachDTO.IdTheLoai)];
                if (idTheLoaiCol != null) idTheLoaiCol.Visible = false;

                // Cột Tên Thể Loại
                var tenTheLoaiCol = columns[nameof(TuaSachDTO.TenTheLoai)];
                if (tenTheLoaiCol != null)
                {
                    tenTheLoaiCol.HeaderText = "Thể Loại";
                    tenTheLoaiCol.Width = 150;
                    tenTheLoaiCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // FIX: Đặt AutoSizeMode cho tenTheLoaiCol
                }

                // Cột Danh sách Tác Giả (DTO) (Ẩn)
                var tacGiasCol = columns[nameof(TuaSachDTO.TacGias)];
                if (tacGiasCol != null) tacGiasCol.Visible = false;

                // Cột Tổng Số Cuốn
                var soLuongSachCol = columns[nameof(TuaSachDTO.SoLuongSach)];
                if (soLuongSachCol != null)
                {
                    soLuongSachCol.HeaderText = "Tổng Số Cuốn";
                    soLuongSachCol.Width = 90;
                    soLuongSachCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    soLuongSachCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }

                // Cột Số Lượng Còn Lại
                var soLuongConLaiCol = columns[nameof(TuaSachDTO.SoLuongSachConLai)];
                if (soLuongConLaiCol != null)
                {
                    soLuongConLaiCol.HeaderText = "Còn Lại";
                    soLuongConLaiCol.Width = 80;
                    soLuongConLaiCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    soLuongConLaiCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }


                // Cột Đã Ẩn (Ẩn)
                var daAnCol = columns[nameof(TuaSachDTO.DaAn)];
                if (daAnCol != null) daAnCol.Visible = false;

                Debug.WriteLine("DataGridView columns configured.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR configuring DataGridView columns: {ex.Message}");
                // Không nên hiển thị MessageBox ở đây vì có thể được gọi nhiều lần
            }
            Debug.WriteLine("--- Exiting SetupDataGridViewColumns ---");
        }


        // --- UI State and Interaction Methods ---

        /// <summary>
        /// Xóa nội dung các trường nhập liệu trong phần Details.
        /// </summary>
        private void ClearInputFields()
        {
            if (txtId != null) txtId.Clear();
            if (txtMaTuaSach != null) txtMaTuaSach.Clear();
            if (txtTenTuaSach != null) txtTenTuaSach.Clear();
            if (cbTheLoai != null) cbTheLoai.SelectedIndex = -1; // Bỏ chọn ComboBox Thể loại

            // Bỏ check tất cả các Tác giả trong CheckedListBox
            if (clbTacGia != null)
            {
                foreach (var item in clbTacGia.Items)
                {
                    if (item is MaterialCheckbox cb)
                    {
                        cb.Checked = false;
                    }
                }
            }
            Debug.WriteLine("Input fields (Details section) cleared.");
        }

        /// <summary>
        /// Cập nhật trạng thái Enabled/Disabled của các controls và buttons dựa trên chế độ (xem/sửa) và việc có dòng được chọn hay không.
        /// </summary>
        /// <param name="isEditingModeActive">True nếu đang ở chế độ Thêm hoặc Sửa.</param>
        /// <param name="rowSelected">True nếu có ít nhất một dòng đang được chọn trong DataGridView.</param>
        private void SetControlState(bool isEditingModeActive, bool rowSelected)
        {
            // --- Trạng thái các nút CRUD chính ---
            if (btnThem != null) btnThem.Enabled = !isEditingModeActive;
            if (btnSua != null) btnSua.Enabled = !isEditingModeActive && rowSelected;
            if (btnXoa != null) btnXoa.Enabled = !isEditingModeActive && rowSelected;
            if (btnLuu != null) btnLuu.Enabled = isEditingModeActive;
            if (btnBoQua != null) btnBoQua.Enabled = isEditingModeActive;

            // --- Trạng thái các khu vực chính ---
            // Khu vực lọc/tìm kiếm chỉ bật khi ở chế độ xem
            if (panelFilterSearch != null) panelFilterSearch.Enabled = !isEditingModeActive;
            // Khu vực lưới và nút CRUD (Thêm/Sửa/Xóa) chỉ bật khi ở chế độ xem
            if (panelGridAndButtons != null) panelGridAndButtons.Enabled = !isEditingModeActive;
            // Khu vực chi tiết (bao gồm TableLayoutPanel) chỉ bật khi ở chế độ sửa
            if (materialCardDetails != null) materialCardDetails.Enabled = isEditingModeActive;

            // --- Trạng thái các trường nhập liệu chi tiết (bên trong materialCardDetails) ---
            // Chỉ cần quan tâm khi materialCardDetails đang enabled (isEditingModeActive = true)
            if (isEditingModeActive)
            {
                if (txtId != null) txtId.Enabled = false; // ID không bao giờ sửa được
                // Mã tựa sách chỉ cho sửa khi Thêm mới (_isAdding = true)
                if (txtMaTuaSach != null) txtMaTuaSach.Enabled = _isAdding;
                // Các trường còn lại cho sửa khi ở chế độ chỉnh sửa
                if (txtTenTuaSach != null) txtTenTuaSach.Enabled = true;
                if (cbTheLoai != null) cbTheLoai.Enabled = true;
                if (clbTacGia != null) clbTacGia.Enabled = true;
            }

            // Cập nhật Text nút Xóa/Phục hồi (chỉ khi ở chế độ xem và có dòng chọn)
            UpdateDeleteButtonText(rowSelected && !isEditingModeActive);

            Debug.WriteLine($"Control state set: isEditingModeActive={isEditingModeActive}, rowSelected={rowSelected}, _isAdding={_isAdding}");
        }


        /// <summary>
        /// Cập nhật Text của nút Xóa thành "Xóa" hoặc "Phục Hồi" dựa trên trạng thái DaAn của dòng được chọn.
        /// </summary>
        /// <param name="rowSelectedAndViewMode">True nếu có dòng được chọn VÀ đang ở chế độ xem.</param>
        private void UpdateDeleteButtonText(bool rowSelectedAndViewMode)
        {
            if (btnXoa == null || dgvTuaSach == null) return;

            if (rowSelectedAndViewMode && dgvTuaSach.SelectedRows.Count > 0 && dgvTuaSach.SelectedRows[0].DataBoundItem is TuaSachDTO dto)
            {
                // FIX: Giả sử TuaSachDTO.DaAn là int?
                bool isDeleted = dto.DaAn == 1; // So sánh với 1
                btnXoa.Text = isDeleted ? "Phục Hồi" : "Xóa";
                btnXoa.UseAccentColor = isDeleted; // Dùng màu nhấn (thường là đỏ) khi là Phục hồi
                Debug.WriteLine($"Delete button text updated: {btnXoa.Text} (ID: {dto.Id}, DaAn: {dto.DaAn})");
            }
            else
            {
                // Mặc định là "Xóa" nếu không có dòng chọn hoặc đang sửa
                btnXoa.Text = "Xóa";
                btnXoa.UseAccentColor = false; // Không dùng màu nhấn khi là "Xóa"
                // Debug.WriteLine("Delete button text reset to Xóa.");
            }
        }

        /// <summary>
        /// Hiển thị dữ liệu của dòng được chọn lên các trường nhập liệu chi tiết.
        /// Chỉ hoạt động khi không ở chế độ chỉnh sửa.
        /// </summary>
        private void DisplaySelectedRow()
        {
            // Chỉ thực hiện nếu đang ở chế độ xem
            bool isEditingModeActive = _isAdding || (btnLuu != null && btnLuu.Enabled);
            if (isEditingModeActive)
            {
                Debug.WriteLine("DisplaySelectedRow skipped: Currently in editing mode.");
                return;
            }

            // Kiểm tra các control cần thiết
            if (dgvTuaSach == null || txtId == null || txtMaTuaSach == null || txtTenTuaSach == null || cbTheLoai == null || clbTacGia == null)
            {
                Debug.WriteLine("Error in DisplaySelectedRow: One or more required controls (Details section) are null. Skipping display.");
                return;
            }

            // Lấy DTO từ dòng được chọn
            if (dgvTuaSach.SelectedRows.Count == 1 && dgvTuaSach.SelectedRows[0].DataBoundItem is TuaSachDTO dto)
            {
                _lastSelectedId = dto.Id; // Lưu lại ID vừa chọn
                Debug.WriteLine($"Displaying data for TuaSach ID: {dto.Id}");

                // Hiển thị dữ liệu cơ bản
                txtId.Text = dto.Id.ToString();
                txtMaTuaSach.Text = dto.MaTuaSach ?? string.Empty;
                txtTenTuaSach.Text = dto.TenTuaSach ?? string.Empty;

                // Hiển thị Thể loại
                // Đảm bảo danh sách thể loại trong ComboBox đã được load
                if (cbTheLoai.Items.Count == 0)
                {
                    // Load nếu chưa có (trường hợp hiếm gặp nếu LoadEditorDataAsync chưa chạy)
                    // await LoadTheLoaiComboBoxData(); // Không nên await ở đây, có thể gây deadlock UI
                    Debug.WriteLine("Warning: cbTheLoai items empty during DisplaySelectedRow. Selection might fail.");
                }
                // Chọn giá trị trong ComboBox
                if (cbTheLoai.Items.OfType<TheLoaiDTO>().Any(item => item != null && item.Id == dto.IdTheLoai))
                {
                    try { cbTheLoai.SelectedValue = dto.IdTheLoai; }
                    catch (Exception exSelVal) { Debug.WriteLine($"Error setting SelectedValue for cbTheLoai to ID {dto.IdTheLoai}: {exSelVal}"); cbTheLoai.SelectedIndex = -1; }
                }
                else
                {
                    cbTheLoai.SelectedIndex = -1; // Không tìm thấy -> không chọn gì
                    Debug.WriteLine($"Warning: TheLoai ID {dto.IdTheLoai} not found in Details ComboBox for TuaSach ID {dto.Id}.");
                }

                // Hiển thị Tác giả
                // Đảm bảo danh sách tác giả trong CheckedListBox đã được load
                if (clbTacGia.Items.Count == 0)
                {
                    // await LoadTacGiaCheckedListData(); // Không nên await ở đây
                    Debug.WriteLine("Warning: clbTacGia items empty during DisplaySelectedRow. Selection might fail.");
                }
                // Lấy danh sách ID tác giả của tựa sách này
                var selectedTacGiaIds = dto.TacGias?.Select(tg => tg.Id).ToList() ?? new List<int>();
                Debug.WriteLine($"TuaSach ID {dto.Id} has {selectedTacGiaIds.Count} authors.");
                // Duyệt qua các checkbox và check/uncheck dựa trên danh sách ID
                foreach (var item in clbTacGia.Items)
                {
                    if (item is MaterialCheckbox cb && cb.Tag is int tacGiaId)
                    {
                        cb.Checked = selectedTacGiaIds.Contains(tacGiaId);
                    }
                }

                Debug.WriteLine("DisplaySelectedRow finished.");
            }
            else
            {
                // Nếu không có dòng nào được chọn hoặc chọn nhiều dòng
                _lastSelectedId = null; // Xóa ID đã lưu
                ClearInputFields();
                Debug.WriteLine("No single row selected. Input fields (Details) cleared. Last selected ID cleared.");
            }
            // Cập nhật trạng thái nút Xóa/Phục hồi sau khi hiển thị
            UpdateDeleteButtonText(dgvTuaSach.SelectedRows.Count > 0 && !isEditingModeActive);
        }


        /// <summary>
        /// Lấy danh sách ID Tác giả đang được check trong CheckedListBox chi tiết.
        /// </summary>
        /// <returns>Danh sách các ID Tác giả được chọn.</returns>
        private List<int> GetSelectedTacGiaIdsFromUI()
        {
            var selectedIds = new List<int>();
            if (clbTacGia != null)
            {
                // Duyệt qua các mục trong CheckedListBox
                foreach (var item in clbTacGia.Items)
                {
                    // Kiểm tra xem mục đó có phải là MaterialCheckbox, có được check, và có Tag chứa ID hợp lệ không
                    if (item is MaterialCheckbox cb && cb.Checked && cb.Tag is int tacGiaId)
                    {
                        selectedIds.Add(tacGiaId); // Thêm ID vào danh sách
                    }
                }
            }
            Debug.WriteLine($"Retrieved {selectedIds.Count} selected author IDs from Details CheckedListBox.");
            return selectedIds;
        }

        /// <summary>
        /// Hiển thị thông báo ngắn gọn bằng MessageBox.
        /// </summary>
        /// <param name="message">Nội dung thông báo.</param>
        /// <param name="title">Tiêu đề thông báo (mặc định là "Thông báo").</param>
        /// <param name="icon">Icon hiển thị (mặc định là Information).</param>
        private void ShowNotification(string message, string title = "Thông báo", MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            // FIX: Luôn sử dụng MaterialMessageBox
            MaterialMessageBox.Show(this.FindForm(), message, title, MessageBoxButtons.OK, icon);
            Debug.WriteLine($"Notification shown: [{icon}] {title} - {message}");
        }


        // --- Button Click Event Handlers ---

        /// <summary>
        /// Xử lý sự kiện click nút Thêm. Chuyển sang chế độ thêm mới.
        /// </summary>
        private async void btnThem_Click(object sender, EventArgs e)
        {
            // Kiểm tra control và trạng thái hiện tại
            if (!CheckRequiredControls()) return;
            bool isCurrentlyEditing = _isAdding || (btnLuu != null && btnLuu.Enabled);
            if (isCurrentlyEditing) { Debug.WriteLine("btnThem_Click skipped: Already in editing mode."); return; }

            Debug.WriteLine(">>> btnThem_Click: Starting Add mode.");
            _isAdding = true; // Đặt trạng thái là thêm mới
            _lastSelectedId = null; // Bỏ lưu ID cũ
            ClearInputFields(); // Xóa các trường nhập liệu
            dgvTuaSach?.ClearSelection(); // Bỏ chọn dòng hiện tại

            // Load dữ liệu cần thiết cho ComboBox/CheckedListBox trong phần Details
            await LoadEditorDataAsync();

            // Cập nhật trạng thái controls cho chế độ Thêm
            SetControlState(isEditingModeActive: true, rowSelected: false);
            txtMaTuaSach?.Focus(); // Đặt focus vào trường Mã tựa sách
            Debug.WriteLine("<<< btnThem_Click: Add mode activated.");
        }

        /// <summary>
        /// Xử lý sự kiện click nút Sửa. Chuyển sang chế độ sửa cho dòng đang chọn.
        /// </summary>
        private async void btnSua_Click(object sender, EventArgs e)
        {
            // Kiểm tra control và trạng thái hiện tại
            if (!CheckRequiredControls()) return;
            bool isCurrentlyEditing = _isAdding || (btnLuu != null && btnLuu.Enabled);
            if (isCurrentlyEditing) { Debug.WriteLine("btnSua_Click skipped: Already in editing mode."); return; }

            // Chỉ cho phép sửa khi có đúng 1 dòng được chọn
            if (dgvTuaSach != null && dgvTuaSach.SelectedRows.Count == 1)
            {
                Debug.WriteLine(">>> btnSua_Click: Starting Edit mode.");
                _isAdding = false; // Đảm bảo không phải chế độ thêm mới

                // Load lại dữ liệu cho ComboBox/CheckedListBox (để đảm bảo là mới nhất)
                await LoadEditorDataAsync();

                // Dữ liệu của dòng chọn đã được hiển thị bởi DisplaySelectedRow() khi SelectionChanged.
                // Chỉ cần chuyển trạng thái control sang chế độ sửa.
                SetControlState(isEditingModeActive: true, rowSelected: true);
                txtTenTuaSach?.Focus(); // Đặt focus vào trường Tên tựa sách
                Debug.WriteLine("<<< btnSua_Click: Edit mode activated.");
            }
            else
            {
                // Thông báo nếu không chọn dòng nào hoặc chọn nhiều dòng
                string message = dgvTuaSach?.SelectedRows.Count == 0
                    ? "Vui lòng chọn tựa sách cần sửa."
                    : "Vui lòng chỉ chọn một tựa sách để sửa.";
                MaterialMessageBox.Show(this.FindForm(), message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Debug.WriteLine($"btnSua_Click: Invalid selection count ({dgvTuaSach?.SelectedRows.Count}).");
            }
        }

        /// <summary>
        /// Xử lý sự kiện click nút Lưu. Lưu dữ liệu đang thêm mới hoặc sửa.
        /// </summary>
        private async void btnLuu_Click(object sender, EventArgs e)
        {
            // Kiểm tra control và trạng thái
            if (!CheckRequiredControls() || _busTuaSach == null)
            {
                MaterialMessageBox.Show(this.FindForm(), "Không thể lưu do lỗi hệ thống hoặc giao diện.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"btnLuu_Click skipped: Controls OK={CheckRequiredControls()}, BUS OK={_busTuaSach != null}");
                return;
            }
            bool isCurrentlyEditing = _isAdding || (btnLuu != null && btnLuu.Enabled);
            if (!isCurrentlyEditing) { Debug.WriteLine("btnLuu_Click skipped: Not in editing mode."); return; }

            Debug.WriteLine(">>> btnLuu_Click: Attempting to save...");

            // --- Thu thập và Validate dữ liệu ---
            string? maTuaSach = txtMaTuaSach?.Text.Trim();
            string tenTuaSach = txtTenTuaSach?.Text.Trim() ?? "";
            int idTheLoai = (int)(cbTheLoai?.SelectedValue ?? 0);
            List<int> selectedTacGiaIds = GetSelectedTacGiaIdsFromUI();

            // Validation cơ bản
            if (string.IsNullOrWhiteSpace(tenTuaSach))
            {
                MaterialMessageBox.Show(this.FindForm(), "Tên tựa sách không được để trống.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenTuaSach?.Focus();
                Debug.WriteLine("Validation failed: TenTuaSach empty.");
                return;
            }
            if (idTheLoai <= 0)
            {
                MaterialMessageBox.Show(this.FindForm(), "Vui lòng chọn một thể loại hợp lệ.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbTheLoai?.Focus();
                Debug.WriteLine($"Validation failed: Invalid TheLoai ID {idTheLoai}.");
                return;
            }
            // Thêm validation cho Mã tựa sách nếu cần (ví dụ: không trùng khi thêm mới)
            if (_isAdding && string.IsNullOrWhiteSpace(maTuaSach))
            {
                MaterialMessageBox.Show(this.FindForm(), "Mã tựa sách không được để trống khi thêm mới.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaTuaSach?.Focus();
                Debug.WriteLine("Validation failed: MaTuaSach empty during add.");
                return;
            }

            // Lấy ID hiện tại nếu đang sửa
            int currentId = 0;
            if (!_isAdding && txtId != null && int.TryParse(txtId.Text, out int id))
            {
                currentId = id;
            }
            Debug.WriteLine($"btnLuu_Click: Data collected - currentId={currentId}, isAdding={_isAdding}, MaTuaSach='{maTuaSach}', TenTuaSach='{tenTuaSach}', IdTheLoai={idTheLoai}, SelectedTacGiaIds={selectedTacGiaIds.Count}");

            // Tạo DTO để gửi đi
            TuaSachDTO tuaSachDto = new TuaSachDTO
            {
                Id = currentId,
                MaTuaSach = string.IsNullOrWhiteSpace(maTuaSach) ? null : maTuaSach, // Cho phép null nếu không phải thêm mới
                TenTuaSach = tenTuaSach,
                IdTheLoai = idTheLoai,
                TacGias = selectedTacGiaIds.Select(tgId => new TacGiaDTO { Id = tgId }).ToList()
                // DaAn không cần set ở đây, BUS/DAL sẽ xử lý
            };

            // --- Thực hiện Lưu ---
            bool success = false;
            string? errorMsg = null;
            TuaSachDTO? resultDto = null;

            // Vô hiệu hóa nút tạm thời và hiển thị con trỏ chờ
            if (btnLuu != null) btnLuu.Enabled = false;
            if (btnBoQua != null) btnBoQua.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (_isAdding) // Thêm mới
                {
                    Debug.WriteLine("btnLuu_Click: Calling AddTuaSachAsync...");
                    resultDto = await _busTuaSach.AddTuaSachAsync(tuaSachDto);
                    success = resultDto != null;
                    if (success && resultDto != null)
                    {
                        _lastSelectedId = resultDto.Id; // Lưu ID mới được tạo
                        Debug.WriteLine($"Add successful. New ID: {_lastSelectedId}");
                    }
                }
                else // Cập nhật
                {
                    if (currentId <= 0)
                    {
                        errorMsg = "Không thể cập nhật. ID Tựa sách không hợp lệ.";
                        Debug.WriteLine($"btnLuu_Click: Update cancelled. Invalid currentId ({currentId}).");
                    }
                    else
                    {
                        Debug.WriteLine($"btnLuu_Click: Calling UpdateTuaSachAsync for ID {currentId}...");
                        success = await _busTuaSach.UpdateTuaSachAsync(tuaSachDto);
                        if (success) _lastSelectedId = currentId; // Lưu lại ID vừa cập nhật
                        Debug.WriteLine($"Update for ID {currentId} result: {success}");
                    }
                }
            }
            catch (ArgumentException argEx) // Lỗi validation từ BUS
            {
                errorMsg = $"Dữ liệu không hợp lệ: {argEx.Message}";
                Debug.WriteLine($"Validation Error caught in GUI (btnLuu_Click): {argEx.ToString()}");
            }
            catch (InvalidOperationException invOpEx) // Lỗi nghiệp vụ từ BUS (vd: mã trùng)
            {
                errorMsg = $"Thao tác không hợp lệ: {invOpEx.Message}";
                Debug.WriteLine($"Business Rule Violation caught in GUI (btnLuu_Click): {invOpEx.ToString()}");
            }
            catch (Exception ex) // Lỗi hệ thống khác
            {
                errorMsg = $"Lỗi hệ thống khi lưu: {ex.Message}";
                Debug.WriteLine($"*** ERROR (btnLuu_Click): {ex.ToString()}");
            }
            finally
            {
                this.Cursor = Cursors.Default;
                // Kích hoạt lại nút Lưu/Bỏ qua chỉ khi thao tác thất bại VÀ control chưa bị dispose
                if (!success && this.IsHandleCreated) // Kiểm tra IsHandleCreated để tránh lỗi cross-thread
                {
                    this.BeginInvoke((MethodInvoker)delegate {
                        if (btnLuu != null && !btnLuu.IsDisposed) btnLuu.Enabled = true;
                        if (btnBoQua != null && !btnBoQua.IsDisposed) btnBoQua.Enabled = true;
                    });
                }
                Debug.WriteLine($"btnLuu_Click finished BUS call. Success={success}.");
            }

            // --- Xử lý kết quả ---
            if (success)
            {
                // FIX: Sử dụng ShowNotification thay vì ShowSnackbar
                ShowNotification(_isAdding ? "Thêm tựa sách thành công!" : "Cập nhật tựa sách thành công!");
                _isAdding = false; // Thoát chế độ thêm/sửa
                await LoadFilteredDataGrid(); // Load lại DataGrid
                                              // *** FIX: Gọi SetControlState sau khi LoadFilteredDataGrid hoàn tất ***
                if (this.IsHandleCreated) { this.BeginInvoke((MethodInvoker)delegate { SetControlState(false, dgvTuaSach?.SelectedRows.Count > 0); }); }
            }
            else
            {
                // Hiển thị lỗi
                MaterialMessageBox.Show(this.FindForm(), errorMsg ?? (_isAdding ? "Thêm tựa sách thất bại." : "Cập nhật tựa sách thất bại."), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Giữ nguyên trạng thái sửa để người dùng sửa lỗi
                if (this.IsHandleCreated)
                {
                    this.BeginInvoke((MethodInvoker)delegate {
                        SetControlState(isEditingModeActive: true, rowSelected: currentId > 0 || (_lastSelectedId.HasValue && _lastSelectedId > 0));
                    });
                }
            }
            Debug.WriteLine("<<< btnLuu_Click: Save process finished.");
        }


        /// <summary>
        /// Xử lý sự kiện click nút Bỏ Qua. Hủy chế độ thêm/sửa và quay lại trạng thái xem.
        /// </summary>
        private async void btnBoQua_Click(object sender, EventArgs e)
        {
            // Kiểm tra control và trạng thái
            if (!CheckRequiredControls()) return;
            bool isCurrentlyEditing = _isAdding || (btnLuu != null && btnLuu.Enabled);
            if (!isCurrentlyEditing) { Debug.WriteLine("btnBoQua_Click skipped: Not in editing mode."); return; }

            Debug.WriteLine(">>> btnBoQua_Click: Cancelling Add/Edit mode.");
            _isAdding = false; // Thoát chế độ thêm/sửa

            // Tải lại grid để hiển thị trạng thái trước đó
            await LoadFilteredDataGrid();

            // *** FIX: Đặt lại trạng thái UI một cách rõ ràng SAU KHI tải lại grid ***
            bool rowSelectedAfterLoad = dgvTuaSach?.SelectedRows.Count > 0;
            // Gọi BeginInvoke để đảm bảo cập nhật UI trên đúng luồng
            if (this.IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate {
                    SetControlState(isEditingModeActive: false, rowSelected: rowSelectedAfterLoad);
                    // Hiển thị lại dữ liệu dòng được chọn (hoặc xóa nếu không có dòng nào)
                    DisplaySelectedRow();
                    Debug.WriteLine("<<< btnBoQua_Click: Add/Edit mode cancelled, state explicitly reset.");
                });
            }
            else
            {
                Debug.WriteLine("<<< btnBoQua_Click: Add/Edit mode cancelled, but handle not created for final state reset.");
            }
        }


        /// <summary>
        /// Xử lý sự kiện click nút Xóa/Phục hồi. Thực hiện xóa mềm hoặc phục hồi dòng đang chọn.
        /// </summary>
        private async void btnXoa_Click(object sender, EventArgs e)
        {
            // Kiểm tra control và trạng thái
            if (!CheckRequiredControls() || _busTuaSach == null)
            {
                MaterialMessageBox.Show(this.FindForm(), "Không thể thực hiện thao tác do lỗi hệ thống hoặc giao diện.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"btnXoa_Click skipped: Controls OK={CheckRequiredControls()}, BUS OK={_busTuaSach != null}");
                return;
            }
            bool isCurrentlyEditing = _isAdding || (btnLuu != null && btnLuu.Enabled);
            if (isCurrentlyEditing)
            {
                Debug.WriteLine("btnXoa_Click skipped: Editing mode active.");
                MaterialMessageBox.Show(this.FindForm(), "Đang ở chế độ chỉnh sửa, không thể xóa hoặc phục hồi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Chỉ thực hiện khi có đúng 1 dòng được chọn
            if (dgvTuaSach != null && dgvTuaSach.SelectedRows.Count == 1 && dgvTuaSach.SelectedRows[0].DataBoundItem is TuaSachDTO dto)
            {
                int id = dto.Id;
                string ten = dto.TenTuaSach ?? $"ID {dto.Id}";
                // FIX: Giả sử TuaSachDTO.DaAn là int?
                bool isDeleted = dto.DaAn == 1; // So sánh với 1

                string actionVerb = isDeleted ? "phục hồi" : "xóa mềm";
                string confirmTitle = isDeleted ? "Xác nhận Phục hồi" : "Xác nhận Xóa mềm";
                MessageBoxIcon icon = isDeleted ? MessageBoxIcon.Question : MessageBoxIcon.Warning;

                // Xác nhận với người dùng
                var confirmResult = MaterialMessageBox.Show(this.FindForm(), $"Bạn có chắc chắn muốn {actionVerb} tựa sách '{ten}'?", confirmTitle, MessageBoxButtons.YesNo, icon);

                if (confirmResult == DialogResult.Yes)
                {
                    Debug.WriteLine($"btnXoa_Click: User confirmed {actionVerb} for ID {id}.");
                    if (isDeleted)
                    {
                        await PerformRestore(id); // Thực hiện phục hồi
                    }
                    else
                    {
                        await PerformSoftDelete(id); // Thực hiện xóa mềm
                    }
                }
                else
                {
                    Debug.WriteLine($"btnXoa_Click: User cancelled {actionVerb} for ID {id}.");
                }
            }
            else
            {
                // Thông báo nếu chọn không hợp lệ
                string message = dgvTuaSach?.SelectedRows.Count == 0
                    ? "Vui lòng chọn tựa sách cần xóa hoặc phục hồi."
                    : "Vui lòng chỉ chọn một tựa sách để xóa hoặc phục hồi.";
                MaterialMessageBox.Show(this.FindForm(), message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Debug.WriteLine($"btnXoa_Click: Invalid selection count ({dgvTuaSach?.SelectedRows.Count}).");
            }
        }

        /// <summary>
        /// Xử lý sự kiện click nút Thoát. Gửi yêu cầu đóng UserControl.
        /// </summary>
        private void btnThoat_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnThoat_Click: Raising RequestClose event.");
            RequestClose?.Invoke(this, EventArgs.Empty); // Kích hoạt sự kiện yêu cầu đóng
        }


        // --- Helper Methods for Delete/Restore ---

        /// <summary>
        /// Thực hiện xóa mềm tựa sách với ID cung cấp.
        /// </summary>
        /// <param name="id">ID của tựa sách cần xóa mềm.</param>
        private async Task PerformSoftDelete(int id)
        {
            Debug.WriteLine($"--- Starting PerformSoftDelete for ID {id} ---");
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (_busTuaSach == null) throw new InvalidOperationException("BUS service is not available.");

                bool success = await _busTuaSach.DeleteTuaSachAsync(id);

                if (success)
                {
                    // FIX: Sử dụng ShowNotification thay vì ShowSnackbar
                    ShowNotification("Xóa mềm tựa sách thành công!");
                    _lastSelectedId = null; // Xóa ID đã lưu vì dòng đã bị "ẩn"
                    await LoadFilteredDataGrid(); // Load lại DataGrid
                    // *** FIX: Gọi SetControlState sau khi LoadFilteredDataGrid hoàn tất ***
                    if (this.IsHandleCreated) { this.BeginInvoke((MethodInvoker)delegate { SetControlState(false, false); }); } // rowSelected = false vì vừa xóa
                    Debug.WriteLine($"PerformSoftDelete: ID {id} successful.");
                }
                else
                {
                    MaterialMessageBox.Show(this.FindForm(), "Xóa mềm tựa sách thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.WriteLine($"PerformSoftDelete: ID {id} failed (BUS returned false).");
                    // *** FIX: Đảm bảo UI state đúng nếu lỗi ***
                    if (this.IsHandleCreated) { this.BeginInvoke((MethodInvoker)delegate { UpdateUIStateBasedOnSelection(); }); }
                }
            }
            catch (InvalidOperationException invOpEx) // Lỗi nghiệp vụ (ví dụ: còn sách đang mượn)
            {
                MaterialMessageBox.Show(this.FindForm(), $"Không thể xóa tựa sách: {invOpEx.Message}", "Lỗi Nghiệp Vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Debug.WriteLine($"Business Rule Violation caught in GUI (PerformSoftDelete ID {id}): {invOpEx.ToString()}");
                // *** FIX: Đảm bảo UI state đúng nếu lỗi ***
                if (this.IsHandleCreated) { this.BeginInvoke((MethodInvoker)delegate { UpdateUIStateBasedOnSelection(); }); }
            }
            catch (Exception ex) // Lỗi hệ thống khác
            {
                MaterialMessageBox.Show(this.FindForm(), $"Lỗi hệ thống khi xóa mềm tựa sách: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"*** ERROR (PerformSoftDelete ID {id}): {ex.ToString()}");
                // *** FIX: Đảm bảo UI state đúng nếu lỗi ***
                if (this.IsHandleCreated) { this.BeginInvoke((MethodInvoker)delegate { UpdateUIStateBasedOnSelection(); }); }
            }
            finally
            {
                this.Cursor = Cursors.Default;
                Debug.WriteLine($"--- Exiting PerformSoftDelete for ID {id} ---");
            }
        }

        /// <summary>
        /// Thực hiện phục hồi tựa sách đã bị xóa mềm.
        /// </summary>
        /// <param name="id">ID của tựa sách cần phục hồi.</param>
        private async Task PerformRestore(int id)
        {
            Debug.WriteLine($"--- Starting PerformRestore for ID {id} ---");
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (_busTuaSach == null) throw new InvalidOperationException("BUS service is not available.");

                bool success = await _busTuaSach.RestoreTuaSachAsync(id);

                if (success)
                {
                    // FIX: Sử dụng ShowNotification thay vì ShowSnackbar
                    ShowNotification("Phục hồi tựa sách thành công!");
                    _lastSelectedId = id; // Lưu lại ID vừa phục hồi để chọn lại
                    await LoadFilteredDataGrid(); // Load lại DataGrid (sẽ tự động chọn lại dòng)
                    // *** FIX: Gọi SetControlState sau khi LoadFilteredDataGrid hoàn tất ***
                    if (this.IsHandleCreated) { this.BeginInvoke((MethodInvoker)delegate { SetControlState(false, true); }); } // rowSelected = true vì vừa phục hồi
                    Debug.WriteLine($"PerformRestore: ID {id} successful.");
                }
                else
                {
                    MaterialMessageBox.Show(this.FindForm(), "Phục hồi tựa sách thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.WriteLine($"PerformRestore: ID {id} failed (BUS returned false).");
                    // *** FIX: Đảm bảo UI state đúng nếu lỗi ***
                    if (this.IsHandleCreated) { this.BeginInvoke((MethodInvoker)delegate { UpdateUIStateBasedOnSelection(); }); }
                }
            }
            catch (Exception ex) // Lỗi hệ thống
            {
                MaterialMessageBox.Show(this.FindForm(), $"Lỗi hệ thống khi phục hồi tựa sách: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"*** ERROR (PerformRestore ID {id}): {ex.ToString()}");
                // *** FIX: Đảm bảo UI state đúng nếu lỗi ***
                if (this.IsHandleCreated) { this.BeginInvoke((MethodInvoker)delegate { UpdateUIStateBasedOnSelection(); }); }
            }
            finally
            {
                this.Cursor = Cursors.Default;
                Debug.WriteLine($"--- Exiting PerformRestore for ID {id} ---");
            }
        }

        // *** FIX: Helper cập nhật UI state dựa trên lựa chọn hiện tại ***
        private void UpdateUIStateBasedOnSelection()
        {
            if (this.IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate {
                    bool rowSelected = dgvTuaSach?.SelectedRows.Count > 0;
                    SetControlState(false, rowSelected); // Luôn ở chế độ xem sau khi xóa/khôi phục/lỗi
                });
            }
        }


        // --- DataGridView Event Handlers ---

        /// <summary>
        /// Xử lý sự kiện khi lựa chọn dòng trong DataGridView thay đổi.
        /// </summary>
        private void dgvTuaSach_SelectionChanged(object sender, EventArgs e)
        {
            // Chỉ hiển thị dữ liệu lên form chi tiết nếu đang ở chế độ xem
            bool isEditingModeActive = _isAdding || (btnLuu != null && btnLuu.Enabled);
            if (!isEditingModeActive)
            {
                DisplaySelectedRow(); // Hiển thị dữ liệu của dòng mới chọn
                                      // *** FIX: Gọi SetControlState ở đây để đảm bảo nút Sửa/Xóa cập nhật đúng ***
                SetControlState(isEditingModeActive: false, rowSelected: dgvTuaSach?.SelectedRows.Count > 0);
            }
            else
            {
                // Nếu đang sửa, không thay đổi dữ liệu trên form chi tiết khi chọn dòng khác
                Debug.WriteLine("dgvTuaSach_SelectionChanged: In edit mode, skipping DisplaySelectedRow().");
            }
            Debug.WriteLine($"dgvTuaSach_SelectionChanged handled. Selected rows: {dgvTuaSach?.SelectedRows.Count}. Editing: {isEditingModeActive}");
        }

        /// <summary>
        /// Xử lý sự kiện double-click vào một dòng trong DataGridView. Mô phỏng hành động nhấn nút Sửa.
        /// </summary>
        private void dgvTuaSach_DoubleClick(object sender, EventArgs e)
        {
            Debug.WriteLine("dgvTuaSach_DoubleClick detected.");
            // Gọi sự kiện click của nút Sửa nếu đang ở chế độ xem và có 1 dòng được chọn
            bool isEditingModeActive = _isAdding || (btnLuu != null && btnLuu.Enabled);
            if (!isEditingModeActive && dgvTuaSach != null && dgvTuaSach.SelectedRows.Count == 1)
            {
                Debug.WriteLine("Simulating btnSua_Click from DoubleClick.");
                btnSua_Click(sender, e); // Gọi trực tiếp handler của nút Sửa
            }
            else
            {
                Debug.WriteLine($"DoubleClick ignored. Editing: {isEditingModeActive}, Selected Rows: {dgvTuaSach?.SelectedRows.Count}");
            }
        }

        // --- Filter/Search Event Handlers ---

        /// <summary>
        /// Xử lý sự kiện khi nội dung ô tìm kiếm thay đổi. Kích hoạt timer trì hoãn.
        /// </summary>
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (_searchDebounceTimer == null) InitializeSearchDebounceTimer(); // Khởi tạo nếu chưa có

            // Dừng timer cũ (nếu đang chạy) và bắt đầu lại
            _searchDebounceTimer?.Stop();
            _searchDebounceTimer?.Start();
            Debug.WriteLine("Search text changed. Debounce timer reset.");
        }

        /// <summary>
        /// Xử lý sự kiện khi lựa chọn ComboBox lọc Thể loại thay đổi.
        /// </summary>
        private async void cbFilterTheLoai_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Chỉ lọc khi ComboBox đang được focus (người dùng chủ động chọn)
            // và không ở chế độ chỉnh sửa
            bool isEditingModeActive = _isAdding || (btnLuu != null && btnLuu.Enabled);
            if (!isEditingModeActive && cbFilterTheLoai != null && (cbFilterTheLoai.Focused || cbFilterTheLoai.ContainsFocus))
            {
                Debug.WriteLine($"Filter TheLoai changed to ID: {cbFilterTheLoai.SelectedValue}. Loading filtered data...");
                await LoadFilteredDataGrid();
                // *** FIX: Gọi SetControlState sau khi LoadFilteredDataGrid hoàn tất ***
                if (this.IsHandleCreated) { this.BeginInvoke((MethodInvoker)delegate { SetControlState(false, dgvTuaSach?.SelectedRows.Count > 0); }); }
            }
            else { Debug.WriteLine("cbFilterTheLoai_SelectedIndexChanged skipped (Not focused, editing, or null)."); }
        }

        /// <summary>
        /// Xử lý sự kiện khi lựa chọn ComboBox lọc Tác giả thay đổi.
        /// </summary>
        private async void cbFilterTacGia_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Chỉ lọc khi ComboBox đang được focus và không ở chế độ chỉnh sửa
            bool isEditingModeActive = _isAdding || (btnLuu != null && btnLuu.Enabled);
            if (!isEditingModeActive && cbFilterTacGia != null && (cbFilterTacGia.Focused || cbFilterTacGia.ContainsFocus))
            {
                Debug.WriteLine($"Filter TacGia changed to ID: {cbFilterTacGia.SelectedValue}. Loading filtered data...");
                await LoadFilteredDataGrid();
                // *** FIX: Gọi SetControlState sau khi LoadFilteredDataGrid hoàn tất ***
                if (this.IsHandleCreated) { this.BeginInvoke((MethodInvoker)delegate { SetControlState(false, dgvTuaSach?.SelectedRows.Count > 0); }); }
            }
            else { Debug.WriteLine("cbFilterTacGia_SelectedIndexChanged skipped (Not focused, editing, or null)."); }
        }

        /// <summary>
        /// Xử lý sự kiện click nút Bỏ lọc. Đặt lại các bộ lọc về giá trị mặc định và tải lại grid.
        /// </summary>
        private async void btnResetFilter_Click(object sender, EventArgs e)
        {
            bool isEditingModeActive = _isAdding || (btnLuu != null && btnLuu.Enabled);
            if (isEditingModeActive) { Debug.WriteLine("btnResetFilter_Click skipped: Editing mode active."); return; }

            Debug.WriteLine(">>> btnResetFilter_Click: Resetting filters...");
            bool filterChanged = false;

            // Đặt lại ô tìm kiếm
            if (txtSearch != null && !string.IsNullOrEmpty(txtSearch.Text))
            {
                txtSearch.Clear();
                filterChanged = true;
                Debug.WriteLine("Search text cleared.");
            }
            // Đặt lại ComboBox lọc Thể loại
            if (cbFilterTheLoai != null && (int)(cbFilterTheLoai.SelectedValue ?? 0) != 0)
            {
                cbFilterTheLoai.SelectedValue = 0;
                filterChanged = true;
                Debug.WriteLine("Filter TheLoai reset to 'All'.");
            }
            // Đặt lại ComboBox lọc Tác giả
            if (cbFilterTacGia != null && (int)(cbFilterTacGia.SelectedValue ?? 0) != 0)
            {
                cbFilterTacGia.SelectedValue = 0;
                filterChanged = true;
                Debug.WriteLine("Filter TacGia reset to 'All'.");
            }

            // Chỉ load lại grid nếu có bộ lọc nào đó thực sự thay đổi
            if (filterChanged)
            {
                Debug.WriteLine("Filters changed, reloading grid...");
                _searchDebounceTimer?.Stop(); // Dừng timer tìm kiếm nếu đang chạy
                await LoadFilteredDataGrid();
                // *** FIX: Gọi SetControlState sau khi LoadFilteredDataGrid hoàn tất ***
                if (this.IsHandleCreated) { this.BeginInvoke((MethodInvoker)delegate { SetControlState(false, dgvTuaSach?.SelectedRows.Count > 0); }); }
            }
            else
            {
                Debug.WriteLine("No filters changed, grid not reloaded.");
            }
            Debug.WriteLine("<<< btnResetFilter_Click finished.");
        }


        // --- Row Selection Helper ---

        /// <summary>
        /// Chọn một dòng trong DataGridView dựa trên ID.
        /// </summary>
        /// <param name="id">ID của dòng cần chọn.</param>
        private void SelectRowById(int id)
        {
            if (dgvTuaSach == null || dgvTuaSach.Rows.Count == 0)
            {
                Debug.WriteLine($"SelectRowById({id}) skipped: dgvTuaSach is null or empty.");
                return;
            }
            Debug.WriteLine($"--- Attempting to select row with ID: {id} ---");

            bool found = false;
            dgvTuaSach.ClearSelection(); // Bỏ chọn tất cả trước

            foreach (DataGridViewRow row in dgvTuaSach.Rows)
            {
                // Kiểm tra xem DataBoundItem có phải là TuaSachDTO và có ID khớp không
                if (row.DataBoundItem is TuaSachDTO dto && dto.Id == id)
                {
                    row.Selected = true; // Chọn dòng
                    // Cuộn đến dòng được chọn nếu nó không hiển thị
                    if (!row.Displayed && row.Index >= 0 && row.Index < dgvTuaSach.RowCount)
                    {
                        try
                        {
                            dgvTuaSach.FirstDisplayedScrollingRowIndex = row.Index;
                            Debug.WriteLine($"Scrolled to row index {row.Index} for ID {id}.");
                        }
                        catch (Exception scrollEx) { Debug.WriteLine($"Error scrolling to row index {row.Index}: {scrollEx.Message}"); }
                    }
                    found = true;
                    Debug.WriteLine($"Row with ID {id} found and selected at index {row.Index}.");
                    break; // Thoát vòng lặp khi tìm thấy
                }
            }

            if (!found)
            {
                _lastSelectedId = null; // Xóa ID đã lưu nếu không tìm thấy dòng
                ClearInputFields(); // Xóa input
                Debug.WriteLine($"Row with ID {id} not found. Input fields cleared. Last selected ID cleared.");
            }
            // Không cần gọi SetControlState ở đây nữa, nó sẽ được gọi bởi SelectionChanged hoặc sau khi LoadFilteredDataGrid
            Debug.WriteLine($"--- Exiting SelectRowById({id}). Found: {found} ---");
        }


        // --- Exit Event ---
        /// <summary>
        /// Sự kiện được kích hoạt khi người dùng yêu cầu đóng UserControl (ví dụ: nhấn nút Thoát).
        /// </summary>
        public event EventHandler? RequestClose;

    } // End Class ucQuanLyTuaSach
} // End Namespace GUI
