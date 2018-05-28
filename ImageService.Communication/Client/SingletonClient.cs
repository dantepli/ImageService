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
using System.Windows;
using ImageService.Communication.Events;
using System.Configuration;

namespace ImageService.Communication.Client
{
    public class SingletonClient : IClient
    {
        private static volatile IClient m_instance;
        private NetworkStream m_networkStream;
        private StreamReader m_streamReader;
        private StreamWriter m_streamWriter;

        private SingletonClient() { }

        public event EventHandler<ConnectedArgs> ConnectedNotifyEvent;
        public bool IsConnected
        {
            get { return Instance.Client.Connected; }
        }
        public TcpClient Client { get; private set; }

        public event EventHandler<DataReceivedEventArgs> DataRecieved;

        public static IClient Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new SingletonClient();
                    bool result = Int32.TryParse(ConfigurationManager.AppSettings["Port"], out int port);
                    if (!result)
                    {
                        // trying with default settings.
                        port = 8000;
                    }
                    m_instance.Connect(ConfigurationManager.AppSettings["IP"], port);
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
                return IsConnected;
            }
            m_networkStream = Client.GetStream();
            m_streamReader = new StreamReader(m_networkStream, Encoding.ASCII);
            m_streamWriter = new StreamWriter(m_networkStream, Encoding.ASCII);
            Task t = new Task(() => { ReadDataFromServer(); });
            t.Start();

            ConnectedNotifyEvent?.Invoke(this, new ConnectedArgs() { IsConnected = IsConnected });
            return IsConnected;

        }

        /// <summary>
        /// disconnects from the server.
        /// </summary>
        public void Disconnect()
        {
            ConnectedNotifyEvent?.Invoke(this, new ConnectedArgs() { IsConnected = false });

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
            if (IsConnected)
            {
                string toSend = cmdMsg.ToJSON();
                try
                {
                    m_streamWriter.WriteLine(toSend);
                    m_streamWriter.Flush();
                }
                catch
                {
                    return;
                }
            }
        }

        /// <summary>
        /// reads data from server as long as the client is connected.
        /// once data is received, invokes an event on the main GUI thread.
        /// </summary>
        private void ReadDataFromServer()
        {
            while (Client.Client.Connected)
            {
                string data = null;
                try
                {
                    data = m_streamReader.ReadLine();
                    while (m_streamReader.Peek() > 0)
                    {
                        data += m_streamReader.ReadLine();
                    }
                }
                catch
                {
                    // stream error. server connection possibly lost.
                    return;
                }
                DataRecieved?.Invoke(this, new DataReceivedEventArgs() { Data = data });
            }
        }
    }
}
