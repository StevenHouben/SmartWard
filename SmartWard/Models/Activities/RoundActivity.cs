using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABC.Model.Primitives;
using SmartWard.Models.Activities;


namespace SmartWard.Models 
{
    public class RoundActivity : Activity
    {
        private string _clinicianId;
        private List<VisitActivity> _visitActivities;
        
        #region properties
        public List<VisitActivity> VisitActivities
        {
            get { return _visitActivities; }
            set { _visitActivities = value; }
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
            _visitActivities = new List<VisitActivity>();
        }

        public void addVisit(VisitActivity id)
        {
            _visitActivities.Add(id);
        }

        /// <summary>
        /// Returns true if all visits are finished.
        /// </summary>
        /// <returns></returns>
        public bool IsFinished()
        {
            // TODO
            return true;
        }

        public DateTime GetTimeFinised()
        {
            return DateTime.Now;
        }

    }
}
