
using ImageService.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging
{
    public class LoggingService : ILoggingService
    {
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        /// <summary>
        /// Logs the message to the event logger.
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="type">type of the message.</param>
        public void Log(string message, MessageTypeEnum type)
        {
            MessageRecieved?.Invoke(this, new MessageRecievedEventArgs(message, type));
        }
    }
}
