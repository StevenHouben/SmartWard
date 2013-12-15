using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SmartWard.Whiteboard.Controls
{
    public partial class DraggablePopup : Popup
    {
        public DraggablePopup(UserControl userControl)
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
        }
    }
}
