using System;
using System.Windows.Input;
using NooSphere.Model.Primitives;
using SmartWard.Commands;
using SmartWard.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;

namespace SmartWard.ViewModels
{
    public class ClinicianViewModelBase : UserViewModelBase
    {
        public ClinicianViewModelBase(Clinician clinician) : base(clinician) 
        {
            clinician.PropertyChanged += ClinicianNameChanged;
        }

        public Clinician Clinician { get { return User as Clinician; } }

        #region Clinician Properties
        public Clinician.ClinicianTypeEnum ClinicianType
        {
            get { return Clinician.ClinicianType; }
            set
            {
                Clinician.ClinicianType = value;
                OnPropertyChanged("ClinicianType");
            }
        }
        public IList<Tuple<string, SmartWard.Models.Clinician.AssignmentType>> AssignedPatients
        {
            get { return Clinician.AssignedPatients; }
        }

        public string Initials
        {
            get { return Clinician.Initials(); }
        }

        private void ClinicianNameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name")
            {
                OnPropertyChanged("Initials");
            }
        }
        #endregion
    }
}