using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Infrastructure.Commands
{
    class CommandMessage
    {
        public int CommandID { get; set; }
        public string[] CommandArgs { get; set; }

        /// <summary>
        /// converts the command message to a json object.
        /// </summary>
        /// <returns>a json object in string format.</returns>
        public string ToJSON()
        {
            JObject command = new JObject();
            command["CommandID"] = CommandID;
            JArray args = new JArray(CommandArgs);
            command["CommandArgs"] = args;
            return command.ToString();
        }

        /// <summary>
        /// converts a string representation of a json object
        /// to a command message object.
        /// </summary>
        /// <param name="json">json object in string format.</param>
        /// <returns>CommandMessage object.</returns>
        public static CommandMessage FromJSON(string json)
        {
            CommandMessage cmdMessage = new CommandMessage();
            JObject command = JObject.Parse(json);
            cmdMessage.CommandID = (int)command["CommandID"];
            JArray args = (JArray)command["CommandArgs"];
            cmdMessage.CommandArgs = args.ToObject<string[]>();
            return cmdMessage;
        }
    }
}
