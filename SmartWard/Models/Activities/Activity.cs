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
        private string _status;
        private string _info;

        #region Properties
        public string ClinicianId
        {
            get { return _clinicianId; }
            set 
            { 
                _clinicianId = value;
                OnPropertyChanged("ClinicianId");
            }
        }
        public string Info 
        {
            get { return _info; }
            set
            {
                _info = value;
                OnPropertyChanged("Info");
            }
        }
        public string Status 
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }
        
        #endregion

        public Activity(string clinicianId)
        {
            Type = typeof(Activity).Name;
            ClinicianId = clinicianId;
        }
    }
}
