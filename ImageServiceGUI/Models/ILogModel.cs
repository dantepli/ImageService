using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure.Enums;

namespace ImageServiceGUI.Models
{
    interface ILogModel
    {
        MessageTypeEnum Type { get; set; }
        string Message { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged(string name);
    }
}
