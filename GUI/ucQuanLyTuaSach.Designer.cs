// Project/Namespace: GUI

using MaterialSkin.Controls;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System;

namespace GUI
{
    partial class ucQuanLyTuaSach
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            materialCard1 = new MaterialCard();
            materialCardDetails = new MaterialCard();
            tableLayoutPanelDetails = new TableLayoutPanel();
            txtId = new MaterialTextBox2();
            txtMaTuaSach = new MaterialTextBox2();
            txtTenTuaSach = new MaterialTextBox2();
            cbTheLoai = new MaterialComboBox();
            lblTacGia = new MaterialLabel();
            clbTacGia = new MaterialCheckedListBox();
            flowLayoutPanelButtonsDetails = new FlowLayoutPanel();
            btnLuu = new MaterialButton();
            btnBoQua = new MaterialButton();
            panelGridAndButtons = new Panel();
            dgvTuaSach = new DataGridView();
            flowLayoutPanelButtonsGrid = new FlowLayoutPanel();
            btnThem = new MaterialButton();
            btnSua = new MaterialButton();
            btnXoa = new MaterialButton();
            panelFilterSearch = new Panel();
            flowLayoutPanelFilter = new FlowLayoutPanel();
            txtSearch = new MaterialTextBox2();
            cbFilterTheLoai = new MaterialComboBox();
            cbFilterTacGia = new MaterialComboBox();
            btnResetFilter = new MaterialButton();
            materialCard1.SuspendLayout();
            materialCardDetails.SuspendLayout();
            tableLayoutPanelDetails.SuspendLayout();
            flowLayoutPanelButtonsDetails.SuspendLayout();
            panelGridAndButtons.SuspendLayout();
            ((ISupportInitialize)dgvTuaSach).BeginInit();
            flowLayoutPanelButtonsGrid.SuspendLayout();
            panelFilterSearch.SuspendLayout();
            flowLayoutPanelFilter.SuspendLayout();
            SuspendLayout();
            // 
            // materialCard1
            // 
            materialCard1.BackColor = Color.FromArgb(255, 255, 255);
            materialCard1.Controls.Add(materialCardDetails);
            materialCard1.Controls.Add(panelGridAndButtons);
            materialCard1.Controls.Add(panelFilterSearch);
            materialCard1.Depth = 0;
            materialCard1.Dock = DockStyle.Fill;
            materialCard1.ForeColor = Color.FromArgb(222, 0, 0, 0);
            materialCard1.Location = new Point(20, 23);
            materialCard1.Margin = new Padding(13, 15, 13, 15);
            materialCard1.MouseState = MaterialSkin.MouseState.HOVER;
            materialCard1.Name = "materialCard1";
            materialCard1.Padding = new Padding(27, 31, 27, 31);
            materialCard1.Size = new Size(1027, 877);
            materialCard1.TabIndex = 0;
            // 
            // materialCardDetails
            // 
            materialCardDetails.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            materialCardDetails.BackColor = Color.FromArgb(255, 255, 255);
            materialCardDetails.Controls.Add(tableLayoutPanelDetails);
            materialCardDetails.Depth = 0;
            materialCardDetails.ForeColor = Color.FromArgb(222, 0, 0, 0);
            materialCardDetails.Location = new Point(31, 585);
            materialCardDetails.Margin = new Padding(19, 22, 19, 22);
            materialCardDetails.MouseState = MaterialSkin.MouseState.HOVER;
            materialCardDetails.Name = "materialCardDetails";
            materialCardDetails.Padding = new Padding(20, 23, 20, 23);
            materialCardDetails.Size = new Size(965, 257);
            materialCardDetails.TabIndex = 2;
            // 
            // tableLayoutPanelDetails
            // 
            tableLayoutPanelDetails.ColumnCount = 3;
            tableLayoutPanelDetails.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tableLayoutPanelDetails.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tableLayoutPanelDetails.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanelDetails.Controls.Add(txtId, 0, 0);
            tableLayoutPanelDetails.Controls.Add(txtMaTuaSach, 1, 0);
            tableLayoutPanelDetails.Controls.Add(txtTenTuaSach, 0, 1);
            tableLayoutPanelDetails.Controls.Add(cbTheLoai, 1, 1);
            tableLayoutPanelDetails.Controls.Add(lblTacGia, 2, 0);
            tableLayoutPanelDetails.Controls.Add(clbTacGia, 2, 1);
            tableLayoutPanelDetails.Controls.Add(flowLayoutPanelButtonsDetails, 0, 2);
            tableLayoutPanelDetails.Dock = DockStyle.Fill;
            tableLayoutPanelDetails.Location = new Point(20, 23);
            tableLayoutPanelDetails.Margin = new Padding(4, 5, 4, 5);
            tableLayoutPanelDetails.Name = "tableLayoutPanelDetails";
            tableLayoutPanelDetails.RowCount = 3;
            tableLayoutPanelDetails.RowStyles.Add(new RowStyle(SizeType.Absolute, 85F));
            tableLayoutPanelDetails.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelDetails.RowStyles.Add(new RowStyle(SizeType.Absolute, 77F));
            tableLayoutPanelDetails.Size = new Size(925, 211);
            tableLayoutPanelDetails.TabIndex = 0;
            // 
            // txtId
            // 
            txtId.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtId.AnimateReadOnly = true;
            txtId.BackgroundImageLayout = ImageLayout.None;
            txtId.CharacterCasing = CharacterCasing.Normal;
            txtId.Depth = 0;
            txtId.Enabled = false;
            txtId.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtId.HideSelection = true;
            txtId.Hint = "ID";
            txtId.LeadingIcon = null;
            txtId.Location = new Point(4, 5);
            txtId.Margin = new Padding(4, 5, 4, 5);
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
            txtId.Size = new Size(315, 48);
            txtId.TabIndex = 0;
            txtId.TabStop = false;
            txtId.TextAlign = HorizontalAlignment.Left;
            txtId.TrailingIcon = null;
            txtId.UseSystemPasswordChar = false;
            // 
            // txtMaTuaSach
            // 
            txtMaTuaSach.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMaTuaSach.AnimateReadOnly = false;
            txtMaTuaSach.BackgroundImageLayout = ImageLayout.None;
            txtMaTuaSach.CharacterCasing = CharacterCasing.Normal;
            txtMaTuaSach.Depth = 0;
            txtMaTuaSach.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtMaTuaSach.HideSelection = true;
            txtMaTuaSach.Hint = "Mã tựa sách";
            txtMaTuaSach.LeadingIcon = null;
            txtMaTuaSach.Location = new Point(327, 5);
            txtMaTuaSach.Margin = new Padding(4, 5, 4, 5);
            txtMaTuaSach.MaxLength = 50;
            txtMaTuaSach.MouseState = MaterialSkin.MouseState.OUT;
            txtMaTuaSach.Name = "txtMaTuaSach";
            txtMaTuaSach.PasswordChar = '\0';
            txtMaTuaSach.PrefixSuffixText = null;
            txtMaTuaSach.ReadOnly = false;
            txtMaTuaSach.RightToLeft = RightToLeft.No;
            txtMaTuaSach.SelectedText = "";
            txtMaTuaSach.SelectionLength = 0;
            txtMaTuaSach.SelectionStart = 0;
            txtMaTuaSach.ShortcutsEnabled = true;
            txtMaTuaSach.Size = new Size(315, 48);
            txtMaTuaSach.TabIndex = 1;
            txtMaTuaSach.TabStop = false;
            txtMaTuaSach.TextAlign = HorizontalAlignment.Left;
            txtMaTuaSach.TrailingIcon = null;
            txtMaTuaSach.UseSystemPasswordChar = false;
            // 
            // txtTenTuaSach
            // 
            txtTenTuaSach.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTenTuaSach.AnimateReadOnly = false;
            txtTenTuaSach.BackgroundImageLayout = ImageLayout.None;
            txtTenTuaSach.CharacterCasing = CharacterCasing.Normal;
            txtTenTuaSach.Depth = 0;
            txtTenTuaSach.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtTenTuaSach.HideSelection = true;
            txtTenTuaSach.Hint = "Tên tựa sách";
            txtTenTuaSach.LeadingIcon = null;
            txtTenTuaSach.Location = new Point(4, 90);
            txtTenTuaSach.Margin = new Padding(4, 5, 4, 5);
            txtTenTuaSach.MaxLength = 100;
            txtTenTuaSach.MouseState = MaterialSkin.MouseState.OUT;
            txtTenTuaSach.Name = "txtTenTuaSach";
            txtTenTuaSach.PasswordChar = '\0';
            txtTenTuaSach.PrefixSuffixText = null;
            txtTenTuaSach.ReadOnly = false;
            txtTenTuaSach.RightToLeft = RightToLeft.No;
            txtTenTuaSach.SelectedText = "";
            txtTenTuaSach.SelectionLength = 0;
            txtTenTuaSach.SelectionStart = 0;
            txtTenTuaSach.ShortcutsEnabled = true;
            txtTenTuaSach.Size = new Size(315, 48);
            txtTenTuaSach.TabIndex = 2;
            txtTenTuaSach.TabStop = false;
            txtTenTuaSach.TextAlign = HorizontalAlignment.Left;
            txtTenTuaSach.TrailingIcon = null;
            txtTenTuaSach.UseSystemPasswordChar = false;
            // 
            // cbTheLoai
            // 
            cbTheLoai.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbTheLoai.AutoResize = false;
            cbTheLoai.BackColor = Color.FromArgb(255, 255, 255);
            cbTheLoai.Depth = 0;
            cbTheLoai.DrawMode = DrawMode.OwnerDrawVariable;
            cbTheLoai.DropDownHeight = 174;
            cbTheLoai.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTheLoai.DropDownWidth = 121;
            cbTheLoai.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            cbTheLoai.ForeColor = Color.FromArgb(222, 0, 0, 0);
            cbTheLoai.FormattingEnabled = true;
            cbTheLoai.Hint = "Thể loại";
            cbTheLoai.IntegralHeight = false;
            cbTheLoai.ItemHeight = 43;
            cbTheLoai.Location = new Point(327, 90);
            cbTheLoai.Margin = new Padding(4, 5, 4, 5);
            cbTheLoai.MaxDropDownItems = 4;
            cbTheLoai.MouseState = MaterialSkin.MouseState.OUT;
            cbTheLoai.Name = "cbTheLoai";
            cbTheLoai.Size = new Size(315, 49);
            cbTheLoai.StartIndex = -1;
            cbTheLoai.TabIndex = 3;
            // 
            // lblTacGia
            // 
            lblTacGia.AutoSize = true;
            lblTacGia.Depth = 0;
            lblTacGia.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblTacGia.Location = new Point(650, 8);
            lblTacGia.Margin = new Padding(4, 8, 4, 0);
            lblTacGia.MouseState = MaterialSkin.MouseState.HOVER;
            lblTacGia.Name = "lblTacGia";
            lblTacGia.Size = new Size(58, 19);
            lblTacGia.TabIndex = 4;
            lblTacGia.Text = "Tác giả:";
            // 
            // clbTacGia
            // 
            clbTacGia.AutoScroll = true;
            clbTacGia.BackColor = Color.FromArgb(255, 255, 255);
            clbTacGia.Depth = 0;
            clbTacGia.Dock = DockStyle.Fill;
            clbTacGia.Location = new Point(650, 90);
            clbTacGia.Margin = new Padding(4, 5, 4, 5);
            clbTacGia.MouseState = MaterialSkin.MouseState.HOVER;
            clbTacGia.Name = "clbTacGia";
            clbTacGia.Size = new Size(271, 39);
            clbTacGia.Striped = false;
            clbTacGia.StripeDarkColor = Color.Empty;
            clbTacGia.TabIndex = 5;
            // 
            // flowLayoutPanelButtonsDetails
            // 
            flowLayoutPanelButtonsDetails.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            tableLayoutPanelDetails.SetColumnSpan(flowLayoutPanelButtonsDetails, 3);
            flowLayoutPanelButtonsDetails.Controls.Add(btnLuu);
            flowLayoutPanelButtonsDetails.Controls.Add(btnBoQua);
            flowLayoutPanelButtonsDetails.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanelButtonsDetails.Location = new Point(4, 139);
            flowLayoutPanelButtonsDetails.Margin = new Padding(4, 5, 4, 5);
            flowLayoutPanelButtonsDetails.Name = "flowLayoutPanelButtonsDetails";
            flowLayoutPanelButtonsDetails.Size = new Size(917, 67);
            flowLayoutPanelButtonsDetails.TabIndex = 6;
            // 
            // btnLuu
            // 
            btnLuu.AutoSize = false;
            btnLuu.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnLuu.Density = MaterialButton.MaterialButtonDensity.Default;
            btnLuu.Depth = 0;
            btnLuu.HighEmphasis = true;
            btnLuu.Icon = null;
            btnLuu.Location = new Point(792, 6);
            btnLuu.Margin = new Padding(5, 6, 13, 6);
            btnLuu.MouseState = MaterialSkin.MouseState.HOVER;
            btnLuu.Name = "btnLuu";
            btnLuu.NoAccentTextColor = Color.Empty;
            btnLuu.Size = new Size(112, 55);
            btnLuu.TabIndex = 1;
            btnLuu.Text = "Lưu";
            btnLuu.Type = MaterialButton.MaterialButtonType.Contained;
            btnLuu.UseAccentColor = true;
            btnLuu.UseVisualStyleBackColor = true;
            btnLuu.Click += btnLuu_Click;
            // 
            // btnBoQua
            // 
            btnBoQua.AutoSize = false;
            btnBoQua.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnBoQua.Density = MaterialButton.MaterialButtonDensity.Default;
            btnBoQua.Depth = 0;
            btnBoQua.HighEmphasis = false;
            btnBoQua.Icon = null;
            btnBoQua.Location = new Point(654, 6);
            btnBoQua.Margin = new Padding(5, 6, 5, 6);
            btnBoQua.MouseState = MaterialSkin.MouseState.HOVER;
            btnBoQua.Name = "btnBoQua";
            btnBoQua.NoAccentTextColor = Color.Empty;
            btnBoQua.Size = new Size(128, 55);
            btnBoQua.TabIndex = 0;
            btnBoQua.Text = "Bỏ Qua";
            btnBoQua.Type = MaterialButton.MaterialButtonType.Outlined;
            btnBoQua.UseAccentColor = false;
            btnBoQua.UseVisualStyleBackColor = true;
            btnBoQua.Click += btnBoQua_Click;
            // 
            // panelGridAndButtons
            // 
            panelGridAndButtons.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelGridAndButtons.Controls.Add(dgvTuaSach);
            panelGridAndButtons.Controls.Add(flowLayoutPanelButtonsGrid);
            panelGridAndButtons.Location = new Point(31, 154);
            panelGridAndButtons.Margin = new Padding(4, 15, 4, 15);
            panelGridAndButtons.Name = "panelGridAndButtons";
            panelGridAndButtons.Size = new Size(965, 394);
            panelGridAndButtons.TabIndex = 1;
            // 
            // dgvTuaSach
            // 
            dgvTuaSach.AllowUserToAddRows = false;
            dgvTuaSach.AllowUserToDeleteRows = false;
            dgvTuaSach.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(240, 240, 240);
            dgvTuaSach.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvTuaSach.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvTuaSach.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTuaSach.BackgroundColor = SystemColors.Window;
            dgvTuaSach.BorderStyle = BorderStyle.None;
            dgvTuaSach.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvTuaSach.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(224, 224, 224);
            dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(64, 64, 64);
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvTuaSach.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvTuaSach.ColumnHeadersHeight = 40;
            dgvTuaSach.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Window;
            dataGridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(222, 0, 0, 0);
            dataGridViewCellStyle3.Padding = new Padding(5, 0, 5, 0);
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvTuaSach.DefaultCellStyle = dataGridViewCellStyle3;
            dgvTuaSach.EnableHeadersVisualStyles = false;
            dgvTuaSach.GridColor = Color.FromArgb(224, 224, 224);
            dgvTuaSach.Location = new Point(0, 0);
            dgvTuaSach.Margin = new Padding(0);
            dgvTuaSach.MultiSelect = false;
            dgvTuaSach.Name = "dgvTuaSach";
            dgvTuaSach.ReadOnly = true;
            dgvTuaSach.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Control;
            dataGridViewCellStyle4.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle4.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            dgvTuaSach.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dgvTuaSach.RowHeadersVisible = false;
            dgvTuaSach.RowHeadersWidth = 51;
            dgvTuaSach.RowTemplate.Height = 35;
            dgvTuaSach.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTuaSach.Size = new Size(965, 308);
            dgvTuaSach.TabIndex = 0;
            dgvTuaSach.SelectionChanged += dgvTuaSach_SelectionChanged;
            dgvTuaSach.DoubleClick += dgvTuaSach_DoubleClick;
            // 
            // flowLayoutPanelButtonsGrid
            // 
            flowLayoutPanelButtonsGrid.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanelButtonsGrid.Controls.Add(btnThem);
            flowLayoutPanelButtonsGrid.Controls.Add(btnSua);
            flowLayoutPanelButtonsGrid.Controls.Add(btnXoa);
            flowLayoutPanelButtonsGrid.Location = new Point(0, 315);
            flowLayoutPanelButtonsGrid.Margin = new Padding(0, 8, 0, 0);
            flowLayoutPanelButtonsGrid.Name = "flowLayoutPanelButtonsGrid";
            flowLayoutPanelButtonsGrid.Padding = new Padding(0, 8, 0, 0);
            flowLayoutPanelButtonsGrid.Size = new Size(965, 78);
            flowLayoutPanelButtonsGrid.TabIndex = 1;
            // 
            // btnThem
            // 
            btnThem.AutoSize = false;
            btnThem.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnThem.Density = MaterialButton.MaterialButtonDensity.Default;
            btnThem.Depth = 0;
            btnThem.HighEmphasis = true;
            btnThem.Icon = null;
            btnThem.Location = new Point(5, 17);
            btnThem.Margin = new Padding(5, 9, 5, 9);
            btnThem.MouseState = MaterialSkin.MouseState.HOVER;
            btnThem.Name = "btnThem";
            btnThem.NoAccentTextColor = Color.Empty;
            btnThem.Size = new Size(113, 55);
            btnThem.TabIndex = 1;
            btnThem.Text = "Thêm";
            btnThem.Type = MaterialButton.MaterialButtonType.Contained;
            btnThem.UseAccentColor = true;
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
            btnSua.Location = new Point(128, 17);
            btnSua.Margin = new Padding(5, 9, 5, 9);
            btnSua.MouseState = MaterialSkin.MouseState.HOVER;
            btnSua.Name = "btnSua";
            btnSua.NoAccentTextColor = Color.Empty;
            btnSua.Size = new Size(113, 55);
            btnSua.TabIndex = 2;
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
            btnXoa.Location = new Point(251, 17);
            btnXoa.Margin = new Padding(5, 9, 5, 9);
            btnXoa.MouseState = MaterialSkin.MouseState.HOVER;
            btnXoa.Name = "btnXoa";
            btnXoa.NoAccentTextColor = Color.Empty;
            btnXoa.Size = new Size(147, 55);
            btnXoa.TabIndex = 3;
            btnXoa.Text = "Xóa";
            btnXoa.Type = MaterialButton.MaterialButtonType.Outlined;
            btnXoa.UseAccentColor = false;
            btnXoa.UseVisualStyleBackColor = true;
            btnXoa.Click += btnXoa_Click;
            // 
            // panelFilterSearch
            // 
            panelFilterSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelFilterSearch.Controls.Add(flowLayoutPanelFilter);
            panelFilterSearch.Location = new Point(31, 35);
            panelFilterSearch.Margin = new Padding(4, 5, 4, 5);
            panelFilterSearch.Name = "panelFilterSearch";
            panelFilterSearch.Size = new Size(865, 100);
            panelFilterSearch.TabIndex = 0;
            // 
            // flowLayoutPanelFilter
            // 
            flowLayoutPanelFilter.Controls.Add(txtSearch);
            flowLayoutPanelFilter.Controls.Add(cbFilterTheLoai);
            flowLayoutPanelFilter.Controls.Add(cbFilterTacGia);
            flowLayoutPanelFilter.Controls.Add(btnResetFilter);
            flowLayoutPanelFilter.Dock = DockStyle.Fill;
            flowLayoutPanelFilter.Location = new Point(0, 0);
            flowLayoutPanelFilter.Margin = new Padding(4, 5, 4, 5);
            flowLayoutPanelFilter.Name = "flowLayoutPanelFilter";
            flowLayoutPanelFilter.Padding = new Padding(0, 8, 0, 0);
            flowLayoutPanelFilter.Size = new Size(865, 100);
            flowLayoutPanelFilter.TabIndex = 0;
            // 
            // txtSearch
            // 
            txtSearch.Anchor = AnchorStyles.Left;
            txtSearch.AnimateReadOnly = false;
            txtSearch.BackgroundImageLayout = ImageLayout.None;
            txtSearch.CharacterCasing = CharacterCasing.Normal;
            txtSearch.Depth = 0;
            txtSearch.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtSearch.HideSelection = true;
            txtSearch.Hint = "Tìm theo Mã hoặc Tên...";
            txtSearch.LeadingIcon = null;
            txtSearch.Location = new Point(4, 20);
            txtSearch.Margin = new Padding(4, 5, 4, 5);
            txtSearch.MaxLength = 32767;
            txtSearch.MouseState = MaterialSkin.MouseState.OUT;
            txtSearch.Name = "txtSearch";
            txtSearch.PasswordChar = '\0';
            txtSearch.PrefixSuffixText = null;
            txtSearch.ReadOnly = false;
            txtSearch.RightToLeft = RightToLeft.No;
            txtSearch.SelectedText = "";
            txtSearch.SelectionLength = 0;
            txtSearch.SelectionStart = 0;
            txtSearch.ShortcutsEnabled = true;
            txtSearch.Size = new Size(293, 48);
            txtSearch.TabIndex = 0;
            txtSearch.TabStop = false;
            txtSearch.TextAlign = HorizontalAlignment.Left;
            txtSearch.TrailingIcon = null;
            txtSearch.UseSystemPasswordChar = false;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // cbFilterTheLoai
            // 
            cbFilterTheLoai.Anchor = AnchorStyles.Left;
            cbFilterTheLoai.AutoResize = false;
            cbFilterTheLoai.BackColor = Color.FromArgb(255, 255, 255);
            cbFilterTheLoai.Depth = 0;
            cbFilterTheLoai.DrawMode = DrawMode.OwnerDrawVariable;
            cbFilterTheLoai.DropDownHeight = 174;
            cbFilterTheLoai.DropDownStyle = ComboBoxStyle.DropDownList;
            cbFilterTheLoai.DropDownWidth = 121;
            cbFilterTheLoai.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            cbFilterTheLoai.ForeColor = Color.FromArgb(222, 0, 0, 0);
            cbFilterTheLoai.FormattingEnabled = true;
            cbFilterTheLoai.Hint = "Lọc theo Thể loại";
            cbFilterTheLoai.IntegralHeight = false;
            cbFilterTheLoai.ItemHeight = 43;
            cbFilterTheLoai.Location = new Point(305, 20);
            cbFilterTheLoai.Margin = new Padding(4, 5, 4, 5);
            cbFilterTheLoai.MaxDropDownItems = 4;
            cbFilterTheLoai.MouseState = MaterialSkin.MouseState.OUT;
            cbFilterTheLoai.Name = "cbFilterTheLoai";
            cbFilterTheLoai.Size = new Size(225, 49);
            cbFilterTheLoai.StartIndex = 0;
            cbFilterTheLoai.TabIndex = 1;
            cbFilterTheLoai.SelectedIndexChanged += cbFilterTheLoai_SelectedIndexChanged;
            // 
            // cbFilterTacGia
            // 
            cbFilterTacGia.Anchor = AnchorStyles.Left;
            cbFilterTacGia.AutoResize = false;
            cbFilterTacGia.BackColor = Color.FromArgb(255, 255, 255);
            cbFilterTacGia.Depth = 0;
            cbFilterTacGia.DrawMode = DrawMode.OwnerDrawVariable;
            cbFilterTacGia.DropDownHeight = 174;
            cbFilterTacGia.DropDownStyle = ComboBoxStyle.DropDownList;
            cbFilterTacGia.DropDownWidth = 121;
            cbFilterTacGia.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            cbFilterTacGia.ForeColor = Color.FromArgb(222, 0, 0, 0);
            cbFilterTacGia.FormattingEnabled = true;
            cbFilterTacGia.Hint = "Lọc theo Tác giả";
            cbFilterTacGia.IntegralHeight = false;
            cbFilterTacGia.ItemHeight = 43;
            cbFilterTacGia.Location = new Point(538, 20);
            cbFilterTacGia.Margin = new Padding(4, 5, 4, 5);
            cbFilterTacGia.MaxDropDownItems = 4;
            cbFilterTacGia.MouseState = MaterialSkin.MouseState.OUT;
            cbFilterTacGia.Name = "cbFilterTacGia";
            cbFilterTacGia.Size = new Size(225, 49);
            cbFilterTacGia.StartIndex = 0;
            cbFilterTacGia.TabIndex = 2;
            cbFilterTacGia.SelectedIndexChanged += cbFilterTacGia_SelectedIndexChanged;
            // 
            // btnResetFilter
            // 
            btnResetFilter.Anchor = AnchorStyles.Left;
            btnResetFilter.AutoSize = false;
            btnResetFilter.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnResetFilter.Density = MaterialButton.MaterialButtonDensity.Default;
            btnResetFilter.Depth = 0;
            btnResetFilter.HighEmphasis = false;
            btnResetFilter.Icon = null;
            btnResetFilter.Location = new Point(772, 17);
            btnResetFilter.Margin = new Padding(5, 9, 5, 9);
            btnResetFilter.MouseState = MaterialSkin.MouseState.HOVER;
            btnResetFilter.Name = "btnResetFilter";
            btnResetFilter.NoAccentTextColor = Color.Empty;
            btnResetFilter.Size = new Size(80, 55);
            btnResetFilter.TabIndex = 3;
            btnResetFilter.Text = "Bỏ lọc";
            btnResetFilter.Type = MaterialButton.MaterialButtonType.Text;
            btnResetFilter.UseAccentColor = false;
            btnResetFilter.UseVisualStyleBackColor = true;
            btnResetFilter.Click += btnResetFilter_Click;
            // 
            // ucQuanLyTuaSach
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            Controls.Add(materialCard1);
            Margin = new Padding(0);
            Name = "ucQuanLyTuaSach";
            Padding = new Padding(20, 23, 20, 23);
            Size = new Size(1067, 923);
            Load += ucQuanLyTuaSach_Load;
            materialCard1.ResumeLayout(false);
            materialCardDetails.ResumeLayout(false);
            tableLayoutPanelDetails.ResumeLayout(false);
            tableLayoutPanelDetails.PerformLayout();
            flowLayoutPanelButtonsDetails.ResumeLayout(false);
            panelGridAndButtons.ResumeLayout(false);
            ((ISupportInitialize)dgvTuaSach).EndInit();
            flowLayoutPanelButtonsGrid.ResumeLayout(false);
            panelFilterSearch.ResumeLayout(false);
            flowLayoutPanelFilter.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private MaterialSkin.Controls.MaterialCard materialCard1;
        private System.Windows.Forms.Panel panelGridAndButtons; // Đổi tên panel chứa grid và nút
        private System.Windows.Forms.DataGridView dgvTuaSach;
        private MaterialSkin.Controls.MaterialButton btnXoa;
        private MaterialSkin.Controls.MaterialButton btnSua;
        private MaterialSkin.Controls.MaterialButton btnThem;
        private MaterialSkin.Controls.MaterialCard materialCardDetails;
        private MaterialSkin.Controls.MaterialButton btnBoQua;
        private MaterialSkin.Controls.MaterialButton btnLuu;
        private MaterialSkin.Controls.MaterialTextBox2 txtId;
        private MaterialSkin.Controls.MaterialTextBox2 txtTenTuaSach;
        private MaterialSkin.Controls.MaterialTextBox2 txtMaTuaSach;
        private MaterialSkin.Controls.MaterialComboBox cbTheLoai;
        private MaterialSkin.Controls.MaterialCheckedListBox clbTacGia;
        private System.Windows.Forms.Panel panelFilterSearch;
        private MaterialSkin.Controls.MaterialTextBox2 txtSearch;
        private MaterialSkin.Controls.MaterialComboBox cbFilterTacGia;
        private MaterialSkin.Controls.MaterialComboBox cbFilterTheLoai;
        private MaterialSkin.Controls.MaterialButton btnResetFilter;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelDetails; // Layout cho phần chi tiết
        private MaterialSkin.Controls.MaterialLabel lblTacGia; // Label cho Tác giả
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelButtonsDetails; // Layout cho nút Lưu/Bỏ qua
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelButtonsGrid; // Layout cho nút Thêm/Sửa/Xóa
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelFilter; // Layout cho bộ lọc
    }
}
