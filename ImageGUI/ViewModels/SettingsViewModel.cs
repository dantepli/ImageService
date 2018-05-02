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
        private ObservableCollection<DirectoryPath> m_directoryPaths;
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
            get { return m_directoryPaths; }
            set
            {
                m_directoryPaths = value;
                NotifyPropertyChanged("DirectoryPaths");
            }
        }

        public SettingsViewModel()
        {
            // TODO assign model
            this.m_model = null;
            RemoveCommand = new DelegateCommand<object>(OnRemove, CanRemove);
            this.PropertyChanged += RemoveCommandPropertyChanged;
            m_directoryPaths = new ObservableCollection<DirectoryPath>();


            m_directoryPaths.Add(new DirectoryPath() { DirPath = "I" });
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "AM" });
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "THE" });
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "LORD" });
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "OF" });
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "BINDING" });
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "BOW" });
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "BEFORE" });
            m_directoryPaths.Add(new DirectoryPath() { DirPath = "ME" });
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
            // TODO send path to model
            m_directoryPaths.Remove(SelectedPath);
            SelectedPath = null;
        }
    }
}
