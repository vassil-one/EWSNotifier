using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EWSNotifier.Logging;

namespace EWSNotifier.UI
{
    public partial class LogBox : UserControl, ILogger
    {
        private List<LogBoxItem> _logBoxItems;

        public LogBox()
        {
            InitializeComponent();
            _logBoxItems = new List<LogBoxItem>();
            bindingSource1.DataSource = _logBoxItems;
        }

        public void Log(string message)
        {
            LogBoxItem eventMessage = new LogBoxItem(message);
            bindingSource1.Add(eventMessage);
        }
    }

    public class LogBoxItem
    {
        public DateTime EventDate { get; set; }
        public string EventMessage { get; set; }

        public LogBoxItem(string eventMessage)
        {
            this.EventDate = DateTime.Now;
            this.EventMessage = eventMessage;
        }
    }
}
