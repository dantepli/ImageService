using ImageService.Commands;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Infrastructure.Objects;
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
        private ITcpServer m_tcpServer;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion


        public ImageServer(ILoggingService log, IImageController controller)
        {

            m_controller = controller;

            m_controller.AddCloseCommand(this);

            m_logging = log;

            m_tcpServer = new TcpServer(8000, new ClientHandler(controller));

            // starts the tcp server in a seperate thread.
            new Task(() => { m_tcpServer.Start(); }).Start();
        }

        /// <summary>
        /// Creates handler.
        /// </summary>
        /// <param name="dir_path">path to directory.</param>
        public void CreateHandler(string dir_path)
        {
            if (!System.IO.Directory.Exists(dir_path))
            {
                m_logging.Log($"Failed creating a handler for the following directory: {dir_path}." +
                    $" Reason: Directory doesn't exist.", MessageTypeEnum.FAIL);
                return;
            }
            IDirectoryHandler h = new DirectoyHandler(m_controller, m_logging);
            h.DirectoryClose += OnCloseServer;
            CommandRecieved += h.OnCommandRecieved;
            h.StartHandleDirectory(dir_path);
            m_logging.Log($"Handler created for the following directory: {dir_path}.", MessageTypeEnum.INFO);
        }

        /// <summary>
        /// Handler notifies server it is closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseServer(object sender, DirectoryCloseEventArgs e)
        {
            IDirectoryHandler h = (DirectoyHandler)sender;
            CommandRecieved -= h.OnCommandRecieved;
            m_logging.Log(e.Message, MessageTypeEnum.INFO);
            m_tcpServer.SendToAll(e.DirectoryPath);
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

        /// <summary>
        /// Closes a handler with the specified path.
        /// </summary>
        /// <param name="path">Path of the handler to close.</param>
        public void CloseHandler(string path)
        {
            CommandRecieved?.Invoke(this, new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, path));
        }
    }
}
