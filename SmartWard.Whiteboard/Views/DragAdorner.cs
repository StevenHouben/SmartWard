using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartWard.Whiteboard.Views
{
    public class DragAdorner : Adorner
    {
        private Rectangle _child;
        private double _left;
        private double _top;

        // Be sure to call the base class constructor. 
        public DragAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            var brush = new VisualBrush(adornedElement);

            _child = new Rectangle();
            _child.Width = adornedElement.RenderSize.Width;
            _child.Height = adornedElement.RenderSize.Height;
            _child.Fill = brush;

        }

        protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _child.Arrange(new Rect(finalSize));
            return base.ArrangeOverride(finalSize);
        }

        protected override Visual GetVisualChild(int index)
        {
            return _child;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(new TranslateTransform(Left, Top));
            return result;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        public Double Left
        {
            get { return _left; }
            set
            {
                _left = value;
                UpdatePosition();
            }
        }

        public Double Top
        {
            get { return _top; }
            set
            {
                _top = value;
                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            var layer = Parent as AdornerLayer;
            if (layer != null)
                layer.Update(AdornedElement);
        }
    }
}
