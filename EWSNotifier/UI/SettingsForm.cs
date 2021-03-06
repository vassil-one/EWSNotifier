﻿using System;
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
using EWSNotifier.Logging;

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
            this.btnWatchFolders.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // populate form fields with saved user settings
            txtUsername.Text = Configuration.EWSUsername;
            txtPassword.Text = Configuration.EWSPassword;
            txtDomain.Text = Configuration.EWSDomain;
            txtServer.Text = Configuration.EWSServer;
            txtEWSUrl.Text = Configuration.EWSURL;

            Logger.RegisterLogger(this.logBox);
            Logger.Log("Settings Form Loaded");

            AttemptAutoConnect();
        }

        private void AttemptAutoConnect()
        {
            if (!ValidateControls(showValidationErrors : false))
                return;

            LoadFolders();
        }

        /// <summary>
        /// Persist user settings back to the configuration file
        /// </summary>
        private void SaveSettings()
        {
            Configuration.EWSUsername = txtUsername.Text;
            Configuration.EWSPassword = txtPassword.Text;
            Configuration.EWSDomain = txtDomain.Text;
            Configuration.EWSURL = txtEWSUrl.Text;
            Configuration.EWSServer = txtServer.Text;
            Configuration.SaveSettings();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!ValidateControls(showValidationErrors : true))
                return;

            LoadFolders();
        }

        private bool ValidateControls(bool showValidationErrors)
        {
            bool success = true; 

            string username = txtUsername.Text;
            if (string.IsNullOrWhiteSpace(username))
            {
                if (showValidationErrors)
                {
                    errorProvider1.SetError(txtUsername, "Required");
                    txtUsername.Focus();
                }
                success = false;
            }
            else
                errorProvider1.SetError(txtUsername, string.Empty);

            string password = txtPassword.Text;
            if (string.IsNullOrWhiteSpace(password))
            {
                if (showValidationErrors)
                {
                    errorProvider1.SetError(txtPassword, "Required");
                    txtPassword.Focus();
                }
                success = false;
            }
            else
                errorProvider1.SetError(txtPassword, string.Empty);

            string domain = txtDomain.Text;
            if (string.IsNullOrWhiteSpace(domain))
            {
                if (showValidationErrors)
                {
                    errorProvider1.SetError(txtDomain, "Required");
                    txtDomain.Focus();
                }
                success = false;
            }
            else
                errorProvider1.SetError(txtDomain, string.Empty);

            string ewsUrl = txtEWSUrl.Text;
            if (string.IsNullOrWhiteSpace(ewsUrl))
            {
                if (showValidationErrors)
                {
                    errorProvider1.SetError(txtEWSUrl, "Required. Either enter a value in the Server field or edit the URL directly by clicking Edit.");
                    txtServer.Focus();
                }
                success = false;
            }
            else
                errorProvider1.SetError(txtEWSUrl, string.Empty);

            return success;
        }

        private void LoadFolders()
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string domain = txtDomain.Text;
            string ewsUrl = txtEWSUrl.Text;
            btnConnect.Enabled = false;
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
                btnWatchFolders.Enabled = true;
                SaveSettings();
                btnWatchFolders.PerformClick();
            }
            else
            {
                lblConnectionStatus.Text = "Loading Folders Failed";
            }
            btnConnect.Enabled = true;
        }

        private void btnWatchFolders_Click(object sender, EventArgs e)
        {
            BaseFolderType[] folders = folderView.CheckedFolders.ToArray();
            if (folders.Length <= 0)
                return;

            this.NotificationManager.EWSManager = this.EWSManager;
            NotificationManager.SetupSubscription(folders);
        }

        private void txtServer_TextChanged(object sender, EventArgs e)
        {
            // Update EWSURL textbox with URL based on server value
            string url = Configuration.BuildEWSURL(txtServer.Text);
            txtEWSUrl.Text = url;
        }

        private void lnkEditURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtEWSUrl.Enabled = true;
            txtEWSUrl.Focus();
        }

        private void txtEWSUrl_Leave(object sender, EventArgs e)
        {
            txtEWSUrl.Enabled = false;
        }

    }
}
