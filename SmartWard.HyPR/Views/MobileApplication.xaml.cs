using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Linq;
using NooSphere.Model.Primitives;
using Microsoft.Surface.Presentation.Controls;
using SmartWard.HyPR.ViewModels;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Microsoft.Surface.Presentation;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using SmartWard.ViewModels;
using System.Collections.Generic;
using TimelineLibrary;

namespace SmartWard.HyPR.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MobileApplication:Window
    {

        bool debugging = false;
        public MobileApplication()
        {
            InitializeComponent();


            InitializeWindow();

            ListBoxExtensions.SetAllowReorderSource(Whiteboard.BoardView, true);
            ListBoxExtensions.Reordered += (sender, e) => ((MobileApplicationViewModel)DataContext).ReorganizeDragAndDroppedPatients(e.DroppedData, e.OriginalData);

            var pal = new Microsoft.Surface.Presentation.Palettes.LightSurfacePalette();
            pal.ListBoxItemSelectionBackgroundDisabledColor = pal.ListBoxItemSelectionBackgroundColor = pal.ListBoxItemSelectionBackgroundPressedColor = System.Windows.Media.Colors.White;
            
            SurfaceColors.SetDefaultApplicationPalette(pal);

            Loaded += MobileApplication_Loaded;

        }

        void MobileApplication_Loaded(object sender, RoutedEventArgs e)
        {

                var viewModel = (MobileApplicationViewModel)DataContext;
                viewModel.RFIDLoaded += viewModel_RFIDLoaded;
        }

        void viewModel_RFIDLoaded(object sender, EventArgs e)
        {
                Application.Current.Dispatcher.Invoke(() =>
            {
            AddPatient.Visibility = Visibility.Visible;
            Overview.Visibility = PatientData.Visibility = Records.Visibility = splash.Visibility= Visibility.Hidden;
            });

        }

        private void InitializeWindow()
        {
            KeyDown += MainWindow_KeyDown;
            WindowState = WindowState.Maximized;

            if (debugging)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                Width = 1280;
                Height = 800;
                MaxHeight = 800;
                MaxWidth = 1280;
            }
            else
                WindowStyle = WindowStyle.None;

            MessageFlagSelector.ItemsSource = Enum.GetValues(typeof(SmartWard.Models.MessageFlags));
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
            if(name == "btnHistory")
            {
                PatientData.Visibility = Visibility.Visible;
                Records.Visibility = Overview.Visibility = AddPatient.Visibility = Visibility.Hidden;
                PopulateTimeLine();
            }
            else if (name == "btnRecords")
            {
                Records.Visibility = Visibility.Visible;
                PatientData.Visibility = Overview.Visibility = AddPatient.Visibility = Visibility.Hidden;
            }
            else if (name == "btnOverview")
            {
                Overview.Visibility = Visibility.Visible;
                PatientData.Visibility = Records.Visibility = AddPatient.Visibility = Visibility.Hidden;
            }
            else if (name == "bntAddPatient")
            {
                AddPatient.Visibility = Visibility.Visible;
                Overview.Visibility = PatientData.Visibility = Records.Visibility = Visibility.Hidden;
            }
        }

        private void PopulateTimeLine()
        {
            var selectedUser = ((MobileApplicationViewModel)DataContext).SelectedUser;

            var timeItems = new List<TimelineEvent>();
            foreach (var item in selectedUser.NurseRecords.ToList())
            {
                timeItems.Add(
                    new TimelineEvent()
                    {
                        Title = item.MessageFlag.ToString(),
                        StartDate = DateTime.Parse(item.Date),
                        Description = item.Body,
                       
                        EventColor = (string)new Converters.TimelineColorConverter().Convert(item.MessageFlag,typeof(string), null, null)
                
                    }
                    );
            }

            timeline.ClearEvents();
            timeline.ResetEvents(timeItems);
        }


        private void splash_MouseDown(object sender, MouseButtonEventArgs e)
        {
            splash.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Rectangle_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            var color = GetColorAtPoint((Rectangle)sender,e.GetTouchPoint((Rectangle)sender).Position);
            var rgb = new Rgb(color.R,color.G,color.B);

            var viewModel = (MobileApplicationViewModel)DataContext;
            viewModel.UpdatePatientColor(rgb);
        }


        //http://dotupdate.wordpress.com/2008/01/28/find-the-color-of-a-point-in-a-lineargradientbrush/
        //Calculates the color of a point in a rectangle that is filled
        //with a LinearGradientBrush.
        private Color GetColorAtPoint(Rectangle theRec, Point thePoint)
        {
            //Get properties
            LinearGradientBrush br = (LinearGradientBrush)theRec.Fill;

            double y3 = thePoint.Y;
            double x3 = thePoint.X;

            double x1 = br.StartPoint.X * theRec.ActualWidth;
            double y1 = br.StartPoint.Y * theRec.ActualHeight;
            Point p1 = new Point(x1, y1); //Starting point

            double x2 = br.EndPoint.X * theRec.ActualWidth;
            double y2 = br.EndPoint.Y * theRec.ActualHeight;
            Point p2 = new Point(x2, y2);  //End point

            //Calculate intersecting points 
            Point p4 = new Point(); //with tangent

            if (y1 == y2) //Horizontal case
            {
                p4 = new Point(x3, y1);
            }

            else if (x1 == x2) //Vertical case
            {
                p4 = new Point(x1, y3);
            }

            else //Diagnonal case
            {
                double m = (y2 - y1) / (x2 - x1);
                double m2 = -1 / m;
                double b = y1 - m * x1;
                double c = y3 - m2 * x3;

                double x4 = (c - b) / (m - m2);
                double y4 = m * x4 + b;
                p4 = new Point(x4, y4);
            }

            //Calculate distances relative to the vector start
            double d4 = dist(p4, p1, p2);
            double d2 = dist(p2, p1, p2);

            double x = d4 / d2;

            //Clip the input if before or after the max/min offset values
            double max = br.GradientStops.Max(n => n.Offset);
            if (x > max)
            {
                x = max;
            }
            double min = br.GradientStops.Min(n => n.Offset);
            if (x < min)
            {
                x = min;
            }

            //Find gradient stops that surround the input value
            GradientStop gs0 = br.GradientStops.Where(n => n.Offset <= x).OrderBy(n => n.Offset).Last();
            GradientStop gs1 = br.GradientStops.Where(n => n.Offset >= x).OrderBy(n => n.Offset).First();

            float y = 0f;
            if (gs0.Offset != gs1.Offset)
            {
                y = (float)((x - gs0.Offset) / (gs1.Offset - gs0.Offset));
            }

            //Interpolate color channels
            Color cx = new Color();
            if (br.ColorInterpolationMode == ColorInterpolationMode.ScRgbLinearInterpolation)
            {
                float aVal = (gs1.Color.ScA - gs0.Color.ScA) * y + gs0.Color.ScA;
                float rVal = (gs1.Color.ScR - gs0.Color.ScR) * y + gs0.Color.ScR;
                float gVal = (gs1.Color.ScG - gs0.Color.ScG) * y + gs0.Color.ScG;
                float bVal = (gs1.Color.ScB - gs0.Color.ScB) * y + gs0.Color.ScB;
                cx = Color.FromScRgb(aVal, rVal, gVal, bVal);
            }
            else
            {
                byte aVal = (byte)((gs1.Color.A - gs0.Color.A) * y + gs0.Color.A);
                byte rVal = (byte)((gs1.Color.R - gs0.Color.R) * y + gs0.Color.R);
                byte gVal = (byte)((gs1.Color.G - gs0.Color.G) * y + gs0.Color.G);
                byte bVal = (byte)((gs1.Color.B - gs0.Color.B) * y + gs0.Color.B);
                cx = Color.FromArgb(aVal, rVal, gVal, bVal);
            }
            return cx;
        }

        //Helper method for GetColorAtPoint
        //Returns the signed magnitude of a point on a vector with origin po and pointing to pf
        private double dist(Point px, Point po, Point pf)
        {
            double d = Math.Sqrt((px.Y - po.Y) * (px.Y - po.Y) + (px.X - po.X) * (px.X - po.X));
            if (((px.Y < po.Y) && (pf.Y > po.Y)) ||
                ((px.Y > po.Y) && (pf.Y < po.Y)) ||
                ((px.Y == po.Y) && (px.X < po.X) && (pf.X > po.X)) ||
                ((px.Y == po.Y) && (px.X > po.X) && (pf.X < po.X)))
            {
                d = -d;
            }
            return d;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var color = GetColorAtPoint((Rectangle)sender,e.GetPosition((Rectangle)sender));
            var rgb = new Rgb(color.R, color.G, color.B);

            var viewModel = (MobileApplicationViewModel)DataContext;
            viewModel.UpdatePatientColor(rgb);
        }

        private void Border_Drop(object sender, DragEventArgs e)
        {
            var previous = ((MobileApplicationViewModel)DataContext).SelectedUser;
            var newPatient = ((IDataObject)e.Data).GetData(typeof(PatientViewModel)) as PatientViewModel;
            if(newPatient !=null)
            {
                ((MobileApplicationViewModel)DataContext).SelectedUser = newPatient;
                AddPatient.Visibility = Visibility.Visible;
                Overview.Visibility = PatientData.Visibility = Records.Visibility = Visibility.Hidden;
            }

        }

        private void close_Click_1(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);

        }

    }
}
