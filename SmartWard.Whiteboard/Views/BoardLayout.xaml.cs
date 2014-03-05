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
            DraggablePopup popup = new DraggablePopup(new SmartWard.Whiteboard.Views.EWS.EWSControl { DataContext = pvm.EWSViewModel }, (SurfaceButton)e.OriginalSource);
            popup.Placement = PlacementMode.MousePoint;
            popup.StaysOpen = true;
            popup.IsOpen = true;
        }

        private void BtnNote_Click(object sender, RoutedEventArgs e)
        {
            BoardRowPatientViewModel pvm = (BoardRowPatientViewModel)((SurfaceButton)e.OriginalSource).DataContext;
            DraggablePopup popup = new DraggablePopup(new SmartWard.Whiteboard.Views.Note.NoteControl { DataContext = pvm.NoteViewModel }, (SurfaceButton)e.OriginalSource);
            popup.Placement = PlacementMode.MousePoint;
            popup.StaysOpen = true;
            popup.IsOpen = true;
        }

        private void BtnDayClinicians_Click(object sender, RoutedEventArgs e)
        {
            BoardRowPatientViewModel pvm = (BoardRowPatientViewModel)((SurfaceButton)e.OriginalSource).DataContext;
            OpenClinicianAssignmentPopup(pvm, Clinician.AssignmentType.Day, (SurfaceButton)e.OriginalSource);
        }

        private void BtnEveningClinicians_Click(object sender, RoutedEventArgs e)
        {
            BoardRowPatientViewModel pvm = (BoardRowPatientViewModel)((SurfaceButton)e.OriginalSource).DataContext;
            OpenClinicianAssignmentPopup(pvm, Clinician.AssignmentType.Evening, (SurfaceButton)e.OriginalSource);
        }

        private void BtnNightClinicians_Click(object sender, RoutedEventArgs e)
        {
            BoardRowPatientViewModel pvm = (BoardRowPatientViewModel)((SurfaceButton)e.OriginalSource).DataContext;
            OpenClinicianAssignmentPopup(pvm, Clinician.AssignmentType.Night, (SurfaceButton)e.OriginalSource);
        }

        private void BtnRoundsClinicians_Click(object sender, RoutedEventArgs e)
        {
            BoardRowPatientViewModel pvm = (BoardRowPatientViewModel)((SurfaceButton)e.OriginalSource).DataContext;
            OpenClinicianAssignmentPopup(pvm, Clinician.AssignmentType.Rounds, (SurfaceButton)e.OriginalSource);
        }

        private void OpenClinicianAssignmentPopup(BoardRowPatientViewModel source, SmartWard.Models.Clinician.AssignmentType assignmentType, SurfaceButton sourceButton)
        {
            //Create a list of assignable clinicians
            List<AssignableClinicianViewModel> assignableClinicians = new List<AssignableClinicianViewModel>();
            switch (assignmentType)
            {
                case Clinician.AssignmentType.Rounds:
                    assignableClinicians = source.Parent.Clinicians.Where(c => c.ClinicianType == Clinician.ClinicianTypeEnum.Doctor).Select(cvm => new AssignableClinicianViewModel(cvm.Clinician, source.Patient, assignmentType, source.WardNode)).ToList();
                    break;
                default:
                    assignableClinicians = source.Parent.Clinicians.Where(c => c.ClinicianType == Clinician.ClinicianTypeEnum.Nurse).Select(cvm => new AssignableClinicianViewModel(cvm.Clinician, source.Patient, assignmentType, source.WardNode)).ToList();
                    break;
            }
            AssignableCliniciansListViewModel vm = new AssignableCliniciansListViewModel(assignableClinicians);
            DraggablePopup popup = new DraggablePopup(new SmartWard.Whiteboard.Controls.AssignClinicianControl() { DataContext = vm }, sourceButton);
            popup.Placement = PlacementMode.MousePoint;
            popup.StaysOpen = true;
            popup.IsOpen = true;
        }
    }
}
