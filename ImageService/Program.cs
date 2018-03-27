using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using ImageService.Modal;

namespace ImageService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ImageService()
            };
            ServiceBase.Run(ServicesToRun);



            //// -----------------------------------------------
            //// TEST SECTION

            //string folder = @"C:\Users\Dan\Desktop\OutputDir";
            //string path = @"C:\Users\Dan\Desktop\test2.jpg";
            //IImageServiceModal service = new ImageServiceModal(folder, 300);
            //bool res;
            //service.AddFile(path, out res);

            //// -----------------------------------------------
        }
    }
}
