using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Models.Activities
{
    public class Activity : NooSphere.Model.Activity
    {
        private string _clinicianId;

        #region Properties
        public string ClinicianId
        {
            get { return _clinicianId; }
            set { _clinicianId = value; }
        }
        public string Info { get; set; }
        public string Status { get; set; }
        
        #endregion

        public Activity(string clinicianId)
        {
            Type = typeof(Activity).Name;
            ClinicianId = clinicianId;
        }
    }
}
