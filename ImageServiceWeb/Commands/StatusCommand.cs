using ImageServiceWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Commands
{
    public class StatusCommand : IServiceCommands
    {
        public HomePageModel m;

        public StatusCommand(HomePageModel m)
        {
            this.m = m;
        }

        public void Execute(string args)
        {
            this.m.status = args;
        }
    }
}