using SmartWard.Infrastructure;
using SmartWard.Model;
using SmartWard.Users;
using Microsoft.Surface.Presentation.Controls;
using Raven.Client.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using SmartWard.Infrastructure.Helpers;
using System.Windows.Forms;
using System.ComponentModel;

namespace SmartWard.Whiteboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ActivitySystem activitySystem;

        public ObservableCollection<Activity> Patients { get; set; }

        private bool isLocationEnabled = true;
        public bool IsLocationEnabled
        {
            get { return isLocationEnabled; }
            set 
            {
                isLocationEnabled = value;
                if (activitySystem != null)
                {
                    if (!IsLocationEnabled)
                        activitySystem.StopLocationTracker();
                    else
                        activitySystem.StartLocationTracker();
                }
                OnPropertyChanged("isLocationEnabled");
            }
        }

        private bool isBroadcastEnabled = true;
        public bool IsBroadcastEnabled
        {
            get { return isBroadcastEnabled; }
            set
            {
                isBroadcastEnabled = value;
                if (activitySystem != null)
                {
                    if (!IsBroadcastEnabled)
                        activitySystem.StopBroadcast();
                    else

                        activitySystem.StartBroadcast(Infrastructure.Discovery.DiscoveryType.Zeroconf, "HyPRBoard", "PIT-Lab");
                }
                OnPropertyChanged("isBroadcastEnabled");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            this.WindowStyle = System.Windows.WindowStyle.None;
            this.WindowState = System.Windows.WindowState.Maximized;

            this.DataContext = this;

            InitializeActivitySystem();

            InitializeUsers();
            InitializeMapOverlay();
        }

        private void InitializeActivitySystem()
        {
            //activitySystem = new ActivitySystem(System.Configuration.ConfigurationManager.AppSettings["ravenDB"]);
            activitySystem = new ActivitySystem(Net.GetUrl(Net.GetIp(IPType.All), 8080, "").ToString()); ;

            activitySystem.UserAdded += activitySystem_UserAdded;
            activitySystem.UserRemoved += activitySystem_UserRemoved;
            activitySystem.UserUpdated += activitySystem_UserUpdated;

            activitySystem.Tracker.TagEnter += Tracker_TagEnter;
            activitySystem.Tracker.TagLeave += Tracker_TagLeave;
            activitySystem.Tracker.TagButtonDataReceived += Tracker_TagButtonDataReceived;

            //activitySystem.StartBroadcast(Infrastructure.Discovery.DiscoveryType.Zeroconf, "HyPRBoard", "PIT-Lab");

            //activitySystem.StartLocationTracker();
        }

        private void InitializeUsers()
        {
            foreach (var user in activitySystem.Users.Values)
            {
                AddUserDataToPatientData((Patient)user);
            }
        }

        private void InitializeMapOverlay()
        {
            var sysRect = Screen.PrimaryScreen.Bounds;
            var rect = new Rect(
                0,
                0,// / 2,
                sysRect.Width,
                sysRect.Height);// / 2); 
            popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute;
            popup.PlacementRectangle = rect;
            popup.Width = rect.Width;
            popup.Height = rect.Height;
            popup.AllowsTransparency = true;
            popup.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popup.MouseDown += popup_Down;
            popup.TouchDown += popup_Down;
        }

        void Tracker_TagButtonDataReceived(Infrastructure.Location.Tag tag, Infrastructure.Location.TagEventArgs e)
        {
            if(e.Tag.ButtonA == Infrastructure.Location.ButtonState.Pressed)
                Console.WriteLine("Button A pressed on  {1}", e.Tag.ButtonA, tag.Name);

            if (e.Tag.ButtonB == Infrastructure.Location.ButtonState.Pressed)
                Console.WriteLine("Button B pressed on  {1}", e.Tag.ButtonB, tag.Name);

            if (e.Tag.ButtonC == Infrastructure.Location.ButtonState.Pressed)
                Console.WriteLine("Button C pressed on  {1}", e.Tag.ButtonC, tag.Name);

            if (e.Tag.ButtonD == Infrastructure.Location.ButtonState.Pressed)
                Console.WriteLine("Button D pressed on  {1}", e.Tag.ButtonD, tag.Name);
        }

        void Tracker_TagLeave(Infrastructure.Location.Detector detector, Infrastructure.Location.TagEventArgs e)
        {
           // Console.WriteLine("{0} left {1}", e.Tag.Name, detector.HostName);
        }

        void Tracker_TagEnter(Infrastructure.Location.Detector detector, Infrastructure.Location.TagEventArgs e)
        {
            //Console.WriteLine("{0} entered {1}", e.Tag.Name, detector.HostName);
        }

        void popup_Down(object sender, EventArgs e)
        {
            popup.IsOpen = false;
        }   

        private void activitySystem_UserUpdated(object sender, UserEventArgs e)
        {
            whiteboard.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                for (int i = 0; i < whiteboard.Patients.Count; i++)
                {
                    if (whiteboard.Patients[i].Id == e.User.Id)
                    {
                        whiteboard.Patients[i].UpdateAllProperties<User>(e.User);
                        break;
                    }
                }
            }));
        }

        private void activitySystem_UserRemoved(object sender, UserRemovedEventArgs e)
        {
            whiteboard.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                for (int i = 0; i < whiteboard.Patients.Count; i++)
                {
                    if (whiteboard.Patients[i].Id == e.Id)
                    {
                        whiteboard.Patients.RemoveAt(i);
                        break;
                    }
                }

            }));
        }

        private int roomCounter;
        private void activitySystem_UserAdded(object sender, UserEventArgs e)
        {
            AddUserDataToPatientData((Patient)e.User);
        }

        private void AddUserDataToPatientData(Patient patient)
        {
            whiteboard.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                patient.RoomNumber = roomCounter++;
                patient.PropertyChanged += p_PropertyChanged;
                whiteboard.Patients.Add(patient);
            }));
        }

        void p_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var p = (Patient)sender;
            System.Threading.Tasks.Task.Factory.StartNew(()=>
                {
                     activitySystem.UpdateUser<Patient>(p.Id, p);
                });
        }

        private void btnMap_click(object sender, RoutedEventArgs e)
        {
            txtMap.Text = (popup.IsOpen = !popup.IsOpen) ? "Close Map" : "Map";
        }

        private void btnLocationTracking_Click_1(object sender, RoutedEventArgs e)
        {
            IsLocationEnabled = !IsLocationEnabled;
        }

        private void btnDiscovery_Click_1(object sender, RoutedEventArgs e)
        {
            IsBroadcastEnabled = !IsBroadcastEnabled;
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

    }

}
