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
using Newtonsoft.Json.Linq;

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

            string[] args = { "*" };
            bool result;
            string logsJSON = Client.Instance.ExecuteCommand(CommandEnum.LogCommand, args, out result);

            FromJSON(logsJSON);
        }

        public void FromJSON(string LogsJSON)
        {
            JObject logsObj = JObject.Parse(LogsJSON);

            string allLogs = (string)logsObj["Logs"];
            string[] logs = allLogs.Split(';');

            foreach (string log in logs)
            {
                string[] logDetails = log.Split(',');
                int type;
                int.TryParse(logDetails[0], out type);

                m_ModelLogs.Add(new LogRecord() { Type = (MessageTypeEnum)type, Message = logDetails[1] });
            }

            // NotifyPropertyChanged("ModelLogs");
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
