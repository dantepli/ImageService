using ImageService.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure.Commands;

namespace ImageService.Commands
{
    class CloseCommand : ICommand
    {
        private ImageServer m_server;

        public CloseCommand(ImageServer imageServer)
        {
            m_server = imageServer;
        }

        /// <summary>
        /// Executes the close command.
        /// </summary>
        /// <param name="args">args to command.</param>
        /// <param name="result">return true if successful.</param>
        /// <returns>null.</returns>
        public string Execute(string[] args, out bool result)
        {
            result = true;
            m_server.CloseHandler(args[0]);
            return null;
        }
    }
}
