using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Linq;
using SmartWard.Whiteboard.Views.EWS;
using SmartWard.Whiteboard.Views.Note;
using Microsoft.Surface.Presentation.Controls;
using SmartWard.Whiteboard.ViewModels;
using SmartWard.Models;
using SmartWard.Whiteboard.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmartWard.Whiteboard.Views
{
    public partial class BoardLayout
    {
        public BoardLayout()
        {
            InitializeComponent();
        }

        private void BtnEws_Click(object sender, RoutedEventArgs e)
        {
            BoardRowViewModel pvm = (BoardRowViewModel)((SurfaceButton)e.OriginalSource).DataContext;
            DraggablePopup popup = new DraggablePopup(new SmartWard.Whiteboard.Views.EWS.EWSControl { DataContext = pvm.EWSViewModel });
            popup.Placement = PlacementMode.MousePoint;
            popup.StaysOpen = true;
            popup.IsOpen = true;
        }

        private void BtnNote_Click(object sender, RoutedEventArgs e)
        {
            BoardRowViewModel pvm = (BoardRowViewModel)((SurfaceButton)e.OriginalSource).DataContext;
            DraggablePopup popup = new DraggablePopup(new SmartWard.Whiteboard.Views.Note.NoteControl { DataContext = pvm.NoteViewModel });
            popup.Placement = PlacementMode.MousePoint;
            popup.StaysOpen = true;
            popup.IsOpen = true;
        }

        private void BtnDayClinicians_Click(object sender, RoutedEventArgs e)
        {
            BoardRowViewModel pvm = (BoardRowViewModel)((SurfaceButton)e.OriginalSource).DataContext;
            SelectClinicianViewModel vm = new SelectClinicianViewModel() { AllClinicians = pvm.Parent.Clinicians, SelectedClinicians = new ObservableCollection<ClinicianViewModel>(pvm.DayClinicians) };
            vm.SelectedClinicians.CollectionChanged += pvm.DayClinicians_CollectionChanged;
            DraggablePopup popup = new DraggablePopup(new SmartWard.Whiteboard.Controls.SelectClinicianControl() { DataContext = vm });
            popup.Placement = PlacementMode.MousePoint;
            popup.StaysOpen = true;
            popup.IsOpen = true;
        }
    }
}
