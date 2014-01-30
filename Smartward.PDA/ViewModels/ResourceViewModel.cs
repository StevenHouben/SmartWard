using SmartWard.Commands;
using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.Models.Resources;
using SmartWard.PDA.Controllers;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SmartWard.PDA.ViewModels
{
    public class ResourceViewModel : ViewModelBase
    {
        private Resource _resource;

        public event EventHandler ResourceUpdated;
        
        private ICommand _updateResourceCommand;

        public ICommand UpdateResourceCommand
        {
            get
            {
                return _updateResourceCommand ?? (_updateResourceCommand = new RelayCommand(
                    param => UpdateResource(Resource),
                    param => true
                    ));
            }
        }

        #region Properties
        public WardNode WardNode { get; set; }
        public Patient Patient { get; set; }
        public Resource Resource
        {
            get { return _resource; }
            set
            {
                _resource = value;
            }
        }
        public string Type
        {
            get { return _resource.Type; }
        }
        public string Id
        {
            get { return _resource.Id; }
        }
        public string PatientNameAndCpr
        {
            get { return Patient.Name + ": " + Patient.Cpr; }
        }
        #endregion

        public ResourceViewModel(Resource resource, WardNode wardNode)
        {
            Resource = resource;
            WardNode = wardNode;

            Patient = (Patient) WardNode.UserCollection.Where(p => p.Id.Equals(_resource.PatientId)).ToList().FirstOrDefault();
        }
        
        public void UpdateResource(ABC.Model.Resources.Resource resource)
        {
            ((Resource)resource).UpdatedBy = AuthenticationController.User.Id;
            WardNode.UpdateResource(resource);
            
        }

    }
}
