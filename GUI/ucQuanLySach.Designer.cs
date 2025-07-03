// File: GUI/ucQuanLySach.Designer.cs
// Project/Namespace: GUI

using System;
using System.Windows.Forms; // Cần cho các loại control, AutoScaleMode, SizeF, Point, EventArgs, DockStyle, AnchorStyles
using MaterialSkin.Controls; // Cần cho MaterialTextBox2, MaterialButton (Kiểm tra namespace chính xác)
using System.Drawing; // Cần cho Point, Size
using System.ComponentModel; // Cần cho IContainer, ComponentResourceManager

namespace GUI // Namespace của project GUI của bạn
{
    // Class được định nghĩa một phần tại đây, phần còn lại ở file ucQuanLySach.cs
    partial class ucQuanLySach
    {
        /// <summary>
        /// Required designer variable.
        /// Biến này được quản lý bởi trình thiết kế (Designer).
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // --- KHAI BÁO CÁC CONTROLS UI SỬ DỤNG TRONG CODE-BEHIND ---
        private MaterialSkin.Controls.MaterialTextBox2 txtId;
        private MaterialSkin.Controls.MaterialTextBox2 txtMaSach;
        private MaterialSkin.Controls.MaterialTextBox2 txtDonGia;
        private MaterialSkin.Controls.MaterialTextBox2 txtNamXb;
        private MaterialSkin.Controls.MaterialTextBox2 txtNhaXb;
        private System.Windows.Forms.ComboBox cboTuaSach;

        private MaterialSkin.Controls.MaterialButton btnThem;
        private MaterialSkin.Controls.MaterialButton btnSua;
        private MaterialSkin.Controls.MaterialButton btnXoa;
        private MaterialSkin.Controls.MaterialButton btnLuu;
        private MaterialSkin.Controls.MaterialButton btnBoQua;
        private MaterialSkin.Controls.MaterialButton btnThoat;
        private MaterialSkin.Controls.MaterialButton btnXemCuonSach; // <<< Nút mới

        private System.Windows.Forms.DataGridView dgvSach;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.Panel panelGrid;

        // Thêm các Label nếu bạn có dùng Label để chú thích các ô nhập liệu
        // private MaterialSkin.Controls.MaterialLabel lblId;
        // ... (các label khác)


        /// <summary>
        /// Clean up any resources being used.
        /// Giải phóng các tài nguyên đang được sử dụng.
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
        /// Phương thức bắt buộc cho Designer - không được sửa đổi nội dung
        /// của phương thức này bằng trình soạn thảo mã (code editor).
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelDetails = new System.Windows.Forms.Panel();
            this.txtNhaXb = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtNamXb = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtDonGia = new MaterialSkin.Controls.MaterialTextBox2();
            this.cboTuaSach = new System.Windows.Forms.ComboBox();
            this.txtMaSach = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtId = new MaterialSkin.Controls.MaterialTextBox2();
            this.btnLuu = new MaterialSkin.Controls.MaterialButton();
            this.btnBoQua = new MaterialSkin.Controls.MaterialButton();
            this.panelGrid = new System.Windows.Forms.Panel();
            this.btnXemCuonSach = new MaterialSkin.Controls.MaterialButton(); // <<< Khởi tạo nút mới
            this.btnThoat = new MaterialSkin.Controls.MaterialButton();
            this.btnThem = new MaterialSkin.Controls.MaterialButton();
            this.btnSua = new MaterialSkin.Controls.MaterialButton();
            this.btnXoa = new MaterialSkin.Controls.MaterialButton();
            this.dgvSach = new System.Windows.Forms.DataGridView();

