using ImageService.Modal;
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
        private static readonly string[] m_filters = { ".jpg", ".png", ".gif", ".bmp" }; // file extensions to monitor.
        #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed


        /// <summary>
        /// C'tor.
        /// </summary>
        /// <param name="imageController">an image controller.</param>
        /// <param name="logging">a logging service.</param>
        public DirectoyHandler(IImageController imageController, ILoggingService logging)
        {
            m_controller = imageController;
            m_logging = logging;
        }

        /// <summary>
        /// handles the command, closes the handler or hands off the command to the controller.
        /// </summary>
        /// <param name="sender">object that trigerred the event.</param>
        /// <param name="e">arguments of the event.</param>
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if (e.RequestDirPath == "*" || e.RequestDirPath == m_path)
            {
                if (e.CommandID == (int)CommandEnum.CloseCommand)
                {
                    m_dirWatcher.EnableRaisingEvents = false;
                    m_dirWatcher.Dispose();
                    DirectoryClose?.Invoke(this, new DirectoryCloseEventArgs(m_path, $"Directory {m_path} closed successfully."));
                } else
                {
                    bool result;
                    string msg = m_controller.ExecuteCommand(e.CommandID, e.Args, out result);
                    if(result)
                    {
                        m_logging.Log(msg, MessageTypeEnum.INFO);
                    } else
                    {
                        m_logging.Log(msg, MessageTypeEnum.WARNING);
                    }
                }
            }
        }

        /// <summary>
        /// Starts monitoring the directory given.
        /// </summary>
        /// <param name="dirPath">a path to monitor.</param>
        public void StartHandleDirectory(string dirPath)
        {
            // set the path to handle.
            m_path = dirPath;
            m_dirWatcher = new FileSystemWatcher
            {
                // set FileSystemWatcher Properties.
                Path = m_path,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                        | NotifyFilters.DirectoryName
            };
            m_dirWatcher.Created += new FileSystemEventHandler(OnCreated);
            m_dirWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Moves the file created to the output directory.
        /// </summary>
        /// <param name="source">invoker object.</param>
        /// <param name="e">arguments of the event.</param>
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            string fileName = Path.GetFileName(e.FullPath);
            if (IsAMonitoredExtension(e.FullPath))
            {
                bool result;
                string[] args = { e.FullPath };
                string res = m_controller.ExecuteCommand( (int)CommandEnum.NewFileCommand, args, out result);
                if (result)
                {
                    string msg = $"The file {fileName} was added successfully to {res}.";
                    m_logging.Log(msg, MessageTypeEnum.INFO);
                } else
                {
                    string msg = $"File {fileName} addition has failed. {res}";
                    m_logging.Log(msg, MessageTypeEnum.FAIL);
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
            foreach (string extension in m_filters)
            {
                if (String.Equals(extension, fileExtension, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
