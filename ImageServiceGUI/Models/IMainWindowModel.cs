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
        /// <summary>
        /// boolean to represent color of frame -- is the gui connected
        /// </summary>
        bool MainFrameColor { get; }

        /// <summary>
        /// notify listeners
        /// </summary>
        /// <param name="name">proproty that had changed</param>
        void NotifyPropertyChanged(string name);
    }
}
