// Project/Namespace: GUI
// File: QuanLyThuVien/GUI/ucQuanLyNguoiDung.Designer.cs
// Phiên bản đã sửa lỗi - loại bỏ gán sự kiện InputField_Changed không tồn tại

using System;
using System.Windows.Forms;
using MaterialSkin.Controls;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;

namespace GUI
{
    partial class ucQuanLyNguoiDung
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // --- KHAI BÁO CÁC CONTROLS UI (Biến thành viên partial class) ---
        private MaterialSkin.Controls.MaterialTextBox2 txtId;
        private MaterialSkin.Controls.MaterialTextBox2 txtMaNguoiDung;
        private MaterialSkin.Controls.MaterialTextBox2 txtTenDangNhap;
        private MaterialSkin.Controls.MaterialTextBox2 txtMatKhau;
        private MaterialSkin.Controls.MaterialTextBox2 txtTenHienThi;
        private System.Windows.Forms.DateTimePicker dtpNgaySinh;
        private MaterialSkin.Controls.MaterialTextBox2 txtChucVu;
        // Loại bỏ khai báo không dùng: txtDiaChi, txtDienThoai, txtEmail
        private System.Windows.Forms.ComboBox cboNhomNguoiDung;
        private MaterialSkin.Controls.MaterialButton btnThem;
        private MaterialSkin.Controls.MaterialButton btnSua;
        private MaterialSkin.Controls.MaterialButton btnXoa;
        private MaterialSkin.Controls.MaterialButton btnKhoiPhuc; // Giữ lại khai báo nếu cần logic phục hồi
        private MaterialSkin.Controls.MaterialButton btnLuu;
        private MaterialSkin.Controls.MaterialButton btnBoQua;
        private MaterialSkin.Controls.MaterialButton btnThoat;
        private System.Windows.Forms.DataGridView dgvNguoiDung;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.Panel panelGrid;
        private System.Windows.Forms.Panel panelButtons; // Panel cho nút


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
            this.panelDetails = new System.Windows.Forms.Panel();
            this.cboNhomNguoiDung = new System.Windows.Forms.ComboBox();
            // Loại bỏ khởi tạo không dùng: txtEmail, txtDienThoai, txtDiaChi
            this.dtpNgaySinh = new System.Windows.Forms.DateTimePicker();
            this.txtChucVu = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtTenHienThi = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtMatKhau = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtTenDangNhap = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtMaNguoiDung = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtId = new MaterialSkin.Controls.MaterialTextBox2();
            this.btnLuu = new MaterialSkin.Controls.MaterialButton();
            this.btnBoQua = new MaterialSkin.Controls.MaterialButton();
            this.panelGrid = new System.Windows.Forms.Panel();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.dgvNguoiDung = new System.Windows.Forms.DataGridView();
            this.btnThoat = new MaterialSkin.Controls.MaterialButton();
            this.btnKhoiPhuc = new MaterialSkin.Controls.MaterialButton(); // Giữ lại nếu cần
            this.btnXoa = new MaterialSkin.Controls.MaterialButton();
            this.btnSua = new MaterialSkin.Controls.MaterialButton();
            this.btnThem = new MaterialSkin.Controls.MaterialButton();

