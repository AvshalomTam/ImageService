using ImageService.Infrastructure.Enums;
using ImageService.Modal.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.ImageService.Configuration
{
    class ConfigManager
    {
        public List<string> Handlers { get; set; }
        public string OutputDirectory { get; set; }
        public string SourceName { get; set; }
        public string LogName { get; set; }
        public int ThumbnailSize { get; set; }

        public ConfigManager()
        {
            this.Handlers = new List<string>();
            foreach (string handler in ConfigurationManager.AppSettings["Handler"].Split(';'))
                this.Handlers.Add(handler);
            this.OutputDirectory = ConfigurationManager.AppSettings["OutputDir"];
            this.SourceName = ConfigurationManager.AppSettings["SourceName"];
            this.LogName = ConfigurationManager.AppSettings["LogName"];
            if (Int32.TryParse(ConfigurationManager.AppSettings["ThumbnailSize"], out int thumbnailSize))
                this.ThumbnailSize = thumbnailSize;
            else
                this.ThumbnailSize = 120;            
        }

        /// <summary>
        /// Called when [handler remove].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DirectoryCloseEventArgs"/> instance containing the event data.</param>
        public void OnHandlerRemove(object sender, DirectoryCloseEventArgs args)
        {
            this.Handlers.Remove(args.DirectoryPath);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            JObject obj = new JObject();
            obj["command"] = (int)CommandEnum.ConfigCommand;
            JObject obj2 = new JObject();
            obj2["Output Directory"] = OutputDirectory;
            obj2["Source Name"] = SourceName;
            obj2["Log Name"] = LogName;
            obj2["Thumbnail Size"] = ThumbnailSize;
            obj2["handlers"] = JsonConvert.SerializeObject(Handlers);
            obj["arguments"] = obj2.ToString();
            return obj.ToString();
        }
    }
}
