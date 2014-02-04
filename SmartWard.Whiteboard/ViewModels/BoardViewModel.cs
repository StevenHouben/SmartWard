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
using NooSphere.Model.Device;
using SmartWard.Models.Devices;
using SmartWard.Models.Notifications;

namespace SmartWard.Whiteboard.ViewModels
{
    public class BoardViewModel : ViewModelBase
    {
        public ObservableCollection<BoardRowPatientViewModel> Patients { get; set; }
        public ObservableCollection<ClinicianViewModelBase> Clinicians { get; set; }
        public ObservableCollection<RoundActivity> RoundActivities { get; set; }
        public ObservableCollection<DeviceViewModelBase> Tablets { get; set; }
        public ObservableCollection<EWSViewModelBase> EWSs { get; set; }
        public ObservableCollection<NoteViewModelBase> Notes { get; set; }
        
        public WardNode WardNode { get; set; }

        private int _roomNumber = 1;

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
            //TODO: DEVICE USED!?
            PdaDevice device = new PdaDevice()
            {
                Name = "Whiteboard",
                DeviceType = DeviceType.WallDisplay,
                DevicePortability = DevicePortability.Stationary,
                Owner = new User() { Name = "NILU" }
            };

            WardNode = WardNode.StartWardNodeAsSystem(WebConfiguration.DefaultWebConfiguration);

            WardNode.AddDevice(device);

            Patients = new ObservableCollection<BoardRowPatientViewModel>();
            Patients.CollectionChanged += Patients_CollectionChanged;

            Clinicians = new ObservableCollection<ClinicianViewModelBase>();
            Clinicians.CollectionChanged += Clinicians_CollectionChanged;

            RoundActivities = new ObservableCollection<RoundActivity>();

            EWSs = new ObservableCollection<EWSViewModelBase>();
            Notes = new ObservableCollection<NoteViewModelBase>();

            Tablets = new ObservableCollection<DeviceViewModelBase>();

            WardNode.UserAdded += WardNode_UserAdded;
            WardNode.UserRemoved += WardNode_UserRemoved;
            WardNode.UserChanged += WardNode_UserChanged;
            WardNode.ActivityAdded += WardNode_ActivityAdded;
            WardNode.ActivityRemoved += WardNode_ActivityRemoved;
            WardNode.ActivityChanged += WardNode_ActivityChanged;
            WardNode.ResourceAdded += WardNode_ResourceAdded;
            WardNode.ResourceRemoved += WardNode_ResourceRemoved;
            WardNode.ResourceChanged += WardNode_ResourceChanged;

            WardNode.UserCollection.Where(p => p.Type == typeof(Patient).Name).ToList().ForEach(p => Patients.Add(new BoardRowPatientViewModel((Patient)p, WardNode, this) {RoomNumber = _roomNumber++}));
            WardNode.UserCollection.Where(p => p.Type == typeof(Clinician).Name).ToList().ForEach(c => Clinicians.Add(new ClinicianViewModelBase((Clinician)c)));
            WardNode.ActivityCollection.Where(a => a.Type == typeof(RoundActivity).Name).ToList().ForEach(a => RoundActivities.Add(a as RoundActivity));
            WardNode.ResourceCollection.Where(r => r.Type == typeof(EWS).Name).ToList().ForEach(ews => EWSs.Add(new EWSViewModelBase((EWS)ews, WardNode)));
            WardNode.ResourceCollection.Where(r => r.Type == typeof(Note).Name).ToList().ForEach(n => Notes.Add(new NoteViewModelBase((Note)n, WardNode)));
            WardNode.DeviceCollection.Where(r => r.Type == typeof(PdaDevice).Name).ToList().ForEach(d => Tablets.Add(new DeviceViewModelBase(d)));
        }

