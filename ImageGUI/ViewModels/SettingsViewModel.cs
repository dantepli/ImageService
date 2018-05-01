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

        public SettingsViewModel()
        {
            // TODO assign model
            this.model = null;

            m_directoryPaths = new ObservableCollection<DirectoryPath>();
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
