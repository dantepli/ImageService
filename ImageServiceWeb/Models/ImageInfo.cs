using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class ImageInfo
    {
        public string absoluteFullImagePath { get; set; }
        public string relativeFullImagePath { get; set; }
        public string absoluteThumbImagePath { get; set; }
        public string relativeThumbImagePath { get; set; }
        public DateTime dateTaken { get; set; }
    }
}