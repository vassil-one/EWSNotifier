using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EWSNotifier.Logging
{
    public interface ILogger
    {
        void Log(string message);
    }
}
