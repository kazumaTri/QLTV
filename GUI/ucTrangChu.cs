// --- USING ---
using BUS; // Namespace chứa các lớp BUS và Interface IBUS
using DTO; // Namespace chứa các DTO (bao gồm ThongKeItemDTO, MonthlyBorrowCountsDTO, ThongBaoDTO)
using MaterialSkin.Controls; // Dùng MaterialMessageBox
using System;
using System.Collections.Generic; // <<< THÊM using cho List
using System.Drawing;
using System.IO; // Dùng Path, File
using System.Linq; // Dùng cho .Count(), LINQ operations
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics; // Thêm using cho Debug
using System.Globalization; // <<< THÊM: Để lấy tên tháng
using System.Windows.Forms.DataVisualization.Charting; // <<< THÊM: Để dùng Chart control
using static GUI.frmMain; // <<< GIỮ NGUYÊN: Để dùng IRequiresDataLoading

namespace GUI
{
    /// <summary>
    /// UserControl Trang Chủ, hiển thị thống kê tổng quan, top list, biểu đồ và hoạt động gần đây.
    /// </summary>
    public partial class ucTrangChu : UserControl, IRequiresDataLoading
    {
        // --- THÊM: KHAI BÁO DELEGATE VÀ EVENT CHO ĐIỀU HƯỚNG ---
        public delegate void NavigateRequestHandler(string targetScreenKey);
        public event NavigateRequestHandler? RequestNavigate;
        // --- KẾT THÚC THÊM ---

        // --- KHAI BÁO BIẾN DEPENDENCIES (Sử dụng Interfaces) ---
        private readonly IBUSSach _busSach;
        private readonly IBUSDocGia _busDocGia;
        private readonly IBUSPhieuMuonTra _busPhieuMuonTra;
        private readonly IBUSThongKe _busThongKe;
        private readonly IBUSThongBao _busThongBao; // <<< THÊM: Dependency cho BUS Thông báo

        private readonly string _iconFolderPath;

        // --- CONSTRUCTOR NHẬN DI (ĐÃ CẬP NHẬT) ---
        public ucTrangChu(
            IBUSSach busSach,
            IBUSDocGia busDocGia,
            IBUSPhieuMuonTra busPhieuMuonTra,
            IBUSThongKe busThongKe,
            IBUSThongBao busThongBao) // <<< THÊM: Tham số IBUSThongBao
        {
            InitializeComponent(); // Hàm khởi tạo các control từ Designer

            // Gán các dependencies được inject
            _busSach = busSach ?? throw new ArgumentNullException(nameof(busSach));
            _busDocGia = busDocGia ?? throw new ArgumentNullException(nameof(busDocGia));
            _busPhieuMuonTra = busPhieuMuonTra ?? throw new ArgumentNullException(nameof(busPhieuMuonTra));
            _busThongKe = busThongKe ?? throw new ArgumentNullException(nameof(busThongKe));
            _busThongBao = busThongBao ?? throw new ArgumentNullException(nameof(busThongBao)); // <<< THÊM: Gán và kiểm tra null

            // Xác định đường dẫn thư mục Icons
            _iconFolderPath = Path.Combine(Application.StartupPath, "Icons");
            if (!Directory.Exists(_iconFolderPath))
            {
                _iconFolderPath = Path.Combine(Application.StartupPath, "..", "..", "..", "Icons");
                if (!Directory.Exists(_iconFolderPath))
                {
                    Debug.WriteLine($"Warning: Icons folder not found in standard locations for ucTrangChu. Using StartupPath: {Application.StartupPath}");
                    _iconFolderPath = Application.StartupPath;
                }
            }
        }

