using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure.Enums;
using ImageService.Infrastructure.Objects;
using ImageService.Communication;

namespace ImageServiceGUI.Models
{
    class LogModel : ILogModel
    {
        private ObservableCollection<LogRecord> m_ModelLogs;

        public ObservableCollection<LogRecord> ModelLogs
        {
            get { return m_ModelLogs; }
            set
            {
                m_ModelLogs = value;
                NotifyPropertyChanged("ModelLogs");
            }
        }

        public LogModel()
        {
            m_ModelLogs = new ObservableCollection<LogRecord>();

            //Client.Instance.sendCommand(CommandEnum.AllLogCommand);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string name)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
