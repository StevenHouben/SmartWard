using SmartWard.Infrastructure;
using SmartWard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.ViewModels
{
    public class EWSViewModelBase : ResourceViewModelBase
    {
        private string _identifier;
        public EWSViewModelBase(EWS ews, WardNode wardNode)
            : base(ews) 
        {
            var patient = wardNode.UserCollection.FirstOrDefault(u => u.Type == typeof(Patient).Name && u.Id == ews.PatientId) as Patient;
            if (patient != null) Identifier = patient.Name + ": " + patient.Cpr;
            WardNode = wardNode;
            ews.PropertyChanged += EWSValueChanged;
        }

        public EWS EWS { get { return Resource as EWS; } }

        public WardNode WardNode { get; set; }
        private void EWSValueChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Value");
        }

        #region EWS Properties
        public int Value { get { return EWS.GetEWS(); } }
        public string Identifier
        {
            get { return _identifier; }
            set
            {
                _identifier = value;
                OnPropertyChanged("Identifier");
            }
        }
        public string PatientId
        {
            get { return EWS.PatientId; }
            set
            {
                EWS.PatientId = value;
                OnPropertyChanged("PatientId");
            }
        }
        public int HeartRate
        {
            get { return EWS.HeartRate; }
            set
            {
                EWS.HeartRate = value;
                OnPropertyChanged("heartRate");
            }
        }
        public int SystolicBloodPressure
        {
            get { return EWS.SystolicBloodPressure; }
            set
            {
                EWS.SystolicBloodPressure = value;
                OnPropertyChanged("systolicBloodPressure");
            }
        }
        public int RespiratoryRate
        {
            get { return EWS.RespiratoryRate; }
            set
            {
                EWS.RespiratoryRate = value;
                OnPropertyChanged("respiratoryRate");
            }
        }
        public double Temperature
        {
            get { return EWS.Temperature; }
            set
            {
                EWS.Temperature = value;
                OnPropertyChanged("temperature");
            }
        }
        public int Sp02
        {
            get { return EWS.Sp02; }
            set
            {
                EWS.Sp02 = value;
                OnPropertyChanged("sp02");
            }
        }
        public string CentralNervousSystem
        {
            get { return EWS.CentralNervousSystem; }
            set
            {
                EWS.CentralNervousSystem = value;
                OnPropertyChanged("centralNervousSystem");
            }
        }
        #endregion
    }
}
