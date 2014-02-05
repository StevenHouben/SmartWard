using SmartWard.Infrastructure;
using SmartWard.ViewModels;
using SmartWard.Models.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartWard.Models;
using System.ComponentModel;


namespace SmartWard.PDA.ViewModels
{
    public class ActivityViewModel : ViewModelBase
    {
        private Activity _activity;

        #region Properties
        public Activity Activity
        {
            get { return _activity; }
            set
            {
                _activity = value;
                OnPropertyChanged("Activity");
            }
        }

        public string Id
        {
            get { return _activity.Id; }
            set
            {
                _activity.Id = value;
                OnPropertyChanged("Id");
            }
        }
       
        public string Info
        {
            get { return _activity.Info; }
        }
        public string Status
        {
            get 
            { 
                switch (_activity.Type)
                {
                    case "RoundActivity":
                        return ((RoundActivity)_activity).Status;
                    default:
                        return _activity.Status;
                }
            }
        }
        public List<string> Participants
        {
            get { return _activity.Participants; }
            set {
                _activity.Participants = value;
                OnPropertyChanged("Participants");
            }
        }
        public string Name
        {
            get { 
                switch (_activity.Type)
                {
                    case "RoundActivity":
                        return "Round";
                        break;
                }
                return "";
            }
        }
        public WardNode WardNode { get; set; }
        #endregion
        public event EventHandler ActivityUpdated;

        public ActivityViewModel(Activity activity, WardNode wardNode)
        {
            Activity = activity;
            Activity.PropertyChanged += new PropertyChangedEventHandler(StatusChanged);
            WardNode = wardNode;
        }

        public void StatusChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Status");
        }
    }
}
