using ImageServiceGUI.Converter;
using ImageServiceGUI.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Commands
{
    class LogCommand : IServiceCommands
    {
        public ILogViewModel vm;
        
        public LogCommand(ILogViewModel vm)
        {
            this.vm = vm;
        }

        /// <summary>
        /// Executes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Execute(string args)
        {
            ObservableCollection<string> logs = JsonConvert.DeserializeObject<ObservableCollection<string>>(args);
            foreach (string l in logs)
            {
                JObject Jlog = JObject.Parse(l);
                string type = (string)Jlog["type"];
                string message = (string)Jlog["message"];
                this.vm.OnLog(type, message);                
            }
        }
    }
}
