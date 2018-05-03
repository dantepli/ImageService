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
        ObservableCollection<LogRecord> ModelLogs { get; set; }
        
        void NotifyPropertyChanged(string name);
    }
}
