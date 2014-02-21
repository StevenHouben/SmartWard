using SmartWard.Commands;
using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.Models.Resources;
using SmartWard.PDA.Helpers;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SmartWard.PDA.ViewModels
{
    public class PatientsLayoutViewModel : PatientViewModelBase
    {
        private VisitActivity _visitActivity;
        private RoundActivity _roundActivity;
        private ICommand _visitDoneCommand;
        public PatientsLayoutViewModel(Patient patient, WardNode wardNode)
            : base(patient)
        {
            //Instantiate properties
            WardNode = wardNode;
            Resources = new ObservableCollection<ResourceViewModelBase>();

            //Hook up to collection events
            Resources.CollectionChanged += Resources_CollectionChanged;
            WardNode.ResourceAdded += WardNode_ResourceAdded;
            WardNode.ResourceChanged += WardNode_ResourceChanged;
            WardNode.ResourceRemoved += WardNode_ResourceRemoved;

            //Initialize data in the resource viewmodel collection
            foreach (Resource r in WardNode.ResourceCollection)
            {
                switch (r.Type)
                {
                    case "EWS":
                        if((r as EWS).PatientId == Patient.Id) Resources.Add(new UpdatableEWSViewModel((EWS)r, WardNode));
                        break;
                    case "Note":
                        if ((r as Note).PatientId == Patient.Id) Resources.Add(new UpdatableNoteViewModel((Note)r, WardNode));
                        break;
                    default:
                        //TODO: Don't know if related to this patient
                        //Resources.Add(new ResourceViewModel(r, WardNode));
                        break;
                }
            }

            // Check if patient is a part of a user visit activity. If so, and if visit isn't done set ShowVisitDone to true.
            // If patient is not a part of a user visit activity set ShowVisitDone to false. 
            List<RoundActivity> roundActivities = new List<RoundActivity>();
            WardNode.ActivityCollection.
                   Where(a => a.Type.Equals(typeof(RoundActivity).Name) && (a as RoundActivity).ClinicianId.Equals(AuthenticationHelper.User.Id)).ToList().ForEach(r => roundActivities.Add(r as RoundActivity));
            
            foreach (RoundActivity rA in roundActivities)
            {
                foreach (VisitActivity vA in rA.Visits)
                {
                    if (vA.PatientId.Equals(Patient.Id))
                    {
                        VisitActivity = vA;
                        RoundActivity = rA;
                    }
                }
            }

            if (VisitActivity != null)
            {
                if (VisitActivity.IsDone)
                {
                    ShowVisitDone = false;
                    ShowVisitDoneCheckMark = true;
                }
                else
                {
                    ShowVisitDone = true;
                    ShowVisitDoneCheckMark = false;
                }
            }
            else
            {
                ShowVisitDone = false;
                ShowVisitDoneCheckMark = false;
            }
        }

        #region Properties
        public WardNode WardNode { get; set; }
        public ObservableCollection<ResourceViewModelBase> Resources { get; set; }
        public string NameAndCpr
        {
            get { return Patient.Name + ": " + Patient.Cpr; }
        }
        public int EWS
        {
            get { return 1;  }
        }
        public string Info
        {
            get { return "F"; }
        }
        public bool ShowVisitDone { get; set; }

        public bool ShowVisitDoneCheckMark { get; set; }
        public ICommand VisitDoneCommand
        {
            get
            {
                return _visitDoneCommand ?? (_visitDoneCommand = new RelayCommand(
                    param => SetVisitDone(),
                    param => true
                    ));
            }
        }
        public VisitActivity VisitActivity
        {
            get { return _visitActivity; }
            set
            {
                _visitActivity = value;
                OnPropertyChanged("VisitActivity");
            }
        }
        public RoundActivity RoundActivity
        {
            get { return _roundActivity; }
            set
            {
                _roundActivity = value;
                OnPropertyChanged("RoundActivity");
            }
        }
        #endregion
        #region WardNode Hooks
        void WardNode_ResourceAdded(object sender, NooSphere.Model.Resources.Resource resource)
        {
            switch (resource.Type)
            {
                case "EWS":
                    if((resource as EWS).PatientId == Patient.Id) Resources.Add(new UpdatableEWSViewModel((EWS)resource, WardNode));
                    break;
                case "Note":
                    if ((resource as Note).PatientId == Patient.Id) Resources.Add(new UpdatableNoteViewModel((Note)resource, WardNode));
                    break;
                default:
                    //TODO: No idea if this is related to the row's patient
                    //Resources.Add(new ResourceViewModel((Resource)resource, WardNode));
                    break;
            }
        }
        void WardNode_ResourceChanged(object sender, NooSphere.Model.Resources.Resource resource)
        {
            var index = -1;
            //Find patient
            var a = Resources.FirstOrDefault(t => t.Id == resource.Id);
            if (a == null)
                return;

            index = Resources.IndexOf(a);

            if (index == -1)
                return;

            switch(resource.Type){
                case "EWS":
                    UpdatableEWSViewModel evm = new UpdatableEWSViewModel(resource as EWS, WardNode);
                    evm.ResourceUpdated += ResourceUpdated;
                    Resources[index] = evm;
                    break;
                case "Note":
                    UpdatableNoteViewModel nvm = new UpdatableNoteViewModel(resource as Note, WardNode);
                    nvm.ResourceUpdated += ResourceUpdated;
                    Resources[index] = nvm;
                    break;
            }
        }
        void WardNode_ResourceRemoved(object sender, NooSphere.Model.Resources.Resource resource)
        {
            foreach (var a in Resources.ToList())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (a.Id == resource.Id)
                        Resources.Remove(a);
                });
            }
        }
        #endregion

        void Resources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (var item in list)
                {
                    var resource = item as ResourceViewModelBase;
                    if (resource == null) return;
                    switch (resource.Resource.Type)
                    {
                        case "EWS":
                            (resource as UpdatableEWSViewModel).ResourceUpdated += ResourceUpdated;
                            break;
                        case "Note":
                            (resource as UpdatableNoteViewModel).ResourceUpdated += ResourceUpdated;
                            break;
                    }
                }
            }
        }
        void ResourceUpdated(object sender, EventArgs e)
        {
            WardNode.UpdateResource((Resource)sender);
        }

        private void SetVisitDone()
        {
            if (VisitActivity != null)
            {
                VisitActivity.IsDone = true;
                RoundActivity.Visits.RemoveAll(v => v.Id.Equals(VisitActivity.Id));
                RoundActivity.Visits.Add(VisitActivity);
                WardNode.UpdateActivity(RoundActivity);
            }
        }
    }
}
