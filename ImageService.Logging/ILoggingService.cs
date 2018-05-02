using ImageService.Logging.Modal;
using ImageService.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging
{
    public interface ILoggingService
    {
        event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        /// <summary>
        /// Logs the message to the event logger.
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="type">type of the message.</param>
        void Log(string message, MessageTypeEnum type);           // Logging the Message
    }
}
