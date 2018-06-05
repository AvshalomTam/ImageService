using ImageServiceGUI.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Commands
{
    class CloseHandlerCommand : IServiceCommands
    {
        public ObservableCollection<string> list;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseHandlerCommand"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        public CloseHandlerCommand(ObservableCollection<string> list)
        {
            this.list = list;
        }

        /// <summary>
        /// Executes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Execute(string args)
        {
            this.list.Remove(args);
        }
    }
}
