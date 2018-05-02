using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageServiceGUI.Models;
using System.Collections.ObjectModel;

namespace ImageServiceGUI.ViewModels
{
    class SettingsViewModel : ViewModel
    {
        private ISettingsModel model;
        private ObservableCollection<DirectoryPath> m_directoryPaths;

        public ObservableCollection<DirectoryPath> DirectoryPaths
        {
            get { return m_directoryPaths; }
            set
            {
                m_directoryPaths = value;
                NotifyPropertyChanged("DirectoryPaths");
            }
        }

        public SettingsViewModel()
        {
            // TODO assign model
            this.model = null;

            m_directoryPaths = new ObservableCollection<DirectoryPath>();
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "I" });
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "AM" });
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "THE" });
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "LORD" });
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "OF" });
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "BINDING" });
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "BOW" });
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "BEFORE" });
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "ME" });

        }

        public string OutputDir
        {
            get { return "output"; }
        }

        public string SourceName
        {
            get { return "source"; }
        }

        public string LogName
        {
            get { return "log"; }
        }

        public int ThumbnailSize
        {
            get { return 120; }
        }
    }
}