        // --- PHƯƠNG THỨC KHỞI TẠO DỮ LIỆU (TỪ IRequiresDataLoading - ĐÃ CẬP NHẬT) ---
        public async Task InitializeDataAsync()
        {
            if (!this.DesignMode)
            {
                Debug.WriteLine("ucTrangChu.InitializeDataAsync called.");
                // --- Set text mặc định/loading ---
                lblSoSachValue.Text = "...";
                lblDocGiaValue.Text = "...";
                lblDangMuonValue.Text = "...";
                lblQuaHanValue.Text = "...";
                UpdateTopLabels(new[] { lblTopTuaSach1, lblTopTuaSach2, lblTopTuaSach3 }, new List<ThongKeItemDTO>(), true);
                UpdateTopLabels(new[] { lblTopTheLoai1, lblTopTheLoai2, lblTopTheLoai3 }, new List<ThongKeItemDTO>(), true);
                ClearChartsOnError(true);
                ClearRecentActivitiesOnError(true); // <<< THÊM: Xóa list hoạt động khi loading

                LoadIcons(); // Load icon đồng bộ

                try
                {
                    // --- Chạy song song các tác vụ load dữ liệu ---
                    var loadStatsTask = LoadStatistics();
                    var loadTopListsTask = LoadTopBorrowedStatsAsync();
                    var loadChartsTask = LoadChartDataAsync();
                    var loadActivitiesTask = LoadRecentActivitiesAsync(); // <<< THÊM: Task load hoạt động gần đây

                    // Chờ tất cả hoàn thành
                    await Task.WhenAll(loadStatsTask, loadTopListsTask, loadChartsTask, loadActivitiesTask); // <<< THÊM: Chờ cả loadActivitiesTask

                    Debug.WriteLine("ucTrangChu.InitializeDataAsync completed successfully.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error during ucTrangChu InitializeDataAsync: {ex}");
                    // --- Set text lỗi cho các label ---
                    lblSoSachValue.Text = "Lỗi";
                    lblDocGiaValue.Text = "Lỗi";
                    lblDangMuonValue.Text = "Lỗi";
                    lblQuaHanValue.Text = "Lỗi";
                    UpdateTopLabels(new[] { lblTopTuaSach1, lblTopTuaSach2, lblTopTuaSach3 }, new List<ThongKeItemDTO>(), true);
                    UpdateTopLabels(new[] { lblTopTheLoai1, lblTopTheLoai2, lblTopTheLoai3 }, new List<ThongKeItemDTO>(), true);
                    ClearChartsOnError();
                    ClearRecentActivitiesOnError(); // <<< THÊM: Xóa list hoạt động khi lỗi
                }
            }
        }

        // --- HÀM TẢI ICON CHO PICTUREBOX (Giữ nguyên) ---
        private void LoadIcons()
        {
            picIconSach.Image = LoadIconFromFile("sach_icon_48.png");
            picIconDocGia.Image = LoadIconFromFile("docgia_icon_48.png");
            picIconDangMuon.Image = LoadIconFromFile("muonsach_icon_48.png");
            picIconQuaHan.Image = LoadIconFromFile("warning_icon_48.png");
        }

        private Image? LoadIconFromFile(string iconFileName)
        {
            if (string.IsNullOrEmpty(iconFileName)) return null;
            string iconPath = Path.Combine(_iconFolderPath, iconFileName);
            try
            {
                if (File.Exists(iconPath))
                {
                    byte[] fileBytes = File.ReadAllBytes(iconPath);
                    using (var ms = new MemoryStream(fileBytes)) { return new Bitmap(ms); }
                }
                else { Debug.WriteLine($"Icon file not found for ucTrangChu: {iconPath}"); return null; }
            }
            catch (Exception ex) { Debug.WriteLine($"Error loading icon file '{iconFileName}' for ucTrangChu: {ex.Message}"); return null; }
        }

