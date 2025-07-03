// Project/Namespace: GUI

using System;
using System.Windows.Forms; // Cần cho UserControl, DataGridView, Panel, AutoScaleMode, SizeF, Point, MouseEventHandler, DataGridViewCellEventHandler, DataGridViewRowsAddedEventHandler, DataGridViewRowsRemovedEventHandler, DataGridViewSelectionEventHandler
using MaterialSkin.Controls; // Cần cho MaterialTextBox2, MaterialButton (Kiểm tra namespace chính xác trong project MaterialSkin.2 của bạn)
using System.Drawing; // Cần cho Point, Size, SizeF
using System.ComponentModel; // Cần cho IContainer, ComponentResourceManager

namespace GUI // Namespace của project GUI của bạn
{
    // Class được định nghĩa một phần tại đây, phần còn lại ở file ucQuanLyTheLoai.cs
    partial class ucQuanLyTheLoai
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // --- KHAI BÁO CÁC CONTROLS UI SỬ DỤNG TRONG CODE-BEHIND ---
        private MaterialSkin.Controls.MaterialTextBox2 txtId;
        private MaterialSkin.Controls.MaterialTextBox2 txtMaTheLoai;
        private MaterialSkin.Controls.MaterialTextBox2 txtTenTheLoai;

        private MaterialSkin.Controls.MaterialButton btnThem;
        private MaterialSkin.Controls.MaterialButton btnSua;
        private MaterialSkin.Controls.MaterialButton btnXoa;
        private MaterialSkin.Controls.MaterialButton btnLuu;
        private MaterialSkin.Controls.MaterialButton btnBoQua;

        private System.Windows.Forms.DataGridView dgvTheLoai;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.Panel panelGrid;

        // *** THÊM KHAI BÁO CHO Ô TÌM KIẾM ***
        private MaterialSkin.Controls.MaterialTextBox2 txtTimKiem;
        // Khai báo btnThoat nếu bạn có nút này trong Designer

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
            this.txtId = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtMaTheLoai = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtTenTheLoai = new MaterialSkin.Controls.MaterialTextBox2();
            this.btnThem = new MaterialSkin.Controls.MaterialButton();
            this.btnSua = new MaterialSkin.Controls.MaterialButton();
            this.btnXoa = new MaterialSkin.Controls.MaterialButton();
            this.btnLuu = new MaterialSkin.Controls.MaterialButton();
            this.btnBoQua = new MaterialSkin.Controls.MaterialButton();
            this.dgvTheLoai = new System.Windows.Forms.DataGridView();
            this.panelDetails = new System.Windows.Forms.Panel();
            this.panelGrid = new System.Windows.Forms.Panel();
            // *** THÊM KHỞI TẠO CHO Ô TÌM KIẾM ***
            this.txtTimKiem = new MaterialSkin.Controls.MaterialTextBox2();

