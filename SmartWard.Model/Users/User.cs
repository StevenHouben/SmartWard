/****************************************************************************
 (c) 2012 Steven Houben(shou@itu.dk) and Søren Nielsen(snielsen@itu.dk)

 Pervasive Interaction Technology Laboratory (pIT lab)
 IT University of Copenhagen

 This library is free software; you can redistribute it and/or 
 modify it under the terms of the GNU GENERAL PUBLIC LICENSE V3 or later, 
 as published by the Free Software Foundation. Check 
 http://www.gnu.org/licenses/gpl.html for details.
****************************************************************************/

using SmartWard.Primitives;
using SmartWard.Users.Users;

namespace SmartWard.Users
{
    public class User : Noo
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

        private string id;
        public string Id
        {
            get { return this.id; }
            set
            {
                this.id = value;
                OnPropertyChanged("id");
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

        private RGB stateColor;
        public RGB StateColor
        {
            get { return this.stateColor; }
            set
            {
                this.stateColor = value;
                OnPropertyChanged("stateColor");
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
        #endregion

        public override string ToString()
        {
            return Name;
        }
    }
}
