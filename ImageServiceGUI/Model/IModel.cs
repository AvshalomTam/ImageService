using ImageServiceGUI.Commands;
using ImageServiceGUI.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Model
{
    interface IModel
    {
        // this eventhandler sends messages to viewmodel
        event EventHandler<CommandMessage> dataReceive;

        void OnRequest(object sender, CommandMessage message);
        void OnDataReceived(object sender, MessageEventArgs args);
    }
}
