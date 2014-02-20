using System;
using System.Windows.Input;
using NooSphere.Model.Primitives;
using SmartWard.Commands;
using SmartWard.Models;
using System.Collections.ObjectModel;
using SmartWard.ViewModels;

namespace SmartWard.AdministrationTool.ViewModels
{
    public class UpdatablePatientViewModel : PatientViewModelBase
    {
        public event EventHandler PatientUpdated;
        
        private ICommand _updatePatientCommand;

        public ICommand UpdatePatientCommand
        {
            get
            {
                return _updatePatientCommand ?? (_updatePatientCommand = new RelayCommand(
                    param => UpdatePatient(),
                    param => true //Patients may always be saved
                    ));
            }
        }

        public void UpdateAllProperties(Patient updatedPatient)
        {
            Patient.UpdateAllProperties(updatedPatient);
        }

        public void UpdatePatient()
        {
            if (PatientUpdated != null)
                PatientUpdated(Patient, new EventArgs());
        }

        public UpdatablePatientViewModel(Patient patient) : base(patient) { }
    }
}