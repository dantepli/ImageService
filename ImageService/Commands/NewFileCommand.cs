using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public class NewFileCommand : ICommand
    {
        private IImageServiceModel m_modal;

        public NewFileCommand(IImageServiceModel m)
        {
            this.m_modal = m;
        }

        /// <summary>
        /// Execute new file command in image modal.
        /// </summary>
        /// <param name="args">path to directory to create file in</param>
        /// <param name="result">return true if successful</param>
        /// <returns>path is successful, error message otherwise.</returns>
        public string Execute(string[] args, out bool result)
        {
            return m_modal.AddFile(args[0], out result);
        }
    }
}
