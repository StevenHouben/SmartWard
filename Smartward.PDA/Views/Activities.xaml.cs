using System;
using System.Windows;
using SmartWard.PDA.ViewModels;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;

namespace SmartWard.PDA.Views
{
    /// <summary>
    /// Interaction logic for Activities.xaml
    /// </summary>
    public partial class Activities : Page
    {
        public Activities()
        {
            InitializeComponent();
            InitializeMapOverlay();
        }
        
        private void InitializeMapOverlay()
        {
            var sysRect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
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

        void popup_Down(object sender, EventArgs e)
        {
            popup.IsOpen = false;
        }

        private void btnMap_click(object sender, RoutedEventArgs e)
        {
            txtMap.Text = popup != null && (popup.IsOpen = !popup.IsOpen) ? "Close Map" : "Map";
        }

        private void btnSettings_click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLogout_click(object sender, RoutedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.NavigationService.RemoveBackEntry();
        }
    }
}
