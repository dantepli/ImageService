using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using ImageService.Controller;
using ImageService.Infrastructure.Enums;

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
            clientInfo.NetworkStream = client.GetStream();
            clientInfo.StreamReader = new StreamReader(clientInfo.NetworkStream);
            clientInfo.StreamWriter = new StreamWriter(clientInfo.NetworkStream);
            m_clients.Add(clientInfo);
            new Task(() =>
            {
                clientInfo.StreamWriter.AutoFlush = true;
                string args = clientInfo.StreamReader.ReadLine();
                string[] splitArgs = args.Split(';');
                int commandID = int.Parse(splitArgs[0]);
                splitArgs = splitArgs.Skip(1).ToArray();
                bool result;
                string response = m_controller.ExecuteCommand(commandID, splitArgs, out result);
                clientInfo.StreamWriter.WriteLine(response);
                clientInfo.StreamWriter.Flush();
                //client.Close();
            }).Start();
        }

        /// <summary>
        /// sends to all connected clients the message given.
        /// </summary>
        /// <param name="msg">a message to send.</param>
        public void SendToAll(string msg)
        {
            new Task(() =>
            {
                foreach (ClientInfo clientInfo in m_clients)
                {
                    if (clientInfo.Client.Connected == true)
                    {
                        clientInfo.StreamWriter.WriteLine(msg);
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
