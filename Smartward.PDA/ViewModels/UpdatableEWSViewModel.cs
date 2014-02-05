using SmartWard.Commands;
using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.Models.Activities;
using SmartWard.Models.Notifications;
using SmartWard.PDA.Helpers;
using SmartWard.PDA.Views;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SmartWard.PDA.ViewModels
{
    public class UpdatableEWSViewModel : EWSViewModelBase
    {
        public string Status { get { return Value.ToString(); } }
        public UpdatableEWSViewModel(EWS e, WardNode wardNode) : base(e, wardNode) 
        {
            e.PropertyChanged += StatusChanged;
        }

        private void StatusChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "value")
            {
                OnPropertyChanged("Status");
            }
        }

        public event EventHandler ResourceUpdated;
        private ICommand _updateResourceCommand;
        public ICommand UpdateResourceCommand
        {
            get
            {
                return _updateResourceCommand ?? (_updateResourceCommand = new RelayCommand(
                    param => UpdateResource(),
                    param => true
                    ));
            }
        }
        
        public void UpdateResource()
        {
            int EWSValue = EWS.GetEWS();
            // If EWS score is critical, make a notification for all 
            if (EWSValue > 0)
            {
                List<string> clinicianIds = new List<string>();
                WardNode.ActivityCollection.
                    Where(a => a.Type.Equals(typeof(RoundActivity).Name) && (a as RoundActivity).Visits.Any(v => v.PatientId.Equals(EWS.PatientId))).ToList().
                    ForEach(a => clinicianIds.AddRange((a as RoundActivity).Participants));
                Patient p = (Patient) WardNode.UserCollection.Where(u => u.Type.Equals(typeof(Patient).Name) && u.Id.Equals(EWS.PatientId)).ToList().FirstOrDefault();
                Notification n = new Notification(clinicianIds, EWS.Id, "EWS", p.Name + ", EWS: " + EWSValue);

                WardNode.AddNotification(n);
            }

            EWS.UpdatedBy = AuthenticationHelper.User.Id;
            EWS.Updated = DateTime.Now;
            EWS.SeenBy = new List<string>() { AuthenticationHelper.User.Id }; // Resetting seenby list, after updating resource.
            if (WardNode.ResourceCollection.Where(r => r.Id.Equals(EWS.Id)).ToList().FirstOrDefault() != null)
                WardNode.UpdateResource(EWS);
            else
                WardNode.AddResource(EWS);

            PDAWindow pdaWindow = (PDAWindow)Application.Current.MainWindow;
            NavigationHelper.NavigateBack(pdaWindow);
        }
    }
}
