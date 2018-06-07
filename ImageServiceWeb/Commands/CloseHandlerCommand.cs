using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace ImageServiceWeb.Commands
{
    public class CloseHandlerCommand : IServiceCommands
    {
        private List<string> handlers;
        private Thread waitingThread;

        public CloseHandlerCommand(List<string> handlers, Thread t)
        {
            this.handlers = handlers;
            this.waitingThread = t;
        }

        public void Execute(string args)
        {
            this.handlers.Remove(args);
            this.waitingThread.Interrupt();
        }
    }
}