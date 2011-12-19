using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EWSNotifier.ewswebreference;
using System.Net;
using System.Windows.Forms;
using EWSNotifier.Properties;
using EWSNotifier.Model;
using EWSNotifier.Utility;
using System.ComponentModel;
using EWSNotifier.UI;
using System.Drawing;
using EWSNotifier.Logging;

namespace EWSNotifier
{
    public class NotificationManager : IDisposable, ILoadingAware
    {
        public EWSManager EWSManager;
        private NotifyIcon _notifyIcon;
        private SettingsForm _settingsForm;
        private BaseFolderType[] _folders;
        private Timer _checkMailTimer, _retryTimer;
        private BackgroundWorker _checkMailBackgroundWorker;

        public Subscription Subscription { get; set; }

        private bool _hasNewMail;
        public bool HasNewMail 
        {
            get 
            {
                return _hasNewMail;
            }
            set
            {
                _hasNewMail = value;
                if(_hasNewMail)
                    _notifyIcon.Icon = new System.Drawing.Icon(Settings.Default.NewMailIconFilename); 
                else if(IsConnected)
                    _notifyIcon.Icon = new System.Drawing.Icon(Settings.Default.ConnectedIconFilename); 
                else
                    _notifyIcon.Icon = new System.Drawing.Icon(Settings.Default.DisconnectedIconFilename); 
            }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (value == true)
                {
                    _notifyIcon.Icon = new Icon(Settings.Default.ConnectedIconFilename);
                    ShowBalloonNotification("EWS Notifier", "Connected to Exchange Server");
                }
                else
                {
                    _notifyIcon.Icon = new Icon(Settings.Default.DisconnectedIconFilename);
                    if(_isConnected == true)
                        ShowBalloonNotification("EWS Notifier", "Exchange Connection Dropped");
                }
                _isConnected = value;

            }
        }

