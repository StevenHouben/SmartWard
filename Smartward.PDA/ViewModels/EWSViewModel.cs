using SmartWard.Infrastructure;
using SmartWard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.PDA.ViewModels
{
    public class EWSViewModel : ResourceViewModel
    {
        #region Properties
        public EWS EWS
        {
            get { return (EWS)Resource; }
        }
        public string Status
        {
            get { return EWS.GetEWS().ToString(); }
        }
        #endregion
        public EWSViewModel(EWS e, WardNode wardNode) : base(e, wardNode)
        {
            e.PropertyChanged += new PropertyChangedEventHandler(EWSChanged);
            
        }
        public void EWSChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Status");
        }
    }
}
