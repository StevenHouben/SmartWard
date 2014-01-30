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

        private List<VisitActivity> _visits;
        
        #region properties
        public List<VisitActivity> Visits
        {
            get { return _visits; }
            protected set { _visits = value; }
        }
        public string ClinicianId
        {
            get { return _clinicianId; }
            set { _clinicianId = value; }
        }
        #endregion
        public RoundActivity(string clinicianId) : base(clinicianId)
        {
            Status = "In a relationship";
            Info = "Store guns";
            Type = typeof(RoundActivity).Name;
            _clinicianId = clinicianId;
            _visits = new List<VisitActivity>();
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

        /// <summary>
        /// Returns true if all visits are finished.
        /// </summary>
        /// <returns></returns>
        public bool IsFinished
        {
            get {
                return _visits.Count > 0
                ? _visits.Select<VisitActivity, bool>(v => v.IsDone).Aggregate((b1, b2) => b1 && b2)
                : false;
            }
        }

        public DateTime? GetTimeOfCompletion
        {
            get {
                return IsFinished
                ? _visits.Select<VisitActivity, DateTime>(v => v.TimeOfCompletion).Aggregate(DateTime.MinValue, (d1, d2) => DateTime.Compare(d1, d2) > 0 ? d1 : d2)
                : new Nullable<DateTime>();
            }
        }

        public void VisitDoneChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDone") //Both of the below properties need to check if this was the last visit to be completed
            {
                OnPropertyChanged("IsFinished");
                OnPropertyChanged("GetTimeOfCompletion");
            }
        }
    }
}
