using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using SmartWard.Commands;
using SmartWard.HyPR.ViewModels;
using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.ViewModels;


namespace SmartWard.Whiteboard.ViewModels
{
    internal class BoardViewModel:ViewModelBase
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

            WardNode.PatientAdded += WardNode_PatientAdded;
            WardNode.PatientRemoved += WardNode_PatientRemoved;

            WardNode.PatientChanged += WardNode_PatientChanged;
            WardNode.Patients.ToList().ForEach(p => Patients.Add(new PatientViewModel(p) {RoomNumber = _roomNumber++}));
        }


        void WardNode_PatientAdded(object sender, Patient e)
        {
            Patients.Add(new PatientViewModel(e) { RoomNumber = _roomNumber++ });
        }

        void WardNode_PatientChanged(object sender, Patient e)
        {
            var index = -1;

            //Find patient
            var patient = Patients.FirstOrDefault(t => t.Id == e.Id);

            if (patient == null)
                return;

            index = Patients.IndexOf(patient);

            if (index == -1)
                return;

            Patients[index] = new PatientViewModel(e);
            Patients[index].PatientUpdated += PatientUpdated;
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

        void WardNode_PatientRemoved(object sender, Patient e)
        {
            foreach (var p in Patients.ToList())
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (p.Id == e.Id)
                            Patients.Remove(p);
                    });
                }
        }


        void PatientUpdated(object sender, EventArgs e)
        {
            WardNode.UpdatePatient((Patient)sender);
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

            _roomNumber = 1;
            Patients.ToList().ForEach(p => p.RoomNumber = _roomNumber++);
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
            WardNode.AddPatient(new Patient());
        }
        private void ToggleWebAPi()
        {
            WardNode.IsWebApiEnabled = !WardNode.IsWebApiEnabled;
        }
    }
}
