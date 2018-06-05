using ImageServiceGUI.Commands;
using ImageServiceGUI.Communication;
using ImageServiceGUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.ViewModel
{
    interface ISettingsViewModel : IViewModel
    {
        string OutputDir { get; set; }
        string SrcName { get; set; }
        string LogName { get; set; }
        string ThumbSize { get; set; }
        ObservableCollection<string> List { get; set; }

        // send a request to remove handler
        void OnRemove();
    }
}