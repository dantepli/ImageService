using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure.Enums;
using ImageService.Infrastructure.Objects;

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

            m_ModelLogs.Add(new LogRecord() { Type = MessageTypeEnum.INFO, Message = "text about log..." });
            m_ModelLogs.Add(new LogRecord() { Type = MessageTypeEnum.INFO, Message = "text about log..." });
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
