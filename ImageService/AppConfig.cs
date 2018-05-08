using ImageService.Infrastructure;
using ImageService.Modal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    class AppConfig
    {
        public static string OutputDir { get; set; }
        public static string SourceName { get; set; }
        public static string LogName { get; set; }
        public static int ThumbnailSize { get; set; }
        public static string[] Handlers { get; set; }

        /// <summary>
        /// C'tor.
        /// reads the properties from the app config.
        /// </summary>
        public AppConfig()
        {
            OutputDir = ConfigurationManager.AppSettings["OutputDir"];
            SourceName = ConfigurationManager.AppSettings["SourceName"];
            LogName = ConfigurationManager.AppSettings["LogName"];
            ThumbnailSize = Int32.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);
            Handlers = ParseHandlers();
        }

        /// <summary>
        /// parses the handlers in the app config. 
        /// if the handler path specified is not valid, it is not added.
        /// </summary>
        /// <returns>an array of handlers.</returns>
        private string[] ParseHandlers()
        {
            string[] handlersToCheck = ConfigurationManager.AppSettings["Handler"].Split(Consts.DELIM);
            List<string> handlers = new List<string>();
            foreach (string handler in handlersToCheck)
            {
                if (System.IO.Directory.Exists(handler))
                {
                    handlers.Add(handler);
                }
            }
            return handlers.ToArray();
        }

        public static string toJSON()
        {
            JObject appConfigObj = new JObject();
            appConfigObj["OutputDir"] = OutputDir;
            appConfigObj["SourceName"] = SourceName;
            appConfigObj["LogName"] = LogName;
            appConfigObj["ThumbnailSize"] = ThumbnailSize;
            appConfigObj["Handler"] = String.Join(Consts.DELIM.ToString(), Handlers);
            return appConfigObj.ToString();
        }

        public static void OnHandlerClose(object sender, DirectoryCloseEventArgs e)
        {
            // TODO: Mutex?
            var handlersList = Handlers.ToList();
            handlersList.Remove(e.DirectoryPath);
            Handlers = handlersList.ToArray();
        }
        //public static string toJSON()
        //{
        //    JObject appConfigObj = new JObject();
        //    appConfigObj["OutputDir"] = ConfigurationManager.AppSettings["OutputDir"];
        //    appConfigObj["SourceName"] = ConfigurationManager.AppSettings["SourceName"];
        //    appConfigObj["LogName"] = ConfigurationManager.AppSettings["LogName"];
        //    appConfigObj["ThumbnailSize"] = Int32.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);
        //    appConfigObj["Handler"] = ConfigurationManager.AppSettings["Handler"];
        //    return appConfigObj.ToString();
        //}
    }
}
