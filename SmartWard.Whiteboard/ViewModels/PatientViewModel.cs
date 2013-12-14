using System;
using System.Windows.Input;
using ABC.Model.Primitives;
using SmartWard.Commands;
using SmartWard.Models;
using SmartWard.ViewModels;
using SmartWard.Infrastructure;
using System.Collections.ObjectModel;

namespace SmartWard.Whiteboard.ViewModels
{
    public class PatientViewModel : ViewModelBase
    {
        private readonly Patient _patient;
        private EWSViewModel _ewsViewModel;
        private WardNode _wardNode;

        public event EventHandler PatientUpdated;
        public event EventHandler PatientSelected;

        #region ICommands

        private ICommand _updateCommand;

        public ICommand UpdateCommand
        {
            get
            {
                return _updateCommand ?? (_updateCommand = new RelayCommand(
                    param => UpdatePatient(),
                    param => CanUpdatePatient()
                    ));
            }
        }

        private ICommand _selectCommand;

        public ICommand SelectCommand
        {
            get
            {
                return _selectCommand ?? (_selectCommand = new RelayCommand(
                   param => SelectPatient(),
                    param => CanSelectPatient()
                    ));
            }
        }

        private ICommand _newEWSCommand;

        public ICommand NewEWSCommand
        {
            get
            {
                return _updateCommand ?? (_updateCommand = new RelayCommand(
                    param => NewEWS(),
                    param => CanCreateNewEWS()
                    ));
            }
        }
        
        private bool CanCreateNewEWS()
        {
            return true;
        }

        public void NewEWS()
        {
            WardNode.NewEWS(new EWS(_patient.Id));
        }

        private bool CanSelectPatient()
        {
            return true;
        }

        private void SelectPatient()
        {
            if (PatientSelected != null)
                PatientSelected(this, new EventArgs());
        }


        public void UpdateAllProperties(Patient data)
        {
            _patient.UpdateAllProperties(data);
        }
        private bool CanUpdatePatient()
        {
            return true;
        }

        public void UpdatePatient()
        {

            if (PatientUpdated != null)
                PatientUpdated(_patient, new EventArgs());
        }
        #endregion

        public PatientViewModel(Patient patient, WardNode wardNode)
        {
            _patient = patient;
            WardNode = wardNode;
        }

        public WardNode WardNode { get; set; }

        public int RoomNumber
        {
            get { return _patient.RoomNumber; }
            set
            {
                _patient.RoomNumber = value;
                OnPropertyChanged("RoomNumber");
            }
        }

        public Patient Patient
        {
            get { return _patient; }
        }

        public string Tag
        {
            get { return _patient.Tag; }
            set
            {
                _patient.Tag = value;
                OnPropertyChanged("Tag");
            }
        }

        public string Cid
        {
            get { return _patient.Cid; }
            set
            {
                _patient.Cid = value;
                OnPropertyChanged("Cid");
            }
        }

        public int Status
        {
            get { return _patient.Status; }
            set
            {
                _patient.Status = value;
                OnPropertyChanged("Status");
            }
        }

        public Rgb Color
        {
            get { return _patient.Color; }
            set
            {
                _patient.Color = value;
                OnPropertyChanged("Color");
            }
        }


        public bool Selected
        {
            get { return _patient.Selected; }
            set
            {
                _patient.Selected = value;
                OnPropertyChanged("Selected");
            }
        }

        public string Cpr
        {
            get { return _patient.Cpr; }
            set
            {
                _patient.Cpr = value;
                OnPropertyChanged("Cpr");
            }
        }

        public int State
        {
            get { return _patient.State; }
            set
            {
                _patient.State = value;
                OnPropertyChanged("State");
            }
        }

        public string Name
        {
            get { return _patient.Name; }
            set
            {
                _patient.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Id
        {
            get { return _patient.Id; }
            set
            {
                _patient.Id = value;
                OnPropertyChanged("Id");
            }
        }
        
        public EWSViewModel EWSViewModel
        {
            get
            {
                if (_ewsViewModel == null)
                {
                    _ewsViewModel = new EWSViewModel(new EWS(_patient.Id));
                    OnPropertyChanged("EWSViewModel");
                }
                return _ewsViewModel;
            }
        }
    }
}