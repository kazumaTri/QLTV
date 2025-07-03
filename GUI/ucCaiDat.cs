// Project/Namespace: GUI

// --- USING DIRECTIVES ---
// Chỉ giữ lại các using cần thiết
using BUS; // Cần cho IBUSThamSo
using DTO; // Cần cho ThamSoDTO
using MaterialSkin.Controls; // Cần cho MaterialMessageBox, MaterialTextBox2, MaterialSkin controls khác
using System; // Cần cho Exception, ArgumentNullException, InvalidOperationException, EventArgs, int, decimal, bool
using System.Threading.Tasks; // Cần cho async/await Task // <<< THÊM Using này nếu chưa có
using System.Windows.Forms; // Cần cho UserControl, Panel, MessageBoxButtons, MessageBoxIcon, Cursor
// Không cần using Microsoft.Extensions.DependencyInjection vì IServiceProvider đã được loại bỏ
// Không cần using GUI; vì ucCaiDat nằm trong namespace GUI và không refer đến các forms/ucs khác trong cùng namespace ở đây
// using System.Collections.Generic; // Giữ lại nếu thực sự cần list/collection nào đó
// using System.Linq; // Giữ lại nếu thực sự cần LINQ

namespace GUI // Namespace của project GUI của bạn
{
    /// <summary>
    /// UserControl quản lý thông tin Tham số hệ thống (Cài đặt).
    /// Cho phép xem và cập nhật các tham số cấu hình của thư viện.
    /// Sử dụng Dependency Injection và MaterialSkin.2.
    /// </summary>
    public partial class ucCaiDat : UserControl // Kế thừa từ UserControl chuẩn
    {
        // --- DEPENDENCIES (Nhận qua Constructor Injection) ---
        // ucCaiDat cần tương tác với tầng BUS cho Tham số hệ thống
        private readonly IBUSThamSo _busThamSo;
        // Không cần IServiceProvider ở đây vì UserControl không tự tạo Forms/UC con thông qua DI.
        // Việc mở Forms/UC con sẽ được xử lý ở tầng chứa (ví dụ: Form cha) thông qua sự kiện.

        // --- STATE ---
        // Lưu trữ bộ tham số gốc được load từ DB để kiểm tra sự thay đổi
        private ThamSoDTO? _currentThamSo;

        // --- EVENTS ---
        // Định nghĩa sự kiện để thông báo cho Form/Container cha khi UserControl này muốn đóng/thay thế.
        // Form/Container cha sẽ lắng nghe sự kiện này và thực hiện hành động tương ứng (ví dụ: chuyển về Dashboard).
        public event EventHandler? RequestClose;

        // --- CONSTRUCTOR (Sử dụng Dependency Injection) ---
        // Constructor chỉ nhận IBUSThamSo là dependency cần thiết
        public ucCaiDat(IBUSThamSo busThamSo) // Nhận dependencies qua DI
        {
            // InitializeComponent() được gọi đầu tiên để tạo và cấu hình các controls UI
            InitializeComponent();

            // Gán dependencies cho các readonly fields và kiểm tra null
            _busThamSo = busThamSo ?? throw new ArgumentNullException(nameof(busThamSo));

            // Việc gắn sự kiện TextChanged cho các ô nhập liệu
            // NÊN được thực hiện trong file .Designer.cs bởi Visual Studio Designer
            // hoặc trong InitializeComponent().
            // Chỉ cần đảm bảo phương thức InputField_TextChanged tồn tại ở đây.
            // Ví dụ trong Designer.cs:
            // this.txtTuoiToiThieu.TextChanged += new System.EventHandler(this.InputField_TextChanged);
            // this.txtTuoiToiDa.TextChanged += new System.EventHandler(this.InputField_TextChanged);
            // ... và các controls nhập liệu khác.
            // Nếu dùng CheckBox cho AdQdkttienThu:
            // this.chkAdQdkttienThu.CheckedChanged += new System.EventHandler(this.InputField_TextChanged); // Hoặc một tên khác CheckBox_CheckedChanged
        }

