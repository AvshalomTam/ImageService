using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication
{
    class Client
    {
        public TcpClient tcpClient { get; set; }
        public NetworkStream Stream { get; set; }
        public BinaryWriter Writer { get; set; }
        public BinaryReader Reader { get; set; }
        
        public Client(TcpClient client)
        {
            this.tcpClient = client;
            this.Stream = client.GetStream();
            this.Writer = new BinaryWriter(Stream);
            this.Reader = new BinaryReader(Stream);
        }
    }
}
