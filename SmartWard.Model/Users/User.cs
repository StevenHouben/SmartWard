/****************************************************************************
 (c) 2012 Steven Houben(shou@itu.dk) and Søren Nielsen(snielsen@itu.dk)

 Pervasive Interaction Technology Laboratory (pIT lab)
 IT University of Copenhagen

 This library is free software; you can redistribute it and/or 
 modify it under the terms of the GNU GENERAL PUBLIC LICENSE V3 or later, 
 as published by the Free Software Foundation. Check 
 http://www.gnu.org/licenses/gpl.html for details.
****************************************************************************/

using SmartWard.Model;
using SmartWard.Primitives;
using SmartWard.Users;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SmartWard.Users
{
    public class User : Noo,IUser
    {
        #region Properties

        private string tag;
        public string Tag
        {
            get { return this.tag; }
            set
            {
                this.tag = value;
                OnPropertyChanged("tag");
            }
        }

        private string image;
        public string Image
        {
            get { return this.image; }
            set
            {
                this.image = value;
                OnPropertyChanged("image");
            }
        }

        private string email;
        public string Email
        {
            get { return this.email; }
            set
            {
                this.email = value;
                OnPropertyChanged("email");
            }
        }

        private RGB color;
        public RGB Color
        {
            get { return this.color; }
            set
            {
                this.color = value;
                OnPropertyChanged("color");
            }
        }

        private Role role;
        public Role Role
        {
            get { return this.role; }
            set
            {
                this.role = value;
                OnPropertyChanged("role");
            }
        }

        private bool selected;
        public bool Selected
        {
            get { return this.selected; }
            set
            {
                this.selected = value;
                OnPropertyChanged("selected");
            }
        }

        private int state;
        public int State
        {
            get { return this.state; }
            set
            {
                this.state = value;
                OnPropertyChanged("state");
            }
        }

        private string cid;
        public string Cid
        {
            get { return this.cid; }
            set
            {
                this.cid = value;
                OnPropertyChanged("cid");
            }
        }

        private Dictionary<string, Activity> activities;
        public Dictionary<string, Activity> Activities
        {
            get { return this.activities; }
            set 
            {
                this.activities = value;
                OnPropertyChanged("activities");
            }
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return Name;
        }
        #endregion

        #region Methods
        public void UpdateAllProperties<T>(object newUser)
        {
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                if (propertyInfo.CanRead)
                    propertyInfo.SetValue(this,propertyInfo.GetValue(newUser, null));
        }
        #endregion

    }
}
