using ImageServiceWeb.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Commands
{
    public class ConfigCommand : IServiceCommands
    {
        private Config configuration;

        public ConfigCommand(Config configuration)
        {
            this.configuration = configuration;
        }

        public void Execute(string args)
        {
            JObject Jconfig = JObject.Parse(args);
            this.configuration.OutputDir = (string)Jconfig["Output Directory"];
            this.configuration.SrcName = (string)Jconfig["Source Name"];
            this.configuration.LogName = (string)Jconfig["Log Name"];
            this.configuration.ThumbnailSize = (string)Jconfig["Thumbnail Size"];
            List<string> handlers = JsonConvert.DeserializeObject<List<string>>((string)Jconfig["handlers"]);
            foreach (string h in handlers)
                this.configuration.Handlers.Add(h);
        }
    }
}