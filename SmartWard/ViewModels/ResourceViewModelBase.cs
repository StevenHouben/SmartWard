using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.ViewModels
{
    public abstract class ResourceViewModelBase : NooViewModelBase
    {
        public ResourceViewModelBase(Resource resource, WardNode wardNode) : base(resource) 
        {
            WardNode = wardNode;
            resource.PropertyChanged += (s, e) => { OnPropertyChanged("UpdatedByName"); OnPropertyChanged("SeenByNames"); };
        }
        public Resource Resource { get { return Noo as Resource; } }

        #region Resource Properties
        public WardNode WardNode { get; set; }
        public string UpdatedByName
        {
            get
            {
                Clinician c = (Clinician)WardNode.UserCollection.Where(u => u.Id == Resource.UpdatedBy).ToList().FirstOrDefault();
                return c.Name;
            }
        }
        public string SeenByNames
        {
            get
            {
                List<string> names = new List<string>();
                Resource.SeenBy.ForEach(s => names.Add(WardNode.UserCollection.Where(u => u.Id == s).ToList().FirstOrDefault().Name));

                return String.Join(", ", names);
            }
        }

        public DateTime Created { get { return Resource.Created; } set { Resource.Created = value; OnPropertyChanged("Created"); } }
        public DateTime Updated { get { return Resource.Updated; } set { Resource.Updated = value; OnPropertyChanged("Updated"); } }
        public string UpdatedBy { get { return Resource.UpdatedBy; } set { Resource.UpdatedBy = value; OnPropertyChanged("UpdatedBy"); } }
        
        #endregion
    }
}
