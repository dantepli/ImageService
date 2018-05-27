using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ImageServiceGUI.Models;

namespace ImageServiceGUI.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        private IMainWindowModel m_model;
        
        public bool MainFrameColor
        {
            get { return m_model.MainFrameColor; }
        }

        /// <summary>
        /// c'tor
        /// </summary>
        /// <param name="mwm">main window model</param>
        public MainWindowViewModel(IMainWindowModel mwm)
        {
            this.m_model = mwm;

            m_model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged(e.PropertyName);
            };
        }
    }
}
