using Microsoft.Surface.Presentation.Controls;
using SmartWard.Models;
using SmartWard.Models.Notifications;
using SmartWard.ViewModels;
using SmartWard.Whiteboard.ViewModels;
using SmartWard.Whiteboard.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SmartWard.Whiteboard.Controls
{
    public partial class DraggablePopup : Popup
    {
        Button _sourceButton;
        ResourceViewModelBase _dataContext;

        public DraggablePopup(UserControl userControl, Button sourceButton)
        {
            var thumb = new Thumb
            {
                Width = 0,
                Height = 0,
            };

            MouseDown += (sender, e) =>
            {
                thumb.RaiseEvent(e);
            };

            thumb.DragDelta += (sender, e) =>
            {
                HorizontalOffset += e.HorizontalChange;
                VerticalOffset += e.VerticalChange;
            };

            Grid canvas = new Grid();
            canvas.Children.Add(thumb);
            canvas.Children.Add(userControl);
            this.Child = canvas;

            _dataContext = userControl.DataContext as ResourceViewModelBase;


            _sourceButton = sourceButton;
            _sourceButton.PreviewMouseDown += swallowDatMouseEvent;
            _sourceButton.PreviewTouchDown += swallowDatTouchEvent;
            this.Closed += StopSwallowing;

            this.PreviewTouchUp += CheckIfTouchDrop;
            this.PreviewMouseUp += CheckIfMouseDrop;
        }

        private void CheckIfMouseDrop(object sender, MouseEventArgs e)
        {
            CheckIfDrop();
            
        }

        private void CheckIfTouchDrop(object sender, TouchEventArgs e)
        {
            CheckIfDrop();
        }

        private void CheckIfDrop()
        {
            var listBox = ((Board)Application.Current.MainWindow).TabletBar.TabletListBox;
            var coord = Mouse.GetPosition(listBox);
            if (listBox.InputHitTest(coord) != null)
            {
                int index = (int)Math.Floor(coord.X / 202);
                if (listBox.Items.Count > index)
                {
                    string type = null;
                    if (_dataContext is EWSViewModelBase)
                    {
                        type = typeof(EWS).Name;
                    }
                    else if (_dataContext is NoteViewModelBase)
                    {
                        type = typeof(Note).Name;
                    }
                    var n = new PushNotification(new List<string> { ((DeviceViewModelBase)listBox.Items[index]).Owner.Id }, _dataContext.Id, type, "");
                    ((BoardViewModel)((Board)Application.Current.MainWindow).DataContext).WardNode.AddNotification(n);
                    this.IsOpen = false;
                }
            }
        }

        private void StopSwallowing(object sender, EventArgs e)
        {
            _sourceButton.PreviewMouseDown -= swallowDatMouseEvent;
            _sourceButton.PreviewTouchDown -= swallowDatTouchEvent;
        }

        private void swallowDatMouseEvent(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void swallowDatTouchEvent(object sender, TouchEventArgs e)
        {
            e.Handled = true;
        }
    }
}
