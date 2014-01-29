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
        #region properties
        private string _text;
        private bool _fasting;
       
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

        public Note(string patientId, string text) : base(patientId)
        {
            Type = typeof(Note).Name;
            _text = text;
        }

    }
}
