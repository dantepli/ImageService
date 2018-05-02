using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageServiceGUI.Models;
using ImageServiceGUI.ViewModels;

namespace ImageServiceGUI.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private SettingsViewModel vm;

        public SettingsView()
        {
            InitializeComponent();
            vm = new SettingsViewModel();
            this.DataContext = vm;

            HandlersList.ItemsSource = vm.DirectoryPaths;
        }
    }
}
