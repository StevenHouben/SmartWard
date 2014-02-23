using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NooSphere.Model;

namespace SmartWard.Models
{
    public class VisitActivity : Activity
    {
        private string _patientId;
        private bool _isDone;
        private DateTime _timeOfCompletion;

        public VisitActivity(string patientId)
        {
            Type = typeof(VisitActivity).Name;
            _patientId = patientId;
        }

        public string PatientId
        {
            get { return _patientId; }
            set { 
                _patientId = value;
                OnPropertyChanged("PatientId");
            }
        }

        public bool IsDone
        {
            get { return _isDone; }
            set
            {
                if (_isDone && value) throw new InvalidOperationException("Visit is already done"); 
                _isDone = value;
                if (value) TimeOfCompletion = DateTime.Now;
                OnPropertyChanged("IsDone");
            }
        }

        public void MarkAsDone()
        {
            IsDone = true;
        }

        public DateTime TimeOfCompletion
        {
            get { return _timeOfCompletion; }
            set
            {
                _timeOfCompletion = value;
                OnPropertyChanged("TimeOfCompletion");
            }
        }

    }
}
