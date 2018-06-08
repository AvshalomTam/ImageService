using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class Photo
    {
        private static Regex r = new Regex(":");

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Pic name")]
        public string PicName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Date Taken")]
        public string DateTaken { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Path")]
        public string FullPath { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Path")]
        public string RelativePath { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Path")]
        public string t_FullPath { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Path")]
        public string t_RelativePath { get; set; }

        /**
         * path: path to thumbnail pic
         */
        public Photo(string path)
        {
            // full path for the thumbnail
            this.t_FullPath = path;
                        
            string[] seperator = new string[] { @"\output\Thumbnails" };
            string[] parts1 = this.t_FullPath.Split(seperator, StringSplitOptions.None);
            seperator = new string[] { @"\Thumbnails" };
            string[] parts2 = this.t_FullPath.Split(seperator, StringSplitOptions.None); ;

            // full path for the real pic
            this.FullPath = parts2[0] + parts1[1];            
            // relative path for the real pic
            this.RelativePath = @"..\output" + parts1[1];
            // relative path for the thumbnail
            this.t_RelativePath = @"..\output\Thumbnails" + parts2[1];

            this.PicName = this.FullPath.Substring(this.FullPath.LastIndexOf('\\') + 1);

            DateTime timeCreated = GetDateTakenFromImage();
            this.DateTaken = timeCreated.Month.ToString() + @"/" + timeCreated.Year.ToString();
        }

        private DateTime GetDateTakenFromImage()
        {
            using (FileStream fs = new FileStream(this.FullPath, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = null;
                try
                {
                    propItem = myImage.GetPropertyItem(36867);
                }
                catch { }
                if (propItem != null)
                {
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    return DateTime.Parse(dateTaken);
                }
                else
                    return new FileInfo(this.FullPath).LastWriteTime;
            }
        }
    }
}