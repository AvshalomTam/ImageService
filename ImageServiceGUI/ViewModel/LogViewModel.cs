using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageServiceGUI.Model;
using ImageService.Logging.Modal;
using System.Drawing;
using ImageServiceGUI.Converter;
using ImageServiceGUI.Commands;
using ImageServiceGUI.Communication;
using ImageServiceGUI.Enums;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace ImageServiceGUI.ViewModel
{
    class Log
    {
        public string Type { get; set; }

        public string Message { get; set; }

        public string Color { get; set; }
    }

    class LogViewModel : ILogViewModel
    {
        public ObservableCollection<Log> List { get; set; }
        public Dictionary<int, IServiceCommands> commandDictionary;
        public event EventHandler<CommandMessage> Request;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogViewModel"/> class.
        /// </summary>
        public LogViewModel()
        {
            List = new ObservableCollection<Log>();
            // in order to be able to make changes to the list from other threads
            BindingOperations.EnableCollectionSynchronization(List, List);
            new LogModel(this).dataReceive += OnDataReceived;

            commandDictionary = new Dictionary<int, IServiceCommands>()
            {
                { (int)CommandEnum.LogCommand, new LogCommand(this) }
            };

            Request?.Invoke(this, new CommandMessage((int)CommandEnum.LogCommand));
        }

        /// <summary>
        /// Called when [data received].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        public void OnDataReceived(object sender, CommandMessage args)
        {
            if (commandDictionary.TryGetValue(args.commandID, out IServiceCommands command))
            {
                //add a task to handle the command
                command.Execute(args.commandArgs);
            }
        }

        /// <summary>
        /// Called when [log].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        public void OnLog(string type, string message)
        {
            List.Insert(0, new Log() { Type = type, Message = message, Color = TypeToBrushConverter.getBrush(type) });
        }
    }
}
