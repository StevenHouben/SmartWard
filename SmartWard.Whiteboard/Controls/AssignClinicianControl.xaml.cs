using SmartWard.Whiteboard.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartWard.Whiteboard.Controls
{
    /// <summary>
    /// Interaction logic for SelectUserControl.xaml
    /// </summary>
    public partial class AssignClinicianControl : UserControl
    {
        public AssignClinicianControl()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((Popup)((Grid)this.Parent).Parent).IsOpen = false;
        }
    }
}
