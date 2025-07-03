// Project/Namespace: GUI

using System;
using System.Windows.Forms; // Cần cho các loại control, AutoScaleMode, SizeF, Point, EventArgs, DockStyle, ComboBoxStyle
using MaterialSkin.Controls; // Cần cho MaterialButton, MaterialSkin.MouseState
using System.Drawing; // Cần cho Point, Size
using System.ComponentModel; // Cần cho IContainer, ComponentResourceManager
using System.Collections.Generic; // Cần cho List (cho DataSource của ComboBox/DGV trong Designer nếu có)


namespace GUI // Namespace của project GUI của bạn
{
    // Sử dụng partial để class được định nghĩa ở nhiều file
    partial class ucThongKe
    {
        /// <summary>
        /// Required designer variable.
        /// Biến này được quản lý bởi trình thiết kế (Designer).
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // --- KHAI BÁO CÁC CONTROLS UI (Biến thành viên partial class) ---
        private System.Windows.Forms.ComboBox cboMonth;
        private System.Windows.Forms.ComboBox cboYear;

        private MaterialSkin.Controls.MaterialButton btnXemBaoCao;
        private MaterialSkin.Controls.MaterialButton btnThoat;

        private System.Windows.Forms.DataGridView dgvReport;

        private System.Windows.Forms.Panel panelParameters;
        private System.Windows.Forms.Panel panelReport;

        // Thêm các Label nếu bạn có dùng Label để chú thích
        private MaterialSkin.Controls.MaterialLabel lblMonth;
        private MaterialSkin.Controls.MaterialLabel lblYear;


        /// <summary>
        /// Clean up any resources being used.
        /// Giải phóng các tài nguyên đang được sử dụng.
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
        /// Phương thức bắt buộc cho Designer - không được sửa đổi nội dung
        /// của phương thức này bằng trình soạn thảo mã (code editor).
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelParameters = new System.Windows.Forms.Panel();
            this.lblYear = new MaterialSkin.Controls.MaterialLabel(); // Khởi tạo label
            this.lblMonth = new MaterialSkin.Controls.MaterialLabel(); // Khởi tạo label
            this.cboYear = new System.Windows.Forms.ComboBox();
            this.cboMonth = new System.Windows.Forms.ComboBox();
            this.btnXemBaoCao = new MaterialSkin.Controls.MaterialButton();
            this.panelReport = new System.Windows.Forms.Panel();
            this.dgvReport = new System.Windows.Forms.DataGridView();
            this.btnThoat = new MaterialSkin.Controls.MaterialButton();

