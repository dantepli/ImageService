using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ImageService.Server
{
    public interface IClientHandler
    {
        /// <summary>
        /// handles a client request.
        /// </summary>
        /// <param name="client">a tcp client object.</param>
        void HandleClient(TcpClient client);

        /// <summary>
        /// sends to all connected clients the message given.
        /// </summary>
        /// <param name="msg">a message to send.</param>
        void SendToAll(string msg);
    }
}
