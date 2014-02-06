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

namespace SmartWard.PDA.Views
{
    /// <summary>
    /// Interaction logic for EWSView.xaml
    /// </summary>
    public partial class EWSView : Page
    {
        public EWSView()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var f = NavigationService.GetNavigationService(this);
            f.GoBack();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as SurfaceButton).Focus();
        }

    }
}
