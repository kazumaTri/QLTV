// File: QuanLyThuVien/GUI/ucQuanLyPhieuMuonTra.Designer.cs
// Phiên bản đã được sửa đổi để đồng nhất Padding/Margin

using System;
using System.Windows.Forms;
using MaterialSkin.Controls;
using System.Drawing;
using System.ComponentModel;

namespace GUI
{
    partial class ucQuanLyPhieuMuonTra
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // --- KHAI BÁO CÁC CONTROLS UI ---
        private MaterialSkin.Controls.MaterialTextBox2 txtSoPhieuMuonTra;
        private System.Windows.Forms.DateTimePicker dtpNgayMuon;
        private System.Windows.Forms.DateTimePicker dtpHanTra;
        private System.Windows.Forms.DateTimePicker dtpNgayTra;
        private MaterialSkin.Controls.MaterialTextBox2 txtSoTienPhat;
        private System.Windows.Forms.ComboBox cboDocGia;
        private System.Windows.Forms.ComboBox cboCuonSach;
        private MaterialSkin.Controls.MaterialButton btnThem;
        private MaterialSkin.Controls.MaterialButton btnTraSach;
        private MaterialSkin.Controls.MaterialButton btnLuu;
        private MaterialSkin.Controls.MaterialButton btnBoQua;
        private MaterialSkin.Controls.MaterialButton btnThoat;
        private System.Windows.Forms.DataGridView dgvPhieuMuonTra;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.Panel panelReturnDetails;
        private System.Windows.Forms.Panel panelGrid;
        private System.Windows.Forms.Panel panelButtons;


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
            this.dtpHanTra = new System.Windows.Forms.DateTimePicker();
            this.dtpNgayMuon = new System.Windows.Forms.DateTimePicker();
            this.cboCuonSach = new System.Windows.Forms.ComboBox();
            this.cboDocGia = new System.Windows.Forms.ComboBox();
            this.txtSoPhieuMuonTra = new MaterialSkin.Controls.MaterialTextBox2();
            this.btnLuu = new MaterialSkin.Controls.MaterialButton();
            this.btnBoQua = new MaterialSkin.Controls.MaterialButton();
            this.panelReturnDetails = new System.Windows.Forms.Panel();
            this.txtSoTienPhat = new MaterialSkin.Controls.MaterialTextBox2();
            this.dtpNgayTra = new System.Windows.Forms.DateTimePicker();
            this.panelGrid = new System.Windows.Forms.Panel();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.dgvPhieuMuonTra = new System.Windows.Forms.DataGridView();
            this.btnThoat = new MaterialSkin.Controls.MaterialButton();
            this.btnTraSach = new MaterialSkin.Controls.MaterialButton();
            this.btnThem = new MaterialSkin.Controls.MaterialButton();

