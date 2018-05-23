using ImageService.Communication.Events;
using ImageService.Infrastructure.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication.Client
{
    public interface IClient
    {
        bool IsConnected { get; }
        TcpClient Client { get; }
        event EventHandler<DataReceivedEventArgs> DataRecieved;

        /// <summary>
        /// connects to the given IP:port.
        /// </summary>
        /// <param name="IP">IP address.</param>
        /// <param name="port">port number.</param>
        /// <returns>true if connection was established.</returns>
        bool Connect(string IP, int port);

        /// <summary>
        /// disconnects from the server.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// sends a command to the server.
        /// </summary>
        /// <param name="cmdMsg">A command message object to send to the server.</param>
        void SendCommand(CommandMessage cmdMsg);
    }
}
