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

namespace SmartWard.PDA.ViewModels
{
    public class PatientsViewModel : ViewModelBase
    {
        public ObservableCollection<PatientsLayoutViewModel> Patients { get; set; }
        
        public WardNode WardNode { get; set; }

        public PatientsViewModel(List<string> patientIds)
        {
            WardNode = WardNode.StartWardNodeAsSystem(WebConfiguration.DefaultWebConfiguration);

            Patients = new ObservableCollection<PatientsLayoutViewModel>();
            Patients.CollectionChanged += Patients_CollectionChanged;

            WardNode.UserAdded += WardNode_UserAdded;
            WardNode.UserRemoved += WardNode_UserRemoved;

            WardNode.UserChanged += WardNode_UserChanged;
            List<ABC.Model.Users.User> asda = WardNode.UserCollection.Where(p => patientIds.Contains(p.Id)).ToList();
            WardNode.UserCollection.Where(p => patientIds.Contains(p.Id)).ToList().ForEach(a => Patients.Add(new PatientsLayoutViewModel((Patient)a, WardNode)));
        }

        void WardNode_UserAdded(object sender, ABC.Model.Users.User patient)
        {
            Patients.Add(new PatientsLayoutViewModel((Patient)patient, WardNode));
        }
        void WardNode_UserChanged(object sender, ABC.Model.Users.User patient)
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
            Patients[index].UserUpdated += UserUpdated;
              
        }
        void WardNode_UserRemoved(object sender, ABC.Model.Users.User patient)
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
                    patient.UserUpdated += UserUpdated;
                }
            }
        }

        void UserUpdated(object sender, EventArgs e)
        {
            WardNode.UpdateUser((Patient)sender);
        }
    }
}
