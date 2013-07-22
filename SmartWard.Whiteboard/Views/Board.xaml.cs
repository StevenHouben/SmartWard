using System;
using System.Windows;
using SmartWard.Whiteboard.ViewModel;


namespace SmartWard.Whiteboard.Views
{
    public partial class Board
    {
        public Board()
        {
            InitializeComponent();
            InitializeMapOverlay();

            ListBoxExtensions.SetAllowReorderSource(Whiteboard.BoardView,true);
            ListBoxExtensions.Reordered += (sender, e) => ((BoardViewModel)DataContext).ReorganizeDragAndDroppedPatients(e.DroppedData,e.OriginalData);
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
    }
   
}