using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication.Events
{
    public class DataReceivedEventArgs
    {
        public string Data { get; set; }
        public TcpClient Client { get; set; }
    }
}
