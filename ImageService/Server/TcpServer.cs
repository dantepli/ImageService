using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using ImageService.Infrastructure.Enums;

namespace ImageService.Server
{
    public class TcpServer : ITcpServer
    {
        private int m_port;
        private TcpListener m_listener;
        private IClientHandler m_ch;

        public TcpServer(int port, IClientHandler ch)
        {
            m_port = port;
            m_ch = ch;
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
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void SendToAll(CommandEnum commandEnum, string msg)
        {
            m_ch.SendToAll(commandEnum, msg);
        }
    }
}
