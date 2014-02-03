using NooSphere.Infrastructure.Helpers;
using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.Models.Activities;
using SmartWard.PDA.Helpers;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmartWard.PDA.ViewModels
{
    internal class ActivitiesViewModel : ViewModelBase
    {
        public ObservableCollection<ActivityViewModel> Activities { get; set; }

        public WardNode WardNode { get; set; }

        public ActivitiesViewModel(WardNode wardNode)
        {
            WardNode = wardNode;

            Activities = new ObservableCollection<ActivityViewModel>();
            Activities.CollectionChanged += Activities_CollectionChanged;

            WardNode.ActivityAdded += WardNode_ActivityAdded;
            WardNode.ActivityRemoved += WardNode_ActivityRemoved;

            WardNode.ActivityChanged += WardNode_ActivityChanged;
            WardNode.ActivityCollection.Where(a => a.Participants.Contains(AuthenticationHelper.User.Id)).ToList().ForEach(a => Activities.Add(new ActivityViewModel((Activity)a)));
        }

        void WardNode_ActivityAdded(object sender, NooSphere.Model.Activity activity)
        {
            switch (activity.Type)
            {
                case "Round":
                    Activities.Add(new ActivityViewModel((RoundActivity)activity));
                    break;
                default:
                    throw new Exception("User type not found");
            }

        }
        void WardNode_ActivityChanged(object sender, NooSphere.Model.Activity activity)
        {
            var index = -1;

            switch (activity.Type)
            {
                case "RoundActivity":

                    //Find patient
                    var a = Activities.FirstOrDefault(t => t.Id == activity.Id);
                    if (a == null)
                        return;

                    index = Activities.IndexOf(a);

                    if (index == -1)
                        return;

                    Activities[index] = new ActivityViewModel((RoundActivity)activity);
                    Activities[index].ActivityUpdated += ActivityUpdated;
                    break;
                case "Clinician":
                    break;
                default:
                    throw new Exception("Activity type not found");
            }
        }
        void WardNode_ActivityRemoved(object sender, NooSphere.Model.Activity activity)
        {
            foreach (var a in Activities.ToList())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (a.Id == activity.Id)
                        Activities.Remove(a);
                });
            }
        }

        void Activities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (var item in list)
                {
                    var activity = item as ActivityViewModel;
                    if (activity == null) return;
                    activity.ActivityUpdated += ActivityUpdated;
                }
            }
        }

        void ActivityUpdated(object sender, EventArgs e)
        {
            WardNode.UpdateActivity((Activity)sender);
        }
    }
}
