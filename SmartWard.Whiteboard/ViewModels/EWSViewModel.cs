using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartWard.ViewModels;
using SmartWard.Models;
using System.Windows.Input;
using SmartWard.Commands;

namespace SmartWard.Whiteboard.ViewModels
{
    public class EWSViewModel : ViewModelBase
    {
        private readonly EWS _ews;

        public event EventHandler EWSUpdated;

        private ICommand _updateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                return _updateCommand ?? (_updateCommand = new RelayCommand(
                    param => UpdateEWS(),
                    param => CanUpdateEWS()
                    ));
            }
        }

        public EWSViewModel(EWS ews)
        {
            _ews = ews;
        }
        private bool CanUpdateEWS()
        {
            return true;
        }

        public void UpdateEWS()
        {
            Value++;

            if (EWSUpdated != null)
                EWSUpdated(_ews, new EventArgs());
        }
        
        public int Value
        {
            get { return _ews.Value; }
            set
            {
                _ews.Value = value;
                OnPropertyChanged("Value");
            }
        }
    }
}
