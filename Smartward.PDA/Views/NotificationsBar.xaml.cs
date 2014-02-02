using Microsoft.Surface.Presentation.Controls;
using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.Models.Notifications;
using SmartWard.PDA.Controllers;
using SmartWard.PDA.ViewModels;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
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

namespace SmartWard.PDA.Views
{
    /// <summary>
    /// Interaction logic for NotificationsBar.xaml
    /// </summary>
    public partial class NotificationsBar : UserControl
    {
        public NotificationsBar()
        {
            InitializeComponent();
        }

        private void createDummyNotification(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)DataContext).WardNode.AddNotification(new Notification(new List<string>(), "referenceID", "IResource", "Notificationlol"));
        }

        private void SurfaceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SurfaceListBox src = (SurfaceListBox)e.Source;
            if (src.SelectedItems.Count > 0)
            {
                NotificationViewModel notificationViewModel = (NotificationViewModel)src.SelectedItems[0];
                Notification n = notificationViewModel.Notification;
                WardNode wardNode = ((WindowViewModel)DataContext).WardNode;
                switch (n.ReferenceType)
                {
                    case "EWS":
                        EWS ews = (EWS) wardNode.ResourceCollection.Where(r => r.Id.Equals(n.ReferenceId)).ToList().FirstOrDefault();
                        NotificationsPopup.IsOpen = false;
                        ((PDAWindow)Application.Current.MainWindow).ContentFrame.Navigate(new EWSView() { DataContext = new EWSViewModel(ews, wardNode) });
                        break;
                    default:
                        break;
                }

                n.SeenBy.Add(AuthenticationController.User.Id);
                wardNode.UpdateNotification(n);
            }             
        }
    }
}
