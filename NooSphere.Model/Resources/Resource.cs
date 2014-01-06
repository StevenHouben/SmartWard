using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABC.Model.Primitives;

namespace ABC.Model.Resources
{
    public class Resource : Noo, IResource
    {

        private string _patientId;
        private DateTime _timestamp;

        public string PatientId { get; set; }
        public DateTime Timestamp { get; set; }

        public Resource(string patient_id)
		{
            PatientId = patient_id;
            Timestamp = DateTime.Now;
            Type = typeof( IResource ).Name;
		}

    }
}
