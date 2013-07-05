using ABC.Infrastructure.Context.Location;
using ABC.Model;
using ABC.Model.Users;
using System;
using System.Windows;
using System.Windows.Forms;
using System.ComponentModel;
using SmartWard.Infrastructure;
using SmartWard.Model;
using ButtonState = ABC.Infrastructure.Context.Location.ButtonState;

namespace SmartWard.Whiteboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public WardNode WardNode { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Maximized;

            DataContext = this;

            InitializeMapOverlay();

            WardNode = WardNode.StartWardNodeAsSystem(WebConfiguration.DefaultWebConfiguration);
            whiteboard.Patients = WardNode.Patients;
        }

        private void InitializeMapOverlay()
        {
            var sysRect = Screen.PrimaryScreen.Bounds;
            var rect = new Rect(
                0,
                0,
                sysRect.Width,
                sysRect.Height);
            popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute;
            popup.PlacementRectangle = rect;
            popup.Width = rect.Width;
            popup.Height = rect.Height;
            popup.AllowsTransparency = true;
            popup.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popup.MouseDown += popup_Down;
            popup.TouchDown += popup_Down;
        }

        void Tracker_TagButtonDataReceived(Tag tag, TagEventArgs e)
        {
            if(e.Tag.ButtonA == ButtonState.Pressed)
                Console.WriteLine(@"Button A pressed on  {0}", tag.Name);

            if (e.Tag.ButtonB == ButtonState.Pressed)
                Console.WriteLine(@"Button B pressed on  {0}", tag.Name);

            if (e.Tag.ButtonC == ButtonState.Pressed)
                Console.WriteLine(@"Button C pressed on  {0}", tag.Name);

            if (e.Tag.ButtonD == ButtonState.Pressed)
                Console.WriteLine(@"Button D pressed on  {0}",  tag.Name);
        }

        void popup_Down(object sender, EventArgs e)
        {
            popup.IsOpen = false;
        }   

        private void btnMap_click(object sender, RoutedEventArgs e)
        {
            txtMap.Text = popup != null && (popup.IsOpen = !popup.IsOpen) ? "Close Map" : "Map";
        }

        private void btnLocationTracking_Click_1(object sender, RoutedEventArgs e)
        {
            WardNode.IsLocationEnabled = !WardNode.IsLocationEnabled;
        }

        private void btnDiscovery_Click_1(object sender, RoutedEventArgs e)
        {
            WardNode.IsBroadcastEnabled = !WardNode.IsBroadcastEnabled;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
           WardNode.AddPatient(new Patient());
        }


        private void btnAddActivity_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnWebApi_OnClick(object sender, RoutedEventArgs e)
        {
            WardNode.IsWebApiEnabled = !WardNode.IsWebApiEnabled;
        }

        private int _roomCounter;
    }
    public class Doctor : User { public string Specialisation { get; set; } }
    public class Treatment : Activity { public int Progress { get; set; } }

}
