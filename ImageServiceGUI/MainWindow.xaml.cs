using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageServiceGUI.Communication;
using ImageServiceGUI.ViewModel;
using ImageServiceGUI.Views;

namespace ImageServiceGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(this);
        }
        
        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            window.Title = "closing connection";
            CommunicationSingleton.Instance.closeService();
            Thread.Sleep(2000);
        }
    }
}
