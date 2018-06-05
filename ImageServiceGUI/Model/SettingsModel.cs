using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageServiceGUI.ViewModel;
using ImageServiceGUI.Communication;
using Newtonsoft.Json.Linq;
using ImageServiceGUI.Enums;
using ImageServiceGUI.Commands;

namespace ImageServiceGUI.Model
{
    class SettingsModel : GuiModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsModel"/> class.
        /// </summary>
        /// <param name="settingsVM">The settings vm.</param>
        public SettingsModel(IViewModel settingsVM)
        {
            settingsVM.Request += OnRequest;
            CommunicationSingleton.Instance.msgReceived += OnDataReceived;            
        }    
    }
}
