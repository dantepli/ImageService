using ImageService.Communication.Client;
using ImageService.Communication.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ImageServiceGUI.Models
{
    class MainWindowModel : IMainWindowModel
    {
        private bool m_mainFrameColor;

        public bool MainFrameColor
        {
            get
            {
                return m_mainFrameColor;
            }
            set
            {
                m_mainFrameColor = value;
                NotifyPropertyChanged("MainFrameColor");
            }
        }

        public MainWindowModel()
        {
            MainFrameColor = SingletonClient.Instance.IsConnected;

            SingletonClient.Instance.ConnectedNotifyEvent += ConnectStatus;
        }

        public void ConnectStatus(object sender, ConnectedArgs e)
        {
            MainFrameColor = e.IsConnected;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
