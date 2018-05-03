using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageServiceGUI.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;

namespace ImageServiceGUI.ViewModels
{
    class SettingsViewModel : ViewModel
    {
        private ISettingsModel m_model;
        private DirectoryPath m_selectedPath;

        public ICommand RemoveCommand { get; private set; }
        public DirectoryPath SelectedPath
        {
            get { return m_selectedPath; }
            set
            {
                m_selectedPath = value;
                NotifyPropertyChanged("SelectedPath");
            }
        }

        public ObservableCollection<DirectoryPath> DirectoryPaths
        {
            get { return m_model.ModelDirPaths; }
            set
            {
                m_model.ModelDirPaths = value;
            }
        }

        public SettingsViewModel(ISettingsModel sm)
        {
            this.m_model = sm;
            RemoveCommand = new DelegateCommand<object>(OnRemove, CanRemove);
            this.PropertyChanged += RemoveCommandPropertyChanged;

            m_model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e) {
                if (e.PropertyName == "ModelDirPaths")
                {
                    NotifyPropertyChanged("DirectoryPaths");
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">sender of the event.</param>
        /// <param name="e">the event arguments.</param>
        private void RemoveCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var command = this.RemoveCommand as DelegateCommand<object>;
            command.RaiseCanExecuteChanged();
        }

        public string OutputDir
        {
            get { return "output"; }
        }

        public string SourceName
        {
            get { return "source"; }
        }

        public string LogName
        {
            get { return "log"; }
        }

        public int ThumbnailSize
        {
            get { return 120; }
        }

        /// <summary>
        /// Checks if a directory path was selected.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if a directory path was selected.</returns>
        private bool CanRemove(object obj)
        {
            if (SelectedPath == null)
            {
                return false;
            }
            return true;
        }

        private void OnRemove(object obj)
        {
            bool success = m_model.RemoveHandler(SelectedPath);

            if (success)
            {
                m_model.ModelDirPaths.Remove(SelectedPath);
                SelectedPath = null;
            }
        }
    }
}
