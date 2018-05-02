using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ImageService.Communication
{
    interface IClientHandler
    {
        /// <summary>
        /// handles a client request.
        /// </summary>
        /// <param name="client">a tcp client object.</param>
        void HandleClient(TcpClient client);
    }
}
