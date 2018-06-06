using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class RemoveHandlerArgs : EventArgs
    {
        public string Handler { get; set; }
    }
}