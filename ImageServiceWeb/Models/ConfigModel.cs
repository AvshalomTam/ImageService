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
    public class ConfigModel : CommandsExecuter
    {
        public Config configuration { get; set; }
        public Thread waitingThread;
        public bool isSleeping;
        public string handler { get; set; }
        
        public ConfigModel()
        {
            this.handler = null;
            this.waitingThread = Thread.CurrentThread;
            this.configuration = new Config();
            this.commandDictionary = new Dictionary<int, IServiceCommands>()
            {
                { (int)CommandEnum.ConfigCommand, new ConfigCommand(this.configuration)},
                { (int)CommandEnum.CloseCommand, new CloseHandlerCommand(this) }
            };

            // in order to get messages
            CommunicationSingleton.Instance.msgReceived += OnDataReceived;

            CommunicationSingleton.Instance.writeToService(new CommandMessage((int)CommandEnum.ConfigCommand).toJson());
        }

        public void RemoveHandler(string handler)
        {
            CommunicationSingleton.Instance.writeToService(new CommandMessage((int)CommandEnum.CloseCommand, handler).toJson());
            try
            {
                this.waitingThread = Thread.CurrentThread;
                this.handler = handler;
                Thread.Sleep(-1);
            }
            catch { }
            
        }
    }
}