        // --- HÀM TẢI DỮ LIỆU THỐNG KÊ CƠ BẢN (Giữ nguyên) ---
        private async Task LoadStatistics()
        {
            Debug.WriteLine("LoadStatistics starting...");
            int soSach = 0;
            int soDocGia = 0;
            int soDangMuon = 0;
            int soQuaHan = 0;

            try
            {
                var sachTask = _busSach.GetAllSachAsync();
                var docGiaTask = _busDocGia.GetActiveReaderCountAsync();
                var dangMuonTask = _busPhieuMuonTra.GetBorrowedCountAsync();
                var quaHanTask = _busPhieuMuonTra.GetOverdueCountAsync();

                await Task.WhenAll(sachTask, docGiaTask, dangMuonTask, quaHanTask);

                soSach = (await sachTask)?.Count() ?? 0;
                soDocGia = await docGiaTask;
                soDangMuon = await dangMuonTask;
                soQuaHan = await quaHanTask;

                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate {
                        if (lblSoSachValue != null && !lblSoSachValue.IsDisposed) lblSoSachValue.Text = soSach.ToString("N0");
                        if (lblDocGiaValue != null && !lblDocGiaValue.IsDisposed) lblDocGiaValue.Text = soDocGia.ToString("N0");
                        if (lblDangMuonValue != null && !lblDangMuonValue.IsDisposed) lblDangMuonValue.Text = soDangMuon.ToString("N0");
                        if (lblQuaHanValue != null && !lblQuaHanValue.IsDisposed)
                        {
                            lblQuaHanValue.Text = soQuaHan.ToString("N0");
                            lblQuaHanValue.ForeColor = (soQuaHan > 0) ? Color.FromArgb(192, 0, 0) : (this.ParentForm as MaterialForm)?.ForeColor ?? Color.Black;
                        }
                        Debug.WriteLine("LoadStatistics UI updated.");
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR LoadStatistics: {ex}");
                // Lỗi đã được xử lý ở InitializeDataAsync
            }
        }

        // --- HÀM TẢI DỮ LIỆU THỐNG KÊ TOP MƯỢN (Cho Labels - Giữ nguyên) ---
        private async Task LoadTopBorrowedStatsAsync()
        {
            const int topNLabels = 3;
            Debug.WriteLine($"LoadTopBorrowedStatsAsync starting (Top {topNLabels} for labels)...");
            List<ThongKeItemDTO> topTuaSachResult = new List<ThongKeItemDTO>();
            List<ThongKeItemDTO> topTheLoaiResult = new List<ThongKeItemDTO>();

            try
            {
                var topTuaSachTask = _busThongKe.GetTopBorrowedTuaSachAsync(topNLabels);
                var topTheLoaiTask = _busThongKe.GetTopBorrowedTheLoaiAsync(topNLabels);

                await Task.WhenAll(topTuaSachTask, topTheLoaiTask);

                topTuaSachResult = await topTuaSachTask ?? new List<ThongKeItemDTO>();
                topTheLoaiResult = await topTheLoaiTask ?? new List<ThongKeItemDTO>();

                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate {
                        UpdateTopLabels(new[] { lblTopTuaSach1, lblTopTuaSach2, lblTopTuaSach3 }, topTuaSachResult);
                        UpdateTopLabels(new[] { lblTopTheLoai1, lblTopTheLoai2, lblTopTheLoai3 }, topTheLoaiResult);
                        Debug.WriteLine("LoadTopBorrowedStatsAsync UI (labels) updated.");
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading top borrowed stats for labels: {ex}");
                // Lỗi đã được xử lý ở InitializeDataAsync
            }
        }

        // --- HÀM HELPER CẬP NHẬT LABEL TOP (Giữ nguyên) ---
        private void UpdateTopLabels(MaterialLabel[] labels, List<ThongKeItemDTO> data, bool forceClear = false)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                var label = labels[i];
                if (label != null && !label.IsDisposed)
                {
                    if (i < data.Count)
                    {
                        string ten = data[i].Ten;
                        const int maxLen = 25;
                        if (ten.Length > maxLen) ten = ten.Substring(0, maxLen) + "...";
                        label.Text = $"{i + 1}. {ten} ({data[i].SoLuotMuon} lượt)";
                        label.Visible = true;
                        label.Tag = data[i];
                    }
                    else
                    {
                        label.Text = "-";
                        label.Visible = false;
                        label.Tag = null;
                    }
                }
                else if (forceClear && label != null)
                {
                    label.Text = string.Empty;
                    label.Visible = false;
                }
            }
        }

