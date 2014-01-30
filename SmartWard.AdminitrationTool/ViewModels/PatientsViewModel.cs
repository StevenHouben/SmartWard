using SmartWard.Commands;
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
using System.Windows.Input;

namespace SmartWard.AdministrationTool.ViewModels
{
    internal class PatientsViewModel : ViewModelBase
    {
        public WardNode WardNode { get; set; }
        public PatientsViewModel(WardNode systemNode)
        {
            WardNode = systemNode;

            Patients = new ObservableCollection<PatientViewModel>();
            Patients.CollectionChanged += Patients_CollectionChanged;

            WardNode.UserAdded += WardNode_PatientAdded;
            WardNode.UserRemoved += WardNode_PatientRemoved;

            WardNode.UserChanged += WardNode_PatientChanged;
            WardNode.UserCollection.Where(u => u.Type.Equals(typeof(Patient).Name)).ToList().ForEach(a => Patients.Add(new PatientViewModel((Patient)a)));
        }
        #region PatientsCollection
        public ObservableCollection<PatientViewModel> Patients { get; set; }
        void WardNode_PatientAdded(object sender, NooSphere.Model.Users.User user)
        {
            if(user.Type.Equals("Patient"))
                Patients.Add(new PatientViewModel((Patient)user));
        }
        void WardNode_PatientChanged(object sender, NooSphere.Model.Users.User user)
        {
            var index = -1;
            if (user.Type.Equals("Patient")) {
                //Find patient
                var a = Patients.FirstOrDefault(t => t.Id == user.Id);
                if (a == null)
                    return;

                index = Patients.IndexOf(a);

                if (index == -1)
                    return;

                Patients[index] = new PatientViewModel((Patient)user);
                Patients[index].PatientUpdated += PatientUpdated;
            }
        }
        void WardNode_PatientRemoved(object sender, NooSphere.Model.Users.User user)
        {
            foreach (var a in Patients.ToList())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (a.Id == user.Id)
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
                    var patient = item as PatientViewModel;
                    if (patient == null) return;
                    patient.PatientUpdated += PatientUpdated;
                }
            }
        }
        void PatientUpdated(object sender, EventArgs e)
        {
            WardNode.UpdateUser((Patient)sender);
        }
        #endregion

        private ICommand _addPatientCommand;

        public ICommand AddPatientCommand
        {
            get
            {
                return _addPatientCommand ?? (_addPatientCommand = new RelayCommand(
                    param => AddNewAnonymousPatient(),
                    param => true
                    ));
            }
        }

        private void AddNewAnonymousPatient()
        {
            WardNode.AddUser(new Patient());
        }
    }
}
