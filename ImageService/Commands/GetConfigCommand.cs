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
        public GetConfigCommand()
        {
            AppConfig = new AppConfig();
        }
        public string Execute(string[] args, out bool result)
        {
            result = true;
            return AppConfig.toJSON();
        }
    }
}
