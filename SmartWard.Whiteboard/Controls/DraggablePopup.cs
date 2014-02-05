using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SmartWard.Whiteboard.Controls
{
    public partial class DraggablePopup : Popup
    {
        Button _sourceButton;
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


            _sourceButton = sourceButton;
            _sourceButton.PreviewMouseDown += swallowDatMouseEvent;
            _sourceButton.PreviewTouchDown += swallowDatTouchEvent;
            this.Closed += StopSwallowing;
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
