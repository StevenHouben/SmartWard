using SmartWard.Whiteboard.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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
    }
}
