using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ABC.Model.Primitives;
using Microsoft.Surface.Presentation.Controls;
using SmartWard.HyPR.ViewModels;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace SmartWard.HyPR.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MobileApplication
    {
        public MobileApplication()
        {
            InitializeComponent();
          
            InitializeWindow();

            ListBoxExtensions.SetAllowReorderSource(Whiteboard.BoardView, true);
            ListBoxExtensions.Reordered += (sender, e) => ((MobileApplicationViewModel)DataContext).ReorganizeDragAndDroppedPatients(e.DroppedData, e.OriginalData);
        }

        private void InitializeWindow()
        {
            KeyDown += MainWindow_KeyDown;
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.SingleBorderWindow;
        }

        private void sliders_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var rgb = new Rgb(Convert.ToByte(sliderRed.Value), Convert.ToByte(sliderGreen.Value), Convert.ToByte(sliderBlue.Value));

            var viewModel = (MobileApplicationViewModel) DataContext;
            viewModel.UpdatePatientColor(rgb);
        }
        void UpdateSliders(Rgb color)
        {
            sliderRed.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    sliderRed.Value = color.Red;
                }));
            sliderBlue.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    sliderBlue.Value = color.Blue;
                }));
            sliderGreen.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    sliderGreen.Value = color.Green;
                }));
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Environment.Exit(0);
        }


        private void txtName_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            System.Diagnostics.Process.Start(@"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe");
            txtName.Focus();
        }


        private void menu_click(object sender, RoutedEventArgs e)
        {
            string name = ((SurfaceButton)sender).Name;
            if(name == "btnRegister")
            {
                Register.Visibility = Visibility.Visible;
                Records.Visibility = Overview.Visibility = Visibility.Hidden;
            }
            else if (name == "btnRecords")
            {
                Records.Visibility = Visibility.Visible;
                Register.Visibility = Overview.Visibility = Visibility.Hidden;
            }
            else if (name == "btnOverview")
            {
                Overview.Visibility = Visibility.Visible;
                Register.Visibility = Records.Visibility = Visibility.Hidden;
            }

        }

        private void splash_MouseDown(object sender, MouseButtonEventArgs e)
        {
            splash.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
