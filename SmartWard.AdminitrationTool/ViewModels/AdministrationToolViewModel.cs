using NooSphere.Infrastructure.Helpers;
using SmartWard.Infrastructure;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.AdministrationTool.ViewModels
{
    class AdministrationToolViewModel : ViewModelBase
    {
        public WardNode WardNode { get; set; }
        public AdministrationToolViewModel()
        {
            WardNode = WardNode.StartWardNodeAsSystem(WebConfiguration.DefaultWebConfiguration);
        }
    }
}
