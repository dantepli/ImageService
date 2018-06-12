using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ImageService.Communication.Client;
using ImageService.Communication.Events;
using ImageService.Infrastructure.Commands;
using ImageService.Infrastructure.Enums;
using Newtonsoft.Json;

namespace ImageServiceWeb.Models
{
    public class HomePageModel
    {
        private int numOfPhotos = 0;

        public List<Student> Students { get; set; }

        public int NumberOfPhotos
        {
            get
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

        public IClient TcpClient
        {
            get
            {
                return SingletonClient.Instance;
            }
        }

        /// <summary>
        /// C'tor. Reads the students data from file.
        /// </summary>
        public HomePageModel()
        {
            string path = HttpRuntime.AppDomainAppPath;
            string studentDataPath = path + @"\App_Data" + @"\studentDetails.json";
            Students = JsonConvert.DeserializeObject<List<Student>>(File.ReadAllText(studentDataPath));
        }
    }
}