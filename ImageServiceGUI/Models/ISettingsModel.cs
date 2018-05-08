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
    interface ISettingsModel : INotifyPropertyChanged
    {
        string OutputDir { get; }
        string SourceName { get; }
        string LogName { get; }
        int ThumbnailSize { get; }

        ObservableCollection<DirectoryPath> DirectoryPaths { get; set; }

        void NotifyPropertyChanged(string name);
        void RemoveHandler(DirectoryPath rmPath);
    }
}
