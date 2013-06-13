using System.Linq;
using SmartWard.Infrastructure.ActivityBase;
using SmartWard.Infrastructure.Context.Location;
using SmartWard.Model;
using SmartWard.Users;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using SmartWard.Infrastructure.Helpers;
using System.Windows.Forms;
using System.ComponentModel;
using ButtonState = SmartWard.Infrastructure.Context.Location.ButtonState;
using MessageBox = System.Windows.MessageBox;

namespace SmartWard.Whiteboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public ActivitySystem ActivitySystem;
        public ActivityService ActivityService;
        public ActivityClient Client;

        public ObservableCollection<Activity> Patients { get; set; }

        private bool _isLocationEnabled = true;
        public bool IsLocationEnabled
        {
            get { return _isLocationEnabled; }
            set 
            {
                _isLocationEnabled = value;
                if (ActivitySystem != null)
                {
                    if (!IsLocationEnabled)
                        ActivitySystem.StopLocationTracker();
                    else
                        ActivitySystem.StartLocationTracker();
                }
                OnPropertyChanged("isLocationEnabled");
            }
        }

        private bool _isWebApiEnabled = true;
        public bool IsWebApiEnabled
        {
            get { return _isWebApiEnabled; }
            set 
            {
                _isWebApiEnabled = value;
                if (ActivityService != null)
                {
                    if (!_isWebApiEnabled)
                        ActivityService.Stop();
                    else
                        ActivityService.Start(Net.GetIp(IPType.All), 8000);
                }
                OnPropertyChanged("isWebApiEnabled");
            }
        }

        
        private bool _isBroadcastEnabled = true;
        public bool IsBroadcastEnabled
        {
            get { return _isBroadcastEnabled; }
            set
            {
                _isBroadcastEnabled = value;
                if (ActivitySystem != null)
                {
                    if (!IsBroadcastEnabled)
                        ActivitySystem.StopBroadcast();
                    else

                        ActivitySystem.StartBroadcast(Infrastructure.Discovery.DiscoveryType.Zeroconf, "HyPRBoard", "PIT-Lab");
                }
                OnPropertyChanged("isBroadcastEnabled");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Maximized;

            DataContext = this;

            InitializeActivitySystem();

            InitializeUsers();
            InitializeMapOverlay();

        }

        private void InitializeActivitySystem()
        {
            //activitySystem = new ActivitySystem(System.Configuration.ConfigurationManager.AppSettings["ravenDB"]);
            var addr = Net.GetUrl(Net.GetIp(IPType.All), 8080, "").ToString();
            ActivitySystem = new ActivitySystem();

            ActivitySystem.ConnectionEstablished += activitySystem_ConnectionEstablished;

            ActivitySystem.UserAdded += activitySystem_UserAdded;
            ActivitySystem.UserRemoved += activitySystem_UserRemoved;
            ActivitySystem.UserChanged += activitySystem_UserUpdated;

            ActivitySystem.Tracker.TagEnter += Tracker_TagEnter;
            ActivitySystem.Tracker.TagLeave += Tracker_TagLeave;
            ActivitySystem.Tracker.TagButtonDataReceived += Tracker_TagButtonDataReceived;

            ActivitySystem.Run(addr);
            //ActivitySystem.StartBroadcast(Infrastructure.Discovery.DiscoveryType.Zeroconf, "HyPRBoard", "PIT-Lab");

            //ActivitySystem.StartLocationTracker();
        }

        void activitySystem_ConnectionEstablished(object sender, EventArgs e)
        {
            try
            {
                ActivityService = new ActivityService(ActivitySystem, Net.GetIp(IPType.All), 8000);
                ActivityService.ConnectionEstablished += ActivityService_ConnectionEstablished;
                ActivityService.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        void ActivityService_ConnectionEstablished(object sender, EventArgs e)
        {
            Client = new ActivityClient(ActivityService.Ip,ActivityService.Port);
        }

        private void InitializeUsers()
        {
            foreach (var patient in ActivitySystem.Users.Values.OfType<Patient>())
            {
                AddUserDataToPatientData(patient);
            }
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

        void Tracker_TagLeave(Detector detector, TagEventArgs e)
        {
           // Console.WriteLine("{0} left {1}", e.Tag.Name, detector.HostName);
        }

        void Tracker_TagEnter(Detector detector, TagEventArgs e)
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
                foreach (var t in whiteboard.Patients)
                {
                    if (t.Id == e.User.Id)
                    {
                        t.UpdateAllProperties<User>(e.User);
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

        private int _roomCounter;
        private void activitySystem_UserAdded(object sender, UserEventArgs e)
        {
            var patient = e.User as Patient;

            if (patient != null)
                AddUserDataToPatientData((Patient)e.User);

        }

        private void AddUserDataToPatientData(Patient patient)
        {
            whiteboard.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                patient.RoomNumber = _roomCounter++;
                patient.PropertyChanged += p_PropertyChanged;
                whiteboard.Patients.Add(patient);
            }));
        }

        void p_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var p = (Patient)sender;
            System.Threading.Tasks.Task.Factory.StartNew(()=> ActivitySystem.UpdateUser( p));
        }

        private void btnMap_click(object sender, RoutedEventArgs e)
        {
            txtMap.Text = popup != null && (popup.IsOpen = !popup.IsOpen) ? "Close Map" : "Map";
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

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            foreach (var act in Client.Users.Values)
            {
                Client.RemoveUser(act.Id);
            }
        }


        private void btnAddActivity_Click(object sender, RoutedEventArgs e)
        {
            Client.ActivityAdded += Client_ActivityAdded;
            Client.AddActivity(new Treatment());
        }

        void Client_ActivityAdded(object sender, Infrastructure.ActivityEventArgs e)
        {
            MessageBox.Show(e.Activity.Name);
        }

        private void BtnWebApi_OnClick(object sender, RoutedEventArgs e)
        {
            IsWebApiEnabled = !IsWebApiEnabled;
        }
    }
    public class Doctor : User { public string Specialisation { get; set; } }
    public class Treatment : Activity { public int Progress { get; set; } }

}
