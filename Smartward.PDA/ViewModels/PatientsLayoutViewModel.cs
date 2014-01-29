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
    public class PatientsLayoutViewModel : ViewModelBase
    {
        private readonly Patient _patient;
        
        
        #region Properties
        public WardNode WardNode { get; set; }
        public ObservableCollection<ResourceViewModel> Resources { get; set; }
        public Patient Patient
        {
            get { return _patient; }
        }
        public string Id
        {
            get { return _patient.Id; }
        }

        public string Name
        {
            get { return _patient.Name; }
        }
        public string Cpr
        {
            get { return _patient.Cpr; }
        }
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
        public int Room
        {
            get { return _patient.RoomNumber;  }
        }
        #endregion

        public event EventHandler UserUpdated;

        public PatientsLayoutViewModel(Patient patient, WardNode wardNode)
        {
            _patient = patient;
            WardNode = wardNode;
            Resources = new ObservableCollection<ResourceViewModel>();

            Resources.CollectionChanged += Resources_CollectionChanged;

            WardNode.ResourceAdded += WardNode_ResourceAdded;
            WardNode.ResourceRemoved += WardNode_ResourceRemoved;
            WardNode.ResourceChanged += WardNode_ResourceChanged;

            foreach (Resource r in WardNode.ResourceCollection.Where(r => ((Resource)r).PatientId.Equals(Patient.Id)).ToList())
            {
                switch (r.Type)
                {
                    case "EWS":
                        Resources.Add(new EWSViewModel((EWS)r, WardNode));
                        break;
                    case "Note":
                        Resources.Add(new NoteViewModel((Note)r, WardNode));
                        break;
                    default:
                        Resources.Add(new ResourceViewModel(r, WardNode));
                        break;
                }
            }
        }

        void WardNode_ResourceAdded(object sender, NooSphere.Model.Resources.Resource resource)
        {
            Resources.Add(new ResourceViewModel((Resource)resource, WardNode));
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



            Resources[index].Resource = (Resource)resource;
            Resources[index].ResourceUpdated += ResourceUpdated;
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

        void Resources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (var item in list)
                {
                    var resource = item as Resource;
                    if (resource == null) return;
                    //resource.ResourceUpdated += ResourceUpdated;
                }
            }
        }

        void ResourceUpdated(object sender, EventArgs e)
        {
            WardNode.UpdateResource((Resource)sender);
        }

        public string NameAndCprString()
        {
            return Name + ": " + Cpr;
        }

    }
}
