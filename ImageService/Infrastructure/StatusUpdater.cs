using ImageService.Infrastructure.Enums;
using ImageService.Logging.Modal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Infrastructure
{
    class StatusUpdater
    {
        public EventHandler<TransferEventArgs> statusUpdate;
        private string status;

        public StatusUpdater(string status = "Building service")
        {
            this.status = status;
        }

        public void OnStatus(string status)
        {
            this.status = status;
            JObject obj = new JObject();
            obj["command"] = (int)CommandEnum.StatusCommand;
            obj["arguments"] = status;

            statusUpdate?.Invoke(this, new TransferEventArgs(obj.ToString()));
        }

        public override string ToString()
        {
            JObject obj = new JObject();
            obj["command"] = (int)CommandEnum.StatusCommand;
            obj["arguments"] = this.status;
            return obj.ToString();
        }
    }
}