            this.panelParameters.SuspendLayout();
            this.panelReport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).BeginInit();
            this.SuspendLayout();
            // 
            // panelParameters
            // 
            this.panelParameters.Controls.Add(this.lblYear); // Thêm label vào panel
            this.panelParameters.Controls.Add(this.lblMonth); // Thêm label vào panel
            this.panelParameters.Controls.Add(this.cboYear);
            this.panelParameters.Controls.Add(this.cboMonth);
            this.panelParameters.Controls.Add(this.btnXemBaoCao);
            this.panelParameters.Dock = DockStyle.Top;
            this.panelParameters.Location = new System.Drawing.Point(10, 10); // Sau Padding UserControl
            this.panelParameters.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.panelParameters.Name = "panelParameters";
            this.panelParameters.Padding = new Padding(10); // *** ĐỔI PADDING ***
            this.panelParameters.Size = new System.Drawing.Size(780, 50); // Kích thước sau Padding
            this.panelParameters.TabIndex = 0;
            // 
            // lblYear
            // 
            this.lblYear.AutoSize = true;
            this.lblYear.Depth = 0;
            this.lblYear.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblYear.Location = new Point(164, 17); // Vị trí trước cboYear
            this.lblYear.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.lblYear.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblYear.Name = "lblYear";
            this.lblYear.Size = new System.Drawing.Size(41, 19);
            this.lblYear.TabIndex = 4;
            this.lblYear.Text = "Năm:";
            // 
            // lblMonth
            // 
            this.lblMonth.AutoSize = true;
            this.lblMonth.Depth = 0;
            this.lblMonth.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblMonth.Location = new Point(13, 17); // Vị trí trước cboMonth
            this.lblMonth.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.lblMonth.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblMonth.Name = "lblMonth";
            this.lblMonth.Size = new System.Drawing.Size(51, 19);
            this.lblMonth.TabIndex = 3;
            this.lblMonth.Text = "Tháng:";
            // 
            // cboYear
            // 
            this.cboYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboYear.FormattingEnabled = true;
            this.cboYear.Location = new Point(211, 15); // Vị trí sau lblYear
            this.cboYear.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.cboYear.Name = "cboYear";
            this.cboYear.Size = new System.Drawing.Size(100, 24);
            this.cboYear.TabIndex = 1;
            // 
            // cboMonth
            // 
            this.cboMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMonth.FormattingEnabled = true;
            this.cboMonth.Location = new Point(70, 15); // Vị trí sau lblMonth
            this.cboMonth.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.cboMonth.Name = "cboMonth";
            this.cboMonth.Size = new System.Drawing.Size(80, 24); // Điều chỉnh Size nếu cần
            this.cboMonth.TabIndex = 0;
            // 
            // btnXemBaoCao
            // 
            this.btnXemBaoCao.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnXemBaoCao.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnXemBaoCao.Depth = 0;
            this.btnXemBaoCao.HighEmphasis = true;
            this.btnXemBaoCao.Icon = null;
            this.btnXemBaoCao.Location = new Point(331, 10); // Vị trí sau cboYear
            this.btnXemBaoCao.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.btnXemBaoCao.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnXemBaoCao.Name = "btnXemBaoCao";
            this.btnXemBaoCao.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnXemBaoCao.Size = new System.Drawing.Size(129, 36);
            this.btnXemBaoCao.TabIndex = 2;
            this.btnXemBaoCao.Text = "Xem báo cáo";
            this.btnXemBaoCao.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnXemBaoCao.UseAccentColor = false;
            this.btnXemBaoCao.UseVisualStyleBackColor = true;
            this.btnXemBaoCao.Click += new System.EventHandler(this.btnXemBaoCao_Click);
            // 
            // panelReport
            // 
            this.panelReport.Controls.Add(this.dgvReport);
            this.panelReport.Dock = DockStyle.Fill;
            this.panelReport.Location = new Point(10, 60); // Vị trí sau panelParameters và Padding UserControl
            this.panelReport.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.panelReport.Name = "panelReport";
            this.panelReport.Padding = new Padding(10); // *** ĐỔI PADDING ***
            this.panelReport.Size = new System.Drawing.Size(780, 530); // Kích thước sau Padding UserControl
            this.panelReport.TabIndex = 1;
            // 
            // dgvReport
            // 
            this.dgvReport.AllowUserToAddRows = false;
            this.dgvReport.AllowUserToDeleteRows = false;
            this.dgvReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReport.Location = new System.Drawing.Point(10, 10); // Vị trí sau Padding panelReport
            this.dgvReport.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.dgvReport.Name = "dgvReport";
            this.dgvReport.RowHeadersWidth = 51;
            this.dgvReport.RowTemplate.Height = 24;
            this.dgvReport.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReport.MultiSelect = false;
            this.dgvReport.ReadOnly = true;
            this.dgvReport.Size = new System.Drawing.Size(760, 510); // Kích thước sau Padding
            this.dgvReport.TabIndex = 0;
            // 
            // btnThoat
            // 
            this.btnThoat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnThoat.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnThoat.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnThoat.Depth = 0;
            this.btnThoat.HighEmphasis = true;
            this.btnThoat.Icon = null;
            this.btnThoat.Location = new Point(474, 10); // Vị trí sau btnXemBaoCao
            this.btnThoat.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.btnThoat.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnThoat.Size = new System.Drawing.Size(74, 36);
            this.btnThoat.TabIndex = 3;
            this.btnThoat.Text = "Thoát";
            this.btnThoat.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text;
            this.btnThoat.UseAccentColor = false;
            this.btnThoat.UseVisualStyleBackColor = true;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // ucThongKe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelReport);
            this.Controls.Add(this.panelParameters);
            // Nút Thoát giờ nằm trong panelParameters, không cần add riêng ở đây
            this.Margin = new Padding(3); // *** ĐỔI MARGIN ***
            this.Name = "ucThongKe";
            this.Padding = new Padding(10); // *** ĐỔI PADDING ***
            this.Size = new System.Drawing.Size(800, 600);
            this.Load += new System.EventHandler(this.ucThongKe_Load);
            this.panelParameters.ResumeLayout(false);
            this.panelParameters.PerformLayout();
            this.panelReport.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}