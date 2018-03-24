using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {
        #region Members
        private static Regex r = new Regex(":");
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size 
        #endregion

        public ImageServiceModal(string outputFolder, int thumbnailSize)
        {
            m_OutputFolder = outputFolder;
            m_thumbnailSize = thumbnailSize;
        }

        public string AddFile(string path, out bool result)
        {
            // check if a folder exists
            Directory.CreateDirectory(m_OutputFolder);
            try
            {
                DateTime dateTime = GetDateTakenFromImage(path);
                CreateImageFolder(dateTime);
                result = MoveFile(path, dateTime);
            }
            catch(ArgumentException e)
            {
                // handle error, currently error msg only
                result = false;
                return "Error. Failed moving the file.";
            }
            return null;
        }


        /// <summary>
        /// Copies a file from path to the appropriate outputDir path, i.e: outputDir\year\month.
        /// </summary>
        /// <param name="path">a file path.</param>
        /// <param name="dateTime">a DateTime object corresponding to the file.</param>
        /// <returns>true if successful.</returns>
        private bool MoveFile(string path, DateTime dateTime) {
            string fileName = Path.GetFileName(path);
            string outputPath = Path.Combine(ParseMonthYearPath(dateTime), fileName);
            try
            {
                File.Copy(path, outputPath);
                return true;
            }
            catch(IOException e)
            {
                return HandleDuplicateFile(path, outputPath);
            }
        }


        /// <summary>
        /// Moves a file from path to outputPath with duplications. For example, filename.txt will become filename(1).txt and etc.
        /// </summary>
        /// <param name="path">a path to move a file from.</param>
        /// <param name="outputPath">a path to move the file to.</param>
        /// <returns>true if successful.</returns>
        private bool HandleDuplicateFile(string path, string outputPath)
        {
            string directory = Path.GetDirectoryName(outputPath);
            string fileName = Path.GetFileNameWithoutExtension(outputPath) + "({0})";
            string extension = Path.GetExtension(outputPath);
            int duplicatesAllowed = 10;
            for(int i = 1; i < duplicatesAllowed; ++i)
            {
                string filePath = Path.Combine(directory, string.Format(fileName, i) + extension);
                if(!File.Exists(filePath)) {
                    File.Copy(path, filePath);
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Creates a folder in the outputDir according to the year and month. Path is m_outputDir\month\year.
        /// </summary>
        /// <param name="dateTime">a DateTime to extract year and month info from.</param>
        private void CreateImageFolder(DateTime dateTime)
        {
            string path = ParseMonthYearPath(dateTime);
            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Parses a m_outPutDir\month\year from the DateTime.
        /// </summary>
        /// <param name="dateTime">a DateTime object to parse Month and Year Properties from.</param>
        /// <returns>a string represntation of the path.</returns>
        private string ParseMonthYearPath(DateTime dateTime)
        {
            string yearPath = Path.Combine(m_OutputFolder, dateTime.Year.ToString());
            return Path.Combine(yearPath, dateTime.ToString("MMMM"));
        }

        /// <summary>
        /// Creates a date time object from the ImageTaken Property.
        /// </summary>
        /// <param name="path">a path to the image file.</param>
        /// <returns>a DateTime object corresponding to the ImageTaken Property.</returns>
        private DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                  PropertyItem propItem = myImage.GetPropertyItem(36867);
                  string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                  return DateTime.Parse(dateTaken);
            }
        }

    }
}
