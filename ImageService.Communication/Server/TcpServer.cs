using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using ImageService.Infrastructure.Enums;
using ImageService.Communication.Events;
using ImageService.Infrastructure.Commands;

namespace ImageService.Communication.Server
{
    public class TcpServer : ITcpServer
    {
        private int m_port;
        private TcpListener m_listener;
        private IClientHandler m_ch;

        public event EventHandler<DataReceivedEventArgs> DataRecieved;

        public TcpServer(int port, IClientHandler ch)
        {
            m_port = port;
            m_ch = ch;
            m_ch.DataRecieved += OnDataRecieved;

        }

        private void OnDataRecieved(object sender, DataReceivedEventArgs e)
        {
            DataRecieved?.Invoke(this, e);
        }

        /// <summary>
        /// starts the server and starts accepting connections.
        /// </summary>
        public void Start()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), m_port);
            m_listener = new TcpListener(ep);

            m_listener.Start();
            Console.WriteLine("Waiting for connections...");
            Task task = new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        TcpClient client = m_listener.AcceptTcpClient();
                        Console.WriteLine("Got new connection");
                        m_ch.HandleClient(client);
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }
                Console.WriteLine("Server stopped");
            });
            task.Start();
        }

        /// <summary>
        /// stops the server.
        /// </summary>
        public void Stop()
        {
            m_listener.Stop();
        }

        /// <summary>
        /// sends a given message to all connected clients.
        /// </summary>
        /// <param name="commandEnum">the command enum.</param>
        /// <param name="msg">the message to send.</param>
        public void SendToAll(CommandEnum commandEnum, string msg)
        {
            m_ch.SendToAll(commandEnum, msg);
        }

        /// <summary>
        /// send a message to a specific client.
        /// </summary>
        /// <param name="client">client to send to.</param>
        /// <param name="commandEnum">the command's enum.</param>
        /// <param name="msg">message to send.</param>
        public void sendToClient(TcpClient client, CommandEnum commandEnum, string msg)
        {
            m_ch.sendToClient(client, commandEnum, msg);
        }
    }
}
