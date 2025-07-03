// Project/Namespace: GUI

// --- USING DIRECTIVES ---
using BUS; // Cần cho IBUSThongKe (hoặc tên interface thống kê của bạn)
using DTO; // Cần cho các DTO liên quan đến thống kê (ví dụ: CtBaoCaoLuotMuonTheoTheLoaiDTO)
using MaterialSkin.Controls; // Cần cho MaterialMessageBox
using System; // Cần cho Exception, EventArgs, DateTime, int
using System.Collections.Generic; // Cần cho List
using System.Linq; // Cần cho LINQ (ToList, Range)
using System.Threading.Tasks; // Cần cho async/await Task
using System.Windows.Forms; // Cần cho UserControl, DataGridView, Panel, MessageBoxButtons, MessageBoxIcon, Cursor, ComboBox
using Microsoft.Extensions.DependencyInjection; // Cần cho IServiceProvider (nếu UserControl này mở Form/UC con)
using System.Diagnostics; // Cần cho Debug

// Namespace của chính project GUI để refer các Forms/UserControl khác nếu cần (ví dụ: frmMain)
using GUI; // <<< Ensures interfaces are accessible


namespace GUI // Namespace của project GUI của bạn
{
    /// <summary>
    /// UserControl hiển thị các thống kê và báo cáo.
    /// Cho phép chọn tiêu chí (ví dụ: tháng/năm) và xem kết quả báo cáo trên lưới.
    /// Sử dụng Dependency Injection và MaterialSkin.2.
    /// </summary>
    // *** FIX: Correctly reference the interfaces without the frmMain prefix ***
    public partial class ucThongKe : UserControl, IRequiresDataLoading, IRequestClose // <<< SỬA Ở ĐÂY
    {
        // --- DEPENDENCIES (Nhận qua Constructor Injection) ---
        private readonly IBUSThongKe _busThongKe;
        private readonly IServiceProvider _serviceProvider;

        // *** FIX: THÊM ĐỊNH NGHĨA SỰ KIỆN RequestClose ***
        /// <summary>
        /// Sự kiện được kích hoạt khi UserControl muốn yêu cầu đóng chính nó.
        /// Form cha (frmMain) sẽ lắng nghe sự kiện này.
        /// </summary>
        public event EventHandler? RequestClose;
        // *** KẾT THÚC THÊM SỰ KIỆN ***

        // --- CONSTRUCTOR (Sử dụng Dependency Injection) ---
        public ucThongKe(IBUSThongKe busThongKe, IServiceProvider serviceProvider)
        {
            InitializeComponent(); // Phương thức được sinh tự động

            _busThongKe = busThongKe ?? throw new ArgumentNullException(nameof(busThongKe));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        // --- SỰ KIỆN LOAD USERCONTROL ---
        private void ucThongKe_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                Debug.WriteLine("ucThongKe_Load fired (DesignMode=false). InitializeDataAsync should be called by parent.");
                // Không gọi InitializeDataAsync() ở đây
            }
            else
            {
                Debug.WriteLine("ucThongKe_Load fired (DesignMode=true).");
            }
        }

