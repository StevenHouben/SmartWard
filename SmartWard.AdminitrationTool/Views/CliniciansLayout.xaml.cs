﻿using Microsoft.Surface.Presentation.Controls;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
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

namespace SmartWard.AdministrationTool.Views
{
    /// <summary>
    /// Interaction logic for CliniciansLayout.xaml
    /// </summary>
    public partial class CliniciansLayout : UserControl
    {
        public CliniciansLayout()
        {
            InitializeComponent();
        }

        private void NavigateToSelection(object sender, SelectionChangedEventArgs e)
        {
            NavigationService.GetNavigationService(this).Navigate(new ClinicianPage() { DataContext = (ClinicianViewModelBase)((SurfaceListBox)e.Source).SelectedItem });
        }
    }
}
