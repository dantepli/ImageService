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

        /// <summary>
        /// c'tor
        /// </summary>
        public MainWindowModel()
        {
            MainFrameColor = SingletonClient.Instance.IsConnected;

            SingletonClient.Instance.ConnectedNotifyEvent += ConnectStatus;
        }

        /// <summary>
        /// change connected status to be as the client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ConnectStatus(object sender, ConnectedArgs e)
        {
            MainFrameColor = e.IsConnected;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// notify listeners
        /// </summary>
        /// <param name="name">proproty that had changed</param>
        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
