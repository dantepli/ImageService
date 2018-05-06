using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure.Enums;
using ImageService.Infrastructure.Objects;
using Newtonsoft.Json.Linq;
using ImageServiceGUI.Communication;
using ImageService.Infrastructure.Commands;
using ImageServiceGUI.Models.Events;

namespace ImageServiceGUI.Models
{
    class LogModel : ILogModel
    {
        delegate void CommandAction(CommandMessage message);
        private Dictionary<int, CommandAction> m_actions;

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
            SingletonClient.Instance.DataRecieved += OnDataRecieved;

            m_ModelLogs = new ObservableCollection<LogRecord>();

            //string[] args = { "*" };
            //CommandMessage message = new CommandMessage() { CommandID = (int)CommandEnum.LogCommand, CommandArgs = args };
            //SingletonClient.Instance.SendCommand(message);

            m_actions = new Dictionary<int, CommandAction>()
            {
                { (int)CommandEnum.LogCommand, OnLogRecived }
            };
        }

        public void OnDataRecieved(object sender, DataReceivedEventArgs e)
        {
            CommandMessage message = CommandMessage.FromJSON(e.Data);

            if (m_actions.ContainsKey(message.CommandID))
            {
                m_actions[message.CommandID](message);
            }
        }

        private void OnLogRecived(CommandMessage message)
        {
            InterpretLogs(message.CommandArgs);
        }

        public void InterpretLogs(string[] logs)
        {
            foreach (string log in logs)
            {
                string[] logDetails = log.Split(',');
                int type;
                int.TryParse(logDetails[0], out type);

                m_ModelLogs.Add(new LogRecord() { Type = (MessageTypeEnum)type, Message = logDetails[1] });
            }
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
