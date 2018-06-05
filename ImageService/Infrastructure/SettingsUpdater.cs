using ImageService.Communication;
using ImageService.Infrastructure.Enums;
using ImageService.Logging.Modal;
using ImageService.Modal.Event;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Infrastructure
{
    class SettingsUpdater
    {
        public EventHandler<TransferEventArgs> settingsUpdate;

        /// <summary>
        /// Called when [handler removed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DirectoryCloseEventArgs"/> instance containing the event data.</param>
        public void OnHandlerRemoved(object sender, DirectoryCloseEventArgs args)
        {
            JObject obj = new JObject();
            obj["command"] = (int)CommandEnum.CloseCommand;
            obj["arguments"] = args.DirectoryPath;

            settingsUpdate?.Invoke(this, new TransferEventArgs(obj.ToString()));
        }
    }
}
