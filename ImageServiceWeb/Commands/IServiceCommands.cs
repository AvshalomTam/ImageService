using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ImageServiceWeb.Commands
{
    /// <summary>
    /// interface that represents a command that the modal get
    /// </summary>
    public interface IServiceCommands
    {
        void Execute(string args);          // The Function That will Execute The 
    }
}
