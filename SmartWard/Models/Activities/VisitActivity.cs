using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABC.Model;

namespace SmartWard.Models
{
    public class VisitActivity : Activity
    {
        private string _roundId;
        private string _patientId;
        private bool _isDone;
        private DateTime _timeCompleted;

        public VisitActivity(string patientId, string roundId)
        {
            _patientId = patientId;
            _roundId = roundId;
        }

        public void Done()
        {
            _isDone = true;
            _timeCompleted = DateTime.Now;
        }

    }
}
