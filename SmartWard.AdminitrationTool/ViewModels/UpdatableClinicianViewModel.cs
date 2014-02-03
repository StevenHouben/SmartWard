using SmartWard.Commands;
using SmartWard.Models;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartWard.AdministrationTool.ViewModels
{
    class UpdatableClinicianViewModel : ClinicianViewModelBase
    {
        public UpdatableClinicianViewModel(Clinician clinician) : base(clinician) { }

        public event EventHandler ClinicianUpdated;

        private ICommand _updateClinicianCommand;

        public ICommand UpdateClinicianCommand
        {
            get
            {
                return _updateClinicianCommand ?? (_updateClinicianCommand = new RelayCommand(
                    param => UpdateClinician(),
                    param => true //Clinicians may always be saved
                    ));
            }
        }

        public void UpdateAllProperties(Clinician data)
        {
            Clinician.UpdateAllProperties(data);
        }

        public void UpdateClinician()
        {
            if (ClinicianUpdated != null)
                ClinicianUpdated(Clinician, new EventArgs());
        }
    }
}
