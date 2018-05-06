using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;
using ImageService.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModel m_model;                      // The Model Object
        private Dictionary<int, ICommand> m_commands;

        public ImageController(IImageServiceModel model)
        {
            m_model = model;                    // Storing the Model Of The System
            m_commands = new Dictionary<int, ICommand>()
            {
                {(int) CommandEnum.NewFileCommand, new NewFileCommand(m_model)},
                {(int) CommandEnum.GetConfigCommand, new GetConfigCommand()}
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

        /// <summary>
        /// adds a close command.
        /// </summary>
        /// <param name="server">the server to send the command to.</param>
        public void AddCloseCommand(ImageServer server)
        {
            m_commands.Add((int)CommandEnum.CloseCommand, new CloseCommand(server));
        }
    }
}
