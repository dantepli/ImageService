using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class SettingsContainer
    {
        [DataType(DataType.Text)]
        [Display(Name = "OutputDir")]
        public static string OutputDir { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "SourceName")]
        public static string SourceName { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "LogName")]
        public static string LogName { get; set; }
        [Display(Name = "ThumbnailSize")]
        public static int ThumbnailSize { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "Handlers")]
        public static List<string> Handlers { get; set; }
    }
}