using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure.Objects;

namespace ImageServiceGUI.Models
{
    class SettingsModel : ISettingsModel
    {
        public string OutputDir{ get; set; }

        public string SourceName{ get; set; }

        public string LogName{ get; set; }

        public int ThumbnailSize{ get; set; }

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
            OutputDir = "output";
            SourceName = "source";
            LogName = "log";
            ThumbnailSize = 120;

            m_ModelDirPaths = new ObservableCollection<DirectoryPath>();

            m_ModelDirPaths.Add(new DirectoryPath() { DirPath = "I" });
            m_ModelDirPaths.Add(new DirectoryPath() { DirPath = "AM" });
            m_ModelDirPaths.Add(new DirectoryPath() { DirPath = "THE" });
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
            // TODO ask server to remove handler and get response
            return true;
        }
    }
}
