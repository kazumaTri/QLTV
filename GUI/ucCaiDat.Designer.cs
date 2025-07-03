// Project/Namespace: GUI

// --- USING DIRECTIVES ---
using System;
using System.Windows.Forms;
using MaterialSkin.Controls;
using System.Drawing;
using System.ComponentModel;


namespace GUI
{
    partial class ucCaiDat
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // --- KHAI BÁO CÁC CONTROLS UI ---
        private MaterialSkin.Controls.MaterialTextBox2 txtId;
        private MaterialSkin.Controls.MaterialTextBox2 txtTuoiToiThieu;
        private MaterialSkin.Controls.MaterialTextBox2 txtTuoiToiDa;
        private MaterialSkin.Controls.MaterialTextBox2 txtThoiHanThe;
        private MaterialSkin.Controls.MaterialTextBox2 txtKhoangCachXuatBan;
        private MaterialSkin.Controls.MaterialTextBox2 txtSoSachMuonToiDa;
        private MaterialSkin.Controls.MaterialTextBox2 txtSoNgayMuonToiDa;
        private MaterialSkin.Controls.MaterialTextBox2 txtDonGiaPhat;
        private MaterialSkin.Controls.MaterialTextBox2 txtAdQdkttienThu;
        private MaterialSkin.Controls.MaterialButton btnLuu;
        private MaterialSkin.Controls.MaterialButton btnBoQua;
        private MaterialSkin.Controls.MaterialButton btnThoat;
        private System.Windows.Forms.Panel panelDetails;
        // Thêm khai báo cho các Labels nếu bạn sử dụng chúng trong Designer
        private MaterialSkin.Controls.MaterialLabel lblId;
        private MaterialSkin.Controls.MaterialLabel lblTuoiToiThieu;
        private MaterialSkin.Controls.MaterialLabel lblTuoiToiDa;
        private MaterialSkin.Controls.MaterialLabel lblThoiHanThe;
        private MaterialSkin.Controls.MaterialLabel lblKhoangCachXB;
        private MaterialSkin.Controls.MaterialLabel lblSoSachMuonTD;
        private MaterialSkin.Controls.MaterialLabel lblSoNgayMuonTD;
        private MaterialSkin.Controls.MaterialLabel lblDonGiaPhat;
        private MaterialSkin.Controls.MaterialLabel lblAdQDKTTienThu;


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
            this.lblAdQDKTTienThu = new MaterialSkin.Controls.MaterialLabel();
            this.lblDonGiaPhat = new MaterialSkin.Controls.MaterialLabel();
            this.lblSoNgayMuonTD = new MaterialSkin.Controls.MaterialLabel();
            this.lblSoSachMuonTD = new MaterialSkin.Controls.MaterialLabel();
            this.lblKhoangCachXB = new MaterialSkin.Controls.MaterialLabel();
            this.lblThoiHanThe = new MaterialSkin.Controls.MaterialLabel();
            this.lblTuoiToiDa = new MaterialSkin.Controls.MaterialLabel();
            this.lblTuoiToiThieu = new MaterialSkin.Controls.MaterialLabel();
            this.lblId = new MaterialSkin.Controls.MaterialLabel();
            this.txtDonGiaPhat = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtSoNgayMuonToiDa = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtSoSachMuonToiDa = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtKhoangCachXuatBan = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtThoiHanThe = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtTuoiToiDa = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtTuoiToiThieu = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtId = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtAdQdkttienThu = new MaterialSkin.Controls.MaterialTextBox2();
            this.btnLuu = new MaterialSkin.Controls.MaterialButton();
            this.btnBoQua = new MaterialSkin.Controls.MaterialButton();
            this.btnThoat = new MaterialSkin.Controls.MaterialButton();
            this.panelDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelDetails
            // 
            this.panelDetails.Controls.Add(this.lblAdQDKTTienThu);
            this.panelDetails.Controls.Add(this.lblDonGiaPhat);
            this.panelDetails.Controls.Add(this.lblSoNgayMuonTD);
            this.panelDetails.Controls.Add(this.lblSoSachMuonTD);
            this.panelDetails.Controls.Add(this.lblKhoangCachXB);
            this.panelDetails.Controls.Add(this.lblThoiHanThe);
            this.panelDetails.Controls.Add(this.lblTuoiToiDa);
            this.panelDetails.Controls.Add(this.lblTuoiToiThieu);
            this.panelDetails.Controls.Add(this.lblId);
            this.panelDetails.Controls.Add(this.txtDonGiaPhat);
            this.panelDetails.Controls.Add(this.txtSoNgayMuonToiDa);
            this.panelDetails.Controls.Add(this.txtSoSachMuonToiDa);
            this.panelDetails.Controls.Add(this.txtKhoangCachXuatBan);
            this.panelDetails.Controls.Add(this.txtThoiHanThe);
            this.panelDetails.Controls.Add(this.txtTuoiToiDa);
            this.panelDetails.Controls.Add(this.txtTuoiToiThieu);
            this.panelDetails.Controls.Add(this.txtId);
            this.panelDetails.Controls.Add(this.txtAdQdkttienThu);
            this.panelDetails.Controls.Add(this.btnLuu);
            this.panelDetails.Controls.Add(this.btnBoQua);
            this.panelDetails.Controls.Add(this.btnThoat);
            this.panelDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDetails.Location = new System.Drawing.Point(10, 10); // Vị trí sau Padding của UserControl
            this.panelDetails.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Padding = new Padding(10); // *** ĐỔI PADDING ***
            this.panelDetails.Size = new System.Drawing.Size(780, 580); // Kích thước sau Padding của UserControl
            this.panelDetails.TabIndex = 0;
            // 
            // lblAdQDKTTienThu
            // 
            this.lblAdQDKTTienThu.AutoSize = true;
            this.lblAdQDKTTienThu.Depth = 0;
            this.lblAdQDKTTienThu.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblAdQDKTTienThu.Location = new System.Drawing.Point(14, 331);
            this.lblAdQDKTTienThu.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.lblAdQDKTTienThu.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblAdQDKTTienThu.Name = "lblAdQDKTTienThu";
            this.lblAdQDKTTienThu.Size = new System.Drawing.Size(243, 19);
            this.lblAdQDKTTienThu.TabIndex = 20;
            this.lblAdQDKTTienThu.Text = "Áp dụng quy định kiểm tra tiền thu:";
            // 
            // lblDonGiaPhat
            // 
            this.lblDonGiaPhat.AutoSize = true;
            this.lblDonGiaPhat.Depth = 0;
            this.lblDonGiaPhat.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblDonGiaPhat.Location = new System.Drawing.Point(404, 260);
            this.lblDonGiaPhat.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.lblDonGiaPhat.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblDonGiaPhat.Name = "lblDonGiaPhat";
            this.lblDonGiaPhat.Size = new System.Drawing.Size(149, 19);
            this.lblDonGiaPhat.TabIndex = 18;
            this.lblDonGiaPhat.Text = "Đơn giá phạt / Ngày:";
            // 
            // lblSoNgayMuonTD
            // 
            this.lblSoNgayMuonTD.AutoSize = true;
            this.lblSoNgayMuonTD.Depth = 0;
            this.lblSoNgayMuonTD.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblSoNgayMuonTD.Location = new System.Drawing.Point(14, 260);
            this.lblSoNgayMuonTD.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.lblSoNgayMuonTD.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblSoNgayMuonTD.Name = "lblSoNgayMuonTD";
            this.lblSoNgayMuonTD.Size = new System.Drawing.Size(154, 19);
            this.lblSoNgayMuonTD.TabIndex = 16;
            this.lblSoNgayMuonTD.Text = "Số ngày mượn tối đa:";
            // 
            // lblSoSachMuonTD
            // 
            this.lblSoSachMuonTD.AutoSize = true;
            this.lblSoSachMuonTD.Depth = 0;
            this.lblSoSachMuonTD.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblSoSachMuonTD.Location = new System.Drawing.Point(404, 189);
            this.lblSoSachMuonTD.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.lblSoSachMuonTD.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblSoSachMuonTD.Name = "lblSoSachMuonTD";
            this.lblSoSachMuonTD.Size = new System.Drawing.Size(148, 19);
            this.lblSoSachMuonTD.TabIndex = 14;
            this.lblSoSachMuonTD.Text = "Số sách mượn tối đa:";
            // 
            // lblKhoangCachXB
            // 
            this.lblKhoangCachXB.AutoSize = true;
            this.lblKhoangCachXB.Depth = 0;
            this.lblKhoangCachXB.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblKhoangCachXB.Location = new System.Drawing.Point(14, 189);
            this.lblKhoangCachXB.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.lblKhoangCachXB.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblKhoangCachXB.Name = "lblKhoangCachXB";
            this.lblKhoangCachXB.Size = new System.Drawing.Size(220, 19);
            this.lblKhoangCachXB.TabIndex = 12;
            this.lblKhoangCachXB.Text = "Khoảng cách năm xuất bản (>=):";
            // 
            // lblThoiHanThe
            // 
            this.lblThoiHanThe.AutoSize = true;
            this.lblThoiHanThe.Depth = 0;
            this.lblThoiHanThe.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblThoiHanThe.Location = new System.Drawing.Point(404, 118);
            this.lblThoiHanThe.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.lblThoiHanThe.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblThoiHanThe.Name = "lblThoiHanThe";
            this.lblThoiHanThe.Size = new System.Drawing.Size(134, 19);
            this.lblThoiHanThe.TabIndex = 10;
            this.lblThoiHanThe.Text = "Thời hạn thẻ (tháng):";
            // 
            // lblTuoiToiDa
            // 
            this.lblTuoiToiDa.AutoSize = true;
            this.lblTuoiToiDa.Depth = 0;
            this.lblTuoiToiDa.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblTuoiToiDa.Location = new System.Drawing.Point(210, 118);
            this.lblTuoiToiDa.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.lblTuoiToiDa.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblTuoiToiDa.Name = "lblTuoiToiDa";
            this.lblTuoiToiDa.Size = new System.Drawing.Size(81, 19);
            this.lblTuoiToiDa.TabIndex = 8;
            this.lblTuoiToiDa.Text = "Tuổi tối đa:";
            // 
            // lblTuoiToiThieu
            // 
            this.lblTuoiToiThieu.AutoSize = true;
            this.lblTuoiToiThieu.Depth = 0;
            this.lblTuoiToiThieu.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblTuoiToiThieu.Location = new System.Drawing.Point(14, 118);
            this.lblTuoiToiThieu.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.lblTuoiToiThieu.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblTuoiToiThieu.Name = "lblTuoiToiThieu";
            this.lblTuoiToiThieu.Size = new System.Drawing.Size(98, 19);
            this.lblTuoiToiThieu.TabIndex = 6;
            this.lblTuoiToiThieu.Text = "Tuổi tối thiểu:";
            // 
            // lblId
            // 
            this.lblId.AutoSize = true;
            this.lblId.Depth = 0;
            this.lblId.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblId.Location = new System.Drawing.Point(14, 13);
            this.lblId.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.lblId.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(23, 19);
            this.lblId.TabIndex = 4;
            this.lblId.Text = "ID:";
            // 
            // txtDonGiaPhat
            // 
            this.txtDonGiaPhat.AnimateReadOnly = false;
            this.txtDonGiaPhat.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtDonGiaPhat.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtDonGiaPhat.Depth = 0;
            this.txtDonGiaPhat.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtDonGiaPhat.HideSelection = true;
            this.txtDonGiaPhat.Hint = "Nhập đơn giá phạt";
            this.txtDonGiaPhat.LeadingIcon = null;
            this.txtDonGiaPhat.Location = new System.Drawing.Point(407, 279);
            this.txtDonGiaPhat.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.txtDonGiaPhat.MaxLength = 32767;
            this.txtDonGiaPhat.MouseState = MaterialSkin.MouseState.OUT;
            this.txtDonGiaPhat.Name = "txtDonGiaPhat";
            this.txtDonGiaPhat.PasswordChar = '\0';
            this.txtDonGiaPhat.PrefixSuffixText = null;
            this.txtDonGiaPhat.ReadOnly = false;
            this.txtDonGiaPhat.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtDonGiaPhat.SelectedText = "";
            this.txtDonGiaPhat.SelectionLength = 0;
            this.txtDonGiaPhat.SelectionStart = 0;
            this.txtDonGiaPhat.ShortcutsEnabled = true;
            this.txtDonGiaPhat.Size = new System.Drawing.Size(350, 48);
            this.txtDonGiaPhat.TabIndex = 19;
            this.txtDonGiaPhat.TabStop = false;
            this.txtDonGiaPhat.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtDonGiaPhat.TrailingIcon = null;
            this.txtDonGiaPhat.UseSystemPasswordChar = false;
            this.txtDonGiaPhat.UseTallSize = false;
            this.txtDonGiaPhat.TextChanged += new System.EventHandler(this.InputField_TextChanged);
            // 
            // txtSoNgayMuonToiDa
            // 
            this.txtSoNgayMuonToiDa.AnimateReadOnly = false;
            this.txtSoNgayMuonToiDa.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtSoNgayMuonToiDa.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtSoNgayMuonToiDa.Depth = 0;
            this.txtSoNgayMuonToiDa.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtSoNgayMuonToiDa.HideSelection = true;
            this.txtSoNgayMuonToiDa.Hint = "Nhập số ngày mượn tối đa";
            this.txtSoNgayMuonToiDa.LeadingIcon = null;
            this.txtSoNgayMuonToiDa.Location = new System.Drawing.Point(17, 279);
            this.txtSoNgayMuonToiDa.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.txtSoNgayMuonToiDa.MaxLength = 32767;
            this.txtSoNgayMuonToiDa.MouseState = MaterialSkin.MouseState.OUT;
            this.txtSoNgayMuonToiDa.Name = "txtSoNgayMuonToiDa";
            this.txtSoNgayMuonToiDa.PasswordChar = '\0';
            this.txtSoNgayMuonToiDa.PrefixSuffixText = null;
            this.txtSoNgayMuonToiDa.ReadOnly = false;
            this.txtSoNgayMuonToiDa.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtSoNgayMuonToiDa.SelectedText = "";
            this.txtSoNgayMuonToiDa.SelectionLength = 0;
            this.txtSoNgayMuonToiDa.SelectionStart = 0;
            this.txtSoNgayMuonToiDa.ShortcutsEnabled = true;
            this.txtSoNgayMuonToiDa.Size = new System.Drawing.Size(350, 48);
            this.txtSoNgayMuonToiDa.TabIndex = 17;
            this.txtSoNgayMuonToiDa.TabStop = false;
            this.txtSoNgayMuonToiDa.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtSoNgayMuonToiDa.TrailingIcon = null;
            this.txtSoNgayMuonToiDa.UseSystemPasswordChar = false;
            this.txtSoNgayMuonToiDa.UseTallSize = false;
            this.txtSoNgayMuonToiDa.TextChanged += new System.EventHandler(this.InputField_TextChanged);
            // 
            // txtSoSachMuonToiDa
            // 
            this.txtSoSachMuonToiDa.AnimateReadOnly = false;
            this.txtSoSachMuonToiDa.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtSoSachMuonToiDa.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtSoSachMuonToiDa.Depth = 0;
            this.txtSoSachMuonToiDa.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtSoSachMuonToiDa.HideSelection = true;
            this.txtSoSachMuonToiDa.Hint = "Nhập số sách mượn tối đa";
            this.txtSoSachMuonToiDa.LeadingIcon = null;
            this.txtSoSachMuonToiDa.Location = new System.Drawing.Point(407, 208);
            this.txtSoSachMuonToiDa.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.txtSoSachMuonToiDa.MaxLength = 32767;
            this.txtSoSachMuonToiDa.MouseState = MaterialSkin.MouseState.OUT;
            this.txtSoSachMuonToiDa.Name = "txtSoSachMuonToiDa";
            this.txtSoSachMuonToiDa.PasswordChar = '\0';
            this.txtSoSachMuonToiDa.PrefixSuffixText = null;
            this.txtSoSachMuonToiDa.ReadOnly = false;
            this.txtSoSachMuonToiDa.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtSoSachMuonToiDa.SelectedText = "";
            this.txtSoSachMuonToiDa.SelectionLength = 0;
            this.txtSoSachMuonToiDa.SelectionStart = 0;
            this.txtSoSachMuonToiDa.ShortcutsEnabled = true;
            this.txtSoSachMuonToiDa.Size = new System.Drawing.Size(350, 48);
            this.txtSoSachMuonToiDa.TabIndex = 15;
            this.txtSoSachMuonToiDa.TabStop = false;
            this.txtSoSachMuonToiDa.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtSoSachMuonToiDa.TrailingIcon = null;
            this.txtSoSachMuonToiDa.UseSystemPasswordChar = false;
            this.txtSoSachMuonToiDa.UseTallSize = false;
            this.txtSoSachMuonToiDa.TextChanged += new System.EventHandler(this.InputField_TextChanged);
            // 
            // txtKhoangCachXuatBan
            // 
            this.txtKhoangCachXuatBan.AnimateReadOnly = false;
            this.txtKhoangCachXuatBan.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtKhoangCachXuatBan.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtKhoangCachXuatBan.Depth = 0;
            this.txtKhoangCachXuatBan.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtKhoangCachXuatBan.HideSelection = true;
            this.txtKhoangCachXuatBan.Hint = "Nhập khoảng cách năm xuất bản";
            this.txtKhoangCachXuatBan.LeadingIcon = null;
            this.txtKhoangCachXuatBan.Location = new System.Drawing.Point(17, 208);
            this.txtKhoangCachXuatBan.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.txtKhoangCachXuatBan.MaxLength = 32767;
            this.txtKhoangCachXuatBan.MouseState = MaterialSkin.MouseState.OUT;
            this.txtKhoangCachXuatBan.Name = "txtKhoangCachXuatBan";
            this.txtKhoangCachXuatBan.PasswordChar = '\0';
            this.txtKhoangCachXuatBan.PrefixSuffixText = null;
            this.txtKhoangCachXuatBan.ReadOnly = false;
            this.txtKhoangCachXuatBan.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtKhoangCachXuatBan.SelectedText = "";
            this.txtKhoangCachXuatBan.SelectionLength = 0;
            this.txtKhoangCachXuatBan.SelectionStart = 0;
            this.txtKhoangCachXuatBan.ShortcutsEnabled = true;
            this.txtKhoangCachXuatBan.Size = new System.Drawing.Size(350, 48);
            this.txtKhoangCachXuatBan.TabIndex = 13;
            this.txtKhoangCachXuatBan.TabStop = false;
            this.txtKhoangCachXuatBan.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtKhoangCachXuatBan.TrailingIcon = null;
            this.txtKhoangCachXuatBan.UseSystemPasswordChar = false;
            this.txtKhoangCachXuatBan.UseTallSize = false;
            this.txtKhoangCachXuatBan.TextChanged += new System.EventHandler(this.InputField_TextChanged);
            // 
            // txtThoiHanThe
            // 
            this.txtThoiHanThe.AnimateReadOnly = false;
            this.txtThoiHanThe.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtThoiHanThe.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtThoiHanThe.Depth = 0;
            this.txtThoiHanThe.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtThoiHanThe.HideSelection = true;
            this.txtThoiHanThe.Hint = "Nhập thời hạn thẻ (tháng)";
            this.txtThoiHanThe.LeadingIcon = null;
            this.txtThoiHanThe.Location = new System.Drawing.Point(407, 137);
            this.txtThoiHanThe.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.txtThoiHanThe.MaxLength = 32767;
            this.txtThoiHanThe.MouseState = MaterialSkin.MouseState.OUT;
            this.txtThoiHanThe.Name = "txtThoiHanThe";
            this.txtThoiHanThe.PasswordChar = '\0';
            this.txtThoiHanThe.PrefixSuffixText = null;
            this.txtThoiHanThe.ReadOnly = false;
            this.txtThoiHanThe.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtThoiHanThe.SelectedText = "";
            this.txtThoiHanThe.SelectionLength = 0;
            this.txtThoiHanThe.SelectionStart = 0;
            this.txtThoiHanThe.ShortcutsEnabled = true;
            this.txtThoiHanThe.Size = new System.Drawing.Size(350, 48);
            this.txtThoiHanThe.TabIndex = 11;
            this.txtThoiHanThe.TabStop = false;
            this.txtThoiHanThe.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtThoiHanThe.TrailingIcon = null;
            this.txtThoiHanThe.UseSystemPasswordChar = false;
            this.txtThoiHanThe.UseTallSize = false;
            this.txtThoiHanThe.TextChanged += new System.EventHandler(this.InputField_TextChanged);
            // 
            // txtTuoiToiDa
            // 
            this.txtTuoiToiDa.AnimateReadOnly = false;
            this.txtTuoiToiDa.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtTuoiToiDa.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtTuoiToiDa.Depth = 0;
            this.txtTuoiToiDa.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtTuoiToiDa.HideSelection = true;
            this.txtTuoiToiDa.Hint = "Nhập tuổi tối đa";
            this.txtTuoiToiDa.LeadingIcon = null;
            this.txtTuoiToiDa.Location = new System.Drawing.Point(213, 137);
            this.txtTuoiToiDa.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.txtTuoiToiDa.MaxLength = 32767;
            this.txtTuoiToiDa.MouseState = MaterialSkin.MouseState.OUT;
            this.txtTuoiToiDa.Name = "txtTuoiToiDa";
            this.txtTuoiToiDa.PasswordChar = '\0';
            this.txtTuoiToiDa.PrefixSuffixText = null;
            this.txtTuoiToiDa.ReadOnly = false;
            this.txtTuoiToiDa.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtTuoiToiDa.SelectedText = "";
            this.txtTuoiToiDa.SelectionLength = 0;
            this.txtTuoiToiDa.SelectionStart = 0;
            this.txtTuoiToiDa.ShortcutsEnabled = true;
            this.txtTuoiToiDa.Size = new System.Drawing.Size(150, 48);
            this.txtTuoiToiDa.TabIndex = 9;
            this.txtTuoiToiDa.TabStop = false;
            this.txtTuoiToiDa.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtTuoiToiDa.TrailingIcon = null;
            this.txtTuoiToiDa.UseSystemPasswordChar = false;
            this.txtTuoiToiDa.UseTallSize = false;
            this.txtTuoiToiDa.TextChanged += new System.EventHandler(this.InputField_TextChanged);
            // 
            // txtTuoiToiThieu
            // 
            this.txtTuoiToiThieu.AnimateReadOnly = false;
            this.txtTuoiToiThieu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtTuoiToiThieu.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtTuoiToiThieu.Depth = 0;
            this.txtTuoiToiThieu.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtTuoiToiThieu.HideSelection = true;
            this.txtTuoiToiThieu.Hint = "Nhập tuổi tối thiểu";
            this.txtTuoiToiThieu.LeadingIcon = null;
            this.txtTuoiToiThieu.Location = new System.Drawing.Point(17, 137);
            this.txtTuoiToiThieu.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.txtTuoiToiThieu.MaxLength = 32767;
            this.txtTuoiToiThieu.MouseState = MaterialSkin.MouseState.OUT;
            this.txtTuoiToiThieu.Name = "txtTuoiToiThieu";
            this.txtTuoiToiThieu.PasswordChar = '\0';
            this.txtTuoiToiThieu.PrefixSuffixText = null;
            this.txtTuoiToiThieu.ReadOnly = false;
            this.txtTuoiToiThieu.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtTuoiToiThieu.SelectedText = "";
            this.txtTuoiToiThieu.SelectionLength = 0;
            this.txtTuoiToiThieu.SelectionStart = 0;
            this.txtTuoiToiThieu.ShortcutsEnabled = true;
            this.txtTuoiToiThieu.Size = new System.Drawing.Size(150, 48);
            this.txtTuoiToiThieu.TabIndex = 7;
            this.txtTuoiToiThieu.TabStop = false;
            this.txtTuoiToiThieu.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtTuoiToiThieu.TrailingIcon = null;
            this.txtTuoiToiThieu.UseSystemPasswordChar = false;
            this.txtTuoiToiThieu.UseTallSize = false;
            this.txtTuoiToiThieu.TextChanged += new System.EventHandler(this.InputField_TextChanged);
            // 
            // txtId
            // 
            this.txtId.AnimateReadOnly = true;
            this.txtId.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtId.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtId.Depth = 0;
            this.txtId.Enabled = false; // ID luôn disable
            this.txtId.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtId.HideSelection = true;
            this.txtId.Hint = "ID";
            this.txtId.LeadingIcon = null;
            this.txtId.Location = new System.Drawing.Point(17, 32);
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
            this.txtId.TabIndex = 5;
            this.txtId.TabStop = false;
            this.txtId.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtId.TrailingIcon = null;
            this.txtId.UseSystemPasswordChar = false;
            this.txtId.UseTallSize = false;
            // 
            // txtAdQdkttienThu
            // 
            this.txtAdQdkttienThu.AnimateReadOnly = false;
            this.txtAdQdkttienThu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtAdQdkttienThu.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtAdQdkttienThu.Depth = 0;
            this.txtAdQdkttienThu.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtAdQdkttienThu.HideSelection = true;
            this.txtAdQdkttienThu.Hint = "Nhập 0 (không) hoặc 1 (có)";
            this.txtAdQdkttienThu.LeadingIcon = null;
            this.txtAdQdkttienThu.Location = new System.Drawing.Point(17, 350);
            this.txtAdQdkttienThu.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.txtAdQdkttienThu.MaxLength = 1; // Chỉ cho nhập 1 ký tự
            this.txtAdQdkttienThu.MouseState = MaterialSkin.MouseState.OUT;
            this.txtAdQdkttienThu.Name = "txtAdQdkttienThu";
            this.txtAdQdkttienThu.PasswordChar = '\0';
            this.txtAdQdkttienThu.PrefixSuffixText = null;
            this.txtAdQdkttienThu.ReadOnly = false;
            this.txtAdQdkttienThu.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtAdQdkttienThu.SelectedText = "";
            this.txtAdQdkttienThu.SelectionLength = 0;
            this.txtAdQdkttienThu.SelectionStart = 0;
            this.txtAdQdkttienThu.ShortcutsEnabled = true;
            this.txtAdQdkttienThu.Size = new System.Drawing.Size(350, 48);
            this.txtAdQdkttienThu.TabIndex = 21;
            this.txtAdQdkttienThu.TabStop = false;
            this.txtAdQdkttienThu.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtAdQdkttienThu.TrailingIcon = null;
            this.txtAdQdkttienThu.UseSystemPasswordChar = false;
            this.txtAdQdkttienThu.UseTallSize = false;
            this.txtAdQdkttienThu.TextChanged += new System.EventHandler(this.InputField_TextChanged);
            // 
            // btnLuu
            // 
            this.btnLuu.AutoSize = false;
            this.btnLuu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLuu.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnLuu.Depth = 0;
            this.btnLuu.HighEmphasis = true;
            this.btnLuu.Icon = null;
            this.btnLuu.Location = new System.Drawing.Point(550, 356); // Điều chỉnh vị trí Y
            this.btnLuu.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.btnLuu.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnLuu.Size = new System.Drawing.Size(100, 36);
            this.btnLuu.TabIndex = 22;
            this.btnLuu.Text = "LƯU";
            this.btnLuu.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnLuu.UseAccentColor = true; // Nút chính dùng màu nhấn
            this.btnLuu.UseVisualStyleBackColor = true;
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            // 
            // btnBoQua
            // 
            this.btnBoQua.AutoSize = false;
            this.btnBoQua.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnBoQua.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnBoQua.Depth = 0;
            this.btnBoQua.HighEmphasis = true;
            this.btnBoQua.Icon = null;
            this.btnBoQua.Location = new System.Drawing.Point(660, 356); // Điều chỉnh vị trí Y
            this.btnBoQua.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.btnBoQua.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnBoQua.Name = "btnBoQua";
            this.btnBoQua.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnBoQua.Size = new System.Drawing.Size(100, 36);
            this.btnBoQua.TabIndex = 23;
            this.btnBoQua.Text = "BỎ QUA";
            this.btnBoQua.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnBoQua.UseAccentColor = false;
            this.btnBoQua.UseVisualStyleBackColor = true;
            this.btnBoQua.Click += new System.EventHandler(this.btnBoQua_Click);
            // 
            // btnThoat
            // 
            this.btnThoat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right))); // Neo góc trên phải
            this.btnThoat.AutoSize = false;
            this.btnThoat.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThoat.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnThoat.Depth = 0;
            this.btnThoat.HighEmphasis = true;
            this.btnThoat.Icon = null;
            this.btnThoat.Location = new System.Drawing.Point(677, 10); // Vị trí mẫu trong panel
            this.btnThoat.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.btnThoat.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnThoat.Size = new System.Drawing.Size(80, 36);
            this.btnThoat.TabIndex = 24;
            this.btnThoat.Text = "THOÁT";
            this.btnThoat.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text;
            this.btnThoat.UseAccentColor = false;
            this.btnThoat.UseVisualStyleBackColor = true;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // ucCaiDat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelDetails);
            this.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.Name = "ucCaiDat";
            this.Padding = new Padding(10); // *** ĐỔI PADDING ***
            this.Size = new System.Drawing.Size(800, 600);
            this.Load += new System.EventHandler(this.ucCaiDat_Load);
            this.panelDetails.ResumeLayout(false);
            this.panelDetails.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
    }
}