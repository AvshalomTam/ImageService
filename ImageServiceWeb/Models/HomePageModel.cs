﻿using ImageServiceWeb.Commands;
using ImageServiceWeb.Communication;
using ImageServiceWeb.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class HomePageModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Status")]
        public string status { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Num of pics")]
        public int numOfPics { get; set; }

        public Dictionary<int, IServiceCommands> commandDictionary;
        public List<Student> students { get; set; }

        public HomePageModel()
        {
            this.students = StudentModel.GetStudentList(@"App_Data/StudentsConfig.xml");
            this.numOfPics = 0;
            this.status = "Waiting for connection...";

            // in order to get messages
            CommunicationSingleton.Instance.msgReceived += OnDataReceived;

            commandDictionary = new Dictionary<int, IServiceCommands>()
            {
                { (int)CommandEnum.StatusCommand, new StatusCommand(this) }
            };

            // get the status of the server
            CommunicationSingleton.Instance.writeToService(new CommandMessage((int)CommandEnum.StatusCommand).toJson());
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