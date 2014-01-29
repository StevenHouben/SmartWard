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

        public ClinicianTypeEnum ClinicianType
        {
            get { return _clinicianType; }
            set { _clinicianType = value; }
        } 

        public Clinician(ClinicianTypeEnum clinicianType)
        {
            Type = typeof(Clinician).Name;
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

        public enum ClinicianTypeEnum
        {
            Intern,
            Nurse,
            Doctor,
            Chief
        }
    }
}
