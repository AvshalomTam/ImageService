using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class PhotosModel
    {
        public List<Photo> PhotoList {
            get
            {
                List<Photo> list = new List<Photo>();
                try
                {
                    string[] pics = Directory.GetFiles(this.configuration.OutputDir + @"\Thumbnails", "*.*", SearchOption.AllDirectories);
                    for (int i = 0; i < pics.Length; i++)
                    {
                        list.Add(new Photo(pics[i]));
                    }
                }
                catch { }
                return list;
            }
        }
        private Config configuration;

        public PhotosModel(Config c)
        {
            this.configuration = c;
        }

        public void DeletePic(string path)
        {
            path = "~" + path.Substring(path.IndexOf('\\'));

            var filePath = System.Web.Hosting.HostingEnvironment.MapPath(path);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}