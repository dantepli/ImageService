using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using ImageService.Infrastructure.Enums;
using ImageService.Communication.Events;

namespace ImageService.Communication.Server
{
    public interface IClientHandler
    {
        event EventHandler<DataReceivedEventArgs> DataRecieved;
        event EventHandler<ImageDataReceivedEventArgs> ImageDataReceived;

        /// <summary>
        /// handles a client request.
        /// </summary>
        /// <param name="client">a tcp client object.</param>
        void HandleClient(TcpClient client);

        /// <summary>
        /// sends to all connected clients the message given.
        /// </summary>
        /// <param name="msg">a message to send.</param>
        void SendToAll(CommandEnum commandEnum, string msg);

        /// <summary>
        /// sends a message to the specified client.
        /// </summary>
        /// <param name="tcpClient">the client to send.</param>
        /// <param name="commandEnum">the command to send.</param>
        /// <param name="msg">message to send.</param>
        void sendToClient(TcpClient tcpClient, CommandEnum commandEnum, string msg);
    }
}