        // --- SỰ KIỆN LOAD USERCONTROL ---
        // Xảy ra khi UserControl được load và hiển thị
        private async void ucCaiDat_Load(object sender, EventArgs e)
        {
            // Không chạy logic load data trong Design Mode của Visual Studio
            if (!this.DesignMode)
            {
                // Không gọi InitializeDataAsync ở đây nữa, frmMain sẽ gọi
                // await InitializeDataAsync();
            }
        }

        // *** THÊM PHƯƠNG THỨC NÀY VÀO ***
        /// <summary>
        /// Phương thức công khai để khởi tạo dữ liệu cho UserControl khi được load bởi frmMain.
        /// </summary>
        public async Task InitializeDataAsync()
        {
            // Tải dữ liệu tham số khi UserControl được load
            await LoadParametersAsync();
            // Trạng thái nút Lưu/Bỏ qua ban đầu sẽ là disabled vì chưa có thay đổi
            // SetControlState sẽ được gọi từ finally block của LoadParametersAsync
        }


        // --- HÀM TẢI DỮ LIỆU & HIỂN THỊ ---

        /// <summary>
        /// Tải bộ tham số hệ thống từ tầng BUS và hiển thị lên các controls UI.
        /// </summary>
        private async Task LoadParametersAsync()
        {
            // Hiển thị con trỏ chờ trong khi tải dữ liệu
            this.Cursor = Cursors.WaitCursor;
            bool loadSuccess = false; // Cờ để biết load thành công hay không

            try
            {
                // Gọi tầng BUS để lấy bộ tham số
                _currentThamSo = await _busThamSo.GetThamSoAsync();

                // Hiển thị dữ liệu lên các controls nếu tìm thấy tham số
                if (_currentThamSo != null)
                {
                    PopulateControlsFromDTO(_currentThamSo);
                    loadSuccess = true;
                }
                else
                {
                    // Trường hợp không tìm thấy tham số (lỗi cấu hình DB hoặc dữ liệu trống)
                    ClearInputFields(); // Xóa trắng các ô
                    MaterialMessageBox.Show(this.FindForm(), "Không tìm thấy bộ tham số hệ thống trong cơ sở dữ liệu. Vui lòng kiểm tra cấu hình hoặc thêm bộ tham số ban đầu.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Vô hiệu hóa tất cả controls nhập liệu và nút Lưu/Bỏ qua vì không có gì để sửa
                    // SetControlState(false, true); // Sẽ gọi ở finally
                }
            }
            catch (Exception ex) // Bắt các Exception có thể xảy ra từ tầng BUS/DAL
            {
                // Hiển thị thông báo lỗi cho người dùng
                MaterialMessageBox.Show(this.FindForm(), $"Lỗi khi tải tham số hệ thống: {ex.Message}", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Ghi chi tiết lỗi vào output debug
                System.Diagnostics.Debug.WriteLine($"ERROR (ucCaiDat - LoadParametersAsync): {ex.ToString()}");
                ClearInputFields(); // Xóa trắng ô nếu có lỗi tải
                // SetControlState(false, true); // Sẽ gọi ở finally
            }
            finally
            {
                // Luôn khôi phục con trỏ chuột
                this.Cursor = Cursors.Default;
                // Cập nhật trạng thái controls sau khi tải dữ liệu
                // Nếu load thành công (_currentThamSo != null), trạng thái ban đầu là false (không có thay đổi)
                // Nếu load thất bại (_currentThamSo == null), disable hết
                SetControlState(_currentThamSo != null && HasUnsavedChanges(), _currentThamSo == null); // Ban đầu sau load thành công, HasUnsavedChanges là false
            }
        }

        /// <summary>
        /// Đổ dữ liệu từ ThamSoDTO vào các controls nhập liệu.
        /// Kiểm tra null cho controls trước khi truy cập.
        /// </summary>
        /// <param name="dto">Đối tượng ThamSoDTO.</param>
        private void PopulateControlsFromDTO(ThamSoDTO dto)
        {
            // Đảm bảo controls không null trước khi gán giá trị
            if (txtId != null) txtId.Text = dto.Id.ToString(); // ID chỉ hiển thị, không sửa
            if (txtTuoiToiThieu != null) txtTuoiToiThieu.Text = dto.TuoiToiThieu.ToString();
            if (txtTuoiToiDa != null) txtTuoiToiDa.Text = dto.TuoiToiDa.ToString();
            if (txtThoiHanThe != null) txtThoiHanThe.Text = dto.ThoiHanThe.ToString();
            if (txtKhoangCachXuatBan != null) txtKhoangCachXuatBan.Text = dto.KhoangCachXuatBan.ToString();
            if (txtSoSachMuonToiDa != null) txtSoSachMuonToiDa.Text = dto.SoSachMuonToiDa.ToString();
            if (txtSoNgayMuonToiDa != null) txtSoNgayMuonToiDa.Text = dto.SoNgayMuonToiDa.ToString();
            if (txtDonGiaPhat != null) txtDonGiaPhat.Text = dto.DonGiaPhat.ToString();
            // Nếu txtAdQdkttienThu là TextBox
            if (txtAdQdkttienThu != null) txtAdQdkttienThu.Text = dto.AdQdkttienThu.ToString();
            // Nếu dùng CheckBox cho AdQdkttienThu
            // if (chkAdQdkttienThu != null) chkAdQdkttienThu.Checked = dto.AdQdkttienThu == 1;
        }

        // --- HÀM TIỆN ÍCH TRẠNG THÁI & GIAO DIỆN ---

        /// <summary>
        /// Xóa trắng nội dung của các ô nhập liệu.
        /// Kiểm tra null cho controls trước khi truy cập.
        /// </summary>
        private void ClearInputFields()
        {
            if (txtId != null) txtId.Clear();
            if (txtTuoiToiThieu != null) txtTuoiToiThieu.Clear();
            if (txtTuoiToiDa != null) txtTuoiToiDa.Clear();
            if (txtThoiHanThe != null) txtThoiHanThe.Clear();
            if (txtKhoangCachXuatBan != null) txtKhoangCachXuatBan.Clear();
            if (txtSoSachMuonToiDa != null) txtSoSachMuonToiDa.Clear();
            if (txtSoNgayMuonToiDa != null) txtSoNgayMuonToiDa.Clear();
            if (txtDonGiaPhat != null) txtDonGiaPhat.Clear();
            if (txtAdQdkttienThu != null) txtAdQdkttienThu.Clear();
            // Nếu dùng CheckBox
            // if (chkAdQdkttienThu != null) chkAdQdkttienThu.Checked = false;
        }


        /// <summary>
        /// Cập nhật trạng thái (Enabled/Disabled) của các controls UI.
        /// </summary>
        /// <param name="isEditing">True nếu có sự thay đổi chưa lưu trên các ô nhập liệu.</param>
        /// <param name="disableAll">True nếu muốn vô hiệu hóa tất cả controls nhập liệu và nút Lưu/Bỏ qua (ví dụ: khi không tải được tham số).</param>
        private void SetControlState(bool isEditing, bool disableAll = false)
        {
            // Kiểm tra null cho controls trước khi truy cập Enabled
            if (btnLuu != null) btnLuu.Enabled = isEditing && !disableAll;
            if (btnBoQua != null) btnBoQua.Enabled = isEditing && !disableAll;
            if (btnThoat != null) btnThoat.Enabled = !disableAll; // Nút Thoát luôn bật trừ khi disableAll

            // Các controls nhập liệu chỉ cho phép nhập khi KHÔNG bị disableAll
            if (txtId != null) txtId.Enabled = false; // ID luôn disable
            if (txtTuoiToiThieu != null) txtTuoiToiThieu.Enabled = !disableAll;
            if (txtTuoiToiDa != null) txtTuoiToiDa.Enabled = !disableAll;
            if (txtThoiHanThe != null) txtThoiHanThe.Enabled = !disableAll;
            if (txtKhoangCachXuatBan != null) txtKhoangCachXuatBan.Enabled = !disableAll;
            if (txtSoSachMuonToiDa != null) txtSoSachMuonToiDa.Enabled = !disableAll;
            if (txtSoNgayMuonToiDa != null) txtSoNgayMuonToiDa.Enabled = !disableAll;
            if (txtDonGiaPhat != null) txtDonGiaPhat.Enabled = !disableAll;
            if (txtAdQdkttienThu != null) txtAdQdkttienThu.Enabled = !disableAll; // Nếu là TextBox
            // Nếu dùng CheckBox
            // if (chkAdQdkttienThu != null) chkAdQdkttienThu.Enabled = !disableAll;

            // Panel chứa controls nhập liệu (tùy chọn)
            // if (panelDetails != null) panelDetails.Enabled = !disableAll;
        }


        /// <summary>
        /// Kiểm tra xem có bất kỳ thay đổi nào trên các ô nhập liệu so với dữ liệu gốc (_currentThamSo) hay không.
        /// Returns True nếu có thay đổi, False nếu không hoặc dữ liệu gốc/controls trống/parsing lỗi.
        /// </summary>
        private bool HasUnsavedChanges()
        {
            // Nếu chưa load được tham số gốc thì không có gì để so sánh
            if (_currentThamSo == null)
            {
                return false;
            }

            // Cố gắng parse giá trị hiện tại từ controls
            if (!int.TryParse(txtTuoiToiThieu?.Text.Trim(), out int tuoiToiThieu)) return false; // Parsing lỗi -> không coi là có thay đổi hợp lệ
            if (!int.TryParse(txtTuoiToiDa?.Text.Trim(), out int tuoiToiDa)) return false;
            if (!int.TryParse(txtThoiHanThe?.Text.Trim(), out int thoiHanThe)) return false;
            if (!int.TryParse(txtKhoangCachXuatBan?.Text.Trim(), out int khoangCachXuatBan)) return false;
            if (!int.TryParse(txtSoSachMuonToiDa?.Text.Trim(), out int soSachMuonToiDa)) return false;
            if (!int.TryParse(txtSoNgayMuonToiDa?.Text.Trim(), out int soNgayMuonToiDa)) return false;
            if (!int.TryParse(txtDonGiaPhat?.Text.Trim(), out int donGiaPhat)) return false;
            // Nếu txtAdQdkttienThu là TextBox
            if (!int.TryParse(txtAdQdkttienThu?.Text.Trim(), out int adQdkttienThu)) return false;
            // Nếu dùng CheckBox
            // int adQdkttienThu = chkAdQdkttienThu?.Checked == true ? 1 : 0;


            // So sánh giá trị đã parse với giá trị gốc
            if (tuoiToiThieu != _currentThamSo.TuoiToiThieu) return true;
            if (tuoiToiDa != _currentThamSo.TuoiToiDa) return true;
            if (thoiHanThe != _currentThamSo.ThoiHanThe) return true;
            if (khoangCachXuatBan != _currentThamSo.KhoangCachXuatBan) return true;
            if (soSachMuonToiDa != _currentThamSo.SoSachMuonToiDa) return true;
            if (soNgayMuonToiDa != _currentThamSo.SoNgayMuonToiDa) return true;
            if (donGiaPhat != _currentThamSo.DonGiaPhat) return true;
            if (adQdkttienThu != _currentThamSo.AdQdkttienThu) return true;

            // Nếu không có trường nào khác biệt
            return false;
        }

        // --- HÀM LẤY DỮ LIỆU TỪ CONTROLS VÀ PARSING ---

        /// <summary>
        /// Lấy dữ liệu từ các controls nhập liệu và tạo ra một đối tượng ThamSoDTO.
        /// Bao gồm parsing và validation định dạng cơ bản tại UI.
        /// </summary>
        /// <returns>Đối tượng ThamSoDTO được tạo từ dữ liệu nhập.</returns>
        /// <exception cref="ArgumentException">Ném ra nếu dữ liệu nhập không hợp lệ (định dạng sai, giá trị âm không cho phép, hoặc vi phạm quy tắc UI validation như Tuổi min > Tuổi max).</exception>
        /// <exception cref="InvalidOperationException">Ném ra nếu không xác định được ID tham số gốc (ví dụ: _currentThamSo là null).</exception>
        private ThamSoDTO GetDTOFromControls()
        {
            // ID: Lấy từ _currentThamSo (đã load từ DB) để đảm bảo đúng ID cần cập nhật
            if (_currentThamSo == null || _currentThamSo.Id <= 0)
            {
                // Trường hợp này xảy ra nếu cố gắng lưu trước khi load được tham số,
                // hoặc load thất bại và _currentThamSo là null/ID không hợp lệ.
                throw new InvalidOperationException("Không thể lưu. Thông tin tham số gốc chưa được tải hoặc bị lỗi.");
            }
            int id = _currentThamSo.Id;


            // Parsing và validation các trường số nguyên (int)
            int tuoiToiThieu, tuoiToiDa, thoiHanThe, khoangCachXuatBan, soSachMuonToiDa, soNgayMuonToiDa, donGiaPhat, adQdkttienThu;

            // Sử dụng int.TryParse và kiểm tra kết quả cùng với giá trị hợp lệ
            if (txtTuoiToiThieu == null || !int.TryParse(txtTuoiToiThieu.Text.Trim(), out tuoiToiThieu) || tuoiToiThieu < 0)
                throw new ArgumentException("Tuổi tối thiểu không hợp lệ. Vui lòng nhập số nguyên không âm.", nameof(txtTuoiToiThieu)); // Sử dụng tên control cho paramName
            if (txtTuoiToiDa == null || !int.TryParse(txtTuoiToiDa.Text.Trim(), out tuoiToiDa) || tuoiToiDa < 0)
                throw new ArgumentException("Tuổi tối đa không hợp lệ. Vui lòng nhập số nguyên không âm.", nameof(txtTuoiToiDa)); // Sử dụng tên control cho paramName

            // Thêm validation nghiệp vụ cơ bản tại UI: Tuổi tối thiểu <= Tuổi tối đa
            if (tuoiToiThieu > tuoiToiDa)
            {
                // Ném ArgumentException để UI có thể bắt và focus vào control liên quan (hoặc đơn giản chỉ hiển thị lỗi)
                throw new ArgumentException("Tuổi tối thiểu không được lớn hơn tuổi tối đa.", nameof(txtTuoiToiThieu)); // Chỉ định control đầu tiên gây lỗi
            }

            if (txtThoiHanThe == null || !int.TryParse(txtThoiHanThe.Text.Trim(), out thoiHanThe) || thoiHanThe <= 0)
                throw new ArgumentException("Thời hạn thẻ không hợp lệ. Vui lòng nhập số nguyên dương.", nameof(txtThoiHanThe));
            if (txtKhoangCachXuatBan == null || !int.TryParse(txtKhoangCachXuatBan.Text.Trim(), out khoangCachXuatBan) || khoangCachXuatBan < 0) // Có thể cho phép = 0
                throw new ArgumentException("Khoảng cách xuất bản không hợp lệ. Vui lòng nhập số nguyên không âm.", nameof(txtKhoangCachXuatBan));
            if (txtSoSachMuonToiDa == null || !int.TryParse(txtSoSachMuonToiDa.Text.Trim(), out soSachMuonToiDa) || soSachMuonToiDa <= 0)
                throw new ArgumentException("Số sách mượn tối đa không hợp lệ. Vui lòng nhập số nguyên dương.", nameof(txtSoSachMuonToiDa));
            if (txtSoNgayMuonToiDa == null || !int.TryParse(txtSoNgayMuonToiDa.Text.Trim(), out soNgayMuonToiDa) || soNgayMuonToiDa <= 0)
                throw new ArgumentException("Số ngày mượn tối đa không hợp lệ. Vui lòng nhập số nguyên dương.", nameof(txtSoNgayMuonToiDa));
            if (txtDonGiaPhat == null || !int.TryParse(txtDonGiaPhat.Text.Trim(), out donGiaPhat) || donGiaPhat < 0)
                throw new ArgumentException("Đơn giá phạt không hợp lệ. Vui lòng nhập số nguyên không âm.", nameof(txtDonGiaPhat));

            // Nếu txtAdQdkttienThu là TextBox
            if (txtAdQdkttienThu == null || !int.TryParse(txtAdQdkttienThu.Text.Trim(), out adQdkttienThu) || (adQdkttienThu != 0 && adQdkttienThu != 1))
                throw new ArgumentException("Quy định tiền thu không hợp lệ (chỉ nhập 0 hoặc 1).", nameof(txtAdQdkttienThu));
            // Nếu dùng CheckBox
            // adQdkttienThu = chkAdQdkttienThu != null && chkAdQdkttienThu.Checked ? 1 : 0;


            // Tạo đối tượng DTO
            return new ThamSoDTO
            {
                Id = id, // ID từ dữ liệu gốc (_currentThamSo)
                TuoiToiThieu = tuoiToiThieu,
                TuoiToiDa = tuoiToiDa,
                ThoiHanThe = thoiHanThe,
                KhoangCachXuatBan = khoangCachXuatBan,
                SoSachMuonToiDa = soSachMuonToiDa,
                SoNgayMuonToiDa = soNgayMuonToiDa,
                DonGiaPhat = donGiaPhat,
                AdQdkttienThu = adQdkttienThu
            };
        }

        /// <summary>
        /// Tìm và focus vào control dựa trên tên ParameterName của ArgumentException.
        /// </summary>
        /// <param name="paramName">Tên control cần focus.</param>
        private void FocusControlByName(string? paramName)
        {
            if (string.IsNullOrEmpty(paramName)) return;

            // Sử dụng Controls.Find để tìm control theo tên
            Control[] foundControls = this.Controls.Find(paramName, true); // true để tìm cả controls con
            if (foundControls.Length > 0 && foundControls[0] is Control ctrl && ctrl.CanFocus)
            {
                ctrl.Focus();
            }
        }


        // --- SỰ KIỆN CỦA CÁC BUTTON ---

        // Xử lý sự kiện click nút "Lưu" (chỉ cho cập nhật)
        private async void btnLuu_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có thực sự có thay đổi chưa lưu không trước khi gọi BUS
            if (!HasUnsavedChanges())
            {
                MaterialMessageBox.Show(this.FindForm(), "Không có thay đổi nào để lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetControlState(false); // Vẫn giữ nguyên trạng thái nút disable
                return; // Không làm gì nếu không có thay đổi
            }

            // Kiểm tra _currentThamSo có dữ liệu gốc không trước khi cố gắng lưu
            if (_currentThamSo == null)
            {
                MaterialMessageBox.Show(this.FindForm(), "Không thể lưu. Thông tin tham số gốc chưa được tải hoặc bị lỗi.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetControlState(false, true); // Disable hết
                return;
            }

            // Lấy dữ liệu từ controls và thực hiện UI Validation/Parsing
            ThamSoDTO? thamSoDtoToUpdate = null;

            try
            {
                // Lấy dữ liệu và validate định dạng tại UI
                thamSoDtoToUpdate = GetDTOFromControls();
            }
            catch (ArgumentException argEx) // Bắt lỗi validation định dạng/nghiệp vụ cơ bản từ GetDTOFromControls
            {
                MaterialMessageBox.Show(this.FindForm(), $"Lỗi dữ liệu nhập: {argEx.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"UI Validation Error: {argEx.ToString()}");
                // Focus vào control gây lỗi nếu ArgumentException cung cấp ParamName
                FocusControlByName(argEx.ParamName);
                // Giữ nguyên trạng thái sửa để người dùng sửa lại
                SetControlState(true);
                return; // Ngừng xử lý nếu dữ liệu không hợp lệ
            }
            catch (InvalidOperationException invOpEx) // Bắt lỗi nghiêm trọng từ GetDTOFromControls (ví dụ: ID gốc không xác định)
            {
                MaterialMessageBox.Show(this.FindForm(), $"Lỗi nội bộ: {invOpEx.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Internal UI Error (GetDTOFromControls): {invOpEx.ToString()}");
                SetControlState(false, true); // Disable hết nếu lỗi nghiêm trọng
                return;
            }
            catch (Exception ex) // Bắt lỗi không mong muốn khi lấy DTO
            {
                MaterialMessageBox.Show(this.FindForm(), $"Lỗi không mong muốn khi xử lý dữ liệu nhập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Unexpected error in GetDTOFromControls: {ex.ToString()}");
                SetControlState(false, true);
                return;
            }

            // Nếu lấy DTO thành công và không có lỗi UI Validation
            bool success = false;
            string? errorMsg = null;

            // Vô hiệu hóa nút Lưu và thay đổi con trỏ chuột trong khi xử lý
            if (btnLuu != null) btnLuu.Enabled = false; // Disable chỉ nút Lưu
            this.Cursor = Cursors.WaitCursor;

            try
            {
                // Gọi tầng BUS để cập nhật bộ tham số
                // BUS sẽ thực hiện validation nghiệp vụ sâu hơn và gọi DAL
                success = await _busThamSo.UpdateThamSoAsync(thamSoDtoToUpdate!); // Sử dụng null-forgiving vì đã kiểm tra thamSoDtoToUpdate != null ở trên

            }
            catch (ArgumentException argEx) // Bắt lỗi validation từ BUS (ví dụ: Tuổi min > Tuổi max nếu chưa validate ở UI, hoặc các luật khác)
            {
                // Lưu ý: Nếu UI đã validate Tuổi min > Tuổi max, lỗi này từ BUS có thể chỉ ra vấn đề
                // hoặc BUS có các luật phức tạp hơn mà UI chưa check.
                errorMsg = $"Lỗi dữ liệu: {argEx.Message}"; // Thông báo lỗi nghiệp vụ thân thiện từ BUS
                System.Diagnostics.Debug.WriteLine($"BUS Validation Error: {argEx.ToString()}");
                // Không focus control dựa trên argEx.ParamName của BUS, vì tên parameter trong BUS/DAL khác tên control UI.
            }
            catch (InvalidOperationException invOpEx) // Bắt lỗi nghiệp vụ từ BUS/DAL (ví dụ: không tìm thấy bộ tham số ID đó, lỗi concurrency...)
            {
                errorMsg = $"Lỗi nghiệp vụ: {invOpEx.Message}";
                System.Diagnostics.Debug.WriteLine($"BUS/DAL Business Logic Error: {invOpEx.ToString()}");
            }
            catch (Exception ex) // Bắt các loại lỗi khác (hệ thống, lỗi từ DAL,...)
            {
                errorMsg = $"Lỗi hệ thống khi lưu tham số: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"ERROR (btnLuu_Click - Update): {ex.ToString()}");
                // CS0169: Nếu bạn chỉ ghi ex.Message vào debug mà không dùng biến 'ex' trực tiếp hoặc ex.ToString() ở đâu khác,
                // compiler có thể cảnh báo CS0169. Sử dụng ex.ToString() trong debug message là một cách dùng hợp lệ.
            }
            finally
            {
                // Khôi phục con trỏ chuột
                this.Cursor = Cursors.Default;
                // Bật lại nút Lưu chỉ khi quá trình THẤT BẠI và không bị disableAll
                // Nếu thành công, SetControlState(false) sẽ được gọi bởi LoadParametersAsync
                if (!success && _currentThamSo != null) // Kiểm tra _currentThamSo != null để không bật nút nếu không có tham số gốc
                {
                    SetControlState(true); // Vẫn ở trạng thái sửa nếu lưu thất bại
                }
            }

            // *** Xử lý kết quả sau khi gọi BUS ***
            if (success)
            {
                // Thông báo thành công
                MaterialMessageBox.Show(this.FindForm(), "Cập nhật tham số thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Tải lại dữ liệu tham số gốc từ DB để xác nhận và cập nhật _currentThamSo
                // Điều này cũng tự động gọi PopulateControlsFromDTO và SetControlState(false) ở finally block của LoadParametersAsync
                await LoadParametersAsync();

            }
            else if (!string.IsNullOrEmpty(errorMsg)) // Nếu có lỗi đã được bắt và lưu trong errorMsg
            {
                // Hiển thị thông báo lỗi chi tiết
                MaterialMessageBox.Show(this.FindForm(), errorMsg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Giữ nguyên trạng thái sửa (SetControlState(true) đã gọi ở finally)
            }
            else // Trường hợp success = false nhưng không có errorMsg (ít xảy ra nếu bắt lỗi đầy đủ)
            {
                // Thông báo lỗi chung
                MaterialMessageBox.Show(this.FindForm(), "Cập nhật tham số thất bại (lý do không xác định).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Giữ nguyên trạng thái sửa (SetControlState(true) đã gọi ở finally)
            }
        }

        // Xử lý sự kiện click nút "Bỏ qua"
        private async void btnBoQua_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có thực sự có thay đổi chưa lưu không
            if (!HasUnsavedChanges())
            {
                // Nếu không có thay đổi, chỉ cần reset trạng thái nút
                SetControlState(false);
                return;
            }

            // Hỏi xác nhận người dùng nếu có thay đổi chưa lưu
            DialogResult confirm = MaterialMessageBox.Show(this.FindForm(), "Bạn có muốn hủy bỏ các thay đổi chưa lưu?", "Xác nhận Bỏ qua", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes) // Nếu người dùng xác nhận hủy bỏ
            {
                // Tải lại dữ liệu tham số gốc từ DB để reset các ô nhập liệu
                await LoadParametersAsync(); // Hàm này sẽ tự gọi PopulateControlsFromDTO và SetControlState(false)
            }
            // Nếu người dùng không xác nhận, giữ nguyên trạng thái sửa
        }

        // Xử lý sự kiện click nút "Thoát" (hoặc đóng UserControl)
        private void btnThoat_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có thay đổi chưa lưu trước khi thoát
            if (HasUnsavedChanges())
            {
                DialogResult confirm = MaterialMessageBox.Show(this.FindForm(), "Bạn có thay đổi chưa lưu. Bạn có chắc chắn muốn thoát?", "Xác nhận Thoát", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning); // Dùng OKCancel
                if (confirm == DialogResult.Cancel) // Nếu người dùng chọn Cancel
                {
                    return; // Ngừng nếu người dùng không xác nhận thoát khi có thay đổi
                }
            }

            // Bắn sự kiện RequestClose để Form/Container cha biết UserControl này muốn đóng.
            RequestClose?.Invoke(this, EventArgs.Empty);
        }


        // --- SỰ KIỆN TEXTCHANGED CỦA CÁC Ô NHẬP LIỆU ---
        // Đây là định nghĩa DUY NHẤT cho sự kiện TextChanged chung.
        // Việc gán sự kiện này cho các controls cụ thể (+=) phải được làm trong InitializeComponent()
        // hoặc file .Designer.cs, KHÔNG PHẢI định nghĩa đầy đủ phương thức ở đó.

        /// <summary>
        /// Phương thức xử lý sự kiện TextChanged chung cho các ô nhập liệu cần theo dõi sự thay đổi.
        /// Cập nhật trạng thái nút Lưu/Bỏ qua dựa trên HasUnsavedChanges().
        /// </summary>
        private void InputField_TextChanged(object sender, EventArgs e)
        {
            // Khi bất kỳ ô nhập liệu nào thay đổi text, kiểm tra xem có thay đổi so với gốc không.
            // Nếu có thay đổi, bật nút Lưu và Bỏ qua.
            // Chỉ cập nhật trạng thái nếu UserControl đã được tải và _currentThamSo không null
            // (tránh lỗi khi TextChanged xảy ra trong quá trình InitializeComponent hoặc Load)
            if (_currentThamSo != null)
            {
                SetControlState(HasUnsavedChanges());
            }
        }

        // Xử lý sự kiện CheckedChanged cho CheckBox AdQdkttienThu (nếu dùng CheckBox)
        // private void CheckBox_CheckedChanged(object sender, EventArgs e)
        // {
        //     if (_currentThamSo != null)
        //     {
        //         SetControlState(HasUnsavedChanges());
        //     }
        // }
    }
}