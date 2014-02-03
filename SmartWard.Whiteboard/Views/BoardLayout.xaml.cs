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
            BoardRowPatientViewModel pvm = (BoardRowPatientViewModel)((SurfaceButton)e.OriginalSource).DataContext;
            DraggablePopup popup = new DraggablePopup(new SmartWard.Whiteboard.Views.EWS.EWSControl { DataContext = pvm.EWSViewModel });
            popup.Placement = PlacementMode.MousePoint;
            popup.StaysOpen = true;
            popup.IsOpen = true;
        }

        private void BtnNote_Click(object sender, RoutedEventArgs e)
        {
            BoardRowPatientViewModel pvm = (BoardRowPatientViewModel)((SurfaceButton)e.OriginalSource).DataContext;
            DraggablePopup popup = new DraggablePopup(new SmartWard.Whiteboard.Views.Note.NoteControl { DataContext = pvm.NoteViewModel });
            popup.Placement = PlacementMode.MousePoint;
            popup.StaysOpen = true;
            popup.IsOpen = true;
        }

        private void BtnDayClinicians_Click(object sender, RoutedEventArgs e)
        {
            BoardRowPatientViewModel pvm = (BoardRowPatientViewModel)((SurfaceButton)e.OriginalSource).DataContext;
            OpenClinicianAssignmentPopup(pvm, Clinician.AssignmentType.Day);
        }

        private void BtnEveningClinicians_Click(object sender, RoutedEventArgs e)
        {
            BoardRowPatientViewModel pvm = (BoardRowPatientViewModel)((SurfaceButton)e.OriginalSource).DataContext;
            OpenClinicianAssignmentPopup(pvm, Clinician.AssignmentType.Evening);
        }

        private void BtnNightClinicians_Click(object sender, RoutedEventArgs e)
        {
            BoardRowPatientViewModel pvm = (BoardRowPatientViewModel)((SurfaceButton)e.OriginalSource).DataContext;
            OpenClinicianAssignmentPopup(pvm, Clinician.AssignmentType.Night);
        }

        private void BtnRoundsClinicians_Click(object sender, RoutedEventArgs e)
        {
            BoardRowPatientViewModel pvm = (BoardRowPatientViewModel)((SurfaceButton)e.OriginalSource).DataContext;
            OpenClinicianAssignmentPopup(pvm, Clinician.AssignmentType.Rounds);
        }

        private void OpenClinicianAssignmentPopup(BoardRowPatientViewModel source, SmartWard.Models.Clinician.AssignmentType assignmentType)
        {
            //Create a list of assignable clinicians
            var assignableClinicians = source.Parent.Clinicians.Select(cvm => new AssignableClinicianViewModel(cvm.Clinician, source.Patient, assignmentType, source.WardNode));
            AssignableCliniciansListViewModel vm = new AssignableCliniciansListViewModel(assignableClinicians.ToList());
            DraggablePopup popup = new DraggablePopup(new SmartWard.Whiteboard.Controls.AssignClinicianControl() { DataContext = vm });
            popup.Placement = PlacementMode.MousePoint;
            popup.StaysOpen = true;
            popup.IsOpen = true;
        }
    }
}
