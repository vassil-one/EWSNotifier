using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EWSNotifier.Model;

namespace EWSNotifier.Logging
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
        /// Log the given message to the local logging database and any registered ILoggers.
        /// </summary>
        public static void Log(string message)
        {
            LogToDatabase(message);

            if (_loggers == null)
                return;

            foreach (ILogger logger in _loggers)
                logger.Log(message);
        }

        /// <summary>
        /// Insert message into the EventLog table in the local database
        /// </summary>
        public static void LogToDatabase(string message)
        {
            LogEntities dataContext = new LogEntities();
            EventLog log = EventLog.CreateEventLog(Guid.NewGuid(), DateTime.Now, message);
            dataContext.EventLogs.AddObject(log);
            dataContext.SaveChanges();
        }

    }
}
