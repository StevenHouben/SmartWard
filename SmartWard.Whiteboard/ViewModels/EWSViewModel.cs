using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartWard.ViewModels;
using SmartWard.Models;
using System.Windows.Input;
using SmartWard.Commands;
using SmartWard.Infrastructure;

namespace SmartWard.Whiteboard.ViewModels
{
    public class EWSViewModel : ViewModelBase
    {
        private readonly EWS _ews;
        private WardNode _wardNode;

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

        public EWSViewModel(EWS ews, WardNode wardNode)
        {
            _ews = ews;
            _wardNode = wardNode;
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

        public EWS EWS
        {
            get { return _ews; }
        }
        public WardNode WardNode
        {
            get { return _wardNode; }
            set
            {
                _wardNode = value;
            }
        }

        public void SaveEWS()
        {
            WardNode.NewEWS(EWS);
        }
    }
}
