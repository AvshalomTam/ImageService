using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Server;

namespace ImageService.Communication.Commands
{
    class RemoveHandlerCommand : IHandlerCommand
    {
        private ImageServer server;

        public RemoveHandlerCommand(ImageServer server)
        {
            this.server = server;
        }

        public string execute(string directory)
        {
            server.CloseHandler(directory);
            return "";
        }
    }
}
