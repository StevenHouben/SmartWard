using NooSphere.Model.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.ViewModels
{
    public abstract class NooViewModelBase : ViewModelBase
    {
        private readonly Noo _noo;
        public NooViewModelBase(Noo noo)
        {
            _noo = noo;
        }

        public Noo Noo { get { return _noo; } }

        #region Noo Properties
        public string Id
        {
            get { return Noo.Id; }
            set
            {
                Noo.Id = value;
                OnPropertyChanged("id");
            }
        }

        public string Name
        {
            get { return Noo.Name; }
            set
            {
                Noo.Name = value;
                OnPropertyChanged("name");
            }
        }

        public string Description
        {
            get { return Noo.Description; }
            set
            {
                Noo.Description = value;
                OnPropertyChanged("description");
            }
        }

        public string Uri
        {
            get { return Noo.Uri; }
            set
            {
                Noo.Uri = value;
                OnPropertyChanged("uri");
            }
        }
        #endregion
    }
}
