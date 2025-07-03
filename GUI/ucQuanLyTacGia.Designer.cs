// Project/Namespace: GUI

// --- USING ---
using MaterialSkin.Controls;
using System.Drawing;
using System.Windows.Forms;

namespace GUI
{
    partial class ucQuanLyTacGia
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.materialCard1 = new MaterialSkin.Controls.MaterialCard();
            this.materialCardDetails = new MaterialSkin.Controls.MaterialCard();
            // *** KHAI BÁO btnKhoiPhuc ***
            this.btnKhoiPhuc = new MaterialSkin.Controls.MaterialButton();
            this.btnBoQua = new MaterialSkin.Controls.MaterialButton();
            this.btnLuu = new MaterialSkin.Controls.MaterialButton();
            this.txtTenTacGia = new MaterialSkin.Controls.MaterialTextBox2();
            this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
            this.txtMaTacGia = new MaterialSkin.Controls.MaterialTextBox2();
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            this.txtId = new MaterialSkin.Controls.MaterialTextBox2();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.panelGrid = new System.Windows.Forms.Panel();
            // *** KHAI BÁO chkHienThiDaXoa ***
            this.chkHienThiDaXoa = new MaterialSkin.Controls.MaterialCheckbox();
            this.txtTimKiem = new MaterialSkin.Controls.MaterialTextBox2();
            this.btnXoa = new MaterialSkin.Controls.MaterialButton();
            this.btnSua = new MaterialSkin.Controls.MaterialButton();
            this.btnThem = new MaterialSkin.Controls.MaterialButton();
            this.dgvTacGia = new System.Windows.Forms.DataGridView();
            this.materialCard1.SuspendLayout();
            this.materialCardDetails.SuspendLayout();
            this.panelGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTacGia)).BeginInit();
            this.SuspendLayout();
            //
            // materialCard1
            //
            this.materialCard1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.materialCard1.Controls.Add(this.materialCardDetails);
            this.materialCard1.Controls.Add(this.panelGrid);
            this.materialCard1.Depth = 0;
            this.materialCard1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialCard1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialCard1.Location = new System.Drawing.Point(10, 10);
            this.materialCard1.Margin = new System.Windows.Forms.Padding(3);
            this.materialCard1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialCard1.Name = "materialCard1";
            this.materialCard1.Padding = new System.Windows.Forms.Padding(10);
            this.materialCard1.Size = new System.Drawing.Size(780, 580);
            this.materialCard1.TabIndex = 0;
            //
            // materialCardDetails
            //
            this.materialCardDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialCardDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // *** THÊM btnKhoiPhuc vào Controls ***
            this.materialCardDetails.Controls.Add(this.btnKhoiPhuc);
            this.materialCardDetails.Controls.Add(this.btnBoQua);
            this.materialCardDetails.Controls.Add(this.btnLuu);
            this.materialCardDetails.Controls.Add(this.txtTenTacGia);
            this.materialCardDetails.Controls.Add(this.materialLabel3);
            this.materialCardDetails.Controls.Add(this.txtMaTacGia);
            this.materialCardDetails.Controls.Add(this.materialLabel2);
            this.materialCardDetails.Controls.Add(this.txtId);
            this.materialCardDetails.Controls.Add(this.materialLabel1);
            this.materialCardDetails.Depth = 0;
            this.materialCardDetails.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialCardDetails.Location = new System.Drawing.Point(13, 417);
            this.materialCardDetails.Margin = new System.Windows.Forms.Padding(3);
            this.materialCardDetails.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialCardDetails.Name = "materialCardDetails";
            this.materialCardDetails.Padding = new System.Windows.Forms.Padding(10);
            this.materialCardDetails.Size = new System.Drawing.Size(754, 150);
            this.materialCardDetails.TabIndex = 1;
            //
            // btnKhoiPhuc
            //
            this.btnKhoiPhuc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnKhoiPhuc.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnKhoiPhuc.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnKhoiPhuc.Depth = 0;
            this.btnKhoiPhuc.Enabled = false; // Bắt đầu bị vô hiệu hóa
            this.btnKhoiPhuc.HighEmphasis = true;
            this.btnKhoiPhuc.Icon = null;
            this.btnKhoiPhuc.Location = new System.Drawing.Point(477, 101); // Đặt cạnh nút Lưu/Bỏ qua
            this.btnKhoiPhuc.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnKhoiPhuc.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnKhoiPhuc.Name = "btnKhoiPhuc";
            this.btnKhoiPhuc.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnKhoiPhuc.Size = new System.Drawing.Size(99, 36);
            this.btnKhoiPhuc.TabIndex = 10; // TabIndex sau btnLuu/btnBoQua
            this.btnKhoiPhuc.Text = "Khôi phục";
            this.btnKhoiPhuc.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnKhoiPhuc.UseAccentColor = true; // Màu nhấn để nổi bật
            this.btnKhoiPhuc.UseVisualStyleBackColor = true;
            this.btnKhoiPhuc.Click += new System.EventHandler(this.btnKhoiPhuc_Click); // Gắn sự kiện Click
            //
            // btnBoQua
            //
            this.btnBoQua.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBoQua.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnBoQua.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnBoQua.Depth = 0;
            this.btnBoQua.HighEmphasis = true;
            this.btnBoQua.Icon = null;
            this.btnBoQua.Location = new System.Drawing.Point(658, 101);
            this.btnBoQua.Margin = new System.Windows.Forms.Padding(3);
            this.btnBoQua.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnBoQua.Name = "btnBoQua";
            this.btnBoQua.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnBoQua.Size = new System.Drawing.Size(83, 36);
            this.btnBoQua.TabIndex = 9;
            this.btnBoQua.Text = "Bỏ qua";
            this.btnBoQua.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnBoQua.UseAccentColor = false;
            this.btnBoQua.UseVisualStyleBackColor = true;
            this.btnBoQua.Click += new System.EventHandler(this.btnBoQua_Click);
            //
            // btnLuu
            //
            this.btnLuu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLuu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLuu.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnLuu.Depth = 0;
            this.btnLuu.HighEmphasis = true;
            this.btnLuu.Icon = null;
            this.btnLuu.Location = new System.Drawing.Point(586, 101);
            this.btnLuu.Margin = new System.Windows.Forms.Padding(3);
            this.btnLuu.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnLuu.Size = new System.Drawing.Size(64, 36);
            this.btnLuu.TabIndex = 8;
            this.btnLuu.Text = "Lưu";
            this.btnLuu.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnLuu.UseAccentColor = false;
            this.btnLuu.UseVisualStyleBackColor = true;
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            //
            // txtTenTacGia
            //
            this.txtTenTacGia.AnimateReadOnly = false;
            this.txtTenTacGia.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtTenTacGia.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtTenTacGia.Depth = 0;
            this.txtTenTacGia.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtTenTacGia.HideSelection = true;
            this.txtTenTacGia.Hint = "Nhập tên tác giả";
            this.txtTenTacGia.LeadingIcon = null;
            this.txtTenTacGia.Location = new System.Drawing.Point(269, 40);
            this.txtTenTacGia.Margin = new System.Windows.Forms.Padding(3);
            this.txtTenTacGia.MaxLength = 32767;
            this.txtTenTacGia.MouseState = MaterialSkin.MouseState.OUT;
            this.txtTenTacGia.Name = "txtTenTacGia";
            this.txtTenTacGia.PasswordChar = '\0';
            this.txtTenTacGia.PrefixSuffixText = null;
            this.txtTenTacGia.ReadOnly = false;
            this.txtTenTacGia.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtTenTacGia.SelectedText = "";
            this.txtTenTacGia.SelectionLength = 0;
            this.txtTenTacGia.SelectionStart = 0;
            this.txtTenTacGia.ShortcutsEnabled = true;
            this.txtTenTacGia.Size = new System.Drawing.Size(300, 48);
            this.txtTenTacGia.TabIndex = 7; // Giữ nguyên TabIndex
            this.txtTenTacGia.TabStop = true;
            this.txtTenTacGia.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtTenTacGia.TrailingIcon = null;
            this.txtTenTacGia.UseSystemPasswordChar = false;
            this.txtTenTacGia.UseTallSize = false;
            //
            // materialLabel3
            //
            this.materialLabel3.AutoSize = true;
            this.materialLabel3.Depth = 0;
            this.materialLabel3.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel3.Location = new System.Drawing.Point(266, 17);
            this.materialLabel3.Margin = new System.Windows.Forms.Padding(3);
            this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel3.Name = "materialLabel3";
            this.materialLabel3.Size = new System.Drawing.Size(86, 19);
            this.materialLabel3.TabIndex = 6;
            this.materialLabel3.Text = "Tên tác giả:";
            //
            // txtMaTacGia
            //
            this.txtMaTacGia.AnimateReadOnly = false;
            this.txtMaTacGia.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtMaTacGia.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtMaTacGia.Depth = 0;
            this.txtMaTacGia.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtMaTacGia.HideSelection = true;
            this.txtMaTacGia.Hint = "(Tự động tạo)";
            this.txtMaTacGia.LeadingIcon = null;
            this.txtMaTacGia.Location = new System.Drawing.Point(13, 102);
            this.txtMaTacGia.Margin = new System.Windows.Forms.Padding(3);
            this.txtMaTacGia.MaxLength = 50;
            this.txtMaTacGia.MouseState = MaterialSkin.MouseState.OUT;
            this.txtMaTacGia.Name = "txtMaTacGia";
            this.txtMaTacGia.PasswordChar = '\0';
            this.txtMaTacGia.PrefixSuffixText = null;
            this.txtMaTacGia.ReadOnly = true;
            this.txtMaTacGia.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtMaTacGia.SelectedText = "";
            this.txtMaTacGia.SelectionLength = 0;
            this.txtMaTacGia.SelectionStart = 0;
            this.txtMaTacGia.ShortcutsEnabled = true;
            this.txtMaTacGia.Size = new System.Drawing.Size(210, 48);
            this.txtMaTacGia.TabIndex = 5; // Giữ nguyên TabIndex
            this.txtMaTacGia.TabStop = false;
            this.txtMaTacGia.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtMaTacGia.TrailingIcon = null;
            this.txtMaTacGia.UseSystemPasswordChar = false;
            this.txtMaTacGia.UseTallSize = false;
            //
            // materialLabel2
            //
            this.materialLabel2.AutoSize = true;
            this.materialLabel2.Depth = 0;
            this.materialLabel2.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel2.Location = new System.Drawing.Point(10, 79);
            this.materialLabel2.Margin = new System.Windows.Forms.Padding(3);
            this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel2.Name = "materialLabel2";
            this.materialLabel2.Size = new System.Drawing.Size(81, 19);
            this.materialLabel2.TabIndex = 4;
            this.materialLabel2.Text = "Mã tác giả:";
            //
            // txtId
            //
            this.txtId.AnimateReadOnly = true;
            this.txtId.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtId.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtId.Depth = 0;
            this.txtId.Enabled = false;
            this.txtId.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtId.HideSelection = true;
            this.txtId.Hint = "ID";
            this.txtId.LeadingIcon = null;
            this.txtId.Location = new System.Drawing.Point(13, 40);
            this.txtId.Margin = new System.Windows.Forms.Padding(3);
            this.txtId.MaxLength = 32767;
            this.txtId.MouseState = MaterialSkin.MouseState.OUT;
            this.txtId.Name = "txtId";
            this.txtId.PasswordChar = '\0';
            this.txtId.PrefixSuffixText = null;
            this.txtId.ReadOnly = true;
            this.txtId.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtId.SelectedText = "";
            this.txtId.SelectionLength = 0;
            this.txtId.SelectionStart = 0;
            this.txtId.ShortcutsEnabled = true;
            this.txtId.Size = new System.Drawing.Size(100, 48);
            this.txtId.TabIndex = 3; // Giữ nguyên TabIndex
            this.txtId.TabStop = false;
            this.txtId.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtId.TrailingIcon = null;
            this.txtId.UseSystemPasswordChar = false;
            this.txtId.UseTallSize = false;
            //
            // materialLabel1
            //
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel1.Location = new System.Drawing.Point(10, 17);
            this.materialLabel1.Margin = new System.Windows.Forms.Padding(3);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(23, 19);
            this.materialLabel1.TabIndex = 2;
            this.materialLabel1.Text = "ID:";
            //
            // panelGrid
            //
            this.panelGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            // *** THÊM chkHienThiDaXoa vào Controls ***
            this.panelGrid.Controls.Add(this.chkHienThiDaXoa);
            this.panelGrid.Controls.Add(this.txtTimKiem);
            this.panelGrid.Controls.Add(this.btnXoa);
            this.panelGrid.Controls.Add(this.btnSua);
            this.panelGrid.Controls.Add(this.btnThem);
            this.panelGrid.Controls.Add(this.dgvTacGia);
            this.panelGrid.Location = new System.Drawing.Point(13, 13);
            this.panelGrid.Margin = new System.Windows.Forms.Padding(3);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Padding = new System.Windows.Forms.Padding(10);
            this.panelGrid.Size = new System.Drawing.Size(754, 388);
            this.panelGrid.TabIndex = 0;
            //
            // chkHienThiDaXoa
            //
            this.chkHienThiDaXoa.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkHienThiDaXoa.AutoSize = true;
            this.chkHienThiDaXoa.Depth = 0;
            this.chkHienThiDaXoa.Location = new System.Drawing.Point(586, 67); // Đặt dưới ô tìm kiếm, bên phải
            this.chkHienThiDaXoa.Margin = new System.Windows.Forms.Padding(0);
            this.chkHienThiDaXoa.MouseLocation = new System.Drawing.Point(-1, -1);
            this.chkHienThiDaXoa.MouseState = MaterialSkin.MouseState.HOVER;
            this.chkHienThiDaXoa.Name = "chkHienThiDaXoa";
            this.chkHienThiDaXoa.ReadOnly = false;
            this.chkHienThiDaXoa.Ripple = true;
            this.chkHienThiDaXoa.Size = new System.Drawing.Size(155, 37);
            this.chkHienThiDaXoa.TabIndex = 5; // TabIndex sau các nút Thêm/Sửa/Xóa
            this.chkHienThiDaXoa.Text = "Hiển thị đã xóa";
            this.chkHienThiDaXoa.UseVisualStyleBackColor = true;
            this.chkHienThiDaXoa.CheckedChanged += new System.EventHandler(this.chkHienThiDaXoa_CheckedChanged); // Gắn sự kiện
            //
            // txtTimKiem
            //
            this.txtTimKiem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTimKiem.AnimateReadOnly = false;
            this.txtTimKiem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtTimKiem.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtTimKiem.Depth = 0;
            this.txtTimKiem.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtTimKiem.HideSelection = true;
            this.txtTimKiem.Hint = "Tìm kiếm theo Mã hoặc Tên Tác Giả...";
            this.txtTimKiem.LeadingIcon = null;
            this.txtTimKiem.Location = new System.Drawing.Point(13, 13);
            this.txtTimKiem.Margin = new System.Windows.Forms.Padding(3);
            this.txtTimKiem.MaxLength = 100;
            this.txtTimKiem.MouseState = MaterialSkin.MouseState.OUT;
            this.txtTimKiem.Name = "txtTimKiem";
            this.txtTimKiem.PasswordChar = '\0';
            this.txtTimKiem.PrefixSuffixText = null;
            this.txtTimKiem.ReadOnly = false;
            this.txtTimKiem.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtTimKiem.SelectedText = "";
            this.txtTimKiem.SelectionLength = 0;
            this.txtTimKiem.SelectionStart = 0;
            this.txtTimKiem.ShortcutsEnabled = true;
            this.txtTimKiem.Size = new System.Drawing.Size(728, 48);
            this.txtTimKiem.TabIndex = 0; // TabIndex đầu tiên
            this.txtTimKiem.TabStop = true;
            this.txtTimKiem.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtTimKiem.TrailingIcon = null;
            this.txtTimKiem.UseSystemPasswordChar = false;
            this.txtTimKiem.UseTallSize = false;
            //
            // btnXoa
            //
            this.btnXoa.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnXoa.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnXoa.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnXoa.Depth = 0;
            this.btnXoa.HighEmphasis = true;
            this.btnXoa.Icon = null;
            this.btnXoa.Location = new System.Drawing.Point(190, 339);
            this.btnXoa.Margin = new System.Windows.Forms.Padding(3);
            this.btnXoa.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnXoa.Size = new System.Drawing.Size(64, 36);
            this.btnXoa.TabIndex = 4; // TabIndex sau btnSua
            this.btnXoa.Text = "Xóa"; // Giữ nguyên Text, logic sẽ là Soft Delete
            this.btnXoa.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnXoa.UseAccentColor = false;
            this.btnXoa.UseVisualStyleBackColor = true;
            this.btnXoa.Click += new System.EventHandler(this.btnXoa_Click);
            //
            // btnSua
            //
            this.btnSua.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSua.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSua.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnSua.Depth = 0;
            this.btnSua.HighEmphasis = true;
            this.btnSua.Icon = null;
            this.btnSua.Location = new System.Drawing.Point(118, 339);
            this.btnSua.Margin = new System.Windows.Forms.Padding(3);
            this.btnSua.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSua.Name = "btnSua";
            this.btnSua.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnSua.Size = new System.Drawing.Size(64, 36);
            this.btnSua.TabIndex = 3; // TabIndex sau btnThem
            this.btnSua.Text = "Sửa";
            this.btnSua.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnSua.UseAccentColor = false;
            this.btnSua.UseVisualStyleBackColor = true;
            this.btnSua.Click += new System.EventHandler(this.btnSua_Click);
            //
            // btnThem
            //
            this.btnThem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnThem.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThem.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnThem.Depth = 0;
            this.btnThem.HighEmphasis = true;
            this.btnThem.Icon = null;
            this.btnThem.Location = new System.Drawing.Point(21, 339);
            this.btnThem.Margin = new System.Windows.Forms.Padding(3);
            this.btnThem.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnThem.Name = "btnThem";
            this.btnThem.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnThem.Size = new System.Drawing.Size(76, 36);
            this.btnThem.TabIndex = 2; // TabIndex sau dgvTacGia
            this.btnThem.Text = "Thêm";
            this.btnThem.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnThem.UseAccentColor = false;
            this.btnThem.UseVisualStyleBackColor = true;
            this.btnThem.Click += new System.EventHandler(this.btnThem_Click);
            //
            // dgvTacGia
            //
            this.dgvTacGia.AllowUserToAddRows = false;
            this.dgvTacGia.AllowUserToDeleteRows = false;
            this.dgvTacGia.AllowUserToResizeRows = false;
            this.dgvTacGia.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTacGia.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTacGia.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.dgvTacGia.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTacGia.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvTacGia.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(81)))), ((int)(((byte)(181)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Roboto Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(81)))), ((int)(((byte)(181)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTacGia.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTacGia.ColumnHeadersHeight = 40;
            this.dgvTacGia.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Roboto", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTacGia.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvTacGia.EnableHeadersVisualStyles = false;
            this.dgvTacGia.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvTacGia.Location = new System.Drawing.Point(13, 110); // Điều chỉnh Y để có chỗ cho checkbox
            this.dgvTacGia.Margin = new System.Windows.Forms.Padding(3);
            this.dgvTacGia.Name = "dgvTacGia";
            this.dgvTacGia.ReadOnly = true;
            this.dgvTacGia.RowHeadersVisible = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.dgvTacGia.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvTacGia.RowTemplate.Height = 35;
            this.dgvTacGia.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTacGia.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvTacGia.Size = new System.Drawing.Size(728, 217); // Điều chỉnh Height
            this.dgvTacGia.TabIndex = 1; // TabIndex sau txtTimKiem
            this.dgvTacGia.SelectionChanged += new System.EventHandler(this.dgvTacGia_SelectionChanged);
            this.dgvTacGia.DoubleClick += new System.EventHandler(this.dgvTacGia_DoubleClick);
            //
            // ucQuanLyTacGia
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.materialCard1);
            this.Font = new System.Drawing.Font("Roboto", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(3);
            this.Name = "ucQuanLyTacGia";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(800, 600);
            this.Load += new System.EventHandler(this.ucQuanLyTacGia_Load);
            this.materialCard1.ResumeLayout(false);
            this.materialCardDetails.ResumeLayout(false);
            this.materialCardDetails.PerformLayout();
            this.panelGrid.ResumeLayout(false);
            this.panelGrid.PerformLayout(); // Thêm dòng này để áp dụng AutoSize cho Checkbox
            ((System.ComponentModel.ISupportInitialize)(this.dgvTacGia)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        // Declare controls as public or protected so they can be accessed by the code-behind file (ucQuanLyTacGia.cs)
        private MaterialSkin.Controls.MaterialCard materialCard1;
        private System.Windows.Forms.Panel panelGrid;
        private System.Windows.Forms.DataGridView dgvTacGia;
        private MaterialSkin.Controls.MaterialButton btnXoa;
        private MaterialSkin.Controls.MaterialButton btnSua;
        private MaterialSkin.Controls.MaterialButton btnThem;
        private MaterialSkin.Controls.MaterialCard materialCardDetails;
        private MaterialSkin.Controls.MaterialButton btnBoQua;
        private MaterialSkin.Controls.MaterialButton btnLuu;
        private MaterialSkin.Controls.MaterialTextBox2 txtId;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialTextBox2 txtTenTacGia;
        private MaterialSkin.Controls.MaterialLabel materialLabel3;
        private MaterialSkin.Controls.MaterialTextBox2 txtMaTacGia;
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
        private MaterialTextBox2 txtTimKiem;
        // *** THÊM KHAI BÁO CHO CONTROLS MỚI ***
        private MaterialCheckbox chkHienThiDaXoa;
        private MaterialButton btnKhoiPhuc;
    }
}
