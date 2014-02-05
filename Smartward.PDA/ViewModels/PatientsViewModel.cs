using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NooSphere.Infrastructure.Helpers;

namespace SmartWard.PDA.ViewModels
{
    public class PatientsViewModel : ViewModelBase
    {
        public ObservableCollection<PatientsLayoutViewModel> Patients { get; set; }
        
        public WardNode WardNode { get; set; }

        public PatientsViewModel(List<string> patientIds, WardNode wardNode)
        {
            WardNode = wardNode;

            Patients = new ObservableCollection<PatientsLayoutViewModel>();
            Patients.CollectionChanged += Patients_CollectionChanged;

            WardNode.UserAdded += WardNode_UserAdded;
            WardNode.UserRemoved += WardNode_UserRemoved;

            WardNode.UserChanged += WardNode_UserChanged;

            if (patientIds.Count > 0)
            {
                WardNode.UserCollection.Where(p => patientIds.Contains(p.Id)).ToList().ForEach(a => Patients.Add(new PatientsLayoutViewModel((Patient)a, WardNode)));
            }
            else
            {
                WardNode.UserCollection.Where(p => p.Type.Equals("Patient")).ToList().ForEach(a => Patients.Add(new PatientsLayoutViewModel((Patient)a, WardNode)));
            }
        }

        void WardNode_UserAdded(object sender, NooSphere.Model.Users.User patient)
        {
            Patients.Add(new PatientsLayoutViewModel((Patient)patient, WardNode));
        }
        void WardNode_UserChanged(object sender, NooSphere.Model.Users.User patient)
        {
            var index = -1;
            //Find patient
            var a = Patients.FirstOrDefault(t => t.Id == patient.Id);
            if (a == null)
                return;

            index = Patients.IndexOf(a);

            if (index == -1)
                return;

            Patients[index] = new PatientsLayoutViewModel((Patient)patient, WardNode);
        }
        void WardNode_UserRemoved(object sender, NooSphere.Model.Users.User patient)
        {
            foreach (var a in Patients.ToList())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (a.Id == patient.Id)
                        Patients.Remove(a);
                });
            }
        }

        void Patients_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (var item in list)
                {
                    var patient = item as PatientsLayoutViewModel;
                    if (patient == null) return;
                }
            }
        }

        void UserUpdated(object sender, EventArgs e)
        {
            WardNode.UpdateUser((Patient)sender);
        }
    }
}
