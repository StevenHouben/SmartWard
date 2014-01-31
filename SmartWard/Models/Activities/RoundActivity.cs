using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NooSphere.Model.Primitives;
using SmartWard.Models.Activities;
using System.ComponentModel;


namespace SmartWard.Models 
{
    public class RoundActivity : Activity
    {
        private string _clinicianId;
        private bool _isFinished;
        private DateTime? _getTimeOfCompletion;
        private string _status;

        private List<VisitActivity> _visits;
        
        #region Properties
        /// <summary>
        /// Returns true if all visits are finished.
        /// </summary>
        /// <returns></returns>
        public bool IsFinished
        {
            get
            {
                return  _isFinished;
            }
            set
            {
                _isFinished = _visits.Count > 0
                        ? _visits.Select<VisitActivity, bool>(v => v.IsDone).Aggregate((b1, b2) => b1 && b2)
                        : false;
                OnPropertyChanged("IsFinished");
            }
        }
        public DateTime? GetTimeOfCompletion
        {
            get
            {
                return _getTimeOfCompletion;
            }
            set
            {
                _getTimeOfCompletion = IsFinished
                ? _visits.Select<VisitActivity, DateTime>(v => v.TimeOfCompletion).Aggregate(DateTime.MinValue, (d1, d2) => DateTime.Compare(d1, d2) > 0 ? d1 : d2)
                : new Nullable<DateTime>();
                OnPropertyChanged("GetTimeOfCompletion");
            }
        }
        public List<VisitActivity> Visits
        {
            get { return _visits; }
            set
            {
                _visits = value;
                OnPropertyChanged("Visits");
            }
        }
        new public string Status
        {
            get { return _status; }
            set
            {
                int visitCount = Visits.Count;
                int visitsDone = 0;
                foreach (VisitActivity visit in Visits)
                {
                    if (visit.IsDone)
                        visitsDone++;
                }
                _status = visitsDone + "/" + visitCount;
                OnPropertyChanged("Status");
            }
        }
        
        #endregion
        public RoundActivity(string clinicianId) : base(clinicianId)
        {
            Type = typeof(RoundActivity).Name;
            _clinicianId = clinicianId;
            _visits = new List<VisitActivity>();
        }
        public void VisitDoneChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDone") //Both of the below properties need to check if this was the last visit to be completed
            {
                OnPropertyChanged("IsFinished");
                OnPropertyChanged("GetTimeOfCompletion");
                OnPropertyChanged("Status");
            }
        }
        public void addVisit(VisitActivity v)
        {
            if (v.IsDone) throw new InvalidOperationException("Can't add a finished visit to a round");
            _visits.Add(v);
            v.PropertyChanged += new PropertyChangedEventHandler(VisitDoneChanged);
        }

        public void removeVisit(VisitActivity v)
        {
            v.PropertyChanged -= new PropertyChangedEventHandler(VisitDoneChanged);
            _visits.Remove(v);
            //Update IsFinished and GetTimeOfCompletion as this was perhaps the last unfinished visit
            OnPropertyChanged("IsFinished");
            OnPropertyChanged("GetTimeOfCompletion");
        }

        public List<string> GetPatientIds()
        {
            List<string> patientIds = new List<string>();
            foreach (VisitActivity visit in Visits)
            {
                if (visit.PatientId != null)
                    patientIds.Add(visit.PatientId);
            }

            return patientIds;
        }

        
    }
}
