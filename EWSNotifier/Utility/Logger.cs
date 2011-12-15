using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace EWSNotifier.Utility
{
    /// <summary>
    /// Logging utility
    /// </summary>
    public class Logger
    {
        private static List<ILogger> _loggers;
        /// <summary>
        /// Register an ILogger instance to receive log messages when Log method is called.
        /// </summary>
        public static void RegisterLogger(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("ILogger cannot be null");
            }
            if (_loggers == null)
            {
                _loggers = new List<ILogger>();
            }
            _loggers.Add(logger);
        }

        /// <summary>
        /// Log the given message to the system EventLog and any registered ILoggers.
        /// </summary>
        public static void Log(string message)
        {
            Logger.LogWithEventLog(message);
            if (_loggers == null)
                return;
            foreach (ILogger logger in _loggers)
                logger.Log(message);
        }

        /// <summary>
        /// Log the given message to the EventLog. Does not log to any registered ILoggers
        /// </summary>
        public static void LogWithEventLog(string message)
        {
            // Create an EventLog instance and assign its source.
            //EventLog myLog = new EventLog();
            //myLog.WriteEntry(message);
        }

    }
}
