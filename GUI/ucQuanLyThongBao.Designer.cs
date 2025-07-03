using MaterialSkin.Controls; // Thêm using cho MaterialSkin
using System.Windows.Forms;  // Thêm using cho WinForms cơ bản

namespace GUI
{
    partial class ucQuanLyThongBao
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelTopActions = new System.Windows.Forms.Panel();
            this.btnXoa = new MaterialSkin.Controls.MaterialButton();
            this.btnSua = new MaterialSkin.Controls.MaterialButton();
            this.btnThem = new MaterialSkin.Controls.MaterialButton();
            this.panelInput = new MaterialSkin.Controls.MaterialCard();
            this.lblInputError = new MaterialSkin.Controls.MaterialLabel();
            this.btnHuy = new MaterialSkin.Controls.MaterialButton();
            this.btnLuu = new MaterialSkin.Controls.MaterialButton();
            this.cmbTrangThai = new MaterialSkin.Controls.MaterialComboBox();
            this.materialLabel6 = new MaterialSkin.Controls.MaterialLabel();
            this.cmbMucDo = new MaterialSkin.Controls.MaterialComboBox();
            this.materialLabel5 = new MaterialSkin.Controls.MaterialLabel();
            this.chkKhongHetHan = new MaterialSkin.Controls.MaterialCheckbox();
            this.dtpNgayKetThuc = new System.Windows.Forms.DateTimePicker();
            this.materialLabel4 = new MaterialSkin.Controls.MaterialLabel();
            this.dtpNgayBatDau = new System.Windows.Forms.DateTimePicker();
            this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
            this.txtNoiDung = new MaterialSkin.Controls.MaterialMultiLineTextBox2();
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            this.txtTieuDe = new MaterialSkin.Controls.MaterialTextBox();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.dgvThongBao = new System.Windows.Forms.DataGridView();
            this.panelTopActions.SuspendLayout();
            this.panelInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvThongBao)).BeginInit();
            this.SuspendLayout();
            //
            // panelTopActions
            //
            this.panelTopActions.Controls.Add(this.btnXoa);
            this.panelTopActions.Controls.Add(this.btnSua);
            this.panelTopActions.Controls.Add(this.btnThem);
            this.panelTopActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopActions.Location = new System.Drawing.Point(0, 0);
            this.panelTopActions.Name = "panelTopActions";
            this.panelTopActions.Padding = new System.Windows.Forms.Padding(5);
            this.panelTopActions.Size = new System.Drawing.Size(800, 50); // Chiều rộng ví dụ
            this.panelTopActions.TabIndex = 0;
            //
            // btnXoa
            //
            this.btnXoa.AutoSize = false;
            this.btnXoa.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnXoa.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnXoa.Depth = 0;
            this.btnXoa.HighEmphasis = true;
            this.btnXoa.Icon = null; // Đặt icon nếu muốn
            this.btnXoa.Location = new System.Drawing.Point(232, 8);
            this.btnXoa.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnXoa.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnXoa.Size = new System.Drawing.Size(100, 36);
            this.btnXoa.TabIndex = 2;
            this.btnXoa.Text = "Xóa";
            this.btnXoa.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnXoa.UseAccentColor = true; // Màu nhấn (thường là đỏ hoặc cam)
            this.btnXoa.UseVisualStyleBackColor = true;
            this.btnXoa.Click += new System.EventHandler(this.btnXoa_Click);
            //
            // btnSua
            //
            this.btnSua.AutoSize = false;
            this.btnSua.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSua.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnSua.Depth = 0;
            this.btnSua.HighEmphasis = true;
            this.btnSua.Icon = null; // Đặt icon nếu muốn
            this.btnSua.Location = new System.Drawing.Point(120, 8);
            this.btnSua.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnSua.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSua.Name = "btnSua";
            this.btnSua.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnSua.Size = new System.Drawing.Size(100, 36);
            this.btnSua.TabIndex = 1;
            this.btnSua.Text = "Sửa";
            this.btnSua.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnSua.UseAccentColor = false;
            this.btnSua.UseVisualStyleBackColor = true;
            this.btnSua.Click += new System.EventHandler(this.btnSua_Click);
            //
            // btnThem
            //
            this.btnThem.AutoSize = false;
            this.btnThem.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThem.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnThem.Depth = 0;
            this.btnThem.HighEmphasis = true;
            this.btnThem.Icon = null; // Đặt icon nếu muốn
            this.btnThem.Location = new System.Drawing.Point(8, 8);
            this.btnThem.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnThem.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnThem.Name = "btnThem";
            this.btnThem.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnThem.Size = new System.Drawing.Size(100, 36);
            this.btnThem.TabIndex = 0;
            this.btnThem.Text = "Thêm Mới";
            this.btnThem.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnThem.UseAccentColor = false;
            this.btnThem.UseVisualStyleBackColor = true;
            this.btnThem.Click += new System.EventHandler(this.btnThem_Click);
            //
            // panelInput
            //
            this.panelInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.panelInput.Controls.Add(this.lblInputError);
            this.panelInput.Controls.Add(this.btnHuy);
            this.panelInput.Controls.Add(this.btnLuu);
            this.panelInput.Controls.Add(this.cmbTrangThai);
            this.panelInput.Controls.Add(this.materialLabel6);
            this.panelInput.Controls.Add(this.cmbMucDo);
            this.panelInput.Controls.Add(this.materialLabel5);
            this.panelInput.Controls.Add(this.chkKhongHetHan);
            this.panelInput.Controls.Add(this.dtpNgayKetThuc);
            this.panelInput.Controls.Add(this.materialLabel4);
            this.panelInput.Controls.Add(this.dtpNgayBatDau);
            this.panelInput.Controls.Add(this.materialLabel3);
            this.panelInput.Controls.Add(this.txtNoiDung);
            this.panelInput.Controls.Add(this.materialLabel2);
            this.panelInput.Controls.Add(this.txtTieuDe);
            this.panelInput.Controls.Add(this.materialLabel1);
            this.panelInput.Depth = 1;
            this.panelInput.Dock = System.Windows.Forms.DockStyle.Bottom; // Đặt panel input ở dưới cùng
            this.panelInput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.panelInput.Location = new System.Drawing.Point(0, 400); // Vị trí ví dụ
            this.panelInput.Margin = new System.Windows.Forms.Padding(14);
            this.panelInput.MouseState = MaterialSkin.MouseState.HOVER;
            this.panelInput.Name = "panelInput";
            this.panelInput.Padding = new System.Windows.Forms.Padding(14);
            this.panelInput.Size = new System.Drawing.Size(800, 350); // Chiều cao ví dụ
            this.panelInput.TabIndex = 2;
            this.panelInput.Visible = false; // Ẩn ban đầu
            //
            // lblInputError
            //
            this.lblInputError.AutoSize = true;
            this.lblInputError.Depth = 0;
            this.lblInputError.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblInputError.ForeColor = System.Drawing.Color.Red; // Màu đỏ cho lỗi
            this.lblInputError.Location = new System.Drawing.Point(17, 300); // Vị trí ví dụ
            this.lblInputError.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblInputError.Name = "lblInputError";
            this.lblInputError.Size = new System.Drawing.Size(105, 19);
            this.lblInputError.TabIndex = 15;
            this.lblInputError.Text = "[Error Message]";
            this.lblInputError.Visible = false;
            //
            // btnHuy
            //
            this.btnHuy.AutoSize = false;
            this.btnHuy.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnHuy.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnHuy.Depth = 0;
            this.btnHuy.HighEmphasis = false; // Nút Hủy không cần nhấn mạnh
            this.btnHuy.Icon = null;
            this.btnHuy.Location = new System.Drawing.Point(682, 289); // Vị trí ví dụ
            this.btnHuy.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnHuy.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnHuy.Name = "btnHuy";
            this.btnHuy.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnHuy.Size = new System.Drawing.Size(90, 36);
            this.btnHuy.TabIndex = 14;
            this.btnHuy.Text = "Hủy";
            this.btnHuy.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text; // Nút dạng Text
            this.btnHuy.UseAccentColor = false;
            this.btnHuy.UseVisualStyleBackColor = true;
            this.btnHuy.Click += new System.EventHandler(this.btnHuy_Click);
            //
            // btnLuu
            //
            this.btnLuu.AutoSize = false;
            this.btnLuu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLuu.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnLuu.Depth = 0;
            this.btnLuu.HighEmphasis = true;
            this.btnLuu.Icon = null;
            this.btnLuu.Location = new System.Drawing.Point(578, 289); // Vị trí ví dụ
            this.btnLuu.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnLuu.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnLuu.Size = new System.Drawing.Size(90, 36);
            this.btnLuu.TabIndex = 13;
            this.btnLuu.Text = "Lưu";
            this.btnLuu.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnLuu.UseAccentColor = false;
            this.btnLuu.UseVisualStyleBackColor = true;
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            //
            // cmbTrangThai
            //
            this.cmbTrangThai.AutoResize = false;
            this.cmbTrangThai.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbTrangThai.Depth = 0;
            this.cmbTrangThai.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbTrangThai.DropDownHeight = 174;
            this.cmbTrangThai.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTrangThai.DropDownWidth = 121;
            this.cmbTrangThai.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.cmbTrangThai.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.cmbTrangThai.FormattingEnabled = true;
            this.cmbTrangThai.IntegralHeight = false;
            this.cmbTrangThai.ItemHeight = 43;
            this.cmbTrangThai.Location = new System.Drawing.Point(568, 228); // Vị trí ví dụ
            this.cmbTrangThai.MaxDropDownItems = 4;
            this.cmbTrangThai.MouseState = MaterialSkin.MouseState.OUT;
            this.cmbTrangThai.Name = "cmbTrangThai";
            this.cmbTrangThai.Size = new System.Drawing.Size(204, 49);
            this.cmbTrangThai.StartIndex = 0;
            this.cmbTrangThai.TabIndex = 12;
            //
            // materialLabel6
            //
            this.materialLabel6.AutoSize = true;
            this.materialLabel6.Depth = 0;
            this.materialLabel6.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel6.Location = new System.Drawing.Point(458, 242); // Vị trí ví dụ
            this.materialLabel6.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel6.Name = "materialLabel6";
            this.materialLabel6.Size = new System.Drawing.Size(78, 19);
            this.materialLabel6.TabIndex = 11;
            this.materialLabel6.Text = "Trạng thái:";
            //
            // cmbMucDo
            //
            this.cmbMucDo.AutoResize = false;
            this.cmbMucDo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbMucDo.Depth = 0;
            this.cmbMucDo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbMucDo.DropDownHeight = 174;
            this.cmbMucDo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMucDo.DropDownWidth = 121;
            this.cmbMucDo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.cmbMucDo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.cmbMucDo.FormattingEnabled = true;
            this.cmbMucDo.IntegralHeight = false;
            this.cmbMucDo.ItemHeight = 43;
            this.cmbMucDo.Location = new System.Drawing.Point(148, 228); // Vị trí ví dụ
            this.cmbMucDo.MaxDropDownItems = 4;
            this.cmbMucDo.MouseState = MaterialSkin.MouseState.OUT;
            this.cmbMucDo.Name = "cmbMucDo";
            this.cmbMucDo.Size = new System.Drawing.Size(270, 49);
            this.cmbMucDo.StartIndex = 0;
            this.cmbMucDo.TabIndex = 10;
            //
            // materialLabel5
            //
            this.materialLabel5.AutoSize = true;
            this.materialLabel5.Depth = 0;
            this.materialLabel5.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel5.Location = new System.Drawing.Point(17, 242); // Vị trí ví dụ
            this.materialLabel5.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel5.Name = "materialLabel5";
            this.materialLabel5.Size = new System.Drawing.Size(59, 19);
            this.materialLabel5.TabIndex = 9;
            this.materialLabel5.Text = "Mức độ:";
            //
            // chkKhongHetHan
            //
            this.chkKhongHetHan.AutoSize = true;
            this.chkKhongHetHan.Depth = 0;
            this.chkKhongHetHan.Location = new System.Drawing.Point(458, 183); // Vị trí ví dụ
            this.chkKhongHetHan.Margin = new System.Windows.Forms.Padding(0);
            this.chkKhongHetHan.MouseLocation = new System.Drawing.Point(-1, -1);
            this.chkKhongHetHan.MouseState = MaterialSkin.MouseState.HOVER;
            this.chkKhongHetHan.Name = "chkKhongHetHan";
            this.chkKhongHetHan.ReadOnly = false;
            this.chkKhongHetHan.Ripple = true;
            this.chkKhongHetHan.Size = new System.Drawing.Size(135, 37);
            this.chkKhongHetHan.TabIndex = 8;
            this.chkKhongHetHan.Text = "Không hết hạn";
            this.chkKhongHetHan.UseVisualStyleBackColor = true;
            // Sự kiện CheckedChanged thêm trong code .cs
            //
            // dtpNgayKetThuc
            //
            this.dtpNgayKetThuc.CustomFormat = "dd/MM/yyyy HH:mm"; // Ví dụ định dạng
            this.dtpNgayKetThuc.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpNgayKetThuc.Location = new System.Drawing.Point(568, 149); // Vị trí ví dụ
            this.dtpNgayKetThuc.Name = "dtpNgayKetThuc";
            this.dtpNgayKetThuc.Size = new System.Drawing.Size(204, 23); // Font size mặc định là 9pt
            this.dtpNgayKetThuc.TabIndex = 7;
            //
            // materialLabel4
            //
            this.materialLabel4.AutoSize = true;
            this.materialLabel4.Depth = 0;
            this.materialLabel4.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel4.Location = new System.Drawing.Point(458, 152); // Vị trí ví dụ
            this.materialLabel4.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel4.Name = "materialLabel4";
            this.materialLabel4.Size = new System.Drawing.Size(101, 19);
            this.materialLabel4.TabIndex = 6;
            this.materialLabel4.Text = "Ngày kết thúc:";
            //
            // dtpNgayBatDau
            //
            this.dtpNgayBatDau.CustomFormat = "dd/MM/yyyy HH:mm"; // Ví dụ định dạng
            this.dtpNgayBatDau.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpNgayBatDau.Location = new System.Drawing.Point(148, 149); // Vị trí ví dụ
            this.dtpNgayBatDau.Name = "dtpNgayBatDau";
            this.dtpNgayBatDau.Size = new System.Drawing.Size(270, 23); // Font size mặc định là 9pt
            this.dtpNgayBatDau.TabIndex = 5;
            //
            // materialLabel3
            //
            this.materialLabel3.AutoSize = true;
            this.materialLabel3.Depth = 0;
            this.materialLabel3.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel3.Location = new System.Drawing.Point(17, 152); // Vị trí ví dụ
            this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel3.Name = "materialLabel3";
            this.materialLabel3.Size = new System.Drawing.Size(99, 19);
            this.materialLabel3.TabIndex = 4;
            this.materialLabel3.Text = "Ngày bắt đầu:";
            //
            // txtNoiDung
            //
            this.txtNoiDung.AnimateReadOnly = false;
            this.txtNoiDung.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtNoiDung.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtNoiDung.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtNoiDung.Depth = 0;
            this.txtNoiDung.HideSelection = true;
            this.txtNoiDung.Location = new System.Drawing.Point(148, 72); // Vị trí ví dụ
            this.txtNoiDung.MaxLength = 32767;
            this.txtNoiDung.MouseState = MaterialSkin.MouseState.OUT;
            this.txtNoiDung.Name = "txtNoiDung";
            this.txtNoiDung.PasswordChar = '\0';
            this.txtNoiDung.ReadOnly = false;
            this.txtNoiDung.ScrollBars = System.Windows.Forms.ScrollBars.Vertical; // Cho phép cuộn dọc
            this.txtNoiDung.SelectedText = "";
            this.txtNoiDung.SelectionLength = 0;
            this.txtNoiDung.SelectionStart = 0;
            this.txtNoiDung.ShortcutsEnabled = true;
            this.txtNoiDung.Size = new System.Drawing.Size(624, 66); // Kích thước ví dụ
            this.txtNoiDung.TabIndex = 3;
            this.txtNoiDung.TabStop = false;
            this.txtNoiDung.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtNoiDung.UseSystemPasswordChar = false;
            //
            // materialLabel2
            //
            this.materialLabel2.AutoSize = true;
            this.materialLabel2.Depth = 0;
            this.materialLabel2.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel2.Location = new System.Drawing.Point(17, 75); // Vị trí ví dụ
            this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel2.Name = "materialLabel2";
            this.materialLabel2.Size = new System.Drawing.Size(69, 19);
            this.materialLabel2.TabIndex = 2;
            this.materialLabel2.Text = "Nội dung:";
            //
            // txtTieuDe
            //
            this.txtTieuDe.AnimateReadOnly = false;
            this.txtTieuDe.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTieuDe.Depth = 0;
            this.txtTieuDe.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtTieuDe.LeadingIcon = null;
            this.txtTieuDe.Location = new System.Drawing.Point(148, 10); // Vị trí ví dụ
            this.txtTieuDe.MaxLength = 200; // Giới hạn ký tự cho tiêu đề
            this.txtTieuDe.MouseState = MaterialSkin.MouseState.OUT;
            this.txtTieuDe.Multiline = false;
            this.txtTieuDe.Name = "txtTieuDe";
            this.txtTieuDe.Size = new System.Drawing.Size(624, 50); // Cao 50 để chuẩn MaterialSkin
            this.txtTieuDe.TabIndex = 1;
            this.txtTieuDe.Text = "";
            this.txtTieuDe.TrailingIcon = null;
            //
            // materialLabel1
            //
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel1.Location = new System.Drawing.Point(17, 24); // Vị trí ví dụ
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(59, 19);
            this.materialLabel1.TabIndex = 0;
            this.materialLabel1.Text = "Tiêu đề:";
            //
            // dgvThongBao
            //
            this.dgvThongBao.AllowUserToAddRows = false;
            this.dgvThongBao.AllowUserToDeleteRows = false;
            this.dgvThongBao.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240))))); // Màu xen kẽ
            this.dgvThongBao.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvThongBao.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvThongBao.BackgroundColor = System.Drawing.SystemColors.Window; // Màu nền grid
            this.dgvThongBao.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control; // Màu header mặc định
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvThongBao.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvThongBao.ColumnHeadersHeight = 29; // Chiều cao header
            this.dgvThongBao.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvThongBao.Dock = System.Windows.Forms.DockStyle.Fill; // Lấp đầy khoảng trống giữa 2 panel
            this.dgvThongBao.GridColor = System.Drawing.SystemColors.ControlLight; // Màu đường kẻ
            this.dgvThongBao.Location = new System.Drawing.Point(0, 50); // Nằm dưới panelTopActions
            this.dgvThongBao.MultiSelect = false;
            this.dgvThongBao.Name = "dgvThongBao";
            this.dgvThongBao.ReadOnly = true;
            this.dgvThongBao.RowHeadersVisible = false; // Ẩn cột header bên trái
            this.dgvThongBao.RowTemplate.Height = 25;
            this.dgvThongBao.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvThongBao.Size = new System.Drawing.Size(800, 350); // Kích thước ví dụ
            this.dgvThongBao.TabIndex = 1;
            this.dgvThongBao.SelectionChanged += new System.EventHandler(this.dgvThongBao_SelectionChanged);
            //
            // ucQuanLyThongBao
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvThongBao); // Thêm grid trước panel input
            this.Controls.Add(this.panelInput);
            this.Controls.Add(this.panelTopActions);
            this.Name = "ucQuanLyThongBao";
            this.Size = new System.Drawing.Size(800, 750); // Kích thước tổng thể ví dụ
            this.Load += new System.EventHandler(this.ucQuanLyThongBao_Load);
            this.panelTopActions.ResumeLayout(false);
            this.panelInput.ResumeLayout(false);
            this.panelInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvThongBao)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        // --- KHAI BÁO BIẾN CHO CONTROLS ---
        private Panel panelTopActions;
        private MaterialButton btnXoa;
        private MaterialButton btnSua;
        private MaterialButton btnThem;
        private MaterialCard panelInput;
        private MaterialLabel lblInputError;
        private MaterialButton btnHuy;
        private MaterialButton btnLuu;
        private MaterialComboBox cmbTrangThai;
        private MaterialLabel materialLabel6;
        private MaterialComboBox cmbMucDo;
        private MaterialLabel materialLabel5;
        private MaterialCheckbox chkKhongHetHan;
        private DateTimePicker dtpNgayKetThuc;
        private MaterialLabel materialLabel4;
        private DateTimePicker dtpNgayBatDau;
        private MaterialLabel materialLabel3;
        private MaterialMultiLineTextBox2 txtNoiDung;
        private MaterialLabel materialLabel2;
        private MaterialTextBox txtTieuDe;
        private MaterialLabel materialLabel1;
        private DataGridView dgvThongBao;
    }
}