using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure.Enums;

namespace ImageService.Infrastructure.Objects
{
    public class LogRecord
    {
        public MessageTypeEnum Type { get; set; }
        public string Message { get; set; }
    }
}
