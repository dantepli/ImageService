using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using ImageService.Controller;
using ImageService.Infrastructure.Enums;

namespace ImageService.Server
{
    public class ClientHandler : IClientHandler
    {
        private IImageController m_controller;

        public ClientHandler(IImageController controller)
        {
            m_controller = controller;
        }

        public void HandleClient(TcpClient client)
        {
            new Task(() =>
            {
                using (NetworkStream stream = client.GetStream())
                using (BinaryReader reader = new BinaryReader(stream))
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    CommandEnum command = (CommandEnum)reader.ReadInt32();
                    string args = reader.ReadString();
                    string[] splitArgs = args.Split(';');
                    bool result;
                    string toSend = m_controller.ExecuteCommand((int)command, splitArgs, out result);
                    writer.Write(toSend);
                }
                //client.Close();
            }).Start();
        }
    }
}
