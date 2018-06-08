using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class PhotosModel
    {
        private static Regex r = new Regex(":");
        private readonly string thumbnailsFolder = "Thumbnails";
        public List<ImageInfo> images { get; private set; }

        public PhotosModel()
        {
            UpdatePhotos();
        }

        /// <summary>
        /// Updates the photos.
        /// </summary>
        public void UpdatePhotos()
        {
            images = new List<ImageInfo>();
            List<string> thumbnailPhotosPaths = new List<string>();
            if (Directory.Exists(SettingsContainer.Instance.OutputDir))
            {
                thumbnailPhotosPaths = Directory.GetFiles(SettingsContainer.Instance.OutputDir, "*.*", SearchOption.AllDirectories).Where(d => d.Contains(thumbnailsFolder)).ToList();
            }
            foreach (string thumbPath in thumbnailPhotosPaths)
            {
                ImageInfo imageInfo = new ImageInfo()
                {
                    absoluteThumbImagePath = thumbPath,
                    relativeThumbImagePath = thumbPath.Replace(HttpRuntime.AppDomainAppPath, Path.DirectorySeparatorChar.ToString()).Replace(Path.DirectorySeparatorChar, '/'),
                    absoluteFullImagePath = thumbPath.Replace(thumbnailsFolder, ""),
                };
                imageInfo.relativeFullImagePath = imageInfo.absoluteFullImagePath.Replace(HttpRuntime.AppDomainAppPath, Path.DirectorySeparatorChar.ToString())
                    .Replace(Path.DirectorySeparatorChar, '/');
                imageInfo.dateTaken = GetDateTakenFromImage(imageInfo.absoluteFullImagePath);
                images.Add(imageInfo);
            }
        }

        /// <summary>
        /// Deletes the photo by relative full image path.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <returns></returns>
        public bool DeletePhotoByRelativeFullImgPath(string relativePath)
        {
            try
            {
                foreach (ImageInfo imgInfo in images)
                {
                    if (imgInfo.relativeFullImagePath == relativePath)
                    {
                        File.Delete(imgInfo.absoluteFullImagePath);
                        File.Delete(imgInfo.absoluteThumbImagePath);
                        images.Remove(imgInfo);
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// Finds the image by relative full img path.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <returns>The image info.</returns>
        public ImageInfo FindImageByRelativeFullImgPath(string relativePath)
        {
            foreach (ImageInfo imgInfo in images)
            {
                if (imgInfo.relativeFullImagePath == relativePath)
                {
                    return imgInfo;
                }
            }
            return new ImageInfo();
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