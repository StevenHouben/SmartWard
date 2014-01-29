using Microsoft.Surface.Presentation.Controls;
using SmartWard.Models;
using SmartWard.PDA.ViewModels;
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

namespace SmartWard.PDA.Views
{
    /// <summary>
    /// Interaction logic for PatientsLayout.xaml
    /// </summary>
    public partial class PatientsLayout : UserControl
    {
        public PatientsLayout()
        {
            InitializeComponent();
        }

        public void NavigateToPatientView(PatientsLayoutViewModel plViewModel)
        {
            NavigationService.GetNavigationService(this).Navigate(new PatientView() { DataContext = plViewModel });
        }

        private void Patient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SurfaceListBox src = (SurfaceListBox)e.Source;
            PatientsLayoutViewModel plViewModel = (PatientsLayoutViewModel)src.SelectedItems[0];
            NavigateToPatientView(plViewModel);
        }
    }
}