        #region Wardnode Users
        void WardNode_UserAdded(object sender, User user)
        {
            switch (user.Type) {
                case "Patient":
                    Patients.Add(new BoardRowPatientViewModel((Patient)user, WardNode, this) { RoomNumber = _roomNumber++ });
                    break;
                case "Clinician":
                    Clinicians.Add(new ClinicianViewModelBase((Clinician)user));
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

                    Patients[index] = new BoardRowPatientViewModel((Patient)user, WardNode, this);
                    break;
                case "Clinician":
                    //Find patient
                    var clinician = Clinicians.FirstOrDefault(t => t.Id == user.Id);
                    if (clinician == null)
                        return;

                    index = Clinicians.IndexOf(clinician);

                    if (index == -1)
                        return;

                    Clinicians[index] = new ClinicianViewModelBase((Clinician)user);
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

        #region Wardnode Resources
        void WardNode_ResourceAdded(object sender, NooSphere.Model.Resources.Resource resource)
        {
            switch (resource.Type)
            {
                case "EWS":
                    EWSs.Add(new EWSViewModelBase((EWS)resource, WardNode));
                    break;
                case "Note":
                    Notes.Add(new NoteViewModelBase((Note)resource, WardNode));
                    break;
                default:
                    throw new Exception("Resource type not found");
            }

        }

        void WardNode_ResourceChanged(object sender, NooSphere.Model.Resources.Resource resource)
        {
            var index = -1;

            switch (resource.Type)
            {
                case "EWS":
                    //Find ews
                    var ews = EWSs.FirstOrDefault(e => e.Resource.Id == resource.Id);
                    if (ews == null)
                        return;

                    index = EWSs.IndexOf(ews);

                    if (index == -1)
                        return;

                    EWSs[index] = new EWSViewModelBase((EWS)resource, WardNode);
                    break;
                case "Note":
                    //Find note
                    var note = Notes.FirstOrDefault(n => n.Resource.Id == resource.Id);
                    if (note == null)
                        return;

                    index = Notes.IndexOf(note);

                    if (index == -1)
                        return;

                    Notes[index] = new NoteViewModelBase((Note)resource, WardNode);
                    break;
                default:
                    throw new Exception("Resource type not found");
            }

        }
        void WardNode_ResourceRemoved(object sender, NooSphere.Model.Resources.Resource resource)
        {
            switch (resource.Type)
            {
                case "EWS":
                    foreach (var e in EWSs.ToList())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (e.Resource.Id == resource.Id)
                                EWSs.Remove(e);
                        });
                    }
                    break;
                case "Note":
                    foreach (var n in Notes)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (n.Resource.Id == resource.Id)
                                Notes.Remove(n);
                        });
                    }
                    break;
                default:
                    throw new Exception("Resource type not found");
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
                    var patient = item as BoardRowPatientViewModel;
                    if (patient == null) return;
                }
            }
        }

        void Clinicians_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (var item in list)
                {
                    var clinician = item as ClinicianViewModelBase;
                    if (clinician == null) return;
                }
            }
        }

        void ClinicianUpdated(object sender, EventArgs e)
        {
            WardNode.UpdateUser((Clinician)sender);
        }

        public void ReorganizeDragAndDroppedPatients(object droppedData, object targetData)
        {
            var droppedPatientView = ((IDataObject)droppedData).GetData(typeof(BoardRowPatientViewModel)) as BoardRowPatientViewModel;
            var targetPatientView = targetData as BoardRowPatientViewModel;

            if (droppedPatientView == null) return;

            if (targetPatientView != null)
            {
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
            else
            {
                var targetDeviceView = targetData as DeviceViewModelBase;
                if (targetDeviceView == null) return;
                var n = new PushNotification(new List<string>{targetDeviceView.Owner.Id}, droppedPatientView.Id, typeof(Patient).Name, "");
                WardNode.AddNotification(n);
            }
        }

        private void ToggleLocation()
        {
            WardNode.IsLocationEnabled = !WardNode.IsLocationEnabled;
        }

        private void ToggleBroadcasting()
        {
            WardNode.IsBroadcastEnabled = !WardNode.IsBroadcastEnabled;
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
        private void ToggleWebAPi()
        {
            WardNode.IsWebApiEnabled = !WardNode.IsWebApiEnabled;
        }
    }
}
