using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using SmartWard.Whiteboard.ViewModel;

using DragDropEffects = System.Windows.DragDropEffects;
using DragEventHandler = System.Windows.DragEventHandler;

namespace SmartWard.Whiteboard.Views
{
    public partial class Board
    {
        private bool _isDragging;
  
        public Board()
        {
             InitializeComponent();

            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Maximized;

            InitializeMapOverlay();

            var style = Whiteboard.BoardView.ItemContainerStyle;

            style.Setters.Add(new Setter(AllowDropProperty, true));

            style.Setters.Add(new EventSetter(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(Input_Down)));
            style.Setters.Add(new EventSetter(PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(Mouse_Up)));


            style.Setters.Add(new EventSetter(PreviewTouchDownEvent, new EventHandler<TouchEventArgs>(Input_Down)));
            style.Setters.Add(new EventSetter(PreviewTouchUpEvent, new EventHandler<TouchEventArgs>(Input_Up)));


            style.Setters.Add(new EventSetter(DragOverEvent, new DragEventHandler(Over)));
            style.Setters.Add(new EventSetter(DropEvent, new DragEventHandler(Drop)));
            style.Setters.Add(new EventSetter(QueryContinueDragEvent, new QueryContinueDragEventHandler(Query)));

            Whiteboard.BoardView.ItemContainerStyle = style;
        }

        private void Query(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Cancel)
            {
                _layer.Remove(_adorner);
                _adorner = null;
            }
        }

        private void Over(object sender, DragEventArgs e)
        {
            var position = e.GetPosition(Whiteboard);
            _adorner.Left = position.X;
            _adorner.Top = position.Y;
        }
        private new void Drop(object sender,DragEventArgs e)
        {
            _isDragging = false;

            _layer.Remove(_adorner);
            _adorner = null;
            ((BoardViewModel)DataContext).ReorganizeDragAndDroppedPatients(e.Data,
                ((SurfaceListBoxItem)(sender)).DataContext);
        }

        private void Input_Up(object sender, EventArgs e)
        {
            if (_adorner != null)
            {
                _layer.Remove(_adorner);
                _adorner = null;
            }

        }

        private void Mouse_Up(object sender, MouseButtonEventArgs e)
        {
            if (_adorner != null)
            {
                _layer.Remove(_adorner);
                _adorner = null;
            }

        }

        private void Input_Down(object sender, EventArgs e)
        {
            if (!(sender is SurfaceListBoxItem))
                return;

            var draggedItem = sender as SurfaceListBoxItem;

            _isDragging = true;
            StartDrag(draggedItem);
        }

        private DragAdorner _adorner;
        private AdornerLayer _layer;
        private void StartDrag(SurfaceListBoxItem draggedItem)
        {
            if (_adorner != null)
            {
                _layer.Remove(_adorner);
                _adorner = null;
            }
            _adorner = new DragAdorner(draggedItem);
            _layer = AdornerLayer.GetAdornerLayer(Grid);
            _layer.Add(_adorner);

            draggedItem.IsSelected = true;
            DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);

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