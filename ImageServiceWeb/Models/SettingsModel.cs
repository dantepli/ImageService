using ImageService.Communication.Client;
using ImageService.Communication.Events;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Commands;
using ImageService.Infrastructure.Enums;
using ImageService.Infrastructure.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class SettingsModel
    {
        private Dictionary<int, CommandAction> m_actions;
        delegate void CommandAction(CommandMessage message);

        private object Lock { get; set; } = new object();

        public SettingsContainer SettingsContainer
        {
            get
            {
                return SettingsContainer.Instance;
            }
            private set { }
        }

        public IClient TcpClient
        {
            get
            {
                return SingletonClient.Instance;
            }
        }

        /// <summary>
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

        /// <summary>
        /// Sends the removal command to the server.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void RemoveHandler(string handler)
        {
            if (SettingsContainer.Handlers.Contains(handler))
            {
                string[] args = { handler };
                CommandMessage message = new CommandMessage() { CommandID = (int)CommandEnum.CloseCommand, CommandArgs = args };
                TcpClient.SendCommand(message);
                lock (Lock)
                {
                    Monitor.Wait(Lock);
                }
            }
        }

        /// <summary>
        /// Called when a handler is removed.
        /// Removes the handler from the list.
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnRemoveHandler(CommandMessage message)
        {
            foreach (string path in SettingsContainer.Handlers)
            {
                if (path == message.CommandArgs[0])
                {
                    SettingsContainer.Handlers.Remove(path);
                    break;
                }
            }
            lock (Lock)
            {
                Monitor.PulseAll(Lock);
            }
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
        /// Called when config was recieved from the server.
        /// </summary>
        /// <param name="message">The config message.</param>
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

        // TODO: Move To Photos Model.
        private void RelativeOutputDir()
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            //string path = HttpRuntime.AppDomainAppPath;
            string directory = System.IO.Path.GetDirectoryName(path);
            Uri workingDir = new Uri(directory);
            Uri outputDir = new Uri(SettingsContainer.OutputDir);
            string relativePath = Uri.UnescapeDataString(workingDir.MakeRelativeUri(outputDir).ToString().Replace('/', System.IO.Path.DirectorySeparatorChar));
            SettingsContainer.OutputDir = relativePath;
        }
    }
}