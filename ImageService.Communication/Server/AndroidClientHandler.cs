using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ImageService.Communication.Events;
using ImageService.Infrastructure.Enums;

namespace ImageService.Communication.Server
{
    public class AndroidClientHandler : IClientHandler
    {
        public event EventHandler<DataReceivedEventArgs> DataRecieved;
        public event EventHandler<ImageDataReceivedEventArgs> ImageDataReceived;

        public void HandleClient(TcpClient client)
        {
            new Task(() =>
            {
                while (true)
                {
                    byte[] imageBytes = null;
                    BinaryReader binaryReader = new BinaryReader(client.GetStream());
                    int bytesToRead = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
                    byte[] nameBytes = binaryReader.ReadBytes(bytesToRead);
                    bytesToRead = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
                    imageBytes = binaryReader.ReadBytes(bytesToRead);
                    //Array.Reverse(imageBytes);
                    string name = Encoding.Default.GetString(nameBytes);
                    ImageDataReceived?.Invoke(this,
                           new ImageDataReceivedEventArgs() { Name = name,  ImageBytes = imageBytes });
                }
            }).Start();
        }

        public void SendToAll(CommandEnum commandEnum, string msg)
        {
            throw new NotImplementedException();
        }

        public void sendToClient(TcpClient tcpClient, CommandEnum commandEnum, string msg)
        {
            throw new NotImplementedException();
        }
    }
}
