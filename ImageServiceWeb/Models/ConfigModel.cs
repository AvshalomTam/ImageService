using ImageServiceWeb.Commands;
using ImageServiceWeb.Communication;
using ImageServiceWeb.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class ConfigModel
    {
        public Config configuration { get; set; }
        public Dictionary<int, IServiceCommands> commandDictionary;

        public ConfigModel()
        {
            this.configuration = new Config();
            this.commandDictionary = new Dictionary<int, IServiceCommands>()
            {
                { (int)CommandEnum.ConfigCommand, new ConfigCommand(this.configuration)},
                { (int)CommandEnum.CloseCommand, new CloseHandlerCommand(this.configuration.Handlers) }
            };

            // in order to get messages
            CommunicationSingleton.Instance.msgReceived += OnDataReceived;

            CommunicationSingleton.Instance.writeToService(new CommandMessage((int)CommandEnum.ConfigCommand).toJson());
        }

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