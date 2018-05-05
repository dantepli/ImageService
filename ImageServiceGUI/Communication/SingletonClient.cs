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

namespace ImageServiceGUI.Communication
{
    class SingletonClient : IClient
    {
        private static volatile IClient m_instance;

        private SingletonClient() { }

        public bool IsConnected { get; set; }
        public bool CanWrite { get; set; }
        public TcpClient Client { get; private set; }

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
            }            catch (SocketException)
            {
                IsConnected = false;
                CanWrite = false;
                return IsConnected;
            }            IsConnected = true;            CanWrite = true;            return IsConnected;
        }

        /// <summary>
        /// disconnects from the server.
        /// </summary>
        public void Disconnect()
        {
            Client.Close();
        }

        public string ExecuteCommand(CommandEnum command, string[] args, out bool result)
        {
            Connect("127.0.0.1", 8000);
            if (!IsConnected)
            {
                result = false;
                return null;
            }
            using (NetworkStream stream = m_instance.Client.GetStream())
            using (BinaryReader reader = new BinaryReader(stream))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                while (!CanWrite)
                {
                    Thread.Sleep(1000);
                }
                writer.Write((int)command);
                string toSend = String.Join(";", args);
                writer.Write(toSend);
                string serverResponse = reader.ReadString();
                if (String.IsNullOrEmpty(serverResponse))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
                return serverResponse;
            }
        }
    }
}
