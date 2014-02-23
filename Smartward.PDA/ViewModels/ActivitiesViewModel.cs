﻿using NooSphere.Infrastructure.Helpers;
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
        public ObservableCollection<ClinicianViewModelBase> Staff { get; set; }

        public WardNode WardNode { get; set; }

        public ActivitiesViewModel(WardNode wardNode)
        {
            WardNode = wardNode;

            Activities = new ObservableCollection<ActivityViewModel>();
            Activities.CollectionChanged += Activities_CollectionChanged;

            WardNode.ActivityAdded += WardNode_ActivityAdded;
            WardNode.ActivityRemoved += WardNode_ActivityRemoved;

            WardNode.ActivityChanged += WardNode_ActivityChanged;
            WardNode.ActivityCollection.Where(a => a.Participants.Contains(AuthenticationHelper.User.Id)).ToList().ForEach(a => Activities.Add(new ActivityViewModel((Activity)a, WardNode)));

            Staff = new ObservableCollection<ClinicianViewModelBase>();
            Staff.CollectionChanged += Staff_CollectionChanged;

            WardNode.UserAdded += WardNode_UserAdded;
            WardNode.UserRemoved += WardNode_UserRemoved;

            WardNode.UserChanged += WardNode_UserChanged;
            WardNode.UserCollection.Where(u => u.Type.Equals(typeof(Clinician).Name)).ToList().ForEach(c => Staff.Add(new ClinicianViewModelBase((Clinician) c)));
        }

        void WardNode_ActivityAdded(object sender, NooSphere.Model.Activity activity)
        {
            switch (activity.Type)
            {
                case "RoundActivity":
                    Activities.Add(new ActivityViewModel((RoundActivity)activity, WardNode));
                    break;
                default:
                    throw new Exception("Activity type not found");
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

                    Activities[index] = new ActivityViewModel((RoundActivity)activity, WardNode);
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

        void WardNode_UserAdded(object sender, NooSphere.Model.Users.User user)
        {
            Staff.Add(new ClinicianViewModelBase((Clinician) user));
        }
        void WardNode_UserChanged(object sender, NooSphere.Model.Users.User user)
        {
            var index = -1;
           
            //Find patient
            var u = Staff.FirstOrDefault(t => t.Id == user.Id);
            if (u == null)
                return;

            index = Staff.IndexOf(u);

            if (index == -1)
                return;

            Staff[index] = new ClinicianViewModelBase((Clinician)user);
        }
        void WardNode_UserRemoved(object sender, NooSphere.Model.Users.User user)
        {
            foreach (var u in Staff.ToList())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (u.Id == user.Id)
                        Staff.Remove(u);
                });
            }
        }

        void Staff_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (var item in list)
                {
                    var user = item as ClinicianViewModelBase;
                    if (user == null) return;
                }
            }
        }

    }
}
