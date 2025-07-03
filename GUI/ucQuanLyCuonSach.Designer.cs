// File: GUI/ucQuanLyCuonSach.Designer.cs
// Project/Namespace: GUI

using System.Windows.Forms;
using MaterialSkin.Controls;
using System.Drawing;
using System.ComponentModel;

namespace GUI
{
    partial class ucQuanLyCuonSach
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // --- KHAI BÁO CÁC CONTROLS UI ---
        private MaterialSkin.Controls.MaterialTextBox2 txtId;
        private MaterialSkin.Controls.MaterialTextBox2 txtMaCuonSach;
        private MaterialSkin.Controls.MaterialTextBox2 txtTinhTrang;
        private System.Windows.Forms.ComboBox cboSach;

        private MaterialSkin.Controls.MaterialButton btnThem;
        private MaterialSkin.Controls.MaterialButton btnCapNhatTrangThai;
        private MaterialSkin.Controls.MaterialButton btnLuu;
        private MaterialSkin.Controls.MaterialButton btnBoQua;
        private MaterialSkin.Controls.MaterialButton btnThoat; // Optional

        private System.Windows.Forms.DataGridView dgvCuonSach;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.Panel panelGrid;
        private MaterialSkin.Controls.MaterialLabel lblSach;

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
            this.lblSach = new MaterialSkin.Controls.MaterialLabel();
            this.txtTinhTrang = new MaterialSkin.Controls.MaterialTextBox2();
            this.cboSach = new System.Windows.Forms.ComboBox();
            this.txtMaCuonSach = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtId = new MaterialSkin.Controls.MaterialTextBox2();
            this.btnLuu = new MaterialSkin.Controls.MaterialButton();
            this.btnBoQua = new MaterialSkin.Controls.MaterialButton();
            this.panelGrid = new System.Windows.Forms.Panel();
            this.btnThoat = new MaterialSkin.Controls.MaterialButton();
            this.btnCapNhatTrangThai = new MaterialSkin.Controls.MaterialButton();
            this.btnThem = new MaterialSkin.Controls.MaterialButton();
            this.dgvCuonSach = new System.Windows.Forms.DataGridView();
            this.panelDetails.SuspendLayout();
            this.panelGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCuonSach)).BeginInit();
            this.SuspendLayout();
            // 
            // panelDetails
            // 
            this.panelDetails.Controls.Add(this.lblSach);
            this.panelDetails.Controls.Add(this.txtTinhTrang);
            this.panelDetails.Controls.Add(this.cboSach);
            this.panelDetails.Controls.Add(this.txtMaCuonSach);
            this.panelDetails.Controls.Add(this.txtId);
            this.panelDetails.Controls.Add(this.btnLuu);
            this.panelDetails.Controls.Add(this.btnBoQua);
            this.panelDetails.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelDetails.Location = new System.Drawing.Point(10, 10); // Vị trí sau Padding của UserControl
            this.panelDetails.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Padding = new Padding(10); // *** ĐỔI PADDING ***
            this.panelDetails.Size = new System.Drawing.Size(780, 110); // Kích thước sau Padding
            this.panelDetails.TabIndex = 0;
            // 
            // lblSach
            // 
            this.lblSach.AutoSize = true;
            this.lblSach.Depth = 0;
            this.lblSach.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblSach.Location = new Point(13, 70);
            this.lblSach.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.lblSach.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblSach.Name = "lblSach";
            this.lblSach.Size = new Size(109, 19);
            this.lblSach.TabIndex = 8;
            this.lblSach.Text = "Thuộc Ấn Bản:";
            // 
            // txtTinhTrang
            // 
            this.txtTinhTrang.AnimateReadOnly = true;
            this.txtTinhTrang.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtTinhTrang.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtTinhTrang.Depth = 0;
            this.txtTinhTrang.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtTinhTrang.HideSelection = true;
            this.txtTinhTrang.Hint = "Tình Trạng";
            this.txtTinhTrang.LeadingIcon = null;
            this.txtTinhTrang.Location = new Point(363, 13);
            this.txtTinhTrang.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.txtTinhTrang.MaxLength = 50;
            this.txtTinhTrang.MouseState = MaterialSkin.MouseState.OUT;
            this.txtTinhTrang.Name = "txtTinhTrang";
            this.txtTinhTrang.PasswordChar = '\0';
            this.txtTinhTrang.PrefixSuffixText = null;
            this.txtTinhTrang.ReadOnly = true;
            this.txtTinhTrang.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtTinhTrang.SelectedText = "";
            this.txtTinhTrang.SelectionLength = 0;
            this.txtTinhTrang.SelectionStart = 0;
            this.txtTinhTrang.ShortcutsEnabled = true;
            this.txtTinhTrang.Size = new System.Drawing.Size(250, 48);
            this.txtTinhTrang.TabIndex = 2;
            this.txtTinhTrang.TabStop = false;
            this.txtTinhTrang.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtTinhTrang.TrailingIcon = null;
            this.txtTinhTrang.UseSystemPasswordChar = false;
            this.txtTinhTrang.UseTallSize = false;
            // 
            // cboSach
            // 
            this.cboSach.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboSach.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboSach.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSach.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.cboSach.FormattingEnabled = true;
            this.cboSach.Location = new Point(128, 66);
            this.cboSach.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.cboSach.Name = "cboSach";
            this.cboSach.Size = new System.Drawing.Size(485, 28);
            this.cboSach.TabIndex = 3;
            // 
            // txtMaCuonSach
            // 
            this.txtMaCuonSach.AnimateReadOnly = true;
            this.txtMaCuonSach.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtMaCuonSach.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtMaCuonSach.Depth = 0;
            this.txtMaCuonSach.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtMaCuonSach.HideSelection = true;
            this.txtMaCuonSach.Hint = "Mã Cuốn Sách";
            this.txtMaCuonSach.LeadingIcon = null;
            this.txtMaCuonSach.Location = new Point(128, 13);
            this.txtMaCuonSach.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.txtMaCuonSach.MaxLength = 10;
            this.txtMaCuonSach.MouseState = MaterialSkin.MouseState.OUT;
            this.txtMaCuonSach.Name = "txtMaCuonSach";
            this.txtMaCuonSach.PasswordChar = '\0';
            this.txtMaCuonSach.PrefixSuffixText = null;
            this.txtMaCuonSach.ReadOnly = true;
            this.txtMaCuonSach.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtMaCuonSach.SelectedText = "";
            this.txtMaCuonSach.SelectionLength = 0;
            this.txtMaCuonSach.SelectionStart = 0;
            this.txtMaCuonSach.ShortcutsEnabled = true;
            this.txtMaCuonSach.Size = new System.Drawing.Size(220, 48);
            this.txtMaCuonSach.TabIndex = 1;
            this.txtMaCuonSach.TabStop = false;
            this.txtMaCuonSach.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtMaCuonSach.TrailingIcon = null;
            this.txtMaCuonSach.UseSystemPasswordChar = false;
            this.txtMaCuonSach.UseTallSize = false;
            // 
            // txtId
            // 
            this.txtId.AnimateReadOnly = true;
            this.txtId.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtId.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtId.Depth = 0;
            this.txtId.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtId.HideSelection = true;
            this.txtId.Hint = "ID";
            this.txtId.LeadingIcon = null;
            this.txtId.Location = new Point(13, 13);
            this.txtId.Margin = new Padding(3); // *** ĐỔI MARGIN ***
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
            this.txtId.TabIndex = 0;
            this.txtId.TabStop = false;
            this.txtId.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtId.TrailingIcon = null;
            this.txtId.UseSystemPasswordChar = false;
            this.txtId.UseTallSize = false;
            // 
            // btnLuu
            // 
            this.btnLuu.AutoSize = false;
            this.btnLuu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLuu.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnLuu.Depth = 0;
            this.btnLuu.HighEmphasis = true;
            this.btnLuu.Icon = null;
            this.btnLuu.Location = new Point(648, 19);
            this.btnLuu.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.btnLuu.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnLuu.Size = new System.Drawing.Size(120, 36);
            this.btnLuu.TabIndex = 4;
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
            this.btnBoQua.Location = new Point(648, 61);
            this.btnBoQua.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.btnBoQua.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnBoQua.Name = "btnBoQua";
            this.btnBoQua.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnBoQua.Size = new System.Drawing.Size(120, 36);
            this.btnBoQua.TabIndex = 5;
            this.btnBoQua.Text = "Bỏ qua";
            this.btnBoQua.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnBoQua.UseAccentColor = false;
            this.btnBoQua.UseVisualStyleBackColor = true;
            this.btnBoQua.Click += new System.EventHandler(this.btnBoQua_Click);
            // 
            // panelGrid
            // 
            this.panelGrid.Controls.Add(this.btnThoat);
            this.panelGrid.Controls.Add(this.btnCapNhatTrangThai);
            this.panelGrid.Controls.Add(this.btnThem);
            this.panelGrid.Controls.Add(this.dgvCuonSach);
            this.panelGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGrid.Location = new Point(10, 120); // Vị trí sau panelDetails và Padding
            this.panelGrid.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Padding = new Padding(10); // *** ĐỔI PADDING ***
            this.panelGrid.Size = new System.Drawing.Size(780, 320); // Kích thước sau Padding
            this.panelGrid.TabIndex = 1;
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
            this.btnThoat.Location = new System.Drawing.Point(656, 6); // Vị trí góc phải trong panelGrid (sau padding)
            this.btnThoat.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.btnThoat.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnThoat.Size = new System.Drawing.Size(120, 36);
            this.btnThoat.TabIndex = 3;
            this.btnThoat.Text = "Đóng";
            this.btnThoat.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text;
            this.btnThoat.UseAccentColor = false;
            this.btnThoat.UseVisualStyleBackColor = true;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // btnCapNhatTrangThai
            // 
            this.btnCapNhatTrangThai.AutoSize = false;
            this.btnCapNhatTrangThai.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCapNhatTrangThai.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnCapNhatTrangThai.Depth = 0;
            this.btnCapNhatTrangThai.HighEmphasis = false;
            this.btnCapNhatTrangThai.Icon = null;
            this.btnCapNhatTrangThai.Location = new Point(167, 6); // Vị trí sau btnThem
            this.btnCapNhatTrangThai.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.btnCapNhatTrangThai.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnCapNhatTrangThai.Name = "btnCapNhatTrangThai";
            this.btnCapNhatTrangThai.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnCapNhatTrangThai.Size = new System.Drawing.Size(180, 36);
            this.btnCapNhatTrangThai.TabIndex = 1;
            this.btnCapNhatTrangThai.Text = "Cập Nhật Trạng Thái";
            this.btnCapNhatTrangThai.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnCapNhatTrangThai.UseAccentColor = true;
            this.btnCapNhatTrangThai.UseVisualStyleBackColor = true;
            this.btnCapNhatTrangThai.Click += new System.EventHandler(this.btnCapNhatTrangThai_Click);
            // 
            // btnThem
            // 
            this.btnThem.AutoSize = false;
            this.btnThem.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThem.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnThem.Depth = 0;
            this.btnThem.HighEmphasis = true;
            this.btnThem.Icon = null;
            this.btnThem.Location = new Point(13, 6); // Vị trí đầu tiên trong panelGrid (sau padding)
            this.btnThem.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.btnThem.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnThem.Name = "btnThem";
            this.btnThem.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnThem.Size = new System.Drawing.Size(140, 36);
            this.btnThem.TabIndex = 0;
            this.btnThem.Text = "Thêm Cuốn Sách";
            this.btnThem.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnThem.UseAccentColor = false;
            this.btnThem.UseVisualStyleBackColor = true;
            this.btnThem.Click += new System.EventHandler(this.btnThem_Click);
            // 
            // dgvCuonSach
            // 
            this.dgvCuonSach.AllowUserToAddRows = false;
            this.dgvCuonSach.AllowUserToDeleteRows = false;
            this.dgvCuonSach.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCuonSach.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCuonSach.Location = new Point(13, 50); // Vị trí dưới các nút (sau padding)
            this.dgvCuonSach.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.dgvCuonSach.MultiSelect = false;
            this.dgvCuonSach.Name = "dgvCuonSach";
            this.dgvCuonSach.ReadOnly = true;
            this.dgvCuonSach.RowHeadersWidth = 51;
            this.dgvCuonSach.RowTemplate.Height = 24;
            this.dgvCuonSach.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCuonSach.Size = new System.Drawing.Size(754, 257); // Kích thước sau padding
            this.dgvCuonSach.TabIndex = 2;
            this.dgvCuonSach.SelectionChanged += new System.EventHandler(this.dgvCuonSach_SelectionChanged);
            // 
            // ucQuanLyCuonSach
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.panelDetails);
            this.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.Name = "ucQuanLyCuonSach";
            this.Padding = new Padding(10); // *** ĐỔI PADDING ***
            this.Size = new System.Drawing.Size(800, 450);
            this.Load += new System.EventHandler(this.ucQuanLyCuonSach_Load);
            this.panelDetails.ResumeLayout(false);
            this.panelDetails.PerformLayout();
            this.panelGrid.ResumeLayout(false);
            this.panelGrid.PerformLayout(); // Thêm PerformLayout cho panelGrid
            ((System.ComponentModel.ISupportInitialize)(this.dgvCuonSach)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}