        // *** PHẦN CODE CHO BIỂU ĐỒ (Giữ nguyên) ***
        // (Giữ nguyên các hàm LoadChartDataAsync, UpdateTheLoaiPieChart, UpdateLuotMuonColumnChart, ClearChartsOnError)
        private async Task LoadChartDataAsync()
        {
            Debug.WriteLine("LoadChartDataAsync starting...");
            try
            {
                int currentYear = DateTime.Now.Year;
                int topNTheLoaiChart = 5;

                var topTheLoaiTask = _busThongKe.GetTopBorrowedTheLoaiAsync(topNTheLoaiChart);
                var monthlyBorrowsTask = _busThongKe.GetMonthlyBorrowCountsAsync(currentYear);

                await Task.WhenAll(topTheLoaiTask, monthlyBorrowsTask);

                var topTheLoaiData = await topTheLoaiTask ?? new List<ThongKeItemDTO>();
                var monthlyBorrowsData = await monthlyBorrowsTask ?? new List<MonthlyBorrowCountsDTO>();

                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate {
                        UpdateTheLoaiPieChart(topTheLoaiData);
                        UpdateLuotMuonColumnChart(monthlyBorrowsData, currentYear);
                    });
                    Debug.WriteLine("LoadChartDataAsync UI (charts) updated.");
                }
                else
                {
                    Debug.WriteLine("LoadChartDataAsync - Cannot update chart, handle not created or control disposed.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading chart data: {ex}");
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate { ClearChartsOnError(); });
                }
            }
        }

        private void UpdateTheLoaiPieChart(List<ThongKeItemDTO> data)
        {
            if (chartTheLoai == null || chartTheLoai.IsDisposed) return;
            chartTheLoai.Series.Clear();
            chartTheLoai.Legends.Clear();

            if (data == null || !data.Any())
            {
                chartTheLoai.Titles.Clear();
                chartTheLoai.Titles.Add("Tỷ lệ mượn theo Thể loại (Chưa có dữ liệu)");
                chartTheLoai.Invalidate();
                return;
            }

            if (!chartTheLoai.Titles.Any() || chartTheLoai.Titles[0].Text.Contains("Chưa có dữ liệu") || chartTheLoai.Titles[0].Text.Contains("Lỗi"))
            {
                chartTheLoai.Titles.Clear();
                chartTheLoai.Titles.Add("Top 5 Thể loại mượn nhiều nhất");
            }

            Legend legend = new Legend("LegendTheLoai")
            {
                Title = "Thể loại",
                Docking = Docking.Right,
                Alignment = StringAlignment.Center
            };
            chartTheLoai.Legends.Add(legend);

            var series = new Series("TheLoaiPieSeries")
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true,
                LabelFormat = "#,##0",
                Font = new Font("Arial", 8f),
                ToolTip = "#VALX: #VALY lượt (#PERCENT{P1})",
                Legend = "LegendTheLoai",
                LegendText = "#VALX"
            };

            foreach (var item in data)
            {
                DataPoint dp = series.Points.Add(item.SoLuotMuon);
                dp.AxisLabel = item.Ten;
                dp.Label = $"{item.SoLuotMuon}";
            }
            chartTheLoai.Series.Add(series);
            chartTheLoai.Invalidate();
        }

        private void UpdateLuotMuonColumnChart(List<MonthlyBorrowCountsDTO> data, int year)
        {
            if (chartLuotMuonThang == null || chartLuotMuonThang.IsDisposed) return;
            chartLuotMuonThang.Series.Clear();

            if (data == null || !data.Any(m => m.BorrowCount > 0))
            {
                chartLuotMuonThang.Titles.Clear();
                chartLuotMuonThang.Titles.Add($"Số lượt mượn theo Tháng (Năm {year} - Chưa có dữ liệu)");
                chartLuotMuonThang.Invalidate();
                return;
            }

            if (!chartLuotMuonThang.Titles.Any() || chartLuotMuonThang.Titles[0].Text.Contains("Chưa có dữ liệu") || chartLuotMuonThang.Titles[0].Text.Contains("Lỗi"))
            {
                chartLuotMuonThang.Titles.Clear();
                chartLuotMuonThang.Titles.Add($"Số lượt mượn theo Tháng (Năm {year})");
            }

            var series = new Series("LuotMuonColumnSeries")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true,
                LabelFormat = "#,##0",
                Font = new Font("Arial", 8f),
                ToolTip = "Tháng #VALX: #VALY lượt"
            };

            CultureInfo ci = CultureInfo.CurrentCulture;
            foreach (var item in data)
            {
                DataPoint dp = new DataPoint { XValue = item.Month, AxisLabel = ci.DateTimeFormat.GetAbbreviatedMonthName(item.Month) };
                dp.YValues = new double[] { item.BorrowCount };
                series.Points.Add(dp);
            }
            chartLuotMuonThang.Series.Add(series);

            var chartArea = chartLuotMuonThang.ChartAreas[0];
            chartArea.AxisX.Title = "Tháng"; chartArea.AxisX.Interval = 1;
            chartArea.AxisX.LabelStyle.Angle = 0; chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.Title = "Số lượt mượn"; chartArea.AxisY.LabelStyle.Format = "#,##0";
            chartArea.AxisY.MajorGrid.LineColor = Color.Gainsboro;

            chartLuotMuonThang.Invalidate();
        }

        private void ClearChartsOnError(bool isLoading = false)
        {
            string messageSuffix = isLoading ? "(Đang tải...)" : "(Lỗi tải dữ liệu)";
            if (chartTheLoai != null && !chartTheLoai.IsDisposed)
            {
                chartTheLoai.Series.Clear(); chartTheLoai.Legends.Clear();
                if (!chartTheLoai.Titles.Any() || !chartTheLoai.Titles[0].Text.Contains(messageSuffix))
                { chartTheLoai.Titles.Clear(); chartTheLoai.Titles.Add($"Top Thể loại {messageSuffix}"); }
                chartTheLoai.Invalidate();
            }
            if (chartLuotMuonThang != null && !chartLuotMuonThang.IsDisposed)
            {
                chartLuotMuonThang.Series.Clear(); chartLuotMuonThang.Legends.Clear();
                if (!chartLuotMuonThang.Titles.Any() || !chartLuotMuonThang.Titles[0].Text.Contains(messageSuffix))
                { chartLuotMuonThang.Titles.Clear(); chartLuotMuonThang.Titles.Add($"Lượt mượn theo Tháng {messageSuffix}"); }
                chartLuotMuonThang.Invalidate();
            }
        }


        // *** ========================================================== ***
        // *** START: PHẦN CODE MỚI CHO HOẠT ĐỘNG GẦN ĐÂY             ***
        // *** ========================================================== ***

        /// <summary>
        /// Tải dữ liệu hoạt động gần đây vào ListView.
        /// </summary>
        /// <param name="count">Số lượng hoạt động cần hiển thị.</param>
        private async Task LoadRecentActivitiesAsync(int count = 7)
        {
            Debug.WriteLine($"LoadRecentActivitiesAsync starting (Top {count})...");
            if (lvRecentActivity == null || lvRecentActivity.IsDisposed) // Kiểm tra xem ListView đã được tạo chưa
            {
                Debug.WriteLine("LoadRecentActivitiesAsync - ListView 'lvRecentActivity' is null or disposed.");
                return;
            }

            List<ThongBaoDTO> recentActivities = new List<ThongBaoDTO>();
            bool loadError = false;

            try
            {
                recentActivities = await _busThongBao.GetRecentNotificationsAsync(count);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading recent activities: {ex}");
                loadError = true;
            }

            // Cập nhật UI trên luồng chính
            if (this.IsHandleCreated && !this.IsDisposed && lvRecentActivity != null && !lvRecentActivity.IsDisposed)
            {
                this.Invoke((MethodInvoker)delegate {
                    lvRecentActivity.Items.Clear(); // Xóa các mục cũ

                    if (loadError)
                    {
                        // Hiển thị thông báo lỗi trong ListView
                        ListViewItem errorItem = new ListViewItem("Lỗi");
                        errorItem.SubItems.Add("Không thể tải hoạt động gần đây.");
                        errorItem.ForeColor = Color.Red;
                        lvRecentActivity.Items.Add(errorItem);
                    }
                    else if (recentActivities == null || !recentActivities.Any())
                    {
                        // Hiển thị thông báo không có dữ liệu
                        ListViewItem noDataItem = new ListViewItem(""); // Cột thời gian trống
                        noDataItem.SubItems.Add("Chưa có hoạt động nào gần đây.");
                        noDataItem.ForeColor = Color.Gray;
                        lvRecentActivity.Items.Add(noDataItem);
                    }
                    else
                    {
                        // Thêm các hoạt động vào ListView
                        foreach (var activity in recentActivities)
                        {
                            ListViewItem item = new ListViewItem(activity.NgayTao.ToString("dd/MM HH:mm")); // Cột 1: Thời gian
                            item.SubItems.Add(activity.NoiDung); // Cột 2: Nội dung
                            // Thêm tooltip nếu muốn hiển thị đầy đủ hơn khi hover
                            item.ToolTipText = $"Loại: {activity.TieuDe}\nNgày tạo: {activity.NgayTao:dd/MM/yyyy HH:mm:ss}";
                            lvRecentActivity.Items.Add(item);
                        }
                    }
                    // Tự động điều chỉnh độ rộng cột (tùy chọn)
                    // lvRecentActivity.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    // lvRecentActivity.Columns[1].Width = -2; // Hoặc set độ rộng cột thứ 2 tự động theo nội dung

                    Debug.WriteLine("LoadRecentActivitiesAsync UI updated.");
                });
            }
        }

        /// <summary>
        /// Xóa ListView hoạt động và hiển thị thông báo lỗi/loading.
        /// </summary>
        /// <param name="isLoading">True nếu đang loading, False nếu là lỗi.</param>
        private void ClearRecentActivitiesOnError(bool isLoading = false)
        {
            if (lvRecentActivity == null || lvRecentActivity.IsDisposed) return;

            string message = isLoading ? "Đang tải hoạt động..." : "Lỗi tải hoạt động.";
            Color messageColor = isLoading ? Color.Gray : Color.Red;

            // Cập nhật trên luồng UI
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                this.Invoke((MethodInvoker)delegate {
                    if (lvRecentActivity != null && !lvRecentActivity.IsDisposed)
                    {
                        lvRecentActivity.Items.Clear();
                        ListViewItem infoItem = new ListViewItem(""); // Cột thời gian trống
                        infoItem.SubItems.Add(message);
                        infoItem.ForeColor = messageColor;
                        lvRecentActivity.Items.Add(infoItem);
                    }
                });
            }
        }

        // *** ========================================================== ***
        // *** END: PHẦN CODE MỚI CHO HOẠT ĐỘNG GẦN ĐÂY               ***
        // *** ========================================================== ***


        // --- PHƯƠNG THỨC KÍCH HOẠT SỰ KIỆN ĐIỀU HƯỚNG (Giữ nguyên) ---
        private void RaiseRequestNavigate(string targetKey)
        {
            RequestNavigate?.Invoke(targetKey);
            Debug.WriteLine($"ucTrangChu: RequestNavigate invoked with key: {targetKey}");
        }

        // --- CÁC HÀM XỬ LÝ SỰ KIỆN CLICK CHO CÁC CARD (Giữ nguyên) ---
        private void CardSoSach_Click(object? sender, EventArgs e) { RaiseRequestNavigate("qlsach"); }
        private void CardDocGia_Click(object? sender, EventArgs e) { RaiseRequestNavigate("qldocgia"); }
        private void CardDangMuon_Click(object? sender, EventArgs e) { RaiseRequestNavigate("qlphieumuontra"); }
        private void CardQuaHan_Click(object? sender, EventArgs e) { RaiseRequestNavigate("qlphieumuontra"); }

        // --- Xử lý sự kiện click cho các Card Thống kê Top List (Giữ nguyên) ---
        private void CardTopTuaSach_Click(object sender, EventArgs e)
        {
            RaiseRequestNavigate("qltuasach");
        }

        private void CardTopTheLoai_Click(object sender, EventArgs e)
        {
            RaiseRequestNavigate("qltheloai");
        }
    }
}