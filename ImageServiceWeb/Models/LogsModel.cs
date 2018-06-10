using ImageService.Communication.Client;
using ImageService.Communication.Events;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Commands;
using ImageService.Infrastructure.Enums;
using ImageService.Infrastructure.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class LogsModel
    {
        private Dictionary<int, CommandAction> m_actions;
        delegate void CommandAction(CommandMessage message);

        public string filterType;
        private List<LogRecord> m_logs;
        public List<LogRecord> Logs
        {
            get
            {
                List<LogRecord> viewList = new List<LogRecord>();
                
                foreach(LogRecord log in m_logs)
                {
                    if(string.Compare(log.Type.ToString("G"), filterType) == 0 ||
                        string.Compare(Consts.ALL, filterType) == 0)
                    {
                        viewList.Add(log);
                    }
                }

                return viewList;
            }
            private set
            {
                m_logs = value;
            }
        }

        public IClient TcpClient
        {
            get
            {
                return SingletonClient.Instance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogsModel"/> class.
        /// </summary>
        public LogsModel()
        {
            TcpClient.DataRecieved += OnDataRecieved;

            filterType = Consts.ALL;
            m_logs = new List<LogRecord>();

            string[] args = { Consts.ALL };
            CommandMessage message = new CommandMessage() { CommandID = (int)CommandEnum.LogCommand, CommandArgs = args };
            TcpClient.SendCommand(message);

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
        /// Called when logs was recieved from the server.
        /// </summary>
        /// <param name="message">The logs message.</param>
        private void OnLogReceived(CommandMessage message)
        {
            string[] logs = message.CommandArgs;

            ICollection<LogRecord> logRecords = JsonConvert.DeserializeObject<ICollection<LogRecord>>(logs[0]);
            foreach (LogRecord logRecord in logRecords)
            {
                m_logs.Add(logRecord);
            }
        }

        public void UpdateByType(string type)
        {
            filterType = string.Copy(type);
        }
    }
}
