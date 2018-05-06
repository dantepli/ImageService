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
using ImageServiceGUI.Models;

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

        public event EventHandler<DirectoryPathRemovedEventArgs> DirectoryPathRemoved;

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
            Client.Close();
        }

        public string ExecuteCommand(CommandEnum command, string[] args, out bool result)
        {
            if (!IsConnected)
            {
                result = false;
                return null;
            }
            //while (!CanWrite)
            //{
            //    Thread.Sleep(1000);
            //}
            //writer.Write((int)command);
            //string toSend = String.Join(";", args);
            //writer.Write(toSend);
            //string serverResponse = null;
            //Thread.Sleep(50);
            //if (stream.DataAvailable)
            //{
            //    serverResponse = reader.ReadString();
            //}
            //if (!String.IsNullOrEmpty(serverResponse))
            //{
            //    result = true;
            //}
            //else
            //{
            //    result = false;
            //}
            //return serverResponse;

            m_streamWriter.AutoFlush = true;
            string toSend = ((int)command).ToString();
            string response = "";
            toSend = toSend + ";" + String.Join(";", args);
            m_streamWriter.WriteLine(toSend);
            m_streamWriter.Flush();
            while(m_streamReader.Peek() > 0)
            {
                response += m_streamReader.ReadLine();
            }
            if (!String.IsNullOrEmpty(response))
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return response;
        }

        public void WaitForResponse()
        {
            string path = "";
            Thread.Sleep(1000);
            while (m_streamReader.Peek() > 0)
            {
                path += m_streamReader.ReadLine();
            }
            DirectoryPathRemoved?.Invoke(this, new DirectoryPathRemovedEventArgs() { Path = path });
        }
    }
}
