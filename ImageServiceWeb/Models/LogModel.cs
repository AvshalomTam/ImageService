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
        private List<Logs> logs;
        
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

        public List<Logs> getList(string type = "")
        {
            if (string.Equals(type, ""))
                return logs;

            List <Logs> filtered = new List<Logs>();
            foreach (Logs l in logs)
            {
                if (string.Equals(l.Type, type))
                    filtered.Add(l);
            }
            
            return filtered;
        }
    }
}