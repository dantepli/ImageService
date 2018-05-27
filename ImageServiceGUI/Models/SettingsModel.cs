using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure.Objects;
using ImageService.Infrastructure.Enums;
using Newtonsoft.Json.Linq;
using ImageService.Infrastructure.Commands;
using ImageService.Infrastructure;
using ImageService.Communication.Client;
using ImageService.Communication.Events;
using System.Windows;

namespace ImageServiceGUI.Models
{
    class SettingsModel : ISettingsModel
    {
        delegate void CommandAction(CommandMessage message);
        private Dictionary<int, CommandAction> m_actions;

        // settings of service
        private string m_outputDir;
        private string m_sourceName;
        private string m_logName;
        private int m_thumbnailSize;

        public event PropertyChangedEventHandler PropertyChanged;
        public string OutputDir
        {
            get
            {
                return m_outputDir;
            }
            set
            {
                m_outputDir = value;
                NotifyPropertyChanged("OutputDir");
            }
        }

        public string SourceName
        {
            get
            {
                return m_sourceName;
            }
            set
            {
                m_sourceName = value;
                NotifyPropertyChanged("SourceName");
            }
        }

        public string LogName
        {
            get
            {
                return m_logName;
            }
            set
            {
                m_logName = value;
                NotifyPropertyChanged("LogName");
            }
        }

        public int ThumbnailSize
        {
            get
            {
                return m_thumbnailSize;
            }
            set
            {
                m_thumbnailSize = value;
                NotifyPropertyChanged("ThumbnailSize");
            }
        }

        public ObservableCollection<DirectoryPath> DirectoryPaths { get; set; }

        /// <summary>
        /// c'tor
        /// </summary>
        public SettingsModel()
        {
            DirectoryPaths = new ObservableCollection<DirectoryPath>();

            SingletonClient.Instance.DataRecieved += OnDataRecieved;

            string[] args = { };
            CommandMessage message = new CommandMessage() { CommandID = (int)CommandEnum.GetConfigCommand, CommandArgs = args };
            SingletonClient.Instance.SendCommand(message);

            m_actions = new Dictionary<int, CommandAction>()
            {
                { (int)CommandEnum.GetConfigCommand, OnConfigRecived },
                { (int)CommandEnum.CloseCommand, OnRemoveHandler }
            };
        }

        /// <summary>
        /// handle data recived from server
        /// </summary>
        /// <param name="sender">sender of data</param>
        /// <param name="e">args to represent data</param>
        public void OnDataRecieved(object sender, DataReceivedEventArgs e)
        {
            CommandMessage message = CommandMessage.FromJSON(e.Data);

            if (m_actions.ContainsKey(message.CommandID))
            {
                m_actions[message.CommandID](message);
            }
        }

        /// <summary>
        /// action to do once config had been recived
        /// </summary>
        /// <param name="message">message to represet handler's path</param>
        private void OnConfigRecived(CommandMessage message)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                InterpretProperties(message.CommandArgs[0]);
            }));
        }

        /// <summary>
        /// interpret properties
        /// </summary>
        /// <param name="logs">JSON to represent properties</param>
        public void InterpretProperties(string properties)
        {
            JObject appConfigObj = JObject.Parse(properties);


            OutputDir = (string)appConfigObj["OutputDir"];
            SourceName = (string)appConfigObj["SourceName"];
            LogName = (string)appConfigObj["LogName"];
            ThumbnailSize = (int)appConfigObj["ThumbnailSize"];

            string allHandlers = (string)appConfigObj["Handler"];
            string[] handlers = allHandlers.Split(new char[] { Consts.DELIM }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string handler in handlers)
            {
                DirectoryPaths.Add(new DirectoryPath() { DirPath = handler });
            }
        }

        /// <summary>
        /// notify listeners
        /// </summary>
        /// <param name="name">proproty that had changed</param>
        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// remove handler from service
        /// </summary>
        /// <param name="rmPath">path of handler</param>
        public void RemoveHandler(DirectoryPath rmPath)
        {
            string[] args = { rmPath.DirPath };
            CommandMessage message = new CommandMessage() { CommandID = (int)CommandEnum.CloseCommand, CommandArgs = args };
            SingletonClient.Instance.SendCommand(message);
        }

        /// <summary>
        /// action to do once a handler had been removed
        /// </summary>
        /// <param name="message">message to represet handler's path</param>
        private void OnRemoveHandler(CommandMessage message)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                foreach (DirectoryPath path in DirectoryPaths)
                {
                    if (path.DirPath == message.CommandArgs[0])
                    {
                        DirectoryPaths.Remove(path);
                        break;
                    }
                }
            }));
        }
    }
}
