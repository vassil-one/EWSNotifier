using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EWSNotifier.UI
{
    public interface ILoadingAware
    {
        event LoadingEventHandler LoadingBegin;
        event LoadingEventHandler LoadingEnd;
    }

    public delegate void LoadingEventHandler(object sender, LoadingEventArgs e);

    public class LoadingEventArgs : EventArgs
    {
        public bool LoadSuccessful { get; set; }
        public object Payload { get; set; }
    }
    
}
