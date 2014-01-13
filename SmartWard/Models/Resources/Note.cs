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
       
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        #endregion

        public Note(string patientId, string text) : base(patientId)
        {
            Type = typeof(Note).Name;
            _text = text;
        }

    }
}
