using System.Collections.ObjectModel;
using NooSphere.Model.Users;
using System;

namespace SmartWard.Models
{
    public class Patient : User
    {
        private int _roomNumber;
        private int _status;
        private string _cprNumber;
        private DateTime _discharging;

        public Patient()
        {
            Type = typeof(Patient).Name;
            Name = "Name";
            Cpr = "ddmm-yyy-xxx";
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

        public string Cpr
        {
            get { return _cprNumber; }
            set
            {
                _cprNumber = value;
                OnPropertyChanged("Cpr");
            }
        }

        public DateTime Discharging
        {
            get { return _discharging; }
            set
            {
                _discharging = value;
                OnPropertyChanged("Discharging");
            }
        }
    }
}