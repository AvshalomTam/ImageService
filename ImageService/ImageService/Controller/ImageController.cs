using ImageService.Commands;
using ImageService.Modal;
using ImageService.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    /// <summary>
    /// class that represents the controller tha controls the connection between 
    /// Directory handlers and the ImageModal
    /// </summary>
    public class ImageController : IImageController
    {
        private IImageServiceModal m_modal;                      // The Modal Object
        private Dictionary<int, ICommand> commands;

        public ImageController(IImageServiceModal modal)
        {
            m_modal = modal;                    // Storing the Modal Of The System
            commands = new Dictionary<int, ICommand>()
            {
                // For Now will contain NEW_FILE_COMMAND
                {(int)CommandEnum.NewFileCommand, new NewFileCommand(modal) }
            };
        }

        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            if (commands.TryGetValue(commandID, out ICommand command))
            {
                //add a task to handle the command
                Task<Tuple<string, bool>> task = new Task<Tuple<string, bool>>(() =>
                {
                    string msg = command.Execute(args, out bool result);
                    return Tuple.Create(msg, result);
                });
                task.Start();
                resultSuccesful = task.Result.Item2;
                return task.Result.Item1;
            }
            else
            {
                resultSuccesful = false;
                return "Command not found!";
            }
        }
    }
}
