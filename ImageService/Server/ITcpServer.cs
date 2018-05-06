using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public interface ITcpServer
    {
        void Start();
        void Stop();
        void SendToAll(string msg);
    }
}
