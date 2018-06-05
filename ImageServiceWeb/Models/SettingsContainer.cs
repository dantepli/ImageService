using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class SettingsContainer
    {
        private static volatile SettingsContainer m_instance;

        private SettingsContainer() { }
            
        public static SettingsContainer Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new SettingsContainer();
                }
                return m_instance;
            }
        }

        [DataType(DataType.Text)]
        [Display(Name = "OutputDir")]
        public  string OutputDir { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "SourceName")]
        public  string SourceName { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "LogName")]
        public  string LogName { get; set; }
        [Display(Name = "ThumbnailSize")]
        public  int ThumbnailSize { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "Handlers")]
        public List<string> Handlers { get; set; }
    }
}