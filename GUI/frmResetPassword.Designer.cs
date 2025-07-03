namespace GUI // Đảm bảo đúng namespace
{
    partial class frmResetPassword
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
            this.lblTenDangNhap = new System.Windows.Forms.Label();
            this.txtTenDangNhapReset = new System.Windows.Forms.TextBox();
            this.lblMatKhauMoi = new System.Windows.Forms.Label();
            this.txtMatKhauMoiReset = new System.Windows.Forms.TextBox();
            this.lblXacNhanMK = new System.Windows.Forms.Label();
            this.txtXacNhanMatKhauReset = new System.Windows.Forms.TextBox();
            this.btnXacNhanReset = new System.Windows.Forms.Button();
            this.btnHuyBoReset = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblTenDangNhap
            //
            this.lblTenDangNhap.AutoSize = true;
            this.lblTenDangNhap.Location = new System.Drawing.Point(30, 30); // Ví dụ vị trí
            this.lblTenDangNhap.Name = "lblTenDangNhap";
            this.lblTenDangNhap.Size = new System.Drawing.Size(110, 15); // Ví dụ kích thước
            this.lblTenDangNhap.TabIndex = 0; // Thứ tự Tab
            this.lblTenDangNhap.Text = "Tên đăng nhập:";
            //
            // txtTenDangNhapReset
            //
            this.txtTenDangNhapReset.Location = new System.Drawing.Point(150, 27); // Ví dụ vị trí
            this.txtTenDangNhapReset.Name = "txtTenDangNhapReset";
            this.txtTenDangNhapReset.Size = new System.Drawing.Size(200, 23); // Ví dụ kích thước
            this.txtTenDangNhapReset.TabIndex = 1; // Thứ tự Tab
            //
            // lblMatKhauMoi
            //
            this.lblMatKhauMoi.AutoSize = true;
            this.lblMatKhauMoi.Location = new System.Drawing.Point(30, 70); // Ví dụ vị trí
            this.lblMatKhauMoi.Name = "lblMatKhauMoi";
            this.lblMatKhauMoi.Size = new System.Drawing.Size(90, 15); // Ví dụ kích thước
            this.lblMatKhauMoi.TabIndex = 2;
            this.lblMatKhauMoi.Text = "Mật khẩu mới:";
            //
            // txtMatKhauMoiReset
            //
            this.txtMatKhauMoiReset.Location = new System.Drawing.Point(150, 67); // Ví dụ vị trí
            this.txtMatKhauMoiReset.Name = "txtMatKhauMoiReset";
            this.txtMatKhauMoiReset.PasswordChar = '*'; // Hiển thị dấu *
            this.txtMatKhauMoiReset.Size = new System.Drawing.Size(200, 23); // Ví dụ kích thước
            this.txtMatKhauMoiReset.TabIndex = 3;
            //
            // lblXacNhanMK
            //
            this.lblXacNhanMK.AutoSize = true;
            this.lblXacNhanMK.Location = new System.Drawing.Point(30, 110); // Ví dụ vị trí
            this.lblXacNhanMK.Name = "lblXacNhanMK";
            this.lblXacNhanMK.Size = new System.Drawing.Size(118, 15); // Ví dụ kích thước
            this.lblXacNhanMK.TabIndex = 4;
            this.lblXacNhanMK.Text = "Xác nhận MK mới:";
            //
            // txtXacNhanMatKhauReset
            //
            this.txtXacNhanMatKhauReset.Location = new System.Drawing.Point(150, 107); // Ví dụ vị trí
            this.txtXacNhanMatKhauReset.Name = "txtXacNhanMatKhauReset";
            this.txtXacNhanMatKhauReset.PasswordChar = '*'; // Hiển thị dấu *
            this.txtXacNhanMatKhauReset.Size = new System.Drawing.Size(200, 23); // Ví dụ kích thước
            this.txtXacNhanMatKhauReset.TabIndex = 5;
            //
            // btnXacNhanReset
            //
            this.btnXacNhanReset.Location = new System.Drawing.Point(150, 150); // Ví dụ vị trí
            this.btnXacNhanReset.Name = "btnXacNhanReset";
            this.btnXacNhanReset.Size = new System.Drawing.Size(90, 30); // Ví dụ kích thước
            this.btnXacNhanReset.TabIndex = 6;
            this.btnXacNhanReset.Text = "Xác nhận";
            this.btnXacNhanReset.UseVisualStyleBackColor = true;
            this.btnXacNhanReset.Click += new System.EventHandler(this.btnXacNhanReset_Click); // Gắn sự kiện Click
            //
            // btnHuyBoReset
            //
            this.btnHuyBoReset.Location = new System.Drawing.Point(260, 150); // Ví dụ vị trí
            this.btnHuyBoReset.Name = "btnHuyBoReset";
            this.btnHuyBoReset.Size = new System.Drawing.Size(90, 30); // Ví dụ kích thước
            this.btnHuyBoReset.TabIndex = 7;
            this.btnHuyBoReset.Text = "Hủy bỏ";
            this.btnHuyBoReset.UseVisualStyleBackColor = true;
            this.btnHuyBoReset.Click += new System.EventHandler(this.btnHuyBoReset_Click); // Gắn sự kiện Click (có thể không cần nếu đặt DialogResult)
            //
            // frmResetPassword
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F); // Có thể khác tùy cấu hình Font
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 211); // Ví dụ kích thước Form
            this.Controls.Add(this.btnHuyBoReset);
            this.Controls.Add(this.btnXacNhanReset);
            this.Controls.Add(this.txtXacNhanMatKhauReset);
            this.Controls.Add(this.lblXacNhanMK);
            this.Controls.Add(this.txtMatKhauMoiReset);
            this.Controls.Add(this.lblMatKhauMoi);
            this.Controls.Add(this.txtTenDangNhapReset);
            this.Controls.Add(this.lblTenDangNhap);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog; // Như đã cấu hình trong frmResetPassword.cs
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmResetPassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; // Như đã cấu hình trong frmResetPassword.cs
            this.Text = "Đặt Lại Mật Khẩu"; // Tiêu đề Form
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        // Khai báo các biến cho điều khiển để frmResetPassword.cs có thể truy cập
        private System.Windows.Forms.Label lblTenDangNhap;
        private System.Windows.Forms.TextBox txtTenDangNhapReset;
        private System.Windows.Forms.Label lblMatKhauMoi;
        private System.Windows.Forms.TextBox txtMatKhauMoiReset;
        private System.Windows.Forms.Label lblXacNhanMK;
        private System.Windows.Forms.TextBox txtXacNhanMatKhauReset;
        private System.Windows.Forms.Button btnXacNhanReset;
        private System.Windows.Forms.Button btnHuyBoReset;
    }
}