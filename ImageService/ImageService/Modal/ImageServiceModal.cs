using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ImageService.Modal
{
    /// <summary>
    /// class that do the operations on the images as creating folders, coping files and create thumbnails
    /// </summary>
    public class ImageServiceModal : IImageServiceModal
    {
        #region Members
        // The Output Folder
        private string m_OutputFolder;
        // The Size Of The Thumbnail Size
        private int m_thumbnailSize;
        private static Regex r = new Regex(":");
        #endregion

        /// <summary>
        /// the constructor
        /// </summary>
        /// <param name= path> the path of the OutputDir </param>
        /// <param name= size> the size of the thumbnail we create </param>
        public ImageServiceModal(string path, int size)
        {
            m_OutputFolder = path;
            m_thumbnailSize = size;            
        }

        /// <summary>
        /// adds an image to the output directory (as thumbnail) 
        /// </summary>
        /// <param name= path> path to the file we want to copy </param>
        /// <param name= result> an indication if operation was successful or not </param>
        /// <return> returns the path of the new file if succeeded or and error message if failed </return>
        public string AddFile(string path, out bool result)
        {
            try
            {
                //checks if the image is in the path given
                if (File.Exists(path))
                {
                    string newPath;
                    string thumbNewPath;

                    // create all directories needed
                    CreateDirectories(path, out newPath, out thumbNewPath);

                    //extract the name of the image + append it to the new paths
                    newPath = newPath + path.Substring(path.LastIndexOf("\\"));
                    thumbNewPath = thumbNewPath + path.Substring(path.LastIndexOf("\\"));

                    newPath = checkForExisting(newPath);
                    thumbNewPath = checkForExisting(thumbNewPath);

                    // create and save images in their destination folders
                    MoveImages(path, newPath, thumbNewPath);
                    //returns positive result
                    result = true;
                    return "Image successfully moved : " + newPath;
                }
                // if file doesn't exist in the path
                result = false;
                return "Image not exist : " + path;
            }
            catch (Exception e)
            {
                result = false;
                return e.Message;
            }
                //throw new NotImplementedException();
        }

        /// <summary>
        /// creating the directories needed by year and month
        /// </summary>
        /// <param name= path> the path to the file we want to copy </param>
        private void CreateDirectories(string oldPath, out string newPath, out string thumbNewPath)
        {
            //create output directory (if doesn't ealready xists)
            DirectoryInfo dirInfo = Directory.CreateDirectory(this.m_OutputFolder);
            //create as hidden directory
            dirInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            //extract images creation year and month
            DateTime timeCreated = GetDateTakenFromImage(oldPath);
            int year = timeCreated.Year;
            int month = timeCreated.Month;

            //make helping path
            string yearAndMonthPath = year.ToString() + "\\" + getMonth(month);
            //make new path to output folder
            newPath = this.m_OutputFolder + "\\" + yearAndMonthPath;
            //make new path to thumbnail folder
            thumbNewPath = this.m_OutputFolder + "\\Thumbnails" + "\\" + yearAndMonthPath;

            //create directories for images and thumbnails
            Directory.CreateDirectory(newPath);
            Directory.CreateDirectory(thumbNewPath);
        }

        /// <summary>
        /// Returns the name of the month according it's number
        /// </summary>
        /// <param name="m">month number</param>
        /// <returns>month name</returns>
        private string getMonth(int m)
        {
            switch (m)
            {
                case (1):
                    return "January";
                case (2):
                    return "February";
                case (3):
                    return "March";
                case (4):
                    return "April";
                case (5):
                    return "May";
                case (6):
                    return "June";
                case (7):
                    return "July";
                case (8):
                    return "August";
                case (9):
                    return "September";
                case (10):
                    return "October";
                case (11):
                    return "November";
                case (12):
                    return "December";
                default:
                    return "No Date";
            }
        }

        private string checkForExisting(string path)
        {
            int count = 1;
            string fileNameOnly = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);
            string directoryPath = Path.GetDirectoryName(path);
            
            while (File.Exists(path))
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                path = Path.Combine(directoryPath, tempFileName + extension);
            }
            return path;
        }

        /// <summary>
        /// creates the images and thumbnails in the directories we created
        /// </summary>  
        /// <param name= path> the path to the file we want tot copy </param>
        /// <param name= newPath> the path to the destinated folder </param>
        /// <param name= thumbNewPath> the path to the destination folder of the thumbnail we create </param>
        private void MoveImages(string oldPath, string newPath, string thumbNewPath)
        {
            //save image as a thumbnail
            Image image = Image.FromFile(oldPath);
            Image thumb = image.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize, () => false, IntPtr.Zero);

            //save the thumbnail in the correct directory in the output dir
            thumb.Save(thumbNewPath);
            //we dont need the images anymore so release them 
            image.Dispose();
            thumb.Dispose();
            //save the image in the correct place inside the output directory
            Directory.Move(oldPath, newPath);
        }

        //retrieves the datetime WITHOUT loading the whole image
        public static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
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
                    return new FileInfo(path).LastWriteTime;
            }
        }
    }
}