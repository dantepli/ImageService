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
        // settings of service
        string OutputDir { get; }
        string SourceName { get; }
        string LogName { get; }
        int ThumbnailSize { get; }

        ObservableCollection<DirectoryPath> DirectoryPaths { get; set; }

        /// <summary>
        /// notify listeners
        /// </summary>
        /// <param name="name">proproty that had changed</param>
        void NotifyPropertyChanged(string name);

        /// <summary>
        /// remove handler from service
        /// </summary>
        /// <param name="rmPath">path of handler</param>
        void RemoveHandler(DirectoryPath rmPath);
    }
}
