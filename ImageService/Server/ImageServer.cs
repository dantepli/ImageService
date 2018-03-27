using ImageService.Commands;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
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

       
        public ImageServer()
        {
            IImageServiceModal imageModal = new ImageServiceModal(
                System.Configuration.ConfigurationSettings.AppSettings["OutputDir"],
                Int32.Parse(System.Configuration.ConfigurationSettings.AppSettings["ThumbnailSize"]));

            m_controller = new ImageController(imageModal);

            m_logging = new LoggingService();
        }

        public void CreateHandler(string dir_path)
        {
            IDirectoryHandler h = new DirectoyHandler(m_controller, m_logging);
            h.DirectoryClose += OnCloseServer;
            CommandRecieved += h.OnCommandRecieved;
            h.StartHandleDirectory(dir_path);
        }

        private void OnCloseServer(object sender, DirectoryCloseEventArgs e)
        {
            IDirectoryHandler h = (DirectoyHandler) sender;
            CommandRecieved -= h.OnCommandRecieved;
            // TODO do we need onCommand -= h.OnCloseServer;
        }

        public void SendCommand()
        {
            // invoke the event - send a message to all handlers(*)
            CommandRecieved?.Invoke(this, new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, "*"));
        }
    }
}
