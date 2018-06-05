using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceWeb.Commands
{
    class CommandMessage : EventArgs
    {
        public int commandID { get; set; }
        public string commandArgs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMessage"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="args">The arguments.</param>
        public CommandMessage (int id, string args = "")
        {
            this.commandID = id;
            this.commandArgs = args;
        }

        public string toJson ()
        {
            JObject commandObj = new JObject();
            commandObj["command"] = commandID;
            commandObj["arguments"] = commandArgs;
            return commandObj.ToString();
        }
    }
}
