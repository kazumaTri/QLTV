// File: GUI/frmLichSuMuonTra.Designer.cs
namespace GUI
{
    partial class frmLichSuMuonTra
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // Khởi tạo các control
            this.dgvLichSu = new System.Windows.Forms.DataGridView();
            // Sử dụng MaterialButton nếu dự án GUI của bạn đang dùng MaterialSkin
            this.btnDong = new MaterialSkin.Controls.MaterialButton();
            // Nếu không dùng MaterialSkin, thay dòng trên bằng:
            // this.btnDong = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvLichSu)).BeginInit();
            this.SuspendLayout();

            //
            // dgvLichSu (DataGridView để hiển thị lịch sử)
            //
            this.dgvLichSu.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLichSu.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            // Điều chỉnh vị trí và kích thước cho phù hợp
            // Nếu dùng MaterialForm, vị trí Top thường bắt đầu từ khoảng 70-80 do có thanh tiêu đề Material
            this.dgvLichSu.Location = new System.Drawing.Point(16, 76);
            this.dgvLichSu.Name = "dgvLichSu";
            this.dgvLichSu.RowHeadersWidth = 51; // Có thể đặt thành 40 hoặc ẩn đi (RowHeadersVisible = false)
            this.dgvLichSu.RowTemplate.Height = 28; // Chiều cao dòng
            this.dgvLichSu.Size = new System.Drawing.Size(852, 373); // Kích thước DataGridView
            this.dgvLichSu.TabIndex = 0;
            // Các thuộc tính khác sẽ được đặt trong code-behind (frmLichSuMuonTra.cs) như ReadOnly, AllowUserToAddRows, ...

            //
            // btnDong (Nút Đóng)
            //
            this.btnDong.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            // Các thuộc tính MaterialButton (nếu dùng)
            this.btnDong.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDong.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnDong.Depth = 0;
            this.btnDong.HighEmphasis = true; // Nút chính
            this.btnDong.Icon = null;
            this.btnDong.Location = new System.Drawing.Point(768, 465); // Vị trí nút Đóng (căn phải dưới)
            this.btnDong.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnDong.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnDong.Name = "btnDong";
            this.btnDong.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnDong.Size = new System.Drawing.Size(100, 36); // Kích thước nút
            this.btnDong.TabIndex = 1;
            this.btnDong.Text = "ĐÓNG";
            this.btnDong.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnDong.UseAccentColor = false; // Màu mặc định
            this.btnDong.UseVisualStyleBackColor = true;
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click); // Gắn sự kiện Click

            // Nếu không dùng MaterialSkin, thay cấu hình btnDong bằng:
            // this.btnDong.Location = new System.Drawing.Point(700, 410); // Điều chỉnh vị trí
            // this.btnDong.Name = "btnDong";
            // this.btnDong.Size = new System.Drawing.Size(88, 30); // Điều chỉnh kích thước
            // this.btnDong.TabIndex = 1;
            // this.btnDong.Text = "Đóng";
            // this.btnDong.UseVisualStyleBackColor = true;
            // this.btnDong.Click += new System.EventHandler(this.btnDong_Click);

            //
            // frmLichSuMuonTra (Form chính)
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F); // Hoặc 9F, 20F tùy vào Font mặc định
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 516); // Kích thước cửa sổ Form
            this.Controls.Add(this.btnDong);
            this.Controls.Add(this.dgvLichSu);
            this.Name = "frmLichSuMuonTra";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; // Hiển thị giữa form cha
            this.Text = "Lịch sử Mượn Trả Sách"; // Tiêu đề Form
            this.Load += new System.EventHandler(this.frmLichSuMuonTra_Load); // Gắn sự kiện Load
            ((System.ComponentModel.ISupportInitialize)(this.dgvLichSu)).EndInit();
            this.ResumeLayout(false);
            // Nếu dùng MaterialButton, cần thêm PerformLayout
            this.PerformLayout(); // <<< Thêm dòng này nếu dùng MaterialButton


        }

        #endregion

        // Khai báo biến cho các control
        private System.Windows.Forms.DataGridView dgvLichSu;
        // Khai báo MaterialButton nếu dùng MaterialSkin
        private MaterialSkin.Controls.MaterialButton btnDong;
        // Nếu không dùng MaterialSkin, thay bằng:
        // private System.Windows.Forms.Button btnDong;
    }
}