using ABC.Model.Users;

namespace SmartWard.Model
{
    public class Patient : User
    {
        private string _procedure;
        private int _roomNumber;
        private string _plan;
        private int _status;

        public Patient()
        {
            Status = 2;
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
    }
}