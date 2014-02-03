﻿using SmartWard.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.ViewModels
{
    public abstract class ResourceViewModelBase : NooViewModelBase
    {
        public ResourceViewModelBase(Resource resource) : base(resource) { }
        public Resource Resource { get { return Noo as Resource; } }

        #region Resource Properties
        public DateTime Created { get { return Resource.Created; } set { Resource.Created = value; OnPropertyChanged("Created"); } }
        public DateTime Updated { get { return Resource.Updated; } set { Resource.Updated = value; OnPropertyChanged("Updated"); } }
        public string UpdatedBy { get { return Resource.UpdatedBy; } set { Resource.UpdatedBy = value; OnPropertyChanged("UpdatedBy"); } }
        #endregion
    }
}
