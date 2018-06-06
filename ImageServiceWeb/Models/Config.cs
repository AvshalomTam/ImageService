using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class Config
    {
        private Thread sleepingThread;
        public string outputDir = null;

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Output Directory")]
        public string OutputDir
        {
            get
            {
                if (this.outputDir != null)
                    return this.outputDir;
                
                this.sleepingThread = Thread.CurrentThread;
                try
                {
                    Thread.Sleep(-1);
                }
                catch { }                
                return outputDir;
            }
            set
            {
                outputDir = value;
                this.sleepingThread.Interrupt();
            }
        }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Source Name")]
        public string SrcName { get; set; }

        [Required]
        [Display(Name = "Log Name")]
        public string LogName { get; set; }

        [Required]
        [Display(Name = "Thumbnail Size")]
        public string ThumbnailSize { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Handlers")]
        public List<string> Handlers { get; set; }

        public Config()
        {
            this.Handlers = new List<string>();
            this.sleepingThread = Thread.CurrentThread; ;
        }
    }
}