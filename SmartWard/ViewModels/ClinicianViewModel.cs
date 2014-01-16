using System;
using System.Windows.Input;
using ABC.Model.Primitives;
using SmartWard.Commands;
using SmartWard.Models;
using System.Collections.ObjectModel;

namespace SmartWard.ViewModels
{
    public class ClinicianViewModel : ViewModelBase
    {
        private readonly Clinician _clinician;

        public event EventHandler ClinicianUpdated;
        
        public void UpdateAllProperties(Clinician data)
        {
            _clinician.UpdateAllProperties(data);
        }
        private bool CanUpdateClinician()
        {
            return true;
        }

        public void UpdateClinician()
        {
            if (ClinicianUpdated != null)
                ClinicianUpdated(_clinician, new EventArgs());
        }


        public ClinicianViewModel(Clinician clinician)
        {
            _clinician = clinician;
        }

        public Clinician Clinician
        {
            get { return _clinician; }
        }

        public string Tag
        {
            get { return _clinician.Tag; }
            set
            {
                _clinician.Tag = value;
                OnPropertyChanged("Tag");
            }
        }

        public string Cid
        {
            get { return _clinician.Cid; }
            set
            {
                _clinician.Cid = value;
                OnPropertyChanged("Cid");
            }
        }

        public Rgb Color
        {
            get { return _clinician.Color; }
            set
            {
                _clinician.Color = value;
                OnPropertyChanged("Color");
            }
        }


        public bool Selected
        {
            get { return _clinician.Selected; }
            set
            {
                _clinician.Selected = value;
                OnPropertyChanged("Selected");
            }
        }

        public int State
        {
            get { return _clinician.State; }
            set
            {
                _clinician.State = value;
                OnPropertyChanged("State");
            }
        }

        public string Name
        {
            get { return _clinician.Name; }
            set
            {
                _clinician.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Id
        {
            get { return _clinician.Id; }
            set
            {
                _clinician.Id = value;
                OnPropertyChanged("Id");
            }
        }

        public Clinician.ClinicianTypeEnum ClinicianType
        {
            get { return _clinician.ClinicianType; }
            set
            {
                _clinician.ClinicianType = value;
                OnPropertyChanged("ClinicianType");
            }
        }
    }
}