            this.panelDetails.SuspendLayout();
            this.panelGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSach)).BeginInit();
            this.SuspendLayout();
            //
            // panelDetails
            //
            this.panelDetails.Controls.Add(this.txtNhaXb);
            this.panelDetails.Controls.Add(this.txtNamXb);
            this.panelDetails.Controls.Add(this.txtDonGia);
            this.panelDetails.Controls.Add(this.cboTuaSach);
            this.panelDetails.Controls.Add(this.txtMaSach);
            this.panelDetails.Controls.Add(this.txtId);
            this.panelDetails.Controls.Add(this.btnLuu);
            this.panelDetails.Controls.Add(this.btnBoQua);
            this.panelDetails.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelDetails.Location = new System.Drawing.Point(10, 10); // Vị trí sau Padding UserControl
            this.panelDetails.Margin = new Padding(3);
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Padding = new Padding(10);
            this.panelDetails.Size = new System.Drawing.Size(680, 150); // Kích thước sau Padding
            this.panelDetails.TabIndex = 0;
            //
            // txtNhaXb
            //
            this.txtNhaXb.AnimateReadOnly = false;
            this.txtNhaXb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtNhaXb.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtNhaXb.Depth = 0;
            this.txtNhaXb.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtNhaXb.HideSelection = true;
            this.txtNhaXb.Hint = "Nhà XB";
            this.txtNhaXb.LeadingIcon = null;
            this.txtNhaXb.Location = new Point(283, 74);
            this.txtNhaXb.Margin = new Padding(3);
            this.txtNhaXb.MaxLength = 100;
            this.txtNhaXb.MouseState = MaterialSkin.MouseState.OUT;
            this.txtNhaXb.Name = "txtNhaXb";
            this.txtNhaXb.PasswordChar = '\0';
            this.txtNhaXb.PrefixSuffixText = null;
            this.txtNhaXb.ReadOnly = false;
            this.txtNhaXb.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtNhaXb.SelectedText = "";
            this.txtNhaXb.SelectionLength = 0;
            this.txtNhaXb.SelectionStart = 0;
            this.txtNhaXb.ShortcutsEnabled = true;
            this.txtNhaXb.Size = new System.Drawing.Size(270, 48);
            this.txtNhaXb.TabIndex = 5; // TabIndex cho Nhà XB
            this.txtNhaXb.TabStop = true;
            this.txtNhaXb.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtNhaXb.TrailingIcon = null;
            this.txtNhaXb.UseSystemPasswordChar = false;
            this.txtNhaXb.UseTallSize = false; // Sử dụng UseTallSize = false để có chiều cao chuẩn
            //
            // txtNamXb
            //
            this.txtNamXb.AnimateReadOnly = false;
            this.txtNamXb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtNamXb.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtNamXb.Depth = 0;
            this.txtNamXb.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtNamXb.HideSelection = true;
            this.txtNamXb.Hint = "Năm XB";
            this.txtNamXb.LeadingIcon = null;
            this.txtNamXb.Location = new Point(173, 74);
            this.txtNamXb.Margin = new Padding(3);
            this.txtNamXb.MaxLength = 4;
            this.txtNamXb.MouseState = MaterialSkin.MouseState.OUT;
            this.txtNamXb.Name = "txtNamXb";
            this.txtNamXb.PasswordChar = '\0';
            this.txtNamXb.PrefixSuffixText = null;
            this.txtNamXb.ReadOnly = false;
            this.txtNamXb.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtNamXb.SelectedText = "";
            this.txtNamXb.SelectionLength = 0;
            this.txtNamXb.SelectionStart = 0;
            this.txtNamXb.ShortcutsEnabled = true;
            this.txtNamXb.Size = new System.Drawing.Size(100, 48);
            this.txtNamXb.TabIndex = 4; // TabIndex cho Năm XB
            this.txtNamXb.TabStop = true;
            this.txtNamXb.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtNamXb.TrailingIcon = null;
            this.txtNamXb.UseSystemPasswordChar = false;
            this.txtNamXb.UseTallSize = false; // Sử dụng UseTallSize = false
            //
            // txtDonGia
            //
            this.txtDonGia.AnimateReadOnly = false;
            this.txtDonGia.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtDonGia.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtDonGia.Depth = 0;
            this.txtDonGia.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtDonGia.HideSelection = true;
            this.txtDonGia.Hint = "Đơn Giá";
            this.txtDonGia.LeadingIcon = null;
            this.txtDonGia.Location = new Point(13, 74);
            this.txtDonGia.Margin = new Padding(3);
            this.txtDonGia.MaxLength = 32767; // Hoặc giới hạn phù hợp
            this.txtDonGia.MouseState = MaterialSkin.MouseState.OUT;
            this.txtDonGia.Name = "txtDonGia";
            this.txtDonGia.PasswordChar = '\0';
            this.txtDonGia.PrefixSuffixText = null;
            this.txtDonGia.ReadOnly = false;
            this.txtDonGia.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtDonGia.SelectedText = "";
            this.txtDonGia.SelectionLength = 0;
            this.txtDonGia.SelectionStart = 0;
            this.txtDonGia.ShortcutsEnabled = true;
            this.txtDonGia.Size = new System.Drawing.Size(150, 48);
            this.txtDonGia.TabIndex = 3; // TabIndex cho Đơn giá
            this.txtDonGia.TabStop = true;
            this.txtDonGia.TextAlign = System.Windows.Forms.HorizontalAlignment.Right; // Căn phải cho tiền tệ
            this.txtDonGia.TrailingIcon = null;
            this.txtDonGia.UseSystemPasswordChar = false;
            this.txtDonGia.UseTallSize = false; // Sử dụng UseTallSize = false
            //
            // cboTuaSach
            //
            this.cboTuaSach.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboTuaSach.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboTuaSach.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; // Hoặc DropDown nếu muốn cho phép nhập
            this.cboTuaSach.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F); // Font phù hợp
            this.cboTuaSach.FormattingEnabled = true;
            this.cboTuaSach.Location = new Point(243, 23); // Điều chỉnh vị trí Y cho phù hợp với TextBox
            this.cboTuaSach.Margin = new Padding(3);
            this.cboTuaSach.Name = "cboTuaSach";
            this.cboTuaSach.Size = new System.Drawing.Size(310, 28); // Điều chỉnh Size
            this.cboTuaSach.TabIndex = 2; // TabIndex cho Tựa sách
            //
            // txtMaSach
            //
            this.txtMaSach.AnimateReadOnly = false;
            this.txtMaSach.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtMaSach.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtMaSach.Depth = 0;
            this.txtMaSach.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtMaSach.HideSelection = true;
            this.txtMaSach.Hint = "Mã Sách";
            this.txtMaSach.LeadingIcon = null;
            this.txtMaSach.Location = new Point(103, 13);
            this.txtMaSach.Margin = new Padding(3);
            this.txtMaSach.MaxLength = 6; // Hoặc độ dài mã sách quy định
            this.txtMaSach.MouseState = MaterialSkin.MouseState.OUT;
            this.txtMaSach.Name = "txtMaSach";
            this.txtMaSach.PasswordChar = '\0';
            this.txtMaSach.PrefixSuffixText = null;
            this.txtMaSach.ReadOnly = false; // Sẽ được quản lý bởi code-behind (chỉ bật khi thêm)
            this.txtMaSach.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtMaSach.SelectedText = "";
            this.txtMaSach.SelectionLength = 0;
            this.txtMaSach.SelectionStart = 0;
            this.txtMaSach.ShortcutsEnabled = true;
            this.txtMaSach.Size = new System.Drawing.Size(130, 48);
            this.txtMaSach.TabIndex = 1; // TabIndex cho Mã sách
            this.txtMaSach.TabStop = true;
            this.txtMaSach.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtMaSach.TrailingIcon = null;
            this.txtMaSach.UseSystemPasswordChar = false;
            this.txtMaSach.UseTallSize = false; // Sử dụng UseTallSize = false
            //
            // txtId
            //
            this.txtId.AnimateReadOnly = true; // Đặt là true vì luôn ReadOnly
            this.txtId.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtId.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtId.Depth = 0;
            this.txtId.Enabled = false; // Luôn bị vô hiệu hóa
            this.txtId.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtId.HideSelection = true;
            this.txtId.Hint = "ID";
            this.txtId.LeadingIcon = null;
            this.txtId.Location = new Point(13, 13);
            this.txtId.Margin = new Padding(3);
            this.txtId.MaxLength = 32767;
            this.txtId.MouseState = MaterialSkin.MouseState.OUT;
            this.txtId.Name = "txtId";
            this.txtId.PasswordChar = '\0';
            this.txtId.PrefixSuffixText = null;
            this.txtId.ReadOnly = true; // Luôn chỉ đọc
            this.txtId.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtId.SelectedText = "";
            this.txtId.SelectionLength = 0;
            this.txtId.SelectionStart = 0;
            this.txtId.ShortcutsEnabled = true;
            this.txtId.Size = new System.Drawing.Size(80, 48);
            this.txtId.TabIndex = 0; // TabIndex đầu tiên nhưng bị disable
            this.txtId.TabStop = false; // Không cho phép focus bằng Tab
            this.txtId.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtId.TrailingIcon = null;
            this.txtId.UseSystemPasswordChar = false;
            this.txtId.UseTallSize = false; // Sử dụng UseTallSize = false
            //
            // btnLuu
            //
            this.btnLuu.AutoSize = false;
            this.btnLuu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLuu.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnLuu.Depth = 0;
            this.btnLuu.HighEmphasis = true; // Nút chính
            this.btnLuu.Icon = null;
            this.btnLuu.Location = new Point(567, 19); // Vị trí nút Lưu
            this.btnLuu.Margin = new Padding(4, 6, 4, 6);
            this.btnLuu.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnLuu.Size = new System.Drawing.Size(100, 36);
            this.btnLuu.TabIndex = 6; // TabIndex cho nút Lưu
            this.btnLuu.Text = "Lưu";
            this.btnLuu.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained; // Kiểu Contained
            this.btnLuu.UseAccentColor = false; // Hoặc true nếu muốn màu Accent
            this.btnLuu.UseVisualStyleBackColor = true;
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            //
            // btnBoQua
            //
            this.btnBoQua.AutoSize = false;
            this.btnBoQua.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnBoQua.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnBoQua.Depth = 0;
            this.btnBoQua.HighEmphasis = false; // Nút phụ
            this.btnBoQua.Icon = null;
            this.btnBoQua.Location = new Point(567, 75); // Vị trí nút Bỏ qua
            this.btnBoQua.Margin = new Padding(4, 6, 4, 6);
            this.btnBoQua.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnBoQua.Name = "btnBoQua";
            this.btnBoQua.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnBoQua.Size = new System.Drawing.Size(100, 36);
            this.btnBoQua.TabIndex = 7; // TabIndex cho nút Bỏ qua
            this.btnBoQua.Text = "Bỏ qua";
            this.btnBoQua.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined; // Kiểu Outlined
            this.btnBoQua.UseAccentColor = false;
            this.btnBoQua.UseVisualStyleBackColor = true;
            this.btnBoQua.Click += new System.EventHandler(this.btnBoQua_Click);
            //
            // panelGrid
            //
            this.panelGrid.Controls.Add(this.btnXemCuonSach); // <<< Thêm nút Xem Cuốn Sách vào panel
            this.panelGrid.Controls.Add(this.btnThoat);
            this.panelGrid.Controls.Add(this.btnThem);
            this.panelGrid.Controls.Add(this.btnSua);
            this.panelGrid.Controls.Add(this.btnXoa);
            this.panelGrid.Controls.Add(this.dgvSach);
            this.panelGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGrid.Location = new Point(10, 160); // Vị trí sau panelDetails và Padding UserControl
            this.panelGrid.Margin = new Padding(3);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Padding = new Padding(10);
            this.panelGrid.Size = new System.Drawing.Size(680, 280); // Kích thước sau Padding UserControl
            this.panelGrid.TabIndex = 1;
            //
            // btnXemCuonSach
            //
            this.btnXemCuonSach.AutoSize = false;
            this.btnXemCuonSach.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnXemCuonSach.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnXemCuonSach.Depth = 0;
            this.btnXemCuonSach.HighEmphasis = false; // Kiểu nút phụ
            this.btnXemCuonSach.Icon = null; // Có thể thêm icon nếu muốn
            this.btnXemCuonSach.Location = new System.Drawing.Point(278, 6); // Vị trí sau nút Xóa
            this.btnXemCuonSach.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnXemCuonSach.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnXemCuonSach.Name = "btnXemCuonSach";
            this.btnXemCuonSach.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnXemCuonSach.Size = new System.Drawing.Size(140, 36); // Kích thước nút
            this.btnXemCuonSach.TabIndex = 3; // TabIndex sau nút Xóa
            this.btnXemCuonSach.Text = "Xem Cuốn Sách";
            this.btnXemCuonSach.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined; // Kiểu Outlined
            this.btnXemCuonSach.UseAccentColor = false;
            this.btnXemCuonSach.UseVisualStyleBackColor = true;
            this.btnXemCuonSach.Click += new System.EventHandler(this.btnXemCuonSach_Click); // Gán sự kiện
            //
            // btnThoat
            //
            this.btnThoat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right))); // Neo góc trên phải
            this.btnThoat.AutoSize = false;
            this.btnThoat.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThoat.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnThoat.Depth = 0;
            this.btnThoat.HighEmphasis = false; // Kiểu nút phụ
            this.btnThoat.Icon = null;
            this.btnThoat.Location = new Point(597, 6); // Vị trí nút Thoát
            this.btnThoat.Margin = new Padding(4, 6, 4, 6);
            this.btnThoat.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnThoat.Size = new System.Drawing.Size(70, 36); // Kích thước nút Thoát
            this.btnThoat.TabIndex = 4; // TabIndex sau nút Xem Cuốn Sách
            this.btnThoat.Text = "Thoát";
            this.btnThoat.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text; // Kiểu Text
            this.btnThoat.UseAccentColor = false;
            this.btnThoat.UseVisualStyleBackColor = true;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            //
            // btnThem
            //
            this.btnThem.AutoSize = false;
            this.btnThem.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThem.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnThem.Depth = 0;
            this.btnThem.HighEmphasis = true; // Nút chính
            this.btnThem.Icon = null;
            this.btnThem.Location = new Point(14, 6); // Vị trí nút Thêm
            this.btnThem.Margin = new Padding(4, 6, 4, 6);
            this.btnThem.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnThem.Name = "btnThem";
            this.btnThem.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnThem.Size = new System.Drawing.Size(80, 36);
            this.btnThem.TabIndex = 0; // TabIndex đầu tiên trong panelGrid
            this.btnThem.Text = "Thêm";
            this.btnThem.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnThem.UseAccentColor = false;
            this.btnThem.UseVisualStyleBackColor = true;
            this.btnThem.Click += new System.EventHandler(this.btnThem_Click);
            //
            // btnSua
            //
            this.btnSua.AutoSize = false;
            this.btnSua.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSua.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnSua.Depth = 0;
            this.btnSua.HighEmphasis = false; // Nút phụ
            this.btnSua.Icon = null;
            this.btnSua.Location = new Point(102, 6); // Vị trí nút Sửa
            this.btnSua.Margin = new Padding(4, 6, 4, 6);
            this.btnSua.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSua.Name = "btnSua";
            this.btnSua.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnSua.Size = new System.Drawing.Size(80, 36);
            this.btnSua.TabIndex = 1; // TabIndex sau nút Thêm
            this.btnSua.Text = "Sửa";
            this.btnSua.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnSua.UseAccentColor = false;
            this.btnSua.UseVisualStyleBackColor = true;
            this.btnSua.Click += new System.EventHandler(this.btnSua_Click);
            //
            // btnXoa
            //
            this.btnXoa.AutoSize = false;
            this.btnXoa.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnXoa.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnXoa.Depth = 0;
            this.btnXoa.HighEmphasis = false; // Nút phụ
            this.btnXoa.Icon = null;
            this.btnXoa.Location = new Point(190, 6); // Vị trí nút Xóa
            this.btnXoa.Margin = new Padding(4, 6, 4, 6);
            this.btnXoa.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnXoa.Size = new System.Drawing.Size(80, 36);
            this.btnXoa.TabIndex = 2; // TabIndex sau nút Sửa
            this.btnXoa.Text = "Xóa";
            this.btnXoa.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnXoa.UseAccentColor = true; // Màu đỏ cảnh báo
            this.btnXoa.UseVisualStyleBackColor = true;
            this.btnXoa.Click += new System.EventHandler(this.btnXoa_Click);
            //
            // dgvSach
            //
            this.dgvSach.AllowUserToAddRows = false;
            this.dgvSach.AllowUserToDeleteRows = false;
            this.dgvSach.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))); // Neo vào các cạnh
            this.dgvSach.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSach.Location = new Point(13, 50); // Vị trí sau các nút và padding
            this.dgvSach.Margin = new Padding(3);
            this.dgvSach.MultiSelect = false;
            this.dgvSach.Name = "dgvSach";
            this.dgvSach.ReadOnly = true;
            this.dgvSach.RowHeadersWidth = 51;
            this.dgvSach.RowTemplate.Height = 24;
            this.dgvSach.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSach.Size = new System.Drawing.Size(654, 217); // Kích thước sau padding
            this.dgvSach.TabIndex = 5; // TabIndex cuối cùng trong panelGrid
            this.dgvSach.SelectionChanged += new System.EventHandler(this.dgvSach_SelectionChanged);
            this.dgvSach.DoubleClick += new System.EventHandler(this.dgvSach_DoubleClick);
            //
            // ucQuanLySach
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.panelDetails);
            this.Margin = new Padding(3);
            this.Name = "ucQuanLySach";
            this.Padding = new Padding(10);
            this.Size = new System.Drawing.Size(700, 450);
            this.Load += new System.EventHandler(this.ucQuanLySach_Load);
            this.panelDetails.ResumeLayout(false);
            this.panelDetails.PerformLayout();
            this.panelGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSach)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
