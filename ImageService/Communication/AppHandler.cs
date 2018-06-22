using ImageService.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication
{
    class AppHandler : IClientHandler
    {
        private string output;
        private ILoggingService logger;

        public AppHandler(string output, ILoggingService logger)
        {
            this.output = output;
            this.logger = logger;
        }

        public void HandleClient(TcpClient tcpClient)
        {
            try
            {
                BinaryReader r = new BinaryReader(tcpClient.GetStream());
                new Task(() =>
                {
                    while (true)
                    {
                        byte[] size;

                        // getting the name of the pic
                        size = r.ReadBytes(4); // reading the length
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(size);    // reversing cause of the endianess
                        int i = BitConverter.ToInt32(size, 0);
                        byte[] b_name = r.ReadBytes(i);
                        string name = Encoding.ASCII.GetString(b_name);

                        // getting the pic itself
                        size = r.ReadBytes(4); // reading the length
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(size);    // reversing cause of the endianess
                        i = BitConverter.ToInt32(size, 0);

                        byte[] b_pic = r.ReadBytes(i);
                        TransferPic(b_pic, name);
                        tcpClient.Close();
                    }
                }).Start();
            }
            catch (Exception)
            {
                //logger.Log("client disconnected", Logging.Modal.MessageTypeEnum.INFO);
                //tcpClient.Close();
            }            
        }

        private void TransferPic(byte[] pic, string name)
        {
            string path = this.output + "\\" + name;
            MemoryStream ms = new MemoryStream(pic);
            Bitmap bm = new Bitmap(ms);
            bm.Save(path);
            bm.Dispose();                
        }
    }
}
