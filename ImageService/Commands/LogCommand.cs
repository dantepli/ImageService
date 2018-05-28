using ImageService.Infrastructure.Commands;
using ImageService.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ImageService.Commands
{
    class LogCommand : ICommand
    {
        private ILoggingService m_logging;

        /// <summary>
        /// C'tor.
        /// </summary>
        /// <param name="logging">a logging service.</param>
        public LogCommand(ILoggingService logging)
        {
            m_logging = logging;
        }

        /// <summary>
        /// Execute the log command.
        /// </summary>
        /// <param name="args">args to command.</param>
        /// <param name="result">return true if successful.</param>
        /// <returns>the json representation of the logs.</returns>
        public string Execute(string[] args, out bool result)
        {
            string json = JsonConvert.SerializeObject(m_logging.LogRecords);
            result = true;
            return json;
        }
    }
}
