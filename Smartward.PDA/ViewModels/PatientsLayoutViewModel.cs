using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.Models.Resources;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmartWard.PDA.ViewModels
{
    public class PatientsLayoutViewModel : PatientViewModelBase
    {
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
    }
}
