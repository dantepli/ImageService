using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModel
    {
        #region Members
        private static Regex r = new Regex(":");
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size 

        // Image rotation consts.
        private const int OrientationKey = 0x0112;
        private const int NotSpecified = 0;
        private const int NormalOrientation = 1;
        private const int MirrorHorizontal = 2;
        private const int UpsideDown = 3;
        private const int MirrorVertical = 4;
        private const int MirrorHorizontalAndRotateRight = 5;
        private const int RotateLeft = 6;
        private const int MirorHorizontalAndRotateLeft = 7;
        private const int RotateRight = 8;
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
            DirectoryInfo dir = Directory.CreateDirectory(m_OutputFolder);
            dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            Directory.CreateDirectory(Path.Combine(m_OutputFolder, "Thumbnails"));
            try
            {
                if(HandleLocked(path))
                {
                    DateTime dateTime = GetDateTakenFromImage(path);
                    CreateImageFolder(dateTime, "");
                    fileCreatedPath = HandleNewFileAddition(path, dateTime, out result);
                    CreateThumbnail(fileCreatedPath, dateTime);
                } else
                {
                    result = false;
                    return "Error. Failed moving the file.";
                }
            }
            catch(Exception e)
            {
                // handle error, currently error msg only
                result = false;
                return "Error. Failed moving the file." + " Reason: " + e.Message;
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
            string thumbnailPath = Path.Combine(CreateImageFolder(dateTime, "Thumbnails"), Path.GetFileName(path));
            try
            {
                using (Image image = Image.FromFile(path),
                       thumb = image.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize, () => false, IntPtr.Zero))
                {
                    KeepOrientation(image, thumb);
                    thumb.Save(thumbnailPath);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks the orientation property of the image, if exists, keeps the destination image
        /// with the same orientation property.
        /// </summary>
        /// <param name="srcImg">source image to keep the orientation from.</param>
        /// <param name="dstImg">destination image to keep the orientation.</param>
        private void KeepOrientation(Image srcImg, Image dstImg)
        {
            if (srcImg.PropertyIdList.Contains(OrientationKey))
            {
                var orientation = (int)srcImg.GetPropertyItem(OrientationKey).Value[0];
                switch (orientation)
                {
                    case NotSpecified: // Assume it is good.
                    case NormalOrientation:
                        // No rotation required.
                        break;
                    case MirrorHorizontal:
                        dstImg.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case UpsideDown:
                        dstImg.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case MirrorVertical:
                        dstImg.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case MirrorHorizontalAndRotateRight:
                        dstImg.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case RotateLeft:
                        dstImg.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case MirorHorizontalAndRotateLeft:
                        dstImg.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case RotateRight:
                        dstImg.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                    default:
                        throw new NotImplementedException("An orientation of " + orientation + " isn't implemented.");
                }
            }
        }

        /// <summary>
        /// Moves a file from path to the appropriate outputDir path, i.e: outputDir\year\month.
        /// </summary>
        /// <param name="path">a file path.</param>
        /// <param name="dateTime">a DateTime object corresponding to the file.</param>
        /// <param name="result">a result out boolean to indicate if the operation was successful</param>
        /// <returns>The path to the new file added if successfull, null otherwise.</returns>
        private string HandleNewFileAddition(string path, DateTime dateTime, out bool result)
        {
            string fileName = Path.GetFileName(path);
            string outputPath = Path.Combine(ParseMonthYearPath(dateTime, ""), fileName);
            return MoveFile(path, outputPath, out result);
        }

        /// <summary>
        /// Moves a file to the specified path, handles duplicated.
        /// </summary>
        /// <param name="srcPath">source path.</param>
        /// <param name="outputPath">output path.</param>
        /// <param name="result">indiciation of success.</param>
        /// <returns>the path of the file.</returns>
        private string MoveFile(string srcPath, string outputPath, out bool result)
        {
            try
            {
                File.Move(srcPath, outputPath);
                result = true;
                return outputPath;
            }
            catch (IOException)
            {
                string newFilePath = HandleDuplicateFile(srcPath, outputPath, out result);
                result = (newFilePath != null); // true if the Moving was successful.
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
            string path = outputPath.Replace(m_OutputFolder, "");
            //string thumbPath = Path.Combine(m_OutputFolder, "Thumbnails", path);
            string thumbPath = m_OutputFolder + "\\" + "Thumbnails" + "\\" + path;
            File.Delete(thumbPath);
            File.Delete(outputPath);
            File.Move(srcPath, outputPath);
            result = true;
            return outputPath;
            //string directory = Path.GetDirectoryName(outputPath);
            //string fileName = Path.GetFileNameWithoutExtension(outputPath) + " " + "({0})";
            //string extension = Path.GetExtension(outputPath);
            //int duplicatesAllowed = 100;
            //for(int i = 1; i < duplicatesAllowed; ++i)
            //{
            //    string filePath = Path.Combine(directory, string.Format(fileName, i) + extension);
            //    if(!File.Exists(filePath)) {
            //        File.Move(srcPath, filePath);
            //        result = true;
            //        return filePath;
            //    }
            //}
            //result = false;
            //return null;
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
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    PropertyItem propItem = myImage.GetPropertyItem(36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    return DateTime.Parse(dateTaken);
                }
            }
            catch
            {
                return File.GetCreationTime(path);
            }
        }

        /// <summary>
        /// handles a file that is still being used by another process.
        /// timeout time set to 1 second, then the file will not be proccesed.
        /// </summary>
        /// <param name="filePath">a file path to check on.</param>
        /// <returns>true if file is ready for use.</returns>
        private bool HandleLocked(string filePath)
        {
            int intervalsAllowed = 10;
            int i = 1;
            while(IsFileLocked(filePath) && i <= intervalsAllowed)
            {
                System.Threading.Thread.Sleep(10);
            }
            return i != intervalsAllowed;
        }

        /// <summary>
        /// checks if the file is locked.
        /// </summary>
        /// <param name="filePath">a file path to check on.</param>
        /// <returns>true if locked.</returns>
        public bool IsFileLocked(string filePath)
        {
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open)) { }
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }

    }
}
