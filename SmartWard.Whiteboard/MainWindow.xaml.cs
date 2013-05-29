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

namespace SmartWard.Whiteboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Converters converter = new Converters();
        public ActivitySystem activitySystem;

        public ObservableCollection<User> Users { get; set; }

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

            //activitySystem.StartBroadcast(Infrastructure.Discovery.DiscoveryType.Zeroconf, "HyPRBoard", "PIT-Lab");

            //activitySystem.StartLocationTracker();

            Users = new ObservableCollection<User>(activitySystem.Users);

            var sysRect = Screen.PrimaryScreen.Bounds;
            var rect = new Rect(
                0,
                sysRect.Height,// / 2,
                sysRect.Width,
                sysRect.Height);// / 2); 
            popup.Placement = System.Windows.Controls.Primitives.PlacementMode.AbsolutePoint;
            popup.PlacementTarget = this;
            popup.PlacementRectangle = rect;
            popup.Width = rect.Width;
            popup.Height = rect.Height;
            popup.AllowsTransparency = true;
            popup.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popup.MouseDown += popup_MouseDown;
           // popup.IsOpen = true;
        }

        void popup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            popup.IsOpen = false;
        }   

        private void activitySystem_UserUpdated(object sender, UserEventArgs e)
        {
            view.Dispatcher.BeginInvoke(new System.Action(()=>
            {
                for (int i = 0; i < Users.Count; i++)
                {
                    if (Users[i].Id == e.User.Id)
                    {
                        Users[i] = e.User;
                        break;
                    }
                }
            }));
        }

        private void activitySystem_UserRemoved(object sender, UserRemovedEventArgs e)
        {
            view.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                for (int i = 0; i < Users.Count; i++)
                {
                    if (Users[i].Id == e.Id)
                    {
                        Users.RemoveAt(i);
                        break;
                    }
                }
                
            }));
        }

        private void activitySystem_UserAdded(object sender, UserEventArgs e)
        {
            view.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                Users.Add(e.User);

            }));
        }

        private void btnMap_click(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = !popup.IsOpen;
            if (popup.IsOpen)
                btnMap.Content = "Close Map";
            else
                btnMap.Content = "Map";
        }

    }

}
