using SmartWard.AdministrationTool.ViewModels;
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
using System.Windows.Shapes;
using Windows.Networking.Proximity;

namespace SmartWard.AdministrationTool.Views
{
    /// <summary>
    /// Interaction logic for AssociateTokenDialogBox.xaml
    /// </summary>
    public partial class AssociateTokenDialogBox : Window
    {
        public AssociateTokenDialogBox()
        {
            InitializeComponent();
        }

        private void DetectNfc(object sender, RoutedEventArgs e)
        {
            ((UpdatableClinicianViewModel)DataContext).DetectNfc();
        }
    }
}
