using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NooSphere.Model.Users;

namespace SmartWard.Models
{
    public class Clinician : User
    {
        private ClinicianTypeEnum _clinicianType;
        private IList<Tuple<string, AssignmentType>> _assignedPatients;

        #region Properties
        public ClinicianTypeEnum ClinicianType
        {
            get { return _clinicianType; }
            set { _clinicianType = value; }
        }

        public string NfcId { get; set; }

        #endregion
        public Clinician(ClinicianTypeEnum clinicianType, string nfcId)
        {
            Type = typeof(Clinician).Name;
            _clinicianType = clinicianType;
            NfcId = nfcId;
            _assignedPatients = new List<Tuple<string, AssignmentType>>();
        }

        /// <summary>
        /// Returns initials based on Name.
        /// </summary>
        /// <returns>Initials, eg. Søren Buron, returns SB</returns>
        public string GetInitials()
        {
            return Name.Split(' ').ToList().Aggregate("", (current, next) => current += next.Substring(0, 1)).ToUpper();
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
    }
}
