namespace EWSNotifier
{
    partial class SettingsForm
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
            this.lblConnectionStatus = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.folderView = new EWSNotifier.UI.FolderView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.logBox = new EWSNotifier.UI.LogBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtEWSUrl = new System.Windows.Forms.TextBox();
            this.btnWatchFolders = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblConnectionStatus
            // 
            this.lblConnectionStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblConnectionStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblConnectionStatus.Location = new System.Drawing.Point(0, 559);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(646, 13);
            this.lblConnectionStatus.TabIndex = 12;
            this.lblConnectionStatus.Text = "Not Connected";
            this.lblConnectionStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnWatchFolders);
            this.splitContainer1.Panel1.Controls.Add(this.folderView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(646, 559);
            this.splitContainer1.SplitterDistance = 215;
            this.splitContainer1.TabIndex = 13;
            // 
            // folderView
            // 
            this.folderView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.folderView.Location = new System.Drawing.Point(0, 36);
            this.folderView.Name = "folderView";
            this.folderView.Size = new System.Drawing.Size(215, 523);
            this.folderView.TabIndex = 0;
            this.folderView.LoadingBegin += new EWSNotifier.UI.LoadingEventHandler(this.folderView_LoadingBegin);
            this.folderView.LoadingEnd += new EWSNotifier.UI.LoadingEventHandler(this.folderView_LoadingEnd);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.logBox);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(427, 559);
            this.panel1.TabIndex = 0;
            // 
            // logBox
            // 
            this.logBox.Location = new System.Drawing.Point(0, 92);
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(424, 464);
            this.logBox.TabIndex = 24;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtUsername);
            this.panel2.Controls.Add(this.lblUsername);
            this.panel2.Controls.Add(this.txtDomain);
            this.panel2.Controls.Add(this.btnConnect);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.txtPassword);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txtEWSUrl);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(427, 86);
            this.panel2.TabIndex = 23;
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(3, 21);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(100, 20);
            this.txtUsername.TabIndex = 16;
            this.txtUsername.Text = "jstrawther";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(0, 5);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(55, 13);
            this.lblUsername.TabIndex = 15;
            this.lblUsername.Text = "Username";
            // 
            // txtDomain
            // 
            this.txtDomain.Location = new System.Drawing.Point(215, 21);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.Size = new System.Drawing.Size(100, 20);
            this.txtDomain.TabIndex = 20;
            this.txtDomain.Text = "RESDAT";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(320, 58);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 23);
            this.btnConnect.TabIndex = 14;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(106, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "EWS URL";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(109, 21);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(212, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Domain";
            // 
            // txtEWSUrl
            // 
            this.txtEWSUrl.Location = new System.Drawing.Point(3, 60);
            this.txtEWSUrl.Name = "txtEWSUrl";
            this.txtEWSUrl.Size = new System.Drawing.Size(312, 20);
            this.txtEWSUrl.TabIndex = 21;
            this.txtEWSUrl.Text = "https://email.resdat.com/ews/exchange.asmx";
            // 
            // btnWatchFolders
            // 
            this.btnWatchFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWatchFolders.Location = new System.Drawing.Point(77, 12);
            this.btnWatchFolders.Name = "btnWatchFolders";
            this.btnWatchFolders.Size = new System.Drawing.Size(136, 23);
            this.btnWatchFolders.TabIndex = 23;
            this.btnWatchFolders.Text = "Watch Selected Folders";
            this.btnWatchFolders.UseVisualStyleBackColor = true;
            this.btnWatchFolders.Click += new System.EventHandler(this.btnWatchFolders_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(646, 572);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.lblConnectionStatus);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SettingsForm";
            this.Text = "EWS Notifier";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblConnectionStatus;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtEWSUrl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.Panel panel2;
        private UI.FolderView folderView;
        private UI.LogBox logBox;
        private System.Windows.Forms.Button btnWatchFolders;

    }
}

