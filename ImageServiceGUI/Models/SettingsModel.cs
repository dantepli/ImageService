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

namespace ImageServiceGUI.Models
{
    class SettingsModel : ISettingsModel
    {
        delegate void CommandAction(string message);
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

            SingletonClient.Instance.DirectoryPathRemoved += OnRemoveHandler;
            SingletonClient.Instance.DataRecived += OnDataRecived;
            string[] args = { };
            bool result;
            string properties = SingletonClient.Instance.ExecuteCommand(CommandEnum.GetConfigCommand, args, out result);
            if (!String.IsNullOrEmpty(properties))
            {
                FromJSON(properties);
            }

            m_actions = new Dictionary<int, CommandAction>()
            {
                { (int)CommandEnum.GetConfigCommand, OnConfigRecived }
            };
        }

        public void OnConfigRecived(string message)
        {

        }

        public void FromJSON(string properties)
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

            // NotifyPropertyChanged("ModelDirPaths");
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
            bool result;
            string success = SingletonClient.Instance.ExecuteCommand(CommandEnum.CloseCommand, args, out result);
        }

        private void OnRemoveHandler(object sender, DirectoryPathRemovedEventArgs e)
        {
            foreach (DirectoryPath path in m_ModelDirPaths)
            {
                if(path.DirPath == e.Path)
                {
                    m_ModelDirPaths.Remove(path);
                    break;
                }
            }
        }
    }
}
