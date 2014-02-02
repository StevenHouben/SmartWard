﻿using System;
using System.Linq;
using System.Windows.Input;
using NooSphere.Model.Primitives;
using SmartWard.Commands;
using SmartWard.Models;
using SmartWard.ViewModels;
using SmartWard.Infrastructure;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SmartWard.Whiteboard.ViewModels
{
    public class BoardRowViewModel : ViewModelBase
    {
        private readonly Patient _patient;
        private EWSViewModel _ewsViewModel;
        private NoteViewModel _noteViewModel;
        private WardNode _wardNode;
        private IList<ClinicianViewModel> _dayClinicians;
        private IList<ClinicianViewModel> _eveningClinicians;
        private IList<ClinicianViewModel> _nightClinicians;
        private IList<ClinicianViewModel> _roundClinicians;
        public BoardViewModel Parent { get; set; }

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

        public IList<ClinicianViewModel> DayClinicians
        {
            get { return _dayClinicians; }
            protected set
            {
                _dayClinicians = value;
                OnPropertyChanged("DayCliniciansDisplay");
            }
        }
        public string DayCliniciansDisplay
        {
            get { 
                string names = DayClinicians.Aggregate("", (s, c) => s + c.Name + ", ");
                return String.IsNullOrEmpty(names) ? names : names.Substring(0, names.Length - 2);
            }
        }

        public IList<ClinicianViewModel> EveningClinicians
        {
            get { return _eveningClinicians; }
            protected set
            {
                _eveningClinicians = value;
                OnPropertyChanged("EveningCliniciansDisplay");
            }
        }
        public string EveningCliniciansDisplay
        {
            get
            {
                string names = EveningClinicians.Aggregate("", (s, c) => s + c.Name + ", ");
                return String.IsNullOrEmpty(names) ? names : names.Substring(0, names.Length - 2);
            }
        }

        public IList<ClinicianViewModel> NightClinicians
        {
            get { return _nightClinicians; }
            protected set
            {
                _nightClinicians = value;
                OnPropertyChanged("NightCliniciansDisplay");
            }
        }
        public string NightCliniciansDisplay
        {
            get
            {
                string names = NightClinicians.Aggregate("", (s, c) => s + c.Name + ", ");
                return String.IsNullOrEmpty(names) ? names : names.Substring(0, names.Length - 2);
            }
        }

        public IList<ClinicianViewModel> RoundClinicians
        {
            get { return _roundClinicians; }
            protected set
            {
                _roundClinicians = value;
                OnPropertyChanged("RoundCliniciansDisplay");
            }
        }
        public string RoundCliniciansDisplay
        {
            get
            {
                string names = RoundClinicians.Aggregate("", (s, c) => s + c.Name + ", ");
                return String.IsNullOrEmpty(names) ? names : names.Substring(0, names.Length - 2);
            }
        }

        /*public void DayClinicians_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ClinicianViewModel cvm in e.NewItems)
                {
                    DayClinicians.Add(cvm);
                    cvm.Clinician.AssignedPatients.Add(new Tuple<string, SmartWard.Models.Clinician.AssignmentType>(Patient.Id, SmartWard.Models.Clinician.AssignmentType.Day));
                    WardNode.UpdateUser(cvm.Clinician);
                }
                OnPropertyChanged("DayCliniciansDisplay");
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ClinicianViewModel cvm in e.OldItems)
                {
                    DayClinicians.Remove(cvm);
                    var assignment = cvm.Clinician.AssignedPatients.First(a => a.Item1 == Patient.Id && a.Item2 == Clinician.AssignmentType.Day);
                    cvm.Clinician.AssignedPatients.Remove(assignment);
                    WardNode.UpdateUser(cvm.Clinician);
                }
                OnPropertyChanged("DayCliniciansDisplay");
            }
        }*/
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

        public BoardRowViewModel(Patient patient, WardNode wardNode, BoardViewModel parent)
        {
            _patient = patient;
            WardNode = wardNode;
            Parent = parent;

            var ews = from resource in WardNode.ResourceCollection.ToList()
                      where resource.Type == typeof(EWS).Name && ((EWS)resource).PatientId == _patient.Id 
                      orderby ((EWS)resource).Created descending
                      select resource;
            
            var notes = from resource in WardNode.ResourceCollection.ToList()
                        where resource.Type == typeof(Note).Name && ((Note)resource).PatientId == _patient.Id 
                      orderby ((Note)resource).Created descending
                      select resource;

            //Hook up to parent's clinicians
            Parent.Clinicians.CollectionChanged += ParentClinicians_CollectionChanged;
            //Hook up to parent's activities
            Parent.RoundActivities.CollectionChanged += ParentRoundActivities_CollectionChanged;
            
            EWSViewModel = new EWSViewModel((EWS)ews.FirstOrDefault() ?? new EWS(Patient.Id), Patient, WardNode);
            NoteViewModel = new NoteViewModel((Note)notes.FirstOrDefault() ?? new Note(Patient.Id, ""), WardNode);
        }

        public void ParentClinicians_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Reevaluate daily shift assignments
            var assignedClinicians = from clinician in Parent.Clinicians
                                     where ((ClinicianViewModel)clinician).Clinician.AssignedPatients.Any(a => a.Item1 == Patient.Id)
                                     select clinician as ClinicianViewModel;
            IList<ClinicianViewModel> dayClinicians = new List<ClinicianViewModel>();
            IList<ClinicianViewModel> eveningClinicians = new List<ClinicianViewModel>();
            IList<ClinicianViewModel> nightClinicians = new List<ClinicianViewModel>();
            foreach (ClinicianViewModel cvm in assignedClinicians)
            {
                foreach (Tuple<string, SmartWard.Models.Clinician.AssignmentType> assignment in cvm.Clinician.AssignedPatients)
                {
                    if (assignment.Item1 != Patient.Id) continue;
                    switch (assignment.Item2)
                    {
                        case Clinician.AssignmentType.Day:
                            dayClinicians.Add(cvm);
                            break;
                        case Clinician.AssignmentType.Evening:
                            eveningClinicians.Add(cvm);
                            break;
                        case Clinician.AssignmentType.Night:
                            nightClinicians.Add(cvm);
                            break;
                        default:
                            throw new NotImplementedException("Assignment");
                    }
                }
            }
            DayClinicians = dayClinicians;
            EveningClinicians = eveningClinicians;
            NightClinicians = nightClinicians;
        }

        public void ParentRoundActivities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Find the clinicians which have unfinished rounds that are visiting this patient
            var clinicianIds = from activity in Parent.RoundActivities
                              where activity.Visits.Any(v => v.PatientId == Patient.Id) && !activity.IsFinished
                              select activity.ClinicianId;
            RoundClinicians = Parent.Clinicians.Where(c => clinicianIds.Contains(c.Id)).ToList();
            
        }
    }
}