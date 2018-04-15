using ImageService.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// Execute command.
        /// </summary>
        /// <param name="args">args to command.</param>
        /// <param name="result">return true if successful.</param>
        /// <returns></returns>
        string Execute(string[] args, out bool result);          // The Function That will Execute The 
    }
}
