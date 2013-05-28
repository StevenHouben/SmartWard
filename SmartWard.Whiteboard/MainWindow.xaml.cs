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
            activitySystem = new ActivitySystem(System.Configuration.ConfigurationManager.AppSettings["ravenDB"]);

            activitySystem.UserAdded+=activitySystem_UserAdded;
            activitySystem.UserRemoved+=activitySystem_UserRemoved;
            activitySystem.UserUpdated+=activitySystem_UserUpdated;

            activitySystem.StartBroadcast(Infrastructure.Discovery.DiscoveryType.Zeroconf, "HyPRBoard", "PIT-Lab");

            activitySystem.StartLocationTracker();

            Users = new ObservableCollection<User>(activitySystem.Users);

            Style itemContainerStyle = view.ItemContainerStyle;
            itemContainerStyle.Setters.Add(new Setter(SurfaceListBoxItem.AllowDropProperty, true));
            itemContainerStyle.Setters.Add(new EventSetter(SurfaceListBoxItem.PreviewTouchDownEvent, new EventHandler<TouchEventArgs>(touchHandler)));
            itemContainerStyle.Setters.Add(new EventSetter(SurfaceListBoxItem.PreviewMouseDownEvent, new MouseButtonEventHandler(mouseHandler)));
            itemContainerStyle.Setters.Add(new EventSetter(SurfaceListBoxItem.DropEvent, new DragEventHandler(view_Drop)));
            itemContainerStyle.Setters.Add(new EventSetter(SurfaceListBoxItem.DropEvent, new DragEventHandler(view_Drop)));
        }

        void touchHandler(object sender, TouchEventArgs e)
        {
            e.TouchDevice.GetTouchPoint(this);
            if (sender is SurfaceListBoxItem)
            {
                SurfaceListBoxItem draggedItem = sender as SurfaceListBoxItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.None);
                draggedItem.IsSelected = true;
            }
        }
        void mouseHandler(object sender, MouseEventArgs e)
        {
            if (sender is SurfaceListBoxItem)
            {
                SurfaceListBoxItem draggedItem = sender as SurfaceListBoxItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                draggedItem.IsSelected = true;
            }
        }

        void view_Drop(object sender, DragEventArgs e)
        {
            User droppedData = e.Data.GetData(typeof(User)) as User;
            User target = ((ListBoxItem)(sender)).DataContext as User;

            int removedIdx = view.Items.IndexOf(droppedData);
            int targetIdx = view.Items.IndexOf(target);

            if (removedIdx < targetIdx)
            {
                Users.Insert(targetIdx + 1, droppedData);
                Users.RemoveAt(removedIdx);
            }
            else
            {
                int remIdx = removedIdx + 1;
                if (Users.Count + 1 > remIdx)
                {
                    Users.Insert(targetIdx, droppedData);
                    Users.RemoveAt(remIdx);
                }
            }
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

    }

}
