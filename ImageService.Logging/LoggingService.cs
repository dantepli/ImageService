
using ImageService.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure.Enums;
using ImageService.Infrastructure.Objects;

namespace ImageService.Logging
{
    public class LoggingService : ILoggingService
    {
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        public ICollection<LogRecord> LogRecords { get; }

        public LoggingService()
        {
            LogRecords = new List<LogRecord>();
        }
        /// <summary>
        /// Logs the message to the event logger.
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="type">type of the message.</param>
        public void Log(string message, MessageTypeEnum type)
        {
            LogRecords.Add(new LogRecord() { Message = message, Type = type });
            MessageRecieved?.Invoke(this, new MessageRecievedEventArgs(message, type));
        }

    }
}
