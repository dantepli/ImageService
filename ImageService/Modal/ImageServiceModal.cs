using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {
        #region Members
        private static Regex r = new Regex(":");
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size 
        #endregion


        /// <summary>
        /// C'tor.
        /// </summary>
        /// <param name="outputFolder">an output folder path.</param>
        /// <param name="thumbnailSize">the thumbnail size.</param>
        public ImageServiceModal(string outputFolder, int thumbnailSize)
        {
            m_OutputFolder = outputFolder;
            m_thumbnailSize = thumbnailSize;
        }

        /// <summary>
        /// Adds a file to the output directory, ordered by year\month.
        /// </summary>
        /// <param name="path">source path</param>
        /// <param name="result">result of the operation</param>
        /// <returns>The path of the added file, or an error message if there was a failure.
        ///          result is true if the addition was successful.</returns>
        public string AddFile(string path, out bool result)
        {
            string fileCreatedPath;
            System.Threading.Thread.Sleep(1000);
            DirectoryInfo dir = Directory.CreateDirectory(m_OutputFolder);
            dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            Directory.CreateDirectory(Path.Combine(m_OutputFolder, "Thumbnails"));
            try
            {
                DateTime dateTime = GetDateTakenFromImage(path);
                CreateImageFolder(dateTime, "");
                fileCreatedPath = HandleNewFileAddition(path, dateTime, out result);
                CreateThumbnail(fileCreatedPath, dateTime);
            }
            catch(Exception e)
            {
                // handle error, currently error msg only
                result = false;
                return "Error. Failed moving the file.";
            }
            return fileCreatedPath;
        }

        /// <summary>
        /// Creates a thumbnail and adds it to the thumbnails folder.
        /// </summary>
        /// <param name="path">source path</param>
        /// <param name="dateTime">date time object from the file in the path</param>
        /// <returns>true if successful</returns>
        private bool CreateThumbnail(string path, DateTime dateTime)
        {
            bool result;
            string thumbnailPath = Path.Combine(CreateImageFolder(dateTime, "Thumbnails"), Path.GetFileName(path));
            using (Image img = Image.FromFile(path),
                         resized = ResizeImage(img, m_thumbnailSize, m_thumbnailSize)) {
                resized.RotateFlip(RotateFlipType.Rotate270FlipY);
                resized.Save(thumbnailPath);
            }
            return true;


            // --------------------------------------------
            // TO CHANGE
            //try
            //{
            //    Image image = Image.FromFile(path);
            //    Image thumb = image.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize, () => false, IntPtr.Zero);
            //    thumb.Save(Path.ChangeExtension(thumbnailPath, "thumb"));
            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
            // ----------------------------------------------
        }

        /// <summary>
        /// resizes an image.
        /// </summary>
        /// <param name="image">an image object of the file to be resized.</param>
        /// <param name="width">width required.</param>
        /// <param name="height">height required.</param>
        /// <returns>a Bitmap object of the resized image.</returns>
        public Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// Copies a file from path to the appropriate outputDir path, i.e: outputDir\year\month.
        /// </summary>
        /// <param name="path">a file path.</param>
        /// <param name="dateTime">a DateTime object corresponding to the file.</param>
        /// <param name="result">a result out boolean to indicate if the operation was successful</param>
        /// <returns>The path to the new file added if successfull, null otherwise.</returns>
        private string HandleNewFileAddition(string path, DateTime dateTime, out bool result)
        {
            string fileName = Path.GetFileName(path);
            string outputPath = Path.Combine(ParseMonthYearPath(dateTime, ""), fileName);
            return CopyFile(path, outputPath, out result);
        }

        private string CopyFile(string srcPath, string outputPath, out bool result)
        {
            try
            {
                File.Copy(srcPath, outputPath);
                result = true;
                return outputPath;
            }
            catch (IOException e)
            {
                string newFilePath = HandleDuplicateFile(srcPath, outputPath, out result);
                result = newFilePath != null; // true if the copying was successful.
                return newFilePath;
            }
        }

        /// <summary>
        /// Moves a file from path to outputPath with duplications. For example, filename.txt will become filename(1).txt and etc.
        /// </summary>
        /// <param name="srcPath">a path to move a file from.</param>
        /// <param name="outputPath">a path to move the file to.</param>
        /// <param name="result">a result out boolean to indicate if the operation was successful</param>
        /// <returns>true if successful.</returns>
        private string HandleDuplicateFile(string srcPath, string outputPath, out bool result)
        {
            string directory = Path.GetDirectoryName(outputPath);
            string fileName = Path.GetFileNameWithoutExtension(outputPath) + "({0})";
            string extension = Path.GetExtension(outputPath);
            int duplicatesAllowed = 10;
            for(int i = 1; i < duplicatesAllowed; ++i)
            {
                string filePath = Path.Combine(directory, string.Format(fileName, i) + extension);
                if(!File.Exists(filePath)) {
                    File.Copy(srcPath, filePath);
                    result = true;
                    return filePath;
                }
            }
            result = false;
            return null;
        }

        /// <summary>
        /// Creates a folder in the outputDir according to the year and month. Path is m_outputDir\subfolder\month\year.
        /// </summary>
        /// <param name="dateTime">a DateTime to extract year and month info from.</param>
        private string CreateImageFolder(DateTime dateTime, string subfolder)
        {
            string path = ParseMonthYearPath(dateTime, subfolder);
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Parses a m_outPutDir\month\year from the DateTime.
        /// </summary>
        /// <param name="dateTime">a DateTime object to parse Month and Year Properties from.</param>
        /// <returns>a string represntation of the path.</returns>
        private string ParseMonthYearPath(DateTime dateTime, string subfolder)
        {
            return Path.Combine(m_OutputFolder, subfolder, dateTime.Year.ToString(), dateTime.Month.ToString());
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
