using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageServiceGUI.Communication;
using ImageService.Logging.Modal;
using Newtonsoft.Json.Linq;
using ImageServiceGUI.Enums;
using ImageServiceGUI.ViewModel;
using ImageServiceGUI.Converter;
using ImageServiceGUI.Commands;

namespace ImageServiceGUI.Model
{
    class LogModel : GuiModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogModel"/> class.
        /// </summary>
        /// <param name="vm">The vm.</param>
        public LogModel(IViewModel vm)
        {
            vm.Request += OnRequest;
            CommunicationSingleton.Instance.msgReceived += OnDataReceived;           
        }
    }
}