            this.panelDetails.SuspendLayout();
            this.panelGrid.SuspendLayout();
            this.panelButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNguoiDung)).BeginInit();
            this.SuspendLayout();
            // 
            // panelDetails
            // 
            this.panelDetails.Controls.Add(this.cboNhomNguoiDung);
            // Loại bỏ thêm control không dùng: txtEmail, txtDienThoai, txtDiaChi
            this.panelDetails.Controls.Add(this.dtpNgaySinh);
            this.panelDetails.Controls.Add(this.txtChucVu);
            this.panelDetails.Controls.Add(this.txtTenHienThi);
            this.panelDetails.Controls.Add(this.txtMatKhau);
            this.panelDetails.Controls.Add(this.txtTenDangNhap);
            this.panelDetails.Controls.Add(this.txtMaNguoiDung);
            this.panelDetails.Controls.Add(this.txtId);
            this.panelDetails.Controls.Add(this.btnLuu);
            this.panelDetails.Controls.Add(this.btnBoQua);
            this.panelDetails.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelDetails.Location = new System.Drawing.Point(10, 10); // Vị trí sau Padding UserControl
            this.panelDetails.Margin = new System.Windows.Forms.Padding(3); // *** ĐỔI MARGIN ***
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Padding = new System.Windows.Forms.Padding(10); // *** ĐỔI PADDING ***
            this.panelDetails.Size = new System.Drawing.Size(880, 240); // *** TĂNG CHIỀU CAO PANEL DETAILS ĐỂ CHỨA NÚT LƯU/BỎ QUA ***
            this.panelDetails.TabIndex = 0;
            // 
            // cboNhomNguoiDung
            // 
            this.cboNhomNguoiDung.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboNhomNguoiDung.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboNhomNguoiDung.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNhomNguoiDung.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.cboNhomNguoiDung.FormattingEnabled = true;
            this.cboNhomNguoiDung.Location = new System.Drawing.Point(667, 86); // Hàng 2
            this.cboNhomNguoiDung.Margin = new System.Windows.Forms.Padding(3); // *** ĐỔI MARGIN ***
            this.cboNhomNguoiDung.Name = "cboNhomNguoiDung";
            this.cboNhomNguoiDung.Size = new System.Drawing.Size(200, 28);
            this.cboNhomNguoiDung.TabIndex = 7; // TabIndex cho ComboBox
            // 
            // dtpNgaySinh
            // 
            this.dtpNgaySinh.CustomFormat = "dd/MM/yyyy";
            this.dtpNgaySinh.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dtpNgaySinh.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpNgaySinh.Location = new System.Drawing.Point(290, 86); // Hàng 2 
            this.dtpNgaySinh.Margin = new System.Windows.Forms.Padding(3); // *** ĐỔI MARGIN ***
            this.dtpNgaySinh.Name = "dtpNgaySinh";
            this.dtpNgaySinh.Size = new System.Drawing.Size(150, 26);
            this.dtpNgaySinh.TabIndex = 5; // TabIndex cho DateTimePicker
            // 
            // txtChucVu
            // 
            this.txtChucVu.AnimateReadOnly = false;
            this.txtChucVu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtChucVu.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtChucVu.Depth = 0;
            this.txtChucVu.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtChucVu.HideSelection = true;
            this.txtChucVu.Hint = "Chức Vụ";
            this.txtChucVu.LeadingIcon = null;
            this.txtChucVu.Location = new System.Drawing.Point(460, 76); // Hàng 2
            this.txtChucVu.Margin = new System.Windows.Forms.Padding(3); // *** ĐỔI MARGIN ***
            this.txtChucVu.MaxLength = 50;
            this.txtChucVu.MouseState = MaterialSkin.MouseState.OUT;
            this.txtChucVu.Name = "txtChucVu";
            this.txtChucVu.PasswordChar = '\0';
            this.txtChucVu.PrefixSuffixText = null;
            this.txtChucVu.ReadOnly = false;
            this.txtChucVu.SelectedText = "";
            this.txtChucVu.SelectionLength = 0;
            this.txtChucVu.SelectionStart = 0;
            this.txtChucVu.ShortcutsEnabled = true;
            this.txtChucVu.Size = new System.Drawing.Size(200, 48); // Giữ nguyên UseTallSize=false
            this.txtChucVu.TabIndex = 6; // TabIndex cho Chức vụ
            this.txtChucVu.TabStop = true;
            this.txtChucVu.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtChucVu.TrailingIcon = null;
            this.txtChucVu.UseSystemPasswordChar = false;
            this.txtChucVu.UseTallSize = false; // *** QUAN TRỌNG: Giữ UseTallSize = false cho MaterialTextBox2 ***
            // 
            // txtTenHienThi
            // 
            this.txtTenHienThi.AnimateReadOnly = false;
            this.txtTenHienThi.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtTenHienThi.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtTenHienThi.Depth = 0;
            this.txtTenHienThi.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtTenHienThi.HideSelection = true;
            this.txtTenHienThi.Hint = "Tên Hiển Thị";
            this.txtTenHienThi.LeadingIcon = null;
            this.txtTenHienThi.Location = new System.Drawing.Point(20, 76); // Hàng 2
            this.txtTenHienThi.Margin = new System.Windows.Forms.Padding(3); // *** ĐỔI MARGIN ***
            this.txtTenHienThi.MaxLength = 50;
            this.txtTenHienThi.MouseState = MaterialSkin.MouseState.OUT;
            this.txtTenHienThi.Name = "txtTenHienThi";
            this.txtTenHienThi.PasswordChar = '\0';
            this.txtTenHienThi.PrefixSuffixText = null;
            this.txtTenHienThi.ReadOnly = false;
            this.txtTenHienThi.SelectedText = "";
            this.txtTenHienThi.SelectionLength = 0;
            this.txtTenHienThi.SelectionStart = 0;
            this.txtTenHienThi.ShortcutsEnabled = true;
            this.txtTenHienThi.Size = new System.Drawing.Size(250, 48); // Giữ nguyên UseTallSize=false
            this.txtTenHienThi.TabIndex = 4; // TabIndex cho Tên hiển thị
            this.txtTenHienThi.TabStop = true;
            this.txtTenHienThi.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtTenHienThi.TrailingIcon = null;
            this.txtTenHienThi.UseSystemPasswordChar = false;
            this.txtTenHienThi.UseTallSize = false; // *** QUAN TRỌNG: Giữ UseTallSize = false cho MaterialTextBox2 ***
            // 
            // txtMatKhau
            // 
            this.txtMatKhau.AnimateReadOnly = false;
            this.txtMatKhau.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtMatKhau.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtMatKhau.Depth = 0;
            this.txtMatKhau.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtMatKhau.HideSelection = true;
            this.txtMatKhau.Hint = "Mật khẩu";
            this.txtMatKhau.LeadingIcon = null;
            this.txtMatKhau.Location = new System.Drawing.Point(530, 16); // Hàng 1
            this.txtMatKhau.Margin = new System.Windows.Forms.Padding(3); // *** ĐỔI MARGIN ***
            this.txtMatKhau.MaxLength = 50;
            this.txtMatKhau.MouseState = MaterialSkin.MouseState.OUT;
            this.txtMatKhau.Name = "txtMatKhau";
            this.txtMatKhau.PasswordChar = '●';
            this.txtMatKhau.PrefixSuffixText = null;
            this.txtMatKhau.ReadOnly = false;
            this.txtMatKhau.SelectedText = "";
            this.txtMatKhau.SelectionLength = 0;
            this.txtMatKhau.SelectionStart = 0;
            this.txtMatKhau.ShortcutsEnabled = true;
            this.txtMatKhau.Size = new System.Drawing.Size(200, 48); // Giữ nguyên UseTallSize=false
            this.txtMatKhau.TabIndex = 3; // TabIndex cho Mật khẩu
            this.txtMatKhau.TabStop = true;
            this.txtMatKhau.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtMatKhau.TrailingIcon = null;
            this.txtMatKhau.UseSystemPasswordChar = true;
            this.txtMatKhau.UseTallSize = false; // *** QUAN TRỌNG: Giữ UseTallSize = false cho MaterialTextBox2 ***
            // 
            // txtTenDangNhap
            // 
            this.txtTenDangNhap.AnimateReadOnly = false;
            this.txtTenDangNhap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtTenDangNhap.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtTenDangNhap.Depth = 0;
            this.txtTenDangNhap.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtTenDangNhap.HideSelection = true;
            this.txtTenDangNhap.Hint = "Tên Đăng Nhập";
            this.txtTenDangNhap.LeadingIcon = null;
            this.txtTenDangNhap.Location = new System.Drawing.Point(310, 16); // Hàng 1
            this.txtTenDangNhap.Margin = new System.Windows.Forms.Padding(3); // *** ĐỔI MARGIN ***
            this.txtTenDangNhap.MaxLength = 256;
            this.txtTenDangNhap.MouseState = MaterialSkin.MouseState.OUT;
            this.txtTenDangNhap.Name = "txtTenDangNhap";
            this.txtTenDangNhap.PasswordChar = '\0';
            this.txtTenDangNhap.PrefixSuffixText = null;
            this.txtTenDangNhap.ReadOnly = false;
            this.txtTenDangNhap.SelectedText = "";
            this.txtTenDangNhap.SelectionLength = 0;
            this.txtTenDangNhap.SelectionStart = 0;
            this.txtTenDangNhap.ShortcutsEnabled = true;
            this.txtTenDangNhap.Size = new System.Drawing.Size(200, 48); // Giữ nguyên UseTallSize=false
            this.txtTenDangNhap.TabIndex = 2; // TabIndex cho Tên đăng nhập
            this.txtTenDangNhap.TabStop = true;
            this.txtTenDangNhap.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtTenDangNhap.TrailingIcon = null;
            this.txtTenDangNhap.UseSystemPasswordChar = false;
            this.txtTenDangNhap.UseTallSize = false; // *** QUAN TRỌNG: Giữ UseTallSize = false cho MaterialTextBox2 ***
            // 
            // txtMaNguoiDung
            // 
            this.txtMaNguoiDung.AnimateReadOnly = false;
            this.txtMaNguoiDung.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtMaNguoiDung.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtMaNguoiDung.Depth = 0;
            this.txtMaNguoiDung.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtMaNguoiDung.HideSelection = true;
            this.txtMaNguoiDung.Hint = "Mã Người Dùng";
            this.txtMaNguoiDung.LeadingIcon = null;
            this.txtMaNguoiDung.Location = new System.Drawing.Point(140, 16); // Hàng 1
            this.txtMaNguoiDung.Margin = new System.Windows.Forms.Padding(3); // *** ĐỔI MARGIN ***
            this.txtMaNguoiDung.MaxLength = 6;
            this.txtMaNguoiDung.MouseState = MaterialSkin.MouseState.OUT;
            this.txtMaNguoiDung.Name = "txtMaNguoiDung";
            this.txtMaNguoiDung.PasswordChar = '\0';
            this.txtMaNguoiDung.PrefixSuffixText = null;
            this.txtMaNguoiDung.ReadOnly = false;
            this.txtMaNguoiDung.SelectedText = "";
            this.txtMaNguoiDung.SelectionLength = 0;
            this.txtMaNguoiDung.SelectionStart = 0;
            this.txtMaNguoiDung.ShortcutsEnabled = true;
            this.txtMaNguoiDung.Size = new System.Drawing.Size(150, 48); // Giữ nguyên UseTallSize=false
            this.txtMaNguoiDung.TabIndex = 1; // TabIndex cho Mã người dùng
            this.txtMaNguoiDung.TabStop = true;
            this.txtMaNguoiDung.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtMaNguoiDung.TrailingIcon = null;
            this.txtMaNguoiDung.UseSystemPasswordChar = false;
            this.txtMaNguoiDung.UseTallSize = false; // *** QUAN TRỌNG: Giữ UseTallSize = false cho MaterialTextBox2 ***
            // 
            // txtId
            // 
            this.txtId.AnimateReadOnly = false;
            this.txtId.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtId.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtId.Depth = 0;
            this.txtId.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtId.HideSelection = true;
            this.txtId.Hint = "ID";
            this.txtId.LeadingIcon = null;
            this.txtId.Location = new System.Drawing.Point(20, 16); // Hàng 1
            this.txtId.Margin = new System.Windows.Forms.Padding(3); // *** ĐỔI MARGIN ***
            this.txtId.MaxLength = 32767;
            this.txtId.MouseState = MaterialSkin.MouseState.OUT;
            this.txtId.Name = "txtId";
            this.txtId.PasswordChar = '\0';
            this.txtId.PrefixSuffixText = null;
            this.txtId.ReadOnly = true; // ID thường là ReadOnly
            this.txtId.SelectedText = "";
            this.txtId.SelectionLength = 0;
            this.txtId.SelectionStart = 0;
            this.txtId.ShortcutsEnabled = true;
            this.txtId.Size = new System.Drawing.Size(100, 48); // Giữ nguyên UseTallSize=false
            this.txtId.TabIndex = 0; // TabIndex cho ID
            this.txtId.TabStop = false; // ID không cần TabStop
            this.txtId.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtId.TrailingIcon = null;
            this.txtId.UseSystemPasswordChar = false;
            this.txtId.UseTallSize = false; // *** QUAN TRỌNG: Giữ UseTallSize = false cho MaterialTextBox2 ***
            // 
            // btnLuu
            // 
            this.btnLuu.AutoSize = false;
            this.btnLuu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLuu.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnLuu.Depth = 0;
            this.btnLuu.HighEmphasis = true;
            this.btnLuu.Icon = null;
            this.btnLuu.Location = new System.Drawing.Point(340, 140); // *** ĐIỀU CHỈNH VỊ TRÍ NÚT LƯU ***
            this.btnLuu.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnLuu.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnLuu.Size = new System.Drawing.Size(80, 36);
            this.btnLuu.TabIndex = 8; // TabIndex cho nút Lưu
            this.btnLuu.Text = "Lưu";
            this.btnLuu.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnLuu.UseAccentColor = false;
            this.btnLuu.UseVisualStyleBackColor = true;
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            // 
            // btnBoQua
            // 
            this.btnBoQua.AutoSize = false;
            this.btnBoQua.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnBoQua.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnBoQua.Depth = 0;
            this.btnBoQua.HighEmphasis = false;
            this.btnBoQua.Icon = null;
            this.btnBoQua.Location = new System.Drawing.Point(440, 140); // *** ĐIỀU CHỈNH VỊ TRÍ NÚT BỎ QUA ***
            this.btnBoQua.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnBoQua.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnBoQua.Name = "btnBoQua";
            this.btnBoQua.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnBoQua.Size = new System.Drawing.Size(86, 36);
            this.btnBoQua.TabIndex = 9; // TabIndex cho nút Bỏ qua
            this.btnBoQua.Text = "Bỏ qua";
            this.btnBoQua.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnBoQua.UseAccentColor = false;
            this.btnBoQua.UseVisualStyleBackColor = true;
            this.btnBoQua.Click += new System.EventHandler(this.btnBoQua_Click);
            // 
            // panelGrid
            // 
            this.panelGrid.Controls.Add(this.dgvNguoiDung);
            this.panelGrid.Controls.Add(this.panelButtons);
            this.panelGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGrid.Location = new System.Drawing.Point(10, 250); // Vị trí sau panelDetails (đã tăng chiều cao) và Padding UserControl
            this.panelGrid.Margin = new System.Windows.Forms.Padding(3); // *** ĐỔI MARGIN ***
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Padding = new System.Windows.Forms.Padding(10); // *** ĐỔI PADDING ***
            this.panelGrid.Size = new System.Drawing.Size(880, 340); // Kích thước còn lại sau Padding và panelDetails
            this.panelGrid.TabIndex = 1;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnThoat);
            this.panelButtons.Controls.Add(this.btnKhoiPhuc);
            this.panelButtons.Controls.Add(this.btnXoa);
            this.panelButtons.Controls.Add(this.btnSua);
            this.panelButtons.Controls.Add(this.btnThem);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelButtons.Location = new System.Drawing.Point(10, 10); // Vị trí sau padding panelGrid
            this.panelButtons.Margin = new System.Windows.Forms.Padding(3); // *** ĐỔI MARGIN ***
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Padding = new System.Windows.Forms.Padding(3); // *** ĐỔI PADDING ***
            this.panelButtons.Size = new System.Drawing.Size(860, 48); // Điều chỉnh Size
            this.panelButtons.TabIndex = 0; // TabIndex cho panel chứa nút chính
            // 
            // dgvNguoiDung
            // 
            this.dgvNguoiDung.AllowUserToAddRows = false;
            this.dgvNguoiDung.AllowUserToDeleteRows = false;
            this.dgvNguoiDung.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNguoiDung.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvNguoiDung.Location = new System.Drawing.Point(10, 58); // Vị trí sau panelButtons và padding panelGrid
            this.dgvNguoiDung.Margin = new System.Windows.Forms.Padding(3); // *** ĐỔI MARGIN ***
            this.dgvNguoiDung.MultiSelect = false;
            this.dgvNguoiDung.Name = "dgvNguoiDung";
            this.dgvNguoiDung.ReadOnly = true;
            this.dgvNguoiDung.RowHeadersWidth = 51;
            this.dgvNguoiDung.RowTemplate.Height = 24;
            this.dgvNguoiDung.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvNguoiDung.Size = new System.Drawing.Size(860, 272); // Kích thước còn lại sau padding và panelButtons
            this.dgvNguoiDung.TabIndex = 1; // TabIndex cho DataGridView
            this.dgvNguoiDung.SelectionChanged += new System.EventHandler(this.dgvNguoiDung_SelectionChanged);
            this.dgvNguoiDung.DoubleClick += new System.EventHandler(this.dgvNguoiDung_DoubleClick);
            // 
            // btnThoat
            // 
            this.btnThoat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnThoat.AutoSize = false;
            this.btnThoat.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThoat.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnThoat.Depth = 0;
            this.btnThoat.HighEmphasis = false;
            this.btnThoat.Icon = null;
            this.btnThoat.Location = new System.Drawing.Point(777, 6); // Điều chỉnh X
            this.btnThoat.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnThoat.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnThoat.Size = new System.Drawing.Size(80, 36);
            this.btnThoat.TabIndex = 4; // TabIndex cho nút Thoát
            this.btnThoat.Text = "Thoát";
            this.btnThoat.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text;
            this.btnThoat.UseAccentColor = false;
            this.btnThoat.UseVisualStyleBackColor = true;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // btnKhoiPhuc
            // 
            this.btnKhoiPhuc.AutoSize = false;
            this.btnKhoiPhuc.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnKhoiPhuc.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnKhoiPhuc.Depth = 0;
            this.btnKhoiPhuc.HighEmphasis = false;
            this.btnKhoiPhuc.Icon = null;
            this.btnKhoiPhuc.Location = new System.Drawing.Point(296, 6);
            this.btnKhoiPhuc.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnKhoiPhuc.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnKhoiPhuc.Name = "btnKhoiPhuc";
            this.btnKhoiPhuc.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnKhoiPhuc.Size = new System.Drawing.Size(96, 36);
            this.btnKhoiPhuc.TabIndex = 3; // TabIndex cho nút Khôi phục
            this.btnKhoiPhuc.Text = "Phục hồi";
            this.btnKhoiPhuc.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnKhoiPhuc.UseAccentColor = false;
            this.btnKhoiPhuc.UseVisualStyleBackColor = true;
            this.btnKhoiPhuc.Click += new System.EventHandler(this.btnKhoiPhuc_Click);
            // 
            // btnXoa
            // 
            this.btnXoa.AutoSize = false;
            this.btnXoa.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnXoa.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnXoa.Depth = 0;
            this.btnXoa.HighEmphasis = false;
            this.btnXoa.Icon = null;
            this.btnXoa.Location = new System.Drawing.Point(202, 6);
            this.btnXoa.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnXoa.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnXoa.Size = new System.Drawing.Size(80, 36);
            this.btnXoa.TabIndex = 2; // TabIndex cho nút Xóa
            this.btnXoa.Text = "Xóa";
            this.btnXoa.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnXoa.UseAccentColor = true; // Màu nhấn cho nút Xóa
            this.btnXoa.UseVisualStyleBackColor = true;
            this.btnXoa.Click += new System.EventHandler(this.btnXoa_Click);
            // 
            // btnSua
            // 
            this.btnSua.AutoSize = false;
            this.btnSua.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSua.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnSua.Depth = 0;
            this.btnSua.HighEmphasis = false;
            this.btnSua.Icon = null;
            this.btnSua.Location = new System.Drawing.Point(108, 6);
            this.btnSua.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnSua.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSua.Name = "btnSua";
            this.btnSua.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnSua.Size = new System.Drawing.Size(80, 36);
            this.btnSua.TabIndex = 1; // TabIndex cho nút Sửa
            this.btnSua.Text = "Sửa";
            this.btnSua.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
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
            this.btnThem.Icon = null;
            this.btnThem.Location = new System.Drawing.Point(14, 6);
            this.btnThem.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnThem.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnThem.Name = "btnThem";
            this.btnThem.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnThem.Size = new System.Drawing.Size(80, 36);
            this.btnThem.TabIndex = 0; // TabIndex cho nút Thêm
            this.btnThem.Text = "Thêm";
            this.btnThem.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnThem.UseAccentColor = false;
            this.btnThem.UseVisualStyleBackColor = true;
            this.btnThem.Click += new System.EventHandler(this.btnThem_Click);
            // 
            // ucQuanLyNguoiDung
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.panelDetails);
            this.Margin = new System.Windows.Forms.Padding(3); // *** ĐỔI MARGIN ***
            this.Name = "ucQuanLyNguoiDung";
            this.Padding = new System.Windows.Forms.Padding(10); // *** ĐỔI PADDING ***
            this.Size = new System.Drawing.Size(900, 600);
            this.Load += new System.EventHandler(this.ucQuanLyNguoiDung_Load);
            this.panelDetails.ResumeLayout(false);
            this.panelDetails.PerformLayout();
            this.panelGrid.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false); // ResumeLayout cho panel nút
            //this.panelButtons.PerformLayout(); // Không cần PerformLayout ở đây
            ((System.ComponentModel.ISupportInitialize)(this.dgvNguoiDung)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        // Các khai báo biến control đã được đưa lên đầu class
    }
}
