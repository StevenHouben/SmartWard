using SmartWard.Model;
using SmartWard.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Users
{
    public interface IUser
    {

        string Id { get; set; }

        string Cid { get; set; }

        string Tag{get;set;}

        string Image{get;set;}

        string Email{get;set;}

        RGB Color{get;set;}

        Role Role{get;set;}

        bool Selected{get;set;}

        int State{get;set;}

        Dictionary<string, Activity> Activities { get; set; }

        void UpdateAllProperties<T>(object u);
    }
}
