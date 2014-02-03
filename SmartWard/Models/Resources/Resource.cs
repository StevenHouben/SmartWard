using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Models.Resources
{
    public abstract class Resource : NooSphere.Model.Resources.Resource
    {
        private DateTime _created;
        private DateTime _updated;
        private string _updatedBy;
        private string _status;
        public DateTime Created { get { return _created; } set { _created = value; OnPropertyChanged("Created"); } }
        public DateTime Updated { get { return _updated; } set { _updated = value; OnPropertyChanged("Updated"); } }
        public string UpdatedBy { get { return _updatedBy; } set { _updatedBy = value; OnPropertyChanged("UpdatedBy"); } }
        public string Status { get { return _status; } set { _status = value; OnPropertyChanged("Status"); } }

        public Resource()
        {
            Created = DateTime.Now;
            Updated = Created;
            UpdatedBy = "Dr. Buron";
            Status = "Nothing";
        }
    }
}
