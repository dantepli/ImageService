﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageServiceGUI.Models;
using ImageService.Infrastructure.Objects;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;

namespace ImageServiceGUI.ViewModels
{
    class SettingsViewModel : ViewModel
    {
        private ISettingsModel m_model;
        private DirectoryPath m_selectedPath;

        // service settings
        public string OutputDir
        {
            get { return m_model.OutputDir; }
        }

        public string SourceName
        {
            get { return m_model.SourceName; }
        }

        public string LogName
        {
            get { return m_model.LogName; }
        }

        public int ThumbnailSize
        {
            get { return m_model.ThumbnailSize; }
        }
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
            get { return m_model.DirectoryPaths; }
            set
            {
                m_model.DirectoryPaths = value;
            }
        }

        /// <summary>
        /// c'tor
        /// </summary>
        /// <param name="sm">settings model</param>
        public SettingsViewModel(ISettingsModel sm)
        {
            this.m_model = sm;
            RemoveCommand = new DelegateCommand<object>(OnRemove, CanRemove);
            this.PropertyChanged += RemoveCommandPropertyChanged;

            m_model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged(e.PropertyName);
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

        /// <summary>
        /// action to do to remove handler
        /// </summary>
        /// <param name="sender">sender of data</param>
        /// <param name="e">args to represent data</param>
        private void OnRemove(object obj)
        {
            m_model.RemoveHandler(SelectedPath);
        }
    }
}
