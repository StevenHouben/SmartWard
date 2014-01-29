using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Models.Resources
{
    public abstract class Resource : NooSphere.Model.Resources.Resource
    {
        private string _clinicianId;
        private DateTime _created;

        public string PatientId { get; set; }
        public DateTime Created { get; set; }

        public Resource(string patientId)
        {
            PatientId = patientId;
            Created = DateTime.Now;
        }
    }
}
