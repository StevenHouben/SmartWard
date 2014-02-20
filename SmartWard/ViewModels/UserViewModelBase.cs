using NooSphere.Model;
using NooSphere.Model.Primitives;
using NooSphere.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.ViewModels
{
    public abstract class UserViewModelBase : NooViewModelBase
    {
        public UserViewModelBase(User user) : base(user) { }

        public User User { get { return Noo as User; } }

        #region User Properties
        public string Tag
        {
            get { return User.Tag; }
            set
            {
                User.Tag = value;
                OnPropertyChanged("tag");
            }
        }

        public string Image
        {
            get { return User.Image; }
            set
            {
                User.Image = value;
                OnPropertyChanged("image");
            }
        }

        public string Email
        {
            get { return User.Email; }
            set
            {
                User.Email = value;
                OnPropertyChanged("email");
            }
        }

        public Rgb Color
        {
            get { return User.Color; }
            set
            {
                User.Color = value;
                OnPropertyChanged("color");
            }
        }

        public Dictionary<string, string> Descriptors
        {
            get { return User.Descriptors; }
            set
            {
                User.Descriptors = value;
                OnPropertyChanged("descriptors");
            }
        }


        public bool Selected
        {
            get { return User.Selected; }
            set
            {
                User.Selected = value;
                OnPropertyChanged("selected");
            }
        }

        public int State
        {
            get { return User.State; }
            set
            {
                User.State = value;
                OnPropertyChanged("state");
            }
        }

        public string Cid
        {
            get { return User.Cid; }
            set
            {
                User.Cid = value;
                OnPropertyChanged("cid");
            }
        }

        public string Location
        {
            get { return User.Location; }
            set
            {
                User.Location = value;
                OnPropertyChanged("location");
            }
        }

        public List<Activity> Activities
        {
            get { return User.Activities; }
            set
            {
                User.Activities = value;
                OnPropertyChanged("activities");
            }
        }
        #endregion
    }
}
