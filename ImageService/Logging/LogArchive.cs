using ImageService.Logging.Modal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Converter;
using ImageService.Infrastructure.Enums;
using Newtonsoft.Json;

namespace ImageService.Logging
{
    class LogArchive
    {
        public List<MessageRecievedEventArgs> logs;

        public LogArchive()
        {
            this.logs = new List<MessageRecievedEventArgs>();
        }

        public void OnLog(object sender, MessageRecievedEventArgs args)
        {           
            this.logs.Add(args);
        }

        public override string ToString()
        {
            JObject obj = new JObject();
            obj["command"] = (int)CommandEnum.LogCommand;
            List<string> logs = new List<string>();
            JObject obj2;
            foreach (MessageRecievedEventArgs log in this.logs)
            {
                obj2 = new JObject();
                obj2["type"] = TypeToString.getType(log.Type);
                obj2["message"] = log.Message;
                logs.Add(obj2.ToString());
            }
            obj["arguments"] = JsonConvert.SerializeObject(logs);

            return obj.ToString();
        }
    }
}
