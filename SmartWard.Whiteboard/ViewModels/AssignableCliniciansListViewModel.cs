using NooSphere.Model.Users;
using SmartWard.Commands;
using SmartWard.Infrastructure;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartWard.Whiteboard.ViewModels
{
    class AssignableCliniciansListViewModel : ViewModelBase
    {
        private IList<AssignableClinicianViewModel> _assignableClinicians;

        public IList<AssignableClinicianViewModel> AssignableClinicians { 
            get { return _assignableClinicians; } 
            set 
            {
                _assignableClinicians = value;
                OnPropertyChanged("AssignableClinicians");
            } 
        }

        public AssignableCliniciansListViewModel(IList<AssignableClinicianViewModel> assignableClinicians)
        {
            AssignableClinicians = assignableClinicians;
        }
    }
}
