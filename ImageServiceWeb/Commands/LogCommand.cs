using ImageServiceWeb.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceWeb.Commands
{
    class LogCommand : IServiceCommands
    {
        public List<Logs> logs;
        
        public LogCommand(List<Logs> l)
        {
            this.logs = l;
        }

        /// <summary>
        /// Executes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Execute(string args)
        {
            List<string> logs = JsonConvert.DeserializeObject<List<string>>(args);
            foreach (string l in logs)
            {
                JObject Jlog = JObject.Parse(l);
                string type = (string)Jlog["type"];
                string message = (string)Jlog["message"];
                this.logs.Add(new Logs() { Type = type, Log = message });                
            }
        }
    }
}
