﻿using System;
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
using ABC.Model.Users;


namespace SmartWard.Whiteboard.ViewModels
{
    internal class BoardViewModel : ViewModelBase
    {
        public ObservableCollection<PatientViewModel> Patients { get; set; }
        

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

            Patients = new ObservableCollection<PatientViewModel>();
            Patients.CollectionChanged += Patients_CollectionChanged;

            WardNode.UserAdded += WardNode_UserAdded;
            WardNode.UserRemoved += WardNode_UserRemoved;

            WardNode.UserChanged += WardNode_UserChanged;

            WardNode.UserCollection.Where(p => p.Type == typeof(Patient).Name).ToList().ForEach(p => Patients.Add(new PatientViewModel((Patient)p, WardNode) {RoomNumber = _roomNumber++}));
        }

        void WardNode_UserAdded(object sender, User user)
        {
            switch (user.Type) {
                case "Patient":
                    Patients.Add(new PatientViewModel((Patient)user, WardNode) { RoomNumber = _roomNumber++ });
                    break;
                case "Clinician":
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

                    Patients[index] = new PatientViewModel((Patient)user, WardNode);
                    Patients[index].PatientUpdated += PatientUpdated;
                    break;
                case "Clinician":
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
                    break;
                default:
                    throw new Exception("User type not found");
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

        public void ReorganizeDragAndDroppedPatients(object droppedData, object targetData)
        {
            var droppedPatientView = ((IDataObject)droppedData).GetData(typeof(PatientViewModel)) as PatientViewModel;
            var targetPatientView = targetData as PatientViewModel;

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
            WardNode.AddActivity(new RoundActivity("Doc Buron"));
        }
        private void AddNewAnonymousClinician()
        {
            WardNode.AddUser(new Clinician("Dr. Phil", Clinician.ClinicianType.Chief));
        }
        private void ToggleWebAPi()
        {
            WardNode.IsWebApiEnabled = !WardNode.IsWebApiEnabled;
        }
    }
}
