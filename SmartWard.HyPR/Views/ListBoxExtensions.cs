﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace SmartWard.HyPR.Views
{
    public class ListBoxExtensions
    {
        public static readonly DependencyProperty AllowReorderProperty = DependencyProperty.RegisterAttached(
          "AllowReorder",
          typeof(Boolean),
          typeof(ListBoxExtensions),
            new UIPropertyMetadata(
                (sender, e) => InitializeProperty(sender)));


        private static DragAdorner _adorner;
        private static AdornerLayer _layer;
        private static ListBox _listbox;

        private static Window _window;

        public class DropData 
        {
            public object DroppedData { get; set; }
            public object OriginalData { get; set; }
        }

        public static event EventHandler<DropData> Reordered;
        private static void InitializeProperty(DependencyObject sender)
        {
            var listBox = sender as ListBox;
            if (listBox == null)
                throw new ArgumentException("Property can only be used on ListBox");

            _listbox = listBox;

            var style = listBox.ItemContainerStyle;

            style.Setters.Add(new Setter(UIElement.AllowDropProperty, true));

            style.Setters.Add(new EventSetter(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(Input_Down)));
            style.Setters.Add(new EventSetter(UIElement.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(Mouse_Up)));

            style.Setters.Add(new EventSetter(UIElement.PreviewTouchDownEvent, new EventHandler<TouchEventArgs>(Input_Down)));
            style.Setters.Add(new EventSetter(UIElement.PreviewTouchUpEvent, new EventHandler<TouchEventArgs>(Input_Up)));


            style.Setters.Add(new EventSetter(UIElement.DragOverEvent, new DragEventHandler(Handler_Over)));
            style.Setters.Add(new EventSetter(UIElement.DropEvent, new DragEventHandler(Handle_Drop)));

            listBox.ItemContainerStyle = style;

            _window = Window.GetWindow(_listbox);
        }

        private static void ResetAdorner()
        {
            if (_adorner != null)
            {
                _layer.Remove(_adorner);
                _adorner = null;
            }
        }

        private static void Handler_Over(object sender, DragEventArgs e)
        {
            var position = e.GetPosition(_window);
            _adorner.Left = position.X-200;
            _adorner.Top = position.Y-100;
        }
        private static void Handle_Drop(object sender, DragEventArgs e)
        {
            _layer.Remove(_adorner);
            _adorner = null;
            if (Reordered != null)
                Reordered(_listbox, new DropData
                {
                    OriginalData = ((ListBoxItem)(sender)).DataContext,
                    DroppedData =  e.Data
                });
        }

        private static void Input_Up(object sender, System.Windows.Input.TouchEventArgs e)
        {
            ResetAdorner();
        }

        private static void Mouse_Up(object sender, MouseButtonEventArgs e)
        {
            ResetAdorner();
        }

        private static void Input_Down(object sender, RoutedEventArgs e)
        {

            if (EventTriggeredByButtonWithCommand(e))
                return;

            var draggedItem = sender as FrameworkElement;

            if (draggedItem != null)
                StartDrag(draggedItem);

        }

        private static bool EventTriggeredByButtonWithCommand(RoutedEventArgs e)
        {
            var frameWorkElement = e.OriginalSource as FrameworkElement;

            if (frameWorkElement == null)
                return false;

            var button = frameWorkElement.TemplatedParent as Button;

            if (button == null)
                return false;

            return button.Command != null;
        }


        private static void StartDrag(FrameworkElement draggedItem)
        {
            ResetAdorner();
            _adorner = new DragAdorner(draggedItem);
            _layer = AdornerLayer.GetAdornerLayer(_listbox);
            _layer.Add(_adorner);

            //blocking call
            DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);

            ResetAdorner();
        }

        public static void SetAllowReorderSource(UIElement element, Boolean value)
        {
            element.SetValue(AllowReorderProperty, value);
        }
        public static Boolean GetAllowReorderSource(UIElement element)
        {
            return (Boolean)element.GetValue(AllowReorderProperty);
        }
    }
}
