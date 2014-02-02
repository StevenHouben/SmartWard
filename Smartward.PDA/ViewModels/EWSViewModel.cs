using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.Models.Activities;
using SmartWard.Models.Notifications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.PDA.ViewModels
{
    public class EWSViewModel : ResourceViewModel
    {
        #region Properties
        public EWS EWS
        {
            get { return (EWS)Resource; }
        }
        public string Status
        {
            get { return EWS.GetEWS().ToString(); }
        }
        #endregion
        public EWSViewModel(EWS e, WardNode wardNode) : base(e, wardNode)
        {
            e.PropertyChanged += new PropertyChangedEventHandler(EWSChanged);
            
        }
        public void EWSChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Status");
        }

        override public void UpdateResource(NooSphere.Model.Resources.Resource resource)
        {
            if (WardNode.ResourceCollection.Where(r => r.Id.Equals(resource.Id)).ToList().FirstOrDefault() != null)
            {
                int EWSValue = (resource as EWS).GetEWS();
                // If EWS score is critical, make a notification for all 
                if (EWSValue > 0)
                {
                    List<string> clinicianIds = new List<string>();
                    WardNode.ActivityCollection.
                        Where(a => a.Type.Equals(typeof(RoundActivity).Name) && (a as RoundActivity).Visits.Any(v => v.PatientId.Equals((resource as EWS).PatientId))).ToList().
                        ForEach(a => clinicianIds.AddRange((a as RoundActivity).Participants));
                    Patient p = (Patient) WardNode.UserCollection.Where(u => u.Type.Equals(typeof(Patient).Name) && u.Id.Equals((resource as EWS).PatientId)).ToList().FirstOrDefault();
                    Notification n = new Notification(clinicianIds, resource.Id, "EWS", p.Name + ", EWS: " + EWSValue);

                    WardNode.AddNotification(n);
                }
            }

            base.UpdateResource(resource);
        }
    }
}
