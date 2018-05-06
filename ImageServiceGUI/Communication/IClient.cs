using ImageService.Infrastructure.Enums;
using ImageServiceGUI.Models;
using ImageServiceGUI.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Communication
{
    interface IClient
    {
        bool IsConnected { get; set; }
        bool CanWrite { get; set; }
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
        /// sends the command to the server and awaits it's response.
        /// </summary>
        /// <param name="command">The command enum.</param>
        /// <param name="args">the arguments.</param>
        void SendCommand(CommandEnum command, string[] args);


    }
}
