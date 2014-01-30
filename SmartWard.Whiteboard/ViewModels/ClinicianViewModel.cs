using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using NooSphere.Model.Primitives;
using SmartWard.Commands;
using SmartWard.Models;
using SmartWard.ViewModels;



namespace SmartWard.Whiteboard.ViewModels
{
    public class ClinicianViewModel : ViewModelBase
    {
        private readonly Clinician _clinician;

        public Clinician Clinician
        {
            get { return _clinician; }
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
    }
}
