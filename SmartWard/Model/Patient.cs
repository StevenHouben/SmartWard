using ABC.Model.Users;

namespace SmartWard.Model
{
    public class Patient : User
    {
        private int _roomNumber;
        public int RoomNumber
        {
            get { return this._roomNumber; }
            set
            {
                this._roomNumber = value;
                OnPropertyChanged("roomNumber");
            }
        }

        private string _procedure;
        public string Procedure
        {
            get { return this._procedure; }
            set
            {
                this._procedure = value;
                OnPropertyChanged("procedure");
            }
        }

        private string plan;
        public string Plan
        {
            get { return this.plan; }
            set
            {
                this.plan = value;
                OnPropertyChanged("plan");
            }
        }

        private int status;
        public int Status
        {
            get { return this.status; }
            set
            {
                this.status = value;
                if (status > 7)
                    this.status = 1;
                OnPropertyChanged("status");
            }
        }

        public Patient()
        {
            Status = 2;
        }
    }
}
