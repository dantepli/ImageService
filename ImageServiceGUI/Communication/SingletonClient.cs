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
using System.Windows.Threading;
using System.Windows;

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
            Task t = new Task(() => { ReadDataFromServer(); });
            t.Start();
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
        public void SendCommand(CommandMessage cmdMsg)
        {
            string toSend = cmdMsg.ToJSON();
            m_streamWriter.WriteLine(toSend);
            m_streamWriter.Flush();
        }

        /// <summary>
        /// reads data from server as long as the client is connected.
        /// once data is received, invokes an event on the main GUI thread.
        /// </summary>
        private void ReadDataFromServer()
        {
            while(Client.Client.Connected)
            {
                string data;
                data = m_streamReader.ReadLine();
                while (m_streamReader.Peek() > 0)
                {
                    data += m_streamReader.ReadLine();
                }
                // update GUI thread on data received.
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    DataRecieved?.Invoke(this, new DataReceivedEventArgs() { Data = data });
                }));
            }
        }
    }
}
