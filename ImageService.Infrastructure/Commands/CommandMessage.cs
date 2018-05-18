using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Infrastructure.Commands
{
    public class CommandMessage
    {
        public int CommandID { get; set; }
        public string[] CommandArgs { get; set; }

        /// <summary>
        /// converts the command message to a json object.
        /// </summary>
        /// <returns>a json object in string format.</returns>
        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// converts a string representation of a json object
        /// to a command message object.
        /// </summary>
        /// <param name="json">json object in string format.</param>
        /// <returns>CommandMessage object.</returns>
        public static CommandMessage FromJSON(string json)
        {
            return JsonConvert.DeserializeObject<CommandMessage>(json);
        }
    }
}
