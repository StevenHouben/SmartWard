using System.Collections.ObjectModel;
using NooSphere.Model.Users;

namespace SmartWard.Models
{
    public class Patient : User
    {
        private string _procedure;
        private int _roomNumber;
        private string _plan;
        private int _status;
        private string _cprNumber;

        public Patient()
        {
            Name = "Name";
            Cpr = "ddmm-yyy-xxx";

            NurseRecords = new ObservableCollection<NurseRecord>();
        }

        public int RoomNumber
        {
            get { return _roomNumber; }
            set
            {
                _roomNumber = value;
                OnPropertyChanged("roomNumber");
            }
        }

        public string Procedure
        {
            get { return _procedure; }
            set
            {
                _procedure = value;
                OnPropertyChanged("procedure");
            }
        }

        public string Plan
        {
            get { return _plan; }
            set
            {
                _plan = value;
                OnPropertyChanged("plan");
            }
        }

        public int Status
        {
            get { return _status; }
            set
            {
                _status = value;
                if (_status > 7)
                    _status = 1;
                OnPropertyChanged("status");
            }
        }

        public string Cpr
        {
            get { return _cprNumber; }
            set
            {
                _cprNumber = value;
                OnPropertyChanged("Cpr");
            }
        }
        public ObservableCollection<NurseRecord> NurseRecords { get; set; }
    }
}