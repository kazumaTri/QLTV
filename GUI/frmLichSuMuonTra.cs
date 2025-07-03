// File: GUI/frmLichSuMuonTra.cs
using DTO; // Cần using DTO
using MaterialSkin.Controls; // Cần cho MaterialForm, MaterialButton (nếu dùng)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI // Đảm bảo cùng namespace với các Form khác
{
    // Có thể kế thừa MaterialForm nếu bạn đang dùng MaterialSkin
    public partial class frmLichSuMuonTra : MaterialForm // Hoặc kế thừa Form nếu không dùng MaterialSkin
    {
        // Biến lưu trữ dữ liệu được truyền vào
        private List<PhieuMuonTraDTO> _lichSuMuonTra;
        private string _maDocGia;
        private string _tenDocGia;

        // Constructor để nhận dữ liệu từ form gọi (ucQuanLyDocGia)
        public frmLichSuMuonTra(List<PhieuMuonTraDTO> lichSu, string maDocGia, string tenDocGia)
        {
            InitializeComponent(); // Hàm này do Designer tạo

            // Lưu dữ liệu được truyền vào
            _lichSuMuonTra = lichSu ?? new List<PhieuMuonTraDTO>(); // Đảm bảo danh sách không bị null
            _maDocGia = maDocGia;
            _tenDocGia = tenDocGia;

            // Cập nhật tiêu đề Form để hiển thị tên và mã độc giả
            this.Text = $"Lịch sử mượn trả - {_tenDocGia} ({_maDocGia})";

            // Tùy chọn: Nếu bạn có thêm Label trong Designer để hiển thị thông tin này, hãy cập nhật text của nó ở đây
            // Ví dụ:
            // if (lblInfoDocGia != null)
            // {
            //     lblInfoDocGia.Text = $"Độc giả: {_tenDocGia} (Mã: {_maDocGia})";
            // }
        }

        // Sự kiện xảy ra khi Form được tải lên
        private void frmLichSuMuonTra_Load(object sender, EventArgs e)
        {
            // Gọi hàm để tải dữ liệu lên DataGridView
            LoadDataGridView();
        }

        // Hàm tải và cấu hình DataGridView
        private void LoadDataGridView()
        {
            dgvLichSu.DataSource = null; // Xóa dữ liệu cũ (nếu có) để tránh trùng lặp
            dgvLichSu.AutoGenerateColumns = false; // Quan trọng: Tắt tự động tạo cột dựa trên thuộc tính DTO
            dgvLichSu.Columns.Clear(); // Xóa các cột đã được định nghĩa trong Designer (nếu có) hoặc từ lần load trước

            // Gọi hàm để định nghĩa và cấu hình các cột thủ công
            SetupDataGridViewColumns();

            // Gán danh sách lịch sử làm nguồn dữ liệu cho DataGridView
            dgvLichSu.DataSource = _lichSuMuonTra;

            // Hiển thị thông báo nếu không có bản ghi lịch sử nào
            if (_lichSuMuonTra.Count == 0)
            {
                // Sử dụng MaterialMessageBox nếu có, nếu không thì dùng MessageBox thông thường
                MaterialMessageBox.Show(this, "Độc giả này chưa có lịch sử mượn trả sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Hoặc: MessageBox.Show("Độc giả này chưa có lịch sử mượn trả sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Hàm định nghĩa và cấu hình các cột cho DataGridView Lịch Sử
        private void SetupDataGridViewColumns()
        {
            // Ví dụ cấu hình các cột:
            // Cột Số Phiếu (có thể ẩn đi)
            AddColumn("SoPhieuMuonTra", "Số Phiếu", 80, visible: false);
            // Cột Mã Cuốn Sách
            AddColumn("MaCuonSach", "Mã Sách", 100);
            // Cột Tên Tựa Sách (lấy từ DTO, cần Include khi query)
            AddColumn("TenTuaSach", "Tên Sách", 250, DataGridViewAutoSizeColumnMode.Fill);
            // Cột Ngày Mượn (định dạng ngày giờ)
            AddColumn("NgayMuon", "Ngày Mượn", 110, format: "dd/MM/yyyy HH:mm");
            // Cột Hạn Trả (định dạng ngày)
            AddColumn("HanTra", "Hạn Trả", 100, format: "dd/MM/yyyy");
            // Cột Ngày Trả (định dạng ngày giờ, sẽ hiển thị "Chưa trả" nếu null)
            AddColumn("NgayTra", "Ngày Trả", 110, format: "dd/MM/yyyy HH:mm");
            // Cột Tiền Phạt (định dạng số, căn phải, hiển thị "0" nếu null)
            AddColumn("SoTienPhat", "Tiền Phạt", 100, alignment: DataGridViewContentAlignment.MiddleRight, format: "N0");

            // === Cấu hình chung cho DataGridView ===
            dgvLichSu.AllowUserToAddRows = false; // Không cho người dùng thêm dòng
            dgvLichSu.AllowUserToDeleteRows = false; // Không cho người dùng xóa dòng
            dgvLichSu.ReadOnly = true; // Chỉ cho xem, không cho sửa trực tiếp trên lưới
            dgvLichSu.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Chọn cả dòng khi click
            dgvLichSu.MultiSelect = false; // Không cho chọn nhiều dòng
            dgvLichSu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; // Tắt tự động điều chỉnh độ rộng cột chung
            dgvLichSu.RowHeadersVisible = false; // Ẩn cột header mặc định ở đầu mỗi dòng

            // Gắn sự kiện CellFormatting để tùy chỉnh hiển thị cell (ví dụ: tô màu, hiển thị text thay thế)
            dgvLichSu.CellFormatting -= DgvLichSu_CellFormatting; // Hủy nếu đã gắn trước đó
            dgvLichSu.CellFormatting += DgvLichSu_CellFormatting; // Gắn sự kiện
        }

        // Hàm helper để thêm cột vào DataGridView một cách nhanh chóng
        private void AddColumn(string dataPropertyName, string headerText, int width,
                                DataGridViewAutoSizeColumnMode autoSizeMode = DataGridViewAutoSizeColumnMode.None,
                                DataGridViewContentAlignment alignment = DataGridViewContentAlignment.MiddleLeft,
                                string? format = null, bool visible = true)
        {
            var column = new DataGridViewTextBoxColumn
            {
                DataPropertyName = dataPropertyName, // Tên thuộc tính trong PhieuMuonTraDTO
                HeaderText = headerText,          // Tiêu đề cột hiển thị trên lưới
                Name = "col" + dataPropertyName,    // Tên định danh cho cột (duy nhất)
                Width = width,                    // Độ rộng cột
                AutoSizeMode = autoSizeMode,      // Chế độ tự động điều chỉnh độ rộng
                DefaultCellStyle = new DataGridViewCellStyle // Kiểu hiển thị mặc định cho cell
                {
                    Alignment = alignment,        // Căn lề nội dung cell
                    Format = format               // Định dạng hiển thị (vd: ngày tháng, số)
                },
                Visible = visible,                // Cột có hiển thị hay không
                SortMode = DataGridViewColumnSortMode.NotSortable // Tắt chức năng sắp xếp trên cột này
            };
            dgvLichSu.Columns.Add(column); // Thêm cột vào DataGridView
        }

        // Sự kiện định dạng Cell để tùy chỉnh hiển thị (Ví dụ: Tô màu dòng trả trễ)
        private void DgvLichSu_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Đảm bảo có dòng hợp lệ và có dữ liệu DTO
            if (e.RowIndex >= 0 && dgvLichSu.Rows[e.RowIndex].DataBoundItem is PhieuMuonTraDTO dto)
            {
                // --- Tô màu dòng trả trễ ---
                // Kiểm tra xem phiếu đã được trả VÀ ngày trả thực tế > hạn trả (chỉ so sánh phần ngày)
                if (dto.NgayTra.HasValue && dto.NgayTra.Value.Date > dto.HanTra.Date)
                {
                    // Áp dụng màu nền và màu chữ cho toàn bộ dòng
                    e.CellStyle.BackColor = Color.MistyRose; // Màu hồng nhạt cho dễ nhìn
                    e.CellStyle.ForeColor = Color.DarkRed;
                    // Đặt ToolTip để giải thích tại sao dòng bị tô màu
                    foreach (DataGridViewCell cell in dgvLichSu.Rows[e.RowIndex].Cells)
                    {
                        cell.ToolTipText = "Sách trả trễ hạn";
                    }
                }
                else
                {
                    // Nếu không phải trả trễ, đảm bảo màu nền và màu chữ trở về mặc định
                    // Quan trọng để tránh việc màu bị áp dụng sai khi cuộn lưới
                    e.CellStyle.BackColor = SystemColors.Window;
                    e.CellStyle.ForeColor = SystemColors.ControlText;
                    foreach (DataGridViewCell cell in dgvLichSu.Rows[e.RowIndex].Cells)
                    {
                        cell.ToolTipText = string.Empty; // Xóa tooltip
                    }
                }

                // --- Định dạng hiển thị cho cột cụ thể ---
                // Cột Ngày Trả: Nếu NgayTra là null, hiển thị "Chưa trả"
                if (dgvLichSu.Columns[e.ColumnIndex].DataPropertyName == "NgayTra")
                {
                    if (dto.NgayTra == null)
                    {
                        e.Value = "Chưa trả";
                        e.CellStyle.ForeColor = Color.Gray; // Có thể đổi màu chữ cho dễ phân biệt
                        e.FormattingApplied = true; // Báo rằng đã xử lý định dạng, không cần định dạng mặc định nữa
                    }
                    // Nếu đã có ngày trả, đảm bảo màu chữ bình thường (trừ khi là dòng trả trễ)
                    else if (!(dto.NgayTra.Value.Date > dto.HanTra.Date)) // Kiểm tra lại để không ghi đè màu của dòng trả trễ
                    {
                        e.CellStyle.ForeColor = SystemColors.ControlText;
                    }
                }
                // Cột Tiền Phạt: Nếu SoTienPhat là null, hiển thị "0"
                else if (dgvLichSu.Columns[e.ColumnIndex].DataPropertyName == "SoTienPhat")
                {
                    if (dto.SoTienPhat == null)
                    {
                        e.Value = "0"; // Hiển thị số 0 thay vì để trống
                        e.FormattingApplied = true;
                    }
                    // Nếu có tiền phạt và là dòng trả trễ, màu chữ đã được xử lý ở trên
                    // Nếu có tiền phạt nhưng không phải trả trễ (ít xảy ra), đảm bảo màu chữ bình thường
                    else if (dto.SoTienPhat.HasValue && !(dto.NgayTra.HasValue && dto.NgayTra.Value.Date > dto.HanTra.Date))
                    {
                        e.CellStyle.ForeColor = SystemColors.ControlText;
                    }
                }
            }
        }


        // Xử lý sự kiện khi nhấn nút Đóng
        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close(); // Đóng form lịch sử này
        }
    }
}