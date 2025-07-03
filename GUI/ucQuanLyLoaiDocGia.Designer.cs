// --- SỬA CÁC USING ---
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin; // Thêm MaterialSkin
using MaterialSkin.Controls; // Thêm MaterialSkin Controls

namespace GUI
{
    // --- KẾ THỪA TỪ UserControl CHUẨN ---
    partial class ucQuanLyLoaiDocGia // <--- Vẫn kế thừa từ UserControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelDetails = new MaterialSkin.Controls.MaterialCard();
            this.tableLayoutPanelDetails = new System.Windows.Forms.TableLayoutPanel();
            this.txtId = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtMaLoaiDocGia = new MaterialSkin.Controls.MaterialTextBox2();
            this.txtTenLoaiDocGia = new MaterialSkin.Controls.MaterialTextBox2();
            this.panelActions = new System.Windows.Forms.Panel();
            this.flpDetailActions = new System.Windows.Forms.FlowLayoutPanel(); // Sử dụng FlowLayoutPanel
            this.btnLuu = new MaterialSkin.Controls.MaterialButton();
            this.btnBoQua = new MaterialSkin.Controls.MaterialButton();
            this.panelGrid = new System.Windows.Forms.Panel();
            this.dgvLoaiDocGia = new System.Windows.Forms.DataGridView();
            this.panelGridActions = new System.Windows.Forms.Panel();
            this.txtTimKiem = new MaterialSkin.Controls.MaterialTextBox2();
            this.btnThem = new MaterialSkin.Controls.MaterialButton();
            this.flpGridActionsRight = new System.Windows.Forms.FlowLayoutPanel(); // Sử dụng FlowLayoutPanel
            this.btnExportCsv = new MaterialSkin.Controls.MaterialButton();
            this.btnSua = new MaterialSkin.Controls.MaterialButton();
            this.btnXoa = new MaterialSkin.Controls.MaterialButton();
            this.panelDetails.SuspendLayout();
            this.tableLayoutPanelDetails.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.flpDetailActions.SuspendLayout(); // Thêm vào SuspendLayout
            this.panelGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoaiDocGia)).BeginInit();
            this.panelGridActions.SuspendLayout();
            this.flpGridActionsRight.SuspendLayout(); // Thêm vào SuspendLayout
            this.SuspendLayout();
            //
            // panelDetails
            //
            this.panelDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.panelDetails.Controls.Add(this.tableLayoutPanelDetails);
            this.panelDetails.Controls.Add(this.panelActions);
            this.panelDetails.Depth = 0;
            this.panelDetails.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelDetails.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.panelDetails.Location = new System.Drawing.Point(784, 10); // Giữ nguyên hoặc điều chỉnh nếu cần
            this.panelDetails.Margin = new System.Windows.Forms.Padding(3, 10, 3, 2); // Tăng top margin
            this.panelDetails.MouseState = MaterialSkin.MouseState.HOVER;
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Padding = new System.Windows.Forms.Padding(14); // Padding đều 4 cạnh
            this.panelDetails.Size = new System.Drawing.Size(406, 792); // Giữ nguyên hoặc điều chỉnh
            this.panelDetails.TabIndex = 1;
            //
            // tableLayoutPanelDetails
            //
            this.tableLayoutPanelDetails.ColumnCount = 1;
            this.tableLayoutPanelDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelDetails.Controls.Add(this.txtId, 0, 0);
            this.tableLayoutPanelDetails.Controls.Add(this.txtMaLoaiDocGia, 0, 1);
            this.tableLayoutPanelDetails.Controls.Add(this.txtTenLoaiDocGia, 0, 2);
            this.tableLayoutPanelDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelDetails.Location = new System.Drawing.Point(14, 14); // Điều chỉnh theo Padding mới
            this.tableLayoutPanelDetails.Margin = new System.Windows.Forms.Padding(0); // Xóa Margin
            this.tableLayoutPanelDetails.Name = "tableLayoutPanelDetails";
            this.tableLayoutPanelDetails.RowCount = 4;
            this.tableLayoutPanelDetails.RowStyles.Add(new System.Windows.Forms.RowStyle()); // Dùng AutoSize cho các control tự quyết định chiều cao
            this.tableLayoutPanelDetails.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDetails.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F)); // Dòng trống chiếm phần còn lại
            this.tableLayoutPanelDetails.Size = new System.Drawing.Size(378, 706); // (792 - 14 - 14 - 58)
            this.tableLayoutPanelDetails.TabIndex = 0;
            //
            // txtId
            //
            this.txtId.AnimateReadOnly = true;
            this.txtId.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtId.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtId.Depth = 0;
            this.txtId.Dock = System.Windows.Forms.DockStyle.Top; // Dock Top thay vì Fill
            this.txtId.Enabled = false;
            this.txtId.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtId.HideSelection = true;
            this.txtId.Hint = "ID (Tự động)";
            this.txtId.LeadingIcon = null;
            this.txtId.Location = new System.Drawing.Point(3, 8); // Thêm margin top
            this.txtId.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8); // Tăng margin bottom
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
            this.txtId.Size = new System.Drawing.Size(372, 48);
            this.txtId.TabIndex = 0;
            this.txtId.TabStop = false;
            this.txtId.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtId.TrailingIcon = null;
            this.txtId.UseSystemPasswordChar = false;
            this.txtId.UseTallSize = false;
            //
            // txtMaLoaiDocGia
            //
            this.txtMaLoaiDocGia.AnimateReadOnly = false;
            this.txtMaLoaiDocGia.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtMaLoaiDocGia.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtMaLoaiDocGia.Depth = 0;
            this.txtMaLoaiDocGia.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtMaLoaiDocGia.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtMaLoaiDocGia.HideSelection = true;
            this.txtMaLoaiDocGia.Hint = "Mã Loại Độc Giả";
            this.txtMaLoaiDocGia.LeadingIcon = null;
            this.txtMaLoaiDocGia.Location = new System.Drawing.Point(3, 67); // Điều chỉnh vị trí Y
            this.txtMaLoaiDocGia.Margin = new System.Windows.Forms.Padding(3, 3, 3, 8); // Tăng margin bottom
            this.txtMaLoaiDocGia.MaxLength = 10;
            this.txtMaLoaiDocGia.MouseState = MaterialSkin.MouseState.OUT;
            this.txtMaLoaiDocGia.Name = "txtMaLoaiDocGia";
            this.txtMaLoaiDocGia.PasswordChar = '\0';
            this.txtMaLoaiDocGia.PrefixSuffixText = null;
            this.txtMaLoaiDocGia.ReadOnly = false; // Sẽ được kiểm soát bằng code-behind
            this.txtMaLoaiDocGia.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtMaLoaiDocGia.SelectedText = "";
            this.txtMaLoaiDocGia.SelectionLength = 0;
            this.txtMaLoaiDocGia.SelectionStart = 0;
            this.txtMaLoaiDocGia.ShortcutsEnabled = true;
            this.txtMaLoaiDocGia.Size = new System.Drawing.Size(372, 48);
            this.txtMaLoaiDocGia.TabIndex = 1;
            this.txtMaLoaiDocGia.TabStop = false; // Để code-behind quản lý TabStop
            this.txtMaLoaiDocGia.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtMaLoaiDocGia.TrailingIcon = null;
            this.txtMaLoaiDocGia.UseSystemPasswordChar = false;
            this.txtMaLoaiDocGia.UseTallSize = false;
            //
            // txtTenLoaiDocGia
            //
            this.txtTenLoaiDocGia.AnimateReadOnly = false;
            this.txtTenLoaiDocGia.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtTenLoaiDocGia.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtTenLoaiDocGia.Depth = 0;
            this.txtTenLoaiDocGia.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtTenLoaiDocGia.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtTenLoaiDocGia.HideSelection = true;
            this.txtTenLoaiDocGia.Hint = "Tên Loại Độc Giả";
            this.txtTenLoaiDocGia.LeadingIcon = null;
            this.txtTenLoaiDocGia.Location = new System.Drawing.Point(3, 126); // Điều chỉnh vị trí Y
            this.txtTenLoaiDocGia.Margin = new System.Windows.Forms.Padding(3, 3, 3, 8); // Tăng margin bottom
            this.txtTenLoaiDocGia.MaxLength = 50;
            this.txtTenLoaiDocGia.MouseState = MaterialSkin.MouseState.OUT;
            this.txtTenLoaiDocGia.Name = "txtTenLoaiDocGia";
            this.txtTenLoaiDocGia.PasswordChar = '\0';
            this.txtTenLoaiDocGia.PrefixSuffixText = null;
            this.txtTenLoaiDocGia.ReadOnly = false; // Sẽ được kiểm soát bằng code-behind
            this.txtTenLoaiDocGia.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtTenLoaiDocGia.SelectedText = "";
            this.txtTenLoaiDocGia.SelectionLength = 0;
            this.txtTenLoaiDocGia.SelectionStart = 0;
            this.txtTenLoaiDocGia.ShortcutsEnabled = true;
            this.txtTenLoaiDocGia.Size = new System.Drawing.Size(372, 48);
            this.txtTenLoaiDocGia.TabIndex = 2;
            this.txtTenLoaiDocGia.TabStop = false; // Để code-behind quản lý TabStop
            this.txtTenLoaiDocGia.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtTenLoaiDocGia.TrailingIcon = null;
            this.txtTenLoaiDocGia.UseSystemPasswordChar = false;
            this.txtTenLoaiDocGia.UseTallSize = false;
            //
            // panelActions
            //
            this.panelActions.Controls.Add(this.flpDetailActions); // Thêm FlowLayoutPanel vào đây
            this.panelActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelActions.Location = new System.Drawing.Point(14, 720); // Điều chỉnh Y theo Padding
            this.panelActions.Margin = new System.Windows.Forms.Padding(0);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(378, 58); // Điều chỉnh Size nếu cần
            this.panelActions.TabIndex = 1;
            //
            // flpDetailActions
            //
            this.flpDetailActions.Controls.Add(this.btnLuu); // Thêm nút Lưu trước
            this.flpDetailActions.Controls.Add(this.btnBoQua); // Thêm nút Bỏ qua sau
            this.flpDetailActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpDetailActions.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft; // Chảy từ phải sang trái
            this.flpDetailActions.Location = new System.Drawing.Point(0, 0);
            this.flpDetailActions.Margin = new System.Windows.Forms.Padding(0);
            this.flpDetailActions.Name = "flpDetailActions";
            this.flpDetailActions.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10); // Thêm Padding top/bottom
            this.flpDetailActions.Size = new System.Drawing.Size(378, 58);
            this.flpDetailActions.TabIndex = 0;
            this.flpDetailActions.WrapContents = false; // Không xuống dòng
            //
            // btnLuu
            //
            this.btnLuu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLuu.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnLuu.Depth = 0;
            this.btnLuu.HighEmphasis = true;
            this.btnLuu.Icon = null;
            this.btnLuu.Location = new System.Drawing.Point(294, 10); // Vị trí được FlowLayoutPanel quản lý
            this.btnLuu.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0); // Margin top/bottom = 0 do đã có Padding
            this.btnLuu.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnLuu.Size = new System.Drawing.Size(80, 36);
            this.btnLuu.TabIndex = 0; // TabIndex quan trọng
            this.btnLuu.Text = "Lưu Lại";
            this.btnLuu.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnLuu.UseAccentColor = true;
            this.btnLuu.UseVisualStyleBackColor = true;
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            //
            // btnBoQua
            //
            this.btnBoQua.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnBoQua.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnBoQua.Depth = 0;
            this.btnBoQua.HighEmphasis = false;
            this.btnBoQua.Icon = null;
            this.btnBoQua.Location = new System.Drawing.Point(209, 10); // Vị trí được FlowLayoutPanel quản lý
            this.btnBoQua.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.btnBoQua.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnBoQua.Name = "btnBoQua";
            this.btnBoQua.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnBoQua.Size = new System.Drawing.Size(77, 36);
            this.btnBoQua.TabIndex = 1; // TabIndex quan trọng
            this.btnBoQua.Text = "Bỏ Qua";
            this.btnBoQua.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnBoQua.UseAccentColor = false;
            this.btnBoQua.UseVisualStyleBackColor = true;
            this.btnBoQua.Click += new System.EventHandler(this.btnBoQua_Click);
            //
            // panelGrid
            //
            this.panelGrid.Controls.Add(this.dgvLoaiDocGia);
            this.panelGrid.Controls.Add(this.panelGridActions);
            this.panelGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGrid.Location = new System.Drawing.Point(10, 10);
            this.panelGrid.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0); // Giữ Padding phải để cách panelDetails
            this.panelGrid.Size = new System.Drawing.Size(774, 792); // (Width - PaddingLeft - PaddingRight - panelDetails.Width)
            this.panelGrid.TabIndex = 0;
            //
            // dgvLoaiDocGia
            //
            this.dgvLoaiDocGia.AllowUserToAddRows = false;
            this.dgvLoaiDocGia.AllowUserToDeleteRows = false;
            this.dgvLoaiDocGia.AllowUserToResizeRows = false;
            this.dgvLoaiDocGia.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLoaiDocGia.BackgroundColor = System.Drawing.Color.White;
            this.dgvLoaiDocGia.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLoaiDocGia.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvLoaiDocGia.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight; // Giữ màu highlight mặc định
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLoaiDocGia.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLoaiDocGia.ColumnHeadersHeight = 40;
            this.dgvLoaiDocGia.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight; // Giữ màu highlight mặc định
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLoaiDocGia.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvLoaiDocGia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLoaiDocGia.EnableHeadersVisualStyles = false;
            this.dgvLoaiDocGia.GridColor = System.Drawing.Color.Gainsboro;
            this.dgvLoaiDocGia.Location = new System.Drawing.Point(0, 60); // Dưới panelGridActions
            this.dgvLoaiDocGia.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvLoaiDocGia.MultiSelect = false;
            this.dgvLoaiDocGia.Name = "dgvLoaiDocGia";
            this.dgvLoaiDocGia.ReadOnly = true;
            this.dgvLoaiDocGia.RowHeadersVisible = false;
            this.dgvLoaiDocGia.RowHeadersWidth = 51;
            this.dgvLoaiDocGia.RowTemplate.Height = 36;
            this.dgvLoaiDocGia.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLoaiDocGia.Size = new System.Drawing.Size(764, 732); // (panelGrid.Width - panelGrid.Padding.Right)
            this.dgvLoaiDocGia.TabIndex = 1;
            // Sự kiện gán ở code-behind
            //
            // panelGridActions
            //
            this.panelGridActions.Controls.Add(this.txtTimKiem); // Thêm Tìm kiếm trước để Fill
            this.panelGridActions.Controls.Add(this.btnThem);    // Thêm nút Thêm (Dock Left)
            this.panelGridActions.Controls.Add(this.flpGridActionsRight); // Thêm FLP chứa các nút phải (Dock Right)
            this.panelGridActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelGridActions.Location = new System.Drawing.Point(0, 0);
            this.panelGridActions.Margin = new System.Windows.Forms.Padding(0);
            this.panelGridActions.Name = "panelGridActions";
            this.panelGridActions.Padding = new System.Windows.Forms.Padding(5, 8, 5, 8); // Thêm Padding top/bottom
            this.panelGridActions.Size = new System.Drawing.Size(764, 60); // Chiều cao panel action
            this.panelGridActions.TabIndex = 0;
            //
            // txtTimKiem
            //
            this.txtTimKiem.AnimateReadOnly = false;
            this.txtTimKiem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.txtTimKiem.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.txtTimKiem.Depth = 0;
            this.txtTimKiem.Dock = System.Windows.Forms.DockStyle.Fill; // Cho phép Fill không gian giữa nút Trái và Phải
            this.txtTimKiem.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtTimKiem.HideSelection = true;
            this.txtTimKiem.Hint = "Tìm kiếm theo Mã hoặc Tên...";
            this.txtTimKiem.LeadingIcon = null;
            this.txtTimKiem.Location = new System.Drawing.Point(118, 8); // Vị trí X sau nút Thêm + margin
            this.txtTimKiem.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0); // Thêm Margin trái/phải để tạo khoảng cách
            this.txtTimKiem.MaxLength = 32767;
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
            this.txtTimKiem.Size = new System.Drawing.Size(403, 48); // Chiều cao chuẩn, Width sẽ tự Fill
            this.txtTimKiem.TabIndex = 1; // Sau nút Thêm
            this.txtTimKiem.TabStop = false;
            this.txtTimKiem.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtTimKiem.TrailingIcon = null;
            this.txtTimKiem.UseSystemPasswordChar = false;
            this.txtTimKiem.UseTallSize = false; // Đảm bảo chiều cao 48px
            this.txtTimKiem.TextChanged += new System.EventHandler(this.txtTimKiem_TextChanged);
            //
            // btnThem
            //
            this.btnThem.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThem.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnThem.Depth = 0;
            this.btnThem.Dock = System.Windows.Forms.DockStyle.Left; // Dock sang trái
            this.btnThem.HighEmphasis = true;
            this.btnThem.Icon = null;
            this.btnThem.Location = new System.Drawing.Point(5, 8); // Vị trí X theo Padding
            this.btnThem.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0); // Chỉ cần margin phải nếu muốn thêm khoảng cách
            this.btnThem.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnThem.Name = "btnThem";
            this.btnThem.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnThem.Size = new System.Drawing.Size(101, 44); // Height theo Padding panel
            this.btnThem.TabIndex = 0; // TabIndex đầu tiên
            this.btnThem.Text = "Thêm Mới";
            this.btnThem.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnThem.UseAccentColor = true;
            this.btnThem.UseVisualStyleBackColor = true;
            this.btnThem.Click += new System.EventHandler(this.btnThem_Click);
            //
            // flpGridActionsRight
            //
            this.flpGridActionsRight.AutoSize = true; // Tự điều chỉnh width
            this.flpGridActionsRight.Controls.Add(this.btnExportCsv);
            this.flpGridActionsRight.Controls.Add(this.btnSua);
            this.flpGridActionsRight.Controls.Add(this.btnXoa);
            this.flpGridActionsRight.Dock = System.Windows.Forms.DockStyle.Right; // Dock sang phải
            this.flpGridActionsRight.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight; // Chảy từ trái sang phải
            this.flpGridActionsRight.Location = new System.Drawing.Point(521, 8); // Vị trí X trước Padding phải
            this.flpGridActionsRight.Margin = new System.Windows.Forms.Padding(0);
            this.flpGridActionsRight.Name = "flpGridActionsRight";
            this.flpGridActionsRight.Size = new System.Drawing.Size(238, 44); // Width sẽ tự tính, Height theo Padding panel
            this.flpGridActionsRight.TabIndex = 2; // Sau Tìm kiếm
            this.flpGridActionsRight.WrapContents = false;
            //
            // btnExportCsv
            //
            this.btnExportCsv.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnExportCsv.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnExportCsv.Depth = 0;
            this.btnExportCsv.HighEmphasis = false;
            this.btnExportCsv.Icon = null;
            this.btnExportCsv.Location = new System.Drawing.Point(4, 4); // Điều chỉnh margin trong FLP nếu cần
            this.btnExportCsv.Margin = new System.Windows.Forms.Padding(4); // Margin đều 4 cạnh
            this.btnExportCsv.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnExportCsv.Size = new System.Drawing.Size(96, 36); // Giữ nguyên Size
            this.btnExportCsv.TabIndex = 0; // TabIndex trong FLP này
            this.btnExportCsv.Text = "Xuất CSV";
            this.btnExportCsv.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnExportCsv.UseAccentColor = false;
            this.btnExportCsv.UseVisualStyleBackColor = true;
            this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            //
            // btnSua
            //
            this.btnSua.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSua.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnSua.Depth = 0;
            this.btnSua.HighEmphasis = false;
            this.btnSua.Icon = null;
            this.btnSua.Location = new System.Drawing.Point(108, 4); // Vị trí X do FLP quản lý
            this.btnSua.Margin = new System.Windows.Forms.Padding(4);
            this.btnSua.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSua.Name = "btnSua";
            this.btnSua.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnSua.Size = new System.Drawing.Size(64, 36);
            this.btnSua.TabIndex = 1; // TabIndex tiếp theo
            this.btnSua.Text = "Sửa";
            this.btnSua.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text;
            this.btnSua.UseAccentColor = false;
            this.btnSua.UseVisualStyleBackColor = true;
            this.btnSua.Click += new System.EventHandler(this.btnSua_Click);
            //
            // btnXoa
            //
            this.btnXoa.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnXoa.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnXoa.Depth = 0;
            this.btnXoa.HighEmphasis = false;
            this.btnXoa.Icon = null;
            this.btnXoa.Location = new System.Drawing.Point(180, 4); // Vị trí X do FLP quản lý
            this.btnXoa.Margin = new System.Windows.Forms.Padding(4);
            this.btnXoa.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.NoAccentTextColor = System.Drawing.Color.Red;
            this.btnXoa.Size = new System.Drawing.Size(54, 36); // AutoSize theo Text "XÓA"
            this.btnXoa.TabIndex = 2; // TabIndex cuối trong nhóm phải
            this.btnXoa.Text = "Xóa";
            this.btnXoa.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text;
            this.btnXoa.UseAccentColor = false;
            this.btnXoa.UseVisualStyleBackColor = true;
            this.btnXoa.Click += new System.EventHandler(this.btnXoa_Click);
            //
            // ucQuanLyLoaiDocGia
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.panelDetails);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ucQuanLyLoaiDocGia";
            this.Padding = new System.Windows.Forms.Padding(10); // Padding chung
            this.Size = new System.Drawing.Size(1200, 812); // Giữ nguyên Size hoặc điều chỉnh
            this.Load += new System.EventHandler(this.ucQuanLyLoaiDocGia_Load);
            this.panelDetails.ResumeLayout(false);
            this.tableLayoutPanelDetails.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.flpDetailActions.ResumeLayout(false); // Thêm vào ResumeLayout
            this.flpDetailActions.PerformLayout();
            this.panelActions.PerformLayout(); // Cần gọi PerformLayout cho panel chứa FLP
            this.panelGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoaiDocGia)).EndInit();
            this.panelGridActions.ResumeLayout(false);
            this.panelGridActions.PerformLayout(); // Gọi PerformLayout cho panel chứa các control dock
            this.flpGridActionsRight.ResumeLayout(false); // Thêm vào ResumeLayout
            this.flpGridActionsRight.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        // --- KHAI BÁO BIẾN CONTROL ---
        private MaterialSkin.Controls.MaterialCard panelDetails;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelDetails;
        private MaterialSkin.Controls.MaterialTextBox2 txtTenLoaiDocGia;
        private MaterialSkin.Controls.MaterialTextBox2 txtMaLoaiDocGia;
        private MaterialSkin.Controls.MaterialTextBox2 txtId;
        private System.Windows.Forms.Panel panelActions;
        private System.Windows.Forms.Panel panelGrid;
        private System.Windows.Forms.DataGridView dgvLoaiDocGia;
        private System.Windows.Forms.Panel panelGridActions;
        private MaterialSkin.Controls.MaterialButton btnThem;
        // --- KHAI BÁO BIẾN CONTROL MỚI/THAY ĐỔI ---
        private MaterialTextBox2 txtTimKiem;
        private MaterialButton btnExportCsv;
        private MaterialButton btnSua;
        private MaterialButton btnXoa;
        private FlowLayoutPanel flpGridActionsRight; // FLP cho nhóm nút phải (Grid)
        private FlowLayoutPanel flpDetailActions;   // FLP cho nhóm nút phải (Detail)
        private MaterialButton btnLuu;
        private MaterialButton btnBoQua;

    }
}