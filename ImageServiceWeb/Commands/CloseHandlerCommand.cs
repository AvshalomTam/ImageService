using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Commands
{
    public class CloseHandlerCommand : IServiceCommands
    {
        private List<string> handlers;

        public CloseHandlerCommand(List<string> handlers)
        {
            this.handlers = handlers;
        }

        public void Execute(string args)
        {
            this.handlers.Remove(args);
        }
    }
}