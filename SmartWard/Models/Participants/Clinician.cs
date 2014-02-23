﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NooSphere.Model.Users;
using System.ComponentModel;

namespace SmartWard.Models
{
    public class Clinician : User
    {
        private ClinicianTypeEnum _clinicianType;
        private IList<Tuple<string, AssignmentType>> _assignedPatients;
        private string _nfcId;
        private string _currentActivity;

        public Clinician(ClinicianTypeEnum clinicianType, string nfcId)
        {
            Type = typeof(Clinician).Name;
            _clinicianType = clinicianType;
            _assignedPatients = new List<Tuple<string, AssignmentType>>();
            NfcId = nfcId;
        }

        #region Properties
        public string NfcId 
        {
            get { return _nfcId; }
            set
            {
                _nfcId = value;
                OnPropertyChanged("NfcId");
            }
        }
        public ClinicianTypeEnum ClinicianType
        {
            get { return _clinicianType; }
            set 
            { 
                _clinicianType = value;
                OnPropertyChanged("ClinicianType");
            }
        }
        public IList<Tuple<string, AssignmentType>> AssignedPatients
        {
            get { return _assignedPatients; }
            protected set
            {
                _assignedPatients = value;
                OnPropertyChanged("AssignedPatients");
            }
        }
        public string CurrentActivity
        {
            get { return _currentActivity; }
            set
            {
                _currentActivity = value;
                OnPropertyChanged("CurrentActivity");
            }
        }
        /// <summary>
        /// Returns initials based on Name.
        /// </summary>
        /// <returns>Initials, eg. Søren Buron, returns SB</returns>
        public string Initials()
        {
           return Name.Split(' ').ToList().Aggregate("", (current, next) => current += next.Substring(0, 1)).ToUpper();
        }
        private void NameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name")
            {
                OnPropertyChanged("Initials");
            }
        }
        #endregion

        #region Clinician Enumerations
        public enum ClinicianTypeEnum
        {
            Intern,
            Nurse,
            Doctor,
            Chief
        }

        public enum AssignmentType
        {
            Day,
            Evening,
            Night,
            Rounds
        }
        #endregion
    }
}
