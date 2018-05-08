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
using ImageServiceGUI.Communication;
using ImageService.Infrastructure.Commands;
using ImageServiceGUI.Models.Events;

namespace ImageServiceGUI.Models
{
    class SettingsModel : ISettingsModel
    {
        delegate void CommandAction(CommandMessage message);
        private Dictionary<int, CommandAction> m_actions;

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

        public string SourceName {
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

        public string LogName {
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

        public int ThumbnailSize {
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

        public void OnDataRecieved(object sender, DataReceivedEventArgs e)
        {
            CommandMessage message = CommandMessage.FromJSON(e.Data);

            if (m_actions.ContainsKey(message.CommandID))
            {
                m_actions[message.CommandID](message);
            }
        }

        private void OnConfigRecived(CommandMessage message)
        {
            InterpretProperties(message.CommandArgs[0]);
        }

        public void InterpretProperties(string properties)
        {
            JObject appConfigObj = JObject.Parse(properties);


            OutputDir = (string)appConfigObj["OutputDir"];
            SourceName = (string)appConfigObj["SourceName"];
            LogName = (string)appConfigObj["LogName"];
            ThumbnailSize = (int)appConfigObj["ThumbnailSize"];

            string allHandlers = (string)appConfigObj["Handler"];
            string[] handlers = allHandlers.Split(';');

            foreach (string handler in handlers)
            {
                DirectoryPaths.Add(new DirectoryPath() { DirPath = handler });
            }
        }


        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        public void RemoveHandler(DirectoryPath rmPath)
        {
            string[] args = { rmPath.DirPath };
            CommandMessage message = new CommandMessage() { CommandID = (int)CommandEnum.CloseCommand, CommandArgs = args };
            SingletonClient.Instance.SendCommand(message);
        }

        private void OnRemoveHandler(CommandMessage message)
        {
            foreach (DirectoryPath path in DirectoryPaths)
            {
                if (path.DirPath == message.CommandArgs[0])
                {
                    DirectoryPaths.Remove(path);
                    break;
                }
            }
        }
    }
}
