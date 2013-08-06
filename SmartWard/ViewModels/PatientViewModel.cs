using System;
using System.Windows.Input;
using ABC.Model.Primitives;
using SmartWard.Commands;
using SmartWard.Models;
using System.Collections.ObjectModel;

namespace SmartWard.ViewModels
{
    public class PatientViewModel:ViewModelBase
    {
        private readonly Patient _patient;

        public event EventHandler PatientUpdated;
        public event EventHandler PatientSelected;

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
            Status++;

            if (PatientUpdated != null)
                PatientUpdated(_patient, new EventArgs());
        }


        public PatientViewModel(Patient patient)
        {
            _patient = patient;
        }

        public int RoomNumber
        {
            get { return _patient.RoomNumber; }
            set
            {
                _patient.RoomNumber = value;
                OnPropertyChanged("RoomNumber");
            }
        }

        public Patient Patient{
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

        public string Procedure
        {
            get { return _patient.Procedure; }
            set
            {
                _patient.Procedure = value;
                OnPropertyChanged("Procedure");
            }
        }
        public string Plan
        {
            get { return _patient.Plan; }
            set
            {
                _patient.Plan = value;
                OnPropertyChanged("Plan");
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

        public ObservableCollection<NurseRecord> NurseRecords
        {
            get { return _patient.NurseRecords; }
            set { _patient.NurseRecords = value; }
        }
    }
}