using ImageServiceGUI.Models;
using ImageService.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ImageServiceGUI.ViewModels
{
    class LogViewModel : ViewModel
    {
        private ILogModel m_model;
        private ObservableCollection<LogRecord> m_log;

        public ObservableCollection<LogRecord> Logs
        {
            get { return m_log; }
            set
            {
                m_log = value;
                NotifyPropertyChanged("Logs");
            }
        }

        public LogViewModel()
        {
            m_model = new LogModel();
            m_model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e) {
                if(e.PropertyName == "")
            };

            m_log = new ObservableCollection<LogRecord>();

            m_log.Add(new LogRecord() { Type = MessageTypeEnum.INFO, Message = "textttt" });
            m_log.Add(new LogRecord() { Type = MessageTypeEnum.INFO, Message = "textttt" });
            m_log.Add(new LogRecord() { Type = MessageTypeEnum.INFO, Message = "textttt" });
            m_log.Add(new LogRecord() { Type = MessageTypeEnum.INFO, Message = "textttt" });
        }
    }
}
