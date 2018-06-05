using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageServiceGUI.Commands;
using ImageServiceGUI.Communication;
using ImageServiceGUI.ViewModel;
using Newtonsoft.Json.Linq;

namespace ImageServiceGUI.Model
{
    class GuiModel : IModel
    {
        public event EventHandler<CommandMessage> dataReceive;

        public void OnDataReceived(object sender, MessageEventArgs args)
        {
            JObject Jmessage = JObject.Parse(args.Message);
            dataReceive?.Invoke(this, new CommandMessage((int)Jmessage["command"], (string)Jmessage["arguments"]));
        }

        public void OnRequest(object sender, CommandMessage message)
        {
            JObject commandObj = new JObject();
            commandObj["command"] = (int)message.commandID;
            commandObj["arguments"] = message.commandArgs;
            CommunicationSingleton.Instance.writeToService(commandObj.ToString());
        }
    }
}
