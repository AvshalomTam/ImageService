using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging.Modal
{
    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageTypeEnum Type { get; set; }
        public string Message { get; set; }

        public MessageRecievedEventArgs(string message, MessageTypeEnum type)
        {
            Type = type;
            Message = message;
        }
    }
}
