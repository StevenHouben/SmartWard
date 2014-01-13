using SmartWard.Infrastructure;
using SmartWard.ViewModels;
using SmartWard.Models.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SmartWard.PDA.ViewModels
{
    public class ActivityViewModel : ViewModelBase
    {
        private readonly Activity _activity;

        #region Properties
        public Activity Activity
        {
            get { return _activity; }
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
            set
            {
                _activity.Info = value;
                OnPropertyChanged("Info");
            }
        }
        public string Status
        {
            get { return _activity.Status; }
            set
            {
                _activity.Status = value;
                OnPropertyChanged("Status");
            }
        }
        #endregion
        public event EventHandler ActivityUpdated;

        public ActivityViewModel(Activity activity)
        {
            _activity = activity;
        }
    }
}
