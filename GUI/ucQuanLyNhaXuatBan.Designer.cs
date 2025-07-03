namespace GUI
{
    partial class ucQuanLyNhaXuatBan
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
            panelToolBar = new Panel();
            btnHuy = new Button(); // Di chuyển lên trước để khai báo
            btnLuu = new Button(); // Di chuyển lên trước để khai báo
            btnXoa = new Button();
            btnSua = new Button();
            btnThem = new Button();
            panelInput = new Panel();
            txtEmail = new TextBox();
            lblEmail = new Label();
            txtSdt = new TextBox();
            lblSdt = new Label();
            txtDiaChi = new TextBox();
            lblDiaChi = new Label();
            txtTenNXB = new TextBox();
            lblTenNXB = new Label();
            txtIdNXB = new TextBox();
            lblIdNXB = new Label();
            panelTimKiem = new Panel();
            btnTimKiem = new Button();
            txtTimKiem = new TextBox();
            lblTimKiem = new Label();
            dgvNhaXuatBan = new DataGridView();
            lblTitle = new Label();
            panelToolBar.SuspendLayout();
            panelInput.SuspendLayout();
            panelTimKiem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvNhaXuatBan).BeginInit();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.Location = new Point(13, 13); // Vị trí sau Padding UserControl
            lblTitle.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(774, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "QUẢN LÝ NHÀ XUẤT BẢN";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelToolBar
            // 
            panelToolBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelToolBar.Controls.Add(btnHuy);
            panelToolBar.Controls.Add(btnLuu);
            panelToolBar.Controls.Add(btnXoa);
            panelToolBar.Controls.Add(btnSua);
            panelToolBar.Controls.Add(btnThem);
            panelToolBar.Location = new Point(13, 190); // Điều chỉnh vị trí Y
            panelToolBar.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            panelToolBar.Name = "panelToolBar";
            panelToolBar.Padding = new Padding(10); // *** ĐỔI PADDING *** (Có thể giữ 0 nếu không cần padding trong toolbar)
            panelToolBar.Size = new Size(774, 50);
            panelToolBar.TabIndex = 2;
            // 
            // btnHuy
            // 
            btnHuy.Location = new Point(473, 10); // Vị trí sau Padding
            btnHuy.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            btnHuy.Name = "btnHuy";
            btnHuy.Size = new Size(100, 30);
            btnHuy.TabIndex = 4;
            btnHuy.Text = "Hủy";
            btnHuy.UseVisualStyleBackColor = true;
            btnHuy.Visible = false;
            // Thêm sự kiện Click trong file .cs: btnHuy.Click += new System.EventHandler(this.btnHuy_Click);
            // 
            // btnLuu
            // 
            btnLuu.Location = new Point(358, 10); // Vị trí sau Padding
            btnLuu.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            btnLuu.Name = "btnLuu";
            btnLuu.Size = new Size(100, 30);
            btnLuu.TabIndex = 3;
            btnLuu.Text = "Lưu";
            btnLuu.UseVisualStyleBackColor = true;
            btnLuu.Visible = false;
            // Thêm sự kiện Click trong file .cs: btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            // 
            // btnXoa
            // 
            btnXoa.Location = new Point(243, 10); // Vị trí sau Padding
            btnXoa.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            btnXoa.Name = "btnXoa";
            btnXoa.Size = new Size(100, 30);
            btnXoa.TabIndex = 2;
            btnXoa.Text = "Xóa";
            btnXoa.UseVisualStyleBackColor = true;
            // Thêm sự kiện Click trong file .cs: btnXoa.Click += new System.EventHandler(this.btnXoa_Click);
            // 
            // btnSua
            // 
            btnSua.Location = new Point(128, 10); // Vị trí sau Padding
            btnSua.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            btnSua.Name = "btnSua";
            btnSua.Size = new Size(100, 30);
            btnSua.TabIndex = 1;
            btnSua.Text = "Sửa";
            btnSua.UseVisualStyleBackColor = true;
            // Thêm sự kiện Click trong file .cs: btnSua.Click += new System.EventHandler(this.btnSua_Click);
            // 
            // btnThem
            // 
            btnThem.Location = new Point(13, 10); // Vị trí sau Padding
            btnThem.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            btnThem.Name = "btnThem";
            btnThem.Size = new Size(100, 30);
            btnThem.TabIndex = 0;
            btnThem.Text = "Thêm";
            btnThem.UseVisualStyleBackColor = true;
            // Thêm sự kiện Click trong file .cs: btnThem.Click += new System.EventHandler(this.btnThem_Click);
            // 
            // panelInput
            // 
            panelInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelInput.Controls.Add(txtEmail);
            panelInput.Controls.Add(lblEmail);
            panelInput.Controls.Add(txtSdt);
            panelInput.Controls.Add(lblSdt);
            panelInput.Controls.Add(txtDiaChi);
            panelInput.Controls.Add(lblDiaChi);
            panelInput.Controls.Add(txtTenNXB);
            panelInput.Controls.Add(lblTenNXB);
            panelInput.Controls.Add(txtIdNXB);
            panelInput.Controls.Add(lblIdNXB);
            panelInput.Location = new Point(13, 46); // Vị trí sau Padding UserControl và lblTitle
            panelInput.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            panelInput.Name = "panelInput";
            panelInput.Padding = new Padding(10); // *** ĐỔI PADDING ***
            panelInput.Size = new Size(774, 100);
            panelInput.TabIndex = 0;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(483, 70); // Vị trí sau Padding panelInput
            txtEmail.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            txtEmail.MaxLength = 100;
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(278, 23);
            txtEmail.TabIndex = 9;
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(393, 73); // Vị trí sau Padding panelInput
            lblEmail.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(39, 15);
            lblEmail.TabIndex = 8;
            lblEmail.Text = "Email:";
            // 
            // txtSdt
            // 
            txtSdt.Location = new Point(483, 40); // Vị trí sau Padding panelInput
            txtSdt.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            txtSdt.MaxLength = 15;
            txtSdt.Name = "txtSdt";
            txtSdt.Size = new Size(278, 23);
            txtSdt.TabIndex = 7;
            // 
            // lblSdt
            // 
            lblSdt.AutoSize = true;
            lblSdt.Location = new Point(393, 43); // Vị trí sau Padding panelInput
            lblSdt.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            lblSdt.Name = "lblSdt";
            lblSdt.Size = new Size(79, 15);
            lblSdt.TabIndex = 6;
            lblSdt.Text = "Số điện thoại:";
            // 
            // txtDiaChi
            // 
            txtDiaChi.Location = new Point(93, 70); // Vị trí sau Padding panelInput
            txtDiaChi.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            txtDiaChi.MaxLength = 200;
            txtDiaChi.Name = "txtDiaChi";
            txtDiaChi.Size = new Size(280, 23);
            txtDiaChi.TabIndex = 5;
            // 
            // lblDiaChi
            // 
            lblDiaChi.AutoSize = true;
            lblDiaChi.Location = new Point(13, 73); // Vị trí sau Padding panelInput
            lblDiaChi.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            lblDiaChi.Name = "lblDiaChi";
            lblDiaChi.Size = new Size(46, 15);
            lblDiaChi.TabIndex = 4;
            lblDiaChi.Text = "Địa chỉ:";
            // 
            // txtTenNXB
            // 
            txtTenNXB.Location = new Point(93, 40); // Vị trí sau Padding panelInput
            txtTenNXB.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            txtTenNXB.MaxLength = 100;
            txtTenNXB.Name = "txtTenNXB";
            txtTenNXB.Size = new Size(280, 23);
            txtTenNXB.TabIndex = 3;
            // 
            // lblTenNXB
            // 
            lblTenNXB.AutoSize = true;
            lblTenNXB.Location = new Point(13, 43); // Vị trí sau Padding panelInput
            lblTenNXB.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            lblTenNXB.Name = "lblTenNXB";
            lblTenNXB.Size = new Size(55, 15);
            lblTenNXB.TabIndex = 2;
            lblTenNXB.Text = "Tên NXB:";
            // 
            // txtIdNXB
            // 
            txtIdNXB.Location = new Point(93, 10); // Vị trí sau Padding panelInput
            txtIdNXB.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            txtIdNXB.Name = "txtIdNXB";
            txtIdNXB.ReadOnly = true;
            txtIdNXB.Size = new Size(150, 23);
            txtIdNXB.TabIndex = 1;
            txtIdNXB.TabStop = false;
            // 
            // lblIdNXB
            // 
            lblIdNXB.AutoSize = true;
            lblIdNXB.Location = new Point(13, 13); // Vị trí sau Padding panelInput
            lblIdNXB.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            lblIdNXB.Name = "lblIdNXB";
            lblIdNXB.Size = new Size(51, 15);
            lblIdNXB.TabIndex = 0;
            lblIdNXB.Text = "Mã NXB:";
            // 
            // panelTimKiem
            // 
            panelTimKiem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelTimKiem.Controls.Add(btnTimKiem);
            panelTimKiem.Controls.Add(txtTimKiem);
            panelTimKiem.Controls.Add(lblTimKiem);
            panelTimKiem.Location = new Point(13, 149); // Điều chỉnh vị trí Y
            panelTimKiem.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            panelTimKiem.Name = "panelTimKiem";
            panelTimKiem.Padding = new Padding(10); // *** ĐỔI PADDING *** (Có thể giữ 0 nếu không cần)
            panelTimKiem.Size = new Size(774, 40);
            panelTimKiem.TabIndex = 1;
            // 
            // btnTimKiem
            // 
            btnTimKiem.Location = new Point(483, 5); // Vị trí sau Padding
            btnTimKiem.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            btnTimKiem.Name = "btnTimKiem";
            btnTimKiem.Size = new Size(100, 30);
            btnTimKiem.TabIndex = 2;
            btnTimKiem.Text = "Tìm kiếm";
            btnTimKiem.UseVisualStyleBackColor = true;
            // Thêm sự kiện Click trong file .cs: btnTimKiem.Click += new System.EventHandler(this.btnTimKiem_Click);
            // 
            // txtTimKiem
            // 
            txtTimKiem.Location = new Point(93, 10); // Vị trí sau Padding
            txtTimKiem.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            txtTimKiem.Name = "txtTimKiem";
            txtTimKiem.Size = new Size(370, 23);
            txtTimKiem.TabIndex = 1;
            // 
            // lblTimKiem
            // 
            lblTimKiem.AutoSize = true;
            lblTimKiem.Location = new Point(13, 13); // Vị trí sau Padding
            lblTimKiem.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            lblTimKiem.Name = "lblTimKiem";
            lblTimKiem.Size = new Size(59, 15);
            lblTimKiem.TabIndex = 0;
            lblTimKiem.Text = "Tìm kiếm:";
            // 
            // dgvNhaXuatBan
            // 
            dgvNhaXuatBan.AllowUserToAddRows = false;
            dgvNhaXuatBan.AllowUserToDeleteRows = false;
            dgvNhaXuatBan.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvNhaXuatBan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNhaXuatBan.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvNhaXuatBan.Location = new Point(13, 243); // Điều chỉnh vị trí Y
            dgvNhaXuatBan.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            dgvNhaXuatBan.MultiSelect = false;
            dgvNhaXuatBan.Name = "dgvNhaXuatBan";
            dgvNhaXuatBan.ReadOnly = true;
            dgvNhaXuatBan.RowTemplate.Height = 25;
            dgvNhaXuatBan.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNhaXuatBan.Size = new Size(774, 344); // Điều chỉnh kích thước
            dgvNhaXuatBan.TabIndex = 3;
            // 
            // ucQuanLyNhaXuatBan
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(dgvNhaXuatBan);
            Controls.Add(panelTimKiem);
            Controls.Add(panelToolBar);
            Controls.Add(panelInput);
            Controls.Add(lblTitle);
            Margin = new Padding(3); // *** ĐỔI MARGIN ***
            Name = "ucQuanLyNhaXuatBan";
            Padding = new Padding(10); // *** ĐỔI PADDING ***
            Size = new Size(800, 600);
            panelToolBar.ResumeLayout(false);
            panelInput.ResumeLayout(false);
            panelInput.PerformLayout();
            panelTimKiem.ResumeLayout(false);
            panelTimKiem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvNhaXuatBan).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private Panel panelToolBar;
        private Button btnXoa;
        private Button btnSua;
        private Button btnThem;
        private Panel panelInput;
        private TextBox txtEmail;
        private Label lblEmail;
        private TextBox txtSdt;
        private Label lblSdt;
        private TextBox txtDiaChi;
        private Label lblDiaChi;
        private TextBox txtTenNXB;
        private Label lblTenNXB;
        private TextBox txtIdNXB;
        private Label lblIdNXB;
        private Panel panelTimKiem;
        private Button btnTimKiem;
        private TextBox txtTimKiem;
        private Label lblTimKiem;
        private DataGridView dgvNhaXuatBan;
        private Button btnLuu;
        private Button btnHuy;
        private Label lblTitle;
    }
}