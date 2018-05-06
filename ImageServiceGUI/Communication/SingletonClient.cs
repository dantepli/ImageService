using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ImageService.Infrastructure.Enums;
using ImageService.Infrastructure.Commands;
using ImageServiceGUI.Models;
using ImageServiceGUI.Models.Events;

namespace ImageServiceGUI.Communication
{
    class SingletonClient : IClient
    {
        private static volatile IClient m_instance;
        private NetworkStream m_networkStream;
        private StreamReader m_streamReader;
        private StreamWriter m_streamWriter;

        private SingletonClient() { }

        public bool IsConnected { get; set; }
        public bool CanWrite { get; set; }
        public TcpClient Client { get; private set; }

        public event EventHandler<DataReceivedEventArgs> DataRecieved;

        public static IClient Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new SingletonClient();
                    m_instance.Connect("127.0.0.1", 8000);
                }
                return m_instance;
            }
        }

        /// <summary>
        /// connects to the given IP:port.
        /// </summary>
        /// <param name="IP">IP address.</param>
        /// <param name="port">port number.</param>
        /// <returns>true if connection was established.</returns>
        public bool Connect(string IP, int port)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IP), port);
            Client = new TcpClient();
            Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            try
            {
                Client.Connect(ep);
            }
            catch (SocketException)
            {
                IsConnected = false;
                CanWrite = false;
                return IsConnected;
            }
            m_networkStream = Client.GetStream();
            m_streamReader = new StreamReader(m_networkStream);
            m_streamWriter = new StreamWriter(m_networkStream);
            IsConnected = true;
            CanWrite = true;
            return IsConnected;

        }

        /// <summary>
        /// disconnects from the server.
        /// </summary>
        public void Disconnect()
        {
            m_streamReader.Close();
            m_streamWriter.Close();
            m_networkStream.Close();
            Client.Close();
        }

        /// <summary>
        /// sends a command to the server.
        /// </summary>
        /// <param name="command">command id.</param>
        /// <param name="args">arguments for the command.</param>
        public void SendCommand(CommandEnum command, string[] args)
        {
            CommandMessage cmdMsg = new CommandMessage() { CommandID = (int)command, CommandArgs = args };
            string toSend = cmdMsg.ToJSON();
            m_streamWriter.WriteLine(toSend);
            m_streamWriter.Flush();
        }

        private void ReadDataFromServer()
        {

        }
    }
}
