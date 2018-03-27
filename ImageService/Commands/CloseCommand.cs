using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    class CloseCommand : ICommand
    {
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;

        public CloseCommand(EventHandler<CommandRecievedEventArgs> _CommandRecieved)
        {
            CommandRecieved = _CommandRecieved;
        }

        public string Execute(string[] args, out bool result)
        {
            CommandRecieved?.Invoke(this, new CommandRecievedEventArgs(0, null, "*"));
            
        }
    }
}
