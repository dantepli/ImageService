using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Models
{
    interface ISettingsModel
    {
        string OutputDir { get; }
        string SourceName { get; }
        string LogName { get; }
        int ThumbnailSize { get; }

        bool RemoveHandler(DirectoryPath rmPath);
    }
}
