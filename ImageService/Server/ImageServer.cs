using ImageService.Commands;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion

       
        public ImageServer(ILoggingService log, IImageController controller)
        {

            m_controller = controller;

            m_logging = log;
        }

        /// <summary>
        /// Creates handler.
        /// </summary>
        /// <param name="dir_path">path to directory.</param>
        public void CreateHandler(string dir_path)
        {
            IDirectoryHandler h = new DirectoyHandler(m_controller, m_logging);
            h.DirectoryClose += OnCloseServer;
            CommandRecieved += h.OnCommandRecieved;
            h.StartHandleDirectory(dir_path);
        }

        /// <summary>
        /// Handler notifies server it is closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseServer(object sender, DirectoryCloseEventArgs e)
        {
            IDirectoryHandler h = (DirectoyHandler) sender;
            CommandRecieved -= h.OnCommandRecieved;
            m_logging.Log(e.Message, MessageTypeEnum.INFO);
            // TODO do we need onCommand -= h.OnCloseServer;
        }

        /// <summary>
        /// Close server, notifies handlers.
        /// </summary>
        public void CloseServer()
        {
            // invoke the event - send a message to all handlers(*)
            CommandRecieved?.Invoke(this, new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, "*"));
        }
    }
}
