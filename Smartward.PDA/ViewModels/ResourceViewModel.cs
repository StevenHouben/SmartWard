using SmartWard.Commands;
using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.Models.Resources;
using SmartWard.PDA.Controllers;
using SmartWard.PDA.Views;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        
        public virtual void UpdateResource(NooSphere.Model.Resources.Resource resource)
        {
            ((Resource)resource).UpdatedBy = AuthenticationController.User.Id;
            ((Resource)resource).Updated = DateTime.Now;
            if (WardNode.ResourceCollection.Where(r => r.Id.Equals(resource.Id)).ToList().FirstOrDefault() != null)
                WardNode.UpdateResource(resource);
            else
                WardNode.AddResource(resource);

            PDAWindow pdaWindow = (PDAWindow)Application.Current.MainWindow;

            bool popStack = false;
            JournalEntry j = null;
            foreach (JournalEntry journal in pdaWindow.ContentFrame.BackStack)
            {
                if (journal.Name.Equals(typeof(AddResourceView).Name))
                {
                    popStack = true;
                }
                else
                {
                    j = journal;
                }
                break;
            }

            if (popStack)
            {
                pdaWindow.ContentFrame.RemoveBackEntry();
                pdaWindow.ContentFrame.NavigationService.GoBack();
            }
            else
            {
                NavigationCommands.NavigateJournal.Execute(j, pdaWindow.ContentFrame);
            }

            
        }

    }
}
