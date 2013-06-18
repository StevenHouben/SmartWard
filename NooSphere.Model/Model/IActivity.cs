using NooSphere.Primitives;
using NooSphere.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NooSphere.Model
{
    public interface IActivity : INoo
    {
        User Owner { get; set; }
        List<User> Participants { get; set; }
        List<Action> Actions{get;set;}
        Metadata Meta { get; set; }
        List<Resource> Resources { get; set; }
    }
}
