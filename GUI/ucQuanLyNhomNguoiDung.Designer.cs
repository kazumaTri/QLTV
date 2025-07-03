// GUI/ucQuanLyNhomNguoiDung.Designer.cs
// Phiên bản đã được sửa đổi để tách các nút ra panel riêng và đồng nhất Padding/Margin
// *** ĐÃ THÊM CheckedListBox để hiển thị Chức Năng ***

using System; // Cần cho EventArgs
using System.Windows.Forms; // Cần cho Panel, DataGridView, DockStyle, CheckedListBox, etc.
using MaterialSkin.Controls; // Cần cho Material controls
using System.Drawing; // Cần cho Point, SizeF, Size
using System.ComponentModel; // Cần cho IContainer

namespace GUI
{
    partial class ucQuanLyNhomNguoiDung
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
            panelDetails = new Panel();
            lblChucNang = new MaterialLabel();
            clbChucNang = new CheckedListBox();
            btnBoQua = new MaterialButton();
            btnLuu = new MaterialButton();
            txtTenNhom = new MaterialTextBox2();
            txtMaNhom = new MaterialTextBox2();
            txtId = new MaterialTextBox2();
            panelGrid = new Panel();
            dgvNhomNguoiDung = new DataGridView();
            panelButtons = new Panel();
            btnThem = new MaterialButton();
            btnSua = new MaterialButton();
            btnXoa = new MaterialButton();
            panelDetails.SuspendLayout();
            panelGrid.SuspendLayout();
            ((ISupportInitialize)dgvNhomNguoiDung).BeginInit();
            panelButtons.SuspendLayout();
            SuspendLayout();
            // 
            // panelDetails
            // 
            panelDetails.Controls.Add(lblChucNang);
            panelDetails.Controls.Add(clbChucNang);
            panelDetails.Controls.Add(txtTenNhom);
            panelDetails.Controls.Add(txtMaNhom);
            panelDetails.Controls.Add(txtId);
            panelDetails.Dock = DockStyle.Top;
            panelDetails.Location = new Point(10, 12);
            panelDetails.Margin = new Padding(3, 4, 3, 4);
            panelDetails.Name = "panelDetails";
            panelDetails.Padding = new Padding(10, 12, 10, 12);
            panelDetails.Size = new Size(830, 312);
            panelDetails.TabIndex = 0;
            // 
            // lblChucNang
            // 
            lblChucNang.AutoSize = true;
            lblChucNang.Depth = 0;
            lblChucNang.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblChucNang.Location = new Point(13, 94);
            lblChucNang.Margin = new Padding(3, 4, 3, 4);
            lblChucNang.MouseState = MaterialSkin.MouseState.HOVER;
            lblChucNang.Name = "lblChucNang";
            lblChucNang.Size = new Size(133, 19);
            lblChucNang.TabIndex = 5;
            lblChucNang.Text = "Quyền Chức Năng:";
            // 
            // clbChucNang
            // 
            clbChucNang.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            clbChucNang.CheckOnClick = true;
            clbChucNang.Enabled = false;
            clbChucNang.FormattingEnabled = true;
            clbChucNang.Location = new Point(13, 125);
            clbChucNang.Margin = new Padding(3, 4, 3, 4);
            clbChucNang.Name = "clbChucNang";
            clbChucNang.Size = new Size(804, 114);
            clbChucNang.TabIndex = 6;
            // 
            // btnBoQua
            // 
            btnBoQua.AutoSize = false;
            btnBoQua.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnBoQua.Density = MaterialButton.MaterialButtonDensity.Default;
            btnBoQua.Depth = 0;
            btnBoQua.HighEmphasis = false;
            btnBoQua.Icon = null;
            btnBoQua.Location = new Point(421, 8);
            btnBoQua.Margin = new Padding(3, 4, 3, 4);
            btnBoQua.MouseState = MaterialSkin.MouseState.HOVER;
            btnBoQua.Name = "btnBoQua";
            btnBoQua.NoAccentTextColor = Color.Empty;
            btnBoQua.Size = new Size(100, 45);
            btnBoQua.TabIndex = 4;
            btnBoQua.Text = "Bỏ qua";
            btnBoQua.Type = MaterialButton.MaterialButtonType.Outlined;
            btnBoQua.UseAccentColor = false;
            btnBoQua.UseVisualStyleBackColor = true;
            btnBoQua.Click += btnBoQua_Click;
            // 
            // btnLuu
            // 
            btnLuu.AutoSize = false;
            btnLuu.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnLuu.Density = MaterialButton.MaterialButtonDensity.Default;
            btnLuu.Depth = 0;
            btnLuu.HighEmphasis = true;
            btnLuu.Icon = null;
            btnLuu.Location = new Point(298, 9);
            btnLuu.Margin = new Padding(3, 4, 3, 4);
            btnLuu.MouseState = MaterialSkin.MouseState.HOVER;
            btnLuu.Name = "btnLuu";
            btnLuu.NoAccentTextColor = Color.Empty;
            btnLuu.Size = new Size(100, 45);
            btnLuu.TabIndex = 3;
            btnLuu.Text = "Lưu";
            btnLuu.Type = MaterialButton.MaterialButtonType.Contained;
            btnLuu.UseAccentColor = false;
            btnLuu.UseVisualStyleBackColor = true;
            btnLuu.Click += btnLuu_Click;
            // 
            // txtTenNhom
            // 
            txtTenNhom.AnimateReadOnly = false;
            txtTenNhom.BackgroundImageLayout = ImageLayout.None;
            txtTenNhom.CharacterCasing = CharacterCasing.Normal;
            txtTenNhom.Depth = 0;
            txtTenNhom.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtTenNhom.HideSelection = true;
            txtTenNhom.Hint = "Tên Nhóm Người Dùng";
            txtTenNhom.LeadingIcon = null;
            txtTenNhom.Location = new Point(323, 16);
            txtTenNhom.Margin = new Padding(3, 4, 3, 4);
            txtTenNhom.MaxLength = 100;
            txtTenNhom.MouseState = MaterialSkin.MouseState.OUT;
            txtTenNhom.Name = "txtTenNhom";
            txtTenNhom.PasswordChar = '\0';
            txtTenNhom.PrefixSuffixText = null;
            txtTenNhom.ReadOnly = false;
            txtTenNhom.RightToLeft = RightToLeft.No;
            txtTenNhom.SelectedText = "";
            txtTenNhom.SelectionLength = 0;
            txtTenNhom.SelectionStart = 0;
            txtTenNhom.ShortcutsEnabled = true;
            txtTenNhom.Size = new Size(350, 36);
            txtTenNhom.TabIndex = 2;
            txtTenNhom.TextAlign = HorizontalAlignment.Left;
            txtTenNhom.TrailingIcon = null;
            txtTenNhom.UseSystemPasswordChar = false;
            txtTenNhom.UseTallSize = false;
            // 
            // txtMaNhom
            // 
            txtMaNhom.AnimateReadOnly = false;
            txtMaNhom.BackgroundImageLayout = ImageLayout.None;
            txtMaNhom.CharacterCasing = CharacterCasing.Normal;
            txtMaNhom.Depth = 0;
            txtMaNhom.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtMaNhom.HideSelection = true;
            txtMaNhom.Hint = "Mã Nhóm";
            txtMaNhom.LeadingIcon = null;
            txtMaNhom.Location = new Point(123, 16);
            txtMaNhom.Margin = new Padding(3, 4, 3, 4);
            txtMaNhom.MaxLength = 50;
            txtMaNhom.MouseState = MaterialSkin.MouseState.OUT;
            txtMaNhom.Name = "txtMaNhom";
            txtMaNhom.PasswordChar = '\0';
            txtMaNhom.PrefixSuffixText = null;
            txtMaNhom.ReadOnly = false;
            txtMaNhom.RightToLeft = RightToLeft.No;
            txtMaNhom.SelectedText = "";
            txtMaNhom.SelectionLength = 0;
            txtMaNhom.SelectionStart = 0;
            txtMaNhom.ShortcutsEnabled = true;
            txtMaNhom.Size = new Size(180, 36);
            txtMaNhom.TabIndex = 1;
            txtMaNhom.TextAlign = HorizontalAlignment.Left;
            txtMaNhom.TrailingIcon = null;
            txtMaNhom.UseSystemPasswordChar = false;
            txtMaNhom.UseTallSize = false;
            // 
            // txtId
            // 
            txtId.AnimateReadOnly = false;
            txtId.BackgroundImageLayout = ImageLayout.None;
            txtId.CharacterCasing = CharacterCasing.Normal;
            txtId.Depth = 0;
            txtId.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtId.HideSelection = true;
            txtId.Hint = "ID";
            txtId.LeadingIcon = null;
            txtId.Location = new Point(13, 16);
            txtId.Margin = new Padding(3, 4, 3, 4);
            txtId.MaxLength = 32767;
            txtId.MouseState = MaterialSkin.MouseState.OUT;
            txtId.Name = "txtId";
            txtId.PasswordChar = '\0';
            txtId.PrefixSuffixText = null;
            txtId.ReadOnly = true;
            txtId.RightToLeft = RightToLeft.No;
            txtId.SelectedText = "";
            txtId.SelectionLength = 0;
            txtId.SelectionStart = 0;
            txtId.ShortcutsEnabled = true;
            txtId.Size = new Size(100, 36);
            txtId.TabIndex = 0;
            txtId.TabStop = false;
            txtId.TextAlign = HorizontalAlignment.Left;
            txtId.TrailingIcon = null;
            txtId.UseSystemPasswordChar = false;
            txtId.UseTallSize = false;
            // 
            // panelGrid
            // 
            panelGrid.Controls.Add(dgvNhomNguoiDung);
            panelGrid.Controls.Add(panelButtons);
            panelGrid.Dock = DockStyle.Fill;
            panelGrid.Location = new Point(10, 324);
            panelGrid.Margin = new Padding(3, 4, 3, 4);
            panelGrid.Name = "panelGrid";
            panelGrid.Padding = new Padding(10, 12, 10, 12);
            panelGrid.Size = new Size(830, 414);
            panelGrid.TabIndex = 1;
            // 
            // dgvNhomNguoiDung
            // 
            dgvNhomNguoiDung.AllowUserToAddRows = false;
            dgvNhomNguoiDung.AllowUserToDeleteRows = false;
            dgvNhomNguoiDung.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvNhomNguoiDung.Dock = DockStyle.Fill;
            dgvNhomNguoiDung.Location = new Point(10, 74);
            dgvNhomNguoiDung.Margin = new Padding(3, 4, 3, 4);
            dgvNhomNguoiDung.MultiSelect = false;
            dgvNhomNguoiDung.Name = "dgvNhomNguoiDung";
            dgvNhomNguoiDung.ReadOnly = true;
            dgvNhomNguoiDung.RowHeadersWidth = 51;
            dgvNhomNguoiDung.RowTemplate.Height = 24;
            dgvNhomNguoiDung.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNhomNguoiDung.Size = new Size(810, 328);
            dgvNhomNguoiDung.TabIndex = 1;
            dgvNhomNguoiDung.SelectionChanged += dgvNhomNguoiDung_SelectionChanged;
            dgvNhomNguoiDung.DoubleClick += dgvNhomNguoiDung_DoubleClick;
            // 
            // panelButtons
            // 
            panelButtons.Controls.Add(btnThem);
            panelButtons.Controls.Add(btnSua);
            panelButtons.Controls.Add(btnBoQua);
            panelButtons.Controls.Add(btnXoa);
            panelButtons.Controls.Add(btnLuu);
            panelButtons.Dock = DockStyle.Top;
            panelButtons.Location = new Point(10, 12);
            panelButtons.Margin = new Padding(3, 4, 3, 4);
            panelButtons.Name = "panelButtons";
            panelButtons.Padding = new Padding(3, 4, 3, 4);
            panelButtons.Size = new Size(810, 62);
            panelButtons.TabIndex = 0;
            // 
            // btnThem
            // 
            btnThem.AutoSize = false;
            btnThem.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnThem.Density = MaterialButton.MaterialButtonDensity.Default;
            btnThem.Depth = 0;
            btnThem.HighEmphasis = true;
            btnThem.Icon = null;
            btnThem.Location = new Point(14, 8);
            btnThem.Margin = new Padding(3, 4, 3, 4);
            btnThem.MouseState = MaterialSkin.MouseState.HOVER;
            btnThem.Name = "btnThem";
            btnThem.NoAccentTextColor = Color.Empty;
            btnThem.Size = new Size(80, 45);
            btnThem.TabIndex = 0;
            btnThem.Text = "Thêm";
            btnThem.Type = MaterialButton.MaterialButtonType.Contained;
            btnThem.UseAccentColor = false;
            btnThem.UseVisualStyleBackColor = true;
            btnThem.Click += btnThem_Click;
            // 
            // btnSua
            // 
            btnSua.AutoSize = false;
            btnSua.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSua.Density = MaterialButton.MaterialButtonDensity.Default;
            btnSua.Depth = 0;
            btnSua.HighEmphasis = false;
            btnSua.Icon = null;
            btnSua.Location = new Point(108, 8);
            btnSua.Margin = new Padding(3, 4, 3, 4);
            btnSua.MouseState = MaterialSkin.MouseState.HOVER;
            btnSua.Name = "btnSua";
            btnSua.NoAccentTextColor = Color.Empty;
            btnSua.Size = new Size(80, 45);
            btnSua.TabIndex = 1;
            btnSua.Text = "Sửa";
            btnSua.Type = MaterialButton.MaterialButtonType.Outlined;
            btnSua.UseAccentColor = false;
            btnSua.UseVisualStyleBackColor = true;
            btnSua.Click += btnSua_Click;
            // 
            // btnXoa
            // 
            btnXoa.AutoSize = false;
            btnXoa.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnXoa.Density = MaterialButton.MaterialButtonDensity.Default;
            btnXoa.Depth = 0;
            btnXoa.HighEmphasis = false;
            btnXoa.Icon = null;
            btnXoa.Location = new Point(202, 8);
            btnXoa.Margin = new Padding(3, 4, 3, 4);
            btnXoa.MouseState = MaterialSkin.MouseState.HOVER;
            btnXoa.Name = "btnXoa";
            btnXoa.NoAccentTextColor = Color.Empty;
            btnXoa.Size = new Size(80, 45);
            btnXoa.TabIndex = 2;
            btnXoa.Text = "Xóa";
            btnXoa.Type = MaterialButton.MaterialButtonType.Outlined;
            btnXoa.UseAccentColor = true;
            btnXoa.UseVisualStyleBackColor = true;
            btnXoa.Click += btnXoa_Click;
            // 
            // ucQuanLyNhomNguoiDung
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panelGrid);
            Controls.Add(panelDetails);
            Margin = new Padding(3, 4, 3, 4);
            Name = "ucQuanLyNhomNguoiDung";
            Padding = new Padding(10, 12, 10, 12);
            Size = new Size(850, 750);
            panelDetails.ResumeLayout(false);
            panelDetails.PerformLayout();
            panelGrid.ResumeLayout(false);
            ((ISupportInitialize)dgvNhomNguoiDung).EndInit();
            panelButtons.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        // Khai báo biến cho các control
        private System.Windows.Forms.Panel panelDetails;
        private MaterialSkin.Controls.MaterialTextBox2 txtTenNhom;
        private MaterialSkin.Controls.MaterialTextBox2 txtMaNhom;
        private MaterialSkin.Controls.MaterialTextBox2 txtId;
        private MaterialSkin.Controls.MaterialButton btnLuu;
        private MaterialSkin.Controls.MaterialButton btnBoQua;
        private System.Windows.Forms.Panel panelGrid;
        private MaterialSkin.Controls.MaterialButton btnThem;
        private MaterialSkin.Controls.MaterialButton btnSua;
        private MaterialSkin.Controls.MaterialButton btnXoa;
        private System.Windows.Forms.DataGridView dgvNhomNguoiDung;
        private System.Windows.Forms.Panel panelButtons;
        private MaterialSkin.Controls.MaterialLabel lblChucNang;
        private System.Windows.Forms.CheckedListBox clbChucNang;

    }
}