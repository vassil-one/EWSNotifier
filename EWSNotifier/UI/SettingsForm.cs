using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EWSNotifier.ewswebreference;
using System.Net;
using EWSNotifier.UI;
using EWSNotifier.Utility;

namespace EWSNotifier
{
    public partial class SettingsForm : Form
    {
        public EWSManager EWSManager { get; set; }
        public NotificationManager NotificationManager { get; set; }

        public SettingsForm(NotificationManager notificationManager)
        {
            InitializeComponent();
            this.NotificationManager = notificationManager;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Logger.RegisterLogger(this.logBox);
            Logger.Log("Settings Form Loaded");
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string domain = txtDomain.Text;
            string ewsUrl = txtEWSUrl.Text;
            this.EWSManager = new EWSManager(username, password, domain, ewsUrl);

            folderView.LoadFolders(this.EWSManager);
        }

        protected void folderView_LoadingBegin(object sender, LoadingEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
        }

        protected void folderView_LoadingEnd(object sender, LoadingEventArgs e)
        {
            this.Cursor = Cursors.Default;
            folderView.Cursor = Cursors.Default;

            if (e.LoadSuccessful)
            {
                lblConnectionStatus.Text = "Folders Successfully Loaded";
            }
            else
            {
                lblConnectionStatus.Text = "Not Connected";
            }
        }

        private void btnWatchFolders_Click(object sender, EventArgs e)
        {
            BaseFolderType[] folders = folderView.CheckedFolders.ToArray();
            this.NotificationManager.EWSManager = this.EWSManager;
            NotificationManager.SetupSubscription(folders);
        }

    }
}
