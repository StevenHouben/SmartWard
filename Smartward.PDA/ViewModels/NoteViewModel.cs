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
    public class NoteViewModel : ResourceViewModel
    {

        #region Properties
        public Note Note
        {
            get { return (Note)Resource; }
        }
        public string Status
        {
            get
            {
                string status = "";
                if (Note.Fasting)
                {
                    status += "F ";
                }
                // TODO: If no EWS exist for patient, then add % to status string.
                //if (!ews)
                //{
                //    status += "%"
                //}
                return status;
            }
        }
        #endregion

        public NoteViewModel(Note n, WardNode wardNode) : base(n, wardNode)
        {
            n.PropertyChanged += new PropertyChangedEventHandler(EWSChanged);
        }
        public void EWSChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Status");
        }
    }
}
