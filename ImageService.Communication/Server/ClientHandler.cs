using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using ImageService.Infrastructure.Enums;
using ImageService.Infrastructure.Commands;
using ImageService.Communication.Events;

namespace ImageService.Communication.Server
{
    public class ClientHandler : IClientHandler
    {
        private ICollection<ClientInfo> m_clients;

        public event EventHandler<DataReceivedEventArgs> DataRecieved;

        public ClientHandler()
        {
            m_clients = new List<ClientInfo>();

        }

        /// <summary>
        /// handles a specific client.
        /// </summary>
        /// <param name="client">a client to handle.</param>
        public void HandleClient(TcpClient client)
        {
            ClientInfo clientInfo = new ClientInfo();
            clientInfo.Client = client;
            clientInfo.NetworkStream = client.GetStream();
            clientInfo.StreamReader = new StreamReader(clientInfo.NetworkStream, Encoding.ASCII);
            clientInfo.StreamWriter = new StreamWriter(clientInfo.NetworkStream, Encoding.ASCII);
            m_clients.Add(clientInfo);
            new Task(() =>
            {
                while (true)
                {
                    clientInfo.StreamWriter.AutoFlush = true;
                    string json = clientInfo.StreamReader.ReadLine();
                    while (clientInfo.StreamReader.Peek() > 0)
                    {
                        json += clientInfo.StreamReader.ReadLine();
                    }
                    DataRecieved?.Invoke(this, new DataReceivedEventArgs()
                    {
                        Data = json,
                        Client = clientInfo.Client
                    });
                }
            }).Start();
        }

        /// <summary>
        /// sends to all connected clients the message given.
        /// </summary>
        /// <param name="msg">a message to send.</param>
        public void SendToAll(CommandEnum commandEnum, string msg)
        {
            new Task(() =>
            {
                lock (m_clients)
                {
                    foreach (ClientInfo clientInfo in m_clients)
                    {
                        if (clientInfo.Client.Connected == true)
                        {
                            string[] args = { msg };
                            CommandMessage cmdMsg = new CommandMessage()
                            {
                                CommandID = (int)commandEnum,
                                CommandArgs = args
                            };
                            lock (clientInfo.StreamWriter)
                            {
                                clientInfo.StreamWriter.WriteLine(cmdMsg.ToJSON());
                                clientInfo.StreamWriter.Flush();
                            }
                        }
                        else
                        {
                            clientInfo.StreamReader.Close();
                            clientInfo.StreamWriter.Close();
                            clientInfo.Client.Close();
                            m_clients.Remove(clientInfo);
                        }
                    }
                }
            }).Start();
        }

        /// <summary>
        /// sends a msg to a specific client.
        /// </summary>
        /// <param name="tcpClient">client to send a message to.</param>
        /// <param name="commandEnum">the command enum.</param>
        /// <param name="msg">message to send.</param>
        public void sendToClient(TcpClient tcpClient, CommandEnum commandEnum, string msg)
        {
            new Task(() =>
            {
                lock (m_clients)
                {
                    foreach (ClientInfo clientInfo in m_clients)
                    {
                        if (clientInfo.Client == tcpClient)
                        {
                            // client found.
                            string[] args = { msg };
                            CommandMessage response = new CommandMessage()
                            {
                                CommandID = (int)commandEnum,
                                CommandArgs = args
                            };
                            lock (clientInfo.StreamWriter)
                            {
                                clientInfo.StreamWriter.WriteLine(response.ToJSON());
                                clientInfo.StreamWriter.Flush();
                            }
                        }
                    }
                }
            }).Start();
        }
    }
}
