using Microsoft.Surface.Presentation.Controls;
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
    /// Interaction logic for Patient.xaml
    /// </summary>
    public partial class PatientView : Page
    {
        public PatientView()
        {
            InitializeComponent();
        }
        void popup_Down(object sender, EventArgs e)
        {
            popup.IsOpen = false;
        }

        private void AddResourceButton_Click(object sender, RoutedEventArgs e)
        {
            PatientsLayoutViewModel patientLayoutViewModel = (PatientsLayoutViewModel)(sender as SurfaceButton).DataContext;
            NavigationService.GetNavigationService(this).Navigate(new AddResourceView() { DataContext = new AddResourceViewModel(patientLayoutViewModel.Patient, patientLayoutViewModel.WardNode) });
        }

        private void HideButton_click(object sender, RoutedEventArgs e)
        {
            (sender as SurfaceButton).Visibility = Visibility.Collapsed;
            CheckMark.Visibility = Visibility.Visible;
        }
    }
}
