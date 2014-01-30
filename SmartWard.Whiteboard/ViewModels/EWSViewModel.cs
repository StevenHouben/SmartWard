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
        private readonly string _identifier;
        private WardNode _wardNode;

        public event EventHandler EWSUpdated;

        public EWSViewModel(EWS ews, Patient p, WardNode wardNode)
        {
            _identifier = p.Name + ": " + p.Cpr; 
            _ews = ews;
            _wardNode = wardNode;
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
        public string Identifier
        {
            get { return _identifier; }
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
            WardNode.AddResource(EWS);
        }
    }
}
