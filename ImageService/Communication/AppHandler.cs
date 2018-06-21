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
    class AppHandler
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
            logger.Log("Handling client", Logging.Modal.MessageTypeEnum.INFO);
            try
            {
                NetworkStream stream = tcpClient.GetStream();
                new Task(() =>
                {
                    byte[] size = new byte[4];
                    
                    // getting the name of the pic
                    stream.Read(size, 0, size.Length);
                    Array.Reverse(size);
                    int i = BitConverter.ToInt32(size, 0);
                    byte[] b_name = new byte[i];
                    stream.Read(b_name, 0, i);
                    string name = Encoding.ASCII.GetString(b_name);
                    logger.Log(i.ToString() + " " + name, Logging.Modal.MessageTypeEnum.INFO);

                    // getting the pic itself
                    size = new byte[4];
                    stream.Read(size, 0, size.Length);
                    Array.Reverse(size);
                    i = BitConverter.ToInt32(size, 0);                
                    byte[] pic = new byte[i];
                    logger.Log(i.ToString(), Logging.Modal.MessageTypeEnum.INFO);
                    int count = 0;

                    do
                    {
                        count += stream.Read(pic, count, i);
                    }
                    while (count < i);
                    logger.Log(i.ToString() + " " + pic.ToString(), Logging.Modal.MessageTypeEnum.INFO);

                    //TransferPic(pic);
                    tcpClient.Close();

                }).Start();
            }
            catch (Exception) { }            
        }

        private void TransferPic(byte[] pic)
        {
            using (var ms = new MemoryStream(pic))
            {
                using (Image img = Image.FromStream(ms))
                {
                    string path = this.output + "//" + img.GetPropertyItem(269).Value;
                    img.Save(path);
                }
            }
        }
    }
}
