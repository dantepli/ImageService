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
        private IImageServiceModal modal;

        public NewFileCommand(IImageServiceModal m)
        {
            this.modal = m;
        }

        public string Execute(string[] args, out bool result)
        {
            return modal.AddFile(args[0], out result);
        }
    }
}
