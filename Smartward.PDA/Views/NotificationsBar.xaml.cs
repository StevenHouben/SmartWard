using SmartWard.Models.Notifications;
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
            ((ViewModelBase)DataContext).NotificationsNode.AddNotification(new Notification(new List<string>(), "referenceID", "IResource", "Notificationlol"));
        }
    }
}
