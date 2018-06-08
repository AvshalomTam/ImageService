using ImageServiceWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace ImageServiceWeb.Commands
{
    public class CloseHandlerCommand : IServiceCommands
    {
        private ConfigModel m;

        public CloseHandlerCommand(ConfigModel m)
        {
            this.m = m;
        }

        public void Execute(string args)
        {
            this.m.configuration.Handlers.Remove(args);
            this.m.waitingThread.Interrupt();
        }
    }
}