            this.panelDetails.SuspendLayout();
            this.panelGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTheLoai)).BeginInit();
            this.SuspendLayout();
            // 
            // panelDetails
            // 
            this.panelDetails.Controls.Add(this.txtTenTheLoai);
            this.panelDetails.Controls.Add(this.txtMaTheLoai);
            this.panelDetails.Controls.Add(this.txtId);
            this.panelDetails.Controls.Add(this.btnLuu);
            this.panelDetails.Controls.Add(this.btnBoQua);
            this.panelDetails.Dock = DockStyle.Top;
            this.panelDetails.Location = new System.Drawing.Point(10, 10); // Sau Padding UserControl
            this.panelDetails.Margin = new Padding(3);
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Padding = new Padding(10);
            this.panelDetails.Size = new System.Drawing.Size(580, 150); // Kích thước sau Padding
            this.panelDetails.TabIndex = 0;
            // 
            // panelGrid
            // 
            // *** THÊM txtTimKiem VÀO CONTROLS CỦA panelGrid ***
            this.panelGrid.Controls.Add(this.txtTimKiem);
            this.panelGrid.Controls.Add(this.dgvTheLoai);
            this.panelGrid.Controls.Add(this.btnXoa);
            this.panelGrid.Controls.Add(this.btnSua);
            this.panelGrid.Controls.Add(this.btnThem);
            this.panelGrid.Dock = DockStyle.Fill;
            this.panelGrid.Location = new System.Drawing.Point(10, 160); // Sau panelDetails và Padding
            this.panelGrid.Margin = new Padding(3);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Padding = new Padding(10);
            this.panelGrid.Size = new System.Drawing.Size(580, 280); // Kích thước sau Padding - Có thể cần tăng chiều cao
            this.panelGrid.TabIndex = 1;
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
            this.txtId.Location = new Point(13, 13); // Sau Padding panelDetails
            this.txtId.Margin = new Padding(3);
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
            this.txtId.Size = new System.Drawing.Size(150, 48);
            this.txtId.TabIndex = 0;
            this.txtId.TabStop = false;
            this.txtId.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtId.TrailingIcon = null;
            this.txtId.UseSystemPasswordChar = false;
            this.txtId.UseTallSize = false; // ID không cần cao
            // 
            // txtMaTheLoai
            // 
            this.txtMaTheLoai.AnimateReadOnly = false;
            this.txtMaTheLoai.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtMaTheLoai.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtMaTheLoai.Depth = 0;
            this.txtMaTheLoai.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtMaTheLoai.HideSelection = true;
            this.txtMaTheLoai.Hint = "Mã Thể Loại";
            this.txtMaTheLoai.LeadingIcon = null;
            this.txtMaTheLoai.Location = new Point(13, 71); // Điều chỉnh Y
            this.txtMaTheLoai.Margin = new Padding(3);
            this.txtMaTheLoai.MaxLength = 6;
            this.txtMaTheLoai.MouseState = MaterialSkin.MouseState.OUT;
            this.txtMaTheLoai.Name = "txtMaTheLoai";
            this.txtMaTheLoai.PasswordChar = '\0';
            this.txtMaTheLoai.PrefixSuffixText = null;
            this.txtMaTheLoai.ReadOnly = true; // Mã thường không cho sửa trực tiếp
            this.txtMaTheLoai.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtMaTheLoai.SelectedText = "";
            this.txtMaTheLoai.SelectionLength = 0;
            this.txtMaTheLoai.SelectionStart = 0;
            this.txtMaTheLoai.ShortcutsEnabled = true;
            this.txtMaTheLoai.Size = new System.Drawing.Size(200, 48);
            this.txtMaTheLoai.TabIndex = 1;
            this.txtMaTheLoai.TabStop = false; // Không cần focus vào ô Mã
            this.txtMaTheLoai.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtMaTheLoai.TrailingIcon = null;
            this.txtMaTheLoai.UseSystemPasswordChar = false;
            this.txtMaTheLoai.UseTallSize = false;
            // 
            // txtTenTheLoai
            // 
            this.txtTenTheLoai.AnimateReadOnly = false;
            this.txtTenTheLoai.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtTenTheLoai.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtTenTheLoai.Depth = 0;
            this.txtTenTheLoai.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtTenTheLoai.HideSelection = true;
            this.txtTenTheLoai.Hint = "Tên Thể Loại";
            this.txtTenTheLoai.LeadingIcon = null;
            this.txtTenTheLoai.Location = new Point(230, 71); // Điều chỉnh Y và X
            this.txtTenTheLoai.Margin = new Padding(3);
            this.txtTenTheLoai.MaxLength = 100;
            this.txtTenTheLoai.MouseState = MaterialSkin.MouseState.OUT;
            this.txtTenTheLoai.Name = "txtTenTheLoai";
            this.txtTenTheLoai.PasswordChar = '\0';
            this.txtTenTheLoai.PrefixSuffixText = null;
            this.txtTenTheLoai.ReadOnly = false; // Cho phép nhập khi Thêm/Sửa
            this.txtTenTheLoai.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtTenTheLoai.SelectedText = "";
            this.txtTenTheLoai.SelectionLength = 0;
            this.txtTenTheLoai.SelectionStart = 0;
            this.txtTenTheLoai.ShortcutsEnabled = true;
            this.txtTenTheLoai.Size = new System.Drawing.Size(330, 48); // Điều chỉnh Size
            this.txtTenTheLoai.TabIndex = 2; // TabIndex sau Mã
            this.txtTenTheLoai.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtTenTheLoai.TrailingIcon = null;
            this.txtTenTheLoai.UseSystemPasswordChar = false;
            this.txtTenTheLoai.UseTallSize = false;
            // 
            // btnLuu
            // 
            this.btnLuu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLuu.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnLuu.Depth = 0;
            this.btnLuu.HighEmphasis = true;
            this.btnLuu.Icon = null;
            this.btnLuu.Location = new Point(349, 13); // Điều chỉnh vị trí X, Y
            this.btnLuu.Margin = new Padding(4, 6, 4, 6); // Tăng Margin cho dễ bấm
            this.btnLuu.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnLuu.Size = new System.Drawing.Size(64, 36); // Kích thước chữ Lưu
            this.btnLuu.TabIndex = 3; // TabIndex sau Tên
            this.btnLuu.Text = "Lưu";
            this.btnLuu.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnLuu.UseAccentColor = false;
            this.btnLuu.UseVisualStyleBackColor = true;
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            // 
            // btnBoQua
            // 
            this.btnBoQua.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnBoQua.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnBoQua.Depth = 0;
            this.btnBoQua.HighEmphasis = true; // Đổi thành false nếu muốn nút Bỏ qua ít nổi bật hơn
            this.btnBoQua.Icon = null;
            this.btnBoQua.Location = new Point(421, 13); // Điều chỉnh vị trí X, Y (sau nút Lưu)
            this.btnBoQua.Margin = new Padding(4, 6, 4, 6); // Tăng Margin
            this.btnBoQua.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnBoQua.Name = "btnBoQua";
            this.btnBoQua.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnBoQua.Size = new System.Drawing.Size(73, 36); // Kích thước chữ Bỏ qua
            this.btnBoQua.TabIndex = 4; // TabIndex sau Lưu
            this.btnBoQua.Text = "Bỏ qua";
            this.btnBoQua.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined; // Kiểu Outlined cho Bỏ qua
            this.btnBoQua.UseAccentColor = false;
            this.btnBoQua.UseVisualStyleBackColor = true;
            this.btnBoQua.Click += new System.EventHandler(this.btnBoQua_Click);
            // 
            // btnThem
            // 
            this.btnThem.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThem.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnThem.Depth = 0;
            this.btnThem.HighEmphasis = true;
            this.btnThem.Icon = null; // Có thể thêm Icon nếu muốn: global::GUI.Properties.Resources.ic_add_white_24dp;
            this.btnThem.Location = new Point(13, 13); // Sau Padding panelGrid
            this.btnThem.Margin = new Padding(4, 6, 4, 6);
            this.btnThem.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnThem.Name = "btnThem";
            this.btnThem.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnThem.Size = new System.Drawing.Size(64, 36); // Kích thước chữ Thêm
            this.btnThem.TabIndex = 0; // TabIndex đầu tiên trong panel này
            this.btnThem.Text = "Thêm";
            this.btnThem.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnThem.UseAccentColor = false;
            this.btnThem.UseVisualStyleBackColor = true;
            this.btnThem.Click += new System.EventHandler(this.btnThem_Click);
            // 
            // btnSua
            // 
            this.btnSua.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSua.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnSua.Depth = 0;
            this.btnSua.HighEmphasis = true;
            this.btnSua.Icon = null; // Icon sửa: global::GUI.Properties.Resources.ic_edit_white_24dp;
            this.btnSua.Location = new Point(85, 13); // Sau btnThem
            this.btnSua.Margin = new Padding(4, 6, 4, 6);
            this.btnSua.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSua.Name = "btnSua";
            this.btnSua.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnSua.Size = new System.Drawing.Size(64, 36); // Kích thước chữ Sửa
            this.btnSua.TabIndex = 1; // TabIndex tiếp theo
            this.btnSua.Text = "Sửa";
            this.btnSua.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnSua.UseAccentColor = false;
            this.btnSua.UseVisualStyleBackColor = true;
            this.btnSua.Click += new System.EventHandler(this.btnSua_Click);
            // 
            // btnXoa
            // 
            this.btnXoa.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnXoa.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnXoa.Depth = 0;
            this.btnXoa.HighEmphasis = true;
            this.btnXoa.Icon = null; // Icon xóa: global::GUI.Properties.Resources.ic_delete_white_24dp;
            this.btnXoa.Location = new Point(157, 13); // Sau btnSua
            this.btnXoa.Margin = new Padding(4, 6, 4, 6);
            this.btnXoa.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnXoa.Size = new System.Drawing.Size(64, 36); // Kích thước chữ Xóa
            this.btnXoa.TabIndex = 2; // TabIndex tiếp theo
            this.btnXoa.Text = "Xóa";
            this.btnXoa.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnXoa.UseAccentColor = true; // Dùng màu Accent (thường là đỏ) cho nút Xóa
            this.btnXoa.UseVisualStyleBackColor = true;
            this.btnXoa.Click += new System.EventHandler(this.btnXoa_Click);

            // 
            // *** CẤU HÌNH Ô TÌM KIẾM ***
            // 
            this.txtTimKiem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))); // Anchor để co giãn theo chiều ngang
            this.txtTimKiem.AnimateReadOnly = false;
            this.txtTimKiem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtTimKiem.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtTimKiem.Depth = 0;
            this.txtTimKiem.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtTimKiem.HideSelection = true;
            this.txtTimKiem.Hint = "Tìm kiếm theo Mã hoặc Tên";
            this.txtTimKiem.LeadingIcon = null; // Có thể thêm icon kính lúp nếu muốn
            this.txtTimKiem.Location = new System.Drawing.Point(229, 13); // Đặt bên phải nút Xóa
            this.txtTimKiem.Margin = new System.Windows.Forms.Padding(3);
            this.txtTimKiem.MaxLength = 50;
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
            this.txtTimKiem.Size = new System.Drawing.Size(338, 36); // Điều chỉnh kích thước cho phù hợp với Anchor Right
            this.txtTimKiem.TabIndex = 3; // TabIndex sau nút Xóa
            this.txtTimKiem.TabStop = true;
            this.txtTimKiem.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtTimKiem.TrailingIcon = null;
            this.txtTimKiem.UseSystemPasswordChar = false;
            this.txtTimKiem.UseTallSize = true; // Dùng chiều cao 36 giống button
            // *** THÊM SỰ KIỆN TEXTCHANGED ***
            this.txtTimKiem.TextChanged += new System.EventHandler(this.txtTimKiem_TextChanged);

            // 
            // dgvTheLoai
            // 
            this.dgvTheLoai.AllowUserToAddRows = false;
            this.dgvTheLoai.AllowUserToDeleteRows = false;
            this.dgvTheLoai.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTheLoai.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            // *** ĐIỀU CHỈNH VỊ TRÍ Y VÀ SIZE ĐỂ CÓ CHỖ CHO Ô TÌM KIẾM ***
            this.dgvTheLoai.Location = new Point(13, 60); // Tăng Y để xuống dưới các nút và ô tìm kiếm
            this.dgvTheLoai.Margin = new Padding(3);
            this.dgvTheLoai.MultiSelect = false;
            this.dgvTheLoai.Name = "dgvTheLoai";
            this.dgvTheLoai.ReadOnly = true;
            this.dgvTheLoai.RowHeadersWidth = 51;
            this.dgvTheLoai.RowTemplate.Height = 24;
            this.dgvTheLoai.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTheLoai.Size = new System.Drawing.Size(554, 207); // Giảm Height nếu cần (580 panel - 20 padding - 60 vị trí Y - lề dưới)
            this.dgvTheLoai.TabIndex = 5; // Cập nhật TabIndex sau ô tìm kiếm
            this.dgvTheLoai.SelectionChanged += new System.EventHandler(this.dgvTheLoai_SelectionChanged);
            this.dgvTheLoai.DoubleClick += new System.EventHandler(this.dgvTheLoai_DoubleClick);
            // 
            // ucQuanLyTheLoai
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.panelDetails);
            this.Margin = new Padding(3);
            this.Name = "ucQuanLyTheLoai";
            this.Padding = new Padding(10);
            this.Size = new System.Drawing.Size(600, 450); // Có thể cần điều chỉnh Size tổng thể
            this.Load += new System.EventHandler(this.ucQuanLyTheLoai_Load);
            this.panelDetails.ResumeLayout(false);
            this.panelDetails.PerformLayout();
            this.panelGrid.ResumeLayout(false);
            this.panelGrid.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTheLoai)).EndInit();
            this.ResumeLayout(false);
            // Bỏ PerformLayout nếu không cần
        }

        #endregion
    }
}