        public NotificationManager(NotifyIcon notifyIcon)
        {
            _notifyIcon = notifyIcon;
            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripButton("Settings", null, settingsItem_Click));
            _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripButton("&Exit", null, exitItem_Click));
            _notifyIcon.Click += notifyIcon_Click;
            _notifyIcon.DoubleClick += settingsItem_Click;
            _notifyIcon.Text = Settings.Default.DefaultTooltipText;
            _notifyIcon.BalloonTipTitle = "New Mail";
            _notifyIcon.BalloonTipText = "New Mail Found";
            _notifyIcon.Visible = true;

            this.IsConnected = false;

            _checkMailTimer = new Timer();
            _checkMailTimer.Interval = 5000;
            _checkMailTimer.Tick += checkMailTimer_Tick;

            _retryTimer = new Timer();
            _retryTimer.Interval = 10000;
            _retryTimer.Tick += retryTimer_Tick;

            _checkMailBackgroundWorker = new BackgroundWorker();
            _checkMailBackgroundWorker.DoWork += new DoWorkEventHandler(checkMailBackgroundWorker_DoWork);
            _checkMailBackgroundWorker.RunWorkerCompleted += 
                new RunWorkerCompletedEventHandler(checkMailBackgroundWorker_RunWorkerCompleted);

            ShowSettingsForm();
        }

        private void ShowSettingsForm()
        {
            if (_settingsForm == null)
            {
                _settingsForm = new SettingsForm(this);
                _settingsForm.FormClosing += (sender, e) =>
                    { // hide instead of closing to prevent disposing of form.
                        e.Cancel = true;
                        ((Form)sender).Hide();
                    };
            }
            if (!_settingsForm.Visible)
                _settingsForm.Show();
            else if (Form.ActiveForm != _settingsForm)
                _settingsForm.Activate();
        }

        public void ShowBalloonNotification(string title, string text)
        {
            _notifyIcon.BalloonTipTitle = title;
            _notifyIcon.BalloonTipText = text;
            _notifyIcon.ShowBalloonTip(30000);
        }
        #region "Checking Mail"

        public bool SetupSubscription(BaseFolderType[] folders)
        {
            _folders = folders;
            OnLoadingBegin(new LoadingEventArgs());

            bool success = false;
            try
            {
                CloseSubscription();
                SubscribeResponseMessageType response = this.EWSManager.Subscribe(folders);
                this.IsConnected = (response.ResponseClass == ResponseClassType.Success);

                Subscription sub = new Subscription();
                sub.SubscriptionId = response.SubscriptionId;
                sub.Watermark = response.Watermark;
                this.Subscription = sub;
                _checkMailTimer.Start();
                Logger.Log(String.Format("Timer started. Checking for mail every {0} seconds",
                             _checkMailTimer.Interval / 1000));
                success = true;
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to setup folder subscription. Message: " + ex.Message);
                Logger.Log(String.Format("Retrying in {0} seconds", _retryTimer.Interval / 1000));
                this.IsConnected = false;
                _retryTimer.Start();
            }

            OnLoadingEnd(new LoadingEventArgs() { LoadSuccessful = success });
            return success;
        }

        private void CloseSubscription()
        {
            try
            {
                if (this.Subscription != null)
                    this.EWSManager.Unsubscribe(this.Subscription);
            }
            catch { }
        }

        private void checkMailTimer_Tick(object sender, EventArgs e)
        {
            //Logger.Log("Check for mail...");
            _checkMailTimer.Stop();
            _checkMailBackgroundWorker.RunWorkerAsync();
        }

        protected void checkMailBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            GetEventsResponseMessageType response;
            response = this.EWSManager.GetEvents(this.Subscription);

            List<BaseItemIdType> itemIds = new List<BaseItemIdType>();
            for (int i = 0; i < response.Notification.Items.Length; i++)
            {
                BaseNotificationEventType eventType = response.Notification.Items[i];
                this.Subscription.Watermark = eventType.Watermark;
                this.Subscription.SubscriptionId = response.Notification.SubscriptionId;
                ItemsChoiceType eventName = response.Notification.ItemsElementName[i];
                if (eventName != ItemsChoiceType.StatusEvent)
                {
                    BaseObjectChangedEventType objChangedEvent = (BaseObjectChangedEventType)eventType;
                    ItemIdType itemId = (ItemIdType)objChangedEvent.Item;
                    itemIds.Add(itemId);
                }
            }
            if (itemIds.Count > 0)
            {
                List<MessageType> messages = EWSManager.GetMessages(itemIds.ToArray());
                e.Result = messages;
            }
        }

        protected void checkMailBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Logger.Log("Check for mail failed. Message: " + e.Error.Message);
                if (IsConnected)
                    ShowBalloonNotification("Check for mail failed", "Message: " + e.Error.Message);
                _retryTimer.Start();
                return;
            }

            if(e.Result != null)
            {
                List<MessageType> messages = (List<MessageType>)e.Result;
                HandleNewMail(messages);
            }
            _checkMailTimer.Start();
        }

        private void HandleNewMail(List<MessageType> messages)
        {
            HasNewMail = true;
            _notifyIcon.Icon = new System.Drawing.Icon(Settings.Default.NewMailIconFilename);
            string balloonTipMsg = "";
            foreach (MessageType message in messages)
            {
                string from = message.From.Item.Name;
                string folderName = 
                    (from f in _folders where f.FolderId.Id == message.ParentFolderId.Id 
                     select f.DisplayName).FirstOrDefault();

                Logger.Log("New mail from " + from + " in folder " + folderName);
                balloonTipMsg += " * From " + from + " in folder " + folderName + Environment.NewLine;
            }
            ShowBalloonNotification("New Mail", balloonTipMsg);
        }

        private void retryTimer_Tick(object sender, EventArgs e)
        {
            _retryTimer.Stop();
            SetupSubscription(_folders);
        }


        #endregion

        #region "NotifyIcon Event Handlers"

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            HasNewMail = false;
        }

        protected void settingsItem_Click(object sender, EventArgs e)
        {
            ShowSettingsForm();
        }

        protected void exitItem_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
        }

        #endregion

        #region "ILoadingAware"

        public event LoadingEventHandler LoadingBegin;
        protected virtual void OnLoadingBegin(LoadingEventArgs e)
        {
            if (LoadingBegin != null)
                LoadingBegin(this, e);
        }

        public event LoadingEventHandler LoadingEnd;
        protected virtual void OnLoadingEnd(LoadingEventArgs e)
        {
            if (LoadingEnd != null)
                LoadingEnd(this, e);
        }

        #endregion

        #region "IDisposable"
        public void Dispose()
        {
            _settingsForm.Close();
            //_notifyIcon.Visible = false; // should remove lingering tray icon!
        }
        #endregion

    }
}
