﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure.Enums;
using ImageService.Infrastructure.Objects;
using Newtonsoft.Json.Linq;
using ImageService.Infrastructure.Commands;
using ImageService.Infrastructure;
using Newtonsoft.Json;
using ImageService.Communication.Client;
using ImageService.Communication.Events;
using System.Windows;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="LogModel"/> class.
        /// </summary>
        public LogModel()
        {
            SingletonClient.Instance.DataRecieved += OnDataRecieved;

            m_ModelLogs = new ObservableCollection<LogRecord>();

            string[] args = { Consts.ALL };
            CommandMessage message = new CommandMessage() { CommandID = (int)CommandEnum.LogCommand, CommandArgs = args };
            SingletonClient.Instance.SendCommand(message);

            m_actions = new Dictionary<int, CommandAction>()
            {
                { (int)CommandEnum.LogCommand, OnLogReceived }
            };
        }

        /// <summary>
        /// handle data recived from server
        /// </summary>
        /// <param name="sender">sender of data</param>
        /// <param name="e">args to represent data</param>
        public void OnDataRecieved(object sender, DataReceivedEventArgs e)
        {
            CommandMessage message = CommandMessage.FromJSON(e.Data);

            if (m_actions.ContainsKey(message.CommandID))
            {
                m_actions[message.CommandID](message);
            }
        }

        /// <summary>
        /// handle log recived from server
        /// </summary>
        /// <param name="message">Command to represent log\s</param>
        private void OnLogReceived(CommandMessage message)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                InterpretLogs(message.CommandArgs);
            }));
        }

        /// <summary>
        /// interpret logs
        /// </summary>
        /// <param name="logs">string array to represent logs</param>
        public void InterpretLogs(string[] logs)
        {
            ICollection<LogRecord> logRecords = JsonConvert.DeserializeObject<ICollection<LogRecord>>(logs[0]);
            foreach (LogRecord logRecord in logRecords)
            {
                m_ModelLogs.Add(logRecord);
            }

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
