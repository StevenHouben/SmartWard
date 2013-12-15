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

namespace SmartWard.Whiteboard.Views.EWS
{
    /// <summary>
    /// Interaction logic for EWSControl.xaml
    /// </summary>
    public partial class EWSControl : UserControl
    {
        public EWSControl()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((Popup)((Grid)this.Parent).Parent).IsOpen = false;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            EWSViewModel ewsViewModel = (EWSViewModel)this.DataContext;
            ewsViewModel.SaveEWS();
            ((Popup)((Grid)this.Parent).Parent).IsOpen = false;
        }
    }
}
