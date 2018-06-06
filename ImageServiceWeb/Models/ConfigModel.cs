using ImageServiceWeb.Commands;
using ImageServiceWeb.Communication;
using ImageServiceWeb.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ImageServiceWeb.Models
{
    public class ConfigModel
    {
        public Config configuration { get; set; }
        public Dictionary<int, IServiceCommands> commandDictionary;
        private Thread waitingThread;
        private bool requestedRemove;

        public ConfigModel()
        {
            this.requestedRemove = false;
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

            if (commandID == (int)CommandEnum.CloseCommand && this.requestedRemove)
            {
                this.waitingThread.Interrupt();
                this.requestedRemove = false;
            }                
        }

        public void RemoveHandler(string handler)
        {
            CommunicationSingleton.Instance.writeToService(new CommandMessage((int)CommandEnum.CloseCommand, handler).toJson());
            this.waitingThread = Thread.CurrentThread;
            this.requestedRemove = true;
            try
            {
                Thread.Sleep(-1);
            }
            catch { }
            
        }
    }
}