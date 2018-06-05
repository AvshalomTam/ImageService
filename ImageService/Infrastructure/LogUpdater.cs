using ImageService.Communication;
using ImageService.Converter;
using ImageService.Infrastructure.Enums;
using ImageService.Logging.Modal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Infrastructure
{
    class LogUpdater
    {
        public EventHandler<TransferEventArgs> logUpdate;

        /// <summary>
        /// Called when [log].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MessageRecievedEventArgs"/> instance containing the event data.</param>
        public void OnLog(object sender, MessageRecievedEventArgs e)
        {
            JObject obj = new JObject();
            obj["command"] = (int)CommandEnum.LogCommand;
            JObject obj2 = new JObject();
            List<string> log = new List<string>();
            obj2["type"] = TypeToString.getType(e.Type);
            obj2["message"] = e.Message;
            log.Add(obj2.ToString());
            obj["arguments"] = JsonConvert.SerializeObject(log);

            logUpdate?.Invoke(this, new TransferEventArgs(obj.ToString()));
        }
    }
}
