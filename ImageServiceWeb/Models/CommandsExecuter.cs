using ImageServiceWeb.Commands;
using ImageServiceWeb.Communication;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Models
{
    public abstract class CommandsExecuter
    {
        public Dictionary<int, IServiceCommands> commandDictionary;

        public void OnDataReceived(object sender, MessageEventArgs args)
        {
            JObject Jmessage = JObject.Parse(args.Message);
            int commandID = (int)Jmessage["command"];
            string commandArgs = (string)Jmessage["arguments"];

            if (commandDictionary.TryGetValue(commandID, out IServiceCommands command))
            {
                command.Execute(commandArgs);
            }
        }
    }
}