using System;
using System.Windows.Input;
using NooSphere.Model.Primitives;
using SmartWard.Commands;
using SmartWard.Models;
using System.Collections.ObjectModel;
using SmartWard.ViewModels;

namespace SmartWard.ViewModels
{
    public class PatientViewModelBase : UserViewModelBase
    {
        public PatientViewModelBase(Patient patient) : base(patient) { }

        public Patient Patient { get { return User as Patient; } }

        #region Patient Properties
        public int RoomNumber
        {
            get { return Patient.RoomNumber; }
            set
            {
                Patient.RoomNumber = value;
                OnPropertyChanged("RoomNumber");
            }
        }
        public string Cpr
        {
            get { return Patient.Cpr; }
            set 
            {
                Patient.Cpr = value;
                OnPropertyChanged("Cpr");
            }
        }
        #endregion
    }
}