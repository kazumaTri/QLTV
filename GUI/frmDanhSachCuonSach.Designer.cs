// File: GUI/frmDanhSachCuonSach.Designer.cs
// Project/Namespace: GUI

using System.Windows.Forms;
using MaterialSkin.Controls; // Cần cho MaterialForm, MaterialButton
using System.Drawing;       // Cần cho Point, Size, SizeF, Color
using System.ComponentModel;// Cần cho IContainer

namespace GUI
{
    partial class frmDanhSachCuonSach
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
            // Khởi tạo các controls
            this.dgvCuonSachList = new System.Windows.Forms.DataGridView();
            this.btnDong = new MaterialSkin.Controls.MaterialButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCuonSachList)).BeginInit();
            this.SuspendLayout();
            //
            // dgvCuonSachList
            //
            this.dgvCuonSachList.AllowUserToAddRows = false;
            this.dgvCuonSachList.AllowUserToDeleteRows = false;
            this.dgvCuonSachList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))); // Neo vào các cạnh
            this.dgvCuonSachList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCuonSachList.Location = new System.Drawing.Point(12, 76); // Vị trí bắt đầu (dưới thanh tiêu đề MaterialForm)
            this.dgvCuonSachList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvCuonSachList.Name = "dgvCuonSachList";
            this.dgvCuonSachList.ReadOnly = true;
            this.dgvCuonSachList.RowHeadersWidth = 51;
            this.dgvCuonSachList.RowTemplate.Height = 24;
            this.dgvCuonSachList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCuonSachList.Size = new System.Drawing.Size(576, 280); // Kích thước ban đầu
            this.dgvCuonSachList.TabIndex = 0;
            //
            // btnDong
            //
            this.btnDong.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right))); // Neo vào góc dưới bên phải
            this.btnDong.AutoSize = false;
            this.btnDong.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDong.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnDong.Depth = 0;
            this.btnDong.HighEmphasis = true;
            this.btnDong.Icon = null;
            this.btnDong.Location = new System.Drawing.Point(488, 368); // Vị trí nút Đóng
            this.btnDong.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnDong.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnDong.Name = "btnDong";
            this.btnDong.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnDong.Size = new System.Drawing.Size(100, 36); // Kích thước nút Đóng
            this.btnDong.TabIndex = 1;
            this.btnDong.Text = "Đóng";
            this.btnDong.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnDong.UseAccentColor = false;
            this.btnDong.UseVisualStyleBackColor = true;
            this.btnDong.DialogResult = System.Windows.Forms.DialogResult.Cancel; // Quan trọng: Đặt DialogResult
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click); // Gán sự kiện Click
            //
            // frmDanhSachCuonSach
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 420); // Kích thước Form ban đầu
            this.Controls.Add(this.btnDong);
            this.Controls.Add(this.dgvCuonSachList);
            this.MaximizeBox = false; // Không cho phép phóng to tối đa
            this.MinimizeBox = false; // Không cho phép thu nhỏ
            this.Name = "frmDanhSachCuonSach";
            this.Padding = new System.Windows.Forms.Padding(3, 64, 3, 3); // Padding cho MaterialForm
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; // Hiển thị giữa Form cha
            this.Text = "Danh sách Cuốn Sách"; // Tiêu đề mặc định
            this.Load += new System.EventHandler(this.frmDanhSachCuonSach_Load); // Gán sự kiện Load
            ((System.ComponentModel.ISupportInitialize)(this.dgvCuonSachList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        // Khai báo các controls để có thể truy cập từ code-behind (frmDanhSachCuonSach.cs)
        private System.Windows.Forms.DataGridView dgvCuonSachList;
        private MaterialSkin.Controls.MaterialButton btnDong;
    }
}
