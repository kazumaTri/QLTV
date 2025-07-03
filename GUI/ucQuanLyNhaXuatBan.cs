using BUS; // Thêm using cho tầng Business Logic
using DAL; // Thêm using cho tầng Data Access (Nếu cần khởi tạo DAL trực tiếp)
using DAL.Models; // Thêm using cho Models (Nếu cần khởi tạo Context trực tiếp)
using DTO; // Thêm using cho Data Transfer Objects
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions; // Thêm namespace này để dùng Regex
using System.Windows.Forms;

namespace GUI
{
    public partial class ucQuanLyNhaXuatBan : UserControl
    {
        private readonly IBUSNhaXuatBan _busNhaXuatBan; // Biến chứa đối tượng BUS
        private bool _isAdding = false; // Cờ kiểm tra trạng thái Thêm/Sửa
        private NhaXuatBanDTO? _selectedNhaXuatBan = null; // Lưu trữ NXB đang được chọn

        // --- Constructor ---
        // Cách 1: Sử dụng Dependency Injection (Khuyến nghị)
        // Đảm bảo bạn đã đăng ký IBUSNhaXuatBan trong DI Container (ví dụ: trong Program.cs)
        public ucQuanLyNhaXuatBan(IBUSNhaXuatBan busNhaXuatBan)
        {
            InitializeComponent();
            _busNhaXuatBan = busNhaXuatBan;
            // Gắn các sự kiện (có thể làm trong Designer)
            WireUpEvents();
        }

        /*
        // Cách 2: Khởi tạo trực tiếp (Nếu không dùng Dependency Injection)
        // Yêu cầu phải có cách truy cập Connection String hoặc DbContextOptions
        public ucQuanLyNhaXuatBan()
        {
            InitializeComponent();

            // Cần cung cấp DbContextOptions hoặc Connection String phù hợp ở đây
            // Ví dụ đơn giản (không khuyến khích dùng connection string cứng):
            // var contextOptions = new DbContextOptionsBuilder<QLTVContext>()
            //     .UseSqlServer("Your_Connection_String_Here") // Thay bằng Connection String thực tế
            //     .Options;
            // QLTVContext context = new QLTVContext(contextOptions);

            // Hoặc nếu bạn có một cách khác để lấy instance của QLTVContext
            // QLTVContext context = GetYourDbContextInstance(); // Hàm tự định nghĩa

            // IDALNhaXuatBan dalNhaXuatBan = new DALNhaXuatBan(context);
            // _busNhaXuatBan = new BUSNhaXuatBan(dalNhaXuatBan);

            // --- !!! Quan trọng: Phần khởi tạo DAL/BUS ở trên chỉ là ví dụ. ---
            // --- Bạn cần điều chỉnh cho phù hợp với cách quản lý DbContext và khởi tạo lớp trong dự án của bạn ---
            // --- Nếu không chắc chắn, hãy xem lại cách các UserControl khác được khởi tạo hoặc cấu hình DI trong Program.cs ---

             if (_busNhaXuatBan == null)
             {
                  // Xử lý lỗi nếu không khởi tạo được BUS (ví dụ: hiển thị thông báo và thoát)
                  MessageBox.Show("Không thể khởi tạo lớp xử lý nghiệp vụ Nhà Xuất Bản.", "Lỗi Khởi Tạo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   // Cân nhắc đóng UserControl hoặc Form nếu lỗi nghiêm trọng
                  // this.ParentForm?.Close(); // Ví dụ đóng Form cha
             } else {
                WireUpEvents(); // Gắn sự kiện nếu khởi tạo BUS thành công
             }
        }
        */

        // Phương thức gắn các sự kiện cho controls
        private void WireUpEvents()
        {
            this.Load += new System.EventHandler(this.ucQuanLyNhaXuatBan_Load);
            dgvNhaXuatBan.SelectionChanged += new System.EventHandler(this.dgvNhaXuatBan_SelectionChanged);
            btnThem.Click += new System.EventHandler(this.btnThem_Click);
            btnSua.Click += new System.EventHandler(this.btnSua_Click);
            btnXoa.Click += new System.EventHandler(this.btnXoa_Click);
            btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            btnHuy.Click += new System.EventHandler(this.btnHuy_Click);
            btnTimKiem.Click += new System.EventHandler(this.btnTimKiem_Click);
            txtTimKiem.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTimKiem_KeyDown);
        }


