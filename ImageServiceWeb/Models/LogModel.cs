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
    public class LogModel : CommandsExecuter
    {
        public List<Logs> logs { get; set; }
        
        public LogModel()
        {
            logs = new List<Logs>();
            
            // in order to get messages
            CommunicationSingleton.Instance.msgReceived += OnDataReceived;

            commandDictionary = new Dictionary<int, IServiceCommands>()
            {
                { (int)CommandEnum.LogCommand, new LogCommand(this.logs) }
            };

            // get all the old logs
            CommunicationSingleton.Instance.writeToService(new CommandMessage((int)CommandEnum.LogCommand).toJson());            
        }
    }
}