﻿using System;
using System.Linq;
using System.Windows.Input;
using NooSphere.Model.Primitives;
using SmartWard.Commands;
using SmartWard.Models;
using SmartWard.ViewModels;
using SmartWard.Infrastructure;
using System.Collections.ObjectModel;

namespace SmartWard.Whiteboard.ViewModels
{
    public class PatientViewModel : ViewModelBase
    {
        private readonly Patient _patient;
        private EWSViewModel _ewsViewModel;
        private NoteViewModel _noteViewModel;
        private WardNode _wardNode;

        #region properties
        public WardNode WardNode { get; set; }

        public int RoomNumber
        {
            get { return _patient.RoomNumber; }
            set
            {
                _patient.RoomNumber = value;
                OnPropertyChanged("RoomNumber");
            }
        }

        public Patient Patient
        {
            get { return _patient; }
        }

        public string Tag
        {
            get { return _patient.Tag; }
            set
            {
                _patient.Tag = value;
                OnPropertyChanged("Tag");
            }
        }

        public string Cid
        {
            get { return _patient.Cid; }
            set
            {
                _patient.Cid = value;
                OnPropertyChanged("Cid");
            }
        }

        public int Status
        {
            get { return _patient.Status; }
            set
            {
                _patient.Status = value;
                OnPropertyChanged("Status");
            }
        }

        public Rgb Color
        {
            get { return _patient.Color; }
            set
            {
                _patient.Color = value;
                OnPropertyChanged("Color");
            }
        }


        public bool Selected
        {
            get { return _patient.Selected; }
            set
            {
                _patient.Selected = value;
                OnPropertyChanged("Selected");
            }
        }

        public string Cpr
        {
            get { return _patient.Cpr; }
            set
            {
                _patient.Cpr = value;
                OnPropertyChanged("Cpr");
            }
        }

        public int State
        {
            get { return _patient.State; }
            set
            {
                _patient.State = value;
                OnPropertyChanged("State");
            }
        }

        public string Name
        {
            get { return _patient.Name; }
            set
            {
                _patient.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Id
        {
            get { return _patient.Id; }
            set
            {
                _patient.Id = value;
                OnPropertyChanged("Id");
            }
        }

        public EWSViewModel EWSViewModel
        {
            get { return _ewsViewModel; }
            set
            {
                _ewsViewModel = value;
            }
        }

        public NoteViewModel NoteViewModel
        {
            get { return _noteViewModel; }
            set
            {
                _noteViewModel = value;
            }
        }

        public DateTime Discharging
        {
            get { return Patient.Discharging; }
            set 
            { 
                Patient.Discharging = value;
                OnPropertyChanged("Discharging");
            }
        }
        #endregion

        public event EventHandler PatientUpdated;
        public event EventHandler PatientSelected;

        #region ICommands

        private ICommand _updateCommand;

        public ICommand UpdateCommand
        {
            get
            {
                return _updateCommand ?? (_updateCommand = new RelayCommand(
                    param => UpdatePatient(),
                    param => CanUpdatePatient()
                    ));
            }
        }

        private ICommand _selectCommand;

        public ICommand SelectCommand
        {
            get
            {
                return _selectCommand ?? (_selectCommand = new RelayCommand(
                   param => SelectPatient(),
                    param => CanSelectPatient()
                    ));
            }
        }

        private ICommand _newEWSCommand;

        public ICommand NewEWSCommand
        {
            get
            {
                return _updateCommand ?? (_updateCommand = new RelayCommand(
                    param => NewEWS(),
                    param => CanCreateNewEWS()
                    ));
            }
        }
        
        private bool CanCreateNewEWS()
        {
            return true;
        }

        public void NewEWS()
        {
            EWS ews = new EWS(_patient.Id);
            WardNode.AddResource(ews);
        }

        private bool CanSelectPatient()
        {
            return true;
        }

        private void SelectPatient()
        {
            if (PatientSelected != null)
                PatientSelected(this, new EventArgs());
        }


        public void UpdateAllProperties(Patient data)
        {
            _patient.UpdateAllProperties(data);
        }
        private bool CanUpdatePatient()
        {
            return true;
        }

        public void UpdatePatient()
        {

            if (PatientUpdated != null)
                PatientUpdated(_patient, new EventArgs());
        }
        #endregion

        public PatientViewModel(Patient patient, WardNode wardNode)
        {
            _patient = patient;
            WardNode = wardNode;

            var ews = from resource in WardNode.ResourceCollection.ToList()
                      where resource.Type == typeof(EWS).Name && ((EWS)resource).PatientId == _patient.Id 
                      orderby ((EWS)resource).Created descending
                      select resource;
            
            var notes = from resource in WardNode.ResourceCollection.ToList()
                        where resource.Type == typeof(Note).Name && ((Note)resource).PatientId == _patient.Id 
                      orderby ((Note)resource).Created descending
                      select resource;
            
            EWSViewModel = new EWSViewModel((EWS)ews.FirstOrDefault() ?? new EWS(Patient.Id), Patient, WardNode);
            NoteViewModel = new NoteViewModel((Note)notes.FirstOrDefault() ?? new Note(Patient.Id, ""), WardNode);
        }
    }
}