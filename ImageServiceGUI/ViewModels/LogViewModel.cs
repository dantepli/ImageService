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

        public ObservableCollection<LogRecord> Logs
        {
            get { return m_model.ModelLogs; }
            set
            {
                m_model.ModelLogs = value;
            }
        }

        public LogViewModel(ILogModel lm)
        {
            m_model = lm;

            m_model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e) {
                if(e.PropertyName == "ModelLogs")
                {
                    NotifyPropertyChanged("Logs");
                }
            };
        }
    }
}
