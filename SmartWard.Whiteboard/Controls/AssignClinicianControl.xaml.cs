using Microsoft.Surface.Presentation.Controls;
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
        SurfaceButton _sourceButton;
        public AssignClinicianControl(SurfaceButton sourceButton)
        {
            InitializeComponent();
            _sourceButton = sourceButton;
            _sourceButton.PreviewMouseDown += swallowDatMouseEvent;
            _sourceButton.PreviewTouchDown += swallowDatTouchEvent;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((Popup)((Grid)this.Parent).Parent).IsOpen = false;
            _sourceButton.PreviewMouseDown -= swallowDatMouseEvent;
            _sourceButton.PreviewTouchDown -= swallowDatTouchEvent;
        }

        private void swallowDatMouseEvent(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void swallowDatTouchEvent(object sender, TouchEventArgs e)
        {
            e.Handled = true;
        }
    }
}
