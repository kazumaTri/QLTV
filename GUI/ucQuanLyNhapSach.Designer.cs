namespace GUI
{
    partial class ucQuanLyNhapSach
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucQuanLyNhapSach));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            materialCard1 = new MaterialSkin.Controls.MaterialCard();
            lblTongTienValue = new MaterialSkin.Controls.MaterialLabel();
            lblSoPhieuValue = new MaterialSkin.Controls.MaterialLabel();
            dtpNgayNhap = new DateTimePicker();
            materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
            materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            materialCard2 = new MaterialSkin.Controls.MaterialCard();
            btnThemSachVaoPhieu = new MaterialSkin.Controls.MaterialButton();
            txtDonGia = new MaterialSkin.Controls.MaterialTextBox2();
            numSoLuongNhap = new NumericUpDown();
            materialLabel6 = new MaterialSkin.Controls.MaterialLabel();
            cboSach = new MaterialSkin.Controls.MaterialComboBox();
            materialCard3 = new MaterialSkin.Controls.MaterialCard();
            dgvChiTietPhieuNhap = new DataGridView();
            colMaSach = new DataGridViewTextBoxColumn();
            colTenSach = new DataGridViewTextBoxColumn();
            colSoLuongNhap = new DataGridViewTextBoxColumn();
            colDonGia = new DataGridViewTextBoxColumn();
            colThanhTien = new DataGridViewTextBoxColumn();
            ctPhieuNhapDTOBindingSource = new BindingSource(components);
            materialLabel7 = new MaterialSkin.Controls.MaterialLabel();
            btnXoaSachKhoiPhieu = new MaterialSkin.Controls.MaterialButton();
            btnHuyPhieu = new MaterialSkin.Controls.MaterialButton();
            btnLuuPhieu = new MaterialSkin.Controls.MaterialButton();
            btnLapPhieuMoi = new MaterialSkin.Controls.MaterialButton();
            toolTip1 = new ToolTip(components);
            materialCard1.SuspendLayout();
            materialCard2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numSoLuongNhap).BeginInit();
            materialCard3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvChiTietPhieuNhap).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ctPhieuNhapDTOBindingSource).BeginInit();
            SuspendLayout();
            // 
            // materialCard1
            // 
            materialCard1.BackColor = Color.FromArgb(255, 255, 255);
            materialCard1.Controls.Add(lblTongTienValue);
            materialCard1.Controls.Add(lblSoPhieuValue);
            materialCard1.Controls.Add(dtpNgayNhap);
            materialCard1.Controls.Add(materialLabel3);
            materialCard1.Controls.Add(materialLabel2);
            materialCard1.Controls.Add(materialLabel1);
            materialCard1.Depth = 0;
            materialCard1.ForeColor = Color.FromArgb(222, 0, 0, 0);
            materialCard1.Location = new Point(13, 13); // Vị trí sau Padding UserControl
            materialCard1.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            materialCard1.MouseState = MaterialSkin.MouseState.HOVER;
            materialCard1.Name = "materialCard1";
            materialCard1.Padding = new Padding(10); // *** ĐỔI PADDING ***
            materialCard1.Size = new Size(1062, 82);
            materialCard1.TabIndex = 0;
            // 
            // lblTongTienValue
            // 
            lblTongTienValue.AutoSize = true;
            lblTongTienValue.Depth = 0;
            lblTongTienValue.Font = new Font("Roboto", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            lblTongTienValue.FontType = MaterialSkin.MaterialSkinManager.fontType.Button;
            lblTongTienValue.ForeColor = Color.Red;
            lblTongTienValue.Location = new Point(853, 34);
            lblTongTienValue.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            lblTongTienValue.MouseState = MaterialSkin.MouseState.HOVER;
            lblTongTienValue.Name = "lblTongTienValue";
            lblTongTienValue.Size = new Size(33, 17);
            lblTongTienValue.TabIndex = 5;
            lblTongTienValue.Text = "0 VNĐ";
            // 
            // lblSoPhieuValue
            // 
            lblSoPhieuValue.AutoSize = true;
            lblSoPhieuValue.Depth = 0;
            lblSoPhieuValue.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblSoPhieuValue.Location = new Point(121, 34);
            lblSoPhieuValue.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            lblSoPhieuValue.MouseState = MaterialSkin.MouseState.HOVER;
            lblSoPhieuValue.Name = "lblSoPhieuValue";
            lblSoPhieuValue.Size = new Size(47, 19);
            lblSoPhieuValue.TabIndex = 4;
            lblSoPhieuValue.Text = "(Auto)";
            // 
            // dtpNgayNhap
            // 
            dtpNgayNhap.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dtpNgayNhap.CustomFormat = "dd/MM/yyyy";
            dtpNgayNhap.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dtpNgayNhap.Format = DateTimePickerFormat.Custom;
            dtpNgayNhap.Location = new Point(457, 28);
            dtpNgayNhap.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            dtpNgayNhap.Name = "dtpNgayNhap";
            dtpNgayNhap.Size = new Size(216, 30);
            dtpNgayNhap.TabIndex = 3;
            // 
            // materialLabel3
            // 
            materialLabel3.AutoSize = true;
            materialLabel3.Depth = 0;
            materialLabel3.Font = new Font("Roboto", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            materialLabel3.FontType = MaterialSkin.MaterialSkinManager.fontType.Button;
            materialLabel3.Location = new Point(761, 34);
            materialLabel3.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
            materialLabel3.Name = "materialLabel3";
            materialLabel3.Size = new Size(65, 17);
            materialLabel3.TabIndex = 2;
            materialLabel3.Text = "Tổng tiền:";
            // 
            // materialLabel2
            // 
            materialLabel2.AutoSize = true;
            materialLabel2.Depth = 0;
            materialLabel2.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialLabel2.Location = new Point(353, 34);
            materialLabel2.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            materialLabel2.Name = "materialLabel2";
            materialLabel2.Size = new Size(83, 19);
            materialLabel2.TabIndex = 1;
            materialLabel2.Text = "Ngày nhập:";
            // 
            // materialLabel1
            // 
            materialLabel1.AutoSize = true;
            materialLabel1.Depth = 0;
            materialLabel1.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialLabel1.Location = new Point(30, 34);
            materialLabel1.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            materialLabel1.Name = "materialLabel1";
            materialLabel1.Size = new Size(68, 19);
            materialLabel1.TabIndex = 0;
            materialLabel1.Text = "Số phiếu:";
            // 
            // materialCard2
            // 
            materialCard2.BackColor = Color.FromArgb(255, 255, 255);
            materialCard2.Controls.Add(btnThemSachVaoPhieu);
            materialCard2.Controls.Add(txtDonGia);
            materialCard2.Controls.Add(numSoLuongNhap);
            materialCard2.Controls.Add(materialLabel6);
            materialCard2.Controls.Add(cboSach);
            materialCard2.Depth = 0;
            materialCard2.ForeColor = Color.FromArgb(222, 0, 0, 0);
            materialCard2.Location = new Point(13, 113); // Vị trí sau Padding UserControl và card 1
            materialCard2.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            materialCard2.MouseState = MaterialSkin.MouseState.HOVER;
            materialCard2.Name = "materialCard2";
            materialCard2.Padding = new Padding(10); // *** ĐỔI PADDING ***
            materialCard2.Size = new Size(1062, 109);
            materialCard2.TabIndex = 1;
            // 
            // btnThemSachVaoPhieu
            // 
            btnThemSachVaoPhieu.AutoSize = false;
            btnThemSachVaoPhieu.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnThemSachVaoPhieu.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnThemSachVaoPhieu.Depth = 0;
            btnThemSachVaoPhieu.HighEmphasis = true;
            btnThemSachVaoPhieu.Icon = (Image)resources.GetObject("btnThemSachVaoPhieu.Icon");
            btnThemSachVaoPhieu.Location = new Point(879, 32);
            btnThemSachVaoPhieu.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            btnThemSachVaoPhieu.MouseState = MaterialSkin.MouseState.HOVER;
            btnThemSachVaoPhieu.Name = "btnThemSachVaoPhieu";
            btnThemSachVaoPhieu.NoAccentTextColor = Color.Empty;
            btnThemSachVaoPhieu.Size = new System.Drawing.Size(163, 48);
            btnThemSachVaoPhieu.TabIndex = 4;
            btnThemSachVaoPhieu.Text = "Thêm sách";
            btnThemSachVaoPhieu.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnThemSachVaoPhieu.UseAccentColor = false;
            btnThemSachVaoPhieu.UseVisualStyleBackColor = true;
            btnThemSachVaoPhieu.Click += btnThemSachVaoPhieu_Click;
            // 
            // txtDonGia
            // 
            txtDonGia.AnimateReadOnly = false;
            txtDonGia.BackgroundImageLayout = ImageLayout.None;
            txtDonGia.CharacterCasing = CharacterCasing.Normal;
            txtDonGia.Depth = 0;
            txtDonGia.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtDonGia.HideSelection = true;
            txtDonGia.Hint = "Đơn giá";
            txtDonGia.LeadingIcon = null;
            txtDonGia.Location = new Point(655, 32);
            txtDonGia.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            txtDonGia.MaxLength = 32767;
            txtDonGia.MouseState = MaterialSkin.MouseState.OUT;
            txtDonGia.Name = "txtDonGia";
            txtDonGia.PasswordChar = '\0';
            txtDonGia.PrefixSuffixText = null;
            txtDonGia.ReadOnly = false;
            txtDonGia.RightToLeft = RightToLeft.No;
            txtDonGia.SelectedText = "";
            txtDonGia.SelectionLength = 0;
            txtDonGia.SelectionStart = 0;
            txtDonGia.ShortcutsEnabled = true;
            txtDonGia.Size = new System.Drawing.Size(191, 48);
            txtDonGia.TabIndex = 3;
            txtDonGia.TabStop = false;
            txtDonGia.TextAlign = HorizontalAlignment.Left;
            txtDonGia.TrailingIcon = null;
            txtDonGia.UseSystemPasswordChar = false;
            txtDonGia.UseTallSize = false;
            txtDonGia.KeyPress += txtDonGia_KeyPress;
            // 
            // numSoLuongNhap
            // 
            numSoLuongNhap.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            numSoLuongNhap.Location = new Point(502, 39);
            numSoLuongNhap.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            numSoLuongNhap.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numSoLuongNhap.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numSoLuongNhap.Name = "numSoLuongNhap";
            numSoLuongNhap.Size = new Size(104, 38);
            numSoLuongNhap.TabIndex = 2;
            numSoLuongNhap.TextAlign = HorizontalAlignment.Center;
            numSoLuongNhap.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // materialLabel6
            // 
            materialLabel6.AutoSize = true;
            materialLabel6.Depth = 0;
            materialLabel6.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialLabel6.Location = new Point(418, 45);
            materialLabel6.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            materialLabel6.MouseState = MaterialSkin.MouseState.HOVER;
            materialLabel6.Name = "materialLabel6";
            materialLabel6.Size = new Size(70, 19);
            materialLabel6.TabIndex = 1;
            materialLabel6.Text = "Số lượng:";
            // 
            // cboSach
            // 
            cboSach.AutoResize = false;
            cboSach.BackColor = Color.FromArgb(255, 255, 255);
            cboSach.Depth = 0;
            cboSach.DrawMode = DrawMode.OwnerDrawVariable;
            cboSach.DropDownHeight = 174;
            cboSach.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSach.DropDownWidth = 121;
            cboSach.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            cboSach.ForeColor = Color.FromArgb(222, 0, 0, 0);
            cboSach.FormattingEnabled = true;
            cboSach.Hint = "Chọn sách cần nhập";
            cboSach.IntegralHeight = false;
            cboSach.ItemHeight = 43;
            cboSach.Location = new Point(20, 31);
            cboSach.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            cboSach.MaxDropDownItems = 4;
            cboSach.MouseState = MaterialSkin.MouseState.OUT;
            cboSach.Name = "cboSach";
            cboSach.Size = new Size(368, 49);
            cboSach.StartIndex = 0;
            cboSach.TabIndex = 0;
            // 
            // materialCard3
            // 
            materialCard3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            materialCard3.BackColor = Color.FromArgb(255, 255, 255);
            materialCard3.Controls.Add(dgvChiTietPhieuNhap);
            materialCard3.Controls.Add(materialLabel7);
            materialCard3.Depth = 0;
            materialCard3.ForeColor = Color.FromArgb(222, 0, 0, 0);
            materialCard3.Location = new Point(13, 247); // Vị trí sau Padding UserControl và card 2
            materialCard3.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            materialCard3.MouseState = MaterialSkin.MouseState.HOVER;
            materialCard3.Name = "materialCard3";
            materialCard3.Padding = new Padding(10); // *** ĐỔI PADDING ***
            materialCard3.Size = new Size(1062, 330);
            materialCard3.TabIndex = 2;
            // 
            // dgvChiTietPhieuNhap
            // 
            dgvChiTietPhieuNhap.AllowUserToAddRows = false;
            dgvChiTietPhieuNhap.AllowUserToDeleteRows = false;
            dgvChiTietPhieuNhap.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(238, 239, 249);
            dgvChiTietPhieuNhap.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvChiTietPhieuNhap.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvChiTietPhieuNhap.AutoGenerateColumns = false;
            dgvChiTietPhieuNhap.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvChiTietPhieuNhap.BackgroundColor = Color.White;
            dgvChiTietPhieuNhap.BorderStyle = BorderStyle.None;
            dgvChiTietPhieuNhap.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvChiTietPhieuNhap.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(20, 25, 72);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvChiTietPhieuNhap.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvChiTietPhieuNhap.ColumnHeadersHeight = 40;
            dgvChiTietPhieuNhap.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvChiTietPhieuNhap.Columns.AddRange(new DataGridViewColumn[] { colMaSach, colTenSach, colSoLuongNhap, colDonGia, colThanhTien });
            dgvChiTietPhieuNhap.DataSource = ctPhieuNhapDTOBindingSource;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Window;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(222, 0, 0, 0);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(192, 192, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.Black;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvChiTietPhieuNhap.DefaultCellStyle = dataGridViewCellStyle3;
            dgvChiTietPhieuNhap.EnableHeadersVisualStyles = false;
            dgvChiTietPhieuNhap.Location = new Point(13, 48); // Vị trí sau Label và Padding
            dgvChiTietPhieuNhap.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            dgvChiTietPhieuNhap.MultiSelect = false;
            dgvChiTietPhieuNhap.Name = "dgvChiTietPhieuNhap";
            dgvChiTietPhieuNhap.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Control;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle4.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            dgvChiTietPhieuNhap.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dgvChiTietPhieuNhap.RowHeadersVisible = false;
            dgvChiTietPhieuNhap.RowHeadersWidth = 51;
            dgvChiTietPhieuNhap.RowTemplate.Height = 35;
            dgvChiTietPhieuNhap.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvChiTietPhieuNhap.Size = new Size(1036, 257); // Kích thước sau Padding
            dgvChiTietPhieuNhap.TabIndex = 1;
            dgvChiTietPhieuNhap.SelectionChanged += dgvChiTietPhieuNhap_SelectionChanged;
            // 
            // colMaSach
            // 
            colMaSach.DataPropertyName = "MaSach";
            colMaSach.FillWeight = 70F;
            colMaSach.HeaderText = "Mã Sách";
            colMaSach.MinimumWidth = 6;
            colMaSach.Name = "colMaSach";
            colMaSach.ReadOnly = true;
            // 
            // colTenSach
            // 
            colTenSach.DataPropertyName = "TenSach";
            colTenSach.FillWeight = 150F;
            colTenSach.HeaderText = "Tên Sách";
            colTenSach.MinimumWidth = 6;
            colTenSach.Name = "colTenSach";
            colTenSach.ReadOnly = true;
            // 
            // colSoLuongNhap
            // 
            colSoLuongNhap.DataPropertyName = "SoLuongNhap";
            colSoLuongNhap.FillWeight = 60F;
            colSoLuongNhap.HeaderText = "Số Lượng";
            colSoLuongNhap.MinimumWidth = 6;
            colSoLuongNhap.Name = "colSoLuongNhap";
            colSoLuongNhap.ReadOnly = true;
            // 
            // colDonGia
            // 
            colDonGia.DataPropertyName = "DonGia";
            colDonGia.FillWeight = 80F;
            colDonGia.HeaderText = "Đơn Giá";
            colDonGia.MinimumWidth = 6;
            colDonGia.Name = "colDonGia";
            colDonGia.ReadOnly = true;
            // 
            // colThanhTien
            // 
            colThanhTien.DataPropertyName = "ThanhTien";
            colThanhTien.FillWeight = 90F;
            colThanhTien.HeaderText = "Thành Tiền";
            colThanhTien.MinimumWidth = 6;
            colThanhTien.Name = "colThanhTien";
            colThanhTien.ReadOnly = true;
            // 
            // ctPhieuNhapDTOBindingSource
            // 
            ctPhieuNhapDTOBindingSource.DataSource = typeof(DTO.CtPhieuNhapDTO);
            // 
            // materialLabel7
            // 
            materialLabel7.AutoSize = true;
            materialLabel7.Depth = 0;
            materialLabel7.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialLabel7.FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1;
            materialLabel7.Location = new Point(13, 13); // Vị trí sau Padding
            materialLabel7.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            materialLabel7.MouseState = MaterialSkin.MouseState.HOVER;
            materialLabel7.Name = "materialLabel7";
            materialLabel7.Size = new Size(147, 19);
            materialLabel7.TabIndex = 0;
            materialLabel7.Text = "Chi tiết phiếu nhập:";
            // 
            // btnXoaSachKhoiPhieu
            // 
            btnXoaSachKhoiPhieu.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnXoaSachKhoiPhieu.AutoSize = false;
            btnXoaSachKhoiPhieu.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnXoaSachKhoiPhieu.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnXoaSachKhoiPhieu.Depth = 0;
            btnXoaSachKhoiPhieu.Enabled = false;
            btnXoaSachKhoiPhieu.HighEmphasis = true;
            btnXoaSachKhoiPhieu.Icon = (Image)resources.GetObject("btnXoaSachKhoiPhieu.Icon");
            btnXoaSachKhoiPhieu.Location = new Point(13, 595); // Vị trí sau Padding UserControl
            btnXoaSachKhoiPhieu.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            btnXoaSachKhoiPhieu.MouseState = MaterialSkin.MouseState.HOVER;
            btnXoaSachKhoiPhieu.Name = "btnXoaSachKhoiPhieu";
            btnXoaSachKhoiPhieu.NoAccentTextColor = Color.Empty;
            btnXoaSachKhoiPhieu.Size = new System.Drawing.Size(209, 48);
            btnXoaSachKhoiPhieu.TabIndex = 3;
            btnXoaSachKhoiPhieu.Text = "Xóa sách khỏi phiếu";
            btnXoaSachKhoiPhieu.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            btnXoaSachKhoiPhieu.UseAccentColor = false;
            btnXoaSachKhoiPhieu.UseVisualStyleBackColor = true;
            btnXoaSachKhoiPhieu.Click += btnXoaSachKhoiPhieu_Click;
            // 
            // btnHuyPhieu
            // 
            btnHuyPhieu.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnHuyPhieu.AutoSize = false;
            btnHuyPhieu.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnHuyPhieu.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnHuyPhieu.Depth = 0;
            btnHuyPhieu.HighEmphasis = false;
            btnHuyPhieu.Icon = null;
            btnHuyPhieu.Location = new Point(728, 595);
            btnHuyPhieu.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            btnHuyPhieu.MouseState = MaterialSkin.MouseState.HOVER;
            btnHuyPhieu.Name = "btnHuyPhieu";
            btnHuyPhieu.NoAccentTextColor = Color.Empty;
            btnHuyPhieu.Size = new System.Drawing.Size(150, 48);
            btnHuyPhieu.TabIndex = 5;
            btnHuyPhieu.Text = "Hủy phiếu";
            btnHuyPhieu.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text;
            btnHuyPhieu.UseAccentColor = false;
            btnHuyPhieu.UseVisualStyleBackColor = true;
            btnHuyPhieu.Click += btnHuyPhieu_Click;
            // 
            // btnLuuPhieu
            // 
            btnLuuPhieu.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnLuuPhieu.AutoSize = false;
            btnLuuPhieu.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnLuuPhieu.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnLuuPhieu.Depth = 0;
            btnLuuPhieu.HighEmphasis = true;
            btnLuuPhieu.Icon = (Image)resources.GetObject("btnLuuPhieu.Icon");
            btnLuuPhieu.Location = new Point(905, 595);
            btnLuuPhieu.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            btnLuuPhieu.MouseState = MaterialSkin.MouseState.HOVER;
            btnLuuPhieu.Name = "btnLuuPhieu";
            btnLuuPhieu.NoAccentTextColor = Color.Empty;
            btnLuuPhieu.Size = new System.Drawing.Size(175, 48);
            btnLuuPhieu.TabIndex = 6;
            btnLuuPhieu.Text = "Lưu phiếu nhập";
            btnLuuPhieu.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnLuuPhieu.UseAccentColor = true;
            btnLuuPhieu.UseVisualStyleBackColor = true;
            btnLuuPhieu.Click += btnLuuPhieu_Click;
            // 
            // btnLapPhieuMoi
            // 
            btnLapPhieuMoi.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnLapPhieuMoi.AutoSize = false;
            btnLapPhieuMoi.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnLapPhieuMoi.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnLapPhieuMoi.Depth = 0;
            btnLapPhieuMoi.HighEmphasis = true;
            btnLapPhieuMoi.Icon = (Image)resources.GetObject("btnLapPhieuMoi.Icon");
            btnLapPhieuMoi.Location = new Point(533, 595);
            btnLapPhieuMoi.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            btnLapPhieuMoi.MouseState = MaterialSkin.MouseState.HOVER;
            btnLapPhieuMoi.Name = "btnLapPhieuMoi";
            btnLapPhieuMoi.NoAccentTextColor = Color.Empty;
            btnLapPhieuMoi.Size = new System.Drawing.Size(167, 48);
            btnLapPhieuMoi.TabIndex = 4;
            btnLapPhieuMoi.Text = "Lập phiếu mới";
            btnLapPhieuMoi.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnLapPhieuMoi.UseAccentColor = false;
            btnLapPhieuMoi.UseVisualStyleBackColor = true;
            btnLapPhieuMoi.Click += btnLapPhieuMoi_Click;
            // 
            // ucQuanLyNhapSach
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnLapPhieuMoi);
            Controls.Add(btnLuuPhieu);
            Controls.Add(btnHuyPhieu);
            Controls.Add(btnXoaSachKhoiPhieu);
            Controls.Add(materialCard3);
            Controls.Add(materialCard2);
            Controls.Add(materialCard1);
            Margin = new Padding(3); // *** ĐỔI MARGIN ***
            Name = "ucQuanLyNhapSach";
            Padding = new Padding(10); // *** ĐỔI PADDING ***
            Size = new System.Drawing.Size(1100, 660);
            Load += ucQuanLyNhapSach_Load;
            materialCard1.ResumeLayout(false);
            materialCard1.PerformLayout();
            materialCard2.ResumeLayout(false);
            materialCard2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numSoLuongNhap).EndInit();
            materialCard3.ResumeLayout(false);
            materialCard3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvChiTietPhieuNhap).EndInit();
            ((System.ComponentModel.ISupportInitialize)ctPhieuNhapDTOBindingSource).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private MaterialSkin.Controls.MaterialCard materialCard1;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialLabel materialLabel3;
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
        private DateTimePicker dtpNgayNhap;
        private MaterialSkin.Controls.MaterialLabel lblTongTienValue;
        private MaterialSkin.Controls.MaterialLabel lblSoPhieuValue;
        private MaterialSkin.Controls.MaterialCard materialCard2;
        private MaterialSkin.Controls.MaterialComboBox cboSach;
        private MaterialSkin.Controls.MaterialLabel materialLabel6;
        private NumericUpDown numSoLuongNhap;
        private MaterialSkin.Controls.MaterialTextBox2 txtDonGia;
        private MaterialSkin.Controls.MaterialButton btnThemSachVaoPhieu;
        private MaterialSkin.Controls.MaterialCard materialCard3;
        private MaterialSkin.Controls.MaterialLabel materialLabel7;
        private DataGridView dgvChiTietPhieuNhap;
        private MaterialSkin.Controls.MaterialButton btnXoaSachKhoiPhieu;
        private MaterialSkin.Controls.MaterialButton btnHuyPhieu;
        private MaterialSkin.Controls.MaterialButton btnLuuPhieu;
        private MaterialSkin.Controls.MaterialButton btnLapPhieuMoi;
        private BindingSource ctPhieuNhapDTOBindingSource;
        private DataGridViewTextBoxColumn colMaSach;
        private DataGridViewTextBoxColumn colTenSach;
        private DataGridViewTextBoxColumn colSoLuongNhap;
        private DataGridViewTextBoxColumn colDonGia;
        private DataGridViewTextBoxColumn colThanhTien;
        private ToolTip toolTip1;
    }
}