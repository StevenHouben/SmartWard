﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABC.Model.Resources;

namespace SmartWard.Models
{
    public class EWS : Resource
    {
        private int _heartRate;
        private int _systolicBloodPressure;
        private int _respiratoryRate;
        private double _temperature;
        private int _sp02; // Blood oxygen saturation
        private string _centralNervousSystem;
        private string _patientId;
        private string _clinicianId;
        private DateTime _timestamp;

        private int value;
        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        #region properties
        public int HeartRate
        {
            get { return _heartRate; }
            set 
            { 
                _heartRate = value;
                OnPropertyChanged("heartRate");
            }
        }
        public int SystolicBloodPressure
        {
            get { return _systolicBloodPressure; }
            set 
            {
                _systolicBloodPressure = value;
                OnPropertyChanged("systolicBloodPressure");
            }
        }
        public int RespiratoryRate
        {
            get { return _respiratoryRate; }
            set 
            {
                _respiratoryRate = value;
                OnPropertyChanged("respiratoryRate");
            }
        }
        public double Temperature
        {
            get { return _temperature; }
            set 
            { 
                _temperature = value;
                OnPropertyChanged("temperature");
            }
        }
        public int Sp02
        {
            get { return _sp02; }
            set 
            { 
                _sp02 = value;
                OnPropertyChanged("Sp02");
            }
        }
        public string CentralNervousSystem
        {
            get { return _centralNervousSystem; }
            set 
            { 
                _centralNervousSystem = value;
                OnPropertyChanged("centralNervousSystem");
            }
        }
        public string PatientId
        {
            get { return _patientId; }
        }
        public string ClinicianId
        {
            get { return _clinicianId; }
        }
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }
#endregion
        public EWS(string patientId)
        {
            _patientId = patientId;
            //_clinicianId = clinicianId;
            _timestamp = DateTime.Now;
        }
        public int GetHeartRateScore()
        {
            int score = -1;
            if (HeartRate <= 40)
            {
                score = 2;
            }
            else if (HeartRate >= 41 && HeartRate <= 50)
            {
                score = 1;
            }
            else if (HeartRate >= 51 && HeartRate <= 100)
            {
                score = 0;
            }
            else if (HeartRate >= 101 && HeartRate <= 110)
            {
                score = 1;
            }
            else if (HeartRate >= 111 && HeartRate <= 129)
            {
                score = 2;
            }
            else
            {
                score = 3;
            }

            return score;
        }
        public int GetSystolicBloodPressureScore()
        {
            int score = -1;
            if (SystolicBloodPressure <= 70)
            {
                score = 3;
            }
            else if (SystolicBloodPressure >= 71 && SystolicBloodPressure <= 80)
            {
                score = 2;
            }
            else if (SystolicBloodPressure >= 81 && SystolicBloodPressure <= 100)
            {
                score = 1;
            }
            else if (SystolicBloodPressure >= 101 && SystolicBloodPressure <= 199)
            {
                score = 0;
            }
            else
            {
                score = 2;
            }

            return score;
        }
        public int GetRespiratoryRateScore()
        {
            int score = -1;
            if (RespiratoryRate <= 8)
            {
                score = 2;
            }
            else if (RespiratoryRate >= 9 && RespiratoryRate <= 20)
            {
                score = 0;
            }
            else if (RespiratoryRate >= 21 && RespiratoryRate <= 24)
            {
                score = 1;
            }
            else if (RespiratoryRate >= 25 && RespiratoryRate <= 29)
            {
                score = 2;
            }
            else
            {
                score = 3;
            }

            return score;
        }
        public int GetTemperatureScore()
        {
            int score = -1;
            if (Temperature <= 35)
            {
                score = 2;
            }
            else if (Temperature >= 35.1 && Temperature <= 36.0)
            {
                score = 1;
            }
            else if (Temperature >= 36.1 && Temperature <= 37.9)
            {
                score = 0;
            }
            else if (Temperature >= 38 && Temperature <= 38.9)
            {
                score = 1;
            }
            else
            {
                score = 2;
            }

            return score;
        }
        public int GetSp02Score()
        {
            int score = -1;
            if (Sp02 <= 85)
            {
                score = 3;
            }
            else if (Sp02 >= 86 && Sp02 <= 91)
            {
                score = 2;
            }
            else if (Sp02 >= 92 && Sp02 <= 93)
            {
                score = 1;
            }
            else
            {
                score = 0;
            }

            return score;
        }
        public int GetCentralNervousSystemScore()
        {
            int score = -1;
            if (CentralNervousSystem.Equals("A")) // Alert
            {
                score = 0;
            }
            else if (CentralNervousSystem.Equals("V")) // Voice
            {
                score = 1;
            }
            else if (CentralNervousSystem.Equals("NC")) // New confusion
            {
                score = 1;
            }
            else if (CentralNervousSystem.Equals("P")) // Pain
            {
                score = 2;
            }
            else if (CentralNervousSystem.Equals("U")) // Unresponsive
            {
                score = 3;
            }

            return score;
        }
        public int GetEWS()
        {
            return GetHeartRateScore() + GetSystolicBloodPressureScore() + GetRespiratoryRateScore() + GetTemperatureScore() + GetSp02Score() + GetCentralNervousSystemScore();
        }

    }
}