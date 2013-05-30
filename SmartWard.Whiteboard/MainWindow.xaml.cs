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
using SmartWard.Whiteboard.Model; 

namespace SmartWard.Whiteboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ActivitySystem activitySystem;

        public ObservableCollection<Activity> Patients { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.WindowStyle = System.Windows.WindowStyle.None;
            this.WindowState = System.Windows.WindowState.Maximized;

            this.DataContext = this;

            //activitySystem = new ActivitySystem(System.Configuration.ConfigurationManager.AppSettings["ravenDB"]);
            activitySystem = new ActivitySystem(Net.GetUrl(Net.GetIp(IPType.All), 8080, "").ToString()); ;

            activitySystem.UserAdded+=activitySystem_UserAdded;
            activitySystem.UserRemoved+=activitySystem_UserRemoved;
            activitySystem.UserUpdated+=activitySystem_UserUpdated;

            activitySystem.StartBroadcast(Infrastructure.Discovery.DiscoveryType.Zeroconf, "HyPRBoard", "PIT-Lab");

            activitySystem.StartLocationTracker();


            foreach (var user in activitySystem.Users)
            {
                AddUserDataToPatientData(user);
            }


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
           // popup.IsOpen = true;
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
                    if (whiteboard.Patients[i].User.Id == e.User.Id)
                    {
                        whiteboard.Patients[i].User = e.User;
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
                    if (whiteboard.Patients[i].User.Id == e.Id)
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
            AddUserDataToPatientData(e.User);
        }

        private void AddUserDataToPatientData(User user)
        {
            whiteboard.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                whiteboard.Patients.Add(
                    new Patient()
                    {
                        User = user,
                        RoomNumber = roomCounter++
                    });

            }));
        }

        private void btnMap_click(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = !popup.IsOpen;
            if (popup.IsOpen)
               txtMap.Text = "Close Map";
            else
                txtMap.Text = "Map";
        }

    }

}
