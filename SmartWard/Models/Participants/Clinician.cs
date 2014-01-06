using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABC.Model.Users;

namespace SmartWard.Models
{
    public class Clinician : User
    {
        private ClinicianType _clinicianType;

        public ClinicianType ClinicianType1
        {
            get { return _clinicianType; }
            set { _clinicianType = value; }
        } 

        public Clinician(string name, ClinicianType clinicianType)
        {
            Type = typeof(Clinician).Name;
            Name = name;
            _clinicianType = clinicianType;
        }

        /// <summary>
        /// Returns initials based on Name.
        /// </summary>
        /// <returns>Initials, eg. Søren Buron, returns SB</returns>
        public string GetInitials()
        {
            return Name.Split(' ').ToList().Aggregate("", (current, next) => current += next.Substring(0, 1)).ToUpper();
        }

        public enum ClinicianType
        {
            Intern,
            Nurse,
            Doctor,
            Chief
        }
    }
}
