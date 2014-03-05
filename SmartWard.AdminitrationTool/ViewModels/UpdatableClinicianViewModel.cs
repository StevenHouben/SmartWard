using SmartWard.AdministrationTool.Views;
using SmartWard.Commands;
using SmartWard.Models;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.Networking.Proximity;

namespace SmartWard.AdministrationTool.ViewModels
{
    class UpdatableClinicianViewModel : ClinicianViewModelBase
    {
        public ProximityDevice ProximityDevice {get; set;}
        private AssociateTokenDialogBox _associateTokenDialog;
        public UpdatableClinicianViewModel(Clinician clinician) : base(clinician) 
        {
            //ProximityDevice = ProximityDevice.GetDefault();
        }

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

        private ICommand _associateTokenCommand;

        public ICommand AssociateTokenCommand
        {
            get
            {
                return _associateTokenCommand ?? (_associateTokenCommand = new RelayCommand(
                    param => AssociateToken(),
                    param => ProximityDevice != null
                    ));
            }
        }

        private void AssociateToken()
        {
            _associateTokenDialog = new AssociateTokenDialogBox() { DataContext = this };
            _associateTokenDialog.Owner = Application.Current.MainWindow;
            _associateTokenDialog.ShowDialog();
            //The execution flow returns here after dialog is closed. Remove the proximity device listener
            ProximityDevice.DeviceArrived -= _proximityDevice_DeviceArrived;
        }

        public void DetectNfc()
        {
            ProximityDevice.DeviceArrived += _proximityDevice_DeviceArrived;
        }

        private void _proximityDevice_DeviceArrived(ProximityDevice sender)
        {
            NfcId = Regex.Match(sender.DeviceId, @"{.*}").Value;
            Application.Current.Dispatcher.Invoke(()=>
                CloseAssociateTokenDialog()
            );
        }

        private ICommand _cancelAssociateTokenCommand;

        public ICommand CancelAssociateTokenCommand
        {
            get
            {
                return _cancelAssociateTokenCommand ?? (_cancelAssociateTokenCommand = new RelayCommand(
                    param => CloseAssociateTokenDialog(),
                    param => true
                    ));
            }
        }

        private void CloseAssociateTokenDialog()
        {
            _associateTokenDialog.Close();
            _associateTokenDialog = null;
        }
    }
}
