using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using ImageService.Controller;
using ImageService.Infrastructure.Enums;
using ImageService.Infrastructure.Commands;

namespace ImageService.Server
{
    public class ClientHandler : IClientHandler
    {
        private ICollection<ClientInfo> m_clients;
        private IImageController m_controller;

        public ClientHandler(IImageController controller)
        {
            m_controller = controller;
            m_clients = new List<ClientInfo>();
        }

        public void HandleClient(TcpClient client)
        {
            ClientInfo clientInfo = new ClientInfo();
            clientInfo.Client = client;
            clientInfo.NetworkStream = client.GetStream();
            clientInfo.StreamReader = new StreamReader(clientInfo.NetworkStream);
            clientInfo.StreamWriter = new StreamWriter(clientInfo.NetworkStream);
            m_clients.Add(clientInfo);
            new Task(() =>
            {
                while(true)
                {
                    clientInfo.StreamWriter.AutoFlush = true;
                    string json = clientInfo.StreamReader.ReadLine();
                    while(clientInfo.StreamReader.Peek() > 0)
                    {
                        json += clientInfo.StreamReader.ReadLine();
                    }
                    CommandMessage cmdMsg = CommandMessage.FromJSON(json);
                    bool result;
                    string msg = m_controller.ExecuteCommand(cmdMsg.CommandID, cmdMsg.CommandArgs, out result);
                    string[] args = { msg };
                    CommandMessage response = new CommandMessage() { CommandID = cmdMsg.CommandID, CommandArgs = args };
                    clientInfo.StreamWriter.WriteLine(response.ToJSON());
                    clientInfo.StreamWriter.Flush();
                }
                //client.Close();
            }).Start();
        }

        /// <summary>
        /// sends to all connected clients the message given.
        /// </summary>
        /// <param name="msg">a message to send.</param>
        public void SendToAll(CommandEnum commandEnum,string msg)
        {
            new Task(() =>
            {
                foreach (ClientInfo clientInfo in m_clients.ToList())
                {
                    if (clientInfo.Client.Connected == true)
                    {
                        string[] args = { msg };
                        CommandMessage cmdMsg = new CommandMessage() { CommandID = (int)commandEnum, CommandArgs = args };
                        clientInfo.StreamWriter.WriteLine(cmdMsg.ToJSON());
                        clientInfo.StreamWriter.Flush();
                    }
                    else
                    {
                        clientInfo.StreamReader.Close();
                        clientInfo.StreamWriter.Close();
                        clientInfo.Client.Close();
                        m_clients.Remove(clientInfo);
                    }
                    //client.Close();
                }
            }).Start();
        }
    }
}
