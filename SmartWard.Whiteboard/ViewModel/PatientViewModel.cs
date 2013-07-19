using System;
using System.Windows.Input;
using ABC.Model.Primitives;
using SmartWard.Model;
using SmartWard.Whiteboard.Commands;

namespace SmartWard.Whiteboard.ViewModel
{
    public class PatientViewModel:ViewModelBase
    {
        private readonly Patient _patient;

        public event EventHandler PatientUpdated;

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
                OnPropertyChanged("selected");
            }
        }


        public int State
        {
            get { return _patient.State; }
            set
            {
                _patient.State = value;
                OnPropertyChanged("state");
            }
        }

        public string Name
        {
            get { return _patient.Name; }
            set
            {
                _patient.Name = value;
                OnPropertyChanged("name");
            }
        }

        public string Id
        {
            get { return _patient.Id; }
            set
            {
                _patient.Id = value;
                OnPropertyChanged("id");
            }
        }
    }
}