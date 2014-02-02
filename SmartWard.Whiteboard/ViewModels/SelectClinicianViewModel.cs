using NooSphere.Model.Users;
using SmartWard.Infrastructure;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Whiteboard.ViewModels
{
    class SelectClinicianViewModel : ViewModelBase
    {
        public IList<ClinicianViewModel> AllClinicians { get; set; }
        public ObservableCollection<ClinicianViewModel> SelectedClinicians { get; set; }
    }
}
