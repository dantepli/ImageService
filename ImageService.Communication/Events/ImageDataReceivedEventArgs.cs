using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication.Events
{
    public class ImageDataReceivedEventArgs
    {
        public string Name { get; set; }
        public byte[] ImageBytes { get; set; }
    }
}
