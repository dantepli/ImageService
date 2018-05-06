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

        public string OutputDir { get; set; }

        public string SourceName { get; set; }

        public string LogName { get; set; }

        public int ThumbnailSize { get; set; }

        private ObservableCollection<DirectoryPath> m_ModelDirPaths;

        public ObservableCollection<DirectoryPath> ModelDirPaths
        {
            get { return m_ModelDirPaths; }
            set
            {
                m_ModelDirPaths = value;
                NotifyPropertyChanged("ModelDirPaths");
            }
        }

        public SettingsModel()
        {
            m_ModelDirPaths = new ObservableCollection<DirectoryPath>();
            
            SingletonClient.Instance.DataRecived += OnDataRecived;

            string[] args = { };
            CommandMessage message = new CommandMessage() { CommandID = (int)CommandEnum.GetConfigCommand, CommandArgs = args };
            SingletonClient.Instance.SendCommand(message);

            m_actions = new Dictionary<int, CommandAction>()
            {
                { (int)CommandEnum.GetConfigCommand, OnConfigRecived },
                { (int)CommandEnum.CloseCommand, OnRemoveHandler }
            };
        }

        public void OnDataRecived(object sender, DataReceivedEventArgs e)
        {
            CommandMessage message = CommandMessage.FromJSON(e.Data);

            if(m_actions.ContainsKey(message.CommandID))
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
                m_ModelDirPaths.Add(new DirectoryPath() { DirPath = handler });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
            foreach (DirectoryPath path in m_ModelDirPaths)
            {
                if (path.DirPath == message.CommandArgs[0])
                {
                    m_ModelDirPaths.Remove(path);
                    break;
                }
            }
        }
    }
}
