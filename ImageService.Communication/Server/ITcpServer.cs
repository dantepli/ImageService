using ImageService.Communication.Events;
using ImageService.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication.Server
{
    public interface ITcpServer
    {
        event EventHandler<DataReceivedEventArgs> DataRecieved;
        event EventHandler<ImageDataReceivedEventArgs> ImageDataRecieved;
        /// <summary>
        /// starts the server and starts accepting connections.
        /// </summary>
        void Start();

        /// <summary>
        /// stops the server.
        /// </summary>
        void Stop();

        /// <summary>
        /// sends a given message to all connected clients.
        /// </summary>
        /// <param name="commandEnum">the command enum.</param>
        /// <param name="msg">the message to send.</param>
        void SendToAll(CommandEnum commandEnum, string msg);

        /// <summary>
        /// send a message to a specific client.
        /// </summary>
        /// <param name="client">client to send to.</param>
        /// <param name="commandEnum">the command's enum.</param>
        /// <param name="msg">message to send.</param>
        void sendToClient(TcpClient client, CommandEnum commandEnum, string msg);
    }
}
