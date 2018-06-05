using ImageService.Communication.Client;
using ImageService.Communication.Events;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Commands;
using ImageService.Infrastructure.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class SettingsModel
    {
        private Dictionary<int, CommandAction> m_actions;
        delegate void CommandAction(CommandMessage message);

        public SettingsContainer SettingsContainer { get; set; }

        public IClient TcpClient
        {
            get
            {
                return SingletonClient.Instance;
            }
        }

        /// <summary>
        /// 
        /// Initializes a new instance of the <see cref="SettingsModel"/> class.
        /// </summary>
        public SettingsModel()
        {
            TcpClient.DataRecieved += OnDataRecieved;
            string[] args = { };
            CommandMessage message = new CommandMessage() { CommandID = (int)CommandEnum.GetConfigCommand, CommandArgs = args };
            TcpClient.SendCommand(message);

            m_actions = new Dictionary<int, CommandAction>()
            {
                { (int)CommandEnum.GetConfigCommand, OnConfigRecived },
                { (int)CommandEnum.CloseCommand, OnRemoveHandler }
            };
        }

        private void OnRemoveHandler(CommandMessage message)
        {
            throw new NotImplementedException();
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

        private void OnConfigRecived(CommandMessage message)
        {
            JObject settingsObj = JObject.Parse(message.CommandArgs[0]);
            SettingsContainer.OutputDir = (string)settingsObj["OutputDir"];
            SettingsContainer.SourceName = (string)settingsObj["SourceName"];
            SettingsContainer.LogName = (string)settingsObj["LogName"];
            SettingsContainer.ThumbnailSize = (int)settingsObj["ThumbnailSize"];

            string allHandlers = (string)settingsObj["Handler"];
            string[] handlers = allHandlers.Split(new char[] { Consts.DELIM }, StringSplitOptions.RemoveEmptyEntries);
            SettingsContainer.Handlers = handlers.ToList();
        }
    }
}