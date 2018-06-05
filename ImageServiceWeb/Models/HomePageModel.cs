using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ImageService.Communication.Client;
using ImageService.Communication.Events;
using ImageService.Infrastructure.Commands;
using ImageService.Infrastructure.Enums;

namespace ImageServiceWeb.Models
{
    public class HomePageModel
    {
        private int numOfPhotos = 0;

        public List<Student> Students { get; } = new List<Student>()
        {
            new Student { FirstName = "Dan", LastName = "Teplitski", ID = 312895147 },
            new Student { FirstName = "Bar", LastName = "Katz", ID = 1111}
        };

        public int NumberOfPhotos
        {
            get
            {
                if (numOfPhotos != 0)
                {
                    return numOfPhotos;
                }
                else
                {
                    string dirPath = SettingsContainer.Instance.OutputDir;
                    dirPath = dirPath + @"\Thumbnails";
                    if (Directory.Exists(dirPath))
                    {
                        numOfPhotos = (from file in Directory.EnumerateFiles(dirPath, "*.*", SearchOption.AllDirectories)
                                       select file).Count();
                    }
                    return numOfPhotos;
                }
            }
        }

        public IClient TcpClient
        {
            get
            {
                return SingletonClient.Instance;
            }
        }
    }
}