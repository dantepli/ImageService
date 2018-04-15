using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModel m_modal;                      // The Modal Object
        private Dictionary<int, ICommand> m_commands;

        public ImageController(IImageServiceModel modal)
        {
            m_modal = modal;                    // Storing the Modal Of The System
            m_commands = new Dictionary<int, ICommand>()
            {
                {(int) CommandEnum.NewFileCommand, new NewFileCommand(m_modal)}
            };
        }

        /// <summary>
        /// Execute given command in a new task.
        /// </summary>
        /// <param name="commandID">id of command.</param>
        /// <param name="args">args to given command.</param>
        /// <param name="resultSuccesful">true if successful.</param>
        /// <returns>string representing the result of the command.</returns>
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            Task<Tuple<string, bool>> t = new Task<Tuple<string, bool>>(() => 
            {
                bool temp_result;
                string msg = m_commands[commandID].Execute(args, out temp_result);
                
                return Tuple.Create(msg, temp_result);
            });
            t.Start();
            Tuple<string, bool> result = t.Result;
            resultSuccesful = result.Item2;
            
            return result.Item1;
        }
    }
}
