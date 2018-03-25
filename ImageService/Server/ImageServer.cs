﻿using ImageService.Controller;
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
            IImageServiceModal imageModal = new ImageServiceModal(System.Configuration.ConfigurationSettings.AppSettings["OutputDir"],
               Int32.Parse(System.Configuration.ConfigurationSettings.AppSettings["ThumbnailSize"]));

            m_controller = new ImageController(imageModal);

            m_logging = new LoggingService();
        }

        public void createHandler(string dir_path)
        {
            IDirectoryHandler h = new DirectoyHandler(dir_path, m_controller);
            h.DirectoryClose += onCloseServer;
            CommandRecieved += h.OnCommandRecieved;
        }

        private void onCloseServer(object sender, DirectoryCloseEventArgs e)
        {

        }

        public void onCommand()
        {
            // onCommand("*", onCloseServer);
        }
    }
}
