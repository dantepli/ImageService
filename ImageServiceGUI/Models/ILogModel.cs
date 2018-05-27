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
    interface ILogModel : INotifyPropertyChanged
    {
        /// <summary>
        /// logs of the model
        /// </summary>
        ObservableCollection<LogRecord> ModelLogs { get; set; }
        
        /// <summary>
        /// notify listeners
        /// </summary>
        /// <param name="name">proproty that had changed</param>
        void NotifyPropertyChanged(string name);
    }
}
