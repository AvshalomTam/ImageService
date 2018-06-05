using ImageServiceGUI.Model;
using ImageServiceGUI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace ImageServiceGUI.Commands
{
    class ConfigCommand : IServiceCommands
    {
        public ISettingsViewModel vm;

        public ConfigCommand(ISettingsViewModel vm)
        {
            this.vm = vm;
        }

        /// <summary>
        /// Executes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Execute(string args)
        {
            JObject Jconfig = JObject.Parse(args);
            this.vm.OutputDir = (string)Jconfig["Output Directory"];
            this.vm.SrcName = (string)Jconfig["Source Name"];
            this.vm.LogName = (string)Jconfig["Log Name"];
            this.vm.ThumbSize = (string)Jconfig["Thumbnail Size"];
            ObservableCollection<string> handlers = JsonConvert.DeserializeObject<ObservableCollection<string>>((string)Jconfig["handlers"]);
            foreach (string h in handlers)
                this.vm.List.Add(h);
        }
    }
}
