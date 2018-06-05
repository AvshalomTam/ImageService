using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Infrastructure
{
    class TransferEventArgs : EventArgs
    {
        public string message { get; set; }

        public TransferEventArgs(string message)
        {
            this.message = message;
        }
    }
}
