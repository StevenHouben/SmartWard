using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ABC.Infrastructure.Drivers;
using ABC.Model.Primitives;
using ABC.Model.Users;
using SmartWard.Commands;
using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.ViewModels;

namespace SmartWard.HyPR.ViewModels
{
    internal class MobileApplicationViewModel:ViewModelBase
    {

        public ObservableCollection<PatientViewModel> Patients { get; set; }

        private WardNode _wardNode;
        private HyPrDevice _hyPrDevice;
        private int _roomNumber = 1;

        #region Properties
        private PatientViewModel _selectedUser;

        public PatientViewModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
        }

        private int _defaultFontSize;

        public int DefaultFontSize
        {
            get { return _defaultFontSize; }
            set
            {
                _defaultFontSize = value;
                OnPropertyChanged("DefaultFontSize");
            }
        }

        private string _messageFlag;

        public string MessageFlag
        {
            get { return _messageFlag; }
            set
            {
                _messageFlag = value;
                OnPropertyChanged("MessageFlag");
            }
        }

        private string _messageBody;

        public string MessageBody
        {
            get { return _messageBody; }
            set
            {
                _messageBody = value;
                OnPropertyChanged("MessageBody");
            }
        }
        #endregion

        #region commands
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


        private ICommand _saveCommand;

        public ICommand SavePatientCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new RelayCommand(
                    param => SavePatient(),
                    param => CanSaveUpdate()
                    ));
            }
        }

        private ICommand _addNurseRecordcommand;

        public ICommand AddNurseRecordCommand
        {
            get
            {
                return _addNurseRecordcommand ?? (_addNurseRecordcommand = new RelayCommand(
                    param => AddNurseRecordToSelectedPatient(),
                    param => CanAddRecord()
                    ));
            }
        }

        private void AddNurseRecordToSelectedPatient()
        {
            SelectedUser.NurseRecords.Add(new NurseRecord() { Body = MessageBody, MessageFlag = (MessageFlags)Enum.Parse(typeof(MessageFlags), MessageFlag) });
            SavePatient();
            MessageBody = "";
        }

        private bool CanAddRecord()
        {
            return SelectedUser != null;
        }
        #endregion

        public MobileApplicationViewModel()
        {
            DefaultFontSize = 35;

            SelectedUser = new PatientViewModel(new Patient());
            Patients = new ObservableCollection<PatientViewModel>();

            Patients.CollectionChanged += Patients_CollectionChanged;

            InitializeDevice();

            bool found = false;
            WardNode.WardNodeFound+=(sender,config)=>
            {
                if(found)return;
                found = true;
                _wardNode = _wardNode = WardNode.StartWardNodeAsClient(config);

                _wardNode.PatientAdded += WardNode_PatientAdded;
                _wardNode.PatientRemoved += WardNode_PatientRemoved;
                _wardNode.PatientChanged += WardNode_PatientChanged;


                Application.Current.Dispatcher.Invoke(() => _wardNode.Patients.ToList().ForEach(p => Patients.Add(new PatientViewModel(p) { RoomNumber = _roomNumber++ })));

            };
            WardNode.FindWardNodes();

            MessageFlag = MessageFlags.Comment.ToString();
        }
        private void InitializeDevice()
        {
            _hyPrDevice = new HyPrDevice();
            _hyPrDevice.RfidDataReceived += (sender, e) =>
            {
                var user = (Patient)FindUserByCid(e.Rfid);
                if (user != null)
                {
                    SelectedUser = new PatientViewModel(user);
                    SendColorToHyPrDevice(user.Color);
                }
                else
                {
                    SelectedUser = new PatientViewModel(new Patient()) { Cid = e.Rfid };
                    SendColorToHyPrDevice(new Rgb(0, 0, 0)); SelectedUser.Cid = e.Rfid;
                }
            };
        }

        private void SendColorToHyPrDevice(Rgb color)
        {
            _hyPrDevice.UpdateColor(color);
        }

        private IUser FindUserByCid(string rfid)
        {
            return _wardNode.Patients.FirstOrDefault(usr => usr.Cid == rfid);
        }

        public void UpdatePatientColor(Rgb rgb)
        {
            SelectedUser.Color = rgb;
            SendColorToHyPrDevice(rgb);
        }
        private void SavePatient()
        {
            var user = (Patient)FindUserByCid(_hyPrDevice.CurrentRfid);
            if (user != null)
            {
                user.UpdateAllProperties(SelectedUser.Patient);
                SelectedUser = new PatientViewModel(user);
                _wardNode.UpdatePatient(user);
            }
            else
            {
                user = new Patient
                    {
                        Name = SelectedUser.Name,
                        Color = SelectedUser.Color,
                        Cid = SelectedUser.Cid,
                        Tag = SelectedUser.Tag
                    };

                _wardNode.AddPatient(user);

                SelectedUser.UpdateAllProperties(user);
            }
        }
        private bool CanSaveUpdate()
        {
            return true;
        }

        void WardNode_PatientAdded(object sender, Patient e)
        {
            Patients.Add(new PatientViewModel(e) { RoomNumber = _roomNumber++ });
        }

        void WardNode_PatientChanged(object sender, Patient e)
        {
            var index =-1;

            //Find patient
            var patient = Patients.FirstOrDefault(t => t.Id == e.Id);

            if (patient == null)
                return;

            index = Patients.IndexOf(patient);

            if (index == -1)
                return;

            Patients[index] = new PatientViewModel(e);
            Patients[index].PatientUpdated += PatientUpdated;

            SendColorToHyPrDevice(e.Color);

        }

        void Patients_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;
            var list = e.NewItems;
            foreach (var patient in from object item in list select item as PatientViewModel)
            {
                if (patient == null) return;
                patient.PatientUpdated += PatientUpdated;
                patient.PatientSelected += PatientSelected;
            }
        }

        void PatientSelected(object sender, EventArgs e)
        {
            SelectedUser = (PatientViewModel)sender;
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
            _wardNode.UpdatePatient((Patient)sender);
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
            _wardNode.IsLocationEnabled = !_wardNode.IsLocationEnabled;
        }

        private void ToggleBroadcasting()
        {
            _wardNode.IsBroadcastEnabled = !_wardNode.IsBroadcastEnabled;
        }

        private void AddNewAnonymousPatient()
        {
            _wardNode.AddPatient(new Patient());
        }
        private void ToggleWebAPi()
        {
            _wardNode.IsWebApiEnabled = !_wardNode.IsWebApiEnabled;
        }
    }
}
