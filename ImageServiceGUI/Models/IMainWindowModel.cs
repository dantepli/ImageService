using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ImageServiceGUI.Models
{
    interface IMainWindowModel : INotifyPropertyChanged
    {
        bool MainFrameColor { get; }

        void NotifyPropertyChanged(string name);
    }
}
