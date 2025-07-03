using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Windows.Forms.DataVisualization.Charting;

namespace GUI // Đảm bảo namespace này đúng với project của bạn
{
    partial class ucTrangChu // Kế thừa từ UserControl
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
            // --- KHAI BÁO CHO CHART ---
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            // --- KHAI BÁO CHO LISTVIEW ---
            this.colTime = new System.Windows.Forms.ColumnHeader();
            this.colActivity = new System.Windows.Forms.ColumnHeader();

            // --- KHỞI TẠO CÁC CONTROL ---
            this.lblTitle = new MaterialSkin.Controls.MaterialLabel();
            this.cardSoSach = new MaterialSkin.Controls.MaterialCard();
            this.lblSoSachValue = new MaterialSkin.Controls.MaterialLabel();
            this.lblSoSachDesc = new MaterialSkin.Controls.MaterialLabel();
            this.picIconSach = new System.Windows.Forms.PictureBox();
            this.cardDocGia = new MaterialSkin.Controls.MaterialCard();
            this.lblDocGiaValue = new MaterialSkin.Controls.MaterialLabel();
            this.lblDocGiaDesc = new MaterialSkin.Controls.MaterialLabel();
            this.picIconDocGia = new System.Windows.Forms.PictureBox();
            this.cardDangMuon = new MaterialSkin.Controls.MaterialCard();
            this.lblDangMuonValue = new MaterialSkin.Controls.MaterialLabel();
            this.lblDangMuonDesc = new MaterialSkin.Controls.MaterialLabel();
            this.picIconDangMuon = new System.Windows.Forms.PictureBox();
            this.cardQuaHan = new MaterialSkin.Controls.MaterialCard();
            this.lblQuaHanValue = new MaterialSkin.Controls.MaterialLabel();
            this.lblQuaHanDesc = new MaterialSkin.Controls.MaterialLabel();
            this.picIconQuaHan = new System.Windows.Forms.PictureBox();
            this.cardTopTuaSach = new MaterialSkin.Controls.MaterialCard();
            this.lblTopTuaSach3 = new MaterialSkin.Controls.MaterialLabel();
            this.lblTopTuaSach2 = new MaterialSkin.Controls.MaterialLabel();
            this.lblTopTuaSach1 = new MaterialSkin.Controls.MaterialLabel();
            this.lblTopTuaSachTitle = new MaterialSkin.Controls.MaterialLabel();
            this.cardTopTheLoai = new MaterialSkin.Controls.MaterialCard();
            this.lblTopTheLoai3 = new MaterialSkin.Controls.MaterialLabel();
            this.lblTopTheLoai2 = new MaterialSkin.Controls.MaterialLabel();
            this.lblTopTheLoai1 = new MaterialSkin.Controls.MaterialLabel();
            this.lblTopTheLoaiTitle = new MaterialSkin.Controls.MaterialLabel();
            this.chartTheLoai = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartLuotMuonThang = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.cardRecentActivity = new MaterialSkin.Controls.MaterialCard();
            this.lblRecentActivityTitle = new MaterialSkin.Controls.MaterialLabel();
            this.lvRecentActivity = new System.Windows.Forms.ListView();

            // --- KHỞI TẠO TableLayoutPanel MỚI ---
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();

