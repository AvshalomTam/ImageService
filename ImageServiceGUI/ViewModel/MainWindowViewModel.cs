using ImageServiceGUI.Communication;
using ImageServiceGUI.Views;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ImageServiceGUI.ViewModel
{
    class MainWindowViewModel
    {
        public ObservableCollection<SettingsView> Settings { get; set; }
        public ObservableCollection<LogView> Logs { get; set; }
        public string Background { get; set; }
        
        public MainWindowViewModel(MainWindow mainWindow)
        {
            mainWindow.Closed += OnClose;
            if (CommunicationSingleton.Instance.connectToService() != -1)
            {
                this.Settings = new ObservableCollection<SettingsView>();
                this.Settings.Add(new SettingsView());
                this.Logs = new ObservableCollection<LogView>();
                this.Logs.Add(new LogView());
                Background = "White";
            }
            else
            {
                Background = "Gray";
            }
        }

        public void OnClose(Object sender, EventArgs e)
        {
            CommunicationSingleton.Instance.closeService();
        }
    }
}