        // *** SỬA ĐỔI: THÊM DEBUG VÀ TRY-CATCH ***
        /// <summary>
        /// Phương thức công khai để khởi tạo dữ liệu cho UserControl khi được load bởi frmMain.
        /// </summary>
        public async Task InitializeDataAsync()
        {
            Debug.WriteLine("+++ ucThongKe: InitializeDataAsync started."); // <<< THÊM DÒNG NÀY
            if (!this.DesignMode)
            {
                try // <<< THÊM TRY-CATCH
                {
                    Debug.WriteLine("--- ucThongKe: Calling PopulateMonthComboBox..."); // <<< THÊM DÒNG NÀY
                    PopulateMonthComboBox();
                    Debug.WriteLine("--- ucThongKe: Calling PopulateYearComboBox..."); // <<< THÊM DÒNG NÀY
                    PopulateYearComboBox();

                    Debug.WriteLine("--- ucThongKe: Setting default ComboBox values..."); // <<< THÊM DÒNG NÀY
                    // Đảm bảo kiểm tra null trước khi truy cập SelectedValue
                    if (cboMonth != null && cboMonth.Items.Count > 0) // Kiểm tra Items.Count > 0
                    {
                        // Cần kiểm tra xem giá trị có tồn tại trong danh sách không trước khi gán
                        int currentMonth = DateTime.Now.Month;
                        // Kiểm tra xem DataSource đã được gán và ValueMember có tồn tại không
                        if (cboMonth.DataSource != null && !string.IsNullOrEmpty(cboMonth.ValueMember))
                        {
                            // Cần ép kiểu item về đúng kiểu dữ liệu của DataSource
                            // Safer check: use try-cast or check property existence before accessing
                            bool monthFound = false;
                            try
                            {
                                if (cboMonth.Items.OfType<object>().Any(item => {
                                    var prop = item.GetType().GetProperty(cboMonth.ValueMember);
                                    return prop != null && prop.PropertyType == typeof(int) && (int)prop.GetValue(item, null) == currentMonth;
                                }))
                                {
                                    cboMonth.SelectedValue = currentMonth;
                                    Debug.WriteLine($"--- ucThongKe: cboMonth default set to {cboMonth.SelectedValue}"); // <<< THÊM DÒNG NÀY
                                    monthFound = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"--- ucThongKe: Error checking cboMonth items: {ex.Message}");
                            }

                            if (!monthFound)
                            {
                                Debug.WriteLine($"--- ucThongKe: currentMonth {currentMonth} not found or error occurred in cboMonth items (using ValueMember)."); // <<< THÊM DÒNG NÀY
                                if (cboMonth.Items.Count > 0) cboMonth.SelectedIndex = 0; // Chọn item đầu tiên nếu tháng hiện tại không có
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"--- ucThongKe: cboMonth DataSource or ValueMember is not set properly."); // <<< THÊM DÒNG NÀY
                            if (cboMonth.Items.Count > 0) cboMonth.SelectedIndex = 0; // Chọn item đầu tiên như một giải pháp tạm
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"--- ucThongKe: cboMonth is null or has no items. Count: {(cboMonth?.Items?.Count ?? -1)}"); // <<< THÊM DÒNG NÀY
                    }

                    if (cboYear != null && cboYear.Items.Count > 0) // Kiểm tra Items.Count > 0
                    {
                        // Cần kiểm tra xem giá trị có tồn tại trong danh sách không trước khi gán
                        int currentYear = DateTime.Now.Year;
                        // Đối với cboYear, DataSource là List<int>, không cần ValueMember
                        if (cboYear.Items.OfType<int>().Any(item => item == currentYear))
                        {
                            cboYear.SelectedItem = currentYear; // Dùng SelectedItem vì DataSource là List<int>
                            Debug.WriteLine($"--- ucThongKe: cboYear default set to {cboYear.SelectedItem}"); // <<< THÊM DÒNG NÀY
                        }
                        else
                        {
                            Debug.WriteLine($"--- ucThongKe: currentYear {currentYear} not found in cboYear items."); // <<< THÊM DÒNG NÀY
                            if (cboYear.Items.Count > 0) cboYear.SelectedIndex = 0; // Chọn item đầu tiên nếu năm hiện tại không có
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"--- ucThongKe: cboYear is null or has no items. Count: {(cboYear?.Items?.Count ?? -1)}"); // <<< THÊM DÒNG NÀY
                    }


                    Debug.WriteLine("--- ucThongKe: Calling LoadReportData..."); // <<< THÊM DÒNG NÀY
                    await LoadReportData();
                    Debug.WriteLine("--- ucThongKe: LoadReportData finished."); // <<< THÊM DÒNG NÀY
                }
                catch (Exception ex) // <<< THÊM CATCH
                {
                    Debug.WriteLine($"*** ERROR in ucThongKe.InitializeDataAsync: {ex.ToString()}"); // Ghi log lỗi chi tiết
                    // Hiển thị lỗi cho người dùng một cách an toàn trên UI thread nếu có thể
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        this.Invoke((MethodInvoker)delegate {
                            MaterialMessageBox.Show(this.FindForm(), $"Lỗi khởi tạo Thống kê: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        });
                    }
                }
            }
            Debug.WriteLine("+++ ucThongKe: InitializeDataAsync finished."); // <<< THÊM DÒNG NÀY
        }


        // --- HÀM TẢI DỮ LIỆU & CÀI ĐẶT GIAO DIỆN ---

        // *** SỬA ĐỔI: THÊM DEBUG ***
        private void PopulateMonthComboBox()
        {
            Debug.WriteLine("...... ucThongKe: PopulateMonthComboBox started."); // <<< THÊM DÒNG NÀY
            if (cboMonth == null)
            {
                Debug.WriteLine("...... ucThongKe: cboMonth is NULL before populating!"); // <<< THÊM DÒNG NÀY
                return;
            }
            try // Thêm try-catch để bắt lỗi cụ thể ở đây
            {
                var months = Enumerable.Range(1, 12)
                                        .Select(i => new { Month = i, Display = "Tháng " + i.ToString() })
                                        .ToList();
                // Thiết lập đúng thứ tự
                cboMonth.DataSource = null; // Xóa datasource cũ nếu có
                cboMonth.Items.Clear();     // Xóa items cũ nếu có
                cboMonth.DataSource = months;
                cboMonth.DisplayMember = "Display";
                cboMonth.ValueMember = "Month"; // Đảm bảo ValueMember được đặt sau DataSource
                cboMonth.DropDownStyle = ComboBoxStyle.DropDownList;
                // Kiểm tra lại sau khi gán DataSource
                if (cboMonth.Items.Count == 0)
                {
                    Debug.WriteLine("...... ucThongKe: cboMonth has 0 items AFTER setting DataSource!"); // <<< THÊM DÒNG NÀY
                }
                Debug.WriteLine($"...... ucThongKe: PopulateMonthComboBox finished. Items count: {cboMonth.Items.Count}"); // <<< THÊM DÒNG NÀY
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR in PopulateMonthComboBox: {ex.ToString()}"); // <<< THÊM DÒNG NÀY
                                                                                         // Không nên throw lại ở đây, chỉ ghi log
            }
        }

        // *** SỬA ĐỔI: THÊM DEBUG ***
        private void PopulateYearComboBox()
        {
            Debug.WriteLine("...... ucThongKe: PopulateYearComboBox started."); // <<< THÊM DÒNG NÀY
            if (cboYear == null)
            {
                Debug.WriteLine("...... ucThongKe: cboYear is NULL before populating!"); // <<< THÊM DÒNG NÀY
                return;
            }
            try // Thêm try-catch để bắt lỗi cụ thể ở đây
            {
                int currentYear = DateTime.Now.Year;
                var years = Enumerable.Range(currentYear - 5, 7) // Lấy 7 năm: current-5 -> current+1
                                        .OrderByDescending(y => y)
                                        .ToList();
                // Thiết lập đúng thứ tự
                cboYear.DataSource = null; // Xóa datasource cũ
                cboYear.Items.Clear();     // Xóa items cũ
                cboYear.DataSource = years; // DataSource là List<int> nên không cần DisplayMember/ValueMember
                cboYear.DropDownStyle = ComboBoxStyle.DropDownList;
                // Kiểm tra lại sau khi gán DataSource
                if (cboYear.Items.Count == 0)
                {
                    Debug.WriteLine("...... ucThongKe: cboYear has 0 items AFTER setting DataSource!"); // <<< THÊM DÒNG NÀY
                }
                Debug.WriteLine($"...... ucThongKe: PopulateYearComboBox finished. Items count: {cboYear.Items.Count}"); // <<< THÊM DÒNG NÀY
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR in PopulateYearComboBox: {ex.ToString()}"); // <<< THÊM DÒNG NÀY
                                                                                        // Không nên throw lại ở đây, chỉ ghi log
            }
        }


        private async Task LoadReportData()
        {
            Debug.WriteLine("....... ucThongKe: LoadReportData started."); // <<< THÊM DÒNG NÀY
            // Sửa kiểm tra: Dùng SelectedItem cho cboYear, SelectedValue cho cboMonth
            if (cboMonth?.SelectedValue == null || cboYear?.SelectedItem == null)
            {
                Debug.WriteLine("....... ucThongKe: LoadReportData - Month SelectedValue or Year SelectedItem is null, exiting."); // <<< THÊM DÒNG NÀY
                if (dgvReport != null) dgvReport.DataSource = null;
                return;
            }

            // Kiểm tra kiểu dữ liệu trước khi ép kiểu/sử dụng
            if (!(cboMonth.SelectedValue is int selectedMonth))
            {
                Debug.WriteLine($"....... ucThongKe: LoadReportData - Month SelectedValue is not an integer. Type: {cboMonth.SelectedValue.GetType().FullName}");
                MaterialMessageBox.Show(this.FindForm(), "Lỗi kiểu dữ liệu tháng.", "Lỗi Tham Số", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (dgvReport != null) dgvReport.DataSource = null;
                return;
            }

            if (!(cboYear.SelectedItem is int selectedYear))
            {
                Debug.WriteLine($"....... ucThongKe: LoadReportData - Year SelectedItem is not an integer. Type: {cboYear.SelectedItem.GetType().FullName}");
                MaterialMessageBox.Show(this.FindForm(), "Lỗi kiểu dữ liệu năm.", "Lỗi Tham Số", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (dgvReport != null) dgvReport.DataSource = null;
                return;
            }

            Debug.WriteLine($"....... ucThongKe: LoadReportData - Selected Month: {selectedMonth}, Year: {selectedYear}"); // <<< THÊM DÒNG NÀY

            DateTime startDate;
            DateTime endDate;
            try
            {
                startDate = new DateTime(selectedYear, selectedMonth, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
                Debug.WriteLine($"....... ucThongKe: LoadReportData - Date range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}"); // <<< THÊM DÒNG NÀY
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Debug.WriteLine($"....... ucThongKe: LoadReportData - Date creation error: {ex.Message}"); // <<< THÊM DÒNG NÀY
                MaterialMessageBox.Show(this.FindForm(), "Tháng hoặc Năm không hợp lệ.", "Lỗi Tham Số", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (dgvReport != null) dgvReport.DataSource = null;
                return;
            }

            // Sử dụng try-finally để đảm bảo Cursor và Button được reset
            // Use Invoke for UI updates from potentially non-UI thread start
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                this.Invoke((MethodInvoker)delegate {
                    this.Cursor = Cursors.WaitCursor;
                    if (btnXemBaoCao != null) btnXemBaoCao.Enabled = false;
                });
            }
            else
            {
                // Handle case where control is not ready
                Debug.WriteLine("....... ucThongKe: LoadReportData - Cannot set WaitCursor/disable button, handle not created.");
                return; // Or decide how to handle this state
            }


            try
            {
                if (dgvReport == null)
                {
                    Debug.WriteLine("....... ucThongKe: LoadReportData - dgvReport is null!"); // <<< THÊM DÒNG NÀY
                    return;
                }
                // Clear data on UI thread before fetching new data
                if (dgvReport.IsHandleCreated && !dgvReport.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate { dgvReport.DataSource = null; });
                }

                Debug.WriteLine("....... ucThongKe: Calling _busThongKe.GetThongKeLuotMuonTheoTheLoai..."); // <<< THÊM DÒNG NÀY
                List<CtBaoCaoLuotMuonTheoTheLoaiDTO> reportData = await _busThongKe.GetThongKeLuotMuonTheoTheLoai(startDate, endDate);
                Debug.WriteLine($"....... ucThongKe: Received {reportData?.Count ?? 0} items from BUS."); // <<< THÊM DÒNG NÀY
                if (reportData == null) { reportData = new List<CtBaoCaoLuotMuonTheoTheLoaiDTO>(); } // Handle null

                // Kiểm tra lại Handle trước khi gán DataSource
                if (this.IsHandleCreated && !this.IsDisposed && dgvReport.IsHandleCreated && !dgvReport.IsDisposed)
                {
                    // Sử dụng Invoke để đảm bảo gán DataSource trên UI thread
                    this.Invoke((MethodInvoker)delegate {
                        dgvReport.DataSource = reportData.ToList(); // Gán dữ liệu mới
                        Debug.WriteLine("....... ucThongKe: Set dgvReport.DataSource."); // <<< THÊM DÒNG NÀY
                        SetupDataGridViewColumns();
                    });
                }
                else
                {
                    Debug.WriteLine("....... ucThongKe: LoadReportData - Handle not created for Form or DGV, cannot set DataSource."); // <<< THÊM DÒNG NÀY
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR (ucThongKe - LoadReportData): {ex.ToString()}"); // <<< THÊM DÒNG NÀY
                                                                                             // Hiển thị lỗi an toàn
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate {
                        MaterialMessageBox.Show(this.FindForm(), $"Lỗi khi tải dữ liệu báo cáo: {ex.Message}", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            }
            finally
            {
                // Reset trạng thái trên UI thread
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate {
                        this.Cursor = Cursors.Default;
                        if (btnXemBaoCao != null) btnXemBaoCao.Enabled = true;
                        Debug.WriteLine("....... ucThongKe: LoadReportData - Cursor and Button state reset."); // <<< THÊM DÒNG NÀY
                    });
                }
            }
            Debug.WriteLine("....... ucThongKe: LoadReportData finished."); // <<< THÊM DÒNG NÀY
        }

        private void SetupDataGridViewColumns()
        {
            Debug.WriteLine("......... ucThongKe: SetupDataGridViewColumns started."); // <<< THÊM DÒNG NÀY
            if (dgvReport == null || dgvReport.DataSource == null || dgvReport.Columns.Count == 0)
            {
                Debug.WriteLine("......... ucThongKe: SetupDataGridViewColumns - DGV, DataSource, or Columns invalid, exiting."); // <<< THÊM DÒNG NÀY
                return;
            }
            try
            {
                // No need for Invoke here if called from LoadReportData's Invoke block
                // If called independently, Invoke would be needed. Assuming it's called after DataSource set in Invoke.
                var columns = dgvReport.Columns;
                // Dùng ContainsKey để kiểm tra an toàn hơn
                // Use nameof for column names to avoid typos and enable refactoring
                if (columns.Contains(nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.TenTheLoai))) { columns[nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.TenTheLoai)].HeaderText = "Thể Loại"; columns[nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.TenTheLoai)].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; } else Debug.WriteLine($"... Col {nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.TenTheLoai)} not found");
                if (columns.Contains(nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.SoLuotMuon))) { columns[nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.SoLuotMuon)].HeaderText = "Lượt Mượn"; columns[nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.SoLuotMuon)].Width = 100; columns[nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.SoLuotMuon)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; } else Debug.WriteLine($"... Col {nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.SoLuotMuon)} not found");
                if (columns.Contains(nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.TiLe))) { columns[nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.TiLe)].HeaderText = "Tỉ Lệ (%)"; columns[nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.TiLe)].Width = 80; columns[nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.TiLe)].DefaultCellStyle.Format = "N2"; columns[nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.TiLe)].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; } else Debug.WriteLine($"... Col {nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.TiLe)} not found");

                // Ẩn các cột không cần thiết (ID) nếu chúng tồn tại
                if (columns.Contains(nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.IdBaoCao))) columns[nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.IdBaoCao)].Visible = false;
                if (columns.Contains(nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.IdTheLoai))) columns[nameof(CtBaoCaoLuotMuonTheoTheLoaiDTO.IdTheLoai)].Visible = false;
                // MaTheLoai might not exist directly in this DTO, comment out if needed
                // if (columns.Contains("MaTheLoai")) columns["MaTheLoai"].Visible = false;

                dgvReport.ReadOnly = true;
                dgvReport.AllowUserToAddRows = false;
                dgvReport.AllowUserToDeleteRows = false;
                dgvReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvReport.MultiSelect = false;
                Debug.WriteLine("......... ucThongKe: SetupDataGridViewColumns finished successfully."); // <<< THÊM DÒNG NÀY

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR in SetupDataGridViewColumns: {ex.ToString()}"); // <<< THÊM DÒNG NÀY
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate {
                        MaterialMessageBox.Show(this.FindForm(), $"Lỗi cấu hình bảng báo cáo: {ex.Message}", "Lỗi Hiển Thị", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    });
                }
            }
        }

        // --- HÀM TIỆN ÍCH TRẠNG THÁI & GIAO DIỆN (Đơn giản) ---
        // Có thể không cần hàm SetControlState riêng biệt ở đây

        // --- SỰ KIỆN CỦA CÁC BUTTON ---

        private async void btnXemBaoCao_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("--- ucThongKe: btnXemBaoCao_Click called."); // <<< THÊM DÒNG NÀY
            await LoadReportData();
        }

        // *** FIX: SỬA PHƯƠNG THỨC btnThoat_Click ***
        private void btnThoat_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("--- ucThongKe: btnThoat_Click called."); // <<< THÊM DÒNG NÀY
            // Optional: Check HasUnsavedChanges (nếu có gì cần lưu)
            // if (HasUnsavedChanges()) { ... }

            // Kích hoạt sự kiện để thông báo cho frmMain
            RequestClose?.Invoke(this, EventArgs.Empty);
            Debug.WriteLine("--- ucThongKe: RequestClose event invoked."); // <<< THÊM DÒNG NÀY
        }
        // *** KẾT THÚC PHẦN SỬA ***


        // --- SỰ KIỆT CỦA DATAGRIDVIEW (Nếu cần tương tác) ---
        // Thường không cần cho DGV báo cáo

    }
}