            // --- SuspendLayout & BeginInit ---
            this.SuspendLayout();
            this.mainTableLayoutPanel.SuspendLayout(); // Tạm dừng layout cho TLP mới
            this.cardSoSach.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIconSach)).BeginInit();
            this.cardDocGia.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIconDocGia)).BeginInit();
            this.cardDangMuon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIconDangMuon)).BeginInit();
            this.cardQuaHan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIconQuaHan)).BeginInit();
            this.cardTopTuaSach.SuspendLayout();
            this.cardTopTheLoai.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartTheLoai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartLuotMuonThang)).BeginInit();
            this.cardRecentActivity.SuspendLayout();

            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Depth = 0;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top; // Giữ nguyên Dock Top
            this.lblTitle.Font = new System.Drawing.Font("Roboto", 34F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblTitle.FontType = MaterialSkin.MaterialSkinManager.fontType.H4;
            this.lblTitle.HighEmphasis = true;
            this.lblTitle.Location = new System.Drawing.Point(10, 10); // Giữ nguyên vị trí gốc
            this.lblTitle.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10); // Giữ nguyên Margin
            this.lblTitle.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1180, 41); // Kích thước có thể điều chỉnh sau
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Trang Chủ";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.TopLeft; // Giữ nguyên căn lề

            //
            // mainTableLayoutPanel
            //
            this.mainTableLayoutPanel.ColumnCount = 4; // 4 cột
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill; // Lấp đầy không gian còn lại
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(10, 61); // Đặt dưới lblTitle (10 + 41 + 10 = 61)
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.Padding = new System.Windows.Forms.Padding(10); // Padding bên trong TLP
            this.mainTableLayoutPanel.RowCount = 4; // 4 hàng
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F)); // Hàng 0: Stats Cards (Cao 150px)
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 180F)); // Hàng 1: Top Lists (Cao 180px)
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));   // Hàng 2: Charts (Chiếm 55% không gian còn lại)
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));   // Hàng 3: Activity (Chiếm 45% không gian còn lại)
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(1180, 729); // Kích thước TLP (sẽ được Dock=Fill điều chỉnh)
            this.mainTableLayoutPanel.TabIndex = 1; // Sau lblTitle

            // --- THÊM CONTROLS VÀO CÁC Ô CỦA mainTableLayoutPanel ---
            // Hàng 0
            this.mainTableLayoutPanel.Controls.Add(this.cardSoSach, 0, 0);
            this.mainTableLayoutPanel.Controls.Add(this.cardDocGia, 1, 0);
            this.mainTableLayoutPanel.Controls.Add(this.cardDangMuon, 2, 0);
            this.mainTableLayoutPanel.Controls.Add(this.cardQuaHan, 3, 0);
            // Hàng 1
            this.mainTableLayoutPanel.SetColumnSpan(this.cardTopTuaSach, 2); // Gộp 2 cột
            this.mainTableLayoutPanel.Controls.Add(this.cardTopTuaSach, 0, 1);
            this.mainTableLayoutPanel.SetColumnSpan(this.cardTopTheLoai, 2); // Gộp 2 cột
            this.mainTableLayoutPanel.Controls.Add(this.cardTopTheLoai, 2, 1);
            // Hàng 2
            this.mainTableLayoutPanel.SetColumnSpan(this.chartTheLoai, 2); // Gộp 2 cột
            this.mainTableLayoutPanel.Controls.Add(this.chartTheLoai, 0, 2);
            this.mainTableLayoutPanel.SetColumnSpan(this.chartLuotMuonThang, 2); // Gộp 2 cột
            this.mainTableLayoutPanel.Controls.Add(this.chartLuotMuonThang, 2, 2);
            // Hàng 3
            this.mainTableLayoutPanel.SetColumnSpan(this.cardRecentActivity, 4); // Gộp 4 cột
            this.mainTableLayoutPanel.Controls.Add(this.cardRecentActivity, 0, 3);

            // --- CẤU HÌNH CHI TIẾT TỪNG CONTROL ---

            //
            // cardSoSach
            //
            this.cardSoSach.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cardSoSach.Controls.Add(this.lblSoSachValue); // Thêm control con
            this.cardSoSach.Controls.Add(this.lblSoSachDesc);  // Thêm control con
            this.cardSoSach.Controls.Add(this.picIconSach);    // Thêm control con
            this.cardSoSach.Depth = 1;
            this.cardSoSach.Dock = System.Windows.Forms.DockStyle.Fill; // Lấp đầy ô TLP
            this.cardSoSach.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            // Location và Size được quản lý bởi TLP nên không cần set ở đây
            this.cardSoSach.Margin = new System.Windows.Forms.Padding(14); // Giữ Margin để tạo khoảng cách
            this.cardSoSach.MouseState = MaterialSkin.MouseState.HOVER;
            this.cardSoSach.Name = "cardSoSach";
            this.cardSoSach.Padding = new System.Windows.Forms.Padding(14); // Giữ Padding bên trong card
            this.cardSoSach.TabIndex = 0; // Cập nhật TabIndex nếu cần
            this.cardSoSach.Click += new System.EventHandler(this.CardSoSach_Click);
            // lblSoSachValue (Bên trong cardSoSach)
            this.lblSoSachValue.Anchor = System.Windows.Forms.AnchorStyles.Left; // Neo vào trái
            this.lblSoSachValue.AutoSize = true;
            this.lblSoSachValue.Depth = 0;
            this.lblSoSachValue.Font = new System.Drawing.Font("Roboto", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblSoSachValue.FontType = MaterialSkin.MaterialSkinManager.fontType.H5;
            this.lblSoSachValue.Location = new System.Drawing.Point(17, 24);
            this.lblSoSachValue.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblSoSachValue.Name = "lblSoSachValue";
            this.lblSoSachValue.Size = new System.Drawing.Size(25, 29);
            this.lblSoSachValue.TabIndex = 1;
            this.lblSoSachValue.Text = "..."; // Giá trị sẽ được load từ code-behind
            // lblSoSachDesc (Bên trong cardSoSach)
            this.lblSoSachDesc.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSoSachDesc.AutoSize = true;
            this.lblSoSachDesc.Depth = 0;
            this.lblSoSachDesc.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblSoSachDesc.Location = new System.Drawing.Point(17, 65);
            this.lblSoSachDesc.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblSoSachDesc.Name = "lblSoSachDesc";
            this.lblSoSachDesc.Size = new System.Drawing.Size(94, 19);
            this.lblSoSachDesc.TabIndex = 2;
            this.lblSoSachDesc.Text = "Tổng số sách";
            // picIconSach (Bên trong cardSoSach)
            this.picIconSach.Anchor = System.Windows.Forms.AnchorStyles.Right; // Neo vào phải
            this.picIconSach.Location = new System.Drawing.Point(180, 36); // Vị trí tương đối trong card
            this.picIconSach.Name = "picIconSach";
            this.picIconSach.Size = new System.Drawing.Size(48, 48);
            this.picIconSach.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picIconSach.TabIndex = 0;
            this.picIconSach.TabStop = false;

            //
            // cardDocGia (Tương tự cardSoSach)
            //
            this.cardDocGia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cardDocGia.Controls.Add(this.lblDocGiaValue);
            this.cardDocGia.Controls.Add(this.lblDocGiaDesc);
            this.cardDocGia.Controls.Add(this.picIconDocGia);
            this.cardDocGia.Depth = 1;
            this.cardDocGia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardDocGia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.cardDocGia.Margin = new System.Windows.Forms.Padding(14);
            this.cardDocGia.MouseState = MaterialSkin.MouseState.HOVER;
            this.cardDocGia.Name = "cardDocGia";
            this.cardDocGia.Padding = new System.Windows.Forms.Padding(14);
            this.cardDocGia.TabIndex = 1;
            this.cardDocGia.Click += new System.EventHandler(this.CardDocGia_Click);
            // lblDocGiaValue
            this.lblDocGiaValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDocGiaValue.AutoSize = true;
            this.lblDocGiaValue.Depth = 0;
            this.lblDocGiaValue.Font = new System.Drawing.Font("Roboto", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblDocGiaValue.FontType = MaterialSkin.MaterialSkinManager.fontType.H5;
            this.lblDocGiaValue.Location = new System.Drawing.Point(17, 24);
            this.lblDocGiaValue.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblDocGiaValue.Name = "lblDocGiaValue";
            this.lblDocGiaValue.Size = new System.Drawing.Size(25, 29);
            this.lblDocGiaValue.TabIndex = 1;
            this.lblDocGiaValue.Text = "...";
            // lblDocGiaDesc
            this.lblDocGiaDesc.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDocGiaDesc.AutoSize = true;
            this.lblDocGiaDesc.Depth = 0;
            this.lblDocGiaDesc.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblDocGiaDesc.Location = new System.Drawing.Point(17, 65);
            this.lblDocGiaDesc.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblDocGiaDesc.Name = "lblDocGiaDesc";
            this.lblDocGiaDesc.Size = new System.Drawing.Size(119, 19);
            this.lblDocGiaDesc.TabIndex = 2;
            this.lblDocGiaDesc.Text = "Số lượng độc giả";
            // picIconDocGia
            this.picIconDocGia.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.picIconDocGia.Location = new System.Drawing.Point(180, 36);
            this.picIconDocGia.Name = "picIconDocGia";
            this.picIconDocGia.Size = new System.Drawing.Size(48, 48);
            this.picIconDocGia.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picIconDocGia.TabIndex = 0;
            this.picIconDocGia.TabStop = false;

            //
            // cardDangMuon (Tương tự)
            //
            this.cardDangMuon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cardDangMuon.Controls.Add(this.lblDangMuonValue);
            this.cardDangMuon.Controls.Add(this.lblDangMuonDesc);
            this.cardDangMuon.Controls.Add(this.picIconDangMuon);
            this.cardDangMuon.Depth = 1;
            this.cardDangMuon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardDangMuon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.cardDangMuon.Margin = new System.Windows.Forms.Padding(14);
            this.cardDangMuon.MouseState = MaterialSkin.MouseState.HOVER;
            this.cardDangMuon.Name = "cardDangMuon";
            this.cardDangMuon.Padding = new System.Windows.Forms.Padding(14);
            this.cardDangMuon.TabIndex = 2;
            this.cardDangMuon.Click += new System.EventHandler(this.CardDangMuon_Click);
            // lblDangMuonValue
            this.lblDangMuonValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDangMuonValue.AutoSize = true;
            this.lblDangMuonValue.Depth = 0;
            this.lblDangMuonValue.Font = new System.Drawing.Font("Roboto", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblDangMuonValue.FontType = MaterialSkin.MaterialSkinManager.fontType.H5;
            this.lblDangMuonValue.Location = new System.Drawing.Point(17, 24);
            this.lblDangMuonValue.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblDangMuonValue.Name = "lblDangMuonValue";
            this.lblDangMuonValue.Size = new System.Drawing.Size(25, 29);
            this.lblDangMuonValue.TabIndex = 1;
            this.lblDangMuonValue.Text = "...";
            // lblDangMuonDesc
            this.lblDangMuonDesc.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDangMuonDesc.AutoSize = true;
            this.lblDangMuonDesc.Depth = 0;
            this.lblDangMuonDesc.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblDangMuonDesc.Location = new System.Drawing.Point(17, 65);
            this.lblDangMuonDesc.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblDangMuonDesc.Name = "lblDangMuonDesc";
            this.lblDangMuonDesc.Size = new System.Drawing.Size(118, 19);
            this.lblDangMuonDesc.TabIndex = 2;
            this.lblDangMuonDesc.Text = "Sách đang mượn";
            // picIconDangMuon
            this.picIconDangMuon.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.picIconDangMuon.Location = new System.Drawing.Point(180, 36);
            this.picIconDangMuon.Name = "picIconDangMuon";
            this.picIconDangMuon.Size = new System.Drawing.Size(48, 48);
            this.picIconDangMuon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picIconDangMuon.TabIndex = 0;
            this.picIconDangMuon.TabStop = false;

            //
            // cardQuaHan (Tương tự)
            //
            this.cardQuaHan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cardQuaHan.Controls.Add(this.lblQuaHanValue);
            this.cardQuaHan.Controls.Add(this.lblQuaHanDesc);
            this.cardQuaHan.Controls.Add(this.picIconQuaHan);
            this.cardQuaHan.Depth = 1;
            this.cardQuaHan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardQuaHan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.cardQuaHan.Margin = new System.Windows.Forms.Padding(14);
            this.cardQuaHan.MouseState = MaterialSkin.MouseState.HOVER;
            this.cardQuaHan.Name = "cardQuaHan";
            this.cardQuaHan.Padding = new System.Windows.Forms.Padding(14);
            this.cardQuaHan.TabIndex = 3;
            this.cardQuaHan.Click += new System.EventHandler(this.CardQuaHan_Click);
            // lblQuaHanValue
            this.lblQuaHanValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblQuaHanValue.AutoSize = true;
            this.lblQuaHanValue.Depth = 0;
            this.lblQuaHanValue.Font = new System.Drawing.Font("Roboto", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblQuaHanValue.FontType = MaterialSkin.MaterialSkinManager.fontType.H5;
            this.lblQuaHanValue.Location = new System.Drawing.Point(17, 24);
            this.lblQuaHanValue.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblQuaHanValue.Name = "lblQuaHanValue";
            this.lblQuaHanValue.Size = new System.Drawing.Size(25, 29);
            this.lblQuaHanValue.TabIndex = 1;
            this.lblQuaHanValue.Text = "...";
            // lblQuaHanDesc
            this.lblQuaHanDesc.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblQuaHanDesc.AutoSize = true;
            this.lblQuaHanDesc.Depth = 0;
            this.lblQuaHanDesc.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblQuaHanDesc.Location = new System.Drawing.Point(17, 65);
            this.lblQuaHanDesc.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblQuaHanDesc.Name = "lblQuaHanDesc";
            this.lblQuaHanDesc.Size = new System.Drawing.Size(99, 19);
            this.lblQuaHanDesc.TabIndex = 2;
            this.lblQuaHanDesc.Text = "Sách quá hạn";
            // picIconQuaHan
            this.picIconQuaHan.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.picIconQuaHan.Location = new System.Drawing.Point(180, 36);
            this.picIconQuaHan.Name = "picIconQuaHan";
            this.picIconQuaHan.Size = new System.Drawing.Size(48, 48);
            this.picIconQuaHan.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picIconQuaHan.TabIndex = 0;
            this.picIconQuaHan.TabStop = false;

            //
            // cardTopTuaSach
            //
            this.cardTopTuaSach.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cardTopTuaSach.Controls.Add(this.lblTopTuaSach3);
            this.cardTopTuaSach.Controls.Add(this.lblTopTuaSach2);
            this.cardTopTuaSach.Controls.Add(this.lblTopTuaSach1);
            this.cardTopTuaSach.Controls.Add(this.lblTopTuaSachTitle);
            this.cardTopTuaSach.Depth = 1;
            this.cardTopTuaSach.Dock = System.Windows.Forms.DockStyle.Fill; // Lấp đầy ô (2 cột)
            this.cardTopTuaSach.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.cardTopTuaSach.Margin = new System.Windows.Forms.Padding(14);
            this.cardTopTuaSach.MouseState = MaterialSkin.MouseState.HOVER;
            this.cardTopTuaSach.Name = "cardTopTuaSach";
            this.cardTopTuaSach.Padding = new System.Windows.Forms.Padding(14);
            this.cardTopTuaSach.TabIndex = 4;
            this.cardTopTuaSach.Click += new System.EventHandler(this.CardTopTuaSach_Click);
            // lblTopTuaSachTitle
            this.lblTopTuaSachTitle.AutoSize = true;
            this.lblTopTuaSachTitle.Depth = 0;
            this.lblTopTuaSachTitle.Font = new System.Drawing.Font("Roboto Medium", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblTopTuaSachTitle.FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle2;
            this.lblTopTuaSachTitle.HighEmphasis = true;
            this.lblTopTuaSachTitle.Location = new System.Drawing.Point(17, 14);
            this.lblTopTuaSachTitle.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblTopTuaSachTitle.Name = "lblTopTuaSachTitle";
            this.lblTopTuaSachTitle.Size = new System.Drawing.Size(181, 17);
            this.lblTopTuaSachTitle.TabIndex = 0;
            this.lblTopTuaSachTitle.Text = "Top Tựa sách được mượn";
            // lblTopTuaSach1
            this.lblTopTuaSach1.AutoSize = true;
            this.lblTopTuaSach1.Depth = 0;
            this.lblTopTuaSach1.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblTopTuaSach1.Location = new System.Drawing.Point(20, 45); // Vị trí tương đối
            this.lblTopTuaSach1.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblTopTuaSach1.Name = "lblTopTuaSach1";
            this.lblTopTuaSach1.Size = new System.Drawing.Size(12, 19);
            this.lblTopTuaSach1.TabIndex = 1;
            this.lblTopTuaSach1.Text = "-";
            // lblTopTuaSach2
            this.lblTopTuaSach2.AutoSize = true;
            this.lblTopTuaSach2.Depth = 0;
            this.lblTopTuaSach2.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblTopTuaSach2.Location = new System.Drawing.Point(20, 75); // Vị trí tương đối
            this.lblTopTuaSach2.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblTopTuaSach2.Name = "lblTopTuaSach2";
            this.lblTopTuaSach2.Size = new System.Drawing.Size(12, 19);
            this.lblTopTuaSach2.TabIndex = 2;
            this.lblTopTuaSach2.Text = "-";
            // lblTopTuaSach3
            this.lblTopTuaSach3.AutoSize = true;
            this.lblTopTuaSach3.Depth = 0;
            this.lblTopTuaSach3.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblTopTuaSach3.Location = new System.Drawing.Point(20, 105); // Vị trí tương đối
            this.lblTopTuaSach3.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblTopTuaSach3.Name = "lblTopTuaSach3";
            this.lblTopTuaSach3.Size = new System.Drawing.Size(12, 19);
            this.lblTopTuaSach3.TabIndex = 3;
            this.lblTopTuaSach3.Text = "-";


            //
            // cardTopTheLoai (Tương tự cardTopTuaSach)
            //
            this.cardTopTheLoai.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cardTopTheLoai.Controls.Add(this.lblTopTheLoai3);
            this.cardTopTheLoai.Controls.Add(this.lblTopTheLoai2);
            this.cardTopTheLoai.Controls.Add(this.lblTopTheLoai1);
            this.cardTopTheLoai.Controls.Add(this.lblTopTheLoaiTitle);
            this.cardTopTheLoai.Depth = 1;
            this.cardTopTheLoai.Dock = System.Windows.Forms.DockStyle.Fill; // Lấp đầy ô (2 cột)
            this.cardTopTheLoai.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.cardTopTheLoai.Margin = new System.Windows.Forms.Padding(14);
            this.cardTopTheLoai.MouseState = MaterialSkin.MouseState.HOVER;
            this.cardTopTheLoai.Name = "cardTopTheLoai";
            this.cardTopTheLoai.Padding = new System.Windows.Forms.Padding(14);
            this.cardTopTheLoai.TabIndex = 5;
            this.cardTopTheLoai.Click += new System.EventHandler(this.CardTopTheLoai_Click);
            // lblTopTheLoaiTitle
            this.lblTopTheLoaiTitle.AutoSize = true;
            this.lblTopTheLoaiTitle.Depth = 0;
            this.lblTopTheLoaiTitle.Font = new System.Drawing.Font("Roboto Medium", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblTopTheLoaiTitle.FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle2;
            this.lblTopTheLoaiTitle.HighEmphasis = true;
            this.lblTopTheLoaiTitle.Location = new System.Drawing.Point(17, 14);
            this.lblTopTheLoaiTitle.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblTopTheLoaiTitle.Name = "lblTopTheLoaiTitle";
            this.lblTopTheLoaiTitle.Size = new System.Drawing.Size(178, 17);
            this.lblTopTheLoaiTitle.TabIndex = 0;
            this.lblTopTheLoaiTitle.Text = "Top Thể loại được mượn";
            // lblTopTheLoai1
            this.lblTopTheLoai1.AutoSize = true;
            this.lblTopTheLoai1.Depth = 0;
            this.lblTopTheLoai1.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblTopTheLoai1.Location = new System.Drawing.Point(20, 45);
            this.lblTopTheLoai1.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblTopTheLoai1.Name = "lblTopTheLoai1";
            this.lblTopTheLoai1.Size = new System.Drawing.Size(12, 19);
            this.lblTopTheLoai1.TabIndex = 1;
            this.lblTopTheLoai1.Text = "-";
            // lblTopTheLoai2
            this.lblTopTheLoai2.AutoSize = true;
            this.lblTopTheLoai2.Depth = 0;
            this.lblTopTheLoai2.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblTopTheLoai2.Location = new System.Drawing.Point(20, 75);
            this.lblTopTheLoai2.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblTopTheLoai2.Name = "lblTopTheLoai2";
            this.lblTopTheLoai2.Size = new System.Drawing.Size(12, 19);
            this.lblTopTheLoai2.TabIndex = 2;
            this.lblTopTheLoai2.Text = "-";
            // lblTopTheLoai3
            this.lblTopTheLoai3.AutoSize = true;
            this.lblTopTheLoai3.Depth = 0;
            this.lblTopTheLoai3.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblTopTheLoai3.Location = new System.Drawing.Point(20, 105);
            this.lblTopTheLoai3.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblTopTheLoai3.Name = "lblTopTheLoai3";
            this.lblTopTheLoai3.Size = new System.Drawing.Size(12, 19);
            this.lblTopTheLoai3.TabIndex = 3;
            this.lblTopTheLoai3.Text = "-";


            //
            // chartTheLoai
            //
            chartArea1.Name = "ChartAreaTheLoai";
            this.chartTheLoai.ChartAreas.Add(chartArea1);
            this.chartTheLoai.Dock = System.Windows.Forms.DockStyle.Fill; // Lấp đầy ô (2 cột)
            legend1.Name = "LegendTheLoai";
            this.chartTheLoai.Legends.Add(legend1);
            // Location được quản lý bởi TLP
            this.chartTheLoai.Margin = new System.Windows.Forms.Padding(14);
            this.chartTheLoai.Name = "chartTheLoai";
            // Size được quản lý bởi TLP
            this.chartTheLoai.TabIndex = 6;
            this.chartTheLoai.Text = "Biểu đồ Thể loại";
            title1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            title1.Name = "TitleTheLoai";
            title1.Text = "Top 5 Thể loại được mượn";
            this.chartTheLoai.Titles.Add(title1);

            //
            // chartLuotMuonThang
            //
            chartArea2.AxisX.IsLabelAutoFit = false;
            chartArea2.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            chartArea2.AxisX.MajorGrid.Enabled = false;
            chartArea2.AxisY.IsLabelAutoFit = false;
            chartArea2.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            chartArea2.AxisY.LabelStyle.Format = "#,##0";
            chartArea2.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gainsboro;
            chartArea2.Name = "ChartAreaLuotMuon";
            this.chartLuotMuonThang.ChartAreas.Add(chartArea2);
            this.chartLuotMuonThang.Dock = System.Windows.Forms.DockStyle.Fill; // Lấp đầy ô (2 cột)
            // Location được quản lý bởi TLP
            this.chartLuotMuonThang.Margin = new System.Windows.Forms.Padding(14);
            this.chartLuotMuonThang.Name = "chartLuotMuonThang";
            // Size được quản lý bởi TLP
            this.chartLuotMuonThang.TabIndex = 7;
            this.chartLuotMuonThang.Text = "Biểu đồ Lượt mượn";
            title2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            title2.Name = "TitleLuotMuon";
            title2.Text = "Số lượt mượn theo Tháng";
            this.chartLuotMuonThang.Titles.Add(title2);


            //
            // cardRecentActivity
            //
            this.cardRecentActivity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cardRecentActivity.Controls.Add(this.lvRecentActivity); // Thêm ListView
            this.cardRecentActivity.Controls.Add(this.lblRecentActivityTitle); // Thêm Title
            this.cardRecentActivity.Depth = 1;
            this.cardRecentActivity.Dock = System.Windows.Forms.DockStyle.Fill; // Lấp đầy ô (4 cột)
            this.cardRecentActivity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.cardRecentActivity.Margin = new System.Windows.Forms.Padding(14);
            this.cardRecentActivity.MouseState = MaterialSkin.MouseState.HOVER;
            this.cardRecentActivity.Name = "cardRecentActivity";
            this.cardRecentActivity.Padding = new System.Windows.Forms.Padding(14);
            this.cardRecentActivity.TabIndex = 8;
            //
            // lblRecentActivityTitle (Bên trong cardRecentActivity)
            //
            this.lblRecentActivityTitle.AutoSize = true; // Để AutoSize để tính toán Padding đúng
            this.lblRecentActivityTitle.Depth = 0;
            this.lblRecentActivityTitle.Dock = System.Windows.Forms.DockStyle.Top; // Dock lên trên cùng của card
            this.lblRecentActivityTitle.Font = new System.Drawing.Font("Roboto Medium", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblRecentActivityTitle.FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle2;
            this.lblRecentActivityTitle.HighEmphasis = true;
            this.lblRecentActivityTitle.Location = new System.Drawing.Point(14, 14); // Vị trí gốc padding
            this.lblRecentActivityTitle.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5); // Padding dưới title
            this.lblRecentActivityTitle.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblRecentActivityTitle.Name = "lblRecentActivityTitle";
            this.lblRecentActivityTitle.Size = new System.Drawing.Size(135, 17); // Size sẽ tự điều chỉnh do AutoSize
            this.lblRecentActivityTitle.TabIndex = 0;
            this.lblRecentActivityTitle.Text = "Hoạt động gần đây";
            //
            // lvRecentActivity (Bên trong cardRecentActivity)
            //
            this.lvRecentActivity.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTime,
            this.colActivity});
            this.lvRecentActivity.Dock = System.Windows.Forms.DockStyle.Fill; // Lấp đầy phần còn lại của card
            this.lvRecentActivity.FullRowSelect = true;
            this.lvRecentActivity.GridLines = true;
            this.lvRecentActivity.HideSelection = false;
            // Location và Size được quản lý bởi Dock=Fill, vị trí bắt đầu sau Title + Padding
            this.lvRecentActivity.Location = new System.Drawing.Point(14, 36); // (14 padding + 17 title height + 5 title padding = 36)
            this.lvRecentActivity.MultiSelect = false;
            this.lvRecentActivity.Name = "lvRecentActivity";
            this.lvRecentActivity.TabIndex = 1;
            this.lvRecentActivity.UseCompatibleStateImageBehavior = false;
            this.lvRecentActivity.View = System.Windows.Forms.View.Details;
            // Cần tính toán lại Size khi Dock Fill: Size(cardWidth - padding*2, cardHeight - titleTop - paddingBottom)
            // Ví dụ: Size(1092 - 28, 200 - 36 - 14) = Size(1064, 150) -> Sẽ tự động bởi Dock=Fill

            //
            // colTime
            //
            this.colTime.Text = "Thời gian";
            this.colTime.Width = 120;
            //
            // colActivity
            //
            this.colActivity.Text = "Nội dung";
            this.colActivity.Width = 850; // Điều chỉnh nếu cần


            //
            // ucTrangChu (Cấu hình UserControl)
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(242, 242, 242); // Màu nền
                                                                           // QUAN TRỌNG: Thêm TLP mới, giữ Title, không thêm FlowLayoutPanel cũ
            this.Controls.Add(this.mainTableLayoutPanel); // Thêm TLP vào UserControl
            this.Controls.Add(this.lblTitle);             // Thêm Title vào UserControl (đã Dock Top)
            this.Name = "ucTrangChu";
            this.Padding = new System.Windows.Forms.Padding(10); // Padding cho UserControl
            this.Size = new System.Drawing.Size(1200, 800); // Kích thước UserControl

            // --- ResumeLayout & EndInit ---
            ((System.ComponentModel.ISupportInitialize)(this.chartTheLoai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartLuotMuonThang)).EndInit();
            this.cardSoSach.ResumeLayout(false);
            this.cardSoSach.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIconSach)).EndInit();
            this.cardDocGia.ResumeLayout(false);
            this.cardDocGia.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIconDocGia)).EndInit();
            this.cardDangMuon.ResumeLayout(false);
            this.cardDangMuon.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIconDangMuon)).EndInit();
            this.cardQuaHan.ResumeLayout(false);
            this.cardQuaHan.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIconQuaHan)).EndInit();
            this.cardTopTuaSach.ResumeLayout(false);
            this.cardTopTuaSach.PerformLayout();
            this.cardTopTheLoai.ResumeLayout(false);
            this.cardTopTheLoai.PerformLayout();
            this.cardRecentActivity.ResumeLayout(false);
            this.cardRecentActivity.PerformLayout();
            this.mainTableLayoutPanel.ResumeLayout(false); // Resume cho TLP mới
            this.ResumeLayout(false);
            this.PerformLayout(); // Cần thiết nếu có control AutoSize
        }

        #endregion

        // --- KHAI BÁO BIẾN CONTROL ---
        private MaterialLabel lblTitle;
        // private FlowLayoutPanel flowLayoutPanelStats; // <= ĐÃ XÓA KHỎI KHAI BÁO VÀ SỬ DỤNG
        private MaterialCard cardSoSach;
        private PictureBox picIconSach;
        private MaterialLabel lblSoSachValue;
        private MaterialLabel lblSoSachDesc;
        private MaterialCard cardDocGia;
        private PictureBox picIconDocGia;
        private MaterialLabel lblDocGiaValue;
        private MaterialLabel lblDocGiaDesc;
        private MaterialCard cardDangMuon;
        private PictureBox picIconDangMuon;
        private MaterialLabel lblDangMuonValue;
        private MaterialLabel lblDangMuonDesc;
        private MaterialCard cardQuaHan;
        private PictureBox picIconQuaHan;
        private MaterialLabel lblQuaHanValue;
        private MaterialLabel lblQuaHanDesc;
        private MaterialCard cardTopTuaSach;
        private MaterialLabel lblTopTuaSachTitle;
        private MaterialLabel lblTopTuaSach1;
        private MaterialLabel lblTopTuaSach2;
        private MaterialLabel lblTopTuaSach3;
        private MaterialCard cardTopTheLoai;
        private MaterialLabel lblTopTheLoaiTitle;
        private MaterialLabel lblTopTheLoai1;
        private MaterialLabel lblTopTheLoai2;
        private MaterialLabel lblTopTheLoai3;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartTheLoai;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartLuotMuonThang;
        private MaterialCard cardRecentActivity;
        private MaterialLabel lblRecentActivityTitle;
        private ListView lvRecentActivity;
        private ColumnHeader colTime;
        private ColumnHeader colActivity;

        // --- KHAI BÁO BIẾN CHO TableLayoutPanel MỚI ---
        private TableLayoutPanel mainTableLayoutPanel;
    }
}