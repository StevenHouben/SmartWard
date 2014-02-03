using SmartWard.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Models 
{
    public class Note : Resource
    {
        private string _patientId;
        private string _text;
        private bool _fasting;
        public Note(string patientId, string text)
        {
            Type = typeof(Note).Name;
            _patientId = patientId;
            _text = text;
        }
        #region properties
        public string PatientId
        {
            get { return _patientId; }
            set
            {
                _patientId = value;
                OnPropertyChanged("patientId");
            }
        }
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("text");
            }
        }
        public bool Fasting
        {
            get { return _fasting; }
            set
            {
                _fasting = value;
                OnPropertyChanged("fasting");
            }
        }
        #endregion
    }
}
