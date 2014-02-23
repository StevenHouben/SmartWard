using System;
using System.Windows;
using SmartWard.PDA.ViewModels;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;

namespace SmartWard.PDA.Views
{
    /// <summary>
    /// Interaction logic for Activities.xaml
    /// </summary>
    public partial class Activities : Page
    {
        public Activities()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.NavigationService.RemoveBackEntry();
        }
    }
}
