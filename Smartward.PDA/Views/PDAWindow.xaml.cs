using SmartWard.PDA.Helpers;
using SmartWard.PDA.ViewModels;
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
    /// Interaction logic for PDAWindow.xaml
    /// </summary>
    public partial class PDAWindow : Window
    {
        public PDAWindow()
        {
            InitializeComponent();
            InitializeMapOverlay();
        }

        public void InitializeFrame()
        {
            InitializeContentFrame();
        }

        private void click_Close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PatientsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigationService.Navigate(new Patients() { DataContext = new PatientsViewModel(new List<String>(), ((WindowViewModel)this.DataContext).WardNode) });
        }

        private void InitializeMapOverlay()
        {
            var sysRect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            var rect = new Rect(
                0,
                0,
                780,
                400);
            //popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute;
            //popup.PlacementTarget = ContentFrame;
            popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Center;
            //popup.PlacementRectangle = rect;
            popup.Width = rect.Width;
            popup.Height = rect.Height;
            popup.AllowsTransparency = true;
            popup.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popup.MouseDown += popup_Down;
            popup.TouchDown += popup_Down;
        }

        void popup_Down(object sender, EventArgs e)
        {
            popup.IsOpen = false;
        }

        private void btnMap_click(object sender, RoutedEventArgs e)
        {
            txtMap.Text = popup != null && (popup.IsOpen = !popup.IsOpen) ? "Close Map" : "Map";
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationHelper.User = null;
            InitializeContentFrame();
        }

        private void InitializeContentFrame()
        {
            LoginView loginView = new LoginView();
            loginView.DataContext = new AuthenticatedViewModel(((WindowViewModel)this.DataContext).WardNode);

            ContentFrame.NavigationService.Navigate(loginView);
        }
    }
}
