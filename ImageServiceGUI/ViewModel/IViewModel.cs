using ImageServiceGUI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.ViewModel
{
    interface IViewModel
    {
        // eventhandler to send requests to the model
        event EventHandler<CommandMessage> Request;

        // upon receiving data
        void OnDataReceived(object sender, CommandMessage args);
    }
}
