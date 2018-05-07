using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    class JSONConfigParser
    {
        public static string toJSON()
        {
            JObject appConfigObj = new JObject();
            appConfigObj["OutputDir"] = ConfigurationManager.AppSettings["OutputDir"];
            appConfigObj["SourceName"] = ConfigurationManager.AppSettings["SourceName"];
            appConfigObj["LogName"] = ConfigurationManager.AppSettings["LogName"];
            appConfigObj["ThumbnailSize"] = Int32.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);
            appConfigObj["Handler"] = ConfigurationManager.AppSettings["Handler"];
            return appConfigObj.ToString();
        }
    }
}
