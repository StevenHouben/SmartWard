using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABC.Model.Resources;

namespace SmartWard.Models 
{
    public class Note : Resource
    {
        private string _patientId;
        private string _clinicianId;
        private DateTime _timestamp;

        #region properties
        private string _text;
        public string PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }
        public string ClinicianId
        {
            get { return _clinicianId; }
            set { _clinicianId = value; }
        }
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        #endregion

        public Note(string patientId, string text) : base(patientId)
        {
            Type = typeof(Note).Name;
            _patientId = patientId;
            //_clinicianId = clinicianId;
            _timestamp = DateTime.Now;
            _text = text;
        }

    }
}
