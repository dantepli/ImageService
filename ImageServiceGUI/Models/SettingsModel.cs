using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Models
{
    class SettingsModel : ISettingsModel
    {
        public string OutputDir
        {
            get { return null ; }
        }

        public string SourceName
        {
            get { return null; }
        }

        public string LogName
        {
            get { return null; }
        }

        public int ThumbnailSize
        {
            get { return 5; }
        }

        public bool RemoveHandler(DirectoryPath rmPath)
        {
            // TODO ask server to remove handler and get response
            return true;
        }
    }
}
