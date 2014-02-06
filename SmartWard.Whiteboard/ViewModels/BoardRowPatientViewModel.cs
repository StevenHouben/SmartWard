using System;
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
    public class BoardRowPatientViewModel : PatientViewModelBase
    {
        private EWSViewModelBase _ewsViewModel;
        private NoteViewModelBase _noteViewModel;

        public BoardRowPatientViewModel(Patient patient, WardNode wardNode, BoardViewModel parent)
            : base(patient)
        {
            WardNode = wardNode;
            Parent = parent;

            //Initialize Collections and make them update bindings on change
            DayClinicians = new ObservableCollection<ClinicianViewModelBase>();
            DayClinicians.CollectionChanged += (s, e) => OnPropertyChanged("DayCliniciansDisplay");
            EveningClinicians = new ObservableCollection<ClinicianViewModelBase>();
            EveningClinicians.CollectionChanged += (s, e) => OnPropertyChanged("EveningCliniciansDisplay");
            NightClinicians = new ObservableCollection<ClinicianViewModelBase>();
            NightClinicians.CollectionChanged += (s, e) => OnPropertyChanged("NightCliniciansDisplay");
            RoundClinicians = new ObservableCollection<ClinicianViewModelBase>();
            RoundClinicians.CollectionChanged += (s, e) => OnPropertyChanged("RoundCliniciansDisplay");

            var ews = from resource in WardNode.ResourceCollection.ToList()
                      where resource.Type == typeof(EWS).Name && ((EWS)resource).PatientId == Patient.Id
                      orderby ((EWS)resource).Created descending
                      select resource;

            var notes = from resource in WardNode.ResourceCollection.ToList()
                        where resource.Type == typeof(Note).Name && ((Note)resource).PatientId == Patient.Id
                        orderby ((Note)resource).Created descending
                        select resource;

            //Hook up to parent's clinicians
            Parent.Clinicians.CollectionChanged += ParentClinicians_CollectionChanged;
            //Hook up to parent's activities
            Parent.RoundActivities.CollectionChanged += ParentRoundActivities_CollectionChanged;
            //Hook up to parent's EWSs
            Parent.EWSs.CollectionChanged += ParentEWSs_CollectionChanged;
            //Hook up to parent's Notes
            Parent.Notes.CollectionChanged += ParentNotes_CollectionChanged;
        }

        #region Properties
        public BoardViewModel Parent { get; set; }
        public WardNode WardNode { get; set; }
        public ObservableCollection<ClinicianViewModelBase> DayClinicians { get; set; }
        public string DayCliniciansDisplay
        {
            get
            {
                string names = DayClinicians.Aggregate("", (s, c) => s + c.Name + ", ");
                return String.IsNullOrEmpty(names) ? names : names.Substring(0, names.Length - 2);
            }
        }
        public ObservableCollection<ClinicianViewModelBase> EveningClinicians { get; set; }
        public string EveningCliniciansDisplay
        {
            get
            {
                string names = EveningClinicians.Aggregate("", (s, c) => s + c.Name + ", ");
                return String.IsNullOrEmpty(names) ? names : names.Substring(0, names.Length - 2);
            }
        }
        public ObservableCollection<ClinicianViewModelBase> NightClinicians { get; set; }
        public string NightCliniciansDisplay
        {
            get
            {
                string names = NightClinicians.Aggregate("", (s, c) => s + c.Name + ", ");
                return String.IsNullOrEmpty(names) ? names : names.Substring(0, names.Length - 2);
            }
        }
        public ObservableCollection<ClinicianViewModelBase> RoundClinicians { get; set; }
        public string RoundCliniciansDisplay
        {
            get
            {
                string names = RoundClinicians.Aggregate("", (s, c) => s + c.Name + ", ");
                return String.IsNullOrEmpty(names) ? names : names.Substring(0, names.Length - 2);
            }
        }
        public EWSViewModelBase EWSViewModel
        {
            get { return _ewsViewModel; }
            set
            {
                _ewsViewModel = value;
                OnPropertyChanged("EWSViewModel");
            }
        }
        public NoteViewModelBase NoteViewModel
        {
            get { return _noteViewModel; }
            set
            {
                _noteViewModel = value;
                OnPropertyChanged("NoteViewModel");
            }
        }
        #endregion

        #region ICommands
        public event EventHandler PatientSelected;
        private ICommand _selectCommand;
        public ICommand SelectCommand
        {
            get
            {
                return _selectCommand ?? (_selectCommand = new RelayCommand(
                   param => SelectPatient(),
                    param => true //Patients can always be selected
                    ));
            }
        }
        private void SelectPatient()
        {
            if (PatientSelected != null)
                PatientSelected(this, new EventArgs());
        }
        #endregion

        #region Parent Collection Hooks
        public void ParentClinicians_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DayClinicians.Clear();
            EveningClinicians.Clear();
            NightClinicians.Clear();
            //Reevaluate daily shift assignments
            var assignedClinicians = from clinician in Parent.Clinicians
                                     where ((ClinicianViewModelBase)clinician).Clinician.AssignedPatients.Any(a => a.Item1 == Patient.Id)
                                     select clinician as ClinicianViewModelBase;
            foreach (ClinicianViewModelBase cvm in assignedClinicians)
            {
                foreach (Tuple<string, SmartWard.Models.Clinician.AssignmentType> assignment in cvm.Clinician.AssignedPatients)
                {
                    if (assignment.Item1 != Patient.Id) continue;
                    switch (assignment.Item2)
                    {
                        case Clinician.AssignmentType.Day:
                            DayClinicians.Add(cvm);
                            break;
                        case Clinician.AssignmentType.Evening:
                            EveningClinicians.Add(cvm);
                            break;
                        case Clinician.AssignmentType.Night:
                            NightClinicians.Add(cvm);
                            break;
                        default:
                            throw new NotImplementedException("Assignment");
                    }
                }
            }
        }

        public void ParentRoundActivities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (RoundActivity ra in e.NewItems)
                    {
                        //Continues if no visits are relevant
                        if(!ra.Visits.Any(v => v.PatientId == Patient.Id)) continue;
                        //Otherwise add the clinician to the list if it isn't already there
                        if (!RoundClinicians.Any(rc => rc.Id == ra.ClinicianId)) RoundClinicians.Add(Parent.Clinicians.First(cvm => cvm.Id == ra.ClinicianId));
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (RoundActivity ra in e.OldItems)
                    {
                        //Continues if no visits are relevant
                        if (!ra.Visits.Any(v => v.PatientId == Patient.Id)) continue;
                        //Otherwise remove the clinician from the list if it's there
                        var c = RoundClinicians.FirstOrDefault(rc => rc.Id == ra.ClinicianId);
                        if (c != null) RoundClinicians.Remove(c);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RoundClinicians.Clear();
                    Parent.RoundActivities.Where(ra => ra.Visits.Any(v => v.PatientId == Patient.Id)).ToList().ForEach(ra => RoundClinicians.Add(Parent.Clinicians.First(cvm => cvm.Id == ra.ClinicianId)));
                    break;
            }
        }

        public void ParentEWSs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (EWSViewModelBase ews in e.NewItems)
                    {
                        if (ews.PatientId == Patient.Id) EWSViewModel = ews; 
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (EWSViewModelBase ews in e.OldItems)
                    {
                        if (ews.PatientId == Patient.Id) EWSViewModel = null;
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (EWSViewModelBase ews in e.NewItems)
                    {
                        if (ews.PatientId == Patient.Id) EWSViewModel = ews;
                    }
                    break;
            }
        }

        public void ParentNotes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (NoteViewModelBase note in e.NewItems)
                    {
                        if (note.PatientId == Patient.Id) NoteViewModel = note;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (NoteViewModelBase note in e.OldItems)
                    {
                        if (note.PatientId == Patient.Id) NoteViewModel = null;
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (NoteViewModelBase note in e.NewItems)
                    {
                        if (note.PatientId == Patient.Id) NoteViewModel = note;
                    }
                    break;
            }
        }
        #endregion
    }
}