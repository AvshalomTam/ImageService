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
    public class LogModel
    {
        private List<Logs> logs;
        public Dictionary<int, IServiceCommands> commandDictionary;

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