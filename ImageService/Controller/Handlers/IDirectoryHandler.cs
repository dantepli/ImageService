using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller.Handlers
{
    public interface IDirectoryHandler
    {
        event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed
         /// <summary>
        /// Starts monitoring the directory given.
        /// </summary>
        /// <param name="dirPath">a path to monitor.</param>
        void StartHandleDirectory(string dirPath);             // The Function Recieves the directory to Handle
        /// <summary>
        /// handles the command, closes the handler or hands off the command to the controller.
        /// </summary>
        /// <param name="sender">object that trigerred the event.</param>
        /// <param name="e">arguments of the event.</param>
        void OnCommandRecieved(object sender, CommandRecievedEventArgs e);     // The Event that will be activated upon new Command
    }
}