            this.panelDetails.SuspendLayout();
            this.panelReturnDetails.SuspendLayout();
            this.panelGrid.SuspendLayout();
            this.panelButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPhieuMuonTra)).BeginInit();
            this.SuspendLayout();
            // 
            // panelDetails
            // 
            this.panelDetails.Controls.Add(this.dtpHanTra);
            this.panelDetails.Controls.Add(this.dtpNgayMuon);
            this.panelDetails.Controls.Add(this.cboCuonSach);
            this.panelDetails.Controls.Add(this.cboDocGia);
            this.panelDetails.Controls.Add(this.txtSoPhieuMuonTra);
            this.panelDetails.Controls.Add(this.btnLuu);
            this.panelDetails.Controls.Add(this.btnBoQua);
            this.panelDetails.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelDetails.Location = new System.Drawing.Point(10, 10); // Sau Padding UserControl
            this.panelDetails.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Padding = new Padding(10); // *** ĐỔI PADDING ***
            this.panelDetails.Size = new System.Drawing.Size(780, 190); // Kích thước sau Padding
            this.panelDetails.TabIndex = 0;
            // 
            // dtpHanTra
            // 
            this.dtpHanTra.CustomFormat = "dd/MM/yyyy";
            this.dtpHanTra.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dtpHanTra.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpHanTra.Location = new System.Drawing.Point(193, 88); // Sau Padding panel
            this.dtpHanTra.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.dtpHanTra.Name = "dtpHanTra";
            this.dtpHanTra.Size = new System.Drawing.Size(150, 26);
            this.dtpHanTra.TabIndex = 2;
            // 
            // dtpNgayMuon
            // 
            this.dtpNgayMuon.CustomFormat = "dd/MM/yyyy";
            this.dtpNgayMuon.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dtpNgayMuon.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpNgayMuon.Location = new System.Drawing.Point(23, 88); // Sau Padding panel
            this.dtpNgayMuon.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.dtpNgayMuon.Name = "dtpNgayMuon";
            this.dtpNgayMuon.Size = new System.Drawing.Size(150, 26);
            this.dtpNgayMuon.TabIndex = 1;
            // 
            // cboCuonSach
            // 
            this.cboCuonSach.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboCuonSach.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboCuonSach.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCuonSach.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.cboCuonSach.FormattingEnabled = true;
            this.cboCuonSach.Location = new System.Drawing.Point(283, 138); // Sau Padding panel
            this.cboCuonSach.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.cboCuonSach.Name = "cboCuonSach";
            this.cboCuonSach.Size = new System.Drawing.Size(250, 28);
            this.cboCuonSach.TabIndex = 4;
            // 
            // cboDocGia
            // 
            this.cboDocGia.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboDocGia.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboDocGia.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDocGia.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.cboDocGia.FormattingEnabled = true;
            this.cboDocGia.Location = new System.Drawing.Point(23, 138); // Sau Padding panel
            this.cboDocGia.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.cboDocGia.Name = "cboDocGia";
            this.cboDocGia.Size = new System.Drawing.Size(250, 28);
            this.cboDocGia.TabIndex = 3;
            // 
            // txtSoPhieuMuonTra
            // 
            this.txtSoPhieuMuonTra.AnimateReadOnly = false;
            this.txtSoPhieuMuonTra.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtSoPhieuMuonTra.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtSoPhieuMuonTra.Depth = 0;
            this.txtSoPhieuMuonTra.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtSoPhieuMuonTra.HideSelection = true;
            this.txtSoPhieuMuonTra.Hint = "Số Phiếu";
            this.txtSoPhieuMuonTra.LeadingIcon = null;
            this.txtSoPhieuMuonTra.Location = new Point(23, 20); // Sau Padding panel
            this.txtSoPhieuMuonTra.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.txtSoPhieuMuonTra.MaxLength = 50;
            this.txtSoPhieuMuonTra.MouseState = MaterialSkin.MouseState.OUT;
            this.txtSoPhieuMuonTra.Name = "txtSoPhieuMuonTra";
            this.txtSoPhieuMuonTra.PasswordChar = '\0';
            this.txtSoPhieuMuonTra.PrefixSuffixText = null;
            this.txtSoPhieuMuonTra.ReadOnly = true;
            this.txtSoPhieuMuonTra.SelectedText = "";
            this.txtSoPhieuMuonTra.SelectionLength = 0;
            this.txtSoPhieuMuonTra.SelectionStart = 0;
            this.txtSoPhieuMuonTra.ShortcutsEnabled = true;
            this.txtSoPhieuMuonTra.Size = new System.Drawing.Size(150, 48);
            this.txtSoPhieuMuonTra.TabIndex = 0;
            this.txtSoPhieuMuonTra.TabStop = false;
            this.txtSoPhieuMuonTra.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtSoPhieuMuonTra.TrailingIcon = null;
            this.txtSoPhieuMuonTra.UseSystemPasswordChar = false;
            this.txtSoPhieuMuonTra.UseTallSize = false;
            // 
            // btnLuu
            // 
            this.btnLuu.AutoSize = false;
            this.btnLuu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLuu.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnLuu.Depth = 0;
            this.btnLuu.HighEmphasis = true;
            this.btnLuu.Icon = null;
            this.btnLuu.Location = new System.Drawing.Point(553, 134); // Điều chỉnh vị trí
            this.btnLuu.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.btnLuu.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnLuu.Size = new System.Drawing.Size(80, 36);
            this.btnLuu.TabIndex = 5;
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
            this.btnBoQua.Location = new System.Drawing.Point(653, 134); // Điều chỉnh vị trí
            this.btnBoQua.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.btnBoQua.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnBoQua.Name = "btnBoQua";
            this.btnBoQua.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnBoQua.Size = new System.Drawing.Size(86, 36);
            this.btnBoQua.TabIndex = 6;
            this.btnBoQua.Text = "Bỏ qua";
            this.btnBoQua.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnBoQua.UseAccentColor = false;
            this.btnBoQua.UseVisualStyleBackColor = true;
            this.btnBoQua.Click += new System.EventHandler(this.btnBoQua_Click);
            // 
            // panelReturnDetails
            // 
            this.panelReturnDetails.Controls.Add(this.txtSoTienPhat);
            this.panelReturnDetails.Controls.Add(this.dtpNgayTra);
            this.panelReturnDetails.Dock = DockStyle.Top;
            this.panelReturnDetails.Location = new System.Drawing.Point(10, 200); // Vị trí sau panelDetails và Padding
            this.panelReturnDetails.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.panelReturnDetails.Name = "panelReturnDetails";
            this.panelReturnDetails.Padding = new Padding(10); // *** ĐỔI PADDING ***
            this.panelReturnDetails.Size = new System.Drawing.Size(780, 60);
            this.panelReturnDetails.TabIndex = 1;
            this.panelReturnDetails.Visible = false;
            // 
            // txtSoTienPhat
            // 
            this.txtSoTienPhat.AnimateReadOnly = false;
            this.txtSoTienPhat.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtSoTienPhat.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtSoTienPhat.Depth = 0;
            this.txtSoTienPhat.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtSoTienPhat.HideSelection = true;
            this.txtSoTienPhat.Hint = "Tiền Phạt";
            this.txtSoTienPhat.LeadingIcon = null;
            this.txtSoTienPhat.Location = new System.Drawing.Point(193, 6); // Sau Padding
            this.txtSoTienPhat.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.txtSoTienPhat.MaxLength = 50;
            this.txtSoTienPhat.MouseState = MaterialSkin.MouseState.OUT;
            this.txtSoTienPhat.Name = "txtSoTienPhat";
            this.txtSoTienPhat.PasswordChar = '\0';
            this.txtSoTienPhat.PrefixSuffixText = null;
            this.txtSoTienPhat.ReadOnly = true;
            this.txtSoTienPhat.SelectedText = "";
            this.txtSoTienPhat.SelectionLength = 0;
            this.txtSoTienPhat.SelectionStart = 0;
            this.txtSoTienPhat.ShortcutsEnabled = true;
            this.txtSoTienPhat.Size = new System.Drawing.Size(150, 48);
            this.txtSoTienPhat.TabIndex = 1;
            this.txtSoTienPhat.TabStop = false;
            this.txtSoTienPhat.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtSoTienPhat.TrailingIcon = null;
            this.txtSoTienPhat.UseSystemPasswordChar = false;
            this.txtSoTienPhat.UseTallSize = false;
            // 
            // dtpNgayTra
            // 
            this.dtpNgayTra.CustomFormat = "dd/MM/yyyy";
            this.dtpNgayTra.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dtpNgayTra.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpNgayTra.Location = new System.Drawing.Point(23, 17); // Sau Padding
            this.dtpNgayTra.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.dtpNgayTra.Name = "dtpNgayTra";
            this.dtpNgayTra.Size = new System.Drawing.Size(150, 26);
            this.dtpNgayTra.TabIndex = 0;
            // 
            // panelGrid
            // 
            this.panelGrid.Controls.Add(this.dgvPhieuMuonTra);
            this.panelGrid.Controls.Add(this.panelButtons);
            this.panelGrid.Dock = DockStyle.Fill;
            this.panelGrid.Location = new System.Drawing.Point(10, 260); // Vị trí sau panelReturnDetails và Padding
            this.panelGrid.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Padding = new Padding(10); // *** ĐỔI PADDING ***
            this.panelGrid.Size = new System.Drawing.Size(780, 330); // Kích thước sau Padding
            this.panelGrid.TabIndex = 2;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnThoat);
            this.panelButtons.Controls.Add(this.btnTraSach);
            this.panelButtons.Controls.Add(this.btnThem);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelButtons.Location = new System.Drawing.Point(10, 10); // Vị trí sau padding panelGrid
            this.panelButtons.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Padding = new Padding(3); // *** ĐỔI PADDING *** (Có thể giữ 0)
            this.panelButtons.Size = new System.Drawing.Size(760, 50); // Kích thước sau padding
            this.panelButtons.TabIndex = 0;
            // 
            // dgvPhieuMuonTra
            // 
            this.dgvPhieuMuonTra.AllowUserToAddRows = false;
            this.dgvPhieuMuonTra.AllowUserToDeleteRows = false;
            this.dgvPhieuMuonTra.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPhieuMuonTra.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPhieuMuonTra.Location = new Point(10, 60); // Vị trí sau panelButtons và padding panelGrid
            this.dgvPhieuMuonTra.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.dgvPhieuMuonTra.MultiSelect = false;
            this.dgvPhieuMuonTra.Name = "dgvPhieuMuonTra";
            this.dgvPhieuMuonTra.ReadOnly = true;
            this.dgvPhieuMuonTra.RowHeadersWidth = 51;
            this.dgvPhieuMuonTra.RowTemplate.Height = 24;
            this.dgvPhieuMuonTra.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPhieuMuonTra.Size = new System.Drawing.Size(760, 260); // Kích thước sau padding
            this.dgvPhieuMuonTra.TabIndex = 1;
            this.dgvPhieuMuonTra.SelectionChanged += new System.EventHandler(this.dgvPhieuMuonTra_SelectionChanged);
            this.dgvPhieuMuonTra.DoubleClick += new System.EventHandler(this.dgvPhieuMuonTra_DoubleClick);
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
            this.btnThoat.Location = new System.Drawing.Point(677, 7); // Điều chỉnh vị trí X
            this.btnThoat.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.btnThoat.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnThoat.Size = new System.Drawing.Size(80, 36);
            this.btnThoat.TabIndex = 2;
            this.btnThoat.Text = "Thoát";
            this.btnThoat.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text;
            this.btnThoat.UseAccentColor = false;
            this.btnThoat.UseVisualStyleBackColor = true;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // btnTraSach
            // 
            this.btnTraSach.AutoSize = false;
            this.btnTraSach.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnTraSach.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnTraSach.Depth = 0;
            this.btnTraSach.HighEmphasis = false;
            this.btnTraSach.Icon = null;
            this.btnTraSach.Location = new System.Drawing.Point(127, 7); // Vị trí sau btnThem
            this.btnTraSach.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.btnTraSach.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnTraSach.Name = "btnTraSach";
            this.btnTraSach.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnTraSach.Size = new System.Drawing.Size(86, 36);
            this.btnTraSach.TabIndex = 1;
            this.btnTraSach.Text = "Trả sách";
            this.btnTraSach.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnTraSach.UseAccentColor = false;
            this.btnTraSach.UseVisualStyleBackColor = true;
            this.btnTraSach.Click += new System.EventHandler(this.btnTraSach_Click);
            // 
            // btnThem
            // 
            this.btnThem.AutoSize = false;
            this.btnThem.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThem.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnThem.Depth = 0;
            this.btnThem.HighEmphasis = true;
            this.btnThem.Icon = null;
            this.btnThem.Location = new System.Drawing.Point(17, 7); // Vị trí sau Padding panelButtons
            this.btnThem.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.btnThem.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnThem.Name = "btnThem";
            this.btnThem.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnThem.Size = new System.Drawing.Size(96, 36);
            this.btnThem.TabIndex = 0;
            this.btnThem.Text = "Lập phiếu";
            this.btnThem.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnThem.UseAccentColor = false;
            this.btnThem.UseVisualStyleBackColor = true;
            this.btnThem.Click += new System.EventHandler(this.btnThem_Click);
            // 
            // ucQuanLyPhieuMuonTra
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.panelReturnDetails);
            this.Controls.Add(this.panelDetails);
            this.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.Name = "ucQuanLyPhieuMuonTra";
            this.Padding = new Padding(10); // *** ĐỔI PADDING ***
            this.Size = new System.Drawing.Size(800, 600);
            this.Load += new System.EventHandler(this.ucQuanLyPhieuMuonTra_Load);
            this.panelDetails.ResumeLayout(false);
            this.panelDetails.PerformLayout();
            this.panelReturnDetails.ResumeLayout(false);
            this.panelReturnDetails.PerformLayout();
            this.panelButtons.ResumeLayout(false); // Thêm
            this.panelButtons.PerformLayout(); // Thêm
            this.panelGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPhieuMuonTra)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}