        // --- Sự kiện Load UserControl ---
        private void ucQuanLyNhaXuatBan_Load(object sender, EventArgs e)
        {
            if (_busNhaXuatBan == null && !this.DesignMode) // Chỉ kiểm tra khi không ở chế độ Design
            {
                MessageBox.Show("Lỗi: Lớp BUS Nhà Xuất Bản chưa được khởi tạo.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Ngăn không chạy tiếp nếu BUS là null
            }
            ConfigureDataGridView(); // Cấu hình cột trước khi tải dữ liệu
            LoadDataGrid();
            SetControlStates(isViewing: true); // Đặt trạng thái ban đầu
        }

        // --- Tải dữ liệu lên DataGridView ---
        private void LoadDataGrid(List<NhaXuatBanDTO>? dataSource = null)
        {
            // Luu lại dòng đang chọn nếu có
            int? selectedRowIndex = dgvNhaXuatBan.CurrentRow?.Index;

            dgvNhaXuatBan.DataSource = null; // Xóa dữ liệu cũ
            try
            {
                var listNXB = dataSource ?? _busNhaXuatBan?.GetAll(); // Lấy tất cả nếu không có datasource cụ thể
                dgvNhaXuatBan.DataSource = listNXB;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách nhà xuất bản: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvNhaXuatBan.DataSource = new List<NhaXuatBanDTO>(); // Hiển thị bảng trống nếu lỗi
            }
            finally
            {
                dgvNhaXuatBan.ClearSelection(); // Bỏ chọn dòng
                ClearInputFields(); // Xóa các ô nhập liệu
                _selectedNhaXuatBan = null; // Reset NXB đang chọn

                // Khôi phục lựa chọn dòng nếu có thể
                if (selectedRowIndex.HasValue && selectedRowIndex < dgvNhaXuatBan.Rows.Count)
                {
                    dgvNhaXuatBan.Rows[selectedRowIndex.Value].Selected = true;
                    // Đảm bảo cuộn đến dòng được chọn
                    if (dgvNhaXuatBan.FirstDisplayedScrollingRowIndex > selectedRowIndex.Value ||
                        dgvNhaXuatBan.FirstDisplayedScrollingRowIndex + dgvNhaXuatBan.DisplayedRowCount(false) <= selectedRowIndex.Value)
                    {
                        dgvNhaXuatBan.FirstDisplayedScrollingRowIndex = Math.Max(0, selectedRowIndex.Value);
                    }
                }
                else if (dgvNhaXuatBan.Rows.Count > 0)
                {
                    // Nếu không có dòng nào được chọn trước đó, không chọn dòng nào cả
                    // dgvNhaXuatBan.Rows[0].Selected = true; // Bỏ chọn dòng đầu tiên mặc định
                }
                else
                {
                    // Nếu không có dòng nào, đảm bảo ô input bị xóa và nút được cập nhật
                    SetControlStates(isViewing: true);
                }
            }
        }


        // --- Cấu hình hiển thị cho DataGridView ---
        private void ConfigureDataGridView()
        {
            dgvNhaXuatBan.AutoGenerateColumns = false; // Tắt tự động tạo cột

            // Xóa các cột hiện có (nếu có) để tránh trùng lặp khi gọi lại
            dgvNhaXuatBan.Columns.Clear();

            // Thêm các cột theo thứ tự mong muốn
            dgvNhaXuatBan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "Mã NXB", Name = "colId", ReadOnly = true, Width = 80 });
            dgvNhaXuatBan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenNXB", HeaderText = "Tên Nhà Xuất Bản", Name = "colTenNXB", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvNhaXuatBan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiaChi", HeaderText = "Địa Chỉ", Name = "colDiaChi", ReadOnly = true, Width = 200 });
            dgvNhaXuatBan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DienThoai", HeaderText = "Điện Thoại", Name = "colDienThoai", ReadOnly = true, Width = 100 });
            // Thêm cột Email (giả sử bạn đã thêm Email vào NhaXuatBanDTO)
            dgvNhaXuatBan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "Email", Name = "colEmail", ReadOnly = true, Width = 150 });

            // Các cài đặt khác
            dgvNhaXuatBan.AllowUserToAddRows = false;
            dgvNhaXuatBan.AllowUserToDeleteRows = false;
            dgvNhaXuatBan.MultiSelect = false;
            dgvNhaXuatBan.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //dgvNhaXuatBan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Có thể bỏ nếu đã set width và fill cho cột Tên
            dgvNhaXuatBan.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvNhaXuatBan.ReadOnly = true; // Chỉ cho phép đọc trên Grid
        }


        // --- Xóa trắng các ô nhập liệu ---
        private void ClearInputFields()
        {
            txtIdNXB.Clear();
            txtTenNXB.Clear();
            txtDiaChi.Clear();
            txtSdt.Clear();
            txtEmail.Clear(); // Xóa cả ô Email
            // Không xóa ô tìm kiếm ở đây txtTimKiem.Clear();
        }

        // --- Quản lý trạng thái Enabled/Visible của Controls ---
        private void SetControlStates(bool isViewing = false, bool isEditingOrAdding = false)
        {
            // === Trạng thái Panel Nhập liệu ===
            // Bật panel input khi đang thêm/sửa
            panelInput.Enabled = isEditingOrAdding;
            // ID luôn ở chế độ chỉ đọc, ngay cả khi panel được bật
            txtIdNXB.ReadOnly = true;

            // === Trạng thái Nút Chính ===
            btnThem.Enabled = !isEditingOrAdding; // Tắt khi đang thêm/sửa
                                                  // Bật nút Sửa và Xóa chỉ khi đang xem và có dòng được chọn
            btnSua.Enabled = isViewing && _selectedNhaXuatBan != null;
            btnXoa.Enabled = isViewing && _selectedNhaXuatBan != null;

            // === Trạng thái Nút Lưu/Hủy ===
            btnLuu.Visible = isEditingOrAdding;
            btnHuy.Visible = isEditingOrAdding;
            // Nút Lưu và Hủy chỉ bật khi đang trong chế độ thêm/sửa
            btnLuu.Enabled = isEditingOrAdding;
            btnHuy.Enabled = isEditingOrAdding;

            // === Trạng thái Panel Tìm kiếm ===
            // Tắt tìm kiếm khi đang thêm/sửa
            panelTimKiem.Enabled = !isEditingOrAdding;

            // === Trạng thái DataGridView ===
            // Tắt DataGridView khi đang thêm/sửa để tránh chọn dòng khác
            dgvNhaXuatBan.Enabled = !isEditingOrAdding;
        }


        // --- Sự kiện khi chọn dòng trên DataGridView ---
        private void dgvNhaXuatBan_SelectionChanged(object sender, EventArgs e)
        {
            // Chỉ xử lý nếu không đang trong chế độ thêm/sửa
            if (!btnLuu.Visible) // Kiểm tra trạng thái nút Lưu (ẩn khi đang xem)
            {
                if (dgvNhaXuatBan.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = dgvNhaXuatBan.SelectedRows[0];
                    _selectedNhaXuatBan = selectedRow.DataBoundItem as NhaXuatBanDTO;

                    if (_selectedNhaXuatBan != null)
                    {
                        // Hiển thị thông tin
                        txtIdNXB.Text = _selectedNhaXuatBan.Id.ToString();
                        txtTenNXB.Text = _selectedNhaXuatBan.TenNXB;
                        txtDiaChi.Text = _selectedNhaXuatBan.DiaChi;
                        txtSdt.Text = _selectedNhaXuatBan.DienThoai;
                        txtEmail.Text = _selectedNhaXuatBan.Email; // <-- Hiển thị Email
                        SetControlStates(isViewing: true); // Đặt trạng thái xem
                    }
                    else
                    {
                        // Nếu DataBoundItem không phải là NhaXuatBanDTO (trường hợp hiếm)
                        _selectedNhaXuatBan = null;
                        ClearInputFields();
                        SetControlStates(isViewing: true);
                    }
                }
                else
                {
                    // Nếu không có dòng nào được chọn (ví dụ sau khi xóa hết hoặc load lại grid trống)
                    _selectedNhaXuatBan = null;
                    ClearInputFields();
                    SetControlStates(isViewing: true); // Vẫn là trạng thái xem nhưng không có gì được chọn
                }
            }
        }

        // --- Nút Thêm ---
        private void btnThem_Click(object sender, EventArgs e)
        {
            _isAdding = true;
            _selectedNhaXuatBan = null; // Đảm bảo không có NXB nào đang được chọn để sửa
            dgvNhaXuatBan.ClearSelection(); // Bỏ chọn dòng trên grid
            ClearInputFields();
            SetControlStates(isEditingOrAdding: true);
            txtTenNXB.Focus();
        }

        // --- Nút Sửa ---
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (_selectedNhaXuatBan == null)
            {
                MessageBox.Show("Vui lòng chọn nhà xuất bản cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _isAdding = false;
            SetControlStates(isEditingOrAdding: true);
            txtTenNXB.Focus();
            // Đảm bảo dữ liệu trên textbox là mới nhất từ _selectedNhaXuatBan trước khi sửa
            dgvNhaXuatBan_SelectionChanged(dgvNhaXuatBan, EventArgs.Empty);
        }

        // --- Nút Xóa ---
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (_selectedNhaXuatBan == null)
            {
                MessageBox.Show("Vui lòng chọn nhà xuất bản cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa nhà xuất bản '{_selectedNhaXuatBan.TenNXB}' (ID: {_selectedNhaXuatBan.Id}) không?",
                                                   "Xác nhận xóa",
                                                   MessageBoxButtons.YesNo,
                                                   MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    string resultMessage = _busNhaXuatBan.DeleteNhaXuatBan(_selectedNhaXuatBan.Id);
                    MessageBoxIcon icon = resultMessage.Contains("thành công") ? MessageBoxIcon.Information : MessageBoxIcon.Warning; // Hoặc Error tùy theo mức độ lỗi
                    MessageBox.Show(resultMessage, "Thông báo", MessageBoxButtons.OK, icon);

                    if (resultMessage.Contains("thành công"))
                    {
                        LoadDataGrid(); // Tải lại sau khi xóa thành công
                        // Trạng thái nút sẽ tự cập nhật trong LoadDataGrid -> SetControlStates
                    }
                }
                catch (Exception ex)
                {
                    // Log lỗi chi tiết hơn nếu cần
                    // Logger.LogError(ex, "Lỗi khi xóa nhà xuất bản ID: {NxbId}", _selectedNhaXuatBan.Id);
                    MessageBox.Show($"Lỗi khi xóa nhà xuất bản: {ex.Message}\nXem chi tiết trong file log.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // --- Nút Lưu ---
        private void btnLuu_Click(object sender, EventArgs e)
        {
            // --- Validation ---

            // 1. Kiểm tra Tên NXB
            if (string.IsNullOrWhiteSpace(txtTenNXB.Text))
            {
                MessageBox.Show("Tên nhà xuất bản không được để trống.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNXB.Focus();
                return;
            }

            // 2. Kiểm tra định dạng Số điện thoại (cho phép rỗng)
            string dienThoai = txtSdt.Text.Trim();
            // Regex đơn giản cho SĐT Việt Nam (có thể điều chỉnh cho phù hợp hơn)
            // Cho phép số có 10-11 chữ số, có thể bắt đầu bằng 0 hoặc +84
            string phonePattern = @"^(|(\+84|0)\d{9,10})$";
            if (!string.IsNullOrEmpty(dienThoai) && !Regex.IsMatch(dienThoai, phonePattern))
            {
                MessageBox.Show("Định dạng số điện thoại không hợp lệ.\nVui lòng nhập số điện thoại Việt Nam hợp lệ (10-11 chữ số, ví dụ: 09xxxxxxxx hoặc +849xxxxxxxx).", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSdt.Focus();
                return;
            }

            // 3. Kiểm tra định dạng Email (cho phép rỗng)
            string email = txtEmail.Text.Trim();
            // Regex cơ bản kiểm tra định dạng email
            string emailPattern = @"^(|[^@\s]+@[^@\s]+\.[^@\s]+)$"; // Cho phép rỗng hoặc định dạng cơ bản user@domain.com
            // Sử dụng System.Net.Mail để kiểm tra chặt chẽ hơn (nhưng có thể throw exception)
            // try
            // {
            //     if (!string.IsNullOrEmpty(email))
            //     {
            //         var mailAddress = new System.Net.Mail.MailAddress(email);
            //     }
            // }
            // catch (FormatException)
            // {
            //      MessageBox.Show("Định dạng email không hợp lệ.", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //      txtEmail.Focus();
            //      return;
            // }
            if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, emailPattern))
            {
                MessageBox.Show("Định dạng email không hợp lệ.", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            // (Thêm các validation khác nếu cần, ví dụ: kiểm tra độ dài tối thiểu/tối đa nếu MaxLength chưa đủ)
            // Ví dụ: Kiểm tra độ dài tối thiểu tên NXB
            // if (txtTenNXB.Text.Trim().Length < 3)
            // {
            //     MessageBox.Show("Tên nhà xuất bản phải có ít nhất 3 ký tự.", "Thông tin không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //     txtTenNXB.Focus();
            //     return;
            // }

            // --- Tạo DTO ---
            NhaXuatBanDTO nxbDto = new NhaXuatBanDTO
            {
                // Id sẽ được gán bên dưới nếu là chế độ sửa
                TenNXB = txtTenNXB.Text.Trim(),
                DiaChi = string.IsNullOrWhiteSpace(txtDiaChi.Text) ? null : txtDiaChi.Text.Trim(),
                DienThoai = string.IsNullOrWhiteSpace(dienThoai) ? null : dienThoai, // Sử dụng biến đã trim
                Email = string.IsNullOrWhiteSpace(email) ? null : email // <-- Gán giá trị Email đã trim
            };

            string resultMessage = "Có lỗi xảy ra.";
            try
            {
                if (_isAdding) // Lưu Thêm mới
                {
                    resultMessage = _busNhaXuatBan.AddNhaXuatBan(nxbDto);
                }
                else // Lưu Cập nhật
                {
                    if (_selectedNhaXuatBan != null)
                    {
                        nxbDto.Id = _selectedNhaXuatBan.Id; // Gán ID cho DTO để BUS biết cần cập nhật NXB nào
                        resultMessage = _busNhaXuatBan.UpdateNhaXuatBan(nxbDto);
                    }
                    else
                    {
                        // Trường hợp này không nên xảy ra nếu logic đúng, nhưng vẫn kiểm tra
                        MessageBox.Show("Không xác định được nhà xuất bản cần cập nhật.", "Lỗi Logic", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        SetControlStates(isViewing: true); // Đặt lại trạng thái nếu lỗi
                        return;
                    }
                }

                // --- Xử lý kết quả ---
                MessageBoxIcon icon = resultMessage.Contains("thành công") ? MessageBoxIcon.Information : MessageBoxIcon.Warning;
                MessageBox.Show(resultMessage, "Thông báo", MessageBoxButtons.OK, icon);

                if (resultMessage.Contains("thành công"))
                {
                    int? currentSelectedId = _selectedNhaXuatBan?.Id; // Lưu ID trước khi load lại (nếu sửa)
                    int? addedNxbId = null; // Lưu ID của NXB mới thêm (nếu thêm)
                    bool wasAdding = _isAdding; // Lưu trạng thái trước khi reset

                    // Nếu thêm mới thành công, cố gắng lấy ID của NXB vừa thêm
                    if (wasAdding)
                    {
                        // Cách 1: Giả sử hàm Add trả về ID (cần sửa BUS)
                        // int.TryParse(resultMessage.Split(':').LastOrDefault()?.Trim(), out addedNxbId); // Ví dụ nếu BUS trả về "Thêm thành công: 123"

                        // Cách 2: Tìm lại NXB vừa thêm (kém chính xác hơn nếu trùng tên và không có ID trả về)
                        var addedNxb = _busNhaXuatBan.SearchByName(nxbDto.TenNXB)
                                                    .OrderByDescending(n => n.Id)
                                                    .FirstOrDefault();
                        if (addedNxb != null) addedNxbId = addedNxb.Id;
                    }

                    LoadDataGrid(); // Tải lại dữ liệu

                    // Tìm và chọn lại dòng vừa thêm/sửa
                    int? idToSelect = wasAdding ? addedNxbId : currentSelectedId;

                    if (idToSelect.HasValue)
                    {
                        foreach (DataGridViewRow row in dgvNhaXuatBan.Rows)
                        {
                            var nxbInGrid = row.DataBoundItem as NhaXuatBanDTO;
                            if (nxbInGrid != null && nxbInGrid.Id == idToSelect.Value)
                            {
                                dgvNhaXuatBan.ClearSelection();
                                row.Selected = true;
                                // Cuộn đến dòng được chọn nếu nó không hiển thị
                                if (dgvNhaXuatBan.FirstDisplayedScrollingRowIndex > row.Index ||
                                    dgvNhaXuatBan.FirstDisplayedScrollingRowIndex + dgvNhaXuatBan.DisplayedRowCount(false) <= row.Index)
                                {
                                    dgvNhaXuatBan.FirstDisplayedScrollingRowIndex = Math.Max(0, row.Index);
                                }
                                _selectedNhaXuatBan = nxbInGrid; // Cập nhật lại _selectedNhaXuatBan
                                break;
                            }
                        }
                    }

                    _isAdding = false; // Reset cờ sau khi thành công
                    SetControlStates(isViewing: true); // Chuyển về trạng thái xem
                }
                // Nếu không thành công (ví dụ: trùng tên, lỗi DB), giữ nguyên trạng thái thêm/sửa
                // để người dùng có thể sửa lại.
                // else {
                //     SetControlStates(isEditingOrAdding: true);
                // }
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                // Logger.LogError(ex, "Lỗi khi lưu nhà xuất bản. IsAdding: {IsAdding}, DTO: {@NxbDto}", _isAdding, nxbDto);
                MessageBox.Show($"Lỗi nghiêm trọng khi lưu nhà xuất bản: {ex.Message}\nXem chi tiết trong file log.", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetControlStates(isViewing: true); // Đặt lại trạng thái nếu có lỗi ngoại lệ
            }
        }

        // --- Nút Hủy ---
        private void btnHuy_Click(object sender, EventArgs e)
        {
            _isAdding = false; // Luôn tắt cờ thêm/sửa khi hủy

            // Load lại dữ liệu của dòng đang chọn (nếu có) hoặc xóa trắng
            if (_selectedNhaXuatBan != null)
            {
                // Hiển thị lại thông tin của dòng đang chọn
                txtIdNXB.Text = _selectedNhaXuatBan.Id.ToString();
                txtTenNXB.Text = _selectedNhaXuatBan.TenNXB;
                txtDiaChi.Text = _selectedNhaXuatBan.DiaChi;
                txtSdt.Text = _selectedNhaXuatBan.DienThoai;
                txtEmail.Text = _selectedNhaXuatBan.Email;
            }
            else
            {
                ClearInputFields(); // Xóa trắng nếu không có dòng nào được chọn trước đó
            }
            SetControlStates(isViewing: true); // Quay về trạng thái Xem
        }


        // --- Nút Tìm kiếm ---
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string searchTerm = txtTimKiem.Text.Trim();

            // Không cần kiểm tra IsNullOrEmpty vì nếu trống, LoadDataGrid() sẽ được gọi
            // if (string.IsNullOrEmpty(searchTerm))
            // {
            //     LoadDataGrid(); // Tải lại tất cả nếu ô tìm kiếm trống
            //     return;
            // }

            try
            {
                List<NhaXuatBanDTO>? searchResult = null;
                if (string.IsNullOrEmpty(searchTerm))
                {
                    searchResult = _busNhaXuatBan.GetAll(); // Lấy tất cả nếu tìm kiếm trống
                }
                else
                {
                    searchResult = _busNhaXuatBan.SearchByName(searchTerm); // Tìm theo tên
                }


                if (searchResult != null && searchResult.Any())
                {
                    LoadDataGrid(searchResult); // Hiển thị kết quả
                }
                else
                {
                    // Chỉ thông báo nếu thực sự có nhập từ khóa tìm kiếm
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        MessageBox.Show($"Không tìm thấy nhà xuất bản nào có tên chứa '{searchTerm}'.", "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    LoadDataGrid(new List<NhaXuatBanDTO>()); // Hiển thị bảng trống
                }
            }
            catch (Exception ex)
            {
                // Log lỗi
                // Logger.LogError(ex, "Lỗi khi tìm kiếm nhà xuất bản với từ khóa: {SearchTerm}", searchTerm);
                MessageBox.Show($"Lỗi khi tìm kiếm nhà xuất bản: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadDataGrid(); // Tải lại danh sách đầy đủ nếu có lỗi
            }
            // SetControlStates(isViewing: true); // Trạng thái xem sẽ được đặt trong LoadDataGrid
        }

        // --- Tìm kiếm khi nhấn Enter trong ô tìm kiếm ---
        private void txtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnTimKiem_Click(sender, EventArgs.Empty); // Gọi sự kiện click của nút Tìm kiếm
                e.SuppressKeyPress = true; // Ngăn không phát ra tiếng 'beep' khi nhấn Enter
            }
        }

    }
}
