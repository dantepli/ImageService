using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure.Commands;

namespace ImageService.Commands
{
    class GetConfigCommand : ICommand
    {
        public AppConfig AppConfig { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetConfigCommand"/> class.
        /// initializes the app config.
        /// </summary>
        public GetConfigCommand()
        {
            AppConfig = new AppConfig();
        }

        /// <summary>
        /// Execute the get config command.
        /// </summary>
        /// <param name="args">args to command.</param>
        /// <param name="result">return true if successful.</param>
        /// <returns>the json represntation of the app config object.</returns>
        public string Execute(string[] args, out bool result)
        {
            result = true;
            return AppConfig.toJSON();
        }
    }
}
