// --- USING ---
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace GUI
{
    partial class frmMain
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
            components = new System.ComponentModel.Container();
            panelMenu = new MaterialCard();
            flowLayoutPanelMenu = new FlowLayoutPanel();
            panelContent = new MaterialCard();
            panelUserInfoHeader = new Panel();
            btnNotifications = new MaterialButton();
            lblUserInfo = new MaterialLabel();
            panelNotificationList = new MaterialCard();
            flowLayoutPanelNotifications = new FlowLayoutPanel();
            timerNotifications = new System.Windows.Forms.Timer(components);
            panelMenu.SuspendLayout();
            panelUserInfoHeader.SuspendLayout();
            panelNotificationList.SuspendLayout();
            SuspendLayout();
            // 
            // panelMenu
            // 
            panelMenu.BackColor = Color.FromArgb(255, 255, 255);
            panelMenu.Controls.Add(flowLayoutPanelMenu);
            panelMenu.Depth = 0;
            panelMenu.Dock = DockStyle.Left;
            panelMenu.ForeColor = Color.FromArgb(222, 0, 0, 0);
            panelMenu.Location = new Point(3, 64);
            panelMenu.Margin = new Padding(0);
            panelMenu.MouseState = MouseState.HOVER;
            panelMenu.Name = "panelMenu";
            panelMenu.Padding = new Padding(14);
            panelMenu.Size = new Size(260, 708);
            panelMenu.TabIndex = 0;
            // 
            // flowLayoutPanelMenu
            // 
            flowLayoutPanelMenu.AutoScroll = true;
            flowLayoutPanelMenu.BackColor = Color.Transparent;
            flowLayoutPanelMenu.Dock = DockStyle.Fill;
            flowLayoutPanelMenu.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelMenu.Location = new Point(14, 14);
            flowLayoutPanelMenu.Margin = new Padding(0);
            flowLayoutPanelMenu.Name = "flowLayoutPanelMenu";
            flowLayoutPanelMenu.Padding = new Padding(5, 8, 5, 8);
            flowLayoutPanelMenu.Size = new Size(232, 680);
            flowLayoutPanelMenu.TabIndex = 0;
            flowLayoutPanelMenu.WrapContents = false;
            // 
            // panelContent
            // 
            panelContent.BackColor = Color.FromArgb(255, 255, 255);
            panelContent.Depth = 0;
            panelContent.Dock = DockStyle.Fill;
            panelContent.ForeColor = Color.FromArgb(222, 0, 0, 0);
            panelContent.Location = new Point(263, 64);
            panelContent.Margin = new Padding(10);
            panelContent.MouseState = MouseState.HOVER;
            panelContent.Name = "panelContent";
            panelContent.Padding = new Padding(15);
            panelContent.Size = new Size(934, 708);
            panelContent.TabIndex = 1;
            // 
            // panelUserInfoHeader
            // 
            panelUserInfoHeader.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panelUserInfoHeader.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelUserInfoHeader.BackColor = Color.Transparent;
            panelUserInfoHeader.Controls.Add(btnNotifications);
            panelUserInfoHeader.Controls.Add(lblUserInfo);
            panelUserInfoHeader.Location = new Point(923, 25);
            panelUserInfoHeader.Margin = new Padding(0);
            panelUserInfoHeader.Name = "panelUserInfoHeader";
            panelUserInfoHeader.Padding = new Padding(5, 0, 5, 0);
            panelUserInfoHeader.Size = new Size(274, 40);
            panelUserInfoHeader.TabIndex = 3;
            // 
            // btnNotifications
            // 
            btnNotifications.AutoSize = false;
            btnNotifications.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnNotifications.Density = MaterialButton.MaterialButtonDensity.Default;
            btnNotifications.Depth = 0;
            btnNotifications.HighEmphasis = true;
            btnNotifications.Icon = null;
            btnNotifications.Location = new Point(209, 0);
            btnNotifications.Margin = new Padding(4, 6, 4, 6);
            btnNotifications.MouseState = MouseState.HOVER;
            btnNotifications.Name = "btnNotifications";
            btnNotifications.NoAccentTextColor = Color.Empty;
            btnNotifications.Size = new Size(40, 40);
            btnNotifications.TabIndex = 2;
            btnNotifications.Text = "0";
            btnNotifications.Type = MaterialButton.MaterialButtonType.Contained;
            btnNotifications.UseAccentColor = false;
            btnNotifications.UseVisualStyleBackColor = true;
            // 
            // lblUserInfo
            // 
            lblUserInfo.AutoSize = true;
            lblUserInfo.Depth = 0;
            lblUserInfo.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblUserInfo.Location = new Point(8, 7);
            lblUserInfo.Margin = new Padding(3);
            lblUserInfo.MouseState = MouseState.HOVER;
            lblUserInfo.Name = "lblUserInfo";
            lblUserInfo.Size = new Size(161, 19);
            lblUserInfo.TabIndex = 1;
            lblUserInfo.Text = "Chào, Tên Người Dùng";
            lblUserInfo.TextAlign = ContentAlignment.MiddleRight;
            // 
            // panelNotificationList
            // 
            panelNotificationList.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panelNotificationList.BackColor = Color.FromArgb(255, 255, 255);
            panelNotificationList.Controls.Add(flowLayoutPanelNotifications);
            panelNotificationList.Depth = 3;
            panelNotificationList.ForeColor = Color.FromArgb(222, 0, 0, 0);
            panelNotificationList.Location = new Point(802, 75);
            panelNotificationList.Margin = new Padding(10);
            panelNotificationList.MouseState = MouseState.HOVER;
            panelNotificationList.Name = "panelNotificationList";
            panelNotificationList.Padding = new Padding(10);
            panelNotificationList.Size = new Size(380, 350);
            panelNotificationList.TabIndex = 4;
            panelNotificationList.Visible = false;
            // 
            // flowLayoutPanelNotifications
            // 
            flowLayoutPanelNotifications.AutoScroll = true;
            flowLayoutPanelNotifications.Dock = DockStyle.Fill;
            flowLayoutPanelNotifications.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelNotifications.Location = new Point(10, 10);
            flowLayoutPanelNotifications.Name = "flowLayoutPanelNotifications";
            flowLayoutPanelNotifications.Size = new Size(360, 330);
            flowLayoutPanelNotifications.TabIndex = 0;
            flowLayoutPanelNotifications.WrapContents = false;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 775);
            Controls.Add(panelNotificationList);
            Controls.Add(panelContent);
            Controls.Add(panelMenu);
            Controls.Add(panelUserInfoHeader);
            DrawerIndicatorWidth = 3;
            DrawerShowIconsWhenHidden = true;
            DrawerWidth = 260;
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(900, 700);
            Name = "frmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản Lý Thư Viện Hiện Đại";
            Load += frmMain_Load;
            panelMenu.ResumeLayout(false);
            panelUserInfoHeader.ResumeLayout(false);
            panelUserInfoHeader.PerformLayout();
            panelNotificationList.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        // --- KHAI BÁO BIẾN THÀNH VIÊN CHO CÁC CONTROL ---
        // Các biến này cho phép truy cập các control từ code-behind (frmMain.cs)
        private MaterialSkin.Controls.MaterialCard panelMenu;
        // *** THÊM: Khai báo biến cho FlowLayoutPanel menu ***
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelMenu;
        private MaterialSkin.Controls.MaterialCard panelContent;
        private System.Windows.Forms.Panel panelUserInfoHeader; // Dùng Panel thường
        private MaterialSkin.Controls.MaterialLabel lblUserInfo;
        private MaterialSkin.Controls.MaterialButton btnNotifications;
        private MaterialSkin.Controls.MaterialCard panelNotificationList;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelNotifications;
        private System.Windows.Forms.Timer timerNotifications;
        // private System.Windows.Forms.ToolTip toolTipNotifications; // Khai báo nếu dùng ToolTip
    }
}
