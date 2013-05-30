using SmartWard.Primitives;
using SmartWard.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Whiteboard.Model
{
    public class Patient:Base
    {
        private int roomNumber;
        public int RoomNumber
        {
            get { return this.roomNumber; }
            set
            {
                this.roomNumber = value;
                OnPropertyChanged("roomNumber");
            }
        }

        public User User { get; set; }

        private string procedure;
        public string Procedure 
        {
            get { return this.procedure; }
            set
            {
                this.procedure = value;
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
