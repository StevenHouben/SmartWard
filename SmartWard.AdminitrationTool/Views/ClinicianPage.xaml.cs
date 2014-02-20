using Microsoft.Surface.Presentation.Controls;
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
    /// Interaction logic for ClinicianPage.xaml
    /// </summary>
    public partial class ClinicianPage : Page
    {
        public ClinicianPage()
        {
            InitializeComponent();
        }

        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            ((SurfaceButton)sender).Focus();
            NavigationService.GetNavigationService(this).GoBack();
        }
    }
}
