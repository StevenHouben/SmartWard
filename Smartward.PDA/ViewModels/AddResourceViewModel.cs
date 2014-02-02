using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.PDA.ViewModels
{
    public class AddResourceViewModel : ViewModelBase
    {
        private readonly Patient _patient;

        private List<string> _resources = new List<string>(){"EWS", "Note"};

        #region Properties
        public List<string> Resources 
        { 
            get { return _resources; } 
        }

        public Patient Patient 
        { 
            get { return _patient; } 
        }
        public WardNode WardNode { get; set; }
        #endregion
        public AddResourceViewModel(Patient patient, WardNode wardNode)
        {
            _patient = patient;
            WardNode = wardNode;
        }

        public void AddResource(NooSphere.Model.Resources.Resource resource) 
        {
            WardNode.AddResource(resource);
        }

        public bool HasEWS()
        {
            EWS ews = (EWS) WardNode.ResourceCollection.Where(r => r.Type.Equals(typeof(EWS).Name) && ((EWS)r).PatientId.Equals(Patient.Id)).ToList().FirstOrDefault();

            return ews != null ? true : false;
        }

        public bool HasNote()
        {
            Note ews = (Note)WardNode.ResourceCollection.Where(r => r.Type.Equals(typeof(Note).Name) && ((Note)r).PatientId.Equals(Patient.Id)).ToList().FirstOrDefault();

            return ews != null ? true : false;
        }
    }
}
