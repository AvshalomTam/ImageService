using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication.Commands
{
    class DataCommand : IHandlerCommand
    {
        public object data;

        public DataCommand(object data)
        {
            this.data = data;
        }

        public string execute(string command)
        {
            return data.ToString();
        }
    }
}
