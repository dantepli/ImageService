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
        void Start();
        void Stop();
        void SendToAll(CommandEnum commandEnum, string msg);
        void sendToClient(TcpClient client, CommandEnum commandEnum, string msg);
    }
}
