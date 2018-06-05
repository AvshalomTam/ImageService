using ImageServiceGUI.Commands;
using ImageServiceGUI.Communication;
using ImageServiceGUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.ViewModel
{
    interface ILogViewModel : IViewModel
    {
        // upon receiving a log
        void OnLog(string type, string message);
    }
}