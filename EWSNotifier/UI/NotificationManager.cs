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

namespace EWSNotifier
{
    public class NotificationManager : IDisposable, ILoadingAware
    {
        public EWSManager EWSManager;
        private NotifyIcon _notifyIcon;
        private SettingsForm _settingsForm;
        private BaseFolderType[] _folders;
        private Timer _checkMailTimer;
        private BackgroundWorker _checkMailBackgroundWorker;

        public Subscription Subscription { get; set; }
        public bool HasNewMail { get; set; }

        public NotificationManager(NotifyIcon notifyIcon)
        {
            _notifyIcon = notifyIcon;
            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripButton("Settings", null, settingsItem_Click));
            _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripButton("&Exit", null, exitItem_Click));
            _notifyIcon.Click += notifyIcon_Click;
            _notifyIcon.DoubleClick += settingsItem_Click;
            _notifyIcon.Icon = new Icon(Settings.Default.DefaultIconFilename);
            _notifyIcon.Text = Settings.Default.DefaultTooltipText;
            _notifyIcon.BalloonTipTitle = "New Mail";
            _notifyIcon.BalloonTipText = "New Mail Found";
            _notifyIcon.Visible = true;

            _checkMailTimer = new Timer();
            _checkMailTimer.Interval = 5000;
            _checkMailTimer.Tick += checkMailTimer_Tick;

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

        #region "Checking Mail"

        public void SetupSubscription(BaseFolderType[] folders)
        {
            OnLoadingBegin(new LoadingEventArgs());

            CloseSubscription();
            _folders = folders;

            SubscribeResponseMessageType response = this.EWSManager.Subscribe(folders);
            Subscription sub = new Subscription();
            sub.SubscriptionId = response.SubscriptionId;
            sub.Watermark = response.Watermark;
            this.Subscription = sub;

            _checkMailTimer.Start();
            Logger.Log(String.Format("Timer started. Checking for mail every {0} seconds",
                                      _checkMailTimer.Interval / 1000));

            OnLoadingEnd(new LoadingEventArgs() { LoadSuccessful = true });
        }

        private void CloseSubscription()
        {
            if (this.Subscription != null)
                this.EWSManager.Unsubscribe(this.Subscription);
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
                _notifyIcon.BalloonTipText = "Check for mail failed!";
                _notifyIcon.ShowBalloonTip(30000);
            }
            else if(e.Result != null)
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
            _notifyIcon.BalloonTipText = balloonTipMsg;
            _notifyIcon.ShowBalloonTip(30000);
        }

        #endregion

        #region "NotifyIcon Event Handlers"

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            HasNewMail = false;
            _notifyIcon.Icon = new System.Drawing.Icon(Settings.Default.DefaultIconFilename);
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

        #region "IDisposable"
        public void Dispose()
        {
            _settingsForm.Close();
            _notifyIcon.Visible = false; // should remove lingering tray icon!
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

    }
}
