using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public interface IImageController
    {
        /// <summary>
        /// Execute given command in a new task.
        /// </summary>
        /// <param name="commandID">id of command.</param>
        /// <param name="args">args to given command.</param>
        /// <param name="resultSuccesful">true if successful.</param>
        /// <returns>string representing the result of the command.</returns>
        string ExecuteCommand(int commandID, string[] args, out bool result);          // Executing the Command Requet
    }
}
