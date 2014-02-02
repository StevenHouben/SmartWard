using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using SmartWard.Commands;
using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.ViewModels;
using System.Threading.Tasks;
using NooSphere.Model.Users;
using NooSphere.Infrastructure.Helpers;
using SmartWard.Models.Resources;
using System.Collections.Generic;
using SmartWard.Models.Activities;

namespace SmartWard.Whiteboard.ViewModels
{
    public class BoardViewModel : ViewModelBase
    {
        public ObservableCollection<BoardRowViewModel> Patients { get; set; }
        public ObservableCollection<ClinicianViewModel> Clinicians { get; set; }
        public ObservableCollection<RoundActivity> RoundActivities { get; set; }
        

        public WardNode WardNode { get; set; }

        private int _roomNumber = 1;

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

        private ICommand _addActivityCommand;

        public ICommand AddActivityCommand
        {
            get
            {
                return _addActivityCommand ?? (_addActivityCommand = new RelayCommand(
                    param => AddNewAnonymousActivity(),
                    param => true
                    ));
            }
        }

        private ICommand _addClinicianCommand;

        public ICommand AddClinicianCommand
        {
            get
            {
                return _addClinicianCommand ?? (_addClinicianCommand = new RelayCommand(
                    param => AddNewAnonymousClinician(),
                    param => true
                    ));
            }
        }

        private ICommand _toggleWebApiCommand;

        public ICommand ToggleWebApiCommand
        {
            get
            {
                return _toggleWebApiCommand ?? (_toggleWebApiCommand = new RelayCommand(
                    param => ToggleWebAPi(),
                    param => true
                    ));
            }
        }

        private ICommand _toggleLocationTrackerCommand;

        public ICommand ToggleLocationTrackerCommand
        {
            get
            {
                return _toggleLocationTrackerCommand ?? (_toggleLocationTrackerCommand = new RelayCommand(
                    param => ToggleLocation(),
                    param => true
                    ));
            }
        }

        private ICommand _toggleBroadcasterCommand;

        public ICommand ToggleBroadcasterCommand
        {
            get
            {
                return _toggleBroadcasterCommand ?? (_toggleBroadcasterCommand = new RelayCommand(
                    param => ToggleBroadcasting(),
                    param => true
                    ));
            }
        }

        public BoardViewModel()
        {
            WardNode = WardNode.StartWardNodeAsSystem(WebConfiguration.DefaultWebConfiguration);

            Patients = new ObservableCollection<BoardRowViewModel>();
            Patients.CollectionChanged += Patients_CollectionChanged;

            Clinicians = new ObservableCollection<ClinicianViewModel>();
            Clinicians.CollectionChanged += Clinicians_CollectionChanged;

            RoundActivities = new ObservableCollection<RoundActivity>();

            WardNode.UserAdded += WardNode_UserAdded;
            WardNode.UserRemoved += WardNode_UserRemoved;
            WardNode.UserChanged += WardNode_UserChanged;
            WardNode.ActivityAdded += WardNode_ActivityAdded;
            WardNode.ActivityRemoved += WardNode_ActivityRemoved;
            WardNode.ActivityChanged += WardNode_ActivityChanged;
            

            WardNode.UserCollection.Where(p => p.Type == typeof(Patient).Name).ToList().ForEach(p => Patients.Add(new BoardRowViewModel((Patient)p, WardNode, this) {RoomNumber = _roomNumber++}));
            WardNode.UserCollection.Where(p => p.Type == typeof(Clinician).Name).ToList().ForEach(c => Clinicians.Add(new ClinicianViewModel((Clinician)c)));
            WardNode.ActivityCollection.Where(a => a.Type == typeof(RoundActivity).Name).ToList().ForEach(a => RoundActivities.Add(a as RoundActivity));
        }

        #region Wardnode Users
        void WardNode_UserAdded(object sender, User user)
        {
            switch (user.Type) {
                case "Patient":
                    Patients.Add(new BoardRowViewModel((Patient)user, WardNode, this) { RoomNumber = _roomNumber++ });
                    break;
                case "Clinician":
                    Clinicians.Add(new ClinicianViewModel((Clinician)user));
                    break;
                default:
                    throw new Exception("User type not found");
            }
            
        }

