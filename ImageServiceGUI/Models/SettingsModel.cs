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

namespace ImageServiceGUI.Models
{
    class SettingsModel : ISettingsModel
    {
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

            string[] args = { };
            bool result;
            string properties = SingletonClient.Instance.ExecuteCommand(CommandEnum.GetConfigCommand, args, out result);
            if (!String.IsNullOrEmpty(properties))
            {
                FromJSON(properties);
            }
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

        public bool RemoveHandler(DirectoryPath rmPath)
        {
            string[] args = { rmPath.DirPath };
            bool result;
            string success = SingletonClient.Instance.ExecuteCommand(CommandEnum.CloseCommand, args, out result);

            return result;
        }
    }
}
