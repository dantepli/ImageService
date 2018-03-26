﻿using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using System.Text.RegularExpressions;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        private string[] m_filters = { "*.jpg", "*.png", "*.gif", "*.bmp" }; // file extensions to monitor.
        #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed

        public DirectoyHandler(IImageController imageController, ILoggingService logging)
        {
            m_controller = imageController;
            m_logging = logging;
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            string fileName = Path.GetFileName(e.FullPath);
            if (IsAMonitoredExtension(e.FullPath))
            {
                bool result;
                string[] args = { e.FullPath };
                m_controller.ExecuteCommand( (int)CommandEnum.NewFileCommand, args, out result);
                if (result)
                {
                    string msg = $"File {fileName} addition was successful.";
                    m_logging.Log(msg, Logging.Modal.MessageTypeEnum.INFO);
                } else
                {
                    string msg = $"File {fileName} addition has failed.";
                    m_logging.Log(msg, Logging.Modal.MessageTypeEnum.FAIL);
                }
            }
        }

        /// <summary>
        /// Checks if the extension of a file path is a monitored extension.
        /// </summary>
        /// <param name="filePath">a file path to check for.</param>
        /// <returns>true if monitored.</returns>
        private bool IsAMonitoredExtension(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath);
            foreach(string extension in m_filters) {
                if(extension == fileExtension)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if(e.RequestDirPath == m_path)
            {
                bool result;
                m_controller.ExecuteCommand(e.CommandID, e.Args, out result);
                if(result)
                {
                   // Log?
                }
            }
        }

        public void StartHandleDirectory(string dirPath)
        {
            // set the path to handle.
            m_path = dirPath;
            m_dirWatcher = new FileSystemWatcher();
            // set FileSystemWatcher Properties.
            m_dirWatcher.Path = m_path;
            m_dirWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                        | NotifyFilters.DirectoryName;
            m_dirWatcher.Created += new FileSystemEventHandler(OnCreated);
            m_dirWatcher.EnableRaisingEvents = true;
        }
    }
}