        void WardNode_UserChanged(object sender, User user)
        {
            var index = -1;

            switch (user.Type)
            {
                case "Patient":
                    //Find patient
                    var patient = Patients.FirstOrDefault(t => t.Id == user.Id);
                    if (patient == null)
                        return;

                    index = Patients.IndexOf(patient);

                    if (index == -1)
                        return;

                    Patients[index] = new BoardRowViewModel((Patient)user, WardNode, this);
                    Patients[index].PatientUpdated += PatientUpdated;
                    break;
                case "Clinician":
                    //Find patient
                    var clinician = Clinicians.FirstOrDefault(t => t.Id == user.Id);
                    if (clinician == null)
                        return;

                    index = Clinicians.IndexOf(clinician);

                    if (index == -1)
                        return;

                    Clinicians[index] = new ClinicianViewModel((Clinician)user);
                    Clinicians[index].ClinicianUpdated += ClinicianUpdated;
                    break;
                default:
                    throw new Exception("User type not found");
            }
            
        }
        void WardNode_UserRemoved(object sender, User user)
        {
            switch (user.Type)
            {
                case "Patient":
                    foreach (var p in Patients.ToList())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (p.Id == user.Id)
                                Patients.Remove(p);
                        });
                    }
                    break;
                case "Clinician":
                    foreach (var c in Clinicians.ToList())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (c.Id == user.Id)
                                Clinicians.Remove(c);
                        });
                    }
                    break;
                default:
                    throw new Exception("User type not found");
            }
        }
        #endregion

        #region WardNode Activities
        void WardNode_ActivityAdded(object sender, NooSphere.Model.Activity activity)
        {
            switch (activity.Type)
            {
                case "RoundActivity":
                    RoundActivities.Add(activity as RoundActivity);
                    break;
            }

        }

        void WardNode_ActivityChanged(object sender, NooSphere.Model.Activity activity)
        {
            var index = -1;

            switch (activity.Type)
            {
                case "RoundActivity":
                    //Find patient
                    var a = RoundActivities.FirstOrDefault(t => t.Id == activity.Id);
                    if (a == null)
                        return;

                    index = RoundActivities.IndexOf(a);

                    if (index == -1)
                        return;

                    RoundActivities[index] = activity as RoundActivity;
                    //RoundActivities[index].ActivityUpdated += ActivityUpdated;
                    break;
            }

        }
        void WardNode_ActivityRemoved(object sender, NooSphere.Model.Activity activity)
        {
            switch (activity.Type)
            {
                case "RoundActivity":
                    foreach (var a in RoundActivities.ToList())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (a.Id == activity.Id)
                                RoundActivities.Remove(a);
                        });
                    }
                    break;
            }
        }
        #endregion

        void Patients_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (var item in list)
                {
                    var patient = item as BoardRowViewModel;
                    if (patient == null) return;
                    patient.PatientUpdated += PatientUpdated;
                }
            }
        }

        void PatientUpdated(object sender, EventArgs e)
        {
            WardNode.UpdateUser((Patient)sender);
        }

        void Clinicians_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (var item in list)
                {
                    var clinician = item as ClinicianViewModel;
                    if (clinician == null) return;
                    clinician.ClinicianUpdated += ClinicianUpdated;
                }
            }
        }

        void ClinicianUpdated(object sender, EventArgs e)
        {
            WardNode.UpdateUser((Clinician)sender);
        }

        public void ReorganizeDragAndDroppedPatients(object droppedData, object targetData)
        {
            var droppedPatientView = ((IDataObject)droppedData).GetData(typeof(BoardRowViewModel)) as BoardRowViewModel;
            var targetPatientView = targetData as BoardRowViewModel;

            if (droppedPatientView == null) return;
            if (targetPatientView == null) return;

            var removedIdx = Patients.IndexOf(droppedPatientView);
            var targetIdx = Patients.IndexOf(targetPatientView);

            if (removedIdx < targetIdx)
            {
                Patients.Insert(targetIdx + 1, droppedPatientView);
                Patients.RemoveAt(removedIdx);
            }
            else
            {
                var remIdx = removedIdx + 1;
                if (Patients.Count + 1 <= remIdx) return;
                Patients.Insert(targetIdx, droppedPatientView);
                Patients.RemoveAt(remIdx);
            }

            Task.Factory.StartNew(() =>
                {
                    _roomNumber = 1;
                    Patients.ToList().ForEach(
                        p =>
                        {
                            p.RoomNumber = _roomNumber++;
                            WardNode.UpdateUser(p.Patient);
                        });
                });
        }

        private void ToggleLocation()
        {
            WardNode.IsLocationEnabled = !WardNode.IsLocationEnabled;
        }

        private void ToggleBroadcasting()
        {
            WardNode.IsBroadcastEnabled = !WardNode.IsBroadcastEnabled;
        }

        private void AddNewAnonymousPatient()
        {
            WardNode.AddUser(new Patient());
        }
        private void AddNewAnonymousActivity()
        {
            VisitActivity v1 = new VisitActivity("patient1");
            VisitActivity v2 = new VisitActivity("patient2");
            VisitActivity v3 = new VisitActivity("patient3");

            //List<string> patients = new List<string>();
            //foreach (PatientViewModel pvm in Patients) 
            //{
            //    patients.Add(pvm.Patient.Id);
            //}
            RoundActivity r = new RoundActivity("Doc Buron");
            r.addVisit(v1);
            r.addVisit(v2);
            r.addVisit(v3);
            WardNode.AddActivity(r);
        }
        private void AddNewAnonymousClinician()
        {
            WardNode.AddUser(new Clinician(Clinician.ClinicianTypeEnum.Doctor, "nfcId"));
        }
        private void ToggleWebAPi()
        {
            WardNode.IsWebApiEnabled = !WardNode.IsWebApiEnabled;
        }
    }
}
