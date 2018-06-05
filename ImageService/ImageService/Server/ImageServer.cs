using ImageService.Controller;
using ImageService.Logging;
using ImageService.Modal.Event;
using ImageService.Controller.Handlers;
using System;
using ImageService.Infrastructure.Enums;
using ImageService.Logging.Modal;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ImageService.ImageService.Configuration;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logger;
        #endregion

        #region Properties
        // The event that notifies about a new Command being received
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;

        // The event that notifies that a handler was removed
        public event EventHandler<DirectoryCloseEventArgs> RemoveHandler;
        #endregion

        public ImageServer(IImageController controller, ILoggingService logger, List<string> paths)
        {
            m_controller = controller;
            m_logger = logger;

            foreach (string path in paths)
            {
                CreateHandler(path, out bool success);
                if (success)
                {
                    m_logger.Log($"Handler created : {path}", MessageTypeEnum.INFO);
                }
                else
                {
                    m_logger.Log($"Failed to create handler : {path}", MessageTypeEnum.FAIL);
                    RemoveHandler?.Invoke(this, new DirectoryCloseEventArgs(path, "Failed to create handler"));
                }
            }
        }
        
        /// <summary>
        /// The function creates a handler to be listening to a folder given its path
        /// </summary>
        /// <param name="path">The path of the folder needed to be listened to</param>
        public void CreateHandler(string path, out bool success)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                m_logger.Log($"Created new directory: {path}", MessageTypeEnum.INFO);
            }
            IDirectoryHandler handler = new DirectoyHandler(m_controller, m_logger);
            CommandRecieved += handler.OnCommandRecieved;   //that's how the server will send commands
            handler.DirectoryClose += OnHandlerClose;   //that's how the handler will send (close) commands to server
            handler.StartHandleDirectory(path);
            success = true;
        }

        /// <summary>
        /// the server sends an event to all handlers that he's closing
        /// </summary>
        public void ServerClose()
        {
            // server will invoke an event to all handlers (by using "*") that he's closing
            CommandRecieved?.Invoke(this, new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, "*"));
        }

        /// <summary>
        /// the server got notified that a handler got closed
        /// </summary>
        /// <param name="sender">the sender of the event</param>
        /// <param name="e">the arguments of the event</param>
        public void OnHandlerClose(object sender, DirectoryCloseEventArgs e)
        {
            CommandRecieved -= ((IDirectoryHandler)sender).OnCommandRecieved;
            //send a log
            m_logger.Log(e.Message, MessageTypeEnum.INFO);
            //notify that handler was closed
            RemoveHandler?.Invoke(this, e);
        }

        public void CloseHandler(string directory)
        {
            // server invokes a message to all handlers to close directory
            CommandRecieved?.Invoke(this, new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, directory));
        }
    }
}
