using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Models
{
    class DirectoryPathRemovedEventArgs : EventArgs
    {
        public string Path { get; set